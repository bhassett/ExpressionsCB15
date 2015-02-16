using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    public partial class createaccount : SkinBase
    {

        #region Variable Declaration

        private bool _checkOutMode = false;
        private bool _skipRegistration = false;
        private InterpriseShoppingCart _cart = null;
        private bool isAnonPayPal = false;

        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        INavigationService _navigationService = null;
        IAppConfigService _appConfigService = null;
        IShoppingCartService _shoppingCartService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            PageNoCache();
            RequireSecurePage();
            InitializeDomainServices();
            InitAddressControl();
            InitProfileControl();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            InitPageContent();
            base.OnLoad(e);
        }

        private void InitializeDomainServices()
        {
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        }

        private void InitPageContent()
        {
            SectionTitle = AppLogic.GetString("createaccount.aspx.127", true);

            if (ThisCustomer.IsRegistered)
            {
               _navigationService.NavigateToAccountPage();
            }

            _checkOutMode = CommonLogic.QueryStringBool("checkout");
            _skipRegistration = CommonLogic.QueryStringBool("skipreg");

            isAnonPayPal = CommonLogic.QueryStringBool("isAnonPayPal");

            RequireCustomerRecord();
            InitializeShoppingCart();

            CreateAccountHelpfulTips.SetContext = this;
            PerformPageAccessLogic();

   
            if (_checkOutMode && _skipRegistration)
            {
                pnlPageTitle.Visible = false;
                pnlProductUpdates.Visible = false;
              
                CreateAccountHelpfulTips.TopicName = "AnonymousCheckoutHelpfulTips";
                GetAnonymousCustomerProfile();
            }

        }

        private void InitAddressControl()
        {

            bool isAnonymousCheckout = CommonLogic.QueryStringBool("checkout") && CommonLogic.QueryStringBool("skipreg");

            string cityText = _stringResourceService.GetString("createaccount.aspx.84");
            string streetText = _stringResourceService.GetString("createaccount.aspx.128");
            string stateText = _stringResourceService.GetString("createaccount.aspx.129");
            string postalText = _stringResourceService.GetString("createaccount.aspx.130");
            string enterPostalText = _stringResourceService.GetString("createaccount.aspx.131");
            string countyText = _stringResourceService.GetString("createaccount.aspx.132");
            string taxText = _stringResourceService.GetString("createaccount.aspx.133");

            BillingAddressControl.LabelStreetText = streetText;
            BillingAddressControl.LabelCityText = cityText;
            BillingAddressControl.LabelStateText = stateText;
            BillingAddressControl.LabelPostalText = postalText;
            BillingAddressControl.LabelEnterPostalText = enterPostalText;
            BillingAddressControl.LabelCountyText = countyText;
            BillingAddressControl.LabelTaxText = taxText;
            BillingAddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            BillingAddressControl.IsShowBusinessTypesSelector = AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration") && !isAnonymousCheckout;
            BillingAddressControl.DefaultBusinessTypeText = _stringResourceService.GetString("createaccount.aspx.82");

            if (isAnonymousCheckout)
            {
                BillingAddressControl.SelectedCountry = ThisCustomer.PrimaryBillingAddress.Country;  
            }

            BillingAddressControl.BindData();

            ShippingAddressControl.IsShowResidenceTypesSelector=true;
            ShippingAddressControl.IsShowBusinessTypesSelector=false;
            ShippingAddressControl.IsHideStreetAddressInputBox = false;
            ShippingAddressControl.LabelStreetText = streetText;
            ShippingAddressControl.LabelCityText = cityText;
            ShippingAddressControl.LabelStateText = stateText;
            ShippingAddressControl.LabelPostalText = postalText;
            ShippingAddressControl.LabelEnterPostalText = enterPostalText;
            ShippingAddressControl.LabelCountyText = countyText;
            ShippingAddressControl.LabelTaxText = taxText;
            ShippingAddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            ShippingAddressControl.DefaultAddressType = _stringResourceService.GetString("createaccount.aspx.145");

            if (isAnonymousCheckout)
            {
                ShippingAddressControl.SelectedCountry = ThisCustomer.PrimaryShippingAddress.Country;
                ShippingAddressControl.ResidenceTypeSelected = ThisCustomer.PrimaryShippingAddress.ResidenceType.ToString();
            }

            ShippingAddressControl.BindData();
        }

        private void InitProfileControl()
        {
            ProfileControl.AccountType = Interprise.Framework.Base.Shared.Const.CUSTOMER;

            if (CommonLogic.QueryStringBool("checkout") && CommonLogic.QueryStringBool("skipreg"))
            {
                ProfileControl.IsExcludeAccountName = true;
                ProfileControl.IsAnonymousCheckout = true;
            }

            ProfileControl.DefaultSalutationText = _stringResourceService.GetString("createaccount.aspx.81");
            ProfileControl.LabelFirstNameText = _stringResourceService.GetString("createaccount.aspx.134");
            ProfileControl.LabelLastNameText = _stringResourceService.GetString("createaccount.aspx.135");
            ProfileControl.LabelAccountNameText = _stringResourceService.GetString("createaccount.aspx.137");
            ProfileControl.LabelSecurityAccountText = _stringResourceService.GetString("createaccount.aspx.138");
            ProfileControl.LabelCompanyNameText = _stringResourceService.GetString("createaccount.aspx.139");
            ProfileControl.LabelPasswordText = _stringResourceService.GetString("createaccount.aspx.140");
            ProfileControl.LabelConfirmPasswordText = _stringResourceService.GetString("createaccount.aspx.141");
            ProfileControl.LabelContactNumberText = _stringResourceService.GetString("createaccount.aspx.142");
            ProfileControl.LabelEmailAddressText = _stringResourceService.GetString("createaccount.aspx.154");
            ProfileControl.LabelEmailText = _stringResourceService.GetString("createaccount.aspx.155");
            ProfileControl.LabelMobileNumberText = _stringResourceService.GetString("createaccount.aspx.156");
            ProfileControl.LabelAnonymousEmailText = _stringResourceService.GetString("createaccount.aspx.154");
            
            ProfileControl.BindData();
        }

        private void InitializeShoppingCart()
        {
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
        }

        #region PerformPageAccessLogic

        private void PerformPageAccessLogic()
        {

            if (!_checkOutMode) return;

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------

            if (_cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("shoppingcart.aspx.1", true).ToUrlEncode());
            }

            string couponCode = string.Empty;
            string couponErrorMessage = string.Empty;
            if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderWeight(AppLogic.AppConfigUSDecimal("MinOrderWeight")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            pnlCheckoutImage.Visible = true;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
        }

        #endregion

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsSecurityCodeGood(txtCaptcha.Text))
                {
                    errorSummary.DisplayErrorMessage(AppLogic.GetString("createaccount.aspx.126", true));
                }
                else
                {
                    RegisterCustomer();

                }

            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        private void RegisterCustomer()
        {
            string salutation = ProfileControl.Salutation;
            string email =  (_checkOutMode && _skipRegistration) ? ProfileControl.AnonymousEmail : ProfileControl.Email;

            bool infoIsAllGood = true;

            if (salutation == AppLogic.GetString("createaccount.aspx.81", true)) { salutation = String.Empty; }

            if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(email))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("createaccount.aspx.157"));
                infoIsAllGood = false;
            }

            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && Customer.EmailInUse(email, ThisCustomer.CustomerCode))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("createaccount.aspx.94"));
                infoIsAllGood = false;
            }

            if (infoIsAllGood)
            {

                #region Customer Profile 
                string contactNumber = ProfileControl.ContactNumber;
                ThisCustomer.Salutation = salutation;
                ThisCustomer.FirstName = ProfileControl.FirstName;
                ThisCustomer.LastName = ProfileControl.LastName;
                ThisCustomer.EMail = email;
                ThisCustomer.Password = ProfileControl.Password;
                ThisCustomer.Phone = contactNumber;
                ThisCustomer.Mobile = ProfileControl.Mobile;
                ThisCustomer.IsOKToEMail = chkProductUpdates.Checked;
                ThisCustomer.IsOver13 = chkOver13.Checked;

                #endregion

                #region Customer Business Type

                if (AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration"))
                {
                    if (BillingAddressControl.BusinessType.ToLowerInvariant() == Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_WHOLESALE.ToLower()) ThisCustomer.BusinessType = Customer.BusinessTypes.WholeSale;
                    if (BillingAddressControl.BusinessType.ToLowerInvariant() == Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_RETAIL.ToLower()) ThisCustomer.BusinessType = Customer.BusinessTypes.Retail;
                   
                    ThisCustomer.TaxNumber = BillingAddressControl.TaxNumber;
                }

                #endregion

                #region Customer Billing Address

                var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);

                var parsedBillingCityText = InterpriseHelper.ParseCityText(billingTxtCityStates.Text, BillingAddressControl.State, BillingAddressControl.City);
                aBillingAddress.State = parsedBillingCityText[0];
                aBillingAddress.City = parsedBillingCityText[1];

                aBillingAddress.Address1 = BillingAddressControl.Street;
                aBillingAddress.Country = BillingAddressControl.Country;
                aBillingAddress.PostalCode = BillingAddressControl.Postal;

                if (!BillingAddressControl.County.IsNullOrEmptyTrimmed()) aBillingAddress.County = BillingAddressControl.County;

                aBillingAddress.FirstName = ProfileControl.FirstName;
                aBillingAddress.LastName = ProfileControl.LastName;
                aBillingAddress.EMail = email;
                aBillingAddress.Name = ProfileControl.AccountName;
                aBillingAddress.Company = ProfileControl.AccountName;
                aBillingAddress.ResidenceType = ResidenceTypes.Residential;
                aBillingAddress.Phone = contactNumber;

                #endregion

                #region Customer Shipping Address 

                var aShippingAddress = Address.New(ThisCustomer, AddressTypes.Shipping);

                if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
                {
                    if (copyBillingInfo.Checked)
                    {
                        aShippingAddress.State = parsedBillingCityText[0];
                        aShippingAddress.City = parsedBillingCityText[1];
                        aShippingAddress.Address1 = BillingAddressControl.Street;
                        aShippingAddress.Country = BillingAddressControl.Country;
                        aShippingAddress.PostalCode = BillingAddressControl.Postal;
                        if (!BillingAddressControl.County.IsNullOrEmptyTrimmed()) aShippingAddress.County = BillingAddressControl.County;
                    }
                    else
                    {
                        var parsedShippingCityText = InterpriseHelper.ParseCityText(shippingTxtCityStates.Text, ShippingAddressControl.State, ShippingAddressControl.City);
                        aShippingAddress.State = parsedShippingCityText[0];
                        aShippingAddress.City = parsedShippingCityText[1];
                        aShippingAddress.Address1 = ShippingAddressControl.Street;
                        aShippingAddress.Country = ShippingAddressControl.Country;
                        aShippingAddress.PostalCode = ShippingAddressControl.Postal;
                        if (!ShippingAddressControl.County.IsNullOrEmptyTrimmed()) aShippingAddress.County = ShippingAddressControl.County;
                    }

                    aShippingAddress.FirstName = ProfileControl.FirstName;
                    aShippingAddress.LastName = ProfileControl.LastName;
                    aShippingAddress.EMail = email;
                    aShippingAddress.Name = ProfileControl.AccountName;
                    aShippingAddress.Company = ProfileControl.AccountName;
                    aShippingAddress.ResidenceType = InterpriseHelper.ResolveResidenceType(ShippingAddressControl.ResidenceType);
                    aShippingAddress.Phone = contactNumber;

                }
                else
                {
                    aShippingAddress = aBillingAddress.Clone() as Address;
                }

                #endregion

                #region Register Or Checkout 

                if (_checkOutMode && _skipRegistration)
                {
                    SkipRegistrationAndCheckout(aBillingAddress, aShippingAddress, email);
                }
                else
                {
                    ThisCustomer.Register(aBillingAddress, aShippingAddress, _checkOutMode);
                }

                #endregion

                #region Assign Items Shipping / Mail Notification / Redirection 

                _cart.ShipAllItemsToThisAddress(ThisCustomer.PrimaryShippingAddress);
               
                SendEmailNotification(false, email, ProfileControl.FirstName, ProfileControl.AccountName);
                RedirectToSucceedingPage();

                #endregion

            }

        }

        private void SendEmailNotification(bool _skipRegistration, string email, string firstName, string accountName)
        {
            if (AppLogic.AppConfigBool("SendWelcomeEmail") && (!_skipRegistration))
            {
				AppLogic.SendMail(
                AppLogic.GetString("createaccount.aspx.27", true),
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

        protected bool IsSecurityCodeGood(string code)
        {

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount")) return true;

            if (Session["SecurityCode"] != null)
            {

                string sCode = Session["SecurityCode"].ToString();
                string fCode = code;

                if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                {
                    if (fCode.Equals(sCode)) return true;
                }
                else
                {
                    if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            return true;

        }

        private void RedirectToSucceedingPage()
        {

            if (_checkOutMode)
            {
                if (isAnonPayPal)
                {
                    _cart.BuildSalesOrderDetails(false, false);
                    Customer.Current.ThisCustomerSession["paypalfrom"] = "shoppingcart";
                   _navigationService.NavigateToUrl(PayPalExpress.CheckoutURL(_cart));
                }
                else if ((_cart.HasMultipleShippingAddresses() && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.CartAllowsShippingMethodSelection) || _cart.HasRegistryItems())
                {
                    _navigationService.NavigateToCheckoutMult();
                }
                else if (_appConfigService.CheckoutUseOnePageCheckout)
                {
                    if (ThisCustomer.IsNotRegistered && !_appConfigService.AllowShipToDifferentThanBillTo)
                    {
                        _navigationService.NavigateToCheckoutShipping();
                    }
                    else
                    {
                        _navigationService.NavigateToCheckout1();
                    }
                }
                else
                {
                    _navigationService.NavigateToCheckoutShipping();
                }
            }
            else
            {
                _navigationService.NavigateToAccountPage();
            }
        }

        #region SkipRegistrationAndCheckout

        private void SkipRegistrationAndCheckout(Address billing, Address shipping, string email)
        {
            string name = ProfileControl.AccountName;
            if (name.IsNullOrEmptyTrimmed())
            {
                name = "{0} {1}".FormatWith(ProfileControl.FirstName, ProfileControl.LastName);
            }

            ThisCustomer.EMail = email;

            billing.FirstName = ProfileControl.FirstName;
            billing.LastName = ProfileControl.LastName;
            billing.Name = name;
            billing.EMail = email;

            shipping.EMail = email;
            shipping.FirstName = ProfileControl.FirstName;
            shipping.LastName = ProfileControl.LastName;
            shipping.Name = name;
            shipping.AddressType = AddressTypes.Shipping;

            if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsNotRegistered)
            {
                ThisCustomer.IsOver13 = chkOver13.Checked;
                _customerService.UpdateCustomerRequiredAge();
            }

            ThisCustomer.PrimaryBillingAddress = billing;
            ThisCustomer.PrimaryShippingAddress = shipping;

            billing.Save();
            shipping.Save();

            _cart.ShipAllItemsToThisAddress(shipping);

            //Save Anonymous Customer Email Address in Sales Order Note
            ServiceFactory.GetInstance<ICustomerService>()
                          .AssignAnonymousCustomerEmailAddressInSalesOrderNote();

            _shoppingCartService.DoRecomputeCartItemsPrice();

            SendEmailNotificationAndRedirect();
        }


        private void SendEmailNotificationAndRedirect()
        {
            SendEmailNotification(_skipRegistration, ProfileControl.Email, ProfileControl.FirstName, ProfileControl.LastName);
            RedirectToSucceedingPage();
        }

        #endregion

        #region Get Anonymous Customer Profile

        private void GetAnonymousCustomerProfile()
        {

            if (IsPostBack) return;

            // profile

            string customerName = ThisCustomer.PrimaryBillingAddress.Name.Trim();
            string firstName = ThisCustomer.PrimaryBillingAddress.FirstName.Trim();
            string lastName = ThisCustomer.PrimaryBillingAddress.LastName.Trim();

            if (firstName.IsNullOrEmptyTrimmed() || lastName.IsNullOrEmptyTrimmed())
            {
                return;
            }

            pnlResetForm.Visible = true;

            string contactNumber = ThisCustomer.PrimaryBillingAddress.Phone;

            if (AppLogic.AppConfigBool(""))
            {

            }

            ProfileControl.FirstName = firstName;
            ProfileControl.LastName = lastName;
            ProfileControl.Email = ThisCustomer.PrimaryBillingAddress.EMail;
            ProfileControl.AnonymousEmail = ThisCustomer.PrimaryBillingAddress.EMail;
            ProfileControl.ContactNumber = contactNumber;

            // billing info

            string billingState = ThisCustomer.PrimaryBillingAddress.State;
            string billingCity = ThisCustomer.PrimaryBillingAddress.City;

            BillingAddressControl.Postal = ThisCustomer.PrimaryBillingAddress.PostalCode;
            BillingAddressControl.Street = ThisCustomer.PrimaryBillingAddress.Address1;
            BillingAddressControl.State = billingState;
            BillingAddressControl.City = billingCity;
            billingTxtCityStates.Text = String.Format("{0},{1}", billingState, billingCity);

            // shipping info

            string shippingState = ThisCustomer.PrimaryShippingAddress.State;
            string shippingCity = ThisCustomer.PrimaryShippingAddress.City;

            ShippingAddressControl.Postal = ThisCustomer.PrimaryShippingAddress.PostalCode;
            ShippingAddressControl.Street = ThisCustomer.PrimaryShippingAddress.Address1;
            ShippingAddressControl.State = shippingState;
            ShippingAddressControl.City = shippingCity;
            shippingTxtCityStates.Text = String.Format("{0},{1}", shippingState, shippingCity);
           
        }

        #endregion

    }
}
