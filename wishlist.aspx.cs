// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System.Text.RegularExpressions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for wishlist.
    /// </summary>
    public partial class wishlist : SkinBase
    {
        #region Declaration

        InterpriseShoppingCart cart;
        string BACKURL = string.Empty;

        #endregion

        #region DomainServices

        private INavigationService _navigationService = null;
        private IShoppingCartService _shoppingCartService = null;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            RegisterDomainServices();
            BindControls();

            base.OnInit(e);
        }

        private void BindControls()
        {
            btnContinueShopping2.Click += btnContinueShopping1_Click;
            btnContinueShopping1.Click += btnContinueShopping1_Click;

            btnUpateWishList1.Click +=btnUpateWishList1_Click;
            btnUpateWishList2.Click += btnUpateWishList1_Click;
        }

        public void btnUpateWishList1_Click(object sender, EventArgs e)
        {
            UpdateWishList();
            cart = _shoppingCartService.New(CartTypeEnum.WishCart, true);
            InitializePageContent();
        }

        protected void btnContinueShopping1_Click(object sender, EventArgs e)
        {
            string returnUrl = this.ReturnUrl;
            string url = returnUrl.IsNullOrEmptyTrimmed() ? url = AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting) : returnUrl;
            _navigationService.NavigateToUrl(url);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.RequireCustomerRecord();

            SectionTitle = AppLogic.GetString("wishlist.aspx.1", true);

            //Check if from Move To Shopping Cart
            if (!string.IsNullOrEmpty(CommonLogic.QueryStringCanBeDangerousContent("MoveToCartID")))
            {
                string moveToCartID = CommonLogic.QueryStringCanBeDangerousContent("MoveToCartID");
                if (moveToCartID.Where(t => t == '_').Count() > 1)
                {
                    //This logic is for Bundle Only
                    string[] moveToCartIDArray = moveToCartID.Split('_');
                    string bundleCode = string.Empty;
                    using (var con = DB.NewSqlConnection())
                    {
                        con.Open();
                       
                        using (var reader = DB.GetRSFormat(con, "SELECT TOP 1 BundleCode  FROM EcommerceShoppingCart WHERE ShoppingCartRecID IN ({0}) AND CartType={1} AND WebSiteCode={2} AND CustomerCode={3} AND ContactCode={4} GROUP BY BundleCode", 
                            string.Join(",", moveToCartIDArray.Select(t => t.ToDbQuote())), 
                            (int)Convert.ChangeType(CartTypeEnum.WishCart, CartTypeEnum.ShoppingCart.GetTypeCode()),
                            InterpriseHelper.ConfigInstance.WebSiteCode.ToDbQuote(),
                            Customer.Current.CustomerCode.ToDbQuote(),
                            Customer.Current.ContactCode.ToDbQuote()
                            ))
                        {
                            if (reader.Read())
                            {
                                bundleCode = reader.ToRSField("BundleCode");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(bundleCode))
                    {
                        int bundleHeaderID = BundleProductPage.GetShoppingCartBundleHeaderID(bundleCode, CartTypeEnum.ShoppingCart, Customer.Current);
                        string script = string.Format("UPDATE EcommerceShoppingCart " +
                                                  "SET CartType = {0} , BundleHeaderID = {1}" +
                                                  "WHERE ShoppingCartRecID IN ({2})",
                                                  (int)Convert.ChangeType(CartTypeEnum.ShoppingCart, CartTypeEnum.ShoppingCart.GetTypeCode()),
                                                  bundleHeaderID,
                                                  string.Join(",", moveToCartIDArray.Select(t => t.ToDbQuote())));
                        DB.ExecuteSQL(script);
                    }

                
                }
                else
                {
                    int cartId = CommonLogic.QueryStringUSInt("MoveToCartID");
                    decimal quantity = CommonLogic.QueryStringUSDecimal("MoveToCartQty");

                    bool cartItemExisting = false;
                    string itemCode = string.Empty;
                    string itemType = string.Empty;
                    string unitMeasureCode = string.Empty;
                    string shippingAddressID = string.Empty;
                    Guid cartGuid = Guid.Empty;
                    int counter = 0;
                    // NOTE : 
                    // Move this logic on the Shopping Cart Form


                    using (var con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (var reader = DB.GetRSFormat(con, "SELECT wsc.ShoppingCartRecGuid, i.Counter, i.ItemCode, i.ItemType, wsc.UnitMeasureCode, wsc.ShippingAddressID FROM EcommerceShoppingCart wsc with (NOLOCK) INNER JOIN InventoryItem i with (NOLOCK) ON i.ItemCode = wsc.ItemCode WHERE wsc.ShoppingCartRecID = {0}", cartId))
                        {
                            cartItemExisting = reader.Read();
                            if (cartItemExisting)
                            {
                                cartGuid = DB.RSFieldGUID2(reader, "ShoppingCartRecGuid");
                                counter = DB.RSFieldInt(reader, "Counter");
                                itemCode = DB.RSField(reader, "ItemCode");
                                itemType = DB.RSField(reader, "ItemType");
                                unitMeasureCode = DB.RSField(reader, "UnitMeasureCode");
                                shippingAddressID = DB.RSField(reader, "ShippingAddressID");
                            }
                        }
                    }

                    if (cartItemExisting)
                    {
                        cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

                        if (itemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                        {
                            var kitCartWishListComposition = KitComposition.FromCart(ThisCustomer, CartTypeEnum.WishCart, itemCode, cartGuid);
                            cart.AddItem(ThisCustomer,
                                shippingAddressID,
                                itemCode,
                                counter,
                                quantity,
                                unitMeasureCode,
                                CartTypeEnum.ShoppingCart,
                                kitCartWishListComposition, null, CartTypeEnum.WishCart);
                        }
                        else
                        {
                            cart.AddItem(ThisCustomer,
                                shippingAddressID,
                                itemCode,
                                counter,
                                quantity,
                                unitMeasureCode,
                                CartTypeEnum.ShoppingCart);
                        }

                        ServiceFactory.GetInstance<IShoppingCartService>()
                                      .ClearLineItemsAndKitComposition(new String[] { cartGuid.ToString() });
                    }
                
                }

               
                Response.Redirect("ShoppingCart.aspx");
            }

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);

            string[] formkeys = Request.Form.AllKeys;
            foreach (string s in formkeys)
            {
                if (s == "bt_Delete")
                {
                    UpdateWishList();
                    InitializePageContent();
                }
            }

            if (!IsPostBack)
            {
                string returnurl = this.ReturnUrl;
                if (returnurl.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }

                InitializePageContent();
            }
            TopicWishListPageHeader.SetContext = this;
            TopicWishListPageFooter.SetContext = this;
        }

        #endregion

        private void RegisterDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        }

        private void RedirectToSignInPage()
        {
            pnlTopControlLines.Visible = false;
            Panel1.Visible = false;
            Xml_WishListPageBottomControlLines.Visible = false;
            pnlBottomControlLines.Visible = false;
            Xml_WishListPageFooter.Visible = false;

            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", true);

            // perform redirect
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", "signin.aspx".ToUrlDecode()));
        }

        public string ReturnUrl 
        { 
            get 
            {
                return CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
            } 
        }

        private void InitializePageContent()
        {
            int AgeWishListDays = AppLogic.AppConfigUSInt("AgeWishListDays");
            if (AgeWishListDays == 0)
            {
                AgeWishListDays = 7;
            }

            ShoppingCart.Age(AgeWishListDays, CartTypeEnum.WishCart);

            if (cart == null)
            {
                cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);
            }

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.WishListPageHeader");
            if (XmlPackageName.Length != 0)
            {
                throw new NotImplementedException("Not yet ported");
            }

            string CartTopControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageTopControlLines");
            if (CartTopControlLinesXmlPackage.Length != 0)
            {
                XmlPackage_WishListPageTopControlLines.Text = AppLogic.RunXmlPackage(CartTopControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
                XmlPackage_WishListPageTopControlLines.Visible = true;
            }
            else
            {
                pnlTopControlLines.Visible = true;
                btnContinueShopping1.Text = AppLogic.GetString("shoppingcart.cs.12", true);
                btnContinueShopping1.Attributes.Add("onclick", "self.location='" + BACKURL + "'");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList1.Text = AppLogic.GetString("shoppingcart.cs.32", true);
                }
                else
                {
                    btnUpateWishList1.Visible = false;
                }
            }

           
 
 

            string CartItemsXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageItems");
            if (CartItemsXmlPackage.Length != 0)
            {
                CartItems.Text = AppLogic.RunXmlPackage(CartItemsXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }
            else
            {
                CartItems.Text = cart.RenderHTMLLiteral(new WishListPageLiteralRenderer());
            }

            string CartBottomControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageBottomControlLines");
            if (CartBottomControlLinesXmlPackage.Length != 0)
            {
                Xml_WishListPageBottomControlLines.Text = AppLogic.RunXmlPackage(CartBottomControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
                Xml_WishListPageBottomControlLines.Visible = true;
            }
            else
            {
                pnlBottomControlLines.Visible = true;
                btnContinueShopping2.Text = AppLogic.GetString("shoppingcart.cs.12", true);
                btnContinueShopping2.Attributes.Add("onclick", "self.location='" + BACKURL + "'");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList2.Text = AppLogic.GetString("shoppingcart.cs.32", true);
                }
                else
                {
                    btnUpateWishList2.Visible = false;
                }
            }

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnContinueShopping1, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnContinueShopping2, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnUpateWishList1, "shoppingcart.cs.32");
                AppLogic.EnableButtonCaptionEditing(btnUpateWishList2, "shoppingcart.cs.32");

            }

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.WishListPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                Xml_WishListPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }


            GetJSFunctions();
        }

        private void UpdateWishList()
        {
            // update cart quantities:           
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                string fld = Request.Form.Keys[i];
                string fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                string quantity;
                if (fld.StartsWith("Quantity"))
                {
                    if (fld.Where(t => t == '_').Count() > 1)
                    {
                        decimal newQuantity = fldval.ToDecimal();

                        //This logic is for Bundle only
                        string bundleShoppingCartIDs = fld.Remove(0, "Quantity_".Length);
                        string[] bundleShoppingCartIDsArray = bundleShoppingCartIDs.Split('_');

                        string script = string.Empty;
                        if (newQuantity > 0 )
                        {
                            script = string.Format("UPDATE EcommerceShoppingCart " +
                                             "SET BundleQuantity = {0} " +
                                             "WHERE ShoppingCartRecID IN ({1})",
                                             newQuantity,
                                             string.Join(",", bundleShoppingCartIDsArray.Select(t => t.ToDbQuote())));
                            DB.ExecuteSQL(script);
                        }
                        else
                        {
                            foreach (string shoppingCartID in bundleShoppingCartIDsArray)
                            {
                                cart.SetItemQuantity(int.Parse(shoppingCartID), 0);
                            }
                        }
                       
                    





                        //string script = string.Format("DELETE FROM EcommerceShoppingCart WHERE ShoppingCartRecID IN ({0})", string.Join(",", bundleShoppingCartIDsArray.Select(t => t.ToDbQuote())));
                        //DB.ExecuteSQL(script);
                        //foreach (string shoppingCartID in bundleShoppingCartIDsArray)
                        //{
                        //    cart.SetItemQuantity(int.Parse(shoppingCartID), 0);
                        //}
                    }
                    else
                    {
                        if (fldval.StartsWith(Localization.GetNumberDecimalSeparatorLocaleString(cart.ThisCustomer.LocaleSetting)))
                        {
                            fldval = fldval.Insert(0, Localization.GetNumberZeroLocaleString(cart.ThisCustomer.LocaleSetting));
                        }
                        if (Regex.IsMatch(fldval, AppLogic.AllowedQuantityWithDecimalRegEx(cart.ThisCustomer.LocaleSetting), RegexOptions.Compiled))
                        {
                            recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                            quantity = fldval;
                            decimal iquan = Convert.ToDecimal(quantity);//Localization.ParseUSDecimal(quantity);
                            if (iquan < 0)
                            {
                                iquan = 0;
                            }
                            cart.SetItemQuantity(recID, iquan);
                        }
                    }
                   
                }
                if (fld.StartsWith("UnitMeasureCode"))
                {
                    if (!string.IsNullOrEmpty(fldval))
                    {
                        recID = Localization.ParseUSInt(fld.Substring("UnitMeasureCode".Length + 1));
                        string unitMeasureCode = HttpUtility.HtmlDecode(fldval);
                        cart.UpdateUnitMeasureForItem(recID, unitMeasureCode, cart.HasMultipleShippingAddresses());
                    }
                }
            }

        }

        private void GetJSFunctions()
        {
            var s = new StringBuilder("<script type='text/javascript'>");
            s.Append("function " + "FormValidator(theForm){\n");
            string cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);
            foreach (var c in cart.CartItems)
            {
                string itemJS = string.Empty;

                itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression(c.ItemType, true));
                itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting)).Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));

                string quantityValiadtionMessage = AppLogic.GetString("common.cs.22", true);
                if (AppLogic.IsAllowFractional)
                {
                    quantityValiadtionMessage += String.Format("\\n" +
                    AppLogic.GetString("common.cs.26", true),
                    AppLogic.InventoryDecimalPlacesPreference.ToString());
                }
                itemJS = itemJS.Replace("%ALLOW_FRACTIONAL_MSG_INPUT%", quantityValiadtionMessage);

                s.Append(itemJS.Replace("%SKU%", c.m_ShoppingCartRecordID.ToString()));
            }

            s.Append("return(true);\n");
            s.Append("}\n");
            s.Append("</script>\n");
            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), s.ToString());
        }

    }
}