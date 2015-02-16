// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DTO; 

namespace InterpriseSuiteEcommerce.mobile
{
    public partial class ShoppingCartPage : SkinBase
    {
        string SkinImagePath = String.Empty;
        InterpriseShoppingCart cart = null;
        bool RedirectToShoppingCart = false;

        #region DomainServices

        IAppConfigService _appConfigService = null;
        IStringResourceService _stringResourceService = null;
        IShoppingCartService _shoppingCartService = null;

        #endregion

        private void RedirectToSignInPage()
        {
            // disable all buttons
            btnCheckOutNowBottom.Enabled = false;
            btnCheckOutNowTop.Enabled = false;
            btnContinueShoppingTop.Enabled = false;
            btnUpdateCart1.Enabled = false;
            btnUpdateCart2.Enabled = false;
            btnUpdateCart3.Enabled = false;
            btnUpdateCart4.Enabled = false;

            BodyPanel.Visible = false;

            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011");

            // perform redirect
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", "signin.aspx".ToUrlDecode()));
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            SetCustomerSkinID();

            
            this.RequireCustomerRecord();
            RequireSecurePage();
            SectionTitle = AppLogic.GetString("AppConfig.CartPrompt");
            ClearErrors();

            if (!this.IsPostBack)
            {
                string returnurl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
                if (returnurl.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }
                ViewState["returnurl"] = returnurl;
                InitializePageContent();
            }

			//for optimization
            string[] formkeys = Request.Form.AllKeys;
            if (formkeys.Any(k => k.Contains("bt_Delete")))
            {
                ProcessCart(false);
                ProcessDelete();
                RenderOrderOptions();
            }
            InitializePageContent();

            //foreach (string s in formkeys)
            //{
            //    if (s != "bt_Delete") { continue; }
            //    ProcessCart(false);
            //    InitializePageContent();
            //}

            //Check if alternate checkout methods are supported (PayPal)
            if (AppLogic.IsSupportedAlternateCheckout)
            {
                //note false just to disable the section
				//modified for mobile design
                AlternativeCheckouts.Visible = true;

				//modified for mobile design

                bool hidePaypalOptionIfMultiShip = !(cart.HasMultipleShippingAddresses());

                if (AppLogic.AppConfigBool("PayPalCheckout.ShowOnCartPage") && hidePaypalOptionIfMultiShip) 
                {
                    PayPalExpressSpan.Visible = true; 
                }
             }
    		
			//modified for mobile design
            //if no alternative methods are visible, hide the whole row
            AlternativeCheckouts.Visible = PayPalExpressSpan.Visible;
            if (!AppLogic.IsSupportedAlternateCheckout && AlternativeCheckouts.Visible == true)
            {
                ErrorMsgLabel.Text = PayPalExpress.ErrorMsg;
                AlternativeCheckouts.Visible = false;
            }

            HeaderMsg.SetContext = this;
            CartPageFooterTopic.SetContext = this;
        }

		//added for mobile design
        private void ProcessDelete()
        {
            string[] formkeys = Request.Form.AllKeys;
            var btnKeyDelete = formkeys.FirstOrDefault(k => k.Contains("bt_Delete"));

            if (string.IsNullOrEmpty(btnKeyDelete)) return;
            
            int recId = btnKeyDelete.Substring(btnKeyDelete.LastIndexOf("_") + 1,
                        (btnKeyDelete.Length - btnKeyDelete.LastIndexOf("_") - 1)).TryParseIntUsLocalization().Value;

            cart.SetItemQuantity(recId, 0);
        }

        private void SetCustomerSkinID()
        {
            var principal = ((InterpriseSuiteEcommercePrincipal)Context.User);
            principal.ThisCustomer.SkinID = SkinID;
            Context.User = principal;
        }

        #region Web Form Designer generated code

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeDomainServices();
            InitializeShoppingCart();
            InitializeComponent();
            RenderOrderOptions();
            DoOrderNotesChecking();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			//modified for mobile design and new mobile resources
            string continueCaption = AppLogic.GetString("shoppingcart.cs.12");
            btnContinueShoppingTop.Text = continueCaption;

            string checkoutButtonCaption = AppLogic.GetString("shoppingcart.cs.34");
            btnCheckOutNowTop.Text = checkoutButtonCaption;
            btnCheckOutNowBottom.Text = checkoutButtonCaption;

            string caption = AppLogic.GetString("shoppingcart.cs.33");
            btnUpdateCart1.Text = caption;
            btnUpdateCart2.Text = caption;
            btnUpdateCart3.Text = caption;
            btnUpdateCart4.Text = caption;
            
            OrderOptionsList.ItemDataBound += OrderOptionsList_ItemDataBound;
            btnContinueShoppingTop.Click += btnContinueShoppingTop_Click;
            btnCheckOutNowTop.Click += btnCheckOutNowTop_Click;
            btnCheckOutNowBottom.Click += btnCheckOutNowTop_Click;
            btnUpdateCart1.Click += btnUpdateCart1_Click;
            btnUpdateCart2.Click += btnUpdateCart1_Click;
            btnUpdateCart3.Click += btnUpdateCart1_Click;
            btnUpdateCart4.Click += btnUpdateCart1_Click;

            SkinImagePath = "skins/skin_{0}/images/".FormatWith(SkinID.ToString());
        }

        #endregion
        
        public void InitializePageContent()
        {
            int AgeCartDays = AppLogic.AppConfigUSInt("AgeCartDays");
            if (AgeCartDays == 0)
            {
                AgeCartDays = 7;
            }

            ShoppingCart.Age(AgeCartDays, CartTypeEnum.ShoppingCart);
            shoppingcartaspx8.Text = AppLogic.GetString("shoppingcart.aspx.6");
            shoppingcartaspx10.Text = AppLogic.GetString("shoppingcart.aspx.8");
            shoppingcartaspx11.Text = AppLogic.GetString("shoppingcart.aspx.9");
            shoppingcartaspx9.Text = AppLogic.GetString("shoppingcart.aspx.7");
            shoppingcartcs27.Text = AppLogic.GetString("shoppingcart.cs.5");
            shoppingcartcs28.Text = AppLogic.GetString("shoppingcart.cs.6");
            shoppingcartcs29.Text = AppLogic.GetString("shoppingcart.cs.7");
            shoppingcartcs31.Text = AppLogic.GetString("shoppingcart.cs.9");

            lblOrderNotes.Text = AppLogic.GetString("shoppingcart.cs.13");
            btnContinueShoppingTop.Text = AppLogic.GetString("shoppingcart.cs.12");
            btnCheckOutNowTop.Text = AppLogic.GetString("shoppingcart.cs.34");
            btnCheckOutNowBottom.Text = AppLogic.GetString("shoppingcart.cs.34");
            OrderNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 255);");
            RedirectToShoppingCart = false;

            if (!Page.IsPostBack)
            {
                if (cart.HasRegistryItems())
                {
                    cart.RemoveRegistryItems();
                    ErrorMsgLabel.Text = AppLogic.GetString("mobile.shoppingcart.error.1").ToHtmlDecode();
                    ErrorMsgLabel.Visible = true;
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (cart.HasCoupon(ref couponCode) &&
                    cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    CouponCode.Text = couponCode;
                }
                else
                {
                    if(!couponErrorMessage.IsNullOrEmptyTrimmed())
                    {
                        ErrorMsgLabel.Text = couponErrorMessage.ToHtmlDecode();
                    }
                    cart.ClearCoupon();
                }

                //check customer IsCreditHold

                if (ThisCustomer.IsCreditOnHold && cart != null)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("shoppingcart.aspx.18");
                    cart.ClearCoupon();
                    RedirectToShoppingCart = true;
                }
                else
                {
                    if (AppLogic.AppConfigBool("ShowShipDateInCart") && AppLogic.AppConfigBool("ShowStockHints") && cart != null)
                    {
                        cart.BuildSalesOrderDetails();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(CouponCode.Text)) cart.ClearCoupon();
            }

            if (cart.IsEmpty())
            {
                btnUpdateCart1.Visible = false;
                AlternativeCheckouts.Visible = false;
            }
            else
            {
                cart.BuildSalesOrderDetails();
            }

            string BACKURL = AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting);
            var html = new StringBuilder("");
            html.Append("<script type=\"text/javascript\" >\n");
            html.Append("function Cart_Validator(theForm)\n");
            html.Append("{\n");
            string cartJS = CommonLogic.ReadFile("js/shoppingcart.js", true);
            foreach (var c in cart.CartItems)
            {
                string itemJS = string.Empty;

                itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression(c.ItemType, true));
                itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting)).Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));
                html.Append(itemJS.Replace("%SKU%", c.m_ShoppingCartRecordID.ToString()));
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
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            
            string XmlPackageName = AppLogic.AppConfig("XmlPackage.ShoppingCartPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_ShoppingCartPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            string XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            redarrow3.ImageUrl = XRI;
            redarrow4.ImageUrl = XRI;

            ShippingInformation.Visible = (!AppLogic.AppConfigBool("SkipShippingOnCheckout"));
            AddresBookLlink.Visible = (ThisCustomer.IsRegistered);

            btnCheckOutNowTop.Visible = (!cart.IsEmpty());

            if (!IsPostBack)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length != 0 || ErrorMsgLabel.Text.Length > 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        throw new ArgumentException("SECURITY EXCEPTION");
                    }
                    pnlErrorMsg.Visible = true;

                    string errorMsg = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").ToHtmlEncode();
                    if (ErrorMsgLabel.Text != errorMsg)
                    {
                        ErrorMsgLabel.Text += errorMsg;
                    }
                }
            }

            if (cart.InventoryTrimmed)
            {
                pnlInventoryTrimmedError.Visible = true;
                InventoryTrimmedError.Text = AppLogic.GetString("shoppingcart.aspx.1");
            }

            if (cart.MinimumQuantitiesUpdated)
            {
                pnlMinimumQuantitiesUpdatedError.Visible = true;
                MinimumQuantitiesUpdatedError.Text = AppLogic.GetString("shoppingcart.aspx.5");
            }

            if (cart.HasNoStockPhasedOutItem)
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

            if (cart.HaNoStockAndNoOpenPOItem)
            {
                pnlRemovePhasedOutItemWithNoStockError.Visible = true;
                RemovePhasedOutItemWithNoStockError.Text = _stringResourceService.GetString("shoppingcart.aspx.48");
            }

            Decimal MinOrderAmount = AppLogic.AppConfigUSDecimal("CartMinOrderAmount");
            if (!cart.MeetsMinimumOrderAmount(MinOrderAmount))
            {
                pnlMeetsMinimumOrderAmountError.Visible = true;
                string amountFormatted = MinOrderAmount.ToCustomerCurrency();
                MeetsMinimumOrderAmountError.Text = AppLogic.GetString("shoppingcart.aspx.2").FormatWith(amountFormatted);
            }

            int quantityDecimalPlaces = InterpriseHelper.GetInventoryDecimalPlacesPreference();

            var formatter = (new CultureInfo(ThisCustomer.LocaleSetting)).NumberFormat;

            // setup the formatter
            formatter.NumberDecimalDigits = quantityDecimalPlaces;
            formatter.PercentDecimalDigits = quantityDecimalPlaces;

            MeetsMinimumOrderQuantityError.Text = string.Empty;
            decimal MinQuantity = AppLogic.AppConfigUSDecimal("MinCartItemsBeforeCheckout");
            if (!cart.MeetsMinimumOrderQuantity(MinQuantity))
            {
                pnlMeetsMinimumOrderQuantityError.Visible = true;
                MeetsMinimumOrderQuantityError.Text = String.Format(AppLogic.GetString("shoppingcart.aspx.16"), MinQuantity.ToString(), MinQuantity.ToString());
            }
            
            //ShoppingCartGif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCart.gif");
            CartItems.Text = cart.RenderHTMLLiteral(new MobileShoppingCartPageLiteralRenderer());
            //CartSubTotal.Text = cart.RenderHTMLLiteral(new ShoppingCartPageSummaryLiteralRenderer());

            if (!cart.IsEmpty())
            {
                //ShoppingCartorderoptions_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartorderoptions.gif");
                string strXml = String.Empty;
                pnlErrorMsg.Visible = true;

                if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsRegistered && !ThisCustomer.IsOver13)
                {
                    btnCheckOutNowTop.Enabled = false;
                    btnCheckOutNowBottom.Enabled = false;
                    ErrorMsgLabel.Text = AppLogic.GetString("over13oncheckout");
                    return;
                }

                btnCheckOutNowBottom.Enabled = btnCheckOutNowTop.Enabled;

                DisplayUpsellProducts(cart);

                if (cart.CouponsAllowed)
                {
                    pnlCoupon.Visible = true;
                }
                else
                {
                    pnlCoupon.Visible = false;
                }

                btnCheckOutNowBottom.Visible = true;

                if (ThisCustomer.IsNotRegistered)
                {
                    pnlCoupon.Visible = false;
                    pnlOrderNotes.Visible = false;
                }
            }
            else
            {
                pnlOrderOptions.Visible = false;
                pnlUpsellProducts.Visible = false;
                pnlCoupon.Visible = false;
                pnlOrderNotes.Visible = false;
            }
            btnContinueShoppingTop.OnClientClick = "self.location='" + BACKURL + "'";
            CartPageFooterTopic.SetContext = this;
            String XmlPackageName2 = AppLogic.AppConfig("XmlPackage.ShoppingCartPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_ShoppingCartPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }            
        }

        private void InitializeShoppingCart()
        {
            if (cart != null) return;
            ThisCustomer.ThisCustomerSession.ClearVal(DomainConstants.CLEAR_COUPON_DISCOUNT);
            ThisCustomer.ThisCustomerSession.ClearVal(DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS);
            cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
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

        public void DisplayUpsellProducts(ShoppingCart cart)
        {
            // ----------------------------------------------------------------------------------------
            // WRITE OUT UPSELL PRODUCTS:
            // ----------------------------------------------------------------------------------------
            if (!AppLogic.AppConfigBool("ShowAccessoryProductsOnCartPage")) return;
            
            string accessoriesOptionText = string.Empty;
            try
            {
                int upsellProductLimit = AppLogic.AppConfigUSInt("AccessoryProductsLimitNumberOnCart");
                if (upsellProductLimit == 0)
                {
                    upsellProductLimit = 10;
                }

                string shoppingCartAccessoryHelperTemplate = "helper.product.xml.config";
                accessoriesOptionText = InterpriseHelper.ShowInventoryAccessoryOptions(string.Empty, true, upsellProductLimit, string.Empty, ThisCustomer, false, false, InterpriseHelper.ViewingPage.ShoppingCart, shoppingCartAccessoryHelperTemplate);
            }
            catch { }

            if (accessoriesOptionText.Length != 0)  
            {
                accessoriesOptions.Text = accessoriesOptionText;
                pnlUpsellProducts.Visible = true;
            }
            else
            {
                pnlUpsellProducts.Visible = false;
            }
        }

        public void ProcessCart(bool DoingFullCheckout)
        {
         
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;
            if (CommonLogic.QueryStringCanBeDangerousContent("CartType").Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }
            cart = _shoppingCartService.New(cte, true);

            if (!Page.IsPostBack)
            {
                string couponCode = string.Empty;
                if (cart.HasCoupon(ref couponCode))
                    CouponCode.Text = couponCode;
            }
            else
            {
                if (string.IsNullOrEmpty(CouponCode.Text))
                    cart.ClearCoupon();
            }

            // check if credit on hold
            if (ThisCustomer.IsCreditOnHold) { Response.Redirect("shoppingcart.aspx"); }

            if (cart.IsEmpty())
            {
                // can't have this at this point:
                switch (cte)
                {
                    case CartTypeEnum.ShoppingCart:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                    case CartTypeEnum.WishCart:
                        Response.Redirect("wishlist.aspx");
                        break;
                    case CartTypeEnum.GiftRegistryCart:
                        Response.Redirect("giftregistry.aspx");
                        break;
                    default:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                }
            }
			
			//Make it a method
            UpdateCartItems();

            // merge duplicate item/s
            _shoppingCartService.MergeDuplicateCartItems(cart);

            // save coupon code, no need to reload cart object
            // will update customer record also:
            if (cte == CartTypeEnum.ShoppingCart)
            {
                if (!string.IsNullOrEmpty(CouponCode.Text))
                {
                    string errorMessage = string.Empty;
                    if (cart.IsCouponValid(ThisCustomer, CouponCode.Text, ref errorMessage))
                    {
                        cart.ApplyCoupon(CouponCode.Text);
                    }
                    else
                    {
                        // NULL out the coupon for this cusotmer...
                        InterpriseHelper.ClearCustomerCoupon(ThisCustomer.CustomerCode, ThisCustomer.IsRegistered);

                        ErrorMsgLabel.Text = errorMessage;
                        CouponCode.Text = string.Empty;
                        return;
                    }
                }

                // check for upsell products
                if (CommonLogic.FormCanBeDangerousContent("Upsell").Length != 0)
                {
                    foreach (string s in CommonLogic.FormCanBeDangerousContent("Upsell").Split(','))
                    {
                        int ProductID = Localization.ParseUSInt(s);
                        if (ProductID == 0) { continue; }

                        string itemCode = InterpriseHelper.GetInventoryItemCode(ProductID);
                        string shippingAddressID;

                        shippingAddressID = CommonLogic.IIF(ThisCustomer.IsNotRegistered, string.Empty, ThisCustomer.PrimaryShippingAddressID);

                        var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);
                        cart.AddItem(ThisCustomer, shippingAddressID, itemCode, ProductID, 1, umInfo.Code, CartTypeEnum.ShoppingCart);
                    }
                }

                bool hasCheckedOptions = false;

                if (pnlOrderOptions.Visible)
                {
                    // Process the Order Options
                    foreach (RepeaterItem ri in OrderOptionsList.Items)
                    {
                        hasCheckedOptions = true;
                        DataCheckBox cbk = (DataCheckBox)ri.FindControl("OrderOptions");
                        if (cbk.Checked)
                        {
                            string itemCode = (string)cbk.Data;
                            HiddenField hfCounter = ri.FindControl("hfItemCounter") as HiddenField;
                            TextBox txtNotes = ri.FindControl("txtOrderOptionNotes") as TextBox;
                            
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
                            Label lblUnitMeasureCode = ri.FindControl("lblUnitMeasureCode") as Label;
                            if (null != lblUnitMeasureCode && lblUnitMeasureCode.Visible)
                            {
                                unitMeasureCode = lblUnitMeasureCode.Text;
                            }
                            else
                            {
                                // it's rendered as combobox because the item has multiple unit measures configured
                                DropDownList cboUnitMeasureCode = ri.FindControl("cboUnitMeasureCode") as DropDownList;
                                if (null != cboUnitMeasureCode && cboUnitMeasureCode.Visible)
                                {
                                    unitMeasureCode = cboUnitMeasureCode.SelectedValue;
                                }
                            }

                            if (CommonLogic.IsStringNullOrEmpty(unitMeasureCode))
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
                            cart.AddItem(ThisCustomer,ThisCustomer.PrimaryShippingAddressID,itemCode,int.Parse(hfCounter.Value),itemQuantity,unitMeasureCode,CartTypeEnum.ShoppingCart, notes);
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
            if (cart.InventoryTrimmed)
            {
                // inventory got adjusted, send them back to the cart page to confirm the new values!
                ErrorMsgLabel.Text += Server.UrlDecode(AppLogic.GetString("shoppingcart.cs.43"));
                validated = false;
            }
            cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);

            if (AppLogic.AppConfigBool("ShowShipDateInCart") && AppLogic.AppConfigBool("ShowStockHints"))
            {
                cart.BuildSalesOrderDetails();
            }

            if (cte == CartTypeEnum.WishCart)
            {
                Response.Redirect("wishlist.aspx");
            }
            if (cte == CartTypeEnum.GiftRegistryCart)
            {
                Response.Redirect("giftregistry.aspx");
            }

            if (DoingFullCheckout)
            {
                
                if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    validated = false;
                }

                if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    validated = false;
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (cart.HasCoupon(ref couponCode) && !cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    validated = false;
                }

                //One page checkout is not implemented in mobile.

                //if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout") && !cart.HasMultipleShippingAddresses())
                //{
                //    Response.Redirect("checkout1.aspx");
                //}

                if (validated)
                {
                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID == string.Empty)) // || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=true&addressType=Billing");
                    }

                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryShippingAddressID == string.Empty)) //  || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Shipping");
                    }

                    if (ThisCustomer.IsNotRegistered || ThisCustomer.PrimaryBillingAddressID == string.Empty || ThisCustomer.PrimaryShippingAddressID == string.Empty || !ThisCustomer.HasAtLeastOneAddress())
                    {
                        Response.Redirect("checkoutanon.aspx?checkout=true");
                    }
                    else
                    {
                        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || 
                            !cart.HasShippableComponents())
                        {
                            cart.MakeShippingNotRequired();
                            Response.Redirect("checkoutpayment.aspx");
                        }

                        if ((cart.HasMultipleShippingAddresses() && cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && cart.CartAllowsShippingMethodSelection))
                        {
                            Response.Redirect("checkoutshippingmult.aspx");
                        }
                        else
                        {
                            Response.Redirect("checkoutshipping.aspx");
                        }
                    }
                }
                InitializePageContent();
            }
        }

        private void UpdateCartItems()
        {
            var keys = Request.Form.AllKeys;

            //Annonimous objects
            var quantityElementAndIndex = keys
                        .Where(k => k.Contains("Quantity") && !k.Contains("MinOrderQuantity"))
                        .Select((n, i) => new { Value = Request.Form[n], Id = n.Substring("Quantity".Length + 1) });

            foreach (var item in quantityElementAndIndex)
            {
                int itemId = item.Id.TryParseIntUsLocalization().Value;
                decimal? value = item.Value.TryParseDecimalUsLocalization();
                if (!value.HasValue || value.Value < 0) value = 0;
                cart.SetItemQuantity(itemId, value.Value);
            }

            var unitElementAndIndex = keys
                        .Where(k => k.Contains("UnitMeasureCode") && !k.Contains("cboUnitMeasureCode"))
                        .Select((n, i) => new { Value = Request.Form[n], Id = n.Substring("UnitMeasureCode".Length + 1) });

            foreach (var item in unitElementAndIndex)
            {
                int itemId = item.Id.TryParseIntUsLocalization().Value;
                string value = item.Value;
                if (string.IsNullOrEmpty(value)) continue;
                cart.UpdateUnitMeasureForItem(itemId, value.ToHtmlDecode(), (cart.HasMultipleShippingAddresses() || (cart.HasRegistryItems() && cart.CartItems.Count >0)), cart.HasRegistryItems());
            }
        }

        private void ClearErrors()
        {
            CouponError.Text = string.Empty;
            ErrorMsgLabel.Text = string.Empty;
            InventoryTrimmedError.Text = string.Empty;
            MinimumQuantitiesUpdatedError.Text = string.Empty;
            MeetsMinimumOrderAmountError.Text = string.Empty;
            MeetsMinimumOrderQuantityError.Text = string.Empty;
            Micropay_EnabledError.Text = string.Empty;
        }

        private void ContinueShopping()
        {
            if (AppLogic.AppConfig("ContinueShoppingURL") == "")
            {
                if (ViewState["ReturnURL"] == null || ViewState["ReturnURL"].ToString() == "")
                {
                    Response.Redirect("default.aspx");
                }
                else
                {
                    Response.Redirect(ViewState["ReturnURL"].ToString());
                }
            }
            else
            {
                Response.Redirect(AppLogic.AppConfig("ContinueShoppingURL"));
            }
        }

        private void InitializeDomainServices()
        {
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        }

        private void DeleteOutofStockPhasedOutItem()
        {
            if (cart.IsEmpty()) return;

            var lst = cart.CartItems.Where(item => item.Status == "P" && item.IsOutOfStock)
                                     .Select(item => item.Id.ToString());
            _shoppingCartService.ClearLineItemsAndKitComposition(lst.ToArray());
        }

        private void DeleteOutofStockAndWithoutOpenPOItem()
        {
            if (cart.IsEmpty()) return;

            string[] poStatus = { "Open".ToLowerInvariant(), "Partial".ToLowerInvariant() };
            var lstOutOfStockAndWithoutOpenPOItem = cart.CartItems.Where(item => item.IsOutOfStock && !poStatus.Contains(item.POStatus.ToLowerInvariant()));
            foreach (CartItem item in lstOutOfStockAndWithoutOpenPOItem)
            {
                _shoppingCartService.ClearLineItemsAndKitComposition(new string[] { item.Id.ToString() });
            }
        }

        private void DoOrderNotesChecking()
        {
            if (!_appConfigService.DisallowOrderNotes)
            {
                OrderNotes.Text = cart.OrderNotes;
                pnlOrderNotes.Visible = true;
            }
            else
            {
                pnlOrderNotes.Visible = false;
            }
        }

        #region "Event Handlers"

        protected void btnPayPalExpressCheckout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!ThisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout")))
            {
                Response.Redirect("checkoutanon.aspx?checkout=true&checkouttype=pp");
            }
            else
            {
                // Get IS Cart ready
                ProcessCart(false);

                if (cart == null)
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

                cart.BuildSalesOrderDetails(false, false);

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "shoppingcart";
                Response.Redirect(PayPalExpress.CheckoutURL(cart));
            }
        }

        void OrderOptionsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            XmlNode orderOptionNode = e.Item.DataItem as XmlNode;
            int counter = 0;
            if (orderOptionNode != null &&
                int.TryParse(orderOptionNode["Counter"].InnerText, out counter))
            {
                string itemCode = orderOptionNode["ItemCode"].InnerText;
                string itemName = orderOptionNode["ItemName"].InnerText;
                string itemDescription = orderOptionNode["ItemDescription"].InnerText; 
                string popupTitle = string.Empty;

                Label lblDisplayName = e.Item.FindControl("OrderOptionName") as Label;
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

                if (_appConfigService.ShowPicsInCart)
                {
                    var img = ProductImage.Locate("product", counter, "icon");
                    string imgUrl = img.src;

                    //String ImgUrl = InterpriseHelper.LookUpImageByItemCode(itemCode, "icon", SkinID, ThisCustomer.LocaleSetting);
                    if (!imgUrl.IsNullOrEmptyTrimmed() && imgUrl.IndexOf("nopicture") == -1)
                    {
                        var imgControl = (Image)e.Item.FindControl("OptionImage");
                        imgControl.ImageUrl = imgUrl;
                        imgControl.Visible = true;
                    }
                }

                var helpCircle = (Image)e.Item.FindControl("helpcircle_gif");
                helpCircle.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "helpcircle.gif");
                helpCircle.Attributes.Add("onclick", "popuporderoptionwh('Order Option " + popupTitle + "', " + counter.ToString() + ",650,550,'yes');");

                // 2 Control choices for drop down list
                var cboUnitMeasureCode = e.Item.FindControl("cboUnitMeasureCode") as DropDownList;
                var lblUnitMeasureCode = e.Item.FindControl("lblUnitMeasureCode") as Label;
                var availableUnitMeasures = ProductDA.GetProductUnitMeasureAvailability(ThisCustomer.CustomerCode, itemCode,
                                                                                        AppLogic.AppConfigBool("ShowInventoryFromAllWarehouses"),
                                                                                        ThisCustomer.IsNotRegistered);
                if (availableUnitMeasures.Count() > 1)
                {
                    // render as drop down list
                    lblUnitMeasureCode.Visible = false;

                    foreach (string unitMeasureCode in availableUnitMeasures)
                    {
                        cboUnitMeasureCode.Items.Add(new ListItem(HttpUtility.HtmlEncode(unitMeasureCode), HttpUtility.HtmlEncode(unitMeasureCode)));
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
                decimal price = InterpriseHelper.GetSalesPriceAndTax(ThisCustomer.CustomerCode,
                                                itemCode,
                                                ThisCustomer.CurrencyCode,
                                                Decimal.One,
                                                um.Code, withVat,
                                                ref promotionalPrice);

                if (promotionalPrice != Decimal.Zero)
                {
                    price = promotionalPrice;
                }

                string vatDisplay = String.Empty;
                if (AppLogic.AppConfigBool("VAT.Enabled"))
                {
                    vatDisplay = (ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive)?
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.38") + "</span>\n":
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.37") + "</span>\n";
                }

                var lblPrice = e.Item.FindControl("OrderOptionPrice") as Label;
                lblPrice.Text = price.ToCustomerCurrency() + vatDisplay;

                var hfCounter = e.Item.FindControl("hfItemCounter") as HiddenField;
                hfCounter.Value = counter.ToString();

                var cbk = (DataCheckBox)e.Item.FindControl("OrderOptions");
                cbk.Checked = false;

                bool shouldBeAbleToEnterNotes = orderOptionNode["CheckOutOptionAddMessage"].InnerText.TryParseBool().Value;
                var lblNotes = e.Item.FindControl("lblNotes") as Label;
                var txtNotes = e.Item.FindControl("txtOrderOptionNotes") as TextBox;
                lblNotes.Visible = txtNotes.Visible = shouldBeAbleToEnterNotes;
                txtNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 1000);");
            }
        }

        void btnContinueShoppingTop_Click(object sender, EventArgs e)
        {
            ContinueShopping();
        }

        void btnCheckOutNowTop_Click(object sender, EventArgs e)
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

        void btnUpdateCart1_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        #endregion

    }
}