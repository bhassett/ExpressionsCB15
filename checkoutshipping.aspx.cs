// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System.Linq;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutshipping : SkinBase
    {
        #region Variables

        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        public const string PAYMENT_METHOD_CREDITCARD = "Credit Card";
        bool _isFromPayPal = false;

        #endregion

        #region DomainServices

        INavigationService _navigationService = null;
        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        IShoppingCartService _shoppingCartService = null;
        IAppConfigService _appConfigService = null;

        #endregion

        #region Methods

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
        }

        private void RegisterPageScript()
        {
            if (_cartHasCouponAndIncludesFreeShipping) return;

            var script = new StringBuilder();
            script.Append("<script type='text/javascript'>\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", _stringResourceService.GetString("checkoutshipping.aspx.9", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", _stringResourceService.GetString("checkoutshipping.aspx.10", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", _stringResourceService.GetString("checkoutshipping.aspx.11", true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", _stringResourceService.GetString("checkoutshipping.aspx.12", true));

            script.AppendFormat("   ise.Pages.CheckOutShipping.setShippingMethodControlId('{0}');\n", this.ctrlShippingMethod.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShipping.setForm('{0}');\n", this.frmCheckOutShipping.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/checkoutshipping_ajax.js"));
            manager.LoadScriptsBeforeUI = false;
            
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private void InitializeShoppingCart()
        {
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);

            string couponCode = String.Empty;
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
                _cart.BuildSalesOrderDetails(false, true, couponCode,true);
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

        private void PerformPageAccessLogic()
        {
            _customerService.DoIsNotRegisteredChecking();
            _customerService.DoIsCreditOnHoldChecking();
            _customerService.DoIsOver13Checking();

            //If current user came from IS, chances are it has no Primary Billing Info! then tried to checkout
            if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToUrl("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
            }

            SectionTitle = _stringResourceService.GetString("checkoutshipping.aspx.1", true);

            if (_cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(_stringResourceService.GetString("shoppingcart.aspx.1", true));
            }

            string couponCode = String.Empty;
            string couponErrorMessage = String.Empty;
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

            if (!_cart.IsNoShippingRequired() && (_cart.HasMultipleShippingAddresses()) && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.NumItems() > 1)
            {
                _navigationService.NavigateToCheckoutMult();
            }

            if (_cart.HasMultipleShippingMethod())
            {
                _navigationService.NavigateToCheckOutStore();
            }
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
                    _navigationService.NavigateToUrl("checkoutreview.aspx?PayPal=True&token=" + "token".ToQueryString());
                }
            }
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = _stringResourceService.GetString("checkoutshipping.aspx.3", true);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = _stringResourceService.GetString("checkoutshipping.aspx.4", true);

            if (_isFromPayPal)
                checkoutheadergraphic.HotSpots[1].HotSpotMode = HotSpotMode.Inactive;
        }

        private void InitializeShippingMethodControl()
        {
            InitializeShippingMethodControlValues();
            AssignShippingMethodErrorSummary();
            AssignShippingMethodValidationPrerequisites();
            InitializeShippingMethodCaptions();
        }

        private void InitializeShippingMethodControlValues()
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

            ctrlShippingMethod.ShippingAddressID = shippingAddressID;
            ctrlShippingMethod.HideInStorePickUpShippingOption = _isFromPayPal || _cart.HasGiftItems();
            ctrlShippingMethod.HidePickupStoreLink = true;
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.9", true);

            if (_appConfigService.ShippingRatesOnDemand)
            {
                ctrlShippingMethod.ShowShowAllRatesButton = true;
                ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
            }
        }

        private void AssignShippingMethodErrorSummary()
        {
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignShippingMethodValidationPrerequisites()
        {
            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.9", true);
        }

        private void InitializeShippingMethodCaptions()
        {
            if (!_cart.CartAllowsShippingMethodSelection) return;

            if (_cartHasCouponAndIncludesFreeShipping)
            {
                lblSelectShippingMethod.Text = _stringResourceService.GetString("checkoutshipping.aspx.5");
            }
            else
            {
                if (ThisCustomer.IsRegistered && Shipping.MultiShipEnabled() && _cart.TotalQuantity() > 1 && !_isFromPayPal
                    && _appConfigService.AllowShipToDifferentThanBillTo)
                {
                    lblSelectShippingMethod.Text = String.Format(_stringResourceService.GetString("checkoutshipping.aspx.7", true), "checkoutshippingmult.aspx");
                    if (ThisCustomer.IsInEditingMode())
                    {
                        lblSelectShippingMethod.Attributes.Add("data-contentKey", "checkoutshipping.aspx.7");
                        lblSelectShippingMethod.Attributes.Add("data-contentValue", _stringResourceService.GetString("checkoutshipping.aspx.7", true));
                        lblSelectShippingMethod.Attributes.Add("data-contentType", "string resource");
                    }
                }
                else
                {
                    lblSelectShippingMethod.Text = _stringResourceService.GetString("checkout1.aspx.4");
                }
            }
        }

        private void DisplayOrderSummary()
        {
            OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, "page.checkoutshippingordersummary.xml.config", litCouponEntered.Text));
        }

        private void AssignCheckOutButtonCaption()
        {
            string contentKey = String.Empty;

            if (_cartHasCouponAndIncludesFreeShipping)
            {
                btnCompletePurchase.Text = _stringResourceService.GetString("checkoutshipping.aspx.8", true);
                contentKey = "checkoutshipping.aspx.8";
            }
            else
            {
                btnCompletePurchase.Text = _stringResourceService.GetString("checkoutshipping.aspx.6", true);
                contentKey = "checkoutshipping.aspx.6";
            }

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnCompletePurchase, contentKey);
            }
        }

        private void RegisterEvents()
        { 
            btnCompletePurchase.Click += btnCompletePurchase_Click;
        }

        /// <summary>
        /// Compute Sub total needed to avail free shipping. FreeShippingThreshold and ShippingMethodCodeIfFreeShippingIsOn appconfig MUST be setup
        /// properly for this feature to work.
        /// </summary>
        private void ShowFreeshippingInfo()
        {
            decimal threshHold = AppLogic.AppConfigUSDecimal("FreeShippingThreshold");
            string currencyCode = _cart.ThisCustomer.CurrencyCode;
            decimal subTotal = _cart.GetCartSubTotalExcludeOversized();
            string shippingMethods = AppLogic.AppConfig("ShippingMethodCodeIfFreeShippingIsOn");

            if (threshHold > Decimal.Zero && threshHold > subTotal)
            {
                pnlGetFreeShippingMsg.Visible = true;
                GetFreeShippingMsg.Text = _stringResourceService.GetString("checkoutshipping.aspx.2").FormatWith(threshHold.ToCustomerCurrency(), shippingMethods);
            }
        }

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            RegisterEvents();

            _isFromPayPal = ("PayPal".ToQueryString() == bool.TrueString && "token".ToQueryString() != null);

            ctrlShippingMethod.ThisCustomer = ThisCustomer;
            ctrlShippingMethod.RedirectOnlyWhenPickupOption = true;

            this.PageNoCache();

            RequireSecurePage();
            RequireCustomerRecord();

            InitializeShoppingCart();
            PerformPageAccessLogic();
            CheckWhetherToRequireShipping();
            DisplayCheckOutStepsImage();
            InitializeShippingMethodCaptions();
            InitializeShippingMethodControlValues();
            DisplayOrderSummary();
            ShowFreeshippingInfo();
            AssignCheckOutButtonCaption();

            RegisterPageScript();

            _customerService.ClearAddressCheckingKey();

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_cartHasCouponAndIncludesFreeShipping) return;

            var script = new StringBuilder();
            script.Append("<script type='text/javascript'>\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutShipping.setShippingMethodControlId('{0}');\n", this.ctrlShippingMethod.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShipping.setForm('{0}');\n", this.frmCheckOutShipping.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);
            }

            if (!_cartHasCouponAndIncludesFreeShipping)
            {
                //process the Pickup shipping method in checkoutstore page
                if (ctrlShippingMethod.FreightChargeType.ToUpperInvariant() == DomainConstants.PICKUP_FREIGHT_CHARGE_TYPE)
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod);
                    _navigationService.NavigateToCheckOutStore();
                }
                else if (ctrlShippingMethod.FreightCalculation == "1" || ctrlShippingMethod.FreightCalculation == "2")
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, String.Empty, ctrlShippingMethod.RealTimeRateGUID);
                    string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                    ServiceFactory.GetInstance<IShippingService>()
                                  .SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), false);
                    _shoppingCartService.ClearCartWarehouseCodeByCustomer();
                }
                else
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod);
                    _shoppingCartService.ClearCartWarehouseCodeByCustomer();

                    if (_cart.HasOverSizedItemWithPickupShippingMethod())
                    {
                        _navigationService.NavigateToCheckOutStore();
                    }
                }
            }
            
            if (Request.QueryString["PayPal"] == bool.TrueString && Request.QueryString["token"] != null)
            {
                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                _navigationService.NavigateToUrl("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
            }
            else
            {
                _navigationService.NavigateToCheckOutPayment();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }
            base.OnUnload(e);
        }

        #endregion
    }
}