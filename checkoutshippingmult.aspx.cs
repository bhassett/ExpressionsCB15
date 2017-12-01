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
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;

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

        INavigationService _navigationService = null;
        IAppConfigService _appConfigService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        IStringResourceService _stringResourceService = null;

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
            DisplayOrderSummary();
            InitControlText();
        }

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        }

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

        private void PerformPageAccessLogic()
        {
            _customerService.DoIsNotRegisteredChecking();

            if (ThisCustomer.IsCreditOnHold) { _navigationService.NavigateToShoppingCart(); }

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

            if (_cart.HasCoupon() && (ThisCustomer.ThisCustomerSession[DomainConstants.CLEAR_COUPON_DISCOUNT].IsNullOrEmptyTrimmed()))
            {
                DisplayErrorMessageIfAny(AppLogic.GetString("checkoutshippingmult.aspx.cs.5"));
                ThisCustomer.ThisCustomerSession.SetVal(DomainConstants.CLEAR_COUPON_DISCOUNT, true.ToString());
                return;
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if ((_cart.IsNoShippingRequired() || !Shipping.MultiShipEnabled() || _cart.NumItems() == 1 || 
                _cart.NumItems() > AppLogic.MultiShipMaxNumItemsAllowed()) && !_cart.HasRegistryItems())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.3"));
            }

            if (ThisCustomer.PrimaryShippingAddress == null || ThisCustomer.PrimaryShippingAddress.AddressID.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.2"));
            }

            if (_appConfigService.GetInventoryPreference()
                                 .IsAllowFractional && _cart.HasCartItemWithDecimalQuantity())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.cs.3.1"));
            }

        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            _cart.BuildSalesOrderDetails(true);
        }

        private void DisplayCheckOutStepsImage()
        {
            CheckoutStepLiteral.Text = new XSLTExtensionBase(ThisCustomer, ThisCustomer.SkinID).DisplayCheckoutSteps(2, "shoppingcart.aspx", string.Empty, string.Empty);
            //checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            //((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3");
            //((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4");
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
            rptCartItems.ItemDataBound += new RepeaterItemEventHandler(rptCartItems_ItemDataBound);
            InitializeDataSource();
        }

        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                bool isCartItem = (e.Item.DataItem is CartItem);
                if(!isCartItem) return;

                var item = e.Item.DataItemAs<CartItem>();

                var trHeader = new TableRow();
                var converter = new WebColorConverter();
                trHeader.BackColor = (Color)converter.ConvertFrom("#" + AppLogic.AppConfig("LightCellColor"));
                // headers..

                var tdItemNameHeader = new TableCell();
                tdItemNameHeader.Width = Unit.Percentage(30);
                var lblItemNameHeader = new Label();
                lblItemNameHeader.Text = string.Format("<b>{0}</b>", AppLogic.GetString("shoppingcart.cs.1"));
                tdItemNameHeader.Controls.Add(lblItemNameHeader);

                trHeader.Cells.Add(tdItemNameHeader);

                var tdShipHeader = new TableCell();
                tdShipHeader.Width = Unit.Percentage(70);
                var lblShipHeader = new Label();
                lblShipHeader.Text = string.Format("<b>{0}</b>", AppLogic.GetString("shoppingcart.cs.24"));
                tdShipHeader.Controls.Add(lblShipHeader);

                trHeader.Cells.Add(tdShipHeader);
                e.Item.Controls.Add(trHeader);

                // details
                var trDetail = new TableRow();
                var tdDetailCaption = new TableCell()
                {
                    Width = Unit.Percentage(30),
                    VerticalAlign = VerticalAlign.Top,
                };
                trDetail.Cells.Add(tdDetailCaption);
    
                var lblItemName = new Label()
                {
                    Text = item.DisplayName
                };
                tdDetailCaption.Controls.Add(lblItemName);

        
                var tdDetailAddNew = new TableCell()
                {
                    Width = Unit.Percentage(70),
                    VerticalAlign = VerticalAlign.Top
                };
                trDetail.Cells.Add(tdDetailAddNew);

                e.Item.Controls.Add(trDetail);
       
                if (item.IsDownload || item.IsService)
                {
                    var lblNoShippingRequired = new Label() { Text = AppLogic.GetString("checkoutshippingmult.aspx.7") };
                    tdDetailAddNew.Controls.Add(lblNoShippingRequired);
                    tdDetailAddNew.Controls.Add(new LiteralControl("<br />"));
                    tdDetailAddNew.Controls.Add(new LiteralControl("<br />"));
                }
                else
                {
                    var ctrlAddressSelector = new AddressSelectorControl() { ID = "ctrlAddressSelector" };

                    var availableAddresses = new List<Address>();
                    availableAddresses.AddRange(ThisCustomer.ShippingAddresses);

                    bool shouldNotContainingTheSameAddressId = !ThisCustomer.ShippingAddresses.Any(addressItem => addressItem.AddressID == item.m_ShippingAddressID && !item.GiftRegistryID.HasValue);
                    if (item.GiftRegistryID.HasValue && shouldNotContainingTheSameAddressId)
                    {
                        var registryBillingAddress = ThisCustomer.GetRegistryItemShippingAddress(item.m_ShippingAddressID, item.GiftRegistryID);
                        availableAddresses.Add(registryBillingAddress);
                        availableAddresses.Reverse();
                    }

                    ctrlAddressSelector.AddressesDataSource = availableAddresses;
                    ctrlAddressSelector.SelectedAddressID = item.m_ShippingAddressID;
                    tdDetailAddNew.Controls.Add(ctrlAddressSelector);

                    var script = new StringBuilder();

                    script.Append("<script type='text/javascript'>\n");
                    script.Append("$add_windowLoad(\n");                
                    script.Append(" function() { \n");

                    script.AppendFormat("   var row = new ise.Controls.CheckOutShippingMultiItemRowControl();\n");
                    script.AppendFormat("   row.setAddressSelectorcontrolId('{0}');\n", ctrlAddressSelector.ClientID);

                    script.Append(" }\n");
                    script.Append(");\n");
                    script.Append("</script>\n");

                    Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                }
            }
        }

        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetCartItemsDataSource();
            rptCartItems.DataBind();
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutshippingmulti_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/address.verification.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/minified/address.control.js"));
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
                var item = individualItems[ctr].Clone<CartItem>();
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
                foreach (var item in itemsPerAddress[preferredAddress])
                {
                    if (item.ItemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                    {
                        var composition = KitComposition.FromCart(ThisCustomer, CartTypeEnum.ShoppingCart, item.ItemCode, item.Id);
                        var cartRecord = lstRecAndTotalItems.Single(ri => ri.CartRecId == item.m_ShoppingCartRecordID);
                        cartRecord.Total = cartRecord.Total - item.MoveableQuantity;
                        _cart.SetItemQuantity(cartRecord.CartRecId, cartRecord.Total);

                        var sameKits = _cart.CartItems.Where(i=>i.IsAKit==true).Where(c => c.GetKitComposition().Matches(composition)).ToList();
                        bool hasKitCompositionMatch = (sameKits!=null)?sameKits.Count > 0:false;
                        composition.CartID = Guid.NewGuid();
                        Guid newID =_cart.AddItem(ThisCustomer,preferredAddress,item.ItemCode,item.ItemCounter,item.MoveableQuantity,item.UnitMeasureCode,CartTypeEnum.ShoppingCart,composition);
                        
                        if (hasKitCompositionMatch) {
                            foreach (var itemKitCart in sameKits){
                                _cart.SetItemQuantity(itemKitCart.m_ShoppingCartRecordID, itemKitCart.m_Quantity - item.MoveableQuantity);
                            }}
                        
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

            //Clear the cart item warehouse code since the user choose to multi-shipping address.
            ServiceFactory.GetInstance<IShoppingCartService>()
                          .ClearCartWarehouseCodeByCustomer();

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

        private void InitControlText(){

            btnCompletePurchase.Text = AppLogic.GetString("checkoutshippingmult.aspx.6");

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnCompletePurchase, "checkoutshippingmult.aspx.6");
            }
        }

        private void DisplayErrorMessageIfAny(string errorMessage)
        {
            if (errorMessage.IsNullOrEmptyTrimmed()) return;
            errorSummary.DisplayErrorMessage(errorMessage);
        }
    }
}
