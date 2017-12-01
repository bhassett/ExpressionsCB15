// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;
using System.Linq;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutshippingmult2.
    /// </summary>
    public partial class shipping : SkinBase
    {
        #region Variable Declaration

        private InterpriseShoppingCart _cart = null;
        private int _shippingMethodCount = 0;
        bool _isFromPayPal = false;

        #endregion

        #region DomainServices

        INavigationService _navigationService = null;
        IAppConfigService _appConfigService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        IStringResourceService _stringResourceService = null;

        #endregion

        #region Properties

        public int ShippingGroupCounter { get; set; }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.PageNoCache();

            _isFromPayPal = ("PayPal".ToQueryString() == bool.TrueString && "token".ToQueryString() != null);

            InitializeDomainService();
            RequireSecurePage();
            RequireCustomerRecord();
            InitializeShoppingCart();
            PerformPageAccessLogic();
            InitializeShipToAddressControl();
            InitializeCartRepeaterControl();
            DisplayCheckOutStepsImage();
            DisplayOrderSummary();
        }

        private void InitializeDomainService()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        }

        private void PerformPageAccessLogic()
        {
            _customerService.DoIsNotRegisteredChecking();

            if (_appConfigService.RequireOver13Checked && !ThisCustomer.IsOver13)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkout.over13required"));
            }

            if ((ThisCustomer.PrimaryBillingAddress == null || ThisCustomer.PrimaryShippingAddress == null) &&
                (ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed() || ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed()))
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutpayment.aspx.1"));
            }

            SectionTitle = AppLogic.GetString("checkoutshippingmult.aspx.1");

            if (_cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("shoppingcart.aspx.1"));
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (ThisCustomer.PrimaryShippingAddress == null || ThisCustomer.PrimaryShippingAddress.AddressID.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.2"));
            }

            if (_cart.HasRegistryItemButParentRegistryIsRemoved() || _cart.HasRegistryItemsRemovedFromRegistry())
            {
                _cart.RemoveRegistryItemsHasDeletedRegistry();
                _cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.18"));
            }

            if (_cart.HasRegistryItemsAndOneOrMoreItemsHasZeroInNeed())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.15"));
            }

            if (_cart.HasRegistryItemsAndOneOrMoreItemsExceedsToTheInNeedQuantity())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.14"));
            }

            if (_appConfigService.GetInventoryPreference()
                     .IsAllowFractional && _cart.HasCartItemWithDecimalQuantity())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.cs.3.1"));
            }
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
            var newAddressId = CommonLogic.QueryStringCanBeDangerousContent("newAddressId");
            if (!string.IsNullOrWhiteSpace(newAddressId))
            {
                ThisCustomer.SelectedShippingAddressID = newAddressId;
                _cart.SetCartShippingAddressID(newAddressId);
            }
            _cart.BuildSalesOrderDetails(true);
        }

        private void InitializeShipToAddressControl()
        {
            if (ThisCustomer.IsNotRegistered || (_isFromPayPal && !AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress")))
            {
                ctrlShipToAddressControl.Visible = false;
            }
            else
            {
                ctrlShipToAddressControl.Cart = _cart;
                ctrlShipToAddressControl.ThisCustomer = ThisCustomer;
                ctrlShipToAddressControl.ShippingMethodControlId = string.Empty;
                ctrlShipToAddressControl.IsAdvanceFreightEnabled = true;
            }

        }

        private void DisplayCheckOutStepsImage()
        {
            CheckoutStepLiteral.Text = new XSLTExtensionBase(ThisCustomer, ThisCustomer.SkinID).DisplayCheckoutSteps(2, "shoppingcart.aspx", string.Empty, string.Empty);
        }
        private void DisplayOrderSummary()
        {

            DetailsLit.Text = _stringResourceService.GetString("itempopup.aspx.2");
            EditCartLit.Text = _stringResourceService.GetString("checkout1.aspx.44");
            var renderer = new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, "page.checkout.ordersummaryitems.xml.config", ThisCustomer.CouponCode);
            CheckoutOrderSummaryItemsLiteral.Text = _cart.RenderHTMLLiteral(renderer);
            OrderSummaryCardLiteral.Text = AppLogic.RenderOrderSummaryCard(renderer.OrderSummary);
            
        }
        private void InitializeCartRepeaterControl()
        {
            rptCartItems.ItemDataBound += rptCartItems_ItemDataBound;
            InitializeDataSource();
        }

        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetDataSource();
            rptCartItems.DataBind();
        }

        private void InitializeAddressControl(AddressControl2 ctrlShippingAddress)
        {

        }

        private List<InterpriseShoppingCart> GetDataSource()
        {
            return _cart.SplitIntoMultipleOrdersBySpecificType();
        }

        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is InterpriseShoppingCart)
                {
                    var cart = e.Item.DataItem as InterpriseShoppingCart;
                    foreach (CartItem item in cart.CartItems)
                    {
                        cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                     .New(CartTypeEnum.ShoppingCart, string.Empty, false, true, string.Empty, string.Empty, item.ItemSpecificType, true, true);
                    }
                    cart.BuildSalesOrderDetails(true);
                    foreach (CartItem item in cart.CartItems)
                    {
                        var itemContainer = e.Item.FindByParse<Panel>("pnlItemContainer");
                        itemContainer.Controls.Add(new Label() { Text = item.ItemDescription });
                        itemContainer.Controls.Add(new Literal() { Text = " " });
                        itemContainer.Controls.Add(new Label() { Text = AppLogic.GetString("shoppingcart.cs.25") });
                        itemContainer.Controls.Add(new Literal() { Text = " : " });
                        itemContainer.Controls.Add(new Label() { Text = Localization.ParseLocaleDecimal(item.m_Quantity, ThisCustomer.LocaleSetting) });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                    }

                    var lblOptionName = e.Item.FindByParse<Label>("lblOptionName");
                    lblOptionName.Text = (string.IsNullOrEmpty(cart.OptionName)? cart.CartItems[0].ItemSpecificTypeDescription: cart.OptionName);
                    var mainShipMethodContainer = e.Item.FindByParse<Panel>("divShippingInfo");
                    var ctrlShippingMethod = e.Item.FindByParse<UserControls_ShippingMethodControl>("ctrlShippingMethod");
                    if (!cart.HasShippableComponents())
                    {
                        //lblShipmethodHeader.Text  = AppLogic.GetString("checkoutshippingmult.aspx.7");
                        //lblShipmethodHeader.CssClass = "notificationtext";
                        //ctrlShippingMethod.ItemSpecificType = AppLogic.GetString("checkoutshippingmult.aspx.7");
                        mainShipMethodContainer.Visible = false;
                    }
                    else
                    {
                        string shippingAddressID = String.Empty;
                        shippingAddressID = (string.IsNullOrWhiteSpace(ThisCustomer.SelectedShippingAddressID) ? ThisCustomer.PrimaryShippingAddressID : ThisCustomer.SelectedShippingAddressID);

                        var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, shippingAddressID, cart.FirstItem().GiftRegistryID);

                        ctrlShippingMethod.ShippingAddressID = shippingAddress.AddressID;
                        //Set these properties to disable the instore pickup
                        ctrlShippingMethod.HideInStorePickUpShippingOption = false;
                        ctrlShippingMethod.HidePickupStoreLink = true;
                        ctrlShippingMethod.ItemSpecificType = ((cart.CartItems[0].ItemSpecificType.IsNullOrEmpty()) ? "" : cart.CartItems[0].ItemSpecificType);
                        ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
                        ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9");
                        //CustomCartItem item = new CustomCartItem();
                        //item.Counter = cart.CartItems[0].ItemCounter;
                        //item.IsDownload = cart.CartItems[0].IsDownload;
                        //item.ItemCode = cart.CartItems[0].ItemCode;
                        //item.UnitMeassureCode = cart.CartItems[0].UnitMeasureCode;
                        //ctrlShippingMethod.InstoreCartItem = item;

                        if (_appConfigService.ShippingRatesOnDemand)
                        {
                            ctrlShippingMethod.ShowShowAllRatesButton = true;
                            ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
                        }
                        ctrlShippingMethod.IsMultipleShipping = true;

                        mainShipMethodContainer.Visible = true;

                        var script = new StringBuilder();
                        script.Append("<script type='text/javascript'>\n");
                        script.Append("$(document).ready( function() { \n");
                        script.AppendFormat("ise.Pages.CheckOutShippingMulti2.registerShippingMethodControlId('{0}');\n", ctrlShippingMethod.ClientID);
                        script.Append("});\n");
                        script.Append("</script>\n");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                        _shippingMethodCount++;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = new StringBuilder();

            script.Append("<script type=\"text/javascript\">\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", AppLogic.GetString("checkoutshipping.aspx.9"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", AppLogic.GetString("checkoutshipping.aspx.10"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", AppLogic.GetString("checkoutshipping.aspx.11"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", AppLogic.GetString("checkoutshipping.aspx.12"));

            script.AppendFormat("   ise.Pages.CheckOutShippingMulti2.setForm('{0}');\n", this.frmCheckOutMultiShipping2.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShippingMulti2.setExpectedCount({0});\n", _shippingMethodCount);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
            // this is a prerequisite as we can't be sure of the ordering of jscripts called, this will be rendered on the <head> section
            writer.WriteLine("<script type=\"text/javascript\" src=\"jscripts/core.js\" ></script>");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutshippingmulti2_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/checkoutstore_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) return;

            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);
            }

            //Clear EcommerceRealTimeRate record first
            DB.ExecuteSQL(String.Format("DELETE FROM EcommerceRealTimeRate WHERE ContactCode = {0}", ThisCustomer.ContactCode.ToDbQuote()));

            var carts = rptCartItems.DataSource as List<InterpriseShoppingCart>;
            bool hasPickUp = false;
            for (int ctr = 0; ctr < carts.Count; ctr++)
            {
                // the items should be at sync with the cart datasource..
                var cart = carts[ctr];

                if (cart.HasShippableComponents())
                {
                    var ctrlShippingMethod = rptCartItems.Items[ctr].FindControl("ctrlShippingMethod") as UserControls_ShippingMethodControl;

                    string shippingMethod = ctrlShippingMethod.ShippingMethod;

                    if (ctrlShippingMethod.FreightCalculation == "1" || ctrlShippingMethod.FreightCalculation == "2")
                    {
                        foreach (CartItem Cart in cart.CartItems)
                        {
                            cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, ctrlShippingMethod.ShippingAddressID, ctrlShippingMethod.RealTimeRateGUID, Cart.ItemSpecificType);
                        }

                        string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                        cart.SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), true);
                    }
                    else
                    {
                        foreach (CartItem Cart in cart.CartItems)
                        {
                            cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, ctrlShippingMethod.ShippingAddressID, Guid.Empty, Cart.ItemSpecificType);
                        }
                    }
                    if (ctrlShippingMethod.FreightChargeType.ToUpperInvariant() == DomainConstants.PICKUP_FREIGHT_CHARGE_TYPE)
                    {
                        hasPickUp = true;
                    }
                }
                else
                {
                    cart.MakeShippingNotRequired(false);
                }
            }
            if (hasPickUp)
            {
                _navigationService.NavigateToCheckOutStore();
            }
            else
            {
                Response.Redirect("checkoutpayment.aspx");
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
    }
}
