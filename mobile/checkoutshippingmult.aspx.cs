// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls.mobile;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{

    /// <summary>
    /// Summary description for checkoutshippingmult.
    /// </summary>
    public partial class checkoutshippingmult : SkinBase
    {
        #region Variable Declaration

        private InterpriseShoppingCart _cart = null;
        private List<CountryAddressDTO> _countries = null;
        private bool shouldRegisterAddressCountries = true;

        #endregion

        #region DomainServices

        IAppConfigService _appConfigService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        INavigationService _navigationService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitializeDomainServices();
            SetCacheability();
            RequireSecurePage();
            RequireCustomerRecord();
            InitializeShoppingCart();
            PerformPageAccessLogic();
            InitializeCartRepeaterControl();
            DisplayCheckOutStepsImage();

			//mobile setup
            btnCompletePurchase.Text = AppLogic.GetString("checkoutshippingmult.aspx.6");
            litClickHere.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.8");
            lblHeader1.Text = AppLogic.GetString("checkoutshippingmult.aspx.5");
            lblHeader2.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.9");
            lblHeader3.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.10");
            lnkShipAllItemsToPrimaryShippingAddress.Text = AppLogic.GetString("mobile.checkoutshipping.aspx.8");
        }

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

        private void PerformPageAccessLogic()
        {
            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            _customerService.DoIsNotRegisteredChecking();

            if (ThisCustomer.IsCreditOnHold) 
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required")));
            }

            if (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("checkoutpayment.aspx.1").ToUrlEncode());
            }

            SectionTitle = AppLogic.GetString("checkoutshippingmult.aspx.1");

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

            if (_cart.HasCoupon() && (ThisCustomer.ThisCustomerSession[DomainConstants.CLEAR_COUPON_DISCOUNT].IsNullOrEmptyTrimmed()))
            {
                errorSummary.DisplayErrorMessage(AppLogic.GetString("checkoutshippingmult.aspx.cs.5"));
                ThisCustomer.ThisCustomerSession.SetVal(DomainConstants.CLEAR_COUPON_DISCOUNT, true.ToString());
                return;
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (_cart.IsNoShippingRequired() || !Shipping.MultiShipEnabled() || (_cart.NumItems() > AppLogic.MultiShipMaxNumItemsAllowed()) || _cart.NumItems() == 1)
            {
                // not allowed then:
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.3")));
            }

            if (ThisCustomer.PrimaryShippingAddress == null ||
                CommonLogic.IsStringNullOrEmpty(ThisCustomer.PrimaryShippingAddress.AddressID))
            {
                // not allowed here anymore!
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.2")));
            }
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            _cart.BuildSalesOrderDetails();
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

		//Modified the design for Mobile layout - converted to divs
        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is CartItem)
                {
                    // details
                    var item = e.Item.DataItem as CartItem;
                    if (item.IsDownload || item.IsService)
                    {
                        var lblDownloadText = e.Item.FindByParse<Label>("lblDownloadText");
                        lblDownloadText.Text = AppLogic.GetString("checkoutshippingmult.aspx.7");
                        lblDownloadText.Visible = true;
                    }
                    else
                    {
                        var ctrlAddressSelector = new AddressSelectorControl()
                        {
                            ID = "ctrlAddressSelector",
                            AddressesDataSource = ThisCustomer.ShippingAddresses,
                            SelectedAddressID = item.m_ShippingAddressID
                        };

                        var litAddressContainer = e.Item.FindByParse<Panel>("pnlAddressSelector");
                        litAddressContainer.Controls.Add(ctrlAddressSelector);

                        var script = new StringBuilder();
                        script.Append("<script type='text/javascript' >\n");
                        script.Append("$(document).ready(function(){\n");
                        script.AppendFormat("var row = new ise.Controls.CheckOutShippingMultiItemRowControl();\n");
                        script.AppendFormat("row.setAddressSelectorcontrolId('{0}');\n", ctrlAddressSelector.ClientID);
                        script.Append(" });\n");
                        script.Append("</script>\n");
                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                    }
                }
            }
        }

        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetCartItemsDataSource();
            rptCartItems.DataBind();
        }

        private void InitializeAddressControl(AddressControl2 ctrlShippingAddress)
        {
            LoadAllAvailableCountriesAndAssignRegistriesForAddresses(ctrlShippingAddress);
            AssignShippingAddressCaptions(ctrlShippingAddress);
            AssignShippingAddressValidatorPrerequisites(ctrlShippingAddress);
            
            ctrlShippingAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
			//for mobile layout
            ctrlShippingAddress.DataBind();
        }

        private void LoadAllAvailableCountriesAndAssignRegistriesForAddresses(AddressControl2 ctrlShippingAddress)
        {
            if (null == _countries || _countries.Count == 0)
            {
                _countries = CountryAddressDTO.GetAllCountries();
            }

            ctrlShippingAddress.Countries = _countries;
            ctrlShippingAddress.RegisterCountries = shouldRegisterAddressCountries;

            // avoid re-registering
            shouldRegisterAddressCountries = false;
        }

        private void AssignShippingAddressCaptions(AddressControl2 ctrlShippingAddress)
        {
            ctrlShippingAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6");
            ctrlShippingAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7");
            ctrlShippingAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.34");
            ctrlShippingAddress.AddressCaption = AppLogic.GetString("createaccount.aspx.22");
            ctrlShippingAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15");            
            ctrlShippingAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.132");
            ctrlShippingAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16");
            ctrlShippingAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33");
            ctrlShippingAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.77");

            ctrlShippingAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31");
            ctrlShippingAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32");
        }

        private void AssignShippingAddressValidatorPrerequisites(AddressControl2 ctrlShippingAddress)
        {
            ctrlShippingAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.56");
            ctrlShippingAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.57");
            ctrlShippingAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.58");
            ctrlShippingAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.59");
            ctrlShippingAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.60");

            ctrlShippingAddress.FirstNameMaximumCharacterLength = 50;
            ctrlShippingAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.61");
            ctrlShippingAddress.LastNameMaximumCharacterLength = 50;
            ctrlShippingAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.62");
            ctrlShippingAddress.AccountNameMaximumCharacterLength = 100;
            ctrlShippingAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.63");
            ctrlShippingAddress.AddressMaximumCharacterLength = 200;
            ctrlShippingAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.64");
            ctrlShippingAddress.PhoneMaximumCharacterLength = 50;
            ctrlShippingAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.65");

            ctrlShippingAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.71");
            ctrlShippingAddress.CityMaximumCharacterLength = 50;
            ctrlShippingAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.75");

            ctrlShippingAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.72");
            ctrlShippingAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlShippingAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.76");
        }

        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
			//required for mobile layout
            writer.WriteLine("<script type=\"text/javascript\" src=\"js/jquery/jquery-ui.min.js\" ></script>");
            writer.WriteLine("<link  type=\"text/css\" rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/jquery-ui.css\" />");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/checkoutshippingmulti_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private IEnumerable<CartItem> GetCartItemsDataSource()
        {
            var individualItems = new List<CartItem>();

            foreach (CartItem item in _cart.CartItems)
            {
                if (item.IsDownload || item.IsService)
                {
                    individualItems.Add(item);
                }
                else
                {
                    for (int quantity = 1; quantity <= item.m_Quantity; quantity++)
                    {
                        individualItems.Add(item);
                    }
                }
            }

            return individualItems;
        }

        private void ReloadCartAndItemAddresses()
        {
            InitializeShoppingCart();
            shouldRegisterAddressCountries = true;
            InitializeDataSource();            
        }

        private void ProcessCartItemAddresses()
        {
            var itemsPerAddress = new Dictionary<string, List<CartItem>>();
            var individualItems = rptCartItems.DataSource as List<CartItem>;

            for (int ctr = 0; ctr < individualItems.Count; ctr++)
            {
                var item = individualItems[ctr];

                if (item.IsDownload || item.IsService)
                {
                    // ship this to primary if not yet set..
                    _cart.SetItemAddress(item.m_ShoppingCartRecordID, ThisCustomer.PrimaryShippingAddress.AddressID);
                }
                else
                {
                    var ctrlAddressSelector = rptCartItems.Items[ctr].FindControl("ctrlAddressSelector") as AddressSelectorControl;

                    string preferredAddress = ctrlAddressSelector.SelectedAddress.AddressID;

                    if (item.m_ShippingAddressID != ctrlAddressSelector.SelectedAddress.AddressID)
                    {
                        if (!itemsPerAddress.ContainsKey(preferredAddress))
                        {
                            itemsPerAddress.Add(preferredAddress, new List<CartItem>());
                        }

                        var itemsInThisAddress = itemsPerAddress[preferredAddress];


                        // check if we have dups for this item
                        if (!itemsInThisAddress.Any(itemPerAddress => itemPerAddress.ItemCode == item.ItemCode))
                        {
                            itemsInThisAddress.Add(item);
                            item.MoveableQuantity = 1;
                        }
                        else
                        {
                            var savedCartItem = itemsInThisAddress.First(i => i.ItemCode == item.ItemCode);
                            savedCartItem.MoveableQuantity = savedCartItem.MoveableQuantity + 1;
                        }
                    }
                }
            }

            var lstRecAndTotalItems = _cart.CartItems.Select(i => new EcommerceCartRecordPerQuantity()
            {
                CartRecId = i.m_ShoppingCartRecordID,
                Total = i.m_Quantity
            }).ToList();

            foreach (string preferredAddress in itemsPerAddress.Keys)
            {
                foreach (CartItem item in itemsPerAddress[preferredAddress])
                {
                    if (item.ItemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                    {
                        var composition = KitComposition.FromCart(ThisCustomer, CartTypeEnum.ShoppingCart, item.ItemCode, item.Id);

                        var cartRecord = lstRecAndTotalItems.Single(ri => ri.CartRecId == item.m_ShoppingCartRecordID);

                        cartRecord.Total = cartRecord.Total - item.MoveableQuantity;

                        _cart.SetItemQuantity(cartRecord.CartRecId, cartRecord.Total);
                        _cart.AddItem(ThisCustomer,preferredAddress,item.ItemCode,item.ItemCounter,item.MoveableQuantity,item.UnitMeasureCode,CartTypeEnum.ShoppingCart,composition);
                        InitializeShoppingCart();
                    }
                    else
                    {
                        var cartRecord = lstRecAndTotalItems.Single(ri => ri.CartRecId == item.m_ShoppingCartRecordID);

                        cartRecord.Total = cartRecord.Total - item.MoveableQuantity;

                        _cart.SetItemQuantity(cartRecord.CartRecId, cartRecord.Total);
                        _cart.AddItem(ThisCustomer,preferredAddress,item.ItemCode,item.ItemCounter,item.MoveableQuantity,item.UnitMeasureCode,CartTypeEnum.ShoppingCart);
                        InitializeShoppingCart();
                    }
                }
            }
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);
            }
            ProcessCartItemAddresses();
            Response.Redirect("checkoutshippingmult2.aspx");
        }

        protected void lnkShipAllItemsToPrimaryShippingAddress_Click(object sender, EventArgs e)
        {
            ShipAllCartITemsToPrimaryShippingAddress();
        }

        private void ShipAllCartITemsToPrimaryShippingAddress()
        {
            string primaryShippingAddress = ThisCustomer.PrimaryShippingAddress.AddressID;

            foreach (CartItem item in _cart.CartItems)
            {
                _cart.SetItemAddress(item.m_ShoppingCartRecordID, primaryShippingAddress);
            }

            ReloadCartAndItemAddresses();
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
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
        }
    }
}
