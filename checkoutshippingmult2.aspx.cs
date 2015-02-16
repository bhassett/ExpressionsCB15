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

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutshippingmult2.
    /// </summary>
    public partial class checkoutshippingmult2 : SkinBase
    {
        #region Variable Declaration

        private InterpriseShoppingCart _cart = null;
        private int _shippingMethodCount = 0;

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

            InitializeDomainService();
            RequireSecurePage();
            RequireCustomerRecord();
            InitializeShoppingCart();
            PerformPageAccessLogic();
            InitializeCartRepeaterControl();
            DisplayCheckOutStepsImage();
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

            decimal totalCartItem = _cart.NumItems();
            if ((_cart.IsNoShippingRequired() || !Shipping.MultiShipEnabled() || totalCartItem == 1 || 
                    totalCartItem > AppLogic.MultiShipMaxNumItemsAllowed()) && !_cart.HasRegistryItems())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.3"));
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
            _cart.BuildSalesOrderDetails(true);
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4");
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
            return _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
        }

        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is InterpriseShoppingCart)
                {
                    var cart = e.Item.DataItem as InterpriseShoppingCart;
                    cart.BuildSalesOrderDetails(true);

                    foreach (CartItem item in cart.CartItems)
                    {
                        var itemContainer = e.Item.FindByParse<Panel>("pnlItemContainer");
                        itemContainer.Controls.Add(new Label() { Text = item.ItemDescription });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                        itemContainer.Controls.Add(new Label() { Text = AppLogic.GetString("shoppingcart.cs.25") });
                        itemContainer.Controls.Add(new Literal() { Text = " : " });
                        itemContainer.Controls.Add(new Label() { Text = Localization.ParseLocaleDecimal(item.m_Quantity, ThisCustomer.LocaleSetting) });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                    }

                    var mainShipMethodContainer = e.Item.FindByParse<Panel>("divShippingInfo");
                    var lblShipmethodHeader = e.Item.FindByParse<Label>("lblShipmethodHeader");

                    if (!cart.HasShippableComponents())
                    {
                        lblShipmethodHeader.Text = AppLogic.GetString("checkoutshippingmult.aspx.7");
                        lblShipmethodHeader.CssClass = "notificationtext";
                        mainShipMethodContainer.Visible = false;
                    }
                    else
                    {
                        var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, cart.FirstItem().m_ShippingAddressID, cart.FirstItem().GiftRegistryID);
                        var lblShippingAddressString = e.Item.FindByParse<Label>("lblShippingAddressString");
                        lblShippingAddressString.Text = shippingAddress.DisplayString(true, true, true, "<br/>");

                        var ctrlShippingMethod = e.Item.FindByParse<UserControls_ShippingMethodControl>("ctrlShippingMethod");

                        ctrlShippingMethod.ShippingAddressID = shippingAddress.AddressID;
                        //Set these properties to disable the instore pickup
                        ctrlShippingMethod.HideInStorePickUpShippingOption = true;
                        ctrlShippingMethod.HidePickupStoreLink = true;
                        ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
                        ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9");

                        if (_appConfigService.ShippingRatesOnDemand)
                        {
                            ctrlShippingMethod.ShowShowAllRatesButton = true;
                            ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
                        }
                        ctrlShippingMethod.IsMultipleShipping = true;

                        mainShipMethodContainer.Visible = true;
                        lblShipmethodHeader.Text = AppLogic.GetString("shoppingcart.cs.30");

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
                        cart.SetCartShippingMethod(shippingMethod, ctrlShippingMethod.ShippingAddressID, ctrlShippingMethod.RealTimeRateGUID);

                        string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                        cart.SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), true);
                    }
                    else
                    {
                        cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, ctrlShippingMethod.ShippingAddressID);
                    }

                }
                else
                {
                    cart.MakeShippingNotRequired(false);
                }
            }

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
    }
}
