// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Xml;
using System.Web;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using com.paypal.soap.api;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceControls.Validators;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

#region Login Customer


/// <summary>
/// Add this line <add name="loginCustomer.aspx_*" path="loginCustomer.aspx" verb="*" type="LoginCustomer" preCondition="integratedMode,runtimeVersionv4.0"/>
/// to web.config in  /<system.webServer>/<handlers>
/// </summary>
public class LoginCustomer : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable
    {
        get { return true; }
    }
    private INavigationService _navigationService = null;
    private IStringResourceService _stringResourceService = null;
    private IShoppingCartService _shoppingCartService = null;
    private IProductService _productService = null;
    private IAuthenticationService _authenticationService = null;
    private void InitializeDomainServices()
    {
      
    }
    public LoginCustomer()
    {
        _navigationService = ServiceFactory.GetInstance<INavigationService>();
        _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        _productService = ServiceFactory.GetInstance<IProductService>();
        _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
    }

    
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.CacheControl = "private";
        context.Response.Expires = 0;
        context.Response.AddHeader("pragma", "no-cache");

        string email = CommonLogic.FormCanBeDangerousContent("email");
        string password = CommonLogic.FormCanBeDangerousContent("password");
        bool remember = CommonLogic.FormCanBeDangerousContent("remember").ToLower() == "on";

       
         if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
        {
            string errorMessage = _stringResourceService.GetString("signin.aspx.22", true)
                                                        .FormatWith(String.Empty, String.Empty);
            if (context.Session == null) {
                context.Response.Redirect("signin.aspx?error=captcha");
                return;
            }
            if (context.Session["SecurityCode"] != null)
            {
                string sCode = context.Session["SecurityCode"].ToString();
                string fCode = CommonLogic.FormCanBeDangerousContent("captcha");
                bool codeMatch = false;

                if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                {
                    if (fCode.Equals(sCode))
                        codeMatch = true;
                }
                else
                {
                    if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase))
                        codeMatch = true;
                }

                if (!codeMatch)
                {
                    //this.pErrorMessage.InnerText = errorMessage;
                    //this.divErrorContainer.Visible = true;
                    //this.txtCaptcha.Value = String.Empty;
                    //this.imgCaptcha.Src = "Captcha.ashx?id=1";
                    //return;

                    context.Response.Redirect("signin.aspx?error=captcha");
                    return;
                }
            }
            else
            {
                //this.pErrorMessage.InnerText = errorMessage;
                //this.divErrorContainer.Visible = true;
                //this.txtCaptcha.Value = String.Empty;
                //this.imgCaptcha.Src = "Captcha.ashx?id=1";
                context.Response.Redirect("signin.aspx?error=captcha");
                return;
            }
        }

       

        //this.pErrorMessage.InnerText = AppLogic.GetString("signin.aspx.21", true);
        //this.divErrorContainer.Visible = true;
        var _EmailValidator = new RegularExpressionInputValidator
            (
            new System.Web.UI.WebControls.TextBox { Text = email },
            DomainConstants.EmailRegExValidator, 
            AppLogic.GetString("signin.aspx.21", true));
        _EmailValidator.Validate();


        if (_EmailValidator.IsValid)
        {
            var status = _authenticationService.Login(email, password, remember);

            if (!status.IsValid)
            {
                if (status.IsAccountExpired)
                {
                    //this.pErrorMessage.InnerText = _stringResourceService.GetString("signin.aspx.message.1", true);
                    //this.lnkContactUs.InnerText = _stringResourceService.GetString("menu.Contact", true);
                    //this.lnkContactUs.Visible = true;
                    //this.divErrorContainer.Visible = true;
                    context.Response.Redirect("signin.aspx?error=expiredaccount");

                }
                else
                {
                    //this.pErrorMessage.InnerText = _stringResourceService.GetString("signin.aspx.20", true);
                    //this.divErrorContainer.Visible = true;

                    context.Response.Redirect("signin.aspx?error=invalidlogin");
                }
                return;
            }


            context.Response.Redirect(context.Request.UrlReferrer.ToString());
            //var customerWithValidLogin = _authenticationService.GetCurrentLoggedInCustomer();
            //string sReturnURL = _authenticationService.GetRedirectUrl(customerWithValidLogin.ContactGUID.ToString(), remember);

        }
        else
        {
            context.Response.Redirect("signin.aspx?error=invalidemail");
        }
    }
}

#endregion

#region AddtoCart

public class AddtoCart : IHttpHandler
{
    #region Declaration

    private INavigationService _navigationService = null;
    private IStringResourceService _stringResourceService = null;
    private IShoppingCartService _shoppingCartService = null;
    private IProductService _productService = null;

    #endregion

    public bool IsReusable
    {
        get { return true; }
    }

    private void InitializeDomainServices()
    {
        _navigationService = ServiceFactory.GetInstance<INavigationService>();
        _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        _productService = ServiceFactory.GetInstance<IProductService>();
    }

    public void ProcessRequest(HttpContext context)
    {
        InitializeDomainServices(); 
        
        context.Response.CacheControl = "private";
        context.Response.Expires = 0;
        context.Response.AddHeader("pragma", "no-cache");

        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;
        ThisCustomer.RequireCustomerRecord();

        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }
        
        //Anonymous users should not be allowed to used WishList, they must register first.
        if (ThisCustomer.IsNotRegistered)
        {
            string ErrMsg = string.Empty;

            if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
            {
                ErrMsg = AppLogic.GetString("signin.aspx.19");
                context.Response.Redirect("signin.aspx?ErrorMsg=" + ErrMsg + "&ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
        }

        string ShippingAddressID = "ShippingAddressID".ToQueryString(); // only used for multi-ship
        if (ShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = CommonLogic.FormCanBeDangerousContent("ShippingAddressID");
        }

        if (ShippingAddressID.IsNullOrEmptyTrimmed() && !ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
        }

        string ProductID = "ProductID".ToQueryString();
        if (ProductID.IsNullOrEmptyTrimmed())
        {
            ProductID = CommonLogic.FormCanBeDangerousContent("ProductID");
        }

        string itemCode = "ItemCode".ToQueryString();
        // check if the item being added is matrix group
        // look for the matrix item and use it as itemcode instead
        if (!string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("MatrixItem")))
        {
            itemCode = CommonLogic.FormCanBeDangerousContent("MatrixItem");
        }

        bool itemExisting = false;
        string defaultUnitMeasure = string.Empty;

        if (itemCode.IsNullOrEmptyTrimmed())
        {
            int itemCounter = 0;
            if (!ProductID.IsNullOrEmptyTrimmed() &&
                int.TryParse(ProductID, out itemCounter) &&
                itemCounter > 0)
            {
                var validItemCodeAndBaseUnitMeasure = ServiceFactory.GetInstance<IProductService>().GetValidItemCodeAndBaseUnitMeasureById(itemCounter);
                if (validItemCodeAndBaseUnitMeasure != null)
                {
                    itemExisting = true;
                    itemCode = validItemCodeAndBaseUnitMeasure.ItemCode;
                    defaultUnitMeasure = validItemCodeAndBaseUnitMeasure.UnitMeasureCode;
                }
            }
        }
        else
        {
            // verify we have a valid item code
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT i.ItemCode FROM InventoryItem i with (NOLOCK) WHERE i.ItemCode = {0}", DB.SQuote(itemCode)))
                {
                    itemExisting = reader.Read();

                    if (itemExisting)
                    {
                        itemCode = reader.ToRSField("ItemCode");
                    }
                }
            }
        }

        if (!itemExisting)
        {
            _navigationService.NavigateToShoppingCartWitErroMessage(_stringResourceService.GetString("shoppingcart.cs.62"));
        }

        if (ThisCustomer.IsNotRegistered)
        {
            var item = _productService.GetInventoryItem(itemCode);
            if (item != null)
            {
                // do not allow unregistered customer to add giftcard and giftcertificate item to cart
                if (item.ItemType.EqualsIgnoreCase(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CARD) ||
                    item.ItemType.EqualsIgnoreCase(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CERTIFICATE))
                {
                    string message = AppLogic.GetString("signin.aspx.23");
                    _navigationService.NavigateToSignin(message);
                }
            }
        }

        // get the unit measure code
        string unitMeasureCode = "UnitMeasureCode".ToQueryString();
        if (unitMeasureCode.IsNullOrEmptyTrimmed())
        {
            unitMeasureCode = CommonLogic.FormCanBeDangerousContent("UnitMeasureCode");
        }

        // check if the unit measure is default so that we won't have to check
        // if the unit measure specified is valid...
        if (false.Equals(unitMeasureCode.Equals(defaultUnitMeasure, StringComparison.InvariantCultureIgnoreCase)))
        {
            bool isValidUnitMeasureForThisItem = false;

            // if no unit measure was passed use DefaultSelling
            string sqlQuery = string.Empty;
            if (unitMeasureCode.IsNullOrEmptyTrimmed())
            {
               sqlQuery = String.Format( "SELECT UnitMeasureCode FROM InventoryUnitMeasure with (NOLOCK) WHERE ItemCode= {0} AND DefaultSelling = 1", DB.SQuote(itemCode));
            }
            else
            {
               sqlQuery =  String.Format("SELECT UnitMeasureCode FROM InventoryUnitMeasure with (NOLOCK) WHERE ItemCode= {0} AND UnitMeasureCode = {1}", DB.SQuote(itemCode), DB.SQuote(unitMeasureCode));
            }

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con,sqlQuery))
                {
                    isValidUnitMeasureForThisItem = reader.Read();

                    if (isValidUnitMeasureForThisItem)
                    {
                        // maybe mixed case specified, just set..
                        unitMeasureCode = reader.ToRSField("UnitMeasureCode");
                    }
                }
            }
            

            if (!isValidUnitMeasureForThisItem)
            {
                GoNextPage(context);
            }
        }
        decimal Quantity = CommonLogic.FormLocaleDecimal("Quantity",ThisCustomer.LocaleSetting);//CommonLogic.QueryStringUSDecimal("Quantity");
        
        if (Quantity == 0) { Quantity = CommonLogic.FormNativeDecimal("Quantity"); }

        if (Quantity == 0) { Quantity = 1; }

        Quantity = CommonLogic.RoundQuantity(Quantity);

        // Now let's check the shipping address if valid if specified
        if (ShippingAddressID != ThisCustomer.PrimaryShippingAddressID)
        {
            if (ThisCustomer.IsRegistered)
            {
                bool shippingAddressIsValidForThisCustomer = false;

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT ShipToCode FROM CustomerShipTo with (NOLOCK) WHERE CustomerCode = {0} AND IsActive = 1 AND ShipToCode = {1}", DB.SQuote(ThisCustomer.CustomerCode), DB.SQuote(ShippingAddressID)))
                    {
                        shippingAddressIsValidForThisCustomer = reader.Read();

                        if (shippingAddressIsValidForThisCustomer)
                        {
                            // maybe mixed case, just set...
                            ShippingAddressID = reader.ToRSField("ShipToCode");
                        }
                    }
                }

                if (!shippingAddressIsValidForThisCustomer)
                {
                    GoNextPage(context);
                }
            }
            else
            {
                ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
            }
        }

        var CartType = CartTypeEnum.ShoppingCart;
        if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
        {
            CartType = CartTypeEnum.WishCart;
        }

        var giftRegistryItemType = GiftRegistryItemType.vItem;
        if (CommonLogic.FormNativeInt("IsAddToGiftRegistry") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistry") == 1)
        {
            CartType = CartTypeEnum.GiftRegistryCart;
        }

        if (CommonLogic.FormNativeInt("IsAddToGiftRegistryOption") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistryOption") == 1)
        {
            CartType = CartTypeEnum.GiftRegistryCart;
            giftRegistryItemType = GiftRegistryItemType.vOption;
        }

        ShoppingCart cart = null;
        bool itemIsARegistryItem = false;
        if (!itemCode.IsNullOrEmptyTrimmed())
        {
            #region " --GIFTREGISTRY-- "

            if (CartType == CartTypeEnum.GiftRegistryCart)
            {
                CheckOverSizedItemForGiftRegistry(itemCode);
                
                Guid? registryID = CommonLogic.FormCanBeDangerousContent("giftregistryOptions").TryParseGuid();
                if (registryID.HasValue)
                {
                    var selectedGiftRegistry = ThisCustomer.GiftRegistries.FindFromDb(registryID.Value);
                    if (selectedGiftRegistry != null)
                    {
                        bool isKit = AppLogic.IsAKit(itemCode);
                        KitComposition preferredComposition = null;
                        GiftRegistryItem registryItem = null;

                        if (isKit)
                        {
                            preferredComposition = KitComposition.FromForm(ThisCustomer, itemCode);
                            var registrytems = selectedGiftRegistry.GiftRegistryItems.Where(giftItem => giftItem.ItemCode == itemCode &&
                                                                                     giftItem.GiftRegistryItemType == giftRegistryItemType);
                            Guid? matchedRegitryItemCode = null;
                            //Do this routine to check if there are kit items
                            //matched the selected kit items from the cart in the registry items
                            foreach (var regitm in registrytems)
	                        {
                                regitm.IsKit = true;
                                var compositionItems = regitm.GetKitItemsFromComposition();

                                if (compositionItems.Count() == 0) continue;

                                var arrItemCodes = compositionItems.Select(item => item.ItemCode)
                                                                   .ToArray();
                                var preferredItemCodes = preferredComposition.Compositions.Select(kititem => kititem.ItemCode);
                                var lst = arrItemCodes.Except(preferredItemCodes);

                                //has match
                                if (lst.Count() == 0)
                                {
                                    matchedRegitryItemCode = regitm.RegistryItemCode;
                                    break;
                                }
	                        }

                            if (matchedRegitryItemCode.HasValue)
                            {
                                registryItem = selectedGiftRegistry.GiftRegistryItems.FirstOrDefault(giftItem => giftItem.RegistryItemCode == matchedRegitryItemCode);
                            }
                        }

                        //if not kit item get the item as is
                        if (registryItem == null && !isKit)
                        {
                            registryItem = selectedGiftRegistry.GiftRegistryItems.FirstOrDefault(giftItem => giftItem.ItemCode == itemCode &&
                                                                                     giftItem.GiftRegistryItemType == giftRegistryItemType);
                        }

                        if (registryItem != null)
                        {
                            registryItem.Quantity += Quantity;
                            registryItem.UnitMeasureCode = unitMeasureCode;
                            selectedGiftRegistry.GiftRegistryItems.UpdateToDb(registryItem);
                        }
                        else
                        {
                            registryItem = new GiftRegistryItem()
                            {
                                GiftRegistryItemType = giftRegistryItemType,
                                RegistryItemCode = Guid.NewGuid(),
                                ItemCode = itemCode,
                                Quantity = Quantity,
                                RegistryID = registryID.Value,
                                UnitMeasureCode = unitMeasureCode
                            };

                            selectedGiftRegistry.GiftRegistryItems.AddToDb(registryItem);
                        }

                        if (isKit && preferredComposition != null)
                        {
                            registryItem.ClearKitItemsFromComposition();
                            preferredComposition.AddToGiftRegistry(registryID.Value, registryItem.RegistryItemCode);
                        }

                        HttpContext.Current.Response.Redirect(string.Format("~/editgiftregistry.aspx?{0}={1}", DomainConstants.GIFTREGISTRYPARAMCHAR, registryID.Value));
                    }
                }

                GoNextPage(context);
            }

            #endregion

            CartRegistryParam registryCartParam = null;
            if (AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                registryCartParam = new CartRegistryParam ()
                {
                    RegistryID = CommonLogic.FormGuid("RegistryID"),
                    RegistryItemCode = CommonLogic.FormGuid("RegistryItemCode")
                };
            }

            if(registryCartParam != null && registryCartParam.RegistryID.HasValue && registryCartParam.RegistryItemCode.HasValue)
            {
                ShippingAddressID = GiftRegistryDA.GetPrimaryShippingAddressCodeOfOwnerByRegistryID(registryCartParam.RegistryID.Value);
                itemIsARegistryItem = true;

                //Automatically clear the itemcart with warehouse code if added a registry item.
                _shoppingCartService.ClearCartWarehouseCodeByCustomer();
            }

            cart = new ShoppingCart(null, 1, ThisCustomer, CartType, string.Empty, false, true, string.Empty);
            if (Quantity > 0)
            {
                if (AppLogic.IsAKit(itemCode))
                {
                    var preferredComposition = KitComposition.FromForm(ThisCustomer, CartType, itemCode);

                    if (preferredComposition == null)
                    {
                        int itemCounter = 0;
                        int.TryParse(ProductID, out itemCounter);
                        var kitData = KitItemData.GetKitComposition(ThisCustomer, itemCounter, itemCode);

                        var kitContents = new StringBuilder();
                        foreach (var kitGroup in kitData.Groups)
                        {
                            if (kitContents.Length > 0) { kitContents.Append(","); }

                            var selectedItems = new StringBuilder();
                            int kitGroupCounter = kitGroup.Id;

                            var selectedKitItems = kitGroup.Items.Where(i => i.IsSelected == true);

                            foreach (var item in selectedKitItems)
                            {
                                if (selectedItems.Length > 0) { selectedItems.Append(","); }

                                //note: since we are adding the kit counter and kit item counter in KitItemData.GetKitComposition (stored proc. EcommerceGetKitItems)
                                //as "kit item counter", we'll reverse the process in order to get the "real kit item counter"

                                int kitItemCounter = item.Id - itemCounter; 
                                selectedItems.Append(kitGroupCounter.ToString() + DomainConstants.KITCOMPOSITION_DELIMITER + kitItemCounter.ToString());
                            }
                            kitContents.Append(selectedItems.ToString());
                        }
                        preferredComposition = KitComposition.FromComposition(kitContents.ToString(), ThisCustomer, CartType, itemCode);
                    }

                    preferredComposition.PricingType = CommonLogic.FormCanBeDangerousContent("KitPricingType");

                    if (CommonLogic.FormBool("IsEditKit") &&
                        !CommonLogic.IsStringNullOrEmpty(CommonLogic.FormCanBeDangerousContent("KitCartID")) &&
                        InterpriseHelper.IsValidGuid(CommonLogic.FormCanBeDangerousContent("KitCartID")))
                    {
                        Guid cartID = new Guid(CommonLogic.FormCanBeDangerousContent("KitCartID"));
                        preferredComposition.CartID = cartID;
                    }
                    cart.AddItem(ThisCustomer, ShippingAddressID, itemCode, int.Parse(ProductID), Quantity, unitMeasureCode, CartType, preferredComposition, registryCartParam);
                }
                else
                {
                    cart.AddItem(ThisCustomer, ShippingAddressID, itemCode, int.Parse(ProductID), Quantity, unitMeasureCode, CartType, null, registryCartParam);
                }
            }

            string RelatedProducts = CommonLogic.QueryStringCanBeDangerousContent("relatedproducts").Trim();
            string UpsellProducts = CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim();
            string combined = string.Concat(RelatedProducts, UpsellProducts); 

            if (combined.Length != 0 && CartType == CartTypeEnum.ShoppingCart)
            {
                string[] arrUpsell = combined.Split(',');
                foreach (string s in arrUpsell)
                {
                    string PID = s.Trim();
                    if (PID.Length == 0) { continue; }

                    int UpsellProductID;
                    try
                    {
                        UpsellProductID = Localization.ParseUSInt(PID);
                        if (UpsellProductID != 0)
                        {
                            string ItemCode = InterpriseHelper.GetInventoryItemCode(UpsellProductID);
                            string itemUnitMeasure = string.Empty;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT ium.UnitMeasureCode FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryUnitMeasure ium with (NOLOCK) ON i.ItemCode = ium.ItemCode AND IsBase = 1 WHERE i.ItemCode = {0}", DB.SQuote(ItemCode)))
                                {
                                    if (reader.Read())
                                    {
                                        itemUnitMeasure = DB.RSField(reader, "UnitMeasureCode");
                                    }
                                }
                            }

                            cart.AddItem(ThisCustomer, ShippingAddressID, ItemCode, UpsellProductID, 1, itemUnitMeasure, CartType);
                        }
                    }
                    catch { }
                }
            }
        }

        GoNextPage(context, itemIsARegistryItem, CartType, ThisCustomer);
    }

    private void CheckOverSizedItemForGiftRegistry(string itemCode)
    {
        var unitMeasures = ServiceFactory.GetInstance<IInventoryRepository>()
                                                 .GetItemBaseUnitMeasures(itemCode);
        var defaultUm = unitMeasures.FirstOrDefault();
        if (defaultUm != null)
        {
            var shippingMethodOverSize = ServiceFactory.GetInstance<IShippingService>()
                                                       .GetOverSizedItemShippingMethod(itemCode, defaultUm.Code);
            if (shippingMethodOverSize != null && shippingMethodOverSize.FreightChargeType.ToUpperInvariant() == DomainConstants.PICKUP_FREIGHT_CHARGE_TYPE)
            {
                throw new ArgumentException("Securit Error: Pickup Oversized item cannot be added as Gift Registry Item");
            }
        }
    }

    private void GoNextPage(HttpContext context, bool itemIsARegistryItem = false, CartTypeEnum cartType = CartTypeEnum.ShoppingCart, Customer ThisCustomer = null)
    {
        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");        
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }

        CartTypeEnum CartType = CartTypeEnum.ShoppingCart;
        if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
        {
            CartType = CartTypeEnum.WishCart;
        }

        bool isAddRegistryItem = (cartType == CartTypeEnum.ShoppingCart && itemIsARegistryItem);
        if ((isAddRegistryItem) ||
                ("STAY".Equals(AppLogic.AppConfig("AddToCartAction"), StringComparison.InvariantCultureIgnoreCase) && ReturnURL.Length != 0))
        {
            string addedParam = string.Empty;
            if (isAddRegistryItem)
            {
                addedParam = "&" + DomainConstants.NOTIFICATION_QRY_STRING_PARAM + "=" + AppLogic.GetString("editgiftregistry.aspx.48");
            }
            context.Response.Redirect(ReturnURL + addedParam);
        }
        else
        {
            if (ReturnURL.Length == 0)
            {
                ReturnURL = string.Empty;
                if (context.Request.UrlReferrer != null)
                {
                    ReturnURL = context.Request.UrlReferrer.AbsoluteUri; // could be null
                }
                if (ReturnURL == null)
                {
                    ReturnURL = string.Empty;
                }
            }
            if (CartType == CartTypeEnum.WishCart)
            {
                context.Response.Redirect("wishlist.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            if (CartType == CartTypeEnum.GiftRegistryCart)
            {
                context.Response.Redirect("giftregistry.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            context.Response.Redirect("ShoppingCart.aspx?add=true&ReturnUrl=" + Security.UrlEncode(ReturnURL));
        }
    }
}
#endregion

#region ExecXmlPackage
/// <summary>
/// Outputs the raw package results along with setting any http headers specified in the package.
/// The package transform output method needs to match the Content-Type http header or you may not get the results you expect
/// </summary>
public class ExecXmlPackage : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        string pn = CommonLogic.QueryStringCanBeDangerousContent("xmlpackage");
        Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;
        try
        {
            using (XmlPackage2 p = new XmlPackage2(pn, ThisCustomer, ThisCustomer.SkinID, "", XmlPackageParam.FromString(""), "", true))
            {
                if (!p.AllowEngine)
                {
                    context.Response.Write("This XmlPackage is not allowed to be run from the engine.  Set the package element's allowengine attribute to true to enable this package to run.");
                }
                else
                {
                    if (p.HttpHeaders != null)
                    {
                        foreach (XmlNode xn in p.HttpHeaders)
                        {
                            string headername = xn.Attributes["headername"].InnerText;
                            string headervalue = xn.Attributes["headervalue"].InnerText;
                            context.Response.AddHeader(headername, headervalue);
                        }
                    }
                    string output = p.TransformString();
                    context.Response.AddHeader("Content-Length", output.Length.ToString());
                    context.Response.Write(output);
                }
            }
        }
        catch (Exception ex)
        {
            context.Response.Write(ex.Message + "<br/><br/>");
            Exception iex = ex.InnerException;
            while (iex != null)
            {
                context.Response.Write(ex.Message + "<br/><br/>");
                iex = iex.InnerException;
            }
        }
    }
}
#endregion

#region PaypalExpressCheckoutPostback

public class PaypalExpressCheckoutPostback : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {

        if (Customer.Current.ThisCustomerSession["paypalfrom"] == "onlinepayment")
        {
            context.Response.Redirect(String.Format("payment.aspx?invoicecode={0}&PayPal=True&token={1}&amount={2}", context.Request.QueryString["invoicecode"], context.Request.QueryString["token"], context.Request.QueryString["amount"]));
        }

        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;

        var m_PayPalExpress = new PayPalExpress();
        //Get PayPal info
        var PayPalDetails = m_PayPalExpress.GetExpressCheckoutDetails(context.Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
        var paypalShippingAddress = Address.New(ThisCustomer, AddressTypes.Shipping);

        if (PayPalDetails.PayerInfo.Address.Name.IsNullOrEmptyTrimmed() && (PayPalDetails.PayerInfo.Address.Street1.IsNullOrEmptyTrimmed() || PayPalDetails.PayerInfo.Address.Street2.IsNullOrEmptyTrimmed()) &&
            PayPalDetails.PayerInfo.Address.CityName.IsNullOrEmptyTrimmed() && PayPalDetails.PayerInfo.Address.StateOrProvince.IsNullOrEmptyTrimmed() && PayPalDetails.PayerInfo.Address.PostalCode.IsNullOrEmptyTrimmed() &&
            PayPalDetails.PayerInfo.Address.CountryName.ToString().IsNullOrEmptyTrimmed() || PayPalDetails.PayerInfo.ContactPhone.IsNullOrEmptyTrimmed())
        {
            paypalShippingAddress = ThisCustomer.PrimaryShippingAddress;
        }
        else
        {
            string streetAddress = PayPalDetails.PayerInfo.Address.Street1 + (!PayPalDetails.PayerInfo.Address.Street2.IsNullOrEmptyTrimmed() ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2;
            string sql = String.Empty;
            if (ThisCustomer.IsRegistered)
            {
                sql = String.Format("SELECT COUNT(ShipToCode) AS N FROM CustomerShipTo where Address = {0} and City = {1} and State = {2} and PostalCode = {3} and Country = {4} and ShipToName = {5} and CustomerCode = {6}",
                                streetAddress.ToDbQuote(), PayPalDetails.PayerInfo.Address.CityName.ToDbQuote(), PayPalDetails.PayerInfo.Address.StateOrProvince.ToDbQuote(), PayPalDetails.PayerInfo.Address.PostalCode.ToDbQuote(),
                                AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName).ToString().ToDbQuote(), PayPalDetails.PayerInfo.Address.Name.ToDbQuote(), ThisCustomer.CustomerCode.ToDbQuote());
            }
            else
            {
                sql = String.Format("SELECT COUNT(1) AS N FROM EcommerceAddress where ShipToAddress = {0} and ShipToCity = {1} and ShipToState = {2} and ShipToPostalCode = {3} and ShipToCountry = {4} and ShipToName = {5} and CustomerID = {6}",
                                streetAddress.ToDbQuote(), PayPalDetails.PayerInfo.Address.CityName.ToDbQuote(), PayPalDetails.PayerInfo.Address.StateOrProvince.ToDbQuote(), PayPalDetails.PayerInfo.Address.PostalCode.ToDbQuote(),
                                AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName).ToString().ToDbQuote(), PayPalDetails.PayerInfo.Address.Name.ToDbQuote(), ThisCustomer.CustomerCode.ToDbQuote());

                paypalShippingAddress.EMail = ThisCustomer.IsRegistered ? ThisCustomer.EMail : ServiceFactory.GetInstance<ICustomerService>().GetAnonEmail();
                paypalShippingAddress.Name = PayPalDetails.PayerInfo.Address.Name;
                paypalShippingAddress.Address1 = PayPalDetails.PayerInfo.Address.Street1 + (PayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2;
                paypalShippingAddress.City = PayPalDetails.PayerInfo.Address.CityName;
                paypalShippingAddress.State = PayPalDetails.PayerInfo.Address.StateOrProvince;
                paypalShippingAddress.PostalCode = PayPalDetails.PayerInfo.Address.PostalCode;
                paypalShippingAddress.Country = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName.ToString());
                paypalShippingAddress.ResidenceType = ThisCustomer.PrimaryShippingAddress.ResidenceType;
                paypalShippingAddress.Phone = PayPalDetails.PayerInfo.ContactPhone ?? String.Empty;

            }

            int isAddressExists = DB.GetSqlN(sql);

            if (AppLogic.AppConfigBool("PayPalCheckout.RequireConfirmedAddress") || isAddressExists == 0)
            {
                ServiceFactory.GetInstance<ICustomerService>()
                              .AssignPayPalExpressCheckoutNoteInSalesOrderNote();
            }
        }

        ThisCustomer.PrimaryShippingAddress = paypalShippingAddress;
        paypalShippingAddress.Save();

        string redirectUrl = String.Empty;

        //Checking for redirectURL of PayPal -- Express Checkout button in Shopping Cart page or PayPal Radio Button in Payment Page
       
        if (Customer.Current.ThisCustomerSession["paypalfrom"] == "shoppingcart" || Customer.Current.ThisCustomerSession["paypalfrom"] == "checkoutanon")
        {
            redirectUrl = "checkoutshipping.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
        }
        else
        {
            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                if (!AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"))
                {
                    //Insert PayPal call here for response - For authorize and capture of order from paypal inside IS
                    ThisCustomer.ThisCustomerSession["paypalfrom"] = "onepagecheckout";
                    string OrderNumber = String.Empty;
                    string status = String.Empty;
                    string receiptCode = String.Empty;
                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    Address shippingAddress = null;
                    var cart = new InterpriseShoppingCart(null, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
                    if (cart.IsNoShippingRequired())
                    {
                        cart.BuildSalesOrderDetails(false, true);
                    }
                    else
                    {
                        cart.BuildSalesOrderDetails();
                    }

                    if (!AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                    {
                        if (!cart.HasShippableComponents())
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                        else
                        {
                            if (ThisCustomer.IsRegistered)
                            {
                                var GetShippingAddress = new Address()
                                {
                                    Name = PayPalDetails.PayerInfo.Address.Name,
                                    Address1 = PayPalDetails.PayerInfo.Address.Street1 + (PayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2,
                                    City = PayPalDetails.PayerInfo.Address.CityName,
                                    State = PayPalDetails.PayerInfo.Address.StateOrProvince,
                                    PostalCode = PayPalDetails.PayerInfo.Address.PostalCode,
                                    Country = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName.ToString()),
                                    CountryISOCode = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.Country.ToString()),
                                    Phone = PayPalDetails.PayerInfo.ContactPhone ?? String.Empty
                                };
                                shippingAddress = GetShippingAddress;
                            }
                            else
                            {
                                shippingAddress = paypalShippingAddress;
                            }
                        }
                    }

                    var doExpressCheckoutResp = m_PayPalExpress.DoExpressCheckoutPayment(PayPalDetails.Token, PayPalDetails.PayerInfo.PayerID, OrderNumber, cart);
                    string result = String.Empty;
                    if (doExpressCheckoutResp.Errors != null && !doExpressCheckoutResp.Errors[0].ErrorCode.IsNullOrEmptyTrimmed())
                    {
                        if (AppLogic.AppConfigBool("ShowGatewayError"))
                        {
                            result = String.Format(AppLogic.GetString("shoppingcart.aspx.27"), doExpressCheckoutResp.Errors[0].ErrorCode, doExpressCheckoutResp.Errors[0].LongMessage);
                        }
                        else
                        {
                            result = AppLogic.GetString("shoppingcart.aspx.28");
                        }

                        context.Response.Redirect("shoppingcart.aspx?ErrorMsg=" + result.ToUrlEncode(), false);
                        return;
                    }
                    else
                    {
                        Gateway gatewayToUse = null;
                        var payPalResp = new GatewayResponse(String.Empty)
                        {
                            AuthorizationCode = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                            TransactionResponse = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                            Details = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                            AuthorizationTransID = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID
                        };

                        InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                        status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, payPalResp, true, false);

                        ThisCustomer.ThisCustomerSession["paypalFrom"] = String.Empty;
                        ThisCustomer.ThisCustomerSession["notesFromPayPal"] = String.Empty;
                        ThisCustomer.ThisCustomerSession["anonymousCustomerNote"] = String.Empty;

                        if (status != AppLogic.ro_OK)
                        {
                            ThisCustomer.IncrementFailedTransactionCount();
                            if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                            {
                                cart.ClearTransaction();
                                ThisCustomer.ResetFailedTransactionCount();
                                context.Response.Redirect("orderfailed.aspx");
                            }
                            ThisCustomer.ClearTransactions(false);
                            context.Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + status.ToUrlEncode());
                        }

                        AppLogic.ClearCardNumberInSession(ThisCustomer);
                        ThisCustomer.ClearTransactions(true);

                        context.Response.Redirect(String.Format("orderconfirmation.aspx?ordernumber={0}", OrderNumber.ToUrlEncode()));
                    }
                }
                else
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                    redirectUrl = "checkoutreview.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
                }
            }
            else
            {
                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                redirectUrl = "checkoutreview.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
            }
        }

        context.Response.Redirect(redirectUrl);
    }

}
#endregion

#region SagePayNotification

public class SagePayNotification : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var requestPost = context.Request.Form;

        string responseStatus = String.Empty;
        string redirectURL = String.Empty;
        bool isUseOnePageCheckout = ServiceFactory.GetInstance<IAppConfigService>().CheckoutUseOnePageCheckout;
        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().LoginFromSagePay(CommonLogic.QueryStringCanBeDangerousContent("contactguid"));

        if (requestPost["Status"] == DomainConstants.SAGEPAY_RESPONSE_STATUS_OK)
        {
            string securityKey = String.Format("\r\n" + "SecurityKey={0}", thisCustomer.ThisCustomerSession["SecurityKey"]);
            var sagepayResponse = new GatewayResponse(String.Empty)
            {
                AuthorizationCode = requestPost["TxAuthNo"],
                TransactionResponse = requestPost.ToString().ToUrlDecode().Replace("&", "\r\n") + securityKey,
                Details = requestPost["StatusDetail"],
                AuthorizationTransID = requestPost["VPSTxId"],
                AVSResult = requestPost["AVSCV2"],
                CV2Result = requestPost["CV2Result"],
                Status = requestPost["Status"],
                ExpirationMonth = requestPost["ExpiryDate"].Substring(0, 2),
                ExpirationYear = requestPost["ExpiryDate"].Substring(2, 2)
            };

            string sagepayResponseJSON = ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson<GatewayResponse>(sagepayResponse);
            thisCustomer.ThisCustomerSession["sagepayResponseJSON"] = sagepayResponseJSON;

            responseStatus = DomainConstants.SAGEPAY_RESPONSE_STATUS_OK;
            redirectURL = "sagepayredirect.aspx";
        }
        else if (requestPost["Status"] == DomainConstants.SAGEPAY_RESPONSE_STATUS_ABORT)
        {
            responseStatus = DomainConstants.SAGEPAY_RESPONSE_STATUS_OK;
            if (isUseOnePageCheckout)
            {
                redirectURL = "checkout1.aspx";
            }
            else
            {
                redirectURL = "checkoutpayment.aspx";
            }
        }
        else if (requestPost["Status"] == DomainConstants.SAGEPAY_RESPONSE_STATUS_NOTAUTHED)
        {
            responseStatus = DomainConstants.SAGEPAY_RESPONSE_STATUS_OK;
            if (isUseOnePageCheckout)
            {
                redirectURL = String.Format("checkout1.aspx?errormsg={0}", String.Format(AppLogic.GetString("checkout1.aspx.110", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting, true),
                                                                                        requestPost["Status"] + "+" + requestPost["StatusDetail"]).ToUrlEncode());
            }
            else
            {
                redirectURL = String.Format("checkoutpayment.aspx?errormsg={0}", String.Format(AppLogic.GetString("checkoutpayment.aspx.66", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting, true),
                                                                                        requestPost["Status"] + "+" + requestPost["StatusDetail"]).ToUrlEncode());
            }
        }
        else
        {
            responseStatus = DomainConstants.SAGEPAY_RESPONSE_STATUS_INVALID;
            if (isUseOnePageCheckout)
            {
                redirectURL = String.Format("checkout1.aspx?errormsg={0}", String.Format(AppLogic.GetString("checkout1.aspx.110", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting, true),
                                                                                        requestPost["Status"] + "+" + requestPost["StatusDetail"]).ToUrlEncode());
            }
            else
            {
                redirectURL = String.Format("checkoutpayment.aspx?errormsg={0}", String.Format(AppLogic.GetString("checkoutpayment.aspx.66", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting, true),
                                                                                        requestPost["Status"] + "+" + requestPost["StatusDetail"]).ToUrlEncode());
            }
        }

        context.Response.Clear();
        context.Response.ClearHeaders();
        context.Response.ContentType = "text/plain";
        context.Response.Write(String.Format("Status={0}\n", responseStatus));
        
        if (!thisCustomer.ThisCustomerSession["IsRequestingFromMobile"].IsNullOrEmptyTrimmed())
        {
            context.Response.Write(String.Format("RedirectURL={0}{1}", CurrentContext.FullyQualifiedMobileApplicationPath() + "/", redirectURL));
        }
        else
        {
            context.Response.Write(String.Format("RedirectURL={0}{1}", CurrentContext.FullyQualifiedApplicationPath(), redirectURL));
        }
    }
}

#endregion

#region SagePayOrderCreation

public class SagePayOrderCreation : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;

        var cart = new InterpriseShoppingCart(null, 1, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
        cart.BuildSalesOrderDetails();

        var deserializedSagePayResponse = ServiceFactory.GetInstance<ICryptographyService>().DeserializeJson<GatewayResponse>(ThisCustomer.ThisCustomerSession["sagepayResponseJSON"]);

        string orderNumber = String.Empty;
        string receiptCode = String.Empty;
        string orderStatus = String.Empty;
        var billingAddress = ThisCustomer.PrimaryBillingAddress;
        var shippingAddress = ThisCustomer.PrimaryShippingAddress;

        billingAddress.CardExpirationMonth = deserializedSagePayResponse.ExpirationMonth;
        billingAddress.CardExpirationYear = System.Threading.Thread.CurrentThread
                                                                   .CurrentCulture.Calendar.ToFourDigitYear(int.Parse(deserializedSagePayResponse.ExpirationYear)).ToString();

        InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm);
        orderStatus = cart.PlaceOrder(null, billingAddress, shippingAddress, ref orderNumber, ref receiptCode, true, true, deserializedSagePayResponse, true, false);

        if (orderStatus == AppLogic.ro_OK)
        {
            ThisCustomer.ClearTransactions(true);
            context.Response.Redirect(String.Format("orderconfirmation.aspx?ordernumber={0}", orderNumber.ToUrlEncode()));
        }
        else
        {
            if (ServiceFactory.GetInstance<IAppConfigService>().CheckoutUseOnePageCheckout)
            {
                context.Response.Redirect(String.Format("checkout1.aspx?paymentTerm={0}&errormsg={1}", ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm, orderStatus.ToUrlEncode()));
            }
            else
            {
                context.Response.Redirect(String.Format("checkoutpayment.aspx?paymentTerm={0}&errormsg={1}", ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm, orderStatus.ToUrlEncode()));
            }
        }
    }
}

#endregion






#region AddBundletoCart

public class AddBundletoCart : IHttpHandler
{
    //class BundleItem
    //{
    //    public int Counter { get; set; }
    //    public string ItemCode { get; set; }
    //    public string ItemType { get; set; }
    //    public string UnitMeasureCode { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal SalesPrice { get; set; }
    //    public decimal FreeStock { get; set; }
    //    public string MatrixGroupCode { get; set; }
    //    public string MatrixItemCode { get; set; }

    //}
    #region Declaration

    private INavigationService _navigationService = null;
    private IStringResourceService _stringResourceService = null;
    private IShoppingCartService _shoppingCartService = null;
    private IProductService _productService = null;
    private IInventoryRepository _inventoryRepository = null;
    #endregion

    public bool IsReusable
    {
        get { return true; }
    }

    private void InitializeDomainServices()
    {
        _navigationService = ServiceFactory.GetInstance<INavigationService>();
        _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        _productService = ServiceFactory.GetInstance<IProductService>();
        _inventoryRepository = ServiceFactory.GetInstance<IInventoryRepository>();

        
    }
   
    private bool DoesBundleExist(string itemCode)
    {
        bool itemExisting = false;
        using (var con = DB.NewSqlConnection())
        {
            con.Open();
            using (var reader = DB.GetRSFormat(con, "SELECT i.ItemCode FROM InventoryItem i with (NOLOCK) WHERE i.ItemCode = {0}", itemCode.ToDbQuote()))
            {
                itemExisting = reader.Read();
            }
        }
        return itemExisting;
    }
    private bool DoesBundleItemExist(string itemCode)
    {
        bool itemExisting = false;
        using (var con = DB.NewSqlConnection())
        {
            con.Open();
            using (var reader = DB.GetRSFormat(con, "SELECT i.ItemCode FROM InventoryItem i with (NOLOCK) WHERE i.ItemCode = {0}", DB.SQuote(itemCode)))
            {
                itemExisting = reader.Read();
            }
        }
        return itemExisting;
    }

    private string GetMatrixGroupCode(string matrixItemCode)
    {
        string itemCode = string.Empty;
        using (var con = DB.NewSqlConnection())
        {
            con.Open();
            using (var reader = DB.GetRSFormat(con, "SELECT ItemCode FROM InventoryMatrixItem WHERE MatrixItemCode  = {0}", DB.SQuote(matrixItemCode)))
            {
                if (reader.Read())
                {
                    itemCode = reader.ToRSField("ItemCode");
                }
            }
        }
        return itemCode;
    }

    private System.Collections.Generic.List<object> GetMatrixItems(string matrixGroupCode)
    {
        System.Collections.Generic.List<object> listItemCodes = new System.Collections.Generic.List<object>();
        using (var con = DB.NewSqlConnection())
        {
            con.Open();
            using (var reader = DB.GetRSFormat(con, "SELECT II.Counter, IMI.MatrixItemCode FROM InventoryMatrixItem IMI INNER JOIN InventoryItem II ON IMI.MatrixItemCode = II.ItemCode WHERE IMI.ItemCode  = {0}", DB.SQuote(matrixGroupCode)))
            {
                while (reader.Read())
                {
                    listItemCodes.Add(new object[] { reader.ToRSFieldInt("Counter"), reader.ToRSField("MatrixItemCode") });
                }
            }
        }
        return listItemCodes;
    }
    private System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem> GetBundleItems2(string bundleCode)
    {
        var items = new System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem>();
        bool showMatrixGroup = AppLogic.AppConfigBool("EnableMatrixItemInBundle");
        using (System.Data.SqlClient.SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (System.Data.IDataReader reader = DB.GetRSFormat(con, "EXEC ReadInventoryBundleConfigurator @BundleCode={0}, @DefaultPrice={1}, @LanguageCode={2}, @CurrencyCode={3}, @WarehouseCode={4}",
                bundleCode.ToDbQuote(), Customer.Current.DefaultPrice.ToDbQuote(), Customer.Current.LanguageCode.ToDbQuote(), Customer.Current.CurrencyCode.ToDbQuote(), Customer.Current.WarehouseCode.ToDbQuote()))
            {
                while (reader.Read())
                {
                    if (!showMatrixGroup)
                    {
                        if (reader.ToRSField("ItemType").ToLower() == "matrix item") continue;
                    }
                    //Bundle Item automatically divides Wholesale/Retail(SalesPrice) value over Quantity
                    decimal productPrice = reader.ToRSFieldDecimal("SalesPrice") * reader.ToRSFieldDecimal("Quantity");
                    decimal prodcutExtPrice = productPrice;
                    items.Add(new InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem
                    {
                        BundleCounter = _inventoryRepository.GetInventoryItemCounter(reader.ToRSField("BundleCode")),
                        BundleItemCode = reader.ToRSField("BundleCode"),
                        Counter = _inventoryRepository.GetInventoryItemCounter(reader.ToRSField("ItemCode")),
                        ItemCode = reader.ToRSField("ItemCode"),
                        UnitMeasureCode = reader.ToRSField("UnitMeasureCode"),
                        UnitMeasureQuantity = reader.ToRSFieldDecimal("UnitMeasureQty"),
                        Quantity = reader.ToRSFieldDecimal("Quantity"),
                        ItemType = reader.ToRSField("ItemType"),
                        FreeStock = reader.ToRSFieldDecimal("FreeStock"),
                        ProductPrice = productPrice,
                        ProductExtPrice = prodcutExtPrice,
                    });
                }
            }
            con.Close();
        }
      
        return items;
    }
    private System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem> GetBundleItems(string bundleCode)
    {
        var items = new System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem>();
        bool showMatrixGroup = AppLogic.AppConfigBool("EnableMatrixItemInBundle");


        string script = string.Format("EXEC ReadInventoryItemBundleUnified @BundleCode={0},@LanguageCode={1}", bundleCode.ToDbQuote(), Customer.Current.LanguageCode.ToDbQuote());
        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (var dataSet = DB.GetDS(script, false))
            {

                var bundleItems = from header in dataSet.Tables[1].Rows.Cast<DataRow>()
                                  join detail in dataSet.Tables[2].Rows.Cast<DataRow>() on header.Field<string>("ItemCode") equals detail.Field<string>("ItemName")
                                  orderby header.Field<int>("LineNum") ascending
                                  select new { Header = header, Detail = detail };

                foreach (var bundleItem in bundleItems)
                {
                    string itemCode = bundleItem.Detail.Field<string>("ItemName");
                    int counter = ServiceFactory.GetInstance<IInventoryRepository>().GetInventoryItemCounter(itemCode);
                    string itemType = ServiceFactory.GetInstance<IInventoryRepository>().GetInventoryItemType(itemCode);
                    decimal unitMeasureQty = BundleProductPage.GetItemUnitMeasureQuantity(itemCode, bundleItem.Detail.Field<string>("UnitMeasureCode"));

                    if (!showMatrixGroup)
                    {
                        if (itemType == "Matrix Group") continue;
                    }
                    decimal salesPrice = Customer.Current.DefaultPrice.ToUpper() == "WHOLESALE" ? bundleItem.Detail.Field<decimal>("WholeSalePrice") : bundleItem.Detail.Field<decimal>("RetailPrice");

                    decimal productPrice = salesPrice * bundleItem.Header.Field<decimal>("Quantity");
                    decimal prodcutExtPrice = productPrice;
                    items.Add(new InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem
                    {
                        BundleCounter = _inventoryRepository.GetInventoryItemCounter(bundleCode),
                        BundleItemCode = bundleCode,
                        Counter = _inventoryRepository.GetInventoryItemCounter(itemCode),
                        ItemCode = itemCode,
                        UnitMeasureCode = bundleItem.Detail.Field<string>("UnitMeasureCode"),
                        UnitMeasureQuantity = unitMeasureQty,
                        Quantity = bundleItem.Header.Field<decimal>("Quantity"),
                        ItemType = itemType,
                        FreeStock = InterpriseHelper.GetInventoryFreeStock(itemCode, bundleItem.Detail.Field<string>("UnitMeasureCode"), Customer.Current),
                        ProductPrice = productPrice,
                        ProductExtPrice = prodcutExtPrice,
                    });
                }
            }
        }


        return items;
    }
    private bool AddItemToCart(HttpContext context, string bundleCode,int bundleHeaderID, InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem item, string defaultUnitMeasure, string shippingAddressID, decimal bundleQuantity, CartTypeEnum cartType, GiftRegistryItemType giftRegistryItemType)
    {
        bool valid = true;
        int counter = item.Counter;
        string itemCode = item.ItemCode;
        // get the unit measure code
        string unitMeasureCode = item.UnitMeasureCode;
        var customer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;

        // check if the unit measure is default so that we won't have to check
        // if the unit measure specified is valid...
        //if false == (unitMeasureCode == defaultUnitMeasure)
        if (false.Equals(unitMeasureCode.Equals(defaultUnitMeasure, StringComparison.InvariantCultureIgnoreCase)))
        {
            bool isValidUnitMeasureForThisItem = false;

            // if no unit measure was passed use DefaultSelling
            string sqlQuery = string.Empty;
            if (unitMeasureCode.IsNullOrEmptyTrimmed())
            {
                sqlQuery = String.Format("SELECT UnitMeasureCode FROM InventoryUnitMeasure with (NOLOCK) WHERE ItemCode= {0} AND DefaultSelling = 1", DB.SQuote(itemCode));
            }
            else
            {
                sqlQuery = String.Format("SELECT UnitMeasureCode FROM InventoryUnitMeasure with (NOLOCK) WHERE ItemCode= {0} AND UnitMeasureCode = {1}", DB.SQuote(itemCode), DB.SQuote(unitMeasureCode));
            }

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, sqlQuery))
                {
                    isValidUnitMeasureForThisItem = reader.Read();

                    if (isValidUnitMeasureForThisItem)
                    {
                        // maybe mixed case specified, just set..
                        unitMeasureCode = reader.ToRSField("UnitMeasureCode");
                    }
                }
            }


            if (!isValidUnitMeasureForThisItem)
            {
                GoNextPage(context);
            }
        }

        decimal quantity = item.Quantity;
        if (quantity == 0)
            quantity = CommonLogic.FormNativeDecimal("Quantity");

        if (quantity == 0)
            quantity = 1;

        quantity = CommonLogic.RoundQuantity(quantity);
        
        // Now let's check the shipping address if valid if specified
        if (shippingAddressID != customer.PrimaryShippingAddressID)
        {
            if (customer.IsRegistered)
            {
                bool shippingAddressIsValidForThisCustomer = false;

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT ShipToCode FROM CustomerShipTo with (NOLOCK) WHERE CustomerCode = {0} AND IsActive = 1 AND ShipToCode = {1}", DB.SQuote(customer.CustomerCode), DB.SQuote(shippingAddressID)))
                    {
                        shippingAddressIsValidForThisCustomer = reader.Read();

                        if (shippingAddressIsValidForThisCustomer)
                        {
                            // maybe mixed case, just set...
                            shippingAddressID = reader.ToRSField("ShipToCode");
                        }
                    }
                }

                if (!shippingAddressIsValidForThisCustomer)
                {
                    GoNextPage(context);
                }
            }
            else
            {
                shippingAddressID = customer.PrimaryShippingAddressID;
            }
        }

   

        ShoppingCart cart = null;
        bool itemIsARegistryItem = false;
        if (!itemCode.IsNullOrEmptyTrimmed())
        {
            #region " --GIFTREGISTRY-- "

            if (cartType == CartTypeEnum.GiftRegistryCart)
            {
                CheckOverSizedItemForGiftRegistry(itemCode);

                Guid? registryID = CommonLogic.FormCanBeDangerousContent("giftregistryOptions").TryParseGuid();
                if (registryID.HasValue)
                {
                    var selectedGiftRegistry = customer.GiftRegistries.FindFromDb(registryID.Value);
                    if (selectedGiftRegistry != null)
                    {
                        GiftRegistryItem registryItem = null;
                        if (registryItem == null)
                        {
                            registryItem = selectedGiftRegistry.GiftRegistryItems.FirstOrDefault(giftItem => giftItem.ItemCode == itemCode && giftItem.GiftRegistryItemType == giftRegistryItemType);
                        }

                        if (registryItem != null)
                        {
                            registryItem.Quantity += quantity;
                            registryItem.UnitMeasureCode = unitMeasureCode;
                            selectedGiftRegistry.GiftRegistryItems.UpdateToDb(registryItem);
                        }
                        else
                        {
                            registryItem = new GiftRegistryItem()
                            {
                                GiftRegistryItemType = giftRegistryItemType,
                                RegistryItemCode = Guid.NewGuid(),
                                ItemCode = itemCode,
                                Quantity = quantity,
                                RegistryID = registryID.Value,
                                UnitMeasureCode = unitMeasureCode
                            };

                            selectedGiftRegistry.GiftRegistryItems.AddToDb(registryItem);
                        }
                        HttpContext.Current.Response.Redirect(string.Format("~/editgiftregistry.aspx?{0}={1}", DomainConstants.GIFTREGISTRYPARAMCHAR, registryID.Value));
                    }
                }

                GoNextPage(context);
            }

            #endregion

            CartRegistryParam registryCartParam = null;
            if (AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                registryCartParam = new CartRegistryParam()
                {
                    RegistryID = CommonLogic.FormGuid("RegistryID"),
                    RegistryItemCode = CommonLogic.FormGuid("RegistryItemCode")
                };
            }

            if (registryCartParam != null && registryCartParam.RegistryID.HasValue && registryCartParam.RegistryItemCode.HasValue)
            {
                shippingAddressID = GiftRegistryDA.GetPrimaryShippingAddressCodeOfOwnerByRegistryID(registryCartParam.RegistryID.Value);
                itemIsARegistryItem = true;

                //Automatically clear the itemcart with warehouse code if added a registry item.
                _shoppingCartService.ClearCartWarehouseCodeByCustomer();
            }

            cart = new ShoppingCart(null, 1, customer, cartType, string.Empty, false, true, string.Empty);
            
            if (quantity > 0)
            {

                //if (!string.IsNullOrEmpty(item.AlternateItemCode))
                //{
                //    var matrixitemPrice = BundleProductPage.GetPriceSelectedMatrixItem(item.BundleItemCode, item.ItemCode, item.AlternateItemCode);
                //    item.ProductPrice = matrixitemPrice;
                //}
              

                item.BundleQuantity = bundleQuantity;
                item.ProductExtPrice = item.ProductPrice * item.BundleQuantity;
                item.BundleHeaderID = bundleHeaderID;
                cart.AddItem(customer, shippingAddressID, itemCode, counter, quantity, unitMeasureCode, cartType, null, registryCartParam, bundle: item);

            }

            #region for Related and UpSell Products Only
            string RelatedProducts = CommonLogic.QueryStringCanBeDangerousContent("relatedproducts").Trim();
            string UpsellProducts = CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim();
            string combined = string.Concat(RelatedProducts, UpsellProducts);

            if (combined.Length != 0 && cartType == CartTypeEnum.ShoppingCart)
            {
                string[] arrUpsell = combined.Split(',');
                foreach (string s in arrUpsell)
                {
                    string PID = s.Trim();
                    if (PID.Length == 0) { continue; }

                    int UpsellProductID;
                    try
                    {
                        UpsellProductID = Localization.ParseUSInt(PID);
                        if (UpsellProductID != 0)
                        {
                            string ItemCode = InterpriseHelper.GetInventoryItemCode(UpsellProductID);
                            string itemUnitMeasure = string.Empty;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT ium.UnitMeasureCode FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryUnitMeasure ium with (NOLOCK) ON i.ItemCode = ium.ItemCode AND IsBase = 1 WHERE i.ItemCode = {0}", DB.SQuote(ItemCode)))
                                {
                                    if (reader.Read())
                                    {
                                        itemUnitMeasure = reader.ToRSField("UnitMeasureCode");
                                    }
                                }
                            }

                            cart.AddItem(customer, shippingAddressID, ItemCode, UpsellProductID, 1, itemUnitMeasure, cartType);
                        }
                    }
                    catch { }
                }
            }
            #endregion
        }

       // GoNextPage(context, itemIsARegistryItem, CartType, customer);

        return valid;
    }
    private void NavigateToShoppingCartWithErrorMessage()
    {
        _navigationService.NavigateToShoppingCartWitErroMessage(_stringResourceService.GetString("shoppingcart.cs.62"));
    }

    private void NavigateToBundleProductWithErrorMessage(HttpContext context , string message)
    {
       


        var nameValueCollection = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());
        nameValueCollection.Remove("errormsg");
        nameValueCollection.Add("errormsg", message);
        string referrer = HttpContext.Current.Request.UrlReferrer.ToString();
        if (referrer.IndexOf('?') != -1)
        {
            referrer = referrer.Substring(0, referrer.IndexOf('?'));
        }
        nameValueCollection.Remove("returnurl");
        nameValueCollection.Remove("sename");
        
        string url = referrer + "?" + nameValueCollection;
        context.Response.Redirect(url);
    }
    public void ProcessRequest(HttpContext context)
    {
        InitializeDomainServices();

        context.Response.CacheControl = "private";
        context.Response.Expires = 0;
        context.Response.AddHeader("pragma", "no-cache");

        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;
        ThisCustomer.RequireCustomerRecord();

        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }

        //Anonymous users should not be allowed to used WishList, they must register first.
        if (ThisCustomer.IsNotRegistered)
        {
            string ErrMsg = string.Empty;

            if (CommonLogic.QueryStringCanBeDangerousContent("Source") == "addtowishlist")
            {
                ErrMsg = AppLogic.GetString("signin.aspx.19");
                context.Response.Redirect("signin.aspx?ErrorMsg=" + ErrMsg + "&ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
        }

        string ShippingAddressID = "ShippingAddressID".ToQueryString(); // only used for multi-ship
        if (ShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = CommonLogic.FormCanBeDangerousContent("ShippingAddressID");
        }

        if (ShippingAddressID.IsNullOrEmptyTrimmed() && !ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
        }


        string ProductID = "ProductID".ToQueryString();
        if (ProductID.IsNullOrEmptyTrimmed())
        {
            ProductID = CommonLogic.FormCanBeDangerousContent("ProductID");
        }

        string bundleJsonString = "Bundle".ToQueryString();
        if (string.IsNullOrEmpty(bundleJsonString))
            NavigateToBundleProductWithErrorMessage(context, "Please choose attributes from the items");

        dynamic bundleJsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(bundleJsonString);

        string bundleCode = (string)bundleJsonData.BundleCode;
        if (string.IsNullOrEmpty(bundleCode))
        {
            NavigateToShoppingCartWithErrorMessage();
        }

        bool bundleExists = DoesBundleExist(bundleCode);

        if (!bundleExists)
        {
            NavigateToBundleProductWithErrorMessage(context, "Unable to find this product. Kindly refresh your browser and try again.");
        }

            bool isWholeSale = Customer.Current.DefaultPrice == "Wholesale";
        var bundleProduct = BundleProductPage.GetBundleProduct(bundleCode);

        System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem> collectedItems = new System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem>();
        var bundleItemsSubmitted = ((Newtonsoft.Json.Linq.JArray)bundleJsonData.Items).ToList<dynamic>();
        foreach (dynamic item in bundleItemsSubmitted)
        {
        
            string itemCode = (string)item.ItemCode;
            string alternateItemCode = (string)item.AlternateItemCode;
            string matrixGroup = (string)item.MatrixGroup;
            int? shoppingCartRecID = (int?)item.ShoppingCartRecID;
            
            var newItem = new InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem();
            newItem.BundleCounter = bundleProduct.BundleCounter;
            newItem.BundleItemCode = bundleProduct.BundleCode;
            
            if (!string.IsNullOrEmpty(matrixGroup) && !string.IsNullOrEmpty(alternateItemCode) && !string.IsNullOrEmpty(itemCode))
            {
                var matrixGroupItem = bundleProduct.Items.Where(t => t.ItemCode == matrixGroup).Select(t => (t as BundleMatrixGroup)).First();
                var selectedMatrixItem = matrixGroupItem.MatrixItems.Where(t => t.ItemCode == alternateItemCode).First();

                newItem.Counter = matrixGroupItem.Counter;
                newItem.ItemCode = matrixGroupItem.ItemCode;
                newItem.ItemType = matrixGroupItem.ItemType;
                newItem.FreeStock = InterpriseHelper.GetInventoryFreeStock(matrixGroupItem.ItemCode, matrixGroupItem.UnitMeasureCode, Customer.Current);

                newItem.AlternateCounter = selectedMatrixItem.Counter;
                newItem.AlternateItemCode = selectedMatrixItem.ItemCode;
                newItem.AlternateFreeStock = InterpriseHelper.GetInventoryFreeStock(selectedMatrixItem.ItemCode, matrixGroupItem.UnitMeasureCode, Customer.Current);

                decimal productPrice = isWholeSale ? selectedMatrixItem.WholeSalePrice * matrixGroupItem.Quantity : selectedMatrixItem.RetailPrice * matrixGroupItem.Quantity;//
                decimal prodcutExtPrice = productPrice;

                newItem.ProductPrice = productPrice;
                newItem.ProductExtPrice = prodcutExtPrice;
                newItem.Quantity = matrixGroupItem.Quantity;

                newItem.UnitMeasureCode = matrixGroupItem.UnitMeasureCode;
                newItem.UnitMeasureQuantity = InterpriseHelper.GetItemDefaultUnitMeasure(matrixGroupItem.ItemCode).Quantity;

            }
            else
            {
                var bundleItem = bundleProduct.Items.Where(t => t.ItemCode == itemCode).First();
              

                newItem.Counter = bundleItem.Counter;
                newItem.ItemCode = bundleItem.ItemCode;
                newItem.ItemType = bundleItem.ItemType;
                newItem.FreeStock = InterpriseHelper.GetInventoryFreeStock(bundleItem.ItemCode, bundleItem.UnitMeasureCode, Customer.Current);



                decimal productPrice = isWholeSale ? bundleItem.WholeSalePrice * bundleItem.Quantity : bundleItem.RetailPrice * bundleItem.Quantity;
                decimal prodcutExtPrice = productPrice;

                newItem.ProductPrice = productPrice;
                newItem.ProductExtPrice = prodcutExtPrice;
                newItem.Quantity = bundleItem.Quantity;

                newItem.UnitMeasureCode = bundleItem.UnitMeasureCode;
                newItem.UnitMeasureQuantity = InterpriseHelper.GetItemDefaultUnitMeasure(bundleItem.ItemCode).Quantity;
            }
            newItem.ShoppingCartRecID = shoppingCartRecID == null ? "" : shoppingCartRecID.Value.ToString();
            collectedItems.Add(newItem);

        }

        if (collectedItems.Count != bundleProduct.Items.Count)
        {
            NavigateToBundleProductWithErrorMessage(context, "Unable to find some items in this Bundle Product. Kindly recheck the selected attributes of the items or refresh your browser and try again.");
        }

        //decimal totalPriceCollected = collectedItems.Sum(t => t.ProductPrice);
        //decimal totalPriceBundle = bundleProduct.Items.Sum(t => isWholeSale ? t.WholeSalePrice : t.RetailPrice);
        //if (totalPriceCollected > totalPriceBundle)
        //{
        //    NavigateToBundleProductWithErrorMessage(context, "Unable to find some items in this Bundle Product. Kindly recheck the selected attributes of the items or refresh your browser and try again.");
        //}

        if (ThisCustomer.IsNotRegistered)
        {
            var item = _productService.GetInventoryItem(bundleCode);
            if (item != null)
            {
                // do not allow unregistered customer to add giftcard and giftcertificate item to cart
                if (item.ItemType.EqualsIgnoreCase(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CARD) ||
                    item.ItemType.EqualsIgnoreCase(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CERTIFICATE))
                {
                    string message = AppLogic.GetString("signin.aspx.23");
                    _navigationService.NavigateToSignin(message);
                }
            }
        }

        //  DeleteShoppingCartItemsIfExisting(ThisCustomer, bundleCode);

        //decimal bundleQuantity = CommonLogic.FormLocaleDecimal("Quantity", ThisCustomer.LocaleSetting);

        //if (bundleQuantity == 0)
        //    bundleQuantity = CommonLogic.FormNativeDecimal("Quantity");

        //if (bundleQuantity == 0)
        //    bundleQuantity = 1;

        //bundleQuantity = CommonLogic.RoundQuantity(bundleQuantity);

        decimal bundleQuantity = "Quantity".ToQueryString().ToDecimal();
        bundleQuantity = bundleQuantity < 1 ? 1 : bundleQuantity;

        SourceSwitchMethod(
            actionProductPage: () =>
            {
                var cartType = CartTypeEnum.ShoppingCart;
                if (CommonLogic.QueryStringCanBeDangerousContent("Source") == "addtowishlist")
                {
                    cartType = CartTypeEnum.WishCart;
                }

                var giftRegistryItemType = GiftRegistryItemType.vItem;
                if (CommonLogic.FormNativeInt("IsAddToGiftRegistry") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistry") == 1)
                {
                    cartType = CartTypeEnum.GiftRegistryCart;
                }

                if (CommonLogic.FormNativeInt("IsAddToGiftRegistryOption") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistryOption") == 1)
                {
                    cartType = CartTypeEnum.GiftRegistryCart;
                    giftRegistryItemType = GiftRegistryItemType.vOption;
                }

                if (cartType == CartTypeEnum.ShoppingCart)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ClickSource") == "addtowishlist")
                    {
                        cartType = CartTypeEnum.WishCart;
                    }
                }


                int bundleHeaderID = GetShoppingCartBundleHeaderID(bundleCode, cartType, ThisCustomer);
                foreach (var item in collectedItems)
                {
                    AddItemToCart(context, bundleCode, bundleHeaderID, item, string.Empty, ShippingAddressID, bundleQuantity, cartType, giftRegistryItemType);
                }

                if (cartType == CartTypeEnum.WishCart)
                {
                    context.Response.Redirect("wishlist.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
                }
                if (cartType == CartTypeEnum.GiftRegistryCart)
                {
                    context.Response.Redirect("giftregistry.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
                }
                context.Response.Redirect("ShoppingCart.aspx?add=true&ReturnUrl=" + Security.UrlEncode(ReturnURL));

            },
            actionShoppingCart: () =>
            {
                int bundleHeaderID= int.Parse(CommonLogic.QueryStringCanBeDangerousContent("BundleHeaderID"));

                UpdateShoppingCartItems(ThisCustomer, bundleCode,bundleHeaderID, bundleQuantity, collectedItems, (int)Convert.ChangeType(CartTypeEnum.ShoppingCart, CartTypeEnum.ShoppingCart.GetTypeCode()));
                context.Response.Redirect("ReturnUrl".ToQueryString());
            },
            actionWishlist: () =>
            {
                int bundleHeaderID= int.Parse(CommonLogic.QueryStringCanBeDangerousContent("BundleHeaderID"));
                UpdateShoppingCartItems(ThisCustomer, bundleCode,bundleHeaderID, bundleQuantity, collectedItems, (int)Convert.ChangeType(CartTypeEnum.WishCart, CartTypeEnum.ShoppingCart.GetTypeCode()));
                context.Response.Redirect(context.Request.UrlReferrer.ToString());
            });
    }

    private void SourceSwitchMethod(Action actionProductPage = null, Action actionShoppingCart = null, Action actionWishlist = null)
    {
        switch ("Source".ToQueryString().ToLower())
        {
            case "productpage":
                {
                    if (actionProductPage != null)
                        actionProductPage.Invoke();
                } break;

            case "shoppingcart":
                {
                    if (actionShoppingCart != null)
                        actionShoppingCart.Invoke();
                } break;

            case "wishlist":
                {
                    if (actionWishlist != null)
                        actionWishlist.Invoke();
                } break;
        }
    }

    private int GetShoppingCartBundleHeaderID(string bundleCode, CartTypeEnum cartType, Customer thisCustomer)
    {
        int bundleHeaderID = 1;
        string script = string.Format("SELECT MAX(BundleHeaderID) Count FROM  EcommerceShoppingCart where BundleCode={0} and CartType={1} and WebSiteCode={2} and  CustomerCode={3} and ContactCode={4}",
                 bundleCode.ToDbQuote(), ((int)cartType).ToString(), InterpriseHelper.ConfigInstance.WebSiteCode.ToDbQuote(), thisCustomer.CustomerCode.ToDbQuote(), thisCustomer.ContactCode.ToDbQuote());
        using (var con = DB.NewSqlConnection())
        {
            con.Open();
            using (var reader = DB.GetRSFormat(con, script))
            {
                if (reader.Read())
                {
                    bundleHeaderID = reader.ToRSFieldInt("Count");
                    bundleHeaderID++;
                }
            }
        }
        return bundleHeaderID;
    }

    private void DeleteShoppingCartItemsIfExisting(Customer customer, string bundleCode  )
    {
         string script = string.Format("DELETE FROM EcommerceShoppingCart WHERE BundleCode={0} AND CartType={1} AND CustomerCode={2} AND WebSiteCode={3} AND ContactCode={4}",  bundleCode.ToDbQuote(), 0, customer.CustomerCode.ToDbQuote(), InterpriseHelper.ConfigInstance.WebSiteCode.ToDbQuote(), customer.ContactCode.ToDbQuote());
         DB.ExecuteSQL(script);
    }

    private void UpdateShoppingCartItems(Customer customer, string bundleCode,int bundleHeaderID, decimal bundleQuantity, System.Collections.Generic.List<InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem> items, int cartType)
    {
        for (int i = 0; i < items.Count; i++)
        {
            InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel.BundleShoppingCartItem item = items[i];
            if (string.IsNullOrEmpty(item.ShoppingCartRecID)) continue;
            //string script = string.Format("UPDATE EcommerceShoppingCart " +
            //    "SET BundleQuantity={0} , BundleMatrixItemSelected={1} "+
            //    "WHERE NULLIF(BundleMatrixItemSelected,'') IS NOT NULL AND BundleCode={2} AND ItemCode={3} AND CartType={4} AND CustomerCode={5} AND WebSiteCode={6} AND ContactCode={7} AND BundleHeaderID={8} ",
            //    bundleQuantity, item.AlternateItemCode.ToDbQuote(),
            //    bundleCode.ToDbQuote(), item.ItemCode.ToDbQuote(), cartType, customer.CustomerCode.ToDbQuote(), InterpriseHelper.ConfigInstance.WebSiteCode.ToDbQuote(), customer.ContactCode.ToDbQuote(), bundleHeaderID);
            string script = string.Format("UPDATE EcommerceShoppingCart " +
             "SET BundleQuantity={0} , BundleMatrixItemSelected={1} " +
             "WHERE  ShoppingCartRecID={2}",
             bundleQuantity, item.AlternateItemCode.ToDbQuote(),
             item.ShoppingCartRecID);
            DB.ExecuteSQL(script);
        }
     
    
    }

    private void CheckOverSizedItemForGiftRegistry(string itemCode)
    {
        var unitMeasures = ServiceFactory.GetInstance<IInventoryRepository>()
                                                 .GetItemBaseUnitMeasures(itemCode);
        var defaultUm = unitMeasures.FirstOrDefault();
        if (defaultUm != null)
        {
            var shippingMethodOverSize = ServiceFactory.GetInstance<IShippingService>()
                                                       .GetOverSizedItemShippingMethod(itemCode, defaultUm.Code);
            if (shippingMethodOverSize != null && shippingMethodOverSize.FreightChargeType.ToUpperInvariant() == DomainConstants.PICKUP_FREIGHT_CHARGE_TYPE)
            {
                throw new ArgumentException("Securit Error: Pickup Oversized item cannot be added as Gift Registry Item");
            }
        }
    }

    private void GoNextPage(HttpContext context, bool itemIsARegistryItem = false, CartTypeEnum cartType = CartTypeEnum.ShoppingCart, Customer ThisCustomer = null)
    {

        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }

        CartTypeEnum CartType = CartTypeEnum.ShoppingCart;
        if (CommonLogic.QueryStringCanBeDangerousContent("Source") == "addtowishlist")
        {
            CartType = CartTypeEnum.WishCart;
        }

        bool isAddRegistryItem = (cartType == CartTypeEnum.ShoppingCart && itemIsARegistryItem);
        if ((isAddRegistryItem) ||
                ("STAY".Equals(AppLogic.AppConfig("AddToCartAction"), StringComparison.InvariantCultureIgnoreCase) && ReturnURL.Length != 0))
        {
            string addedParam = string.Empty;
            if (isAddRegistryItem)
            {
                addedParam = "&" + DomainConstants.NOTIFICATION_QRY_STRING_PARAM + "=" + AppLogic.GetString("editgiftregistry.aspx.48");
            }
            context.Response.Redirect(ReturnURL + addedParam);
        }
        else
        {
            if (ReturnURL.Length == 0)
            {
                ReturnURL = string.Empty;
                if (context.Request.UrlReferrer != null)
                {
                    ReturnURL = context.Request.UrlReferrer.AbsoluteUri; // could be null
                }
                if (ReturnURL == null)
                {
                    ReturnURL = string.Empty;
                }
            }
            if (CartType == CartTypeEnum.WishCart)
            {
                context.Response.Redirect("wishlist.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            if (CartType == CartTypeEnum.GiftRegistryCart)
            {
                context.Response.Redirect("giftregistry.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            context.Response.Redirect("ShoppingCart.aspx?add=true&ReturnUrl=" + Security.UrlEncode(ReturnURL));
        }
    }
}
#endregion