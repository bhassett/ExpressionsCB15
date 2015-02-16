// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Interprise.Framework.ECommerce.DatasetGateway;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain;
using InterpriseSuiteEcommerceCommon.Domain.Model;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;
using System.Collections;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
/// <summary>
/// Summary description for Action
/// </summary>
[WebService(Namespace = "http://www.interprisesuite.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class ActionService : System.Web.Services.WebService
{
    private const string INVALID_EMAIL = "invalid-email";
    private const string EMAIL_DUPLICATES = "email-duplicates";

    public ActionService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// Gets the list of available shipping method options
    /// </summary>
    /// <param name="overrideDefaultAddress"></param>
    /// <param name="id"></param>
    /// <param name="addressId"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public ShippingMethodDTOCollection ShippingMethod(string addresNameValuePairOverride, string id, string addressId)
    {
        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>()
                                         .GetCurrentLoggedInCustomer();

        var cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                 .New(CartTypeEnum.ShoppingCart, true);

        cart.BuildSalesOrderDetails(false, true, thisCustomer.CouponCode,true);

        bool overrideDefaultAddress = !addresNameValuePairOverride.IsNullOrEmptyTrimmed();

        var giftRegistryItem = cart.CartItems
                                .Where(itm => itm.GiftRegistryID.HasValue)
                                .FirstOrDefault(r => r.m_ShippingAddressID == addressId);

        Guid? giftRegistryItemID = (giftRegistryItem != null && giftRegistryItem.GiftRegistryID.HasValue) ? giftRegistryItem.GiftRegistryID : null;

        Address preferredAddress = null;
        if (overrideDefaultAddress)
        {
            var addressNameValuePair = HttpUtility.ParseQueryString(addresNameValuePairOverride);
            preferredAddress = Address.FromForm(thisCustomer, AddressTypes.Shipping, addressNameValuePair);
        }
        else if (!addressId.IsNullOrEmptyTrimmed())
        {
            if (giftRegistryItemID.HasValue)
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId, giftRegistryItemID);
            }
            else
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId);
            }

            if (cart.HasMultipleShippingAddresses())
            {
                InterpriseShoppingCart originalCart = cart;
                cart = cart.ForAddress(preferredAddress);
                originalCart.Dispose(); // dispose the original cart object
            }
        }
        else
        {
            preferredAddress = thisCustomer.PrimaryShippingAddress;
        }

        string shippingMethodfromSC = String.Empty;
        var myCookie = HttpContext.Current.Request.Cookies["selectedSM"];
        if (myCookie != null)
        {
            shippingMethodfromSC = myCookie.Value;
            cart.SetCartShippingMethod(shippingMethodfromSC);
        }

        return cart.GetShippingMethods(preferredAddress, giftRegistryItemID);
    }

    /// <summary>
    /// This computation is for OnepageCheckout AJAX CALL
    /// </summary>
    [WebMethod, ScriptMethod]
    public ShippingCalculationSummary GetShippingCalculation(string shippingMethodCode, int freightCalculation, string rateID)
    {
        var thisCustomer = Customer.Current;
        var summary = new ShippingCalculationSummary();
        string customerCode = thisCustomer.CustomerCode;

        var cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                 .New(CartTypeEnum.ShoppingCart, true);

        cart.BuildSalesOrderDetails();

        if (!cart.CouponIncludesFreeShipping())
        {
            if (freightCalculation == 1 || freightCalculation == 2)
            {
                cart.SetCartShippingMethod(shippingMethodCode, string.Empty, new Guid(rateID));
            }
            else
            {
                cart.SetCartShippingMethod(shippingMethodCode);
            }
        }

        //Recreate the cart to update the Resultset based from the selected Freight/Tax
        //Totals are converted to string to automatically format the amounts
        cart.BuildSalesOrderDetails();
        summary.SubTotal = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate, customerCode);
        summary.Freight = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightRate, customerCode);
        summary.Tax = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].TaxRate, customerCode);
        summary.Discount = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].CouponDiscountRate, customerCode);
        summary.DueTotal = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].TotalRate, customerCode);
        summary.Balance = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].BalanceRate, customerCode);
        return summary;
    }

    [WebMethod, ScriptMethod]
    public string GetShippingMethodRates(ShippingMethodDTO shippingMethodInfo, string addressNameValuPairOverride, string addressId)
    {
        Security.AuthenticateService();
        var thisCustomer = Customer.Current;

        var cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                 .New(CartTypeEnum.ShoppingCart, true);
        cart.BuildSalesOrderDetails();

        bool overrideDefaultAddress = !addressNameValuPairOverride.IsNullOrEmptyTrimmed();

        var giftRegistryItem = cart.CartItems
                                .Where(itm => itm.GiftRegistryID.HasValue)
                                .FirstOrDefault(r => r.m_ShippingAddressID == addressId);

        Guid? giftRegistryItemID = (giftRegistryItem != null && giftRegistryItem.GiftRegistryID.HasValue) ? giftRegistryItem.GiftRegistryID : null;

        var preferredAddress = new Address();
        if (overrideDefaultAddress)
        {
            var addressNameValuePair = HttpUtility.ParseQueryString(addressNameValuPairOverride);
            preferredAddress = Address.FromForm(thisCustomer, AddressTypes.Shipping, addressNameValuePair);
        }
        else if (!CommonLogic.IsStringNullOrEmpty(addressId))
        {
            if (giftRegistryItemID.HasValue)
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId, giftRegistryItemID);
            }
            else
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId);
            }

            if (cart.HasMultipleShippingAddresses())
            {
                var originalCart = cart;
                cart = cart.ForAddress(preferredAddress);
                originalCart.Dispose(); // dispose the original cart object
            }
        }
        else
        {
            preferredAddress = thisCustomer.PrimaryShippingAddress;
        }

        string rate = String.Empty;
        cart.CalculateShippingMethodRatesOnDemand(shippingMethodInfo, preferredAddress, giftRegistryItemID);

        if (shippingMethodInfo.IsError)
        {
            rate = AppLogic.GetString("checkoutshipping.aspx.15");
        }
        else
        {
            rate = shippingMethodInfo.FreightDisplay;
        }

        return rate;
    }

    private const string ACTION = "action";
    private const string ACTION_GET_STATES = "getStates";
    private const string ACTION_ADD_NEW = "new";

    /// <summary>
    /// Gets the collection of states for a given country
    /// </summary>
    /// <param name="forCountry"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public List<StateDTO> GetStates(string forCountry)
    {
        forCountry = HttpUtility.UrlDecode(forCountry);

        List<StateDTO> states = new List<StateDTO>();

        CountryAddressDTO requestedCountry = CountryAddressDTO.Find(forCountry);
        if (null != requestedCountry)
        {
            states = requestedCountry.GetStates();
        }

        return states;
    }

    /// <summary>
    /// Gets the pricing level html for an item based on the current customer
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public string GetPricingLevel(string itemCode)
    {
        itemCode = HttpUtility.UrlDecode(itemCode);

        Customer thisCustomer = Customer.Current;
        string response = String.Empty;

        //Do not execute code if pricing level is not applicable
        if (thisCustomer.PricingLevel == String.Empty)
        {
            return response;
        }
        bool hasPricingLevel;

        response = InterpriseHelper.GetInventoryPricingLevelTable(Customer.Current, itemCode, out hasPricingLevel);

        if (!hasPricingLevel)
        {
            return string.Empty;
        }

        return response;
    }

    /// <summary>
    /// Gets the order history
    /// </summary>
    /// <param name="pages"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public SalesOrderHistoryCollection GetOrderHistory(int pages, int current)
    {        
        return ServiceFactory.GetInstance<IOrderService>().GetCustomerSalesOrders(pages, current);   
    }

    [WebMethod]
    public OpenInvoicesCollection GetCustomerOpenInvoices(int pages, int current)
    {
        Security.AuthenticateService();

        var thisCustomer = Customer.Current;
        return ServiceFactory.GetInstance<ICustomerRepository>().GetCustomerOpenInvoices(pages, current, InterpriseHelper.ConfigInstance.WebSiteCode, thisCustomer.CustomerCode);
    }

    [WebMethod, ScriptMethod]
    public AddressDTO AddNewAddress(string addresNameValuePair)
    {
        NameValueCollection addressNameValuePair = HttpUtility.ParseQueryString(addresNameValuePair);
        if (null != addressNameValuePair)
        {
            Address newAddress = Address.FromForm(Customer.Current, AddressTypes.Shipping, addressNameValuePair);
            string shipToCode = InterpriseHelper.AddCustomerShipTo(newAddress);

            if (!CommonLogic.IsStringNullOrEmpty(shipToCode))
            {
                return newAddress.ForTransfer();
            }
        }

        return null;
    }

    [WebMethod, ScriptMethod]
    public decimal GetItemPrice(string itemCode, string itemType, string unitMeasureCode, string composition)
    {
        return AppLogic.GetKitItemPrice(itemCode, itemType, unitMeasureCode, composition);
    }

    [WebMethod, ScriptMethod]
    public string[][] GetProductCompareImageLinks(int[] productIDs, bool includejavascript, string xmlpackagename)
    {
        string[] links = InterpriseHelper.GetProductCompareImageLinks(productIDs);
        string[] package = new string[] { InterpriseHelper.GetProductCompareXmlPackage(includejavascript, xmlpackagename) };
        string[][] returnvalue = new string[][] { links, package };
        return returnvalue;
    }

    #region Minicart

    [WebMethod, ScriptMethod]
    public void RemoveMiniCartItem(string cartRecordID)
    {
        Security.AuthenticateService();

        var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);
        cart.RemoveItem(Convert.ToInt32(cartRecordID));
    }

    [WebMethod, ScriptMethod]
    public void AddToCart(string counter)
    {
        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        string shippingAddressID = (thisCustomer.IsNotRegistered)? String.Empty: thisCustomer.PrimaryShippingAddressID;
        var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);

        var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);
        cart.AddItem(thisCustomer, shippingAddressID, itemCode, Convert.ToInt32(counter), 1, umInfo.Code, CartTypeEnum.ShoppingCart);
    }

    [WebMethod, ScriptMethod]
    public string ShoppingCartNumber()
    {
        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
        string tmpS = AppLogic.GetString("AppConfig.CartPrompt");
        tmpS += "&nbsp;(";
        tmpS += Localization.ParseLocaleDecimal(ShoppingCart.NumItems(thisCustomer.CustomerID, CartTypeEnum.ShoppingCart, thisCustomer.ContactCode), thisCustomer.LocaleSetting);
        tmpS += ")";

        return tmpS;
    }

    [WebMethod, ScriptMethod]
    public string GetShoppingCartNumberOfItems()
    {
        var customer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
        decimal numItems = ShoppingCart.NumItems(customer.CustomerID, CartTypeEnum.ShoppingCart, customer.ContactCode);
        return Localization.ParseLocaleDecimal(numItems, customer.LocaleSetting);
    }

    [WebMethod, ScriptMethod]
    public void UpdateCartItemQuantity(string cartRecordID, string Quantity)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .New(CartTypeEnum.ShoppingCart, true)
                      .SetItemQuantity(Convert.ToInt32(cartRecordID), Convert.ToDecimal(Quantity));
    }

    [WebMethod, ScriptMethod]
    public string GetAccessoryItemForMinicart(string counter)
    {
        string result = string.Empty;
        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        result = InterpriseHelper.GetAccessoryProductsForMiniCart(itemCode);
        return result;
    }

    [WebMethod, ScriptMethod]
    public string RedirectToPayPalCheckoutMinicart()
    {
        var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);
        cart.BuildSalesOrderDetails(false, false);

        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
        string result = String.Empty;
        if (!thisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout") && !thisCustomer.IsUpdatedAnonCustRecord))
        {
            result = "checkoutanon.aspx?checkout=true&checkouttype=pp";
        }
        else if (!thisCustomer.IsRegistered && !AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout"))
        {
            result = string.Format("shoppingcart.aspx?errormsg={0}", AppLogic.GetString("shoppingcart.aspx.61"));
        }
        else
        {
            thisCustomer.ThisCustomerSession["paypalfrom"] = "shoppingcart";
            result = PayPalExpress.CheckoutURL(cart);
        }
        return result;
    }

    [WebMethod, ScriptMethod]
    public string BuildMiniCart()
    {
        return ServiceFactory.GetInstance<IShoppingCartService>().BuildMiniCart();
    }

    [WebMethod, ScriptMethod]
    public void UpdateCart(List<string> qtyArray, List<string> chkArray)
    {
        Security.AuthenticateService();

        if (qtyArray != null)
        {
            int index;
            string cartRecordID;
            string Quantity;

            foreach (string i in qtyArray)
            {
                index = i.IndexOf(":");
                cartRecordID = i.Substring(0, index);
                Quantity = i.Substring(index + 1);
                UpdateCartItemQuantity(cartRecordID, Quantity);
            }
        }

        if (chkArray != null)
        {
            foreach (string counter in chkArray)
            {
                AddToCart(counter);
            }
        }
    }

    #endregion

    [WebMethod(EnableSession = true)]
    public string CreateLeadTaskController(List<string> list, string task)
    {
        string status = string.Empty;

        switch (task)
        {

            case AppLogic.VALIDATECAPTCHA:

                string cSecurityCode = HttpContext.Current.Session["SecurityCode"].ToString();
                string submittedCode = list[0];

                if (submittedCode != cSecurityCode)
                {

                    status = AppLogic.CAPTCHA_MISMATCH;
                }
                else
                {
                    status = AppLogic.CAPTCHA_MATCH;
                }

                break;

            case AppLogic.RENDER_STATES:

                string country = list[0];
                status = AppLogic.RenderStatesOptionsHTML(country);

                break;
            default:

                status = AppLogic.UNDEFINED_TASK;

                break;
        }

        return status;
    }

    [WebMethod]
    public string GetCaseHistory(string activityStatus, string period, string searchString)
    {

        var cases = new List<CustomerActivityCase>();
        cases = CustomerActivityCase.GetCustomerActivityCase(activityStatus, period, searchString);

        string jsonValue = JSONHelper.Serialize<List<CustomerActivityCase>>(cases);

        return jsonValue;
    }

    [WebMethod, ScriptMethod]
    //This will provide the ajax autocomplete for the postal code after filtering the country
    public List<SystemPostalCode> GetSystemPostalCode(string countryname, string postalcode)
    {
        return AppLogic.GetSystemPostalCode(countryname, postalcode);
    }

    [WebMethod]
    public bool SaveNotificationService(int notificationType, string itemCode, string itemType)
    {
        string productURL = String.Empty;
        if (itemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_MATRIX_GROUP)
        {
            var matrixInfo = ServiceFactory.GetInstance<IProductService>()
                                           .GetMatrixItemInfo(itemCode);

            productURL = CurrentContext.FullyQualifiedMobileApplicationPath() + "/" + InterpriseHelper.MakeItemLink(matrixInfo.ItemCode);
            productURL = CommonLogic.QueryStringSetParam(productURL, DomainConstants.QUERY_STRING_KEY_MATRIX_ID, matrixInfo.Counter.ToString());
        }
        else
        {
            productURL = CurrentContext.FullyQualifiedMobileApplicationPath() + InterpriseHelper.MakeItemLink(itemCode);
        }

        var ruleloaddataset = new string[][] { new string[] {"ECOMMERCENOTIFICATION", "READECOMMERCENOTIFICATION", "@ContactCode", Customer.Current.ContactCode,
                                                      "@WebsiteCode", InterpriseHelper.ConfigInstance.WebSiteCode, "@ItemCode", itemCode, "@EmailAddress", Customer.Current.EMail}};

        var ruleDatasetContainer = new EcommerceNotificationDatasetGateway();
        if (!Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.LoadDataSet(InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, ruleloaddataset, ruleDatasetContainer)) return false;

        EcommerceNotificationDatasetGateway.EcommerceNotificationRow ruleDatasetContainernewRow = null;

        if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
        {
            ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification.NewEcommerceNotificationRow();
        }
        else
        {
            ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification[0];
        }

        bool onPriceDrop = ServiceFactory.GetInstance<ICustomerService>().IsCustomerSubscribeToProductNotification(itemCode, 1);
        bool onItemAvail = ServiceFactory.GetInstance<ICustomerService>().IsCustomerSubscribeToProductNotification(itemCode, 0);

        if (notificationType == 1)
        {
            onPriceDrop = true;
        }
        else
        {
            onItemAvail = true;
        }


        ruleDatasetContainernewRow.BeginEdit();
        ruleDatasetContainernewRow.WebSiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;
        ruleDatasetContainernewRow.ItemCode = itemCode;
        ruleDatasetContainernewRow.ContactCode = Customer.Current.ContactCode;
        ruleDatasetContainernewRow.EmailAddress = Customer.Current.EMail;
        ruleDatasetContainernewRow.NotifyOnPriceDrop = onPriceDrop;
        ruleDatasetContainernewRow.NotifyOnItemAvail = onItemAvail;
        ruleDatasetContainernewRow.ProductURL = productURL;

        byte[] salt = InterpriseHelper.GenerateSalt();
        byte[] iv = InterpriseHelper.GenerateVector();
        string contactCodeCypher = InterpriseHelper.Encryption(Customer.Current.ContactCode, salt, iv);
        string emailAddressCypher = InterpriseHelper.Encryption(Customer.Current.EMail, salt, iv);

        ruleDatasetContainernewRow.EncryptedContactCode = string.Format("{0}|{1}|{2}", contactCodeCypher, Convert.ToBase64String(salt), Convert.ToBase64String(iv));
        ruleDatasetContainernewRow.EncryptedEmailAddress = string.Format("{0}|{1}|{2}", emailAddressCypher, Convert.ToBase64String(salt), Convert.ToBase64String(iv));
        ruleDatasetContainernewRow.EndEdit();

        if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
        {
            ruleDatasetContainer.EcommerceNotification.AddEcommerceNotificationRow(ruleDatasetContainernewRow);
        }

        var rulecommandset = new string[][] { new string[] { ruleDatasetContainer.EcommerceNotification.TableName, "CREATEECOMMERCENOTIFICATION",
                                                                    "UPDATEECOMMERCENOTIFICATION", "DELETEECOMMERCENOTIFICATION"} };

        return Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.UpdateDataset(InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, rulecommandset, ruleDatasetContainer);
    }

    #region ShippingCalculator

    [WebMethod, ScriptMethod]
    public string GetShippingMethodCalc(string country, string state, string postalCode, string addressType)
    {
        var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);
        cart.BuildSalesOrderDetails();

        var destinationAddress = new Address()
        {
            Country = country,
            PostalCode = postalCode,
            State = state,
            ResidenceType = InterpriseHelper.ResolveResidenceType(addressType)
        };

        string str = string.Empty;
        string formattedFreight = string.Empty;
        decimal freight = 0;

        var availableShippingMethods = cart.GetShippingMethodsForShippingCalc(destinationAddress, String.Empty);
        for (int ctr = 0; ctr < availableShippingMethods.Count; ctr++)
        {
            var shippingMethod = availableShippingMethods[ctr];
            freight = shippingMethod.Freight;
            formattedFreight = "<span class=freightText>" + " " + freight.ToCustomerCurrency() + "</span>";
            str += "<input type=radio name=shippingmethod value='" + shippingMethod.Code + "'>" + "</input> <span>" + shippingMethod.Description + formattedFreight + "</span>" + "<br/>";
        }

        return str;
    }

    [WebMethod]
    public string GetRegisteredCustomerShippingAddress()
    {
        bool CustomerIsRegistered = Customer.Current.IsRegistered;
        string returnValue;

        if (CustomerIsRegistered)
        {
            var dtoShippingAddress = new AddressDTO();
            var custShippingAddress = Customer.Current.ShippingAddresses;

            dtoShippingAddress.country = custShippingAddress[0].Country;
            dtoShippingAddress.state = custShippingAddress[0].State;
            dtoShippingAddress.postalCode = custShippingAddress[0].PostalCode;
            dtoShippingAddress.city = custShippingAddress[0].City;
            dtoShippingAddress.residenceType = custShippingAddress[0].ResidenceType;
            returnValue = JSONHelper.Serialize<AddressDTO>(dtoShippingAddress);
        }
        else
        {
            returnValue = string.Empty;
        }

        return returnValue;
    }

    #endregion

    [WebMethod, ScriptMethod]
    public string PopulateStates(List<string> list)
    {
        string status = string.Empty;
        string country = list[0];
        status = AppLogic.RenderStatesOptionsHTML(country);
        return status;
    }

    [WebMethod]
    public string GetGlobalConfig()
    {
        var listConfiguration = new List<GlobalConfig>();

        string key = string.Empty;
        string value;

        // ------- Add Here the global configuration ------ //

        key = "MiniCart.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "WebSupport.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        bool isInMobile = CurrentContext.IsRequestingFromMobileMode(Customer.Current);
        key = "IsMobile"; value = isInMobile.ToStringLower();
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GiftRegistry.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "Service.Token"; value = Security.GetMD5Hash(Customer.Current.CustomerCode);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GoogleAnalytics.TrackingCode";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value));

        key = "GoogleAnalytics.PageTracking";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GoogleAnalytics.ConversionTracking";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ShowSocialMediaSubscribeBox";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ItemPopup.Enabled";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "IsAdminCurrentlyLoggedIn";
        value = Security.IsAdminCurrentlyLoggedIn().ToString();
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultSkinID"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "Checkout.UseOnePageCheckout";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "UseShippingAddressVerification";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ShippingRatesOnDemand"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "AllowCustomPostal.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        // ------- End Global Configuration --------------- //

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(listConfiguration);
        return jsonValue;
    }

    [WebMethod]
    public string LoadStringResources(List<string> keys)
    {
        if (keys == null || keys.Count() == 0) return String.Empty;

        var thisCustomer = Customer.Current;

        var resources = new List<StringResourceDTO>();
        keys.ForEach(key =>
        {
            string value = AppLogic.GetString(key, true);
            resources.Add(new StringResourceDTO(key, value));
        });

        return resources.ToJSON();
    }

    [WebMethod]
    public string LoadAppConfigs(List<string> keys)
    {
        var appConfigs = new List<GlobalConfig>();
        string value = String.Empty;

        foreach (string key in keys)
        {
            value = AppLogic.AppConfig(key);
            appConfigs.Add(new GlobalConfig(key, value));
        }
        
        string jsonValue = ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson<List<GlobalConfig>>(appConfigs);
        return jsonValue;
    }

    [WebMethod]
    public string GenerateRequestCode()
    {
        return ServiceFactory.GetInstance<ICustomerService>()
                             .GenerateRequestCodeForActiveShopper();
    }

    [WebMethod, ScriptMethod]
    public CreditCardDTO GetCreditCardInfo(string cardCode)
    {
        CreditCardDTO credit = null;

        if (cardCode != string.Empty)
        {
            credit = CreditCardDTO.Find(AppLogic.DecryptCreditCardCode(Customer.Current, cardCode));
            credit.CreditCardCode = cardCode;
        }
        return credit;
    }

    [WebMethod, ScriptMethod]
    public void ClearCreditCardInfo(string cardCode)
    {
        AppLogic.ClearCreditCardInfo(AppLogic.DecryptCreditCardCode(Customer.Current, cardCode));
    }

    private void SendEmailNotification(bool _skipRegistration, string email, string firstName, string accountName)
    {
        if (AppLogic.AppConfigBool("SendWelcomeEmail") && (!_skipRegistration))
        {

            AppLogic.SendMail(
                AppLogic.GetString("createaccount.aspx.27"),
                AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, Customer.Current, Customer.Current.SkinID, string.Empty, AppLogic.MakeXmlPackageParamsFromString("fullname=" + accountName), false, false),
                true,
                AppLogic.AppConfig("MailMe_FromAddress"),
                AppLogic.AppConfig("MailMe_FromName"),
                email,
                CommonLogic.IIF(Customer.Current.IsRegistered, firstName, accountName),
                "",
                AppLogic.AppConfig("MailMe_Server")
            );
        }
    }

    #region GiftRegistry

    [WebMethod]
    public void MoveItemToRegistry(string sourceRegistryID, string targetRegistryID, string registryItemCode)
    {
        Security.AuthenticateService();

        Guid? sourceRegistryGiuid = sourceRegistryID.TryParseGuid();
        Guid? targetRegistryGiuid = targetRegistryID.TryParseGuid();
        Guid? itemToMoveCode = registryItemCode.TryParseGuid();

        GiftRegistryDA.MoveRegistryItem(targetRegistryGiuid.Value, itemToMoveCode.Value);
        GiftRegistryDA.MoveCompositionKitItems(sourceRegistryGiuid.Value, targetRegistryGiuid.Value, itemToMoveCode.Value);
    }

    [WebMethod]
    public void UpdateRegistryItem(string registryItemCode, string comment, int sortOrder, decimal quantity)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? itemToUpdateCode = registryItemCode.TryParseGuid();
        string htmlEncoded = comment.ToHtmlEncode();

        var registryItem = GiftRegistryDA.GetGiftRegistryItemByRegistryItemCode(itemToUpdateCode.Value);
        if (registryItem == null) return;

        registryItem.Comment = htmlEncoded;
        registryItem.SortOrder = sortOrder;
        registryItem.Quantity = quantity;

        GiftRegistryDA.UpdateRegistryItem(registryItem);
    }

    [WebMethod]
    public void DeleteRegistryItem(string registryItemCode, string giftRegistryId)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? itemDeleteCode = registryItemCode.TryParseGuid();
        Guid? giftRegistryIdGuid = giftRegistryId.TryParseGuid();

        GiftRegistryDA.DeleteRegistryItem(itemDeleteCode.Value);
        GiftRegistryDA.ClearKitItemsFromComposition(giftRegistryIdGuid.Value, itemDeleteCode.Value);
    }

    [WebMethod]
    public string FindRegistriesReturnJSON(string firstName, string lastName, string eventTitle, int currentRow)
    {
        var header = GiftRegistryDA.FindRegistries(firstName, lastName, eventTitle, currentRow, InterpriseHelper.ConfigInstance.WebSiteCode);
        var lstfinditems = header.RawItems.Select(item => new GiftRegistryFindItem()
        {
            Title = item.Title,
            PictureFileName = item.PictureFileName,
            StartDate = item.StartDate.Value.ToShortDateString(),
            EndDate = item.EndDate.Value.ToShortDateString(),
            Counter = item.Counter,
            RegistryID = item.RegistryID,
            ContactGUID = item.ContactGUID,
            URLForViewing = item.URLForViewing,
            RowNumber = item.RowNumber,
            OwnersFullName = item.OwnersFullName
        }).ToArray();

        Thread.Sleep(300);

        var dto = new GiftRegistryFindHeaderDTO();
        dto.Items = lstfinditems;
        dto.TotalRecord = header.TotalRecord;
        dto.DefaultRecordPerSet = (int)DomainConstants.DEFAULT_REGISTRY_PAGESIZE;

        double totalSet = header.TotalRecord / DomainConstants.DEFAULT_REGISTRY_PAGESIZE;
        dto.TotalSet = (totalSet <= 1) ? 1 : (int)Math.Ceiling(totalSet);

        var lastItem = lstfinditems.LastOrDefault();
        dto.CurrentRecord = (lastItem != null) ? lastItem.RowNumber : 0;

        return JSONHelper.Serialize<GiftRegistryFindHeaderDTO>(dto);
    }

    [WebMethod]
    public void DeleteGiftRegistry(string giftRegistryID)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? sourceRegistryGiuid = giftRegistryID.TryParseGuid();
        if (sourceRegistryGiuid.HasValue) GiftRegistryDA.DeleteGiftRegistry(sourceRegistryGiuid.Value);
    }

    #endregion

    #region One Page Checkout

    private const string NO_ACTIVE_POSTAL = "no-active-postal";
    private const string INVALID_POSTAL = "invalid-postal";
    private const string INVALID_STATE = "invalid-state";
    private const string IS_VALID = "valid";
    private const string IS_OVER13_REQUIRED = "required-over-13";
    private const string ADDRESS_IS_SAVED = "saved";
    private const string ZERO_POSTAL = "0";

    #region One Page Checkout Customer Info Loader

    [WebMethod]
    public string GetCustomerInfo(string infoType, bool nopaymentOptions)
    {
        string resources = String.Empty;

        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
        var thisAddress = Address.New(thisCustomer, AddressTypes.Shipping);

        thisCustomer.RequireCustomerRecord();

        var listResources = new List<GlobalConfig>();

        string key = String.Empty;
        string value;

        var aShipping = thisAddress.ThisCustomer.PrimaryShippingAddress;

        var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);

        if (!aShipping.Name.IsNullOrEmptyTrimmed())
        {
            switch (infoType)
            {
                case "shipping-contact":

                    key = "im-registered";
                    value = thisCustomer.IsRegistered.ToString().ToLower();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "final-button-text";
                    value = CommonLogic.IIF(AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"), "Continue", "Place Order");
                    listResources.Add(new GlobalConfig(key, value));

                    key = "contact-name";
                    value = aShipping.Name;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "email";
                    value = CommonLogic.IIF(aShipping.EMail.IsNullOrEmptyTrimmed(), thisCustomer.EMail, aShipping.EMail);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "phone";
                    value = aShipping.Phone.Trim();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "country";
                    value = aShipping.Country;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "postal";
                    value = CommonLogic.IIF(!aShipping.Plus4.IsNullOrEmptyTrimmed(), String.Format("{0}-{1}", aShipping.PostalCode, aShipping.Plus4), aShipping.PostalCode);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "city";
                    value = aShipping.City;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "state";
                    value = aShipping.State;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "address";
                    value = aShipping.Address1;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "residence-type";
                    value = aShipping.ResidenceType.ToString();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "force-save-credit-info";
                    value = AppLogic.AppConfigBool("ForceCreditCardInfoSaving").ToString().ToLower();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "county";
                    value = aShipping.County;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "has-shippable-components";
                    value = cart.HasShippableComponents().ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "has-coupon-free-shipping";
                    value = cart.CouponIncludesFreeShipping(thisCustomer.CouponCode).ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "is-skip-shipping";
                    value = AppLogic.AppConfigBool("SkipShippingOnCheckout").ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "is-home-country";
                    value = aShipping.IsHomeCountry.ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();


                    break;
                case "shipping-method":

                    decimal subTotal = Decimal.Zero;
                    decimal tax = Decimal.Zero;
                    decimal freight = Decimal.Zero;
                    decimal freightTax = Decimal.Zero;
                    decimal total = Decimal.Zero;
                    decimal couponDiscount = Decimal.Zero;
                    decimal computedTax = Decimal.Zero;

                    if (cart.CartItems.Count > 0)
                    {
                        cart.BuildSalesOrderDetails(true);
                        if (!cart.IsSalesOrderDetailBuilt)
                        {
                            if (cart.HasCoupon())
                            {
                                foreach (CartItem cartItem in cart.CartItems)
                                {
                                    if (!cartItem.DiscountAmountAlreadyComputed)
                                    {
                                        cartItem.CouponDiscount = cart.GetCartItemCouponDiscount(cartItem);
                                        cartItem.DiscountAmountAlreadyComputed = true;
                                    }
                                    couponDiscount += cartItem.CouponDiscount;

                                    if (cartItem.CouponDiscount > Decimal.Zero && cartItem.TaxRate > Decimal.Zero)
                                    {
                                        // Recompute tax based on discounted price fro GetCartSubTotal()
                                        decimal extPrice = cartItem.Price;
                                        decimal vat = cartItem.TaxRate;
                                        extPrice = (((extPrice / cartItem.m_Quantity) - (cartItem.CouponDiscount / cartItem.m_Quantity)) * cartItem.m_Quantity).ToCustomerRoundedCurrency();
                                        computedTax = computedTax + (vat - (extPrice * (vat / Interprise.Facade.Base.SimpleFacade.Instance.RoundCurrency(cartItem.Price))).ToCustomerRoundedCurrency());
                                    }
                                }
                            }

                            subTotal = cart.GetCartSubTotal();
                            tax = cart.GetCartTaxTotal() - computedTax;
                            freight = cart.GetCartFreightRate();
                            freightTax = cart.GetCartFreightRateTax(cart.ThisCustomer.CurrencyCode, freight, thisCustomer.FreightTaxCode, thisCustomer.PrimaryShippingAddress);
                        }
                        else
                        {
                            subTotal = cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate;
                            tax = cart.SalesOrderDataset.CustomerSalesOrderView[0].TaxRate;
                            freight = cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightRate;
                            freightTax = cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightTaxRate;
                        }
                       
                        if (cart.ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive)
                        {
                            total = subTotal + freight + freightTax;
                        }
                        else 
                        {
                            total = subTotal + tax + freight + freightTax;
                        }
                        
                        // deduct coupon discount
                        total -= couponDiscount;

                    }
                    else
                    {
                        return "cart-is-empty";
                    }

                    if (cart.ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive)
                    {
                        freight += freightTax;
                    }

                    // deduct other payment total (applied credits, loyalty points, gift codes)
                    total -= cart.GetOtherPaymentTotal();
                    if (total < 0)
                    {
                        total = decimal.Zero;
                    }

                    decimal orderBalance = cart.GetOrderBalance();
                    if (AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout") && ((nopaymentOptions && orderBalance > 0) || (nopaymentOptions == false && orderBalance == 0)))
                    {
                        return "reload-page";
                    }

                    string selectedShippingMethod = (cart.IsSalesOrderDetailBuilt) ? cart.SalesOrderDataset.CustomerSalesOrderView[0].ShippingMethod : cart.GetCartShippingMethodSelected();
                    value = selectedShippingMethod;
                    listResources.Add(new GlobalConfig("opc.shipping.method", value));

                    value = (freight == Decimal.Zero) ? AppLogic.GetString("shoppingcart.aspx.13") : freight.ToCustomerCurrency();
                    listResources.Add(new GlobalConfig("opc.freight.rate", value.ToHtmlEncode()));

                    value = freightTax.ToCustomerCurrency();
                    listResources.Add(new GlobalConfig("opc.freight.tax", value.ToHtmlEncode()));

                    value = (tax + freightTax).ToCustomerCurrency();
                    listResources.Add(new GlobalConfig("opc.tax", value.ToHtmlEncode()));
                    
                    // deduct coupon discount
                    value = ((subTotal < Decimal.Zero) ? cart.GetCartSubTotal() - couponDiscount : subTotal - couponDiscount).ToCustomerCurrency();
                    listResources.Add(new GlobalConfig("opc.sub.total", value.ToHtmlEncode()));


                    value = total.ToCustomerCurrency();
                    listResources.Add(new GlobalConfig("opc.grand.total", Server.HtmlEncode(value)));

                    value = (AppLogic.AppConfigBool("ShowTaxBreakDown") && (freightTax + tax) > 0).ToStringLower();
                    listResources.Add(new GlobalConfig("show-tax-breakdown", value.ToHtmlEncode()));

                    //Get selected Shipping Method's Description for display use.
                    string shippingMethodDescription = selectedShippingMethod;
                    using (System.Data.SqlClient.SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (System.Data.IDataReader reader = DB.GetRSFormat(con, "SELECT ShippingMethodDescription FROM SystemShippingMethodGroupDetailView WHERE ShippingMethodCode = {0}", selectedShippingMethod.ToDbQuote()))
                        {
                            if (reader.Read())
                            {
                                shippingMethodDescription = reader.ToRSField("ShippingMethodDescription");
                                if (string.IsNullOrEmpty(shippingMethodDescription))
                                {
                                    shippingMethodDescription = selectedShippingMethod;
                                }
                            }
                        }
                    }
                    value = shippingMethodDescription;
                    listResources.Add(new GlobalConfig("opc.shipping.method.description", shippingMethodDescription.ToHtmlEncode()));

                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();

                    break;
                case "payments-info":

                    var aBilling = thisAddress.ThisCustomer.PrimaryBillingAddress;

                    key = "opc-billing-contact-name";
                    value = aBilling.Name;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-email";
                    value = aBilling.EMail;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-phone";
                    value = aBilling.Phone;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-country";
                    value = aBilling.Country;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-postal";
                    value = CommonLogic.IIF(!aBilling.Plus4.IsNullOrEmptyTrimmed(), String.Format("{0}-{1}", aBilling.PostalCode, aBilling.Plus4), aBilling.PostalCode);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-city";
                    value = aBilling.City;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-state";
                    value = aBilling.State;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-address";
                    value = aBilling.Address1;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-payment-method";
                    value = thisCustomer.PaymentMethod;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-payment-term-code";
                    value = thisCustomer.PaymentTermCode;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-county";
                    value = aBilling.County;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "is-home-country";
                    value = aBilling.IsHomeCountry.ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();

                    break;

                default:
                    break;
            }

        }

        return resources;
    }

    #endregion

    #region One Page Checkout Step 1 (Save Shipping Info)

    [WebMethod]
    public string OnePageCheckoutStep1(List<string> profile, List<string> shippingAddress, bool validate, string addressId)
    {
        Security.AuthenticateService();
        var thisCustomer = Customer.Current;

        try
        {
            string address = shippingAddress[0].Trim();
            string countryCode = shippingAddress[1].Trim();
            string postalCode = shippingAddress[2].Trim();
            string city = shippingAddress[3].Trim();
            string stateCode = shippingAddress[4].Trim();
            

            string name = profile[0].Trim();
            string email = profile[1].Trim();
            string phone = profile[2].Trim();

            if (validate)
            {
                if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(profile[1]) && thisCustomer.IsNotRegistered)
                {
                    return INVALID_EMAIL;
                }

                if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && thisCustomer.IsNotRegistered && Customer.EmailInUse(profile[1], Customer.Current.CustomerCode))
                {
                    return EMAIL_DUPLICATES;
                }

                if (InterpriseHelper.IsSearchablePostal(countryCode) && InterpriseHelper.IsCountryHasActivePostal(countryCode))
                {
                    var splitPostal = postalCode.Split('-');
                    if (splitPostal.Length > 0) postalCode = splitPostal[0];

                    if (!InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty))
                    {
                        return INVALID_POSTAL;
                    }

                    if (InterpriseHelper.IsWithState(countryCode) && !InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode))
                    {
                        return INVALID_STATE;
                    }
                }

                return IS_VALID;
            }
            else
            {
                var aShippingAddress = Address.New(thisCustomer, AddressTypes.Shipping);

                aShippingAddress.AddressID = addressId.IsNullOrEmptyTrimmed() ? thisCustomer.PrimaryShippingAddressID : addressId;
                aShippingAddress.CustomerCode = thisCustomer.CustomerCode;
                aShippingAddress.Address1 = address;
                aShippingAddress.Country = countryCode;
                aShippingAddress.PostalCode = postalCode;
                aShippingAddress.City = city;
                aShippingAddress.State = InterpriseHelper.IsWithState(countryCode) ? stateCode : String.Empty;
                aShippingAddress.ResidenceType = InterpriseHelper.ResolveResidenceType(shippingAddress[5]);

                if (AppLogic.AppConfigBool("Address.ShowCounty"))
                {
                    aShippingAddress.County = shippingAddress[6].Trim();
                }

                aShippingAddress.EMail = (email.IsNullOrEmptyTrimmed() && thisCustomer.IsRegistered) ? thisCustomer.EMail : email;
                aShippingAddress.Name = name;
                aShippingAddress.Phone = phone;
                aShippingAddress.Save();

                ServiceFactory.GetInstance<ICustomerService>()
                              .UpdateCustomerShipTo(aShippingAddress, true);

                var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(CartTypeEnum.ShoppingCart, true);
                cart.ShipAllItemsToThisAddress(aShippingAddress);

                if (thisCustomer.IsRegistered)
                {
                    ServiceFactory.GetInstance<ICustomerService>()
                                  .MakeDefaultAddress(aShippingAddress.AddressID, AddressTypes.Shipping, true);
                }

                AppLogic.SavePostalCode(aShippingAddress);
                return ADDRESS_IS_SAVED;

            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    #endregion

    #region One Page Checkout Step 2 (Save Payments Method)

    [WebMethod]
    public string OnePageCheckoutStep2(string shippingMethod, string freight, string freightCalculation, string realTimeRateGUID)
    {
        Security.AuthenticateService();

        var shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        var thisCustomer = Customer.Current;

        try
        {
            thisCustomer.RequireCustomerRecord();

            shoppingCartService.ClearCartWarehouseCodeByCustomer();

            var cart = shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
            if (shippingMethod.IsNullOrEmptyTrimmed())
            {
                cart.SetCartShippingMethod(String.Empty);
                return String.Empty;
            }

            if (freightCalculation == "1" || freightCalculation == "2")
            {
                cart.SetCartShippingMethod(shippingMethod, String.Empty, new Guid(realTimeRateGUID));
                ServiceFactory.GetInstance<IShippingService>()
                              .SetRealTimeRateRecord(shippingMethod, freight.Trim(new char[] {'$', ' '}), realTimeRateGUID, false);
            }
            else
            {
                cart.SetCartShippingMethod(shippingMethod);
            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return IS_VALID;
    }

    #endregion

    #region One Page Checkout Step 3 (Validate Billing Info)

    [WebMethod]
    public string IsBillingInfoCorrect(string countryCode, string postalCode, string stateCode, bool isWithRequiredAge)
    {
        try
        {
            #region Validate: Over 13 Requirement

            if (AppLogic.AppConfigBool("RequireOver13Checked") && Customer.Current.IsNotRegistered && !isWithRequiredAge)
            {
                return IS_OVER13_REQUIRED;
            }

            #endregion

            #region Validate: Billing Address

            if (InterpriseHelper.IsSearchablePostal(countryCode) && InterpriseHelper.IsCountryHasActivePostal(countryCode))
            {
                var splitPostal = postalCode.Split('-');
                if (splitPostal.Length > 0) postalCode = splitPostal[0];

                if (!InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty))
                {
                    return INVALID_POSTAL;
                }

                if (InterpriseHelper.IsWithState(countryCode) && !InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode))
                {
                    return INVALID_STATE;
                }
            }

            #endregion

            return IS_VALID;
        }
        catch (Exception ex) { return ex.Message; }
    }

    #endregion

    #endregion

    #region Customer

    #region Update Profile

    [WebMethod(EnableSession = true)]
    public string UpdateAccountInfo(string account, string captcha, string newPassword, string oldPassword)
    {   
        try{

            Security.AuthenticateService();

            #region Variable Declaration
            var thisCustomer = Customer.Current;
            var accountInfo = ServiceFactory.GetInstance<ICryptographyService>().DeserializeJson<LiteAccountInfo>(account);
            #endregion

            #region Validate Captcha
            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount") && !Session["SecurityCode"].IsNullOrEmptyTrimmed())
            {
                string sessionSecurityCode = Session["SecurityCode"].ToString();
                bool isCaptchaValid = AppLogic.AppConfigBool("Captcha.CaseSensitive") ? captcha.Equals(sessionSecurityCode) : captcha.Equals(sessionSecurityCode, StringComparison.InvariantCultureIgnoreCase);

                if (!isCaptchaValid)
                {
                    return ServiceFactory.GetInstance<IStringResourceService>().GetString("account.aspx.69");
                }
            }
            #endregion

            #region Validate Email

            if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(accountInfo.Email))
            {
                return ServiceFactory.GetInstance<IStringResourceService>().GetString("account.aspx.90");
            }
            
            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && thisCustomer.EMail != accountInfo.Email)
            {
                if (Customer.EmailInUse(accountInfo.Email, thisCustomer.CustomerCode))
                {
                    return ServiceFactory.GetInstance<IStringResourceService>().GetString("account.aspx.70");
                }
            }
            #endregion

            #region Change Password
            bool isUpdatePassword = (!newPassword.IsNullOrEmptyTrimmed() && !oldPassword.IsNullOrEmptyTrimmed());

            if (isUpdatePassword)
            {
                var customerWithValidLogin = ServiceFactory.GetInstance<IAuthenticationService>().FindByEmailAndPassword(thisCustomer.EMail, oldPassword);
                if (customerWithValidLogin.IsNullOrEmptyTrimmed())
                {
                    return ServiceFactory.GetInstance<IStringResourceService>().GetString("account.aspx.71");
                }

            }
            #endregion

            #region Update 
            thisCustomer.Salutation = (accountInfo.Salutation == ServiceFactory.GetInstance<IStringResourceService>().GetString("account.aspx.61")) ? String.Empty : accountInfo.Salutation;
            thisCustomer.FirstName = accountInfo.FirstName;
            thisCustomer.LastName = accountInfo.LastName;
            thisCustomer.Phone = accountInfo.ContactNumber;
            thisCustomer.IsOver13 = accountInfo.IsOver13Checked;
            thisCustomer.IsOKToEMail = accountInfo.IsOkToEmail;
            thisCustomer.Mobile = accountInfo.Mobile;

            thisCustomer.EMail = accountInfo.Email;
            thisCustomer.Password = isUpdatePassword ? newPassword : AppLogic.PasswordValuePlaceHolder;

            thisCustomer.Update();
            #endregion

            return String.Empty;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }

    #endregion

    [WebMethod(EnableSession = true)]
    public List<string> GetCustomerShipTo(string ShipToCode)
    {
        Security.AuthenticateService();
        return AppLogic.GetCustomerShipTo(ShipToCode);
    }

    #endregion

    #region Matrix Group Items

    [WebMethod]
    public string GetMatrixGroupItems(string itemCode, int pageSize, int pageNumber, string imageSize)
    {
        Security.AuthenticateService();

        var items = MatrixGroupItems.GetMatrixItems(itemCode, pageSize, pageNumber, imageSize);
        var itemCodes = items.Select(i => i.ItemCode)
                             .ToList();
       
        if (itemCodes.Count > 0)
        {
            var appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            var productService = ServiceFactory.GetInstance<IProductService>();
            var customerService = ServiceFactory.GetInstance<ICustomerService>();

            if (appConfigService.ShowProductLoyaltyPoints)
            {
                var itemSettings = productService.GetWebOptions(itemCodes);
                decimal purchaseMultiplier = customerService.GetPurchaseMultiplier();

                items = items.Select(i =>
                {
                    i.IsDontEarnPoints = itemSettings.First(s => s.ItemCode == i.ItemCode).IsDontEarnPoints;
                    i.PurchaseMultiplier = purchaseMultiplier;
                    return i;
                }).ToList();
            }
        }

        return ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson(items, Encoding.UTF8);
    }

    #endregion

    #region Item Popup
    [WebMethod, ScriptMethod]
    public void AddToCartEx(string counter, decimal quantity, string kitcomposition, string unitmeasure)
    {
        AddToCartViaService(counter, quantity, kitcomposition, unitmeasure, String.Empty, 0);
    }

    [WebMethod]
    public string AddToCartWithWarehouseCode(string counter, decimal quantity, IList kitcomposition, string unitmeasure, string wareHouseCode, decimal actualStock)
    {
        string kitCompositionRaw = String.Empty;
        if (kitcomposition != null && kitcomposition.Count > 0)
        {
            kitCompositionRaw = String.Join(",", kitcomposition.Cast<string>());
        }

        return AddToCartViaService(counter, quantity, kitCompositionRaw, unitmeasure, wareHouseCode, actualStock);
    }

    /// <param name="wareHouseCode">WARNING!!: This will only be used to support Store-Pickup in product pages. 
    /// Supplying this paramer will automatically add the item to cart with shipping method StorePickUp 
    /// and convert the cart in multiple shipping methods</param>
    private string AddToCartViaService(string counter, decimal quantity, string kitcomposition, string unitmeasure, string wareHouseCode, decimal actualWarehouseStock)
    {
        string status = String.Empty;

        var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>()
                                         .GetCurrentLoggedInCustomer();

        var cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                 .New(CartTypeEnum.ShoppingCart, true);

        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        string shippingAddressID = (thisCustomer.IsNotRegistered)? String.Empty: thisCustomer.PrimaryShippingAddressID;

        var umInfo = (unitmeasure.IsNullOrEmptyTrimmed()) ? InterpriseHelper.GetItemDefaultUnitMeasure(itemCode) : InterpriseHelper.GetItemUnitMeasure(itemCode, unitmeasure);
        var kitItemsComposition = KitComposition.FromComposition(kitcomposition, thisCustomer, CartTypeEnum.ShoppingCart, itemCode);

        if (!wareHouseCode.IsNullOrEmptyTrimmed() && actualWarehouseStock > 0)
        {
            var pickupItem = cart.CartItems
                     .FirstOrDefault(c => c.ItemCode == itemCode);

            if (pickupItem != null && !pickupItem.InStoreWarehouseCode.IsNullOrEmptyTrimmed() && pickupItem.InStoreWarehouseCode != wareHouseCode)
            {
                cart.RemoveItem(pickupItem.m_ShoppingCartRecordID);

                //reset the cart
                cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                     .New(CartTypeEnum.ShoppingCart, true);
            }

            if (AppLogic.AppConfigBool("Inventory.LimitCartToQuantityOnHand"))
            {
                pickupItem = cart.CartItems.FirstOrDefault(c => c.ItemCode == itemCode);
                //disallow to add item to cart if already exceeded to the stock limit
                decimal needToAdd = actualWarehouseStock - ((pickupItem != null) ? pickupItem.m_Quantity : Decimal.Zero);

                if (quantity > needToAdd)
                {
                    status = "limit";
                    quantity = needToAdd;
                }

                if (quantity == 0)
                {
                    status = "limit";
                    return status;
                }
            }

        }

        cart.AddItem(thisCustomer, shippingAddressID, itemCode, Convert.ToInt32(counter), quantity, umInfo.Code, CartTypeEnum.ShoppingCart, kitItemsComposition, wareHouseCode);
        return status;
    }

    [WebMethod, ScriptMethod]
    public string GetItemPopup(string itemCode)
    {
        return AppLogic.GetItemPopup(itemCode);
    }

    [WebMethod, ScriptMethod]
    public string GetMatrixItemDetails(int itemCounter, string itemCode, string matrixCombination)
    {
        var matrixItems = MatrixItemData.GetMatrixItems(itemCounter, itemCode, false);
        var selectedAttributes = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<AttributeData>>(matrixCombination);
        var itemcode = string.Empty;
        foreach(var matrixItem in matrixItems)
        {
            int attribFound = 0;
            bool exists = false;
            for(int i = 0; i < selectedAttributes.Count; i++)
            {
                exists = matrixItem.Attributes.Exists(attrib => attrib.Code == selectedAttributes[i].Code && attrib.Value == selectedAttributes[i].Value);
                if (exists) { attribFound++; }
            }
            if (attribFound == selectedAttributes.Count) {itemcode = matrixItem.ItemCode; break; }
        }

        if (itemcode != string.Empty)
        {
            var settings = ItemWebOption.GetWebOption(itemcode);
            var itemInfo = new MatrixItemInfo();
            itemInfo.HidePriceUntilCart = settings.HidePriceUntilCart;
            itemInfo.IsCallToOrder = settings.IsCallToOrder;
            itemInfo.ItemCounter = settings.ItemCounter;
            itemInfo.ItemCode = itemcode;
            itemInfo.MinimumOrderQuantity = settings.MinimumOrderQuantity;
            itemInfo.RequiresRegistration = settings.RequiresRegistration;
            itemInfo.RestrictedQuantities = settings.RestrictedQuantities;
            itemInfo.ShowBuyButton = settings.ShowBuyButton;
            itemInfo.UnitMeasures = ProductPricePerUnitMeasure.GetAll(itemcode, Customer.Current, settings.HidePriceUntilCart, true);
            return JSONHelper.Serialize<MatrixItemInfo>(itemInfo);
        }
        return "";
    }

    [WebMethod, ScriptMethod]
    public string GetItemReviews(string itemCode, int sort)
    {
        return AppLogic.GetItemReviews(itemCode, sort);
    }

    [WebMethod]
    public string GetItemImage(string itemCode)
    {
        return AppLogic.GetProductImage(itemCode);
    }

    [WebMethod]
    public void CreateUpdateItemReview(string itemCode, int rating, string comment)
    {
        comment = comment.ToHtmlEncode();
        if (AppLogic.HasItemReview(itemCode)) { AppLogic.UpdateItemReview(itemCode, rating, comment); }
        else { AppLogic.CreateItemReview(itemCode, rating, comment); }
    }

    [WebMethod]
    public void VoteItemReview(string itemCode, string voterCustomerCode, string voterContactCode, string vote, string customerCode, string contactCode)
    {
        AppLogic.VoteItemReview(itemCode, voterCustomerCode, voterContactCode, vote, customerCode, contactCode);
    }

    [WebMethod]
    public bool NotifyOnPriceDrop(string itemcode)
    {
        return AppLogic.ProductNotification(itemcode, 1);
    }

    [WebMethod]
    public bool NotifyOnAvailability(string itemcode)
    {
        return AppLogic.ProductNotification(itemcode, 0);
    }
    #endregion

    #region StoreLocator

    [WebMethod]
    public string GetWarehouseByAddress(int storeTypeCode, string longtitude, string latitude, string distance)
    {
        var selStoreType = (StoreType)storeTypeCode;
        var systeWarehouses = StoreLocatorDA.GetDealersAndWarehouses(selStoreType);

        double inputtedDisctance = double.Parse(distance);
        double radius = 6371;
        double radiance = 3.1459 / 180;
        double inMiles = 0.621371192;

        string json = systeWarehouses.Where(w =>
        {
            double lat1 = double.Parse(latitude);
            double lat2 = (double)w.Coordinate.Latitude;

            double lon1 = double.Parse(longtitude);
            double lon2 = (double)w.Coordinate.Longtitude;

            var dLat = (lat2 - lat1) * radiance;
            var dLon = (lon2 - lon1) * radiance;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * radiance) * 
                       Math.Cos(lat2 * radiance) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = radius * c * inMiles;

            w.Distance = d;

            return (d <= inputtedDisctance);
        })
        .OrderBy(sw => sw.Distance)
        .ToList()
        .ToJSON();

        return json;
    }

    #endregion

    #region CMS Editor

    [WebMethod]
    public string UpdateStringResourceConfigValue(string contentKey, string contentValue)
    {
        try
        {
            Security.AuthenticateService();

            if (Security.IsAdminCurrentlyLoggedIn())
            {
                AppLogic.UpdateStringResourceConfigValue(contentKey, contentValue);

            }
            else
            {
                return AppLogic.GetString("signin.aspx.20", true);
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return String.Empty;
    }

    [WebMethod]
    public bool IsPageEditMode()
    {
        return Security.IsAdminCurrentlyLoggedIn() && Customer.Current.IsInEditingMode();
    }

    [WebMethod]
    public void TogglePageEditMode(bool mode)
    {
        Customer.Current.ThisCustomerSession[DomainConstants.CMS_ENABLE_EDITMODE] = mode.ToString();

        //reset top menu to reset caching
        ApplicationCachingEngine.Reset(DomainConstants.TOP_MENU_CACHE_NAME + "_" + InterpriseHelper.ConfigInstance.WebSiteCode);
    }

    [WebMethod]
    public bool UpdateTopicFromEditor(string topicId, string htmlContent)
    {
        try
        {
            Security.AuthenticateService();

            if (!Security.IsAdminCurrentlyLoggedIn() || !Customer.Current.IsInEditingMode()) return false;

            try
            {
                return ResourcesDA.UpdateTopic(topicId, htmlContent, InterpriseHelper.ConfigInstance.WebSiteCode, Customer.Current.LanguageCode);
            }
            catch (Exception)
            {
                throw;
            }

        }
        catch
        {
            throw;
        }
    }

    [WebMethod]
    public bool UpdateItemDescriptionFromEditor(string contentKey, string contentValue, string contentType)
    {
        try
        {
            Security.AuthenticateService();

            if (!Security.IsAdminCurrentlyLoggedIn() || !Customer.Current.IsInEditingMode()) return false;

            try
            {
                return ResourcesDA.UpdateItemDescription(contentKey, contentValue, contentType, InterpriseHelper.ConfigInstance.WebSiteCode, Customer.Current.LanguageCode);
            }
            catch (Exception)
            {
                throw;
            }

        }
        catch
        {
            throw;
        }

    }

    [WebMethod]
    public string GetProductImageData(int counter, string itemCode, string itemType, int matrixGroupCounter)
    {
        string jsonValue = String.Empty;

        if (itemType == "product")
        {
            var imgData = ProductImageData.GetForImageUpload(counter, itemCode, itemType, matrixGroupCounter);
            jsonValue = imgData.Serialize(true);
            imgData = null;
        }
        else
        {
            bool exist = false;
            string imgMobileUrl = String.Empty;

            foreach (var ext in DomainConstants.GetImageSupportedExtensions())
            {
                imgMobileUrl = AppLogic.GetMobileImagePath(itemType, "mobile", true) + counter + "." + ext;
                if (!System.IO.File.Exists(imgMobileUrl)) continue;

                imgMobileUrl = AppLogic.GetMobileImagePath(itemType, "mobile", false) + counter + "." + ext;
                exist = true;
                break;
            }

            if (!exist)
            {
                imgMobileUrl = "mobile/images/nopictureicon.gif";
            }

            string imagUrlLarge = AppLogic.LocateImageUrl(itemType, counter, "large");
            string imagUrlMedium = AppLogic.LocateImageUrl(itemType, counter, "medium");
            string imagUrlIcon = AppLogic.LocateImageUrl(itemType, counter, "icon");

            var entityImageHeader = new EntityImageHeader()
            {
                ID = counter,
                Thumbnail = new EntityImageDetail()
                {
                    src = imagUrlIcon,
                    exists = !(imagUrlIcon.Contains("skins/") || imagUrlIcon.Contains("nopicture")),
                    ImgFileName = imagUrlIcon.Substring(imagUrlIcon.LastIndexOf("/") + 1)
                },
                Medium = new EntityImageDetail()
                {
                    src = imagUrlMedium,
                    exists = !(imagUrlMedium.Contains("skins/") || imagUrlMedium.Contains("nopicture")),
                    ImgFileName = imagUrlMedium.Substring(imagUrlMedium.LastIndexOf("/") + 1)
                },
                Large = new EntityImageDetail()
                {
                    src = imagUrlLarge,
                    exists = !(imagUrlLarge.Contains("skins/") || imagUrlLarge.Contains("nopicture")),
                    ImgFileName = imagUrlLarge.Substring(imagUrlLarge.LastIndexOf("/") + 1)
                },
                Mobile = new EntityImageDetail()
                {
                    src = imgMobileUrl,
                    exists = exist,
                    ImgFileName = imgMobileUrl.Substring(imgMobileUrl.LastIndexOf("/") + 1)
                }
            };

            jsonValue = entityImageHeader.ToJSON();
            entityImageHeader = null;
        }

        return jsonValue;
    }

    [WebMethod]
    public bool ImageUploadSetAsImageDefault(string itemCode, string fileName, string size)
    {
        Security.AuthenticateService();

        return ProductDA.UpdateDefaultImageSize(itemCode, fileName, size, InterpriseHelper.ConfigInstance.WebSiteCode);
    }

    [WebMethod]
    public string ImageGetDuplicateImageFilename(string filename)
    {
        Security.AuthenticateService();

        return ProductDA.GetDuplicateImageFilename(filename, InterpriseHelper.ConfigInstance.WebSiteCode);
    }

    #endregion

    #region Dashboard

    [WebMethod]
    public string GetNewCustomers(int displayLimit)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }
        var customers = CustomerDA.GetCustomers()
                                  .OrderByDescending(c => c.DateRegistered)
                                  .Take(displayLimit)
                                  .Select(x => new CustomerInfoJSON
                                  {
                                      CustomerCode = x.CustomerCode,
                                      FullName = x.FullName,
                                      FirstName = x.FirstName,
                                      LastName = x.LastName,
                                      DateRegistered = x.DateRegistered.ToShortDateString()
                                  })
                                  .ToList();
        return customers.ToJSON();
    }

    [WebMethod]
    public string GetProductsLowInFreeStock(int threshold, int displayLimit, bool isActiveOnly)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }
        string filter = String.Empty;
        if (isActiveOnly)
        {
            filter = " (Status = 'A' OR (Status = 'P' AND FreeStock > 0)) AND  FreeStock <= {0} ".FormatWith(threshold.ToString());
        }
        else
        {
            filter = " FreeStock <= {0} ".FormatWith(threshold.ToString());
        }

        return ProductDA.GetProductsStockTotal(Customer.Current.LanguageCode, filter)
                        .OrderBy(item => item.StockTotal.FreeStock)
                        .Take(displayLimit)
                        .ToList()
                        .ToJSON();
    }

    [WebMethod]
    public string GetStoreSettings(List<string> keys)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        var configs = new List<GlobalConfig>();

        foreach (string key in keys)
        {
            string value = AppLogic.AppConfig(key);
            configs.Add(new GlobalConfig(key, value.ToLowerInvariant()));
        }

        return configs.ToList().ToJSON();
    }

    [WebMethod]
    public string GetWebRecentOrders(int displayLimit)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        return CustomerDA.GetWebSalesOrders()
                         .OrderByDescending(s => s.SalesOrderDate)
                         .Take(displayLimit)
                         .ToList()
                         .ToJSON();
    }

    [WebMethod]
    public string GetWebStats(DateRangeType rangeType)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        return InterpriseSuiteEcommerceCommon.Integration.Interprise.Admin.Dashboard.GetWebStats(rangeType)
                                                                                    .ToList()
                                                                                    .ToJSON();
    }

    [WebMethod]
    public string GetWebSales(DateRangeType rangeType, string dateFrom, string dateTo)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        var sales = CustomerDA.GetWebInvoice().Where(r => r.InvoiceDate.Between(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo)));

        if (rangeType == DateRangeType.Date)
        {
            return sales.GroupBy(date => date.InvoiceDate)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.ToString("yyyyMMdd") })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Week)
        {
            return sales.GroupBy(week => week.InvoiceDate.Date.DayOfYear / 7)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = (invoice.Key + 1).ToString() })
                        .OrderBy(d => Convert.ToInt32(d.Dimension))
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Month)
        {
            return sales.GroupBy(month => new { month.InvoiceDate.Year, month.InvoiceDate.Month })
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.Month.ToString() })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Year)
        {
            return sales.GroupBy(year => year.InvoiceDate.Year)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.ToString() })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }
        return String.Empty;
    }

    private const string DATA_FEED_URL = "https://www.google.com/analytics/feeds/data";
    private const string ACCOUNT_FEED_URL = "https://www.googleapis.com/analytics/v2.4/management/accounts";

    [WebMethod]
    public string GetWebVisitors(string dimension, string dateFrom, string dateTo)
    {
        try
        {
            string webPropertyUrl = String.Empty;
            string profileFeedUrl = String.Empty;
            string profileUrl = String.Empty;

            string gaUsername = AppLogic.AppConfig("GoogleAnalytics.Username");
            string gaPassword = AppLogic.AppConfig("GoogleAnalytics.Password");
            string gaAPIKey = AppLogic.AppConfig("GoogleAnalytics.APIKey");
            string gaTrackingCode = AppLogic.AppConfig("GoogleAnalytics.TrackingCode");

            if (gaUsername.IsNullOrEmptyTrimmed() ||
                gaPassword.IsNullOrEmptyTrimmed() ||
                gaAPIKey.IsNullOrEmptyTrimmed())
            {
                return String.Empty;
            }

            var service = new Google.GData.Analytics.AnalyticsService("ConnectedBusiness");
            service.setUserCredentials(gaUsername, gaPassword);

            var accountsFeed = service.Query(new Google.GData.Analytics.DataQuery("{0}?key={1}".FormatWith(ACCOUNT_FEED_URL, gaAPIKey)));
            webPropertyUrl = accountsFeed.Entries.First().Links[1].HRef.Content;

            var webPropertiesFeed = service.Query(new Google.GData.Analytics.DataQuery(webPropertyUrl));
            if (!gaTrackingCode.IsNullOrEmptyTrimmed())
            { 
                var webProperty = webPropertiesFeed.Entries
                                                   .OfType<Google.GData.Analytics.DataEntry>()
                                                   .FirstOrDefault(x => x.Links[2].HRef.Content.Contains(gaTrackingCode));
                if (webProperty != null) { profileFeedUrl = webProperty.Links[2].HRef.Content; }
            }

            if (profileFeedUrl.IsNullOrEmptyTrimmed())
            {
                profileFeedUrl = webPropertiesFeed.Entries.First().Links[2].HRef.Content;
            }

            var profileFeed = service.Query(new Google.GData.Analytics.DataQuery(profileFeedUrl));
            profileUrl = profileFeed.Entries.First().Links[0].HRef.Content;

            var profiles = profileUrl.Split('/');
            string profileID = profiles[profiles.Length - 1];
            var query = new Google.GData.Analytics.DataQuery()
            {
                Query = "{0}?key={1}".FormatWith(DATA_FEED_URL, gaAPIKey),
                Ids = "ga:" + profileID,
                Metrics = "ga:visits",
                Dimensions = "ga:" + dimension.ToLower(),
                GAStartDate = dateFrom,
                GAEndDate = dateTo
            };

            var visitsFeed = service.Query(query);
            var result = new List<GoogleAnalytics>();
            result.AddRange(visitsFeed.Entries.OfType<Google.GData.Analytics.DataEntry>()
                                                .Select(entry => new GoogleAnalytics()
                                                {
                                                    Dimension = entry.Title.Text.Split('=')[1],
                                                    Visits = Convert.ToInt32(entry.Metrics[0].Value)
                                                }));
            return result.ToList().ToJSON();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    #region UPS/FedEx Address Verification

    [WebMethod]
    public string RequestAddressBestMatch(string billing, string shipping, bool isResidence)
    {
        Security.AuthenticateService();

        var response = new List<LiteAddressInfoMatch>();
        var cryptographyService = ServiceFactory.GetInstance<ICryptographyService>();

        if (!AppLogic.AppConfigBool("UseShippingAddressVerification"))
        {
            return String.Empty;
        }

        if (billing.IsNullOrEmptyTrimmed() || shipping.IsNullOrEmptyTrimmed())
        {
            return String.Empty;
        }

        try
        {
            List<LiteAddressInfo> billingMatch = null;
            List<LiteAddressInfo> shippingMatch = null;

            var billingAddress = cryptographyService.DeserializeJson<LiteAddressInfo>(billing);
     
            if (!billingAddress.Country.IsNullOrEmptyTrimmed() && !InterpriseHelper.IsSearchablePostal(billingAddress.Country))
            {
                return String.Empty;
            }

            int addressMatchResultLimit = (AppLogic.AppConfigNativeInt("AddressMatchResultLimit") == 0) ? 1 : AppLogic.AppConfigNativeInt("AddressMatchResultLimit");

            if (!billingAddress.Address.IsNullOrEmptyTrimmed())
            {
                billingMatch = AppLogic.GetAddressMatch(billingAddress.Address, 
                                                         billingAddress.Country, 
                                                         billingAddress.PostalCode,
                                                         billingAddress.City,
                                                         billingAddress.State,                             
                                                         isResidence, addressMatchResultLimit);
            }

            var shippingAddress = cryptographyService.DeserializeJson<LiteAddressInfo>(shipping);

            if (!shippingAddress.Address.IsNullOrEmptyTrimmed()) { 

                if (!shippingAddress.Country.IsNullOrEmptyTrimmed() && !InterpriseHelper.IsSearchablePostal(shippingAddress.Country))
                {
                    return String.Empty;
                }

                shippingMatch = AppLogic.GetAddressMatch(shippingAddress.Address,
                                                        shippingAddress.Country,
                                                        shippingAddress.PostalCode,
                                                        shippingAddress.City,
                                                        shippingAddress.State, isResidence, addressMatchResultLimit);
            }
          

            response.Add(new LiteAddressInfoMatch
            {
                Billing = billingMatch.IsNullOrEmptyTrimmed() ? cryptographyService.SerializeToJson(String.Empty) : cryptographyService.SerializeToJson(billingMatch),
                Shipping = shippingMatch.IsNullOrEmptyTrimmed() ? cryptographyService.SerializeToJson(String.Empty) : cryptographyService.SerializeToJson(shippingMatch),
                AddressMatchResultLimit = addressMatchResultLimit
            });

            return cryptographyService.SerializeToJson(response);

        }
        catch (Exception ex)
        {
            return "exception[error]{0}".FormatWith(ex.Message);
        }

    }

    #endregion

    #region Required String Resources and App Config for Address Verification

    [WebMethod]
    public string GetStringResources(List<string> keys)
    {
        var stringResources = new List<GlobalConfig>();

        string value;

        foreach (string key in keys)
        {
            value = AppLogic.GetString(key);
            stringResources.Add(new GlobalConfig(key, value));
        }

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(stringResources);
        return jsonValue;
    }

    #endregion

    #region Address

    [WebMethod]
    public string GetAddressList(string countryCode, string postalCode, string stateCode, string searchString, bool exactMatch, int pageNumber)
    {
        if (searchString.IsNullOrEmptyTrimmed())
        {
            searchString = postalCode;
        }

        return AppLogic.RenderPostalCodeListing(exactMatch, postalCode, stateCode, countryCode, pageNumber, searchString);
    }

    [WebMethod]
    public string GetCity(string countryCode, string postalCode, string stateCode)
    {
        if (countryCode.IsNullOrEmptyTrimmed())
        {
            return String.Empty;
        }

        if (!InterpriseHelper.IsCountryHasActivePostal(countryCode))
        {
            return NO_ACTIVE_POSTAL;
        }

        if (!InterpriseHelper.IsWithState(countryCode))
        {
            stateCode = String.Empty;
        }

        return InterpriseHelper.GetCity(countryCode, postalCode, stateCode);
    }

    [WebMethod]
    public bool IsStateCodeValid(string countryCode, string postalCode, string stateCode)
    {
        if (AppLogic.AppConfigBool("AllowCustomPostal.Enabled") || !InterpriseHelper.IsSearchablePostal(countryCode))
        {
            return true;
        }

        return InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode);
    }

    [WebMethod]
    public bool IsPostalCodeValid(string countryCode, string postalCode)
    {
        if (!InterpriseHelper.IsCountryHasActivePostal(countryCode)) return true;
        return InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty);
    }


    [WebMethod]
    public int ValidatePostalCode(string country, string postal, string stateCode, string shipToCountry, string shipToPostal, string shipToStateCode)
    {
        if (AppLogic.AppConfigBool("AllowCustomPostal.Enabled"))
        {
            return 0;
        }

        int status = 0;

        string bPostal = InterpriseHelper.ParsePostalCode(country, postal).PostalCode;
        string sPostal = (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo")) ? InterpriseHelper.ParsePostalCode(shipToCountry, shipToPostal).PostalCode : String.Empty;

        if (InterpriseHelper.IsSearchablePostal(country) && InterpriseHelper.IsCountryHasActivePostal(country))
        {
            status = InterpriseHelper.IsCorrectAddress(country, bPostal, stateCode) ? 0 : 1;
        }

        if (status == 1 && !sPostal.IsNullOrEmptyTrimmed() && bPostal == sPostal && stateCode == shipToStateCode)
        {
            return 3;
        }

        if (sPostal.IsNullOrEmptyTrimmed())
        {
            return status;
        }

        if (InterpriseHelper.IsSearchablePostal(shipToCountry) && InterpriseHelper.IsCountryHasActivePostal(shipToCountry))
        {
            status = InterpriseHelper.IsCorrectAddress(shipToCountry, sPostal, shipToStateCode) ? status : (status + 2);
        }

        return status;
    }

    #endregion


    #region Credit Memos, Loyalty Points, Gift Codes

    [WebMethod]
    public void ApplyCreditMemos(string jsonData)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .ApplyCreditMemos(jsonData);
    }

    [WebMethod]
    public void ApplyLoyaltyPoints(string points)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .ApplyLoyaltyPoints(points);
    }

    [WebMethod]
    public void ApplyGiftCodes(string jsonData)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .ApplyGiftCodes(jsonData);
    }

    [WebMethod]
    public bool VerifyGiftCode(string code)
    {
        return ServiceFactory.GetInstance<IShoppingCartService>()
                             .IsValidGiftCode(code);
    }

    [WebMethod]
    public bool CheckIfGiftCodeIsOwned(string code)
    {
        return ServiceFactory.GetInstance<IShoppingCartService>()
                             .IsGiftCodeOwnedByCustomer(code);
    }

    [WebMethod]
    public void AddGiftCode(string code)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .AddAdditionalGiftCode(code);
    }

    [WebMethod]
    public void RemoveGiftCode(string code)
    {
        ServiceFactory.GetInstance<IShoppingCartService>()
                      .RemoveAdditionalGiftCode(code);
    }

    #endregion

    [WebMethod]
    public decimal GetInventoryFreeStock(string itemCode, string unitMeasureCode)
    {
        return InterpriseHelper.GetInventoryFreeStock(itemCode, unitMeasureCode, Customer.Current);
    }

    [WebMethod]
    public string Version()
    {
        return InterpriseHelper.GetWebVersionInformation();
    }

    [WebMethod]
    public string SyncImages(int totalImages, int currentImageRow, string syncType)
    {
        var syncTypeEnum = syncType.TryParseEnum<ImageSyncType>();

        //Access the ImagePerBatch to adjust the batch transferring
        //ImageSynchronizer.ImagePerBatch = 30;
        return ImageSynchronizer.SynchronizeImages(new CustomFileUploadJson() { TotalImages = totalImages, CurrentImageRow = currentImageRow }, syncTypeEnum);
    }

    [WebMethod]
    public string GetItemImages(string itemCode)
    {
        var images = ServiceFactory.GetInstance<IProductService>().GetItemImages(itemCode).OrderBy(i => i.ImageIndex).ToList();
        return ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson(images);
    }

    #region InStorePickup

    [WebMethod]
    public string GetStorePickUpInitialnfoToJSON(string itemCode, string unitMeassureCode)
    {
        return ServiceFactory.GetInstance<IProductService>()
                             .GetStorPickUpInitialInfoToJson(itemCode, unitMeassureCode);
    }

    [WebMethod]
    public string GetStorePickupInventoryWarehouseListToJSON(bool isFirstLoad, string itemCode, IList<string> kitComposition, 
                                                             string unitMeasureCode, string postalCode, string city, string state, string country, int nextRecord)
    {
        byte dataLimit = DomainConstants.INSTORE_SEARCH_LIMIT;
        return ServiceFactory.GetInstance<IProductService>()
                             .GetStorePickupInventoryWarehouseListToJSON(isFirstLoad, itemCode, kitComposition, unitMeasureCode, postalCode, city, state, country, nextRecord, dataLimit);
    }

    [WebMethod]
    public string GetStorePickupWarehouseStoreHoursToJSON(string warehouseCode)
    {
        return ServiceFactory.GetInstance<IWarehouseService>()
                             .GetStorePickupWarehouseStoreHoursToJSON(warehouseCode);
    }

    [WebMethod]
    public string GetStorePickupWarehouseInfo(string warehouseCode)
    {
        return ServiceFactory.GetInstance<IWarehouseService>()
                             .GetWarehouseByCodeToJSON(warehouseCode);
    }

    [WebMethod]
    public string GetOverSizedItemShippingMethodToJson(string itemCode, string unitMeasure)
    {
        return ServiceFactory.GetInstance<IShippingService>()
                             .GetOverSizedItemShippingMethodToJson(itemCode, unitMeasure);
    }

    #endregion

    [WebMethod]
    public string AddItemToCart(int itemCounter, string itemCode, decimal quantity, string unitMeasureCode, int cartTypeIndex)
    {
        try
        {
            Security.AuthenticateService();

            CartTypeEnum[] cartTypes = new CartTypeEnum[] {CartTypeEnum.ShoppingCart,
                                                           CartTypeEnum.WishCart, 
                                                           CartTypeEnum.RecurringCart,
                                                           CartTypeEnum.GiftRegistryCart, 
                                                           CartTypeEnum.ShoppingCart};
           
            var cartType = (cartTypeIndex < 0 || cartTypeIndex > cartTypes.Length) ?  CartTypeEnum.ShoppingCart : cartTypes[cartTypeIndex];

            var thisCustomer = ServiceFactory.GetInstance<IAuthenticationService>().GetCurrentLoggedInCustomer();
            var settings = ItemWebOption.GetWebOption(itemCode);


            if (!settings.ShowBuyButton && cartType == CartTypeEnum.ShoppingCart)
            {
                return AppLogic.GetString("showproduct.aspx.84");
            }

            if (settings.IsCallToOrder && cartType == CartTypeEnum.ShoppingCart)
            {
                return AppLogic.GetString("common.cs.20");
            }

            if (thisCustomer.IsNotRegistered && ((cartType == CartTypeEnum.WishCart) || (settings.RequiresRegistration && cartType == CartTypeEnum.ShoppingCart)))
            {
                string msg = (cartType == CartTypeEnum.ShoppingCart) ? "showproduct.aspx.85" : "showproduct.aspx.86";
                return AppLogic.GetString(msg);
            }

            if (AppLogic.AppConfigBool("Inventory.LimitCartToQuantityOnHand"))
            {
                decimal freeStock = InterpriseHelper.GetInventoryFreeStock(itemCode, unitMeasureCode, thisCustomer);
                string strFreeStock = Convert.ToInt32(freeStock).ToString();

                if (freeStock <= 0)
                {
                    return "{0} {1}".FormatWith(AppLogic.GetString("showproduct.aspx.30"), strFreeStock);
                }

                if (freeStock < quantity)
                {
                    return "{0} {1}".FormatWith(AppLogic.GetString("showproduct.aspx.68"), strFreeStock);
                }

            }

            var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);
            string code = (unitMeasureCode.IsNullOrEmptyTrimmed() || ServiceFactory.GetInstance<IAppConfigService>().HideUnitMeasure) ? umInfo.Code : unitMeasureCode;

            var cart = ServiceFactory.GetInstance<IShoppingCartService>().New(cartType, true);
            cart.AddItem(thisCustomer, thisCustomer.PrimaryShippingAddressID, itemCode, itemCounter, quantity, code, cartType);
            cart.Dispose();

            cart = null;
            thisCustomer = null;


        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return String.Empty;

    }

    [WebMethod]
    public string ValidateEmailAddress(bool initializeRequest, string emailAddress, string accountType)
    {
        Security.AuthenticateService();
       
        if (initializeRequest)
        {
            return String.Empty;
        }

        try
        {
            bool requiresUniqueEmail = !ServiceFactory.GetInstance<IAppConfigService>().AllowCustomerDuplicateEMailAddresses;

            if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(emailAddress))
            {
                return INVALID_EMAIL;
            }
 
            switch (accountType)
            {
                case Interprise.Framework.Base.Shared.Const.CUSTOMER:

                    if (requiresUniqueEmail && ServiceFactory.GetInstance<ICustomerService>().IsCustomerEmailNotAvailable(emailAddress))
                    {
                        return EMAIL_DUPLICATES;
                    }

                    break;
                case Interprise.Framework.Base.Shared.Const.LEADS:

                    if (requiresUniqueEmail && ServiceFactory.GetInstance<ICustomerService>().IsLeadEmailNotAvailable(emailAddress))
                    {
                        return EMAIL_DUPLICATES;
                    }

                    break;
                default:
                    break;
            }


        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return String.Empty;
    }


    #region ProductRatings

    [WebMethod]
    public string GetProductRatingSummary(string itemCode)
    {
        return ServiceFactory.GetInstance<IProductService>()
                             .GetRatingHeaderJSON(itemCode);
    }

    [WebMethod]
    public string GetProductRatings(string itemCode, int nextRecord, int ratingPageSize, int sortBy)
    {
        return ServiceFactory.GetInstance<IProductService>()
                             .GetProductRatings(itemCode, nextRecord, ratingPageSize, sortBy);
    }
    
    [WebMethod]
    public void RateComment(string itemCode, string customerId, string contactId, string vote)
    {
        int helpfulVal = (vote == "YES").ToBit();
        CustomerDA.RateComment(itemCode, customerId, contactId, helpfulVal);
    }

    [WebMethod]
    public string GetCustomerCurrentRatingJson(string itemCode)
    {
        return ServiceFactory.GetInstance<IProductService>()
                             .GetProductRatingJSON(itemCode);
    }

    [WebMethod]
    public void SaveRating(string itemCode, int rating, string comment)
    {
        ServiceFactory.GetInstance<IProductService>()
                      .SaveRating(itemCode, rating, comment);
    }

    #endregion  

    [WebMethod]
    public bool CheckProductNotificationSubscription(int notificationType, string itemCode)
    {
        var thisCustomer = Customer.Current;
        return ServiceFactory.GetInstance<ICustomerService>().IsCustomerSubscribeToProductNotification(itemCode, notificationType);
    }

    [WebMethod]
    public void VoidRMA(string rmaCode)
    {
        var cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                 .New(CartTypeEnum.ShoppingCart, true);
        cart.VoidRMA(rmaCode);
    }

    
    [WebMethod, ScriptMethod]
    public decimal GetItemSalesPrice(int counter, string itemCode, decimal quantity)
    {
        decimal promotionalPrice = 0;
        return InterpriseHelper.GetSalesPrice(Customer.Current.CustomerCode, counter, Customer.Current.CurrencyCode, quantity, ref promotionalPrice);
    }


    [WebMethod, ScriptMethod]
    public string GetAmountCustomerCurrencyFormat(decimal amount)
    {
        return amount.ToCustomerCurrency();
    }

}