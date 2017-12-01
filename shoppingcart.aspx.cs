// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Linq;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.DTO;
using System.Web.UI;
using System.Threading.Tasks;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for ShoppingCartPage.
    /// </summary>
    public partial class ShoppingCartPage : SkinBase
    {
        #region Private Members

        string _skinImagePath = String.Empty;
        InterpriseShoppingCart _cart = null;

        #endregion

        #region DomainServices

        IAuthenticationService _authenticationService = null;
        IStringResourceService _stringResourceService = null;
        IAppConfigService _appConfigService = null;
        IShoppingCartService _shoppingCartService = null;
        IShippingService _shippingService = null;
        INavigationService _navigationService = null;
        ILocalizationService _localizationService = null;
        IProductService _productService = null;
        

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            BindControls();

            InitializeShoppingCart();

            RenderOrderOptions();
            InitAddressControl();
            DoOrderNotesChecking();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.RequireCustomerRecord();
            this.RequireSecurePage();

            ClearErrors();

            ProcessDelete();
            ProcessMoveToWishList();

            InitializePageContent();
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
            InitializePageContent();

            SectionTitle = AppLogic.GetString("AppConfig.CartPrompt", true);
            HeaderMsg.SetContext = this;
            CartPageFooterTopic.SetContext = this;
        }

        void OrderOptionsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var orderOptionNode = e.Item.DataItem as XmlNode;
            int counter = 0;
            if (orderOptionNode != null &&
                int.TryParse(orderOptionNode["Counter"].InnerText, out counter))
            {
                string itemCode = orderOptionNode["ItemCode"].InnerText;
                string itemName = orderOptionNode["ItemName"].InnerText;
                string itemDescription = orderOptionNode["ItemDescription"].InnerText;
                string popupTitle = string.Empty;

                var lblDisplayName = e.Item.FindControl("OrderOptionName") as Label;
                if (!CommonLogic.IsStringNullOrEmpty(itemDescription))
                {
                    lblDisplayName.Text = Security.HtmlEncode(itemDescription);
                    popupTitle = CommonLogic.Left(Security.UrlEncode(SE.MungeName(itemDescription)), 90);
                }
                else
                {
                    lblDisplayName.Text = Security.HtmlEncode(itemName);
                    popupTitle = CommonLogic.Left(Security.UrlEncode(SE.MungeName(itemName)), 90);
                }

                if (AppLogic.AppConfigBool("ShowPicsInCart"))
                {
                    var img = ProductImage.Locate("product", counter, "icon");
                    string imgUrl = img.src;

                    if (!imgUrl.IsNullOrEmptyTrimmed() && imgUrl.IndexOf("nopicture") == -1)
                    {
                        var imgControl = (Image)e.Item.FindControl("OptionImage");
                        imgControl.ImageUrl = imgUrl;
                        imgControl.Visible = true;
                    }
                }

                var helpCircle = e.Item.FindControl("helpcircle_gif") as Image;
                helpCircle.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/" + "helpcircle.gif");
                helpCircle.Attributes.Add("onclick", "popuporderoptionwh('Order Option " + popupTitle + "', " + counter.ToString() + ",650,550,'yes');");

                // 2 Control choices for drop down list
                var cboUnitMeasureCode = e.Item.FindControl("cboUnitMeasureCode") as DropDownList;
                var lblUnitMeasureCode = e.Item.FindControl("lblUnitMeasureCode") as Label;
                var availableUnitMeasures = ProductDA.GetProductUnitMeasureAvailability(ThisCustomer.CustomerCode, itemCode,
                                                                                        AppLogic.AppConfigBool("ShowInventoryFromAllWarehouses"),
                                                                                        ThisCustomer.IsNotRegistered);
                if (availableUnitMeasures.Count() > 1)
                {
                    lblUnitMeasureCode.Visible = false;
                    foreach (string unitMeasureCode in availableUnitMeasures)
                    {
                        cboUnitMeasureCode.Items.Add(new ListItem(unitMeasureCode.ToHtmlEncode(), unitMeasureCode.ToHtmlEncode()));
                    }
                }
                else
                {
                    // The only unit measure the item is configured for is the default
                    // which we are guaranteed to be in the first index..
                    cboUnitMeasureCode.Visible = false;
                    lblUnitMeasureCode.Text = availableUnitMeasures.First().ToHtmlEncode();
                }

                bool withVat = AppLogic.AppConfigBool("VAT.Enabled") && ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive;
                var um = UnitMeasureInfo.ForItem(itemCode, UnitMeasureInfo.ITEM_DEFAULT);

                decimal promotionalPrice = Decimal.Zero;
                decimal price = InterpriseHelper.GetSalesPriceAndTax(ThisCustomer.CustomerCode, itemCode, ThisCustomer.CurrencyCode, decimal.One, um.Code, withVat, ref promotionalPrice);
                if (promotionalPrice != decimal.Zero)
                {
                    price = promotionalPrice;
                }

                string vatDisplay = String.Empty;
                if (_appConfigService.VATIsEnabled)
                {
                    vatDisplay = (ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive) ?
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.38") + "</span>\n" :
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.37") + "</span>\n";
                }

                var lblPrice = e.Item.FindControl("OrderOptionPrice") as Label;
                lblPrice.Text = price.ToCustomerCurrency() + vatDisplay;

                var hfCounter = e.Item.FindControl("hfItemCounter") as HiddenField;
                hfCounter.Value = counter.ToString();

                var cbk = e.Item.FindControl("OrderOptions") as DataCheckBox;
                cbk.Checked = false;

                bool shouldBeAbleToEnterNotes = orderOptionNode["CheckOutOptionAddMessage"].InnerText.TryParseBool().Value;
                var lblNotes = e.Item.FindControl("lblNotes") as Label;
                var txtNotes = e.Item.FindControl("txtOrderOptionNotes") as TextBox;
                lblNotes.Visible = txtNotes.Visible = shouldBeAbleToEnterNotes;
                txtNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 1000);");
            }
        }

        protected void btnPayPalExpressCheckout_Click(object sender, ImageClickEventArgs e)
        {
            if (!ThisCustomer.IsRegistered &&
                (_appConfigService.PasswordIsOptionalDuringCheckout && AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout") && !ThisCustomer.IsUpdatedAnonCustRecord))
            {
                _navigationService.NavigateToUrl("checkoutanon.aspx?checkout=true&checkouttype=pp");
            }
            else if (!ThisCustomer.IsRegistered && !AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout"))
            {
                _navigationService.NavigateToShoppingCartWitErroMessage(AppLogic.GetString("shoppingcart.aspx.61"));
            }
            else if (!ThisCustomer.IsRegistered && !ThisCustomer.IsUpdatedAnonCustRecord)
            {
                _navigationService.NavigateToUrl("checkoutanon.aspx?checkout=true&checkouttype=pp");
            }
            else
            {
                ProcessCart(false);

                if (!_cart.IsSalesOrderDetailBuilt)
                {
                    _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart,true);
                    _cart.BuildSalesOrderDetails(false, false, CouponCode.Text);
                }

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "shoppingcart";
                Response.Redirect(PayPalExpress.CheckoutURL(_cart));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void BindControls()
        {
            OrderOptionsList.ItemDataBound += OrderOptionsList_ItemDataBound;

            btnContinueShoppingTop.Click += (ex, sender) => ContinueShopping();
            //btnContinueShoppingBottom.Click += (ex, sender) => ContinueShopping();
            btnCheckOutNowTop.Click += (ex, sender) => CheckoutNow();
            //btnCheckOutNowBottom.Click += (ex, sender) => CheckoutNow();

            btnUpdateCart1.Click += (ex, sender) => ProcessTheCart();
            btnUpdateCart2.Click += (ex, sender) => ProcessTheCart();
            btnUpdateCart3.Click += (ex, sender) => ProcessTheCart();
            btnUpdateCart4.Click += (ex, sender) => ProcessTheCart();
            btnUpdateCart5.Click += (ex, sender) => ProcessTheCart();
        }

        private void CheckoutNow()
        {
            if (_appConfigService.HideOutOfStockProducts) { DeleteOutofStockAndWithoutOpenPOItem(); }
            if (ThisCustomer.ThisCustomerSession[DomainConstants.HAS_NOSTOCK_PHASEDOUT_CART_ITEM].IsNullOrEmptyTrimmed())
            {
                DeleteOutofStockPhasedOutItem();
            }
            else 
            {
                return;
            }
            ProcessCart(true);
            
        }

        private void InitializeShoppingCart()
        {
            if (_cart != null) return;
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);

            // merge duplicate item/s
            if (_cart.CartItems.Count > 0) { _shoppingCartService.MergeDuplicateCartItems(_cart); }
        }

        private void InitAddressControl()
        {
            AddressControl.LabelCityText = _stringResourceService.GetString("shoppingcart.aspx.25");
            AddressControl.LabelStateText = _stringResourceService.GetString("shoppingcart.aspx.23");
            AddressControl.LabelPostalText = _stringResourceService.GetString("shoppingcart.aspx.24");
            AddressControl.LabelEnterPostalText = _stringResourceService.GetString("shoppingcart.aspx.47");
            AddressControl.IsHideStreetAddressInputBox = true;
            AddressControl.IsShowResidenceTypesSelector = true;
            AddressControl.IsShowCounty = false;
            AddressControl.DefaultAddressType = _stringResourceService.GetString("shoppingcart.aspx.46");
            AddressControl.BindData();
        }

        private void InitializeDomainServices()
        {
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _shippingService = ServiceFactory.GetInstance<IShippingService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _localizationService = ServiceFactory.GetInstance<ILocalizationService>();
            _productService = ServiceFactory.GetInstance<IProductService>();
        }

        private void ProcessDelete() 
        {
            string[] formkeys = Request.Form.AllKeys;
            if (formkeys.Any(k => k.Contains("bt_Delete")))
            {
                ProcessCart(false);
                RenderOrderOptions();
            }
        }

        private void ProcessMoveToWishList()
        {
            string[] formkeys = Request.Form.AllKeys;
            if (formkeys.Any(k => k.StartsWith("bt_wishlist_")))
            {
                ProcessWishlist();
                RenderOrderOptions();
            }
        }

        
        private void ProcessTheCart()
        {
            ProcessCart(false);
            InitializePageContent();
        }

        private void RedirectToSignInPage()
        {
            //btnCheckOutNowBottom.Enabled = false;
            btnCheckOutNowTop.Enabled = false;
            //btnContinueShoppingBottom.Enabled = false;
            btnContinueShoppingTop.Enabled = false;
            //btnUpdateCart1.Enabled = false;
            btnUpdateCart2.Enabled = false;
            btnUpdateCart3.Enabled = false;
            btnUpdateCart4.Enabled = false;
            btnUpdateCart5.Enabled = false;
            BodyPanel.Visible = false;
            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", true);
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", Server.UrlDecode("signin.aspx")));
        }

        private void InitializePageContent()
        {
            int AgeCartDays = AppLogic.AppConfigUSInt("AgeCartDays");
            if (AgeCartDays == 0)
            {
                AgeCartDays = 7;
            }
          
            string localeSetting = ThisCustomer.LocaleSetting;

            ShoppingCart.Age(AgeCartDays, CartTypeEnum.ShoppingCart);
            shoppingcartaspx8.Text = AppLogic.GetString("shoppingcart.aspx.6");
            shoppingcartaspx10.Text = AppLogic.GetString("shoppingcart.aspx.8");
            shoppingcartaspx11.Text = AppLogic.GetString("shoppingcart.aspx.9");
            shoppingcartaspx9.Text = AppLogic.GetString("shoppingcart.aspx.7");
            shoppingcartcs27.Text = AppLogic.GetString("shoppingcart.cs.5");
            shoppingcartcs28.Text = AppLogic.GetString("shoppingcart.cs.6");
            shoppingcartcs29.Text = AppLogic.GetString("shoppingcart.cs.7");

            string updateCartKey = "shoppingcart.cs.33";
            string updateCart = AppLogic.GetString(updateCartKey, true);
            //btnUpdateCart1.Text = updateCart;
            btnUpdateCart2.Text = updateCart;
            btnUpdateCart5.Text = updateCart;

            string continueShopping = AppLogic.GetString("shoppingcart.cs.12", true);
            btnContinueShoppingTop.Text = continueShopping;
            //btnContinueShoppingBottom.Text = continueShopping;

            string checkoutnow = AppLogic.GetString("shoppingcart.cs.34", true);
            btnCheckOutNowTop.Text = checkoutnow;
            //btnCheckOutNowBottom.Text = checkoutnow;

            if (ThisCustomer.IsInEditingMode())
            {
                //AppLogic.EnableButtonCaptionEditing(btnUpdateCart1, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart2, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart3, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart4, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart5, updateCartKey);

                AppLogic.EnableButtonCaptionEditing(btnContinueShoppingTop, "shoppingcart.cs.12");
                //AppLogic.EnableButtonCaptionEditing(btnContinueShoppingBottom, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnCheckOutNowTop, "shoppingcart.cs.34");
                //AppLogic.EnableButtonCaptionEditing(btnCheckOutNowBottom, "shoppingcart.cs.34");
            }
            else
            {
                //btnContinueShoppingBottom.OnClientClick = "self.location='" + AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting) + "'";
            }

            OrderNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 255);");

            if (!IsPostBack)
            {
                string couponCode = String.Empty;
                string couponErrorMessage = String.Empty;

                if (_cart.HasCoupon(ref couponCode) &&
                    _cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    CouponCode.Text = couponCode;
                }
                else
                {
                    WriteError(couponErrorMessage);
                    _cart.ClearCoupon();
                }

                //check customer IsCreditHold
                if (ThisCustomer.IsCreditOnHold && _cart != null)
                {
                    WriteError(AppLogic.GetString("shoppingcart.aspx.18", true));
                    _cart.ClearCoupon();
                }
            }
            else
            {
                if (CouponCode.Text.IsNullOrEmptyTrimmed()) _cart.ClearCoupon();
            }

            ThisCustomer.ThisCustomerSession.ClearVal(DomainConstants.CLEAR_COUPON_DISCOUNT);
            BuildSalesOrderCart();

            //Refresh page with errors since some registry items has been removed
            int totalRemoved = _cart.RemoveRegistryItemsHasDeletedRegistry();
            int totalItemsremoved = _cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
            if (totalRemoved > 0 || totalItemsremoved > 0)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("editgiftregistry.error.18", true).ToUrlEncode());
            }

            if (_cart.IsEmpty())
            {
                //btnUpdateCart1.Visible = false;
                AlternativeCheckoutsTop.Visible = false;
                //AlternativeCheckoutsBottom.Visible = false;
            }

            RenderValidationScript();

            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.ShoppingCartPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_ShoppingCartPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }

          

            ShippingInformation.Visible = (!AppLogic.AppConfigBool("SkipShippingOnCheckout"));
            AddresBookLlink.Visible = (ThisCustomer.IsRegistered);

            bool isNotCartEmpty = !_cart.IsEmpty();
            btnCheckOutNowTop.Visible = isNotCartEmpty;

            if (!IsPostBack)
            {
                string errorMsg = "ErrorMsg".ToQueryString().ToHtmlEncode();
                if (ErrorMsgLabel.Text != errorMsg)
                {
                    WriteError(errorMsg);
                }
            }

            var items = _cart.GetOverSizedItemWithShippingMethodNotBelongToCustomerShippingGroup();
            var giftItems = _cart.GetGiftRegistryItemsWithPickupShippingMethod();

            items = items.Union(giftItems).ToList();
                
            if(items.Count() > 0)
            {
                pnlOversizedShippingMethodNotValid.Visible = true;
                items.ForEach(recId => { _cart.RemoveItem(recId); });
                _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
                BuildSalesOrderCart();
            }
                
            if (_cart.HasNoStockPhasedOutItem)
            {
                pnlRemovePhasedOutItemWithNoStockError.Visible = true;
                RemovePhasedOutItemWithNoStockError.Text = _stringResourceService.GetString("shoppingcart.aspx.64");
                if (ThisCustomer.ThisCustomerSession[DomainConstants.HAS_NOSTOCK_PHASEDOUT_CART_ITEM].IsNullOrEmptyTrimmed())
                {
                    ThisCustomer.ThisCustomerSession.SetVal(DomainConstants.HAS_NOSTOCK_PHASEDOUT_CART_ITEM, true.ToString()); 
                }
                else 
                {
                    ThisCustomer.ThisCustomerSession.ClearVal(DomainConstants.HAS_NOSTOCK_PHASEDOUT_CART_ITEM); 
                }
            }

            if (_cart.HaNoStockAndNoOpenPOItem)
            {
                pnlRemovePhasedOutItemWithNoStockError.Visible = true;
                RemovePhasedOutItemWithNoStockError.Text = _stringResourceService.GetString("shoppingcart.aspx.48");
            }

            if (_cart.InventoryTrimmed)
            {
                pnlInventoryTrimmedError.Visible = true;
                InventoryTrimmedError.Text = AppLogic.GetString("shoppingcart.aspx.1", true);
            }

            if (_cart.MinimumQuantitiesUpdated)
            {
                pnlMinimumQuantitiesUpdatedError.Visible = true;
                MinimumQuantitiesUpdatedError.Text = AppLogic.GetString("shoppingcart.aspx.5", true);
            }

            decimal MinOrderAmount = AppLogic.AppConfigUSDecimal("CartMinOrderAmount");
            if (!_cart.MeetsMinimumOrderAmount(MinOrderAmount))
            {
                pnlMeetsMinimumOrderAmountError.Visible = true;
                string amountFormatted = MinOrderAmount.ToCustomerCurrency();
                MeetsMinimumOrderAmountError.Text = String.Format(AppLogic.GetString("shoppingcart.aspx.2", true), amountFormatted);
            }


            int quantityDecimalPlaces = InterpriseHelper.GetInventoryDecimalPlacesPreference();
            var formatter = (new CultureInfo(ThisCustomer.LocaleSetting)).NumberFormat;

            formatter.NumberDecimalDigits = quantityDecimalPlaces;
            formatter.PercentDecimalDigits = quantityDecimalPlaces;
            MeetsMinimumOrderQuantityError.Text = String.Empty;

            decimal MinQuantity = AppLogic.AppConfigUSDecimal("MinCartItemsBeforeCheckout");
            if (!_cart.MeetsMinimumOrderQuantity(MinQuantity))
            {
                pnlMeetsMinimumOrderQuantityError.Visible = true;
                MeetsMinimumOrderQuantityError.Text = String.Format(AppLogic.GetString("shoppingcart.aspx.16", true), MinQuantity.ToString(), MinQuantity.ToString());
            }

            var shoppingCartRenderer = new DefaultShoppingCartPageLiteralRenderer(RenderType.ShoppingCart, CouponCode.Text);
            CartItems.Text = _cart.RenderHTMLLiteral(shoppingCartRenderer);

            InitializeOrderSummary(shoppingCartRenderer.OrderSummary);

            if (!_cart.IsEmpty())
            {
                if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsRegistered && !ThisCustomer.IsOver13)
                {
                    btnCheckOutNowTop.Enabled = false;
                    //btnCheckOutNowBottom.Enabled = false;
                    WriteError(AppLogic.GetString("over13oncheckout", true));
                    return;
                }

                //btnCheckOutNowBottom.Enabled = btnCheckOutNowTop.Enabled;

                DisplayUpSellSection(_cart);
                DisplayCouponSection(_cart);

                //btnCheckOutNowBottom.Visible = true;

                if (!_cart.CartItems.All(item => item.IsOverSized))
                {
                    if (AppLogic.AppConfigBool("ShippingCalculator.Enabled"))
                    {
                        pnlShippingCalculator.Visible = true;
                    }
                }
                else
                {
                    pnlShippingCalculator.Visible = false;
                }

                DoAlternateCheckoutChecking();

                //if no alternative methods are visible, hide the whole row

                if (!AppLogic.IsSupportedAlternateCheckout && AlternativeCheckoutsTop.Visible)// && AlternativeCheckoutsBottom.Visible)
                {
                    WriteError(PayPalExpress.ErrorMsg);
                    AlternativeCheckoutsTop.Visible = false;
                    //AlternativeCheckoutsBottom.Visible = false;
                }

            }
            else
            {
                pnlOrderOptions.Visible = false;
                pnlUpsellProducts.Visible = false;
                pnlCoupon.Visible = false;
                pnlOrderNotes.Visible = false;
                //btnCheckOutNowBottom.Visible = false;
                pnlShippingCalculator.Visible = false;
            }

            //btnContinueShoppingBottom.OnClientClick = "self.location='" + AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting) + "'";
            CartPageFooterTopic.SetContext = this;

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.ShoppingCartPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_ShoppingCartPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }
        }

        private void InitializeOrderSummary(InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.OrderSummaryModel orderSummary)
        {
            OrderSummaryCardLit.Text = AppLogic.RenderOrderSummaryCard(orderSummary);
        }
        private void BuildSalesOrderCart()
        {
            if (_cart != null && !_cart.IsEmpty())
            {
                try
                {
                    _cart.BuildSalesOrderDetails(false, true, CouponCode.Text,true);
                }
                catch (InvalidOperationException ex)
                {
                    WriteError(ex.Message);
                    return;
                }
                catch (Exception ex) { throw ex; }
            }
        }

        private void DoAlternateCheckoutChecking()
        {
            //Check if alternate checkout methods are supported (PayPal)
            if (AppLogic.IsSupportedAlternateCheckout)
            {
                bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());

                if (AppLogic.AppConfigBool("PayPalCheckout.ShowOnCartPage") && (ThisCustomer.IsRegistered || !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                    && (_cart != null && !_cart.IsEmpty() && _cart.GetCartSubTotal() > Decimal.Zero && hidePaypalOptionIfMultiShipAndHasGiftRegistry))
                {
                    if (AppLogic.UseSSL() && AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
                    {
                        btnPayPalExpressCheckoutTop.ImageUrl = "https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                        //btnPayPalExpressCheckoutBottom.ImageUrl = "https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                    }
                    else
                    {
                        btnPayPalExpressCheckoutTop.ImageUrl = "http://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                        //btnPayPalExpressCheckoutBottom.ImageUrl = "http://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                    }
                    AlternativeCheckoutsTop.Visible = true;
                    //AlternativeCheckoutsBottom.Visible = true;
                    PayPalExpressSpanTop.Visible = true;
                    //PayPalExpressSpanBottom.Visible = true;
                }
                else
                {
                    AlternativeCheckoutsTop.Visible = false;
                    //AlternativeCheckoutsBottom.Visible = false;
                    PayPalExpressSpanTop.Visible = false;
                    //PayPalExpressSpanBottom.Visible = false;
                }
            }

            if (_cart.HasMultipleShippingMethod())
            {
                Label3.Text = AppLogic.GetString("shoppingcart.aspx.50");
                //Label1.Text = AppLogic.GetString("shoppingcart.aspx.50");
                PayPalExpressSpanTop.Visible = false;
                //PayPalExpressSpanBottom.Visible = false;
            }

        }

        private void DisplayCouponSection(InterpriseShoppingCart cart)
        {
            pnlCoupon.Visible = cart.CouponsAllowed && ThisCustomer.IsRegistered;
        }

        private void DisplayUpSellSection(InterpriseShoppingCart cart)
        {
            string upsellproductlist = GetUpsellProducts(cart);
            if (upsellproductlist.Length > 0)
            {
                UpsellProducts.Text = upsellproductlist;
                btnUpdateCart5.Visible = true;
                pnlUpsellProducts.Visible = true;
            }
            else
            {
                UpsellProducts.Text = string.Empty;
                pnlUpsellProducts.Visible = false;
            }
        }

        private void RenderOrderOptions()
        {
            string strXml = String.Empty;
            var optionsCount = ServiceFactory.GetInstance<IProductService>().GetCheckOutOptionCount(ref strXml);

            pnlOrderOptions.Visible = (optionsCount > 0);
            if (optionsCount > 0)
            {
                var xDoc = new XmlDocument();
                xDoc.LoadXml(strXml);

                var xslDoc = new XmlDocument();
                xslDoc.LoadXml("<?xml version=\"1.0\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:param name=\"locale\" /><xsl:template match=\"/\"><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:choose><xsl:when test=\"ml\"><xsl:value-of select=\"ml/locale[@name=$locale]\"/></xsl:when><xsl:otherwise><xsl:value-of select=\".\"/></xsl:otherwise></xsl:choose></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:template></xsl:stylesheet>");

                var xsl = new XslCompiledTransform();
                xsl.Load(xslDoc);

                var tw = new StringWriter();
                var xslArgs = new XsltArgumentList();
                xslArgs.AddParam("locale", "", ThisCustomer.LocaleSetting);
                xsl.Transform(xDoc, xslArgs, tw);

                var xResults = new XmlDocument();
                xResults.LoadXml(tw.ToString());

                var nodeList = xResults.SelectNodes("//orderoption");

                OrderOptionsList.DataSource = nodeList;
                OrderOptionsList.DataBind();
            }
        }

        private void RenderValidationScript()
        {
            var html = new StringBuilder();
            html.Append("<script type='text/javascript'>\n");
            html.Append("function Cart_Validator(theForm)\n");
            html.Append("{\n");
            string cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);


            var groupedCartItemsByBundle = _cart.CartItems.GroupBy(item => item.BundleCode).Select(item => new { Key = item.Key, Items = item.ToList() }).ToList();
            foreach (var cartItem in groupedCartItemsByBundle)
            {
                if (string.IsNullOrEmpty(cartItem.Key))
                {
                    //Items not is a Bundle Item
                    foreach (var item in cartItem.Items)
                    {
                        string itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec)
                                              .Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression(item.ItemType, true));
                        itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting))
                                       .Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));

                        string quantityValidationMessage = AppLogic.GetString("common.cs.22");
                        if (AppLogic.IsAllowFractional)
                        {
                            quantityValidationMessage += String.Format("\\n" + AppLogic.GetString("common.cs.26"), AppLogic.InventoryDecimalPlacesPreference.ToString());
                        }
                        itemJS = itemJS.Replace("%ALLOW_FRACTIONAL_MSG_INPUT%", quantityValidationMessage);

                        html.Append(itemJS.Replace("%SKU%", item.m_ShoppingCartRecordID.ToString()));
                    }
                   
                }
                else
                {
                
                    string itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec)
                                            .Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression("Bundle", true));
                    itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting))
                                   .Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));

                    string quantityValidationMessage = AppLogic.GetString("common.cs.22");
                    if (AppLogic.IsAllowFractional)
                    {
                        quantityValidationMessage += String.Format("\\n" + AppLogic.GetString("common.cs.26"), AppLogic.InventoryDecimalPlacesPreference.ToString());
                    }
                    itemJS = itemJS.Replace("%ALLOW_FRACTIONAL_MSG_INPUT%", quantityValidationMessage);

                    html.Append(itemJS.Replace("%SKU%", cartItem.Key));

                    html.Replace(string.Format("theForm.MinOrderQuantity_{0}.value", cartItem.Key), string.Format("$('form#CartForm input#MinOrderQuantity_{0}')[0].value", cartItem.Key))
                        .Replace(string.Format("theForm.Quantity_{0}", cartItem.Key), string.Format("$('form#CartForm input#Quantity_{0}')[0]", cartItem.Key));
                }
                //theForm.MinOrderQuantity_2363.value
                // theForm.Quantity_2363.focus(); 
            }
            
       
            html.Append("return(true);\n");
            html.Append("}\n");
            html.Append("function imposeMaxLength(theControl, maxLength)\n");
            html.Append("{\n");
            html.Append("theControl.value = theControl.value.substring(0, maxLength);\n");
            html.Append("}\n");
            html.Append("</script>\n");

            string x = ThisCustomer.LocaleSetting;
            ValidationScript.Text = html.ToString();
        }

        public string GetUpsellProducts(ShoppingCart cart)
        {
            var UpsellProductList = new StringBuilder();
            var results = new StringBuilder();

            if (AppLogic.AppConfigBool("ShowAccessoryProductsOnCartPage"))
            {
                string S = String.Empty;
                try
                {
                    int upsellProductLimit = AppLogic.AppConfigUSInt("AccessoryProductsLimitNumberOnCart");
                    if (upsellProductLimit == 0)
                    {
                        upsellProductLimit = 10;
                    }
                    S = InterpriseHelper.ShowInventoryAccessoryOptions(String.Empty, true, upsellProductLimit, String.Empty, ThisCustomer, false, false, InterpriseHelper.ViewingPage.ShoppingCart);
                }
                catch { }
                if (S.Length != 0)
                {
                    results.Append("<br/>");
                    results.Append("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
                    results.Append("<tr><td align=\"left\" valign=\"top\">\n");
                    results.Append("<span class=\"UpsellSectionLabel\"> " + AppLogic.GetString("shoppingcart.aspx.19") + " </span>");
                    results.Append("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
                    results.Append("<tr><td align=\"left\" valign=\"top\">\n");
                    results.Append(S);
                    results.Append("</td></tr>\n");
                    results.Append("</table>\n");
                    results.Append("</td></tr>\n");
                    results.Append("</table>\n");
                }

            }
            return results.ToString();
        }

        private void DeleteOutofStockPhasedOutItem()
        {
            if (_cart.IsEmpty()) return;
            
            var lst = _cart.CartItems.Where(item => item.Status == "P" && item.IsOutOfStock)
                                     .Select(item => item.Id.ToString());
            _shoppingCartService.ClearLineItemsAndKitComposition(lst.ToArray());
        }

        private void DeleteOutofStockAndWithoutOpenPOItem()
        {
            if (_cart.IsEmpty()) return;

            string[] poStatus = { "Open".ToLowerInvariant(), "Partial".ToLowerInvariant() };

            _shoppingCartService.ClearLineItemsAndKitComposition(_cart.CartItems.Where(item => item.IsOutOfStock && !poStatus.Contains(item.POStatus.ToLowerInvariant()))
                                                                                .Select(item => item.Id.ToString()).ToArray());
        }

        private void UpdateSelectedShippingMethod()
        {
            string selectedShippingMethod = CommonLogic.CookieCanBeDangerousContent("selectedSM", false);

            if (!string.IsNullOrEmpty(selectedShippingMethod))
            { 
                _cart.SetCartShippingMethod(selectedShippingMethod);
            }
        }

        private void ProcessCart(bool DoingFullCheckout)
        {
            this.PageNoCache();

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;

            if ("CartType".ToQueryString().Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }

            _cart = _shoppingCartService.New(cte, true);

            if (!Page.IsPostBack)
            {
                string couponCode = String.Empty;
                if (_cart.HasCoupon(ref couponCode))
                {
                    CouponCode.Text = couponCode;
                }
            }
            else
            {
                if (CouponCode.Text.IsNullOrEmptyTrimmed())
                {
                    _cart.ClearCoupon();
                }
            }

            UpdateSelectedShippingMethod();

            // check if credit on hold
            if (ThisCustomer.IsCreditOnHold) { _navigationService.NavigateToShoppingCart(); }

            if (_cart.IsEmpty())
            {
                // can't have this at this point:
                switch (cte)
                {
                    case CartTypeEnum.ShoppingCart:
                        _navigationService.NavigateToShoppingCart();
                        break;
                    case CartTypeEnum.WishCart:
                        _navigationService.NavigateToWishList();
                        break;
                    case CartTypeEnum.GiftRegistryCart:
                        _navigationService.NavigateToGiftRegistry();
                        break;
                    default:
                        _navigationService.NavigateToShoppingCart();
                        break;
                }
            }
            

            // update cart quantities:
            List<KeyValuePair<int, string>> cartIdsAndunitMeasureCodes = new List<KeyValuePair<int,string>>();
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                string fld = Request.Form.Keys[i];
                string fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                string quantity;
                if (fld.StartsWith("Quantity"))
                {
                    if (fldval.StartsWith(Localization.GetNumberDecimalSeparatorLocaleString(_cart.ThisCustomer.LocaleSetting)))
                    {
                        fldval = fldval.Insert(0, Localization.GetNumberZeroLocaleString(_cart.ThisCustomer.LocaleSetting));
                    }
                    if (Regex.IsMatch(fldval, AppLogic.AllowedQuantityWithDecimalRegEx(_cart.ThisCustomer.LocaleSetting), RegexOptions.Compiled))
                    {
                        quantity = fldval;

                        string bundleCode = string.Empty;
                        int bundleHeaderID = 0;
                        if (fld.StartsWith("Quantity_ITEM-"))
                        {
                            bundleCode = fld.Remove(0, "Quantity_".Length);
                            bundleHeaderID = int.Parse(bundleCode.Split('_')[1]);
                            bundleCode = bundleCode.Split('_')[0];
                            var productItem = _productService.GetInventoryItem(bundleCode);
                            recID = productItem.Counter;
                          
                        }
                        else
                        {
                            recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                        }
                      
                      
                        decimal iquan = Convert.ToDecimal(quantity);

                        //check if gift registry item exceeds the maximum No.# of IN-NEED quantity
                        decimal? regItemQty = GiftRegistryDA.GetGiftRegistryItemQuantityByCartRecID(recID);
                        if ((regItemQty.HasValue && regItemQty > 0) && iquan > regItemQty)
                        {
                            WriteError(AppLogic.GetString("editgiftregistry.error.14", true));
                        }
                        else if (regItemQty.HasValue && regItemQty == 0) // blocks the user from
                        {
                            WriteError(AppLogic.GetString("editgiftregistry.error.15", true));
                        }
                        else
                        {
                            if (iquan < 0) { iquan = 0; }
                            if (string.IsNullOrEmpty(bundleCode))
                            {
                                _cart.SetItemQuantity(recID, iquan, bundleCode);
                            }
                            else
                            {
                                var bundleItems = _cart.CartItems.Where(item => item.BundleCode == bundleCode && item.BundleHeaderID == bundleHeaderID);
                                int[] shoppingCartRecordIdDelete = bundleItems.Select(item => item.m_ShoppingCartRecordID).ToArray();
                                for (int deleteIndex = 0; deleteIndex < shoppingCartRecordIdDelete.Length; deleteIndex++)
                                {
                                    _cart.SetItemQuantity(shoppingCartRecordIdDelete[deleteIndex], iquan, bundleCode, bundleHeaderID);
                                }
                            }
                        }
                    }
                    else
                    {
                        WriteError("The item quantity must have a valid input.");
                    }
                }

                if (fld.StartsWith("UnitMeasureCode"))
                {
                    if (!fldval.IsNullOrEmptyTrimmed())
                    {
                        recID = Localization.ParseUSInt(fld.Substring("UnitMeasureCode".Length + 1));
                        string unitMeasureCode = fldval.ToHtmlDecode();
                        cartIdsAndunitMeasureCodes.Add(new KeyValuePair<int,string>(recID, unitMeasureCode));
                    }
                }

            }
            
            //update unitMeasureCode
            if (cartIdsAndunitMeasureCodes.Count > 0) { 
                bool hasRegistryItem =  _cart.HasRegistryItems();
                bool isMultiShip = _cart.HasMultipleShippingAddresses();
                if (!AppLogic.AppConfigBool("AllowMultipleShippingAddressPerOrder")) { isMultiShip = false;}
                _cart.BatchUpdateUnitMeasureForItem(cartIdsAndunitMeasureCodes, isMultiShip);
            }

            // merge duplicate item/s
            _shoppingCartService.MergeDuplicateCartItems(_cart);

            // save coupon code, no need to reload cart object
            // will update customer record also:
            if (cte == CartTypeEnum.ShoppingCart)
            {
                if (!string.IsNullOrEmpty(CouponCode.Text))
                {
                    string errorMessage = string.Empty;
                    if (_cart.IsCouponValid(ThisCustomer, CouponCode.Text, ref errorMessage))
                    {
                        _cart.ApplyCoupon(CouponCode.Text);
                    }
                    else
                    {
                        // NULL out the coupon for this cusotmer...
                        string customerCode = CommonLogic.IIF(ThisCustomer.IsRegistered, ThisCustomer.CustomerCode, ThisCustomer.CustomerID);
                        InterpriseHelper.ClearCustomerCoupon(customerCode, ThisCustomer.IsRegistered);

                         
                        WriteError(errorMessage);
                        CouponCode.Text = string.Empty;

                        //rebiuld the shopping for the renderer computation
                        //if not rebuild cart.SalesOrderDataset.TransactionItemTaxDetailView will be null.
                        _cart.BuildSalesOrderDetails(false, true, CouponCode.Text,true);
                        return;
                    }
                }

                CheckUpSell();

                bool hasCheckedOptions = false;

                if (pnlOrderOptions.Visible)
                {
                    // Process the Order Options
                    foreach (RepeaterItem ri in OrderOptionsList.Items)
                    {
                        var cbk = (DataCheckBox)ri.FindControl("OrderOptions");
                        if (cbk.Checked)
                        {
                            hasCheckedOptions = true;
                            string itemCode = (string)cbk.Data;
                            var hfCounter = ri.FindControl("hfItemCounter") as HiddenField;
                            var txtNotes = ri.FindControl("txtOrderOptionNotes") as TextBox;

                            string strNotes = txtNotes.Text.ToHtmlEncode();
                            string notes = CommonLogic.IIF((strNotes != null), CommonLogic.CleanLevelOne(strNotes), string.Empty);

                            //check the length of order option notes
                            //should not exceed 1000 characters including spaces
                            int maxLen = 1000;
                            if (notes.Length > maxLen)
                            {
                                notes = notes.Substring(0, maxLen);
                            }

                            string unitMeasureCode = string.Empty;

                            // check if the item has only 1 unit measure
                            // hence it's rendered as a label
                            // else it would be rendered as a drop down list
                            var lblUnitMeasureCode = ri.FindControl("lblUnitMeasureCode") as Label;
                            if (null != lblUnitMeasureCode && lblUnitMeasureCode.Visible)
                            {
                                unitMeasureCode = lblUnitMeasureCode.Text;
                            }
                            else
                            {
                                // it's rendered as combobox because the item has multiple unit measures configured
                                var cboUnitMeasureCode = ri.FindControl("cboUnitMeasureCode") as DropDownList;
                                if (null != cboUnitMeasureCode && cboUnitMeasureCode.Visible)
                                {
                                    unitMeasureCode = cboUnitMeasureCode.SelectedValue;
                                }
                            }

                            if (unitMeasureCode.IsNullOrEmptyTrimmed())
                            {
                                throw new ArgumentException("Unit Measure not specified!!!");
                            }

                            //check if this Order Option has Restricted Quantity and Minimum Order Qty set.
                            decimal itemQuantity = 1;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT iw.RestrictedQuantity, iw.MinOrderQuantity FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemWebOption iw with (NOLOCK) ON i.ItemCode = iw.ItemCode AND iw.WebsiteCode = {0} WHERE i.ItemCode = {1}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode), DB.SQuote(itemCode)))
                                {
                                    if (reader.Read())
                                    {
                                        string restrictedQuantitiesValue = DB.RSField(reader, "RestrictedQuantity");
                                        decimal minimumOrderQuantity = Convert.ToDecimal(DB.RSFieldDecimal(reader, "MinOrderQuantity"));
                                        if (!CommonLogic.IsStringNullOrEmpty(restrictedQuantitiesValue))
                                        {
                                            string[] quantityValues = restrictedQuantitiesValue.Split(',');
                                            if (quantityValues.Length > 0)
                                            {
                                                int ctr = 0;
                                                bool loop = true;
                                                while (loop)
                                                {
                                                    int quantity = 0;
                                                    string quantityValue = quantityValues[ctr];
                                                    if (int.TryParse(quantityValue, out quantity))
                                                    {
                                                        if (quantity >= minimumOrderQuantity)
                                                        {
                                                            itemQuantity = quantity;
                                                            loop = false;
                                                        }
                                                    }
                                                    ctr++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (minimumOrderQuantity > 0)
                                            {
                                                itemQuantity = minimumOrderQuantity;
                                            }
                                        }
                                    }
                                }
                            }
                            // Add the selected Order Option....
                            Guid cartItemId = Guid.Empty;
                            cartItemId = _cart.AddItem(ThisCustomer, ThisCustomer.PrimaryShippingAddressID, itemCode, int.Parse(hfCounter.Value), itemQuantity, unitMeasureCode, CartTypeEnum.ShoppingCart,notes);
                        }
                    }
                }

                if (hasCheckedOptions)
                {
                    //refresh the option items
                    RenderOrderOptions();
                }

                if (ThisCustomer.IsRegistered)
                {
                    string sOrderNotes = CommonLogic.CleanLevelOne(OrderNotes.Text);
                    //check the length of order notes
                    //should not exceed 255 characters including spaces
                    if (sOrderNotes.Length > DomainConstants.ORDER_NOTE_MAX_LENGTH)
                    {
                        sOrderNotes = sOrderNotes.Substring(0, DomainConstants.ORDER_NOTE_MAX_LENGTH);
                    }

                    DB.ExecuteSQL(
                        String.Format("UPDATE Customer SET Notes = {0} WHERE CustomerCode = {1}",
                        sOrderNotes.ToDbQuote(),
                        ThisCustomer.CustomerCode.ToDbQuote())
                    );
                }

            }
            bool validated = true;
            if (_cart.InventoryTrimmed)
            {
                // inventory got adjusted, send them back to the cart page to confirm the new values!

                WriteError(AppLogic.GetString("shoppingcart.cs.43", true).ToUrlDecode());
                validated = false;
            }

            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);

            if (AppLogic.AppConfigBool("ShowShipDateInCart") && AppLogic.AppConfigBool("ShowStockHints"))
            {
                _cart.BuildSalesOrderDetails(false, true, CouponCode.Text,true);
            }

            if (cte == CartTypeEnum.WishCart)
            {
                _navigationService.NavigateToWishList();
            }

            if (cte == CartTypeEnum.GiftRegistryCart && DoingFullCheckout)
            {
                _navigationService.NavigateToGiftRegistry();
            }

            if ((_cart.HasOverSizedItemWithPickupShippingMethod() || _cart.HasPickupItem()) && DoingFullCheckout)
            {
                _navigationService.NavigateToCheckOutStore();
            }

            if (DoingFullCheckout)
            {

                if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    validated = false;
                }

                if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    validated = false;
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    validated = false;
                }

                if (ThisCustomer.IsRegistered && AppLogic.AppConfigBool("Checkout.UseOnePageCheckout") &&
                    !_cart.HasMultipleShippingAddresses() && !_cart.HasRegistryItems())
                {
                    _navigationService.NavigateToCheckout1();
                }

                if (validated)
                {
                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed())) // || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=true&addressType=Billing");
                    }

                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed())) //  || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Shipping");
                    }
                    if (AppLogic.EnableAdvancedFreightRateCalculation() && _cart.HasShippableComponents() && !(_cart.HasHazardousItem() && AppLogic.ApplyHazardousShipping()))
                    {
                        Response.Redirect("shipping.aspx");
                    }
                    if (ThisCustomer.IsNotRegistered || ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed() || ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed() || !ThisCustomer.HasAtLeastOneAddress())
                    {
                        Response.Redirect("checkoutanon.aspx?checkout=true");
                    }
                    else
                    {
                        if (_cart.HasGiftItems())
                        {
                            Response.Redirect("checkoutgiftemail.aspx");
                        }

                        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents())
                        {
                            _cart.MakeShippingNotRequired();
                            _navigationService.NavigateToCheckOutPayment();
                        }
                        if ((_cart.HasMultipleShippingAddresses() && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.CartAllowsShippingMethodSelection) || _cart.HasRegistryItems())
                        {
                            _navigationService.NavigateToCheckoutMult();
                        }
                        else
                        {
                            _navigationService.NavigateToCheckoutShipping();
                        }
                    }
                }
                InitializePageContent();
            }
        }
        private void ProcessWishlist()
        {
            this.PageNoCache();

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;

            if ("CartType".ToQueryString().Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }

            _cart = _shoppingCartService.New(cte, true);

            if (!Page.IsPostBack)
            {
                string couponCode = String.Empty;
                if (_cart.HasCoupon(ref couponCode))
                {
                    CouponCode.Text = couponCode;
                }
            }
            else
            {
                if (CouponCode.Text.IsNullOrEmptyTrimmed())
                {
                    _cart.ClearCoupon();
                }
            }

            // check if credit on hold
            if (ThisCustomer.IsCreditOnHold) { _navigationService.NavigateToShoppingCart(); }

            if (_cart.IsEmpty())
            {
                // can't have this at this point:
                switch (cte)
                {
                    case CartTypeEnum.ShoppingCart:
                        _navigationService.NavigateToShoppingCart();
                        break;
                    case CartTypeEnum.WishCart:
                        _navigationService.NavigateToWishList();
                        break;
                    case CartTypeEnum.GiftRegistryCart:
                        _navigationService.NavigateToGiftRegistry();
                        break;
                    default:
                        _navigationService.NavigateToShoppingCart();
                        break;
                }
            }

            // update cart quantities:
            List<KeyValuePair<int, string>> cartIdsAndunitMeasureCodes = new List<KeyValuePair<int, string>>();
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                string fld = Request.Form.Keys[i];
                string fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                if (fld.StartsWith("bt_wishlist_"))
                {
                    recID = Localization.ParseUSInt(fld.Substring("bt_wishlist_".Length));
                    _cart.SetItemCartType(recID, CartTypeEnum.WishCart);
                }

            }
            
        }

        private void CheckUpSell()
        {
            // check for upsell products
            if (CommonLogic.FormCanBeDangerousContent("Upsell").Length != 0)
            {
                foreach (string s in CommonLogic.FormCanBeDangerousContent("Upsell").Split(','))
                {
                    int ProductID = Localization.ParseUSInt(s);
                    if (ProductID == 0) { continue; }

                    string itemCode = InterpriseHelper.GetInventoryItemCode(ProductID);
                    string shippingAddressID = ThisCustomer.IsNotRegistered ? String.Empty : ThisCustomer.PrimaryShippingAddressID;

                    var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);
                    _cart.AddItem(ThisCustomer, shippingAddressID, itemCode, ProductID, 1, umInfo.Code, CartTypeEnum.ShoppingCart);
                }
            }
        }

        private void WriteError(string error)
        {
            ErrorMsgLabel.Text += error;
            pnlErrorMsg.Visible = !string.IsNullOrEmpty(ErrorMsgLabel.Text.Trim());
        }

        private void ClearErrors()
        {
            CouponError.Text = String.Empty;
            ErrorMsgLabel.Text = String.Empty;
            RemovePhasedOutItemWithNoStockError.Text = String.Empty;
            InventoryTrimmedError.Text = String.Empty;
            MinimumQuantitiesUpdatedError.Text = String.Empty;
            MeetsMinimumOrderAmountError.Text = String.Empty;
            MeetsMinimumOrderQuantityError.Text = String.Empty;
            Micropay_EnabledError.Text = String.Empty;
        }

        private void ContinueShopping()
        {
            if (AppLogic.AppConfig("ContinueShoppingURL").IsNullOrEmptyTrimmed())
            {
                string returnUrl = ReturnUrl;
                if (returnUrl.ToString().IsNullOrEmptyTrimmed())
                {
                    _navigationService.NavigateToDefaultPage();
                }
                else
                {
                    _navigationService.NavigateToUrl(returnUrl);
                }
            }
            else
            {
                _navigationService.NavigateToUrl(AppLogic.AppConfig("ContinueShoppingURL"));
            }
        }

        private void DoOrderNotesChecking()
        {
            if (!_appConfigService.DisallowOrderNotes && ThisCustomer.IsRegistered)
            {
                OrderNotes.Text = _cart.OrderNotes;
                pnlOrderNotes.Visible = true;
            }
            else
            {
                pnlOrderNotes.Visible = false;
            }
        }

        #endregion

        #region Properties

        private string ReturnUrl 
        {
            get 
            {
                return "ReturnUrl".ToQueryString();
            } 
        }

        public string SkinImagePath 
        {
            get 
            {
                return "skins/skin_" + SkinID.ToString() + "/images/";
            }
        }

        #endregion
    }
}