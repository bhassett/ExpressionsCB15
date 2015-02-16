// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutshipping : SkinBase
    {
        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        public const string PAYMENT_METHOD_CREDITCARD = "Credit Card";

		bool _IsPayPal = false;

        #region DomainServices

        IAppConfigService _appConfigService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        IStringResourceService _stringResourceService = null;
        INavigationService _navigationService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            base.OnInit(e);
                // Check for paypal
            _IsPayPal = (CommonLogic.QueryStringCanBeDangerousContent("PayPal") == bool.TrueString && CommonLogic.QueryStringCanBeDangerousContent("token") != null);

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
			
			//mobile button
            btnCompletePurchase.Click += btnCompletePurchase_Click;
            btnCompletePurchase2.Click += btnCompletePurchase_Click;

        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            string couponCode = ThisCustomer.CouponCode;
            _cart.BuildSalesOrderDetails(false, true, couponCode);
            _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping(couponCode);
        }

        private void PerformPageAccessLogic()
        {
            _customerService.DoMobileIsNotRegisteredChecking();

            if (ThisCustomer.IsCreditOnHold)
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required")));
            }

            //If current user came from IS, chances are it has no Primary Billing Info! then tried to checkout
            if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryBillingAddressID == String.Empty)
            {
                Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
            }

            SectionTitle = AppLogic.GetString("checkoutshipping.aspx.1");

            if (_cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (_cart.HasRegistryItems())
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (_cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1")));
            }

            string couponCode = string.Empty;
            string couponErrorMessage = string.Empty;
            if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.IsNoShippingRequired() && (_cart.HasMultipleShippingAddresses()) && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.NumItems() > 1)
            {
                Response.Redirect("checkoutshippingmult.aspx");
            }
        }

        private void CheckWhetherToRequireShipping()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") ||
                !_cart.HasShippableComponents() ||
                _cartHasCouponAndIncludesFreeShipping)
            {
                _cart.MakeShippingNotRequired();

                if (!_IsPayPal)
                {
                    Response.Redirect("checkoutpayment.aspx");
                }
                else
                {

                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                    Response.Redirect("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
                }
            }
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4");

			if(_IsPayPal)
				checkoutheadergraphic.HotSpots[1].HotSpotMode = HotSpotMode.Inactive;
        }

        private void InitializeShippingMethodControlValues()
        {
            if (_cartHasCouponAndIncludesFreeShipping)
            {
                ctrlShippingMethod.Visible = false;
            }
            else
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
            }

            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9");
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
            ctrlShippingMethod.HidePickupStoreLink = true;
            ctrlShippingMethod.HideInStorePickUpShippingOption = true;

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
            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9");
        }

        private void InitializeShippingMethodCaptions()
        {
            if (!_cart.CartAllowsShippingMethodSelection) return;
            
            if (_cartHasCouponAndIncludesFreeShipping)
            {
                lblSelectShippingMethod.Text = AppLogic.GetString("checkoutshipping.aspx.5");
            }
            else
            {
                if (ThisCustomer.IsRegistered && Shipping.MultiShipEnabled() && _cart.TotalQuantity() > 1 
                    && _appConfigService.AllowShipToDifferentThanBillTo)
                {
					//mobile added resource
                    lblSelectShippingMethod.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.7");
					lblClickHere.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.8"); 
                }
                else
                {
                    lblSelectShippingMethod.Text = AppLogic.GetString("checkout1.aspx.4");
                }
            }
        }

        private void DisplayOrderSummary()
        {
			//mobile renderer
            OrderSummary.Text = _cart.RenderHTMLLiteral(new MobileCheckOutShippingPageLiteralRenderer());
        }

        private void AssignCheckOutButtonCaption()
        {
            if (_cartHasCouponAndIncludesFreeShipping)
            {
                btnCompletePurchase.Text = AppLogic.GetString("checkoutshipping.aspx.8");
                btnCompletePurchase2.Text = btnCompletePurchase.Text;
            }
            else
            {
                btnCompletePurchase.Text = AppLogic.GetString("checkoutshipping.aspx.6");
                btnCompletePurchase2.Text = btnCompletePurchase.Text;
            }
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
            string total;
            
            if (threshHold > decimal.Zero && threshHold > subTotal)
            {
                pnlGetFreeShippingMsg.Visible = true;
                total = threshHold.ToCustomerCurrency();
                GetFreeShippingMsg.Text = String.Format(AppLogic.GetString("checkoutshipping.aspx.2"), total, shippingMethods);
            }
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/jquery-template/shipping-method-template.js"));
            manager.Scripts.Add(new ScriptReference("js/jquery-template/shipping-method-oversized-template.js"));
            manager.Scripts.Add(new ScriptReference("js/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/checkoutshipping_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

		protected void Page_Load(object sender, EventArgs e)
        {
			//mobile button new resources
            btnEditShoppingCart.Text = AppLogic.GetString("checkoutcard.aspx.4");
            btnEditShoppingCart.TheButton.PostBackUrl = "../ShoppingCart.aspx";

            if (_cartHasCouponAndIncludesFreeShipping) return;
            
            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" >\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", AppLogic.GetString("checkoutshipping.aspx.9"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", AppLogic.GetString("checkoutshipping.aspx.10"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", AppLogic.GetString("checkoutshipping.aspx.11"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", AppLogic.GetString("checkoutshipping.aspx.12"));

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
                //  validate the shipping method is existing and is associated for this group
                //  use a validator for this purpose

                if (ctrlShippingMethod.FreightCalculation == "1" || ctrlShippingMethod.FreightCalculation == "2")
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, String.Empty, ctrlShippingMethod.RealTimeRateGUID);

                    string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                    _cart.SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), false);
                }
                else
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod);
                }

            }

            if (Request.QueryString["PayPal"] == bool.TrueString && Request.QueryString["token"] != null)
            {
                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                Response.Redirect("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
            }
            else
                Response.Redirect("checkoutpayment.aspx");
                
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }
            base.OnUnload(e);
        }

        private void InitializeDomainServices()
        {
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
        }

    }

}