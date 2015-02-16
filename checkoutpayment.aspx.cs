using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutpayment : SkinBase
    {

        #region Private Members

        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        private bool isUsingInterpriseGatewayv2 = false;
        private IEnumerable<PaymentTermDTO> _paymentTermOptions = null;
        private bool _isRequirePayment = true;
        private bool _skipCreditCardValidation = false;

        #endregion

        #region DomainServices

        INavigationService _navigationService = null;
        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        IShoppingCartService _shoppingCartService = null;
        IAppConfigService _appConfigService = null;
        ICurrencyService _currencyService = null;
        IAuthenticationService _authenticationService = null;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            Initialize();
            BindControls();

            base.OnInit(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }

            base.OnUnload(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            DoPassingOfValueWhenTokenizedCreditCard();

            base.OnLoad(e);
        }

        #endregion  

        #region Private Methods

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _currencyService = ServiceFactory.GetInstance<ICurrencyService>();
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        }

        private void PerformPageAccessLogic()
        {
            
            _customerService.DoIsNotRegisteredChecking();

            if (_cart.HasGiftItems() && _cart.IsGiftEmailNotSet())
            {
                _navigationService.NavigateToCheckoutGiftEmail();
            }
        }

        private void BindControls()
        {
            btnDoProcessPayment.Click += (senderObject, evt) =>
            {
                try
                {
                    ProcessPayment();
                }
                catch (Exception ex)
                {
                    errorSummary.DisplayErrorMessage(ex.Message);
                }
            };
        }

        private void Initialize()
        {
            InitializeDomainServices();
            RegisterAjaxScript();
            PageNoCache();

            RequireSecurePage();
            RequireCustomerRecord();

            ctrlPaymentTerm.ThisCustomer = ThisCustomer;
            isUsingInterpriseGatewayv2 = AppLogic.IsUsingInterpriseGatewayv2();

            InitializeShoppingCart();
            InitializePaymentTermControl(IsCreditCardTokenizationEnabled);
            InitializeOtherPaymentOptionControl();
            PerformPageAccessLogic();

            if (IsCreditCardTokenizationEnabled)
            {
                litTokenizationFlag.Text = "true";
                CreditCardOptionsRenderer();
            }
            else
            {
                litTokenizationFlag.Text = "false";
                BillingAddressGridRenderer();
            }

            if (_cart.CartItems.Count > 0)
            {
                CheckIfWeShouldRequirePayment();
            }

            DisplayErrorMessageIfAny();

            decimal gCardAllocate = Decimal.Zero;
            decimal gCertAllocate = Decimal.Zero;
            decimal loyaltyPointsAllocate = Decimal.Zero;
            decimal creditMemosAllocate = Decimal.Zero;

            if (_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems())
            {
                var splittedCarts = _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();

               
                foreach (var splitCart in splittedCarts)
                {
                    if (splitCart.IsNoShippingRequired())
                    {
                        splitCart.BuildSalesOrderDetails(false, true, litCouponEntered.Text,true);
                    }
                    else
                    {
                        splitCart.BuildSalesOrderDetails(true,litCouponEntered.Text);
                    }

                    OrderSummary.Text += splitCart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, String.Empty, gCardAllocate, gCertAllocate, loyaltyPointsAllocate, creditMemosAllocate, litCouponEntered.Text));
                    gCardAllocate += splitCart.GiftCardsTotalCreditAllocated;
                    gCertAllocate += splitCart.GiftCertsTotalCreditAllocated;
                    loyaltyPointsAllocate += splitCart.LoyaltyPointsCreditAllocated;
                    creditMemosAllocate += splitCart.CreditMemosCreditAllocated;
                }
            }
            else if (_cart.HasMultipleShippingMethod())
            {
                var ordersWithDifferentShipping = _shoppingCartService.SplitShippingMethodsInMultipleOrders();
                foreach (var order in ordersWithDifferentShipping)
                {
                    if (order.IsNoShippingRequired())
                    {
                        order.BuildSalesOrderDetails(false, true, litCouponEntered.Text,true);
                    }
                    else
                    {
                        order.BuildSalesOrderDetails(true, litCouponEntered.Text);
                    }
                    OrderSummary.Text += order.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, String.Empty, gCardAllocate, gCertAllocate, loyaltyPointsAllocate, creditMemosAllocate, litCouponEntered.Text));
                    gCardAllocate += order.GiftCardsTotalCreditAllocated;
                    gCertAllocate += order.GiftCertsTotalCreditAllocated;
                    loyaltyPointsAllocate += order.LoyaltyPointsCreditAllocated;
                    creditMemosAllocate += order.CreditMemosCreditAllocated;
                }
            }   
            else
            {
                if (_cart.IsNoShippingRequired())
                {
                    _cart.BuildSalesOrderDetails(false, true, litCouponEntered.Text,true);
                }
                else
                {
                    _cart.BuildSalesOrderDetails(true, litCouponEntered.Text);
                }

                string couponCode = string.Empty;
                if (!ThisCustomer.CouponCode.IsNullOrEmptyTrimmed()) couponCode = ThisCustomer.CouponCode;

                OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Payment, "page.checkoutshippingordersummary.xml.config", couponCode));

            }

            DisplayCheckOutStepsImage();

            if (!ThisCustomer.IsRegistered)
            {
                litIsRegistered.Text = "false";
            }

            //Clear the key since user already processing it's shipping
            _customerService.ClearAddressCheckingKey(); 
            InitAddressControl();       
        }

        private void InitAddressControl()
        {
            BillingAddressControl.LabelStreetText = _stringResourceService.GetString("checkoutpayment.aspx.28");
            BillingAddressControl.LabelCityText = _stringResourceService.GetString("checkoutpayment.aspx.29");
            BillingAddressControl.LabelStateText = _stringResourceService.GetString("checkoutpayment.aspx.30");
            BillingAddressControl.LabelPostalText = _stringResourceService.GetString("checkoutpayment.aspx.31");
            BillingAddressControl.LabelEnterPostalText = _stringResourceService.GetString("checkoutpayment.aspx.32");
            BillingAddressControl.LabelCountyText = _stringResourceService.GetString("checkoutpayment.aspx.33");
            BillingAddressControl.LabelTaxText = _stringResourceService.GetString("checkoutpayment.aspx.34 ");
            BillingAddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            BillingAddressControl.IsShowBusinessTypesSelector = false;
            BillingAddressControl.BindData();
        }

        private void RegisterAjaxScript()
        {
            var script = new StringBuilder();

            script.Append("<script type='text/javascript'> \n");
            script.Append("$add_windowLoad( \n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutPayment.setPaymentTermControlId('{0}');\n", ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutPayment.setForm('{0}');\n", frmCheckOutPayment.ClientID);

            script.Append(" } \n");
            script.Append("); \n");
            script.Append("</script> \n");

            SectionTitle = _stringResourceService.GetString("checkoutpayment.aspx.9", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
            if (_cart.CartItems.Count > 0)
            {
                _cart.BuildSalesOrderDetails(true);
                _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping();
            }
            else 
            { 
                _navigationService.NavigateToShoppingCart();
            }

            if (_cart.InventoryTrimmed)
            {
               _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("shoppingcart.aspx.1"));
            }

            string couponCode = String.Empty;
            string error = String.Empty;

            bool hasCoupon = _cart.HasCoupon(ref couponCode);
            if (hasCoupon && _cart.IsCouponValid(ThisCustomer, couponCode, ref error))
            {
                panelCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                panelCoupon.Visible = false;
                if (!error.IsNullOrEmptyTrimmed())
                {
                    _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
            }

            //if in case the user process the order without fractional value until payment then 
            //open a new browser modify the quantity to decimal.
            if (_appConfigService.GetInventoryPreference().IsAllowFractional && _cart.HasCartItemWithDecimalQuantity() && _cart.HasMultipleShippingAddresses())
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(AppLogic.GetString("checkoutshippingmult.aspx.cs.3.1"));
            }
        }

        private void InitializePaymentTermControl(bool IsTokenization)
        {
            AssignCardTypeDataSources();

            bool showPaypalOptionIfMultiShipAndHasGiftRegistryAndHasMultipleShippingMethod = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems() || _cart.HasMultipleShippingMethod());
            ctrlPaymentTerm.ShowPaypalPaymentOption = showPaypalOptionIfMultiShipAndHasGiftRegistryAndHasMultipleShippingMethod;

            _paymentTermOptions = PaymentTermDTO.GetAllForGroup(ThisCustomer.ContactCode, ThisCustomer.PrimaryShippingAddress);
            if (ServiceFactory.GetInstance<IAppConfigService>().AllowMultipleShippingAddressPerOrder || _cart.HasRegistryItems() || _cart.HasMultipleShippingMethod())
            {
                _paymentTermOptions = ServiceFactory.GetInstance<IPaymentTermService>().GetPaymentTermOptionsWithoutSagePay(_paymentTermOptions);
            }
            ctrlPaymentTerm.PaymentTermOptions = _paymentTermOptions;

            if (_paymentTermOptions.Count() > 0) {
                var paymentTermOptionSelected = _paymentTermOptions.FirstOrDefault(item => item.IsSelected);
                if (paymentTermOptionSelected != null)
                {
                    ctrlPaymentTerm.PaymentMethod = paymentTermOptionSelected.PaymentMethod;
                }
                
                ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");

                AssignPaymentTermDatasources();

                if (IsTokenization)
                {
                    ctrlPaymentTerm.IsTokenization = true;
                    ctrlPaymentTerm.IsInOnePageCheckOut = false;
                }

                InitializePaymentTermControlValues();

                AssignPaymentTermCaptions();
                AssignPaymentTermErrorSummary();
                AssignPaymentTermValidationPrerequisites();
                InitializeTermsAndConditions();
            }
            else
            {
                pnlPaymentTerm.Visible = false;
                btnDoProcessPayment.Visible = false;
                pnlCheckoutPaymentButtons.Visible = false;
                litPaymentsMethod.Visible = false;
                litTransactionStatusMessage.Text = _stringResourceService.GetString("checkoutpayment.aspx.63");
                pnlNoAvailablePaymentStatus.Visible = true;
            }
        }

        private void AssignPaymentTermDatasources()
        {
            int currentYear = DateTime.Now.Year;
            var startYears = new List<string>();
            var expirationYears = new List<string>();

            startYears.Add(_stringResourceService.GetString("address.cs.8", true));
            expirationYears.Add(_stringResourceService.GetString("address.cs.8", true));

            for (int offsetYear = 0; offsetYear <= 10; offsetYear++)
            {
                startYears.Add((currentYear - offsetYear).ToString());
                expirationYears.Add((currentYear + offsetYear).ToString());
            }

            var months = new List<string>();
            months.Add(_stringResourceService.GetString("address.cs.7", true));
            for (int month = 1; month <= 12; month++)
            {
                months.Add(month.ToString().PadLeft(2, '0'));
            }

            ctrlPaymentTerm.StartYearDataSource = startYears;
            ctrlPaymentTerm.ExpiryYearDataSource = expirationYears;
            ctrlPaymentTerm.StartMonthDataSource = months;
            ctrlPaymentTerm.ExpiryMonthDataSource = months;
        }

        private void AssignCardTypeDataSources()
        {
            var cardTypeViewDataSource = AppLogic.GetCustomerCreditCardType(_stringResourceService.GetString("address.cs.5", true));
            ctrlPaymentTerm.CardTypeViewDataSource = cardTypeViewDataSource;
        }

        private void InitializePaymentTermControlValues()
        {
            if (!ThisCustomer.IsRegistered) return;

            ctrlPaymentTerm.EnabledCreditCardValidation = _appConfigService.ValidateCardEnabled;
            ctrlPaymentTerm.NameOnCard = ThisCustomer.PrimaryBillingAddress.CardName;
            //ctrlPaymentTerm.CardExpiryMonth = ThisCustomer.PrimaryBillingAddress.CardExpirationMonth;
            //ctrlPaymentTerm.CardExpiryYear = ThisCustomer.PrimaryBillingAddress.CardExpirationYear;
            //ctrlPaymentTerm.CardType = ThisCustomer.PrimaryBillingAddress.CardType;
            //ctrlPaymentTerm.CardStartMonth = ThisCustomer.PrimaryBillingAddress.CardStartMonth;
            //ctrlPaymentTerm.CardStartYear = ThisCustomer.PrimaryBillingAddress.CardStartYear;
        }

        private void AssignPaymentTermCaptions()
        {
            int customerSkinID = Customer.Current.SkinID;
            string customerLocaleSetting = Customer.Current.LocaleSetting;

            var resource = ResourceProvider.GetPaymentTermControlDefaultResources();
            resource.NameOnCardCaption = _stringResourceService.GetString("checkoutpayment.aspx.15");
            resource.NoPaymentRequiredCaption = _stringResourceService.GetString("checkoutpayment.aspx.8");
            resource.CardNumberCaption = _stringResourceService.GetString("checkoutpayment.aspx.16");
            resource.CVVCaption = _stringResourceService.GetString("checkoutpayment.aspx.17");
            resource.WhatIsCaption = _stringResourceService.GetString("checkoutpayment.aspx.23");
            resource.CardTypeCaption = _stringResourceService.GetString("checkoutpayment.aspx.18");
            resource.CardStartDateCaption = _stringResourceService.GetString("checkoutpayment.aspx.19");
            resource.ExpirationDateCaption = _stringResourceService.GetString("checkoutpayment.aspx.20");
            resource.CardIssueNumberCaption = _stringResourceService.GetString("checkoutpayment.aspx.21");
            resource.CardIssueNumberInfoCaption = _stringResourceService.GetString("checkoutpayment.aspx.22");
            resource.SaveCardAsCaption = _stringResourceService.GetString("checkoutpayment.aspx.13");
            resource.SaveThisCreditCardInfoCaption = _stringResourceService.GetString("checkoutpayment.aspx.14");
            resource.PONumberCaption = _stringResourceService.GetString("checkoutpayment.aspx.24");
            ctrlPaymentTerm.LoadStringResources(resource);
        }

        private void AssignPaymentTermErrorSummary()
        {
            ctrlPaymentTerm.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignPaymentTermValidationPrerequisites()
        {
            ctrlPaymentTerm.PaymentTermRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.10", true);
            ctrlPaymentTerm.NameOnCardRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.11", true);
            ctrlPaymentTerm.CardNumberRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.12", true);
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.14", true);
            ctrlPaymentTerm.CVVRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.13", true);
            ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.15", true);
            ctrlPaymentTerm.StartMonthInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.25", true);
            ctrlPaymentTerm.StartYearInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.26", true);
            ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.16", true);
            ctrlPaymentTerm.UnknownCardTypeErrorMessage = _stringResourceService.GetString("checkout1.aspx.17", true);
            ctrlPaymentTerm.NoCardNumberProvidedErrorMessage = _stringResourceService.GetString("checkout1.aspx.18", true);
            ctrlPaymentTerm.CardNumberInvalidFormatErrorMessage = _stringResourceService.GetString("checkout1.aspx.19", true);
            ctrlPaymentTerm.CardNumberInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.20", true);
            ctrlPaymentTerm.CardNumberInAppropriateNumberOfDigitsErrorMessage = _stringResourceService.GetString("checkout1.aspx.21", true);
            ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.27", true);
        }

        private void InitializeTermsAndConditions()
        {
            if (AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout"))
            {
                var t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                string resouce1 = _stringResourceService.GetString("checkoutpayment.aspx.5", true);

                ctrlPaymentTerm.RequireTermsAndConditions = true;
                ctrlPaymentTerm.RequireTermsAndConditionsPrompt = resouce1;
                ctrlPaymentTerm.TermsAndConditionsHTML = t.Contents;
            }
            else
            {
                ctrlPaymentTerm.RequireTermsAndConditions = false;
            }
        }
        
        private void CheckIfWeShouldRequirePayment()
        {

            if (_cart.GetOrderBalance() == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                ctrlPaymentTerm.NoPaymentRequired = true;
                _cart.MakePaymentTermNotRequired();
                _isRequirePayment = false;
            }
            else
            {
                ctrlPaymentTerm.NoPaymentRequired = false;
                _isRequirePayment = true;
            }

        }

        private void CreditCardOptionsRenderer()
        {
            //For RC2 Optimization : need to convert to xml

            if (ThisCustomer.IsRegistered)
            {

                if (!this.IsPostBack)
                {
                    AppLogic.GenerateCreditCardCodeSaltIV(ThisCustomer);
                }

                var creditCards = CreditCardDTO.GetCreditCards(ThisCustomer.CustomerCode);

                var creditOptionsHTML = new StringBuilder();

                creditOptionsHTML.Append("<div class='clear-both height-12'></div>");


                creditOptionsHTML.Append("<div id='credit-card-options-wrapper'>");

                creditOptionsHTML.Append("<div id='credit-card-options-header-wrapper'>");

                creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", String.Empty);
                creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkoutpayment.aspx.11"));
                creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("address.cs.5"));
                creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkoutpayment.aspx.12"));
                creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left custom-font-style'>{0}</div>", String.Empty);

                creditOptionsHTML.Append("</div>");

                creditOptionsHTML.Append("<div class='clear-both'></div>");

                int counter = 1;

                string thisOption = String.Empty;
                string isPrimaryAddress = String.Empty;

                foreach (CreditCardDTO credit in creditCards)
                {

                    if (credit.CreditCardCode == ThisCustomer.PrimaryBillingAddress.AddressID)
                    {
                        thisOption = "checked";
                        isPrimaryAddress = "class='is-primary-address'";
                    }
                    else
                    {
                        thisOption = String.Empty;
                        isPrimaryAddress = String.Empty;
                    }

                    string creditCardCode = AppLogic.EncryptCreditCardCode(ThisCustomer, credit.CreditCardCode);
                    string creditOption = String.Format("<input type='radio' id='{2}' {1} name='credit-card-code' value = '{0}' {3}/>", creditCardCode, thisOption, counter, isPrimaryAddress);

                    string description = String.Empty;

                    if (credit.RefNo > 0)
                    {
                        description = credit.Description;
                    }

                    creditOptionsHTML.Append("<div class='opc-credit-card-options-row'>");

                    creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left'>{0}</div>", creditOption);
                    creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left'>{0}{1}</div>", credit.NameOnCard, String.Format("<div class='clear-both'></div><div id='{1}-credit-card-description'>{0} <div class='clear-both'></div> </div>", description, counter));

                    if (credit.RefNo > 0)
                    {

                        string lastFour = String.Empty;

                        if (credit.CardNumber.Length > 0)
                        {
                            lastFour = credit.CardNumber.Substring(credit.CardNumber.Length - 4);
                            lastFour = String.Format("&nbsp;<span class=\"MaskNumber\">ending in {0}</span>", lastFour);
                        }


                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left' id='{2}-credit-card-type'>{0} {1}</div>", credit.CardType, lastFour, counter);
                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'  id='{1}-credit-card-expiry'>{0}</div>", String.Format("{0}/{1}", credit.ExpMonth, credit.ExpYear), counter);
                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left' id='{1}-credit-card-clear'>{0}</div>", String.Format("<a class='opc-clearcard' id='opc::{0}::{1}' href='javascript:void(1);'>Clear</a>", creditCardCode, counter), counter);

                    }
                    else
                    {

                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left'>{0}</div>", String.Empty);
                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'>{0}</div>", String.Empty);
                        creditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left'>{0}</div>", String.Empty);
                    }


                    creditOptionsHTML.Append("</div>");

                    creditOptionsHTML.Append("<div class='clear-both'></div>");

                    counter++;

                }

                creditOptionsHTML.Append("</div>");
                creditOptionsHTML.Append("<div class='clear-both height-12'></div>");

                LtrCreditCardOptionsRenderer.Text = creditOptionsHTML.ToString();

            }
        }

        private void BillingAddressGridRenderer()
        {
            //For RC2 Optimization : need to convert to xml

            if (!this.IsPostBack)
            {
                AppLogic.GenerateCreditCardCodeSaltIV(ThisCustomer);
            }

            var billingAddressListing = new StringBuilder();

            billingAddressListing.Append("<div id='billing-address-options-wrapper'>");

            billingAddressListing.Append("<div id='credit-card-options-header-wrapper'>");

            billingAddressListing.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", String.Empty);
            billingAddressListing.AppendFormat("<div class='option-billing-account-name-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkoutpayment.aspx.41"));
            billingAddressListing.AppendFormat("<div class='option-billing-country-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkoutpayment.aspx.42"));
            billingAddressListing.AppendFormat("<div class='option-billing-address-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkoutpayment.aspx.28"));

            billingAddressListing.Append("</div>");

            billingAddressListing.Append("<div class='clear-both'></div>");

            int counter = 1;

            string thisOption = String.Empty;
            string isPrimaryAddress = String.Empty;
            string customerCode = String.Empty;

            if (ThisCustomer.IsRegistered)
            {
                customerCode = ThisCustomer.ContactCode;
            }
            else
            {
                customerCode = ThisCustomer.AnonymousCustomerCode;
            }

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), 1, DB.SQuote(customerCode))))
                {
                    while (reader.Read())
                    {

                        if (reader.GetString(0) == ThisCustomer.PrimaryBillingAddress.AddressID)
                        {
                            thisOption = "checked";
                            isPrimaryAddress = "class='is-primary-address'";
                        }
                        else
                        {
                            thisOption = String.Empty;
                            isPrimaryAddress = String.Empty;
                        }

                        string creditCardCode = AppLogic.EncryptCreditCardCode(ThisCustomer, reader.GetString(0));
                        string creditOption = String.Format("<input type='radio' id='{2}' {1} name='multiple-billing-address' value = '{0}' {3}/>", creditCardCode, thisOption, counter, isPrimaryAddress);
                       
                        billingAddressListing.Append("<div class='billing-address-options-row'>");

                        billingAddressListing.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left'>{0}</div>", creditOption);
                        billingAddressListing.AppendFormat("<div class='option-billing-account-name-place-holder float-left custom-font-style'>{0}</div>", reader.GetString(1));
                        billingAddressListing.AppendFormat("<div class='option-billing-country-place-holder float-left custom-font-style'>{0}</div>", reader.GetString(5));
                        billingAddressListing.AppendFormat("<div class='option-billing-address-place-holder float-left custom-font-style'>{0}</div>", reader.GetString(4));

                        billingAddressListing.Append("</div>");

                        billingAddressListing.Append("<div class='clear-both'></div>");

                        counter++;
                    }
                }


                billingAddressListing.Append("</div>");
                billingAddressListing.Append("<div class='clear-both height-12'></div>");

                litBillingAddressGrid.Text = billingAddressListing.ToString();
            }
        }

        private void DisplayErrorMessageIfAny()
        {
            string errorMessage = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg", true);
            DisplayErrorMessageIfAny(errorMessage);
        }

        private void DisplayErrorMessageIfAny(string errorMessage)
        {
            if (errorMessage.IsNullOrEmptyTrimmed()) return;

            if (errorMessage.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (errorMessage == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
            {
                if (AppLogic.AppConfigBool("ShowGatewayError"))
                {
                    errorMessage = ParseGatewayErrorMessage(ThisCustomer.LastGatewayErrorMessage);
                }
                else
                {
                    errorMessage = _stringResourceService.GetString("checkoutpayment.aspx.cs.1", true);
                }
            }

            errorSummary.DisplayErrorMessage(errorMessage);
        }

        public override void Validate()
        {
            base.Validate();
        }

        private void ProcessPayment()
        {
            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);
            }

            if (!_isRequirePayment) {
                _navigationService.NavigateToCheckoutReview();
            }

            bool isCustomerRegistered = Customer.Current.IsRegistered;
            bool isCreditCardTokenizationEnabled = IsCreditCardTokenizationEnabled;

            string paymentMethodFromInput = ctrlPaymentTerm.PaymentMethod;
            string paymentTermCodeFromInput = ctrlPaymentTerm.PaymentTerm;

            #region Payments

            string PAYMENT_METHOD_PAYPALX = DomainConstants.PAYMENT_METHOD_PAYPALX;
            string PAYMENT_METHOD_CREDITCARD = DomainConstants.PAYMENT_METHOD_CREDITCARD;

            if (_cart.GetOrderBalance() == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                _cart.MakePaymentTermNotRequired();
            }
            if (paymentTermCodeFromInput.ToString().Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase))
            {
                ThisCustomer.ThisCustomerSession.SetVal("PONumber", ctrlPaymentTerm.PONumber);
                if (DisplayClearOtherPaymentOptionsWarning()) { return; }
            }
            else if (paymentTermCodeFromInput.ToString().Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase))
            {
                if (DisplayClearOtherPaymentOptionsWarning()) { return; }
            }
            else if (paymentMethodFromInput == PAYMENT_METHOD_PAYPALX)
            {
                ThisCustomer.ThisCustomerSession["paypalfrom"] = "checkoutpayment";
                _cart.BuildSalesOrderDetails(ThisCustomer.CouponCode);
                _navigationService.NavigateToUrl(PayPalExpress.CheckoutURL(_cart));
            }
            else if (paymentTermCodeFromInput == ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm)
            {
                _cart.BuildSalesOrderDetails(ThisCustomer.CouponCode);
                Response.Redirect(SagePayPayment.SetSagePayServerPaymentRequest(_cart));
            }
            else if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD)
            {
                
                //Validate first the inputs (empty and invalid dropdown selection)
                //triggers the input registered validators.

                if (!IsValid) return;

                //Skip credit card valiation when card is tokenized

                if (!_skipCreditCardValidation)
                {
                    //credit card validation

                    if (!IsValidCreditCardInfo())
                    {
                        return;
                    }
                }

                UpdateAnonForAge13();

                #region Posted Data (Credit Card Information)

                string nameOnCard = ctrlPaymentTerm.NameOnCard;
                string cardNumberFromInput = ctrlPaymentTerm.CardNumber;
                string cardTypeFromInput = ctrlPaymentTerm.CardType;
                string cardExpiryYearFromInput = ctrlPaymentTerm.CardExpiryYear;
                string cardExpiryMonthFromInput = ctrlPaymentTerm.CardExpiryMonth;
                string cVVFromInput = ctrlPaymentTerm.CVV;
                string saveCreditCardAsFromInput = ctrlPaymentTerm.CardDescription;

                string cardStartMonth = string.Empty;
                string cardStartYear = string.Empty;
                string cardIssueNumber = string.Empty;

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    cardStartMonth = ctrlPaymentTerm.CardStartMonth;
                    cardStartYear = ctrlPaymentTerm.CardStartYear;
                    cardIssueNumber = ctrlPaymentTerm.CardIssueNumber;
                }

                #endregion

                #region Save Billing Address

                var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);
                var ThisAddress = Address.New(ThisCustomer, AddressTypes.Shipping);
                var aShippingAddress = ThisCustomer.PrimaryShippingAddress;

                string maskedCardNumber = String.Empty;
                bool hasInterpriseGatewayRefNo = false;
                //set the default value of creditCardCode to primary billing address

                string creditCardCode = ThisCustomer.PrimaryBillingAddress.AddressID;

                if (isCustomerRegistered)
                {

                    if (isCreditCardTokenizationEnabled && !txtCode.Text.IsNullOrEmptyTrimmed())
                    {
                        //txtCode.Text - Customer CreditCard code
                        //Override the credit card code if tokenization
                        //decrypt the credit card code from the rendered hidden text box since it is encrypted.

                        creditCardCode = AppLogic.DecryptCreditCardCode(ThisCustomer, txtCode.Text);
                        maskedCardNumber = AppLogic.GetCustomerCreditCardMaskedCardNumber(creditCardCode);
                    }

                    if (maskedCardNumber.StartsWith("X"))
                    {
                        CreditCardDTO credit = null;

                        if (!creditCardCode.IsNullOrEmptyTrimmed())
                        {
                            //set the credit card info using the creditcard code

                            credit = CreditCardDTO.Find(creditCardCode);
                        }

                        //test if the credit card info has been tokenized and saved by the client
                        //if refno > 0 means the credit card has been authorized

                        if (credit.RefNo > 0)
                        {
                            cardNumberFromInput = credit.CardNumber;
                            nameOnCard = credit.NameOnCard;
                            cardTypeFromInput = credit.CardType;
                            cardExpiryMonthFromInput = credit.ExpMonth;
                            cardExpiryYearFromInput = credit.ExpYear;

                            if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                            {
                                cardStartMonth = credit.StartMonth;
                                cardStartYear = credit.StartYear;
                            }

                            hasInterpriseGatewayRefNo = true;

                        }
                    }

                    aBillingAddress.Address1 = BillingAddressControl.Street;
                    aBillingAddress.Country = BillingAddressControl.Country;
                    aBillingAddress.PostalCode = BillingAddressControl.Postal;

                    string bCityStates = txtCityStates.Text;
                    string city = String.Empty;
                    string state = String.Empty;

                    var cityStateArray = GetCityStateArray();
                    aBillingAddress.State = cityStateArray[0];
                    aBillingAddress.City = cityStateArray[1];
                    
                    aBillingAddress.ResidenceType = aShippingAddress.ThisCustomer.PrimaryShippingAddress.ResidenceType;
                    aBillingAddress.Name = txtBillingContactName.Text;
                    aBillingAddress.Phone = txtBillingContactNumber.Text;
                    
                    if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = BillingAddressControl.County; }

                }
                else
                {

                    var primariBillingAddress = ThisCustomer.PrimaryBillingAddress;
                    aBillingAddress.Address1 = primariBillingAddress.Address1;
                    aBillingAddress.Country = primariBillingAddress.Country;
                    aBillingAddress.PostalCode = primariBillingAddress.PostalCode;
                    aBillingAddress.City = primariBillingAddress.City;
                    aBillingAddress.State = primariBillingAddress.State;
                    aBillingAddress.ResidenceType = primariBillingAddress.ResidenceType;
                    aBillingAddress.Name = primariBillingAddress.Name;
                    aBillingAddress.Phone = primariBillingAddress.Phone;
                    aBillingAddress.EMail = primariBillingAddress.EMail;
                    aBillingAddress.FirstName = primariBillingAddress.FirstName;
                    aBillingAddress.LastName = primariBillingAddress.LastName;

                }

                //Credit card code has default value of primary billing addressid
                //This will be overridden when tokenization

                aBillingAddress.AddressID = creditCardCode;
                aBillingAddress.CardNumber = cardNumberFromInput;
                aBillingAddress.CardName = nameOnCard;
                aBillingAddress.CardType = cardTypeFromInput;
                aBillingAddress.CardExpirationMonth = cardExpiryMonthFromInput;
                aBillingAddress.CardExpirationYear = cardExpiryYearFromInput;
                aBillingAddress.CustomerCode = ThisCustomer.CustomerCode;

                //Try save the new billing address if anonymous
                //if registered the billing will not be created

                aBillingAddress.Save();

                //update the address if user is registered and is already exist

                _customerService.UpdateCustomerBillTo(aBillingAddress, true);

                #endregion

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    //-> Some CCs do not have StartDate, so here we should provide Default if none was supplied.

                    string defaultCardStartMonth = DateTime.Now.Month.ToString();
                    string defaultCardStartYear = DateTime.Now.Year.ToString();

                    aBillingAddress.CardStartMonth = (cardStartMonth != "MONTH")? cardStartMonth: defaultCardStartMonth;
                    aBillingAddress.CardStartYear = (cardStartYear != "YEAR")? cardStartYear : defaultCardStartYear;
                    aBillingAddress.CardIssueNumber = cardIssueNumber;

                }

                //-> Capture the credit card number from the payment page and encrypt it so that the gateway can capture from that credit card

                if (!cardNumberFromInput.StartsWith("X"))
                {
                    string salt = String.Empty;
                    string iv = String.Empty;
                    string cardNumberEnc = AppLogic.EncryptCardNumber(cardNumberFromInput, ref salt, ref iv);
                    AppLogic.StoreCardNumberInSession(ThisCustomer, cardNumberEnc, salt, iv);
                }

                if (isCreditCardTokenizationEnabled)
                {

                    _customerService.MakeDefaultAddress(creditCardCode, AddressTypes.Billing, true);
    
                    bool saveCreditCardInfo = (AppLogic.AppConfigBool("ForceCreditCardInfoSaving") || ctrlPaymentTerm.SaveCreditCreditCardInfo);
                    ThisCustomer.ThisCustomerSession["SaveCreditCardChecked"] = saveCreditCardInfo.ToString();

                    #region "Update Address w/ CreditCardInfo"

                    string thisCardNumber = Interprise.Framework.Base.Shared.Common.MaskCardNumber(aBillingAddress.CardNumber);

                    if (!maskedCardNumber.IsNullOrEmptyTrimmed() && hasInterpriseGatewayRefNo)
                    {
                        thisCardNumber = maskedCardNumber;
                    }

                    #region Postal Code Handler

                    var parsedPostalCode = InterpriseHelper.ParsePostalCode(aBillingAddress.Country, aBillingAddress.PostalCode);
                    string postal = parsedPostalCode.PostalCode;
                    int plus4 = parsedPostalCode.Plus4;

                    #endregion

                    var sql = new StringBuilder();

                    sql.Append(" UPDATE CustomerCreditCard ");
                    sql.AppendFormat(" SET CreditCardDescription = {0}, MaskedCardNumber = {1}, NameOnCard = {2}, ", saveCreditCardAsFromInput.ToDbQuote(), thisCardNumber.ToDbQuote(), nameOnCard.ToDbQuote());
                    sql.AppendFormat(" Address = {0}, City = {1}, State={2}, ", aBillingAddress.Address1.ToDbQuote(), aBillingAddress.City.ToDbQuote(), aBillingAddress.State.ToDbQuote());

                    if(plus4 == 0){
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4=NULL, ",  postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote());
                    }else{
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4={2}, ",  postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote(), plus4);
                    }

                    sql.AppendFormat(" ExpMonth={0}, ExpYear={1}, Telephone={2}, ",  InterpriseHelper.ToInterpriseExpMonth(aBillingAddress.CardExpirationMonth).ToDbQuote(),  aBillingAddress.CardExpirationYear.ToDbQuote(),  aBillingAddress.Phone.ToDbQuote());
                    sql.AppendFormat(" CreditCardType = {0}, DateModified=getdate() ",  aBillingAddress.CardType.ToDbQuote());

                    sql.AppendFormat(" WHERE CreditCardCode={0} ",  creditCardCode.ToDbQuote());

                    DB.ExecuteSQL(sql.ToString());
                    sql.Clear();

                    #endregion

                    DB.ExecuteSQL(@"UPDATE Customer SET Creditcardcode={0} WHERE CustomerCode={1}", DB.SQuote(creditCardCode), DB.SQuote(ThisCustomer.CustomerCode));

                    AppLogic.ClearCreditCardCodeInSession(ThisCustomer);

                }
                else
                {
                    if (ThisCustomer.IsRegistered)
                    {
                        _customerService.UpdateCustomerBillTo(aBillingAddress, true);
                        _customerService.MakeDefaultAddress(creditCardCode, AddressTypes.Billing, true);
                    }
                }

                AppLogic.StoreCardExtraCodeInSession(ThisCustomer, cVVFromInput);
                AppLogic.SavePostalCode(aBillingAddress);

                //Redirect to Confirmation Page

            }
            
            InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, paymentTermCodeFromInput);
            _navigationService.NavigateToUrl("checkoutreview.aspx");
            #endregion

        }

        private bool IsValidCreditCardInfo()
        {

            if (IsCreditCardTokenizationEnabled && ctrlPaymentTerm.CVV.IsNullOrEmptyTrimmed())
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CVVRequiredErrorMessage);
                return false;
            }

            var ccValidator = new CreditCardValidator();
            
            //-> Validate Expiration Date
            if (!ccValidator.IsValidExpirationDate(string.Concat(ctrlPaymentTerm.CardExpiryYear, ctrlPaymentTerm.CardExpiryMonth)))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage);
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationYearInvalidErrorMessage);
                return false;
            }

            ccValidator.AcceptedCardTypes = ctrlPaymentTerm.CardType;

            if (ccValidator.AcceptedCardTypes.Contains("0"))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardTypeInvalidErrorMessage);
                return false;
            }

            string cardNumber = ctrlPaymentTerm.CardNumber;

            bool isValidCardNumber = true;
            //if enabled continue the validation else let the gateways validate the card number
            if (_appConfigService.ValidateCardEnabled)
            {
                isValidCardNumber = ccValidator.ValidateCardNumber(cardNumber);
            }

            if (!ccValidator.IsValidCardType(cardNumber) || !isValidCardNumber)
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardNumberInvalidErrorMessage);
                return false;
            }

            return true;
        }

        private string[] GetCityStateArray()
        {
            var arrCityState = new string[2]; 

            if (!txtCityStates.Text.IsNullOrEmptyTrimmed())
            {
                var cityState = txtCityStates.Text.Split(',');
                if (cityState.Length > 1)
                {
                    arrCityState[0] = cityState[0].Trim();
                    arrCityState[1] = cityState[1].Trim();
                }
                else
                {
                    arrCityState[0] = String.Empty;
                    arrCityState[1] = cityState[0].Trim();
                }
            }
            else
            {
                arrCityState[0] = BillingAddressControl.State;
                arrCityState[1] = BillingAddressControl.City;
            }

            return arrCityState;

        }

        private bool IsValidCreditCard()
        {
            bool isTokenization = IsCreditCardTokenizationEnabled;
            string cardType = ctrlPaymentTerm.CardType;
            string cardNumber = ctrlPaymentTerm.CardNumber;
            string expiryYear = ctrlPaymentTerm.CardExpiryYear;
            string expiryMonth = ctrlPaymentTerm.CardExpiryMonth;
            string invalidCardNumber = _stringResourceService.GetString("checkout1.aspx.27", true);

            var ccValidator = new CreditCardValidator();

            string cardNumberInvalidErrorMessage = string.Empty;
            //See if we should use the card number on file.
            //We also want to see if the card number starts with an *.
            //If it doesn't it probably means the user entered a new number.
            if (cardNumber.StartsWith("*"))
            {
                //Get the stored card number.
                cardNumber = ThisCustomer.PrimaryBillingAddress.CardNumber;
            }

            if (cardNumber.StartsWith("X"))
            {
                return true;
            }

            return true;
        }

        private void UpdateAnonForAge13()
        {
            if (ThisCustomer.IsRegistered) return;

            int isupdated = 1;
            string updateAnonRecordIfIsover13checked = string.Format("UPDATE EcommerceCustomer SET IsOver13 = 1, IsUpdated = {0} WHERE CustomerID = {1}", DB.SQuote(isupdated.ToString()), DB.SQuote(ThisCustomer.CustomerID.ToString()));
            DB.ExecuteSQL(updateAnonRecordIfIsover13checked);
            ThisCustomer.Update();
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_4.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                var rhs = (RectangleHotSpot)checkoutheadergraphic.HotSpots[i];
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1) rhs.AlternateText = _stringResourceService.GetString("checkoutpayment.aspx.2", true);
                if (rhs.NavigateUrl.IndexOf("account") != -1) rhs.AlternateText = _stringResourceService.GetString("checkoutpayment.aspx.3", true);
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1) rhs.AlternateText = _stringResourceService.GetString("checkoutpayment.aspx.4", true);
            }
            if (!AppLogic.AppConfigBool("SkipShippingOnCheckout"))
            {
                checkoutheadergraphic.HotSpots[2].HotSpotMode = HotSpotMode.Navigate;
                if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = "checkout1.aspx";
                }
                else
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = CommonLogic.IIF(_cart.HasMultipleShippingAddresses(), "checkoutshippingmult.aspx", "checkoutshipping.aspx");
                }
            }
        }

        private void DoPassingOfValueWhenTokenizedCreditCard()
        {
            if (!ThisCustomer.IsRegistered || !IsCreditCardTokenizationEnabled) return;

            if (txtCode.Text.IsNullOrEmptyTrimmed()) return;

            var defaultBillingAddress = ThisCustomer.PrimaryBillingAddress;

            string addressid = AppLogic.DecryptCreditCardCode(ThisCustomer, txtCode.Text);

            if (addressid.IsNullOrEmptyTrimmed()) return;

            defaultBillingAddress = ThisCustomer.BillingAddresses.FirstOrDefault(a => a.AddressID == addressid);

            var addressDto = CreditCardDTO.Find(addressid);

            if (addressDto.RefNo == 0) return;

            ctrlPaymentTerm.CardExpiryMonth = defaultBillingAddress.CardExpirationMonth;
            ctrlPaymentTerm.CardExpiryYear = defaultBillingAddress.CardExpirationYear;
            ctrlPaymentTerm.CardType = defaultBillingAddress.CardType;
            ctrlPaymentTerm.CardStartMonth = defaultBillingAddress.CardStartMonth;
            ctrlPaymentTerm.CardStartYear = defaultBillingAddress.CardStartYear;

            // set the skip registration for validation

            _skipCreditCardValidation = true;

            // set the credit card control to readonly to bypass validation
            ctrlPaymentTerm.CardNumberControl.ReadOnly = true;
            ctrlPaymentTerm.CCVCControl.ReadOnly = true;
        }

        private string ParseGatewayErrorMessage(string gatewayMessage)
        {
            string returnMessage = String.Empty;

            if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_DIGIT) != -1 ||
                gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_FORMAT) != -1 ||
                gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_FORMAT2) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            }
            else if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_DATE_CHECK_FAILED_EXPIRED) != -1 ||
                     gatewayMessage.IndexOf(GatewayErrorCodes.CARD_DATE_CHECK_FAILED_INVALID) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.cs.2", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            }
            else if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_TYPE) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            }
            else
            {
                returnMessage = gatewayMessage;
            }

            return returnMessage;
        }

        private void InitializeOtherPaymentOptionControl()
        {
            var customer = _authenticationService.GetCurrentLoggedInCustomer();
            if (customer.IsNotRegistered) { return; }

            // set stringresources...
            ctrlOtherPaymentOption.Header = AppLogic.GetString("checkoutpayment.aspx.54");
            ctrlOtherPaymentOption.HeaderCreditMemo = AppLogic.GetString("checkoutpayment.aspx.55");
            ctrlOtherPaymentOption.HeaderLoyaltyPoints = AppLogic.GetString("checkoutpayment.aspx.56");
            ctrlOtherPaymentOption.HeaderGiftCode = AppLogic.GetString("checkoutpayment.aspx.57");
            ctrlOtherPaymentOption.HeaderBalanceAvailable = AppLogic.GetString("checkoutpayment.aspx.58");
            ctrlOtherPaymentOption.HeaderApplyAmount = AppLogic.GetString("checkoutpayment.aspx.59");
            ctrlOtherPaymentOption.ButtonApplyCaption = AppLogic.GetString("checkoutpayment.aspx.60");
            ctrlOtherPaymentOption.PointsEarnedText = AppLogic.GetString("checkoutpayment.aspx.67");
            ctrlOtherPaymentOption.ButtonAddGiftCodeTooltip = AppLogic.GetString("checkoutpayment.aspx.68");
            ctrlOtherPaymentOption.GiftCodeText = AppLogic.GetString("checkoutpayment.aspx.69");
            ctrlOtherPaymentOption.ValidationGiftCodeEmpty = AppLogic.GetString("checkoutpayment.aspx.70");
            ctrlOtherPaymentOption.ValidationGiftCodeInvalid = AppLogic.GetString("checkoutpayment.aspx.71");
            ctrlOtherPaymentOption.ValidationGiftCodeZeroBalance = AppLogic.GetString("checkoutpayment.aspx.72");
            ctrlOtherPaymentOption.LoaderText = AppLogic.GetString("checkoutpayment.aspx.73");
            ctrlOtherPaymentOption.ButtonSaveGiftCodeTooltip = AppLogic.GetString("checkoutpayment.aspx.74");
            ctrlOtherPaymentOption.ButtonCancelGiftCodeTooltip = AppLogic.GetString("checkoutpayment.aspx.75");
            ctrlOtherPaymentOption.IsCreditMemoEnabled = _appConfigService.CreditRedemptionIsEnabled;
            ctrlOtherPaymentOption.IsLoyaltyPointsEnabled = _appConfigService.LoyaltyPointsEnabled;
            ctrlOtherPaymentOption.IsGiftCodeEnabled = _appConfigService.GiftCodeEnabled;
            ctrlOtherPaymentOption.RedemptionMultiplier = _customerService.GetRedemptionMultiplier();
            ctrlOtherPaymentOption.CurrencySymbol = _currencyService.GetCurrencySymbol();
            lblNewGiftCode.Text = AppLogic.GetString("checkoutpayment.aspx.76");

            if (_appConfigService.CreditRedemptionIsEnabled)
            {
                // get available creditmemos
                var creditMemos = _customerService.GetCustomerCreditMemosWithRemainingBalance().ToList();
                if (creditMemos.Count > 0) { ctrlOtherPaymentOption.CreditMemosJSON = creditMemos.ToJSON(); }
                
                // get applied creditmemos
                var creditMemosApplied = _shoppingCartService.GetAppliedCreditMemos().ToList();
                if(creditMemosApplied.Count > 0) { ctrlOtherPaymentOption.CreditMemosAppliedJSON = creditMemosApplied.ToJSON(); }
            }

            if (_appConfigService.LoyaltyPointsEnabled)
            {
                // get available loyaltypoints
                var loyaltyPoints = _customerService.GetLoyaltyPoints();
                if (loyaltyPoints != null) { ctrlOtherPaymentOption.LoyaltyPointsJSON = new List<CustomerLoyaltyPointsCustomModel>() { loyaltyPoints }.ToJSON(); }

                // get applied loyaltypoints
                var loyaltyPointsApplied = customer.ThisCustomerSession[DomainConstants.LOYALTY_POINTS];
                if (!loyaltyPointsApplied.IsNullOrEmptyTrimmed()) { ctrlOtherPaymentOption.LoyaltyPointsApplied = loyaltyPointsApplied.ToDecimal(); }
            }

            if (_appConfigService.GiftCodeEnabled)
            {
                // get available giftcodes
                var giftCodes = new List<GiftCodeCustomModel>();
                giftCodes.AddRange(_customerService.GetCustomerGiftCodes());
                giftCodes.AddRange(_shoppingCartService.GetAdditionalGiftCodes());
                ctrlOtherPaymentOption.GiftCodesJSON = giftCodes.ToList().ToJSON();

                // get applied giftcodes
                ctrlOtherPaymentOption.GiftCodesAppliedJSON = _shoppingCartService.GetAppliedGiftCodes(true)
                                                                                  .ToList()
                                                                                  .ToJSON();
            }
            
            if (_cart.HasAppliedInvalidCreditMemo() || _cart.HasAppliedInvalidLoyaltyPoints() || _cart.HasAppliedInvalidGiftCode())
            {
                errorSummary.DisplayErrorMessage(AppLogic.GetString("checkoutpayment.aspx.cs.3"));
            }
        }

        private bool DisplayClearOtherPaymentOptionsWarning()
        {
            if (ThisCustomer.ThisCustomerSession[DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS].IsNullOrEmptyTrimmed())
            {
                if (_cart.HasCoupon() || _cart.HasAppliedGiftCode() || _cart.HasAppliedLoyaltyPoints())
                {
                    DisplayErrorMessageIfAny(_stringResourceService.GetString("checkoutpayment.aspx.53", true));
                    ThisCustomer.ThisCustomerSession.SetVal(DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS, true.ToString());
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Protected Methods

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/tooltip.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/creditcard.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/paymentterm_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/checkoutpayment_ajax.js"));
            manager.LoadScriptsBeforeUI = false;

            var service = new ServiceReference("~/actionservice.asmx");
            service.InlineScript = false;
            manager.Services.Add(service);
        }

        #endregion

        #region Private Properties

        private bool IsCreditCardTokenizationEnabled
        {
            get { return isUsingInterpriseGatewayv2 && ThisCustomer.IsRegistered && AppLogic.AppConfigBool("AllowCreditCardInfoSaving"); }
        }

        #endregion

    }

}