// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using System.Collections.Generic;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutreview.
    /// </summary>
    public partial class checkoutreview : SkinBase
    {
        #region Declaration
            
        InterpriseShoppingCart cart = null;
        PayPalExpress pp;

        #endregion

        #region DomainServices

        INavigationService _navigationService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        IAppConfigService _appConfigService = null;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            RegisterDomainServices();
            InitializeComponent();

            if (AppLogic.AppConfigBool("MaxMind.Enabled"))
            {
                ctrlScript.ShowMaxMind = true;
            }

            base.OnInit(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.PageNoCache();
            this.RequireSecurePage();

            _customerService.DoIsOver13Checking();
            _customerService.DoIsCreditOnHoldChecking();

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            ThisCustomer.RequireCustomerRecord();

            if (ThisCustomer.IsNotRegistered)
            {
                if (!AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    _navigationService.NavigateToUrl("createaccount.aspx?checkout=true");
                }

                ThisCustomer.EMail = ServiceFactory.GetInstance<ICustomerService>().GetAnonEmail();
            }

            _customerService.DoRegisteredCustomerShippingAndBillingAddressChecking();

            SectionTitle = AppLogic.GetString("checkoutreview.aspx.1");
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

            if (cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("shoppingcart.aspx.1"));
            }

            string couponCode = string.Empty;
            string couponErrorMessage = string.Empty;

            bool hasCoupon = cart.HasCoupon(ref couponCode);
            if (hasCoupon && cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                panelCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                panelCoupon.Visible = false;
                if (!couponErrorMessage.IsNullOrEmptyTrimmed())
                {
                   _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
            }

            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (cart.HasRegistryItemButParentRegistryIsRemoved() || cart.HasRegistryItemsRemovedFromRegistry())
            {
                cart.RemoveRegistryItemsHasDeletedRegistry();
                cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.18"));
            }

            if (cart.HasRegistryItemsAndOneOrMoreItemsHasZeroInNeed())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.15"));
            }

            if (cart.HasRegistryItemsAndOneOrMoreItemsExceedsToTheInNeedQuantity())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.14"));
            }

            _customerService.DoShippingAddressModificationChecking();
            _shoppingCartService.DoHasAppliedInvalidGiftCodesChecking(cart);
            _shoppingCartService.DoHasAppliedInvalidLoyaltyPointsChecking(cart);
            _shoppingCartService.DoHasAppliedInvalidCreditMemosChecking(cart);

            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        private void InitializeComponent()
        {
            this.btnContinueCheckout1.Click += btnContinueCheckout1_Click;
        }

        void btnContinueCheckout1_Click(object sender, EventArgs e)
        {
            ProcessCheckout();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (cart != null)
            {
                cart.Dispose();
            }
            base.OnUnload(e);
        }

        #endregion

        #region Methods

        private void RegisterDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
        }

        private bool IsPayPalCheckout
        {
            get
            {
                return (Request.QueryString["PayPal"] ?? bool.FalseString) == bool.TrueString && Request.QueryString["token"] != null;
            }
        }

        private void InitializePageContent()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_5.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                RectangleHotSpot rhs = (RectangleHotSpot)checkoutheadergraphic.HotSpots[i];
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.2");
                if (rhs.NavigateUrl.IndexOf("account") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.3");
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.4");
                if (rhs.NavigateUrl.IndexOf("checkoutpayment") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.5");
            }
            if (!AppLogic.AppConfigBool("SkipShippingOnCheckout"))
            {
                checkoutheadergraphic.HotSpots[2].HotSpotMode = HotSpotMode.Navigate;
                if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = "checkout1.aspx";
                }
                else
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = CommonLogic.IIF(cart.HasMultipleShippingAddresses(), "checkoutshippingmult.aspx", "checkoutshipping.aspx");
                }
            }
            if (_appConfigService.CheckoutUseOnePageCheckout)
            {
                checkoutheadergraphic.HotSpots[3].NavigateUrl = "checkout1.aspx";
            }

            if (IsPayPalCheckout)
            {
                checkoutheadergraphic.HotSpots[1].HotSpotMode = HotSpotMode.Inactive;
                checkoutheadergraphic.HotSpots[2].NavigateUrl += string.Format("?PayPal={0}&token={1}", bool.TrueString, Request.QueryString["token"]);
                checkoutheadergraphic.HotSpots[3].HotSpotMode = HotSpotMode.Inactive;
            }

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutReviewPageHeader.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            decimal gCardAllocate = Decimal.Zero;
            decimal gCertAllocate = Decimal.Zero;
            decimal loyaltyPointsAllocate = Decimal.Zero;
            decimal creditMemosAllocate = Decimal.Zero;

            if (cart.HasMultipleShippingAddresses() || cart.HasRegistryItems())
            {
                
                var splittedCarts = cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
                foreach (var splitCart in splittedCarts)
                {
                    splitCart.BuildSalesOrderDetails(true, litCouponEntered.Text);
                    OrderSummary.Text += splitCart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, String.Empty, gCardAllocate, gCertAllocate, loyaltyPointsAllocate, creditMemosAllocate, litCouponEntered.Text));
                    gCardAllocate += splitCart.GiftCardsTotalCreditAllocated;
                    gCertAllocate += splitCart.GiftCertsTotalCreditAllocated;
                    loyaltyPointsAllocate += splitCart.LoyaltyPointsCreditAllocated;
                    creditMemosAllocate += splitCart.CreditMemosCreditAllocated;
                }
            }
            else if (cart.HasMultipleShippingMethod())
            {
                var ordersWithDifferentShipping = _shoppingCartService.SplitShippingMethodsInMultipleOrders();
                foreach (var order in ordersWithDifferentShipping)
                {
                    order.BuildSalesOrderDetails(true, litCouponEntered.Text);
                    OrderSummary.Text += order.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, String.Empty, gCardAllocate, gCertAllocate, loyaltyPointsAllocate, creditMemosAllocate, litCouponEntered.Text));
                    gCardAllocate += order.GiftCardsTotalCreditAllocated;
                    gCertAllocate += order.GiftCertsTotalCreditAllocated;
                    loyaltyPointsAllocate += order.LoyaltyPointsCreditAllocated;
                    creditMemosAllocate += order.CreditMemosCreditAllocated;
                }
            }   
            else
            {
                //If the shopping cart contains only Electronic Downloads or Services then pass a "false" parameter for computeFreight.
                if (cart.IsNoShippingRequired())
                {
                    cart.BuildSalesOrderDetails(false, true, litCouponEntered.Text);
                }
                else
                {
                    cart.BuildSalesOrderDetails(true, litCouponEntered.Text);
                }

                string couponCode = String.Empty;
                if (!ThisCustomer.CouponCode.IsNullOrEmptyTrimmed()) couponCode = ThisCustomer.CouponCode;

                OrderSummary.Text = cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, "page.checkoutshippingordersummary.xml.config", couponCode));

            }

            // Show only the "Edit Address" link for registered customer and if appconfig: ShowEditAddressLinkOnCheckOutReview = true
            if (ThisCustomer.IsRegistered && _appConfigService.ShowEditAddressLinkOnCheckOutReview)
            {
                string imageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/redarrow.gif");
                pnlEditBillingAddress.Visible = true;
                imgBillingRedArrow.ImageUrl = imageUrl;

                if (_appConfigService.AllowShipToDifferentThanBillTo)
                {
                    pnlEditShippingAddress.Visible = true;
                    imgShippingRedArrow.ImageUrl = imageUrl;
                }
                else
                {
                    ordercs57.Visible = false;
                }
            }

            litBillingAddress.Text = ThisCustomer.PrimaryBillingAddress.DisplayString(true, true, true, "<br/>");

            if (IsPayPalCheckout)
                litPaymentMethod.Text = "PayPal Express Checkout";
            else
                litPaymentMethod.Text = GetPaymentMethod(ThisCustomer.PrimaryBillingAddress);

            if (cart.HasPickupItem())
            {
                pnlShippingPickMessage.Visible = true;
            }

            if (cart.HasMultipleShippingAddresses() || (cart.HasRegistryItems() && cart.CartItems.Count > 1))
            {
                litShippingAddress.Text = "<br/>Multiple Ship Addresses";
            }
            else
            {
                Address shippingAddress = null;

                //added for PayPal ADDRESSOVERRIDE  
                if (IsPayPalCheckout && !AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                {
                    if (!cart.HasShippableComponents())
                    {
                        shippingAddress = ThisCustomer.PrimaryShippingAddress;
                    }
                    else
                    {
                        pp = new PayPalExpress();
                        var GetPayPalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                        shippingAddress = new Address()
                        {
                            Name = GetPayPalDetails.PayerInfo.Address.Name,
                            Address1 = GetPayPalDetails.PayerInfo.Address.Street1 + (GetPayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + GetPayPalDetails.PayerInfo.Address.Street2,
                            City = GetPayPalDetails.PayerInfo.Address.CityName,
                            State = GetPayPalDetails.PayerInfo.Address.StateOrProvince,
                            PostalCode = GetPayPalDetails.PayerInfo.Address.PostalCode,
                            Country = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.CountryName.ToString()),
                            CountryISOCode = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.Country.ToString()),
                            Phone = GetPayPalDetails.PayerInfo.ContactPhone ?? ThisCustomer.PrimaryShippingAddress.Phone
                        };
                    }
                }
                else
                {
                    if (cart.OnlyShippingAddressIsNotCustomerDefault())
                    {
                        var item = cart.FirstItem();
                        shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, item.m_ShippingAddressID, item.GiftRegistryID);
                    }
                    else
                    {
                        shippingAddress = ThisCustomer.PrimaryShippingAddress;
                    }
                }

                if (_appConfigService.AllowShipToDifferentThanBillTo)
                {
                    litShippingAddress.Text = shippingAddress.DisplayString(true, true, true, "<br/>");
                }
                else
                {
                    ordercs57.Visible = false;
                }

            }

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_CheckoutReviewPageFooter.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            AppLogic.GetButtonDisable(btnContinueCheckout1);
            CheckoutReviewPageHeader.SetContext = this;
            CheckoutReviewPageFooter.SetContext = this;
            ThisCustomer.ThisCustomerSession.ClearVal(DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS);
        }

        private string GetPaymentMethod(Address BillingAddress)
        {
            var sPmtMethod = new StringBuilder();
            var paymentInfo = PaymentTermDTO.Find(ThisCustomer.PaymentTermCode);

            //  We should have a default payment method
            //  For debugging purposes, have this check here
            if (string.IsNullOrEmpty(paymentInfo.PaymentMethod))
            {
                throw new InvalidOperationException("No payment method defined!");
            }

            if (!cart.IsSalesOrderDetailBuilt)
            {
                if (cart.IsNoShippingRequired())
                {
                    cart.BuildSalesOrderDetails(false, true, litCouponEntered.Text, true);
                }
                else
                {
                    cart.BuildSalesOrderDetails(true, litCouponEntered.Text);
                }
            }

            if ((ThisCustomer.PaymentTermCode.Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase)) ||
                (ThisCustomer.PaymentTermCode.Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase)))
            {
                sPmtMethod.Append(ThisCustomer.PaymentTermCode.ToUpperInvariant());
            }
            else
            {
                switch (paymentInfo.PaymentMethod)
                {
                    case DomainConstants.PAYMENT_METHOD_CREDITCARD:

                        if ((cart.GetOrderTotal() == decimal.Zero) && (AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout")))
                        {
                            sPmtMethod.Append(AppLogic.GetString("checkoutpayment.aspx.8"));
                        }
                        else
                        {
                            sPmtMethod.AppendFormat("{0} ({1})", Security.HtmlEncode(paymentInfo.PaymentMethod), HttpUtility.HtmlEncode(ThisCustomer.PaymentTermCode));
                            sPmtMethod.Append("<br/>");
                            sPmtMethod.Append("<table class=\"payment-method\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.10"));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardName);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.11"));
                            sPmtMethod.Append("</td><td>");
                            DataView dt = AppLogic.GetCustomerCreditCardType("");
                            string cardTypeDescription = string.Empty;
                            try
                            {
                                cardTypeDescription = dt.Table.Select(string.Format("CreditCardType = '{0}'", BillingAddress.CardType))[0]["CreditCardTypeDescription"].ToString();
                            }
                            catch
                            {
                                cardTypeDescription = BillingAddress.CardType;
                            }
                            sPmtMethod.Append(cardTypeDescription);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.12"));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardNumberMaskSafeDisplayFormat);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.13"));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardExpirationMonth.PadLeft(2, '0') + "/" + BillingAddress.CardExpirationYear);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("</table>");
                        }
                        break;
                    case DomainConstants.PAYMENT_METHOD_CASH:
                    case DomainConstants.PAYMENT_METHOD_CHECK:
                    case DomainConstants.PAYMENT_METHOD_WEBCHECKOUT:

                        if ((cart.GetOrderTotal() == Decimal.Zero) && (AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout")))
                        {
                            sPmtMethod.Append(AppLogic.GetString("checkoutpayment.aspx.8"));
                        }
                        else
                        {
                            sPmtMethod.AppendFormat("{0} ({1})", paymentInfo.PaymentMethod.ToHtmlEncode(), ThisCustomer.PaymentTermCode.ToHtmlEncode());
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Payment method!");
                }
            }

            return sPmtMethod.ToString();
        }

        private void ProcessCheckout()
        {
            if (!cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(cart.HasNoStockPhasedOutItem, cart.HaNoStockAndNoOpenPOItem);
                //check discountinued

                var discontinuedItems = cart.CartItems.Where(c => c.Status.ToLowerInvariant() == "D".ToLowerInvariant())
                                            .Select(itm => itm.m_ShoppingCartRecordID)
                                            .AsParallel().ToList();
                if (discontinuedItems.Count > 0){
                    discontinuedItems.ForEach(recId => { cart.RemoveItem(recId); });
                    _navigationService.NavigateToShoppingCartWitErroMessage(AppLogic.GetString("checkoutpayment.aspx.cs.4").ToUrlEncode());
                }
            }

            string OrderNumber = String.Empty;

            // ----------------------------------------------------------------
            // Process The Order:
            // ----------------------------------------------------------------
            if (ThisCustomer.PaymentTermCode.IsNullOrEmptyTrimmed())
            {
                Response.Redirect("checkoutpayment.aspx?errormsg=" + AppLogic.GetString("checkoutpayment.aspx.7").ToUrlEncode());
            }
            else
            {
                // validate gift codes, loyalty points, and credit memos applied
                DoOtherPaymentsChecking();

                string receiptCode = string.Empty;
                string status = string.Empty, multiorder = string.Empty;

                bool hasMultipleShippingMethod = cart.HasMultipleShippingMethod();
                bool hasMultipleShippingAddress = cart.HasMultipleShippingAddresses();
                if (hasMultipleShippingAddress || cart.HasRegistryItems() || hasMultipleShippingMethod)	// Paypal will never hit this
                {
                    List<InterpriseShoppingCart> splittedCarts = null;
                    bool gatewayAuthFailed = false;

                    if (hasMultipleShippingAddress || cart.HasRegistryItems())
                    {
                        splittedCarts = cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();                        

                    }
                    else if (hasMultipleShippingMethod)
                    {
                        splittedCarts = _shoppingCartService.SplitShippingMethodsInMultipleOrders().ToList();
                    }

                    for (int ctr = 0; ctr < splittedCarts.Count; ctr++)
                    {
                        var splitCart = splittedCarts[ctr];
                        try
                        {   
                            splitCart.BuildSalesOrderDetails(litCouponEntered.Text);
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (ex.Message == AppLogic.GetString("shoppingcart.cs.35"))
                            {
                                Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                            }
                            else { throw ex; }
                        }
                        catch (Exception ex) { throw ex; }

                        var currentItem = splitCart.FirstItem();

                        var shippingAddress = !currentItem.GiftRegistryID.HasValue? Address.Get(ThisCustomer, AddressTypes.Shipping, currentItem.m_ShippingAddressID) : ThisCustomer.PrimaryShippingAddress;
                        
                        splitCart.ClearAlreadyAppliedOtherPayment = (ctr == splittedCarts.Count - 1); // clear other payment if last cart to be placed order

                        string processedSalesOrderCode = String.Empty;
                        string processedReceiptCode = String.Empty;
                        // NOTE:
                        //  3DSecure using Sagepay Gateway is not supported on multiple shipping orders
                        //  We will revert to the regular IS gateway defined on the WebStore
                        status = splitCart.PlaceOrder(null,
                                    ThisCustomer.PrimaryBillingAddress,
                                    shippingAddress,
                                    ref processedSalesOrderCode,
                                    ref processedReceiptCode,
                                    false,
                                    true,
                                    false);

                        OrderNumber = processedSalesOrderCode;
                        receiptCode = processedReceiptCode;

                        if (status == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
                        {
                            gatewayAuthFailed = true;

                            if (ctr == 0)
                            {
                                ThisCustomer.IncrementFailedTransactionCount();
                                if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                                {
                                    cart.ClearTransaction();
                                    ThisCustomer.ResetFailedTransactionCount();

                                    _navigationService.NavigateToOrderFailed();
                                }

                                ThisCustomer.ClearTransactions(false);

                                if (cart.HasRegistryItems())
                                {
                                    _navigationService.NavigateToCheckOutPayment();
                                }

                                if (cart.HasOverSizedItemWithPickupShippingMethod() || cart.HasPickupItem())
                                {
                                    _navigationService.NavigateToCheckOutPayment();
                                }

                                if (_appConfigService.CheckoutUseOnePageCheckout)
                                {
                                    Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                                }
                                else
                                {
                                    Response.Redirect("checkoutpayment.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                                }
                            }
                        }

                        // NOTE :
                        //  Should handle cases when 1 or more orders failed the payment processor 
                        //  if using a payment gateway on credit card
                        multiorder = multiorder + "," + OrderNumber;

                        if (!gatewayAuthFailed)
                        {
                            if (splitCart.HasRegistryItems())
                            {
                                DoRegistryQuantityDeduction(splitCart.CartItems);
                            }
                        }

                    }

                    if (multiorder != string.Empty) OrderNumber = multiorder.Remove(0, 1);

                    if (!gatewayAuthFailed)
                    {
                        cart.ClearTransaction();
                    }
                }
                else
                {
                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    Address shippingAddress = null;

                    //added for PayPal ADDRESSOVERRIDE  
                    if (IsPayPalCheckout && !AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                    {
                        if (!cart.HasShippableComponents())
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                        else
                        {
                            pp = new PayPalExpress();
                            var GetPayPalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                            shippingAddress = new Address()
                            {
                                Name = GetPayPalDetails.PayerInfo.Address.Name,
                                Address1 = GetPayPalDetails.PayerInfo.Address.Street1 + (GetPayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + GetPayPalDetails.PayerInfo.Address.Street2,
                                City = GetPayPalDetails.PayerInfo.Address.CityName,
                                State = GetPayPalDetails.PayerInfo.Address.StateOrProvince,
                                PostalCode = GetPayPalDetails.PayerInfo.Address.PostalCode,
                                Country = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.CountryName.ToString()),
                                CountryISOCode = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.Country.ToString()),
                                Phone = GetPayPalDetails.PayerInfo.ContactPhone ?? ThisCustomer.PrimaryShippingAddress.Phone
                            };
                        }
                    }
                    else
                    {
                        // Handle the scenario wherein the items in the cart
                        // does not ship to the customer's primary shipping address
                        if (cart.OnlyShippingAddressIsNotCustomerDefault())
                        {
                            var item = cart.FirstItem();
                            shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, item.m_ShippingAddressID, item.GiftRegistryID);
                        }
                        else
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                    }

                    if (!cart.IsSalesOrderDetailBuilt)
                    {
                        cart.BuildSalesOrderDetails(litCouponEntered.Text);
                    }

                    Gateway gatewayToUse = null;

                    try
                    {
                        if (IsPayPalCheckout)
                        {
                            //Insert PayPal call here for response - For authorize and capture of order from paypal inside IS
                            pp = new PayPalExpress();

                            var paypalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                            var doExpressCheckoutResp = pp.DoExpressCheckoutPayment(paypalDetails.Token, paypalDetails.PayerInfo.PayerID, OrderNumber, cart);
                            string result = String.Empty;
                            if (doExpressCheckoutResp.Errors != null && !doExpressCheckoutResp.Errors[0].ErrorCode.IsNullOrEmptyTrimmed())
                            {
                                if (AppLogic.AppConfigBool("ShowGatewayError"))
                                {
                                    result = String.Format(AppLogic.GetString("shoppingcart.aspx.27"), doExpressCheckoutResp.Errors[0].ErrorCode, doExpressCheckoutResp.Errors[0].LongMessage);
                                }
                                else
                                {
                                    result = AppLogic.GetString("shoppingcart.aspx.28");
                                }

                                _navigationService.NavigateToShoppingCartWitErroMessage(result.ToUrlEncode());
                                return;
                            }
                            else
                            {
                                var payPalResp = new GatewayResponse(String.Empty)
                                {
                                    AuthorizationCode = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                                    TransactionResponse = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                                    Details = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                                    AuthorizationTransID = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID
                                };

                                status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, payPalResp, IsPayPalCheckout, false);

                                ThisCustomer.ThisCustomerSession["paypalFrom"] = String.Empty;
                                ThisCustomer.ThisCustomerSession["notesFromPayPal"] = String.Empty;
                                ThisCustomer.ThisCustomerSession["anonymousCustomerNote"] = String.Empty;
                            }
                        }
                        else
                        {
                            status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, null, !IsPayPalCheckout, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        ThisCustomer.ThisCustomerSession["paypalFrom"] = String.Empty;
                        ThisCustomer.ThisCustomerSession["notesFromPayPal"] = String.Empty;
                        ThisCustomer.ThisCustomerSession["anonymousCustomerNote"] = String.Empty;

                        if (ex.Message == "Unable to instantiate Default Credit Card Gateway")
                        {
                            cart.ClearLineItems();
                            Response.Redirect("pageError.aspx?Parameter=" + "An Error Occured while Authorizing your Credit Card, However your order has been Placed.");
                        }

                        Response.Redirect("pageError.aspx?Parameter=" + Server.UrlEncode(ex.Message));
                    }

                    if (status == AppLogic.ro_3DSecure)
                    { 
                        _navigationService.NavigateToSecureForm();
                    }
                    if (status != AppLogic.ro_OK)
                    {
                        ThisCustomer.IncrementFailedTransactionCount();
                        if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                        {
                            cart.ClearTransaction();
                            ThisCustomer.ResetFailedTransactionCount();

                            _navigationService.NavigateToOrderFailed();
                        }

                        ThisCustomer.ClearTransactions(false);

                        if (cart.HasRegistryItems())
                        {
                            _navigationService.NavigateToCheckOutPayment();
                        }

                        if (cart.HasOverSizedItemWithPickupShippingMethod() || cart.HasPickupItem())
                        {
                            _navigationService.NavigateToCheckOutPayment();
                        }

                        if (_appConfigService.CheckoutUseOnePageCheckout)
                        {
                            Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                        }
                        else
                        {
                            Response.Redirect("checkoutpayment.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                        }
                    }

                }
            }

            AppLogic.ClearCardNumberInSession(ThisCustomer);
            ThisCustomer.ClearTransactions(true);
            
            string PM = AppLogic.CleanPaymentMethod(ThisCustomer.PaymentMethod);
            bool multipleAttachment = false;
            if (OrderNumber.IndexOf(',') != -1)
            {
                multipleAttachment = true;
            }

            foreach (string salesOrderToEmail in OrderNumber.Split(','))
            {
                if (ThisCustomer.PaymentTermCode.ToUpper() != "REQUEST QUOTE" && ThisCustomer.PaymentTermCode.ToUpper() != "PURCHASE ORDER")
                {
                    AppLogic.SendOrderEMail(ThisCustomer, cart, salesOrderToEmail, false, PM, true, multipleAttachment);
                }
                else
                {
                    AppLogic.SendOrderEMail(ThisCustomer, cart, salesOrderToEmail, false, PM, multipleAttachment);
                }
            }

            Response.Redirect("orderconfirmation.aspx?ordernumber={0}".FormatWith(OrderNumber.ToUrlEncode()));
        }

        private void DoRegistryQuantityDeduction(CartItemCollection cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                if (!cartItem.GiftRegistryID.HasValue) continue;

                decimal quatityToremove = cartItem.m_Quantity;

                //to avoid negative value upon deduction
                decimal? trueQuantity = cartItem.RegistryItemQuantity;
                if (trueQuantity < cartItem.m_Quantity) { quatityToremove = trueQuantity.Value; }

                GiftRegistryDA.DeductGiftRegistryItemQuantity(cartItem.GiftRegistryID.Value, cartItem.RegistryItemCode.Value, quatityToremove, cartItem.m_Quantity);
            }
        }

        private void DoOtherPaymentsChecking()
        {
            _shoppingCartService.DoHasAppliedInvalidGiftCodesChecking(cart);
            _shoppingCartService.DoHasAppliedInvalidLoyaltyPointsChecking(cart);
            _shoppingCartService.DoHasAppliedInvalidCreditMemosChecking(cart);

            DoGiftCodeOwnershipChecking();
        }

        private void DoGiftCodeOwnershipChecking()
        {
            var giftCodes = _shoppingCartService.GetAppliedGiftCodes(false)
                                                .ToList();
            if (giftCodes.Count == 0) return;

            var customer = ServiceFactory.GetInstance<IAuthenticationService>()
                                         .GetCurrentLoggedInCustomer();

            bool hasInvalidGiftCode = giftCodes.Where(x => x.IsActivated && !x.BillToCode.IsNullOrEmptyTrimmed())
                                               .Any(x => x.BillToCode != customer.CustomerCode);
            if(hasInvalidGiftCode)
            {
                _shoppingCartService.ClearAppliedGiftCodes();
                string message = AppLogic.GetString("checkoutreview.aspx.17");
                if (_appConfigService.CheckoutUseOnePageCheckout)
                {
                    _navigationService.NavigateToCheckout1WithErrorMessage(message);
                }
                else
                {
                    _navigationService.NavigateToCheckOutPaymentWithErrorMessage(message);
                }
            }
        }

        #endregion
    }
}