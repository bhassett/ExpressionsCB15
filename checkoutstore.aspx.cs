using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutstore : SkinBase
    {
        #region Declaration

        IAuthenticationService _authenticationService = null;
        IStringResourceService _stringResourceService = null;
        IShoppingCartService _shoppingCartService = null;
        IRequestCachingService _requestCachingService = null;
        INavigationService _navigationService = null;
        ICustomerService _customerService = null;
        IAppConfigService _appConfigService = null;

        private const string PAYMENT_METHOD_CREDITCARD = "Credit Card";
        private bool _isFromPayPal = false;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        InterpriseShoppingCart _cart = null;

        #endregion

        #region Events

        protected override void OnPreInit(EventArgs e)
        {
            PageNoCache();
            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            RequireSecurePage();

            RegisterDomainServices();
            RegisterEvents();

            InitializeShoppingCart();
            BindShoppingCart();

            ReadPaypalValueFromQueryString();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DoPageAccessLogic();
            DisplayCheckOutStepsImage();
            DisplayOrderSummary();
            RegisterPageScript();
        }

        private void RegisterPageScript()
        {
            var script = new StringBuilder();
            script.Append("<script type='text/javascript'>\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", _stringResourceService.GetString("checkoutshipping.aspx.9", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", _stringResourceService.GetString("checkoutshipping.aspx.10", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", _stringResourceService.GetString("checkoutshipping.aspx.11", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", _stringResourceService.GetString("checkoutshipping.aspx.12", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutstore.aspx.1", _stringResourceService.GetString("checkoutstore.aspx.1", true));
            script.AppendFormat("   ise.Pages.CheckouStore.setForm('{0}');\n", this.frmcheckoutstore.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        private void BindShoppingCart()
        {
            string shippingAddressID = String.Empty;
            if (_cart.OnlyShippingAddressIsNotCustomerDefault())
            {
                var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, _cart.FirstItem().m_ShippingAddressID);
                shippingAddressID = shippingAddress.AddressID;
            }
            else
            {
                shippingAddressID = ThisCustomer.PrimaryShippingAddress.AddressID; 
            }

            //var items = _cart.CartItems.Select((c, idx) => new CustomCartItem
            //{
            //    ShippinAddressID = c.m_ShippingAddressID,
            //    ItemDescription = c.ItemDescription,
            //    Quantity = c.m_Quantity,
            //    ItemCode = c.ItemCode,
            //    UnitMeassureCode = c.UnitMeasureCode,
            //    Counter = c.m_ShoppingCartRecordID,
            //    IsService = c.IsService,
            //    IsDownload = c.IsDownload,
            //    InStoreWarehouseCode = c.InStoreWarehouseCode,
            //    ShippingMethod = c.ShippingMethod,
            //    KitComposition = c.GetKitComposition(),
            //    GroupID = idx + 1
            //});

            rptCartItems.DataSource = _cart.SplitIntoMultipleOrdersByItem(false);
            rptCartItems.DataBind();
        }

        private void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as InterpriseShoppingCart;
                var ctrlShippingMethod = e.Item.FindControl("ctrlShippingMethod") as UserControls_ShippingMethodControl;

                if ((item.HasDownloadComponents() || item.HasServiceComponents()) && ctrlShippingMethod != null)
                {
                    ctrlShippingMethod.Visible = false;
                    var litControl = e.Item.FindControl("litNoShippingMethodText") as Literal;
                    if (litControl != null) litControl.Visible = true;
                }

                var customer = _authenticationService.GetCurrentLoggedInCustomer();
                ctrlShippingMethod.ThisCustomer = customer;
                ctrlShippingMethod.ShippingAddressID = item.FirstItemShippingAddressID();
                ctrlShippingMethod.ShippingMethodRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.9", true);
                ctrlShippingMethod.InStoreNoSelectedWarehouseErrorMessage = _stringResourceService.GetString("checkoutstore.aspx.29", true) + item.CartItems[0].ItemDescription.ToHtmlEncode();
                ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
                //ctrlShippingMethod.InstoreCartItem =  item;

                var lblItemDescription = e.Item.FindByParse<Label>("lblItemDescription");
                lblItemDescription.Text = item.CartItems[0].ItemDescription;
                var lblQuantity = e.Item.FindByParse<Label>("lblQuantity");
                lblQuantity.Text = item.CartItems[0].m_Quantity.ToNumberFormat();

                CustomCartItem CustomItem = new CustomCartItem();
                CustomItem.Counter = item.CartItems[0].m_ShoppingCartRecordID;
                CustomItem.IsDownload = item.CartItems[0].IsDownload;
                CustomItem.ItemCode = item.CartItems[0].ItemCode;
                CustomItem.UnitMeassureCode = item.CartItems[0].UnitMeasureCode;
                CustomItem.InStoreWarehouseCode = item.CartItems[0].InStoreWarehouseCode;
                CustomItem.KitComposition = item.CartItems[0].GetKitComposition();
                ctrlShippingMethod.InstoreCartItem = CustomItem;
                ctrlShippingMethod.InStoreSelectedWareHouseCode = CustomItem.InStoreWarehouseCode;
                ctrlShippingMethod.ItemSpecificType = item.CartItems[0].ItemSpecificType;

                if (_cart != null && _cart.HasGiftItems())
                {
                    ctrlShippingMethod.HideInStorePickUpShippingOption = true;
                    ctrlShippingMethod.HidePickupStoreLink = true;
                }

                if (_appConfigService.ShippingRatesOnDemand)
                {
                    ctrlShippingMethod.ShowShowAllRatesButton = true;
                    ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
                }
                ctrlShippingMethod.IsMultipleShipping = true;

                var script = new StringBuilder();
                script.Append("<script>\n");
                script.Append("$add_windowLoad(\n");
                script.Append(" function() { \n");
                script.AppendFormat("   ise.Pages.CheckouStore.addShippingMethodControlId('{0}');\n", ctrlShippingMethod.ClientID);
                script.Append(" }\n");
                script.Append(");\n");
                script.Append("</script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
            }
        }

        private void btnContinueCheckOut_Click(object sender, EventArgs e)
        {
            if(!IsValid) return;

            var cartShippingDataToSplit = rptCartItems.Items
                                            .OfType<RepeaterItem>()
                                            .Where(ci => {

                                                var shippingMethodControl = ci.FindControl("ctrlShippingMethod") as UserControls_ShippingMethodControl;

                                                if (shippingMethodControl == null) return false;
                                                if (shippingMethodControl.InstoreCartItem.IsDownload || shippingMethodControl.InstoreCartItem.IsService) return false;

                                                return true;
                                                
                                            }).Select(i => {

                                                var shippingMethodControl = i.FindControl("ctrlShippingMethod") as UserControls_ShippingMethodControl;
                                                var item = new CartShippingDataToSplit()
                                                {
                                                    ShippingMethod = shippingMethodControl.ShippingMethod,
                                                    CartId = shippingMethodControl.InstoreCartItem.Counter
                                                };

                                                if (shippingMethodControl.FreightChargeType.ToUpperInvariant() == "PICK UP")
                                                {
                                                    item.WarehouseCode = shippingMethodControl.InStoreSelectedWareHouseCode;
                                                }

                                                if(shippingMethodControl.FreightCalculation == "1" || shippingMethodControl.FreightCalculation == "2")
                                                {
                                                    item.IsRealTime = true;
                                                    item.Freight = shippingMethodControl.Freight.Trim(new char[] { ' ', '$' });
                                                    item.RealTimeRateGuid = shippingMethodControl.RealTimeRateGUID;
                                                }

                                                return item;

                                            }).AsParallel();

            _shoppingCartService.SaveShippingInfoAndContinueCheckout(cartShippingDataToSplit);

        }

        #endregion

        #region Methods

        private void RegisterDomainServices()
        {
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _requestCachingService = ServiceFactory.GetInstance<IRequestCachingService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
        }

        private void DoPageAccessLogic()
        {
            _customerService.DoIsCreditOnHoldChecking();

            _customerService.DoIsOver13Checking();

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                _navigationService.NavigateToUrl("createaccount.aspx?checkout=true");
            }

            //If current user came from IS, chances are it has no Primary Billing Info! then tried to checkout
            if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToUrl("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
            }

            if (_cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(_stringResourceService.GetString("shoppingcart.aspx.1", true));
            }

            string couponCode = string.Empty;
            string couponErrorMessage = string.Empty;
            if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }
            else if (!_cart.IsNoShippingRequired() && (_cart.HasMultipleShippingAddresses()) && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.NumItems() > 1 || _cart.HasRegistryItems())
            {
                _navigationService.NavigateToCheckoutMult();
            }

            CheckWhetherToRequireShipping();
        }

        private void CheckWhetherToRequireShipping()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") ||
                !_cart.HasShippableComponents() ||
                _cart.CouponIncludesFreeShipping(litCouponEntered.Text))
            {
                _cart.MakeShippingNotRequired();

                if (!_isFromPayPal)
                {
                    _navigationService.NavigateToCheckOutPayment();
                }
                else
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                    _navigationService.NavigateToUrl("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
                }
            }
        }

        private void ReadPaypalValueFromQueryString()
        {
            _isFromPayPal = ("PayPal".ToQueryString() == bool.TrueString && "token".ToQueryString() != null);
        }

        private void InitializeShoppingCart()
        {
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);

            string couponCode = string.Empty;
            bool hasCoupon = _cart.HasCoupon(ref couponCode);

            if (hasCoupon)
            {
                panelCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                panelCoupon.Visible = false;
            }

            try
            {
                // Always compute the vat since we need to display the vat even if the the vat enabled = true
                _cart.BuildSalesOrderDetails(false, true, couponCode);
                _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping(couponCode);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == AppLogic.GetString("shoppingcart.cs.35", true))
                {
                    _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
                else { throw ex; }
            }
            catch (Exception ex) { throw ex; }
        }

        private void DisplayCheckOutStepsImage()
        {
            CheckoutStepLiteral.Text = new XSLTExtensionBase(ThisCustomer, ThisCustomer.SkinID).DisplayCheckoutSteps(2, "shoppingcart.aspx", string.Empty, string.Empty);
            //checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            //((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = _stringResourceService.GetString("checkoutshipping.aspx.3", true);
            //((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = _stringResourceService.GetString("checkoutshipping.aspx.4", true);
        }

        private void DisplayOrderSummary()
        {
            DetailsLit.Text = _stringResourceService.GetString("itempopup.aspx.2");
            EditCartLit.Text = _stringResourceService.GetString("checkout1.aspx.44");
            var renderer = new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, "page.checkout.ordersummaryitems.xml.config", litCouponEntered.Text);
            CheckoutOrderSummaryItemsLiteral.Text = _cart.RenderHTMLLiteral(renderer);
            OrderSummaryCardLiteral.Text = AppLogic.RenderOrderSummaryCard(renderer.OrderSummary);
             
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/checkoutstore_ajax.js"));
            manager.LoadScriptsBeforeUI = false;
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private void RegisterEvents()
        {
            rptCartItems.ItemDataBound += rptCartItems_ItemDataBound;
            btnContinueCheckOut.Click += btnContinueCheckOut_Click;
            //btnContinueCheckOutTop.Click += btnContinueCheckOut_Click;
        }

        #endregion
    }
}