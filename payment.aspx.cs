using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using System.Web;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;

namespace InterpriseSuiteEcommerce
{
    public partial class payment : SkinBase
    {

        #region DomainServices

        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        IOrderService _orderService = null;
        IAuthenticationService _authenticationService = null;
        INavigationService _navigationService = null;
        IAppConfigService _appConfigService = null;

        #endregion

        #region Private Members

        private IEnumerable<PaymentTermDTO> _paymentTermOptions = null;
        private CustomerTransactionModel _customerTransactionInfo = null;

        private string _invoiceCode = String.Empty;

        private Address _billToaddress = null;
        private Address _shipToAddress = null;

        private bool IsPayPalCheckout
        {
            get
            {
                return (Request.QueryString["PayPal"] ?? bool.FalseString) == bool.TrueString && Request.QueryString["token"] != null;
            }
        }

        #endregion

        #region Event Handler

        private void BindControls()
        {
            btnProcessPayment.Click += (senderObject, evt) =>
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


        #endregion

        #region On Init / Load

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeDomainServices();
            Initialize();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void InitializeDomainServices()
        {
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _orderService = ServiceFactory.GetInstance<IOrderService>();
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
        }

        private void Initialize()
        {

            RequireSecurePage();
            PageNoCache();

            if (ThisCustomer.IsNotRegistered)
            {
                _navigationService.NavigateToUrl("signin.aspx");
            }

            BindControls();
            SetUpPageRequirements();
            PaymentHelpfulTipsTopic.SetContext = this;

            if (IsPayPalCheckout)
            {
                pnlPaymentTerm.Visible = false;
                pnlPayOnlineButton.Visible = false;
                paypalStatus.Visible = true;

                decimal amountToPay = Decimal.Zero;
                Decimal.TryParse(Request.QueryString["amount"].ToString(), out amountToPay);

                if (amountToPay <= 0)
                {
                    errorSummary.DisplayErrorMessage(_stringResourceService.GetString("payment.aspx.28", true));
                }
                else
                {
                    PaypalCheckout(_invoiceCode, amountToPay, false);
                }
            }
        }

        private void SetUpPageRequirements()
        {
            _invoiceCode = CommonLogic.QueryStringCanBeDangerousContent("InvoiceCode", false);
            _customerTransactionInfo = ServiceFactory.GetInstance<ICustomerRepository>().GetCustomerTransaction(_invoiceCode);

            if (_customerTransactionInfo.IsNullOrEmptyTrimmed() ||
                (_customerTransactionInfo.CustomerCode != ThisCustomer.CustomerCode) ||
                    (_customerTransactionInfo.Type != Interprise.Framework.Base.Shared.Const.CUSTOMER_INVOICE))
            {
                _navigationService.NavigateToUrl("~/t-InvalidOrderNumber.aspx");
            }

            CheckForFailedTransaction();

            RegisterControlPayment();
            InitializePaymentTermControl();

            RenderPageStringResources();
            RenderTransactionDetails();
        }

        #endregion

        #region Bill To Customer


        private void RenderTransactionDetails()
        {
            decimal outstanding = _customerTransactionInfo.Outstanding;

            litInvoiceDate.Text = _customerTransactionInfo.DocumentDate.ToShortDateString();
            litInvoiceAmount.Text = _customerTransactionInfo.Total.ToCustomerCurrency();
            litBalanceDue.Text = outstanding.ToCustomerCurrency();
            litBalanceDueDate.Text = _customerTransactionInfo.DueDate.ToShortDateString();

            txtAmount.Text = outstanding.ToCustomerRawCurrency();

            AppLogic.SetSessionCookie("ContactGUID", ThisCustomer.ContactGUID.ToString());
            AppLogic.SetSessionCookie("OrderNumber", _invoiceCode);

            litReceipt.Text = "<a href='receipt.aspx?ordernumber={0}&customerguid={1}' target='_'>{0}</a>".FormatWith(_invoiceCode, ThisCustomer.ContactGUID);
           
           
        }

        private void RenderPageStringResources()
        {
            litPageTitle.Text = _stringResourceService.GetString("payment.aspx.1");
            litDateCaption.Text = _stringResourceService.GetString("payment.aspx.3");
            litAmountCaption.Text = _stringResourceService.GetString("payment.aspx.5");
            litViewReceipt.Text = _stringResourceService.GetString("payment.aspx.13");
            litReceiptDescription.Text = _stringResourceService.GetString("payment.aspx.15");
        }

        #endregion

        #region Update Customer Info

        private void UpdateBillingAddress(bool isCreditCard)
        {
            _billToaddress = ThisCustomer.PrimaryBillingAddress;

            var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);

            aBillingAddress.Address1 = _billToaddress.Address1;
            aBillingAddress.Country = _billToaddress.Country;
            aBillingAddress.PostalCode = _billToaddress.PostalCode;
            aBillingAddress.City = _billToaddress.City;
            aBillingAddress.State = _billToaddress.State;
            aBillingAddress.County = _billToaddress.Country;
            aBillingAddress.EMail = _billToaddress.EMail;
            aBillingAddress.Name = _billToaddress.Name;
            aBillingAddress.ResidenceType = _billToaddress.ResidenceType;
            aBillingAddress.Phone = _billToaddress.Phone;

            if (isCreditCard)
            {
                string cardNumberFromInput = ctrlPaymentTerm.CardNumber;
                string creditCardCode = ThisCustomer.PrimaryBillingAddress.AddressID;

                aBillingAddress.AddressID = creditCardCode;
                aBillingAddress.CardNumber = cardNumberFromInput;
                aBillingAddress.CardName = ctrlPaymentTerm.NameOnCard;
                aBillingAddress.CardType = ctrlPaymentTerm.CardType;
                aBillingAddress.CardExpirationMonth = ctrlPaymentTerm.CardExpiryMonth;

                aBillingAddress.CardExpirationYear = ctrlPaymentTerm.CardExpiryYear;
                aBillingAddress.CustomerCode = ThisCustomer.CustomerCode;

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    string cardStartMonth = String.Empty;
                    string cardStartYear = String.Empty;
                    string cardIssueNumber = String.Empty;

                    cardStartMonth = ctrlPaymentTerm.CardStartMonth;
                    cardStartYear = ctrlPaymentTerm.CardStartYear;
                    cardIssueNumber = ctrlPaymentTerm.CardIssueNumber;

                    //-> Some CCs do not have StartDate, so here we should provide Default if none was supplied.

                    string defaultCardStartMonth = DateTime.Now.Month.ToString();
                    string defaultCardStartYear = DateTime.Now.Year.ToString();

                    aBillingAddress.CardStartMonth = (cardStartMonth != "MONTH") ? cardStartMonth : defaultCardStartMonth;
                    aBillingAddress.CardStartYear = (cardStartYear != "YEAR") ? cardStartYear : defaultCardStartYear;
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

                AppLogic.StoreCardExtraCodeInSession(ThisCustomer, ctrlPaymentTerm.CVV);
            }

            aBillingAddress.Save();

            ThisCustomer.EMail = aBillingAddress.EMail;
            ThisCustomer.ContactFullName = aBillingAddress.Name;
            ThisCustomer.ContactCode = aBillingAddress.ContactCode;

            ThisCustomer.PrimaryBillingAddress = aBillingAddress;
        }


        #endregion

        #region Payment Control Set Up

        private void AssignCardTypeDataSources()
        {
            var cardTypeViewDataSource = AppLogic.GetCustomerCreditCardType(_stringResourceService.GetString("payment.aspx.72", true));
            ctrlPaymentTerm.CardTypeViewDataSource = cardTypeViewDataSource;
        }


        private void InitializePaymentTermControl()
        {
           

            string statusMessageTextKey = "payment.aspx.21";

            if (_customerTransactionInfo.IsPosted)
            {
                statusMessageTextKey = "payment.aspx.22";
            }

            if (_customerTransactionInfo.IsVoided)
            {
                statusMessageTextKey = "payment.aspx.23";
            }

            if (_customerTransactionInfo.IsPaid)
            {
                statusMessageTextKey = "payment.aspx.21";
            }


            _paymentTermOptions = PaymentTermDTO.GetOnlinePaymentTerms(ThisCustomer.ContactCode, ThisCustomer.PrimaryShippingAddress);

            if (_customerTransactionInfo.Outstanding <= 0 || _customerTransactionInfo.IsVoided || _customerTransactionInfo.IsPaid)
            {
                ctrlPaymentTerm.Visible = false;
                pnlPayOnlineButton.Visible = false;
                pnlBalanceDetails.Visible = false;

                litTransactionStatusMessage.Text = _stringResourceService.GetString(statusMessageTextKey).FormatWith(_customerTransactionInfo.Type, _invoiceCode);
                pnlNoAvailablePaymentStatus.Visible = true;
                return;
            }

            ctrlPaymentTerm.ThisCustomer = ThisCustomer;
            AssignCardTypeDataSources();
            ctrlPaymentTerm.PaymentTermOptions = _paymentTermOptions;

            if (_paymentTermOptions.Count() > 0)
            {
                ctrlPaymentTerm.PaymentMethod = _paymentTermOptions.FirstOrDefault(item => item.IsSelected).PaymentMethod;
                ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");
                
                ctrlPaymentTerm.IsTokenization = false;
                ctrlPaymentTerm.IsInOnePageCheckOut = false;
                ctrlPaymentTerm.IsInPaymentOnline = true;

                AssignPaymentTermDatasources();
                AssignPaymentTermCaptions();
                AssignPaymentTermErrorSummary();
                AssignPaymentTermValidationPrerequisites();
                InitializeTermsAndConditions();
            }
            else
            {
                ctrlPaymentTerm.Visible = false;
                pnlPayOnlineButton.Visible = false;
                litTransactionStatusMessage.Text = _stringResourceService.GetString("payment.aspx.39");
                pnlNoAvailablePaymentStatus.Visible = true;
            }

        }

        private void AssignPaymentTermDatasources()
        {
            int currentYear = DateTime.Now.Year;
            var startYears = new List<string>();
            var expirationYears = new List<string>();
            var cardTypeViewDataSource = AppLogic.GetCustomerCreditCardType(_stringResourceService.GetString("payment.aspx.72", true));

            startYears.Add(_stringResourceService.GetString("payment.aspx.73", true));
            expirationYears.Add(_stringResourceService.GetString("payment.aspx.73", true));

            for (int offsetYear = 0; offsetYear <= 10; offsetYear++)
            {
                startYears.Add((currentYear - offsetYear).ToString());
                expirationYears.Add((currentYear + offsetYear).ToString());
            }

            var months = new List<string>();
            months.Add(_stringResourceService.GetString("payment.aspx.74", true));

            for (int month = 1; month <= 12; month++)
            {
                months.Add(month.ToString().PadLeft(2, '0'));
            }

            ctrlPaymentTerm.CardTypeViewDataSource = cardTypeViewDataSource;
            ctrlPaymentTerm.StartYearDataSource = startYears;
            ctrlPaymentTerm.ExpiryYearDataSource = expirationYears;
            ctrlPaymentTerm.StartMonthDataSource = months;
            ctrlPaymentTerm.ExpiryMonthDataSource = months;
        }

        private void AssignPaymentTermCaptions()
        {
            var resource = ResourceProvider.GetPaymentTermControlDefaultResources();

            resource.NameOnCardCaption = _stringResourceService.GetString("payment.aspx.40");
            resource.NoPaymentRequiredCaption = _stringResourceService.GetString("payment.aspx.41");
            resource.CardNumberCaption = _stringResourceService.GetString("payment.aspx.42"); 
            resource.CVVCaption = _stringResourceService.GetString("payment.aspx.43"); 
            resource.WhatIsCaption = _stringResourceService.GetString("payment.aspx.44"); 
            resource.CardTypeCaption = _stringResourceService.GetString("payment.aspx.45"); 
            resource.CardStartDateCaption = _stringResourceService.GetString("payment.aspx.46"); 
            resource.ExpirationDateCaption = _stringResourceService.GetString("payment.aspx.47"); 
            resource.CardIssueNumberCaption = _stringResourceService.GetString("payment.aspx.48"); 
            resource.CardIssueNumberInfoCaption = _stringResourceService.GetString("payment.aspx.49"); 
            resource.SaveCardAsCaption = _stringResourceService.GetString("payment.aspx.50"); 
            resource.SaveThisCreditCardInfoCaption = _stringResourceService.GetString("payment.aspx.51"); 
            resource.PONumberCaption = _stringResourceService.GetString("payment.aspx.52"); 
            resource.ExternalCaption = _stringResourceService.GetString("payment.aspx.53");

            ctrlPaymentTerm.LoadStringResources(resource);
        }

        private void AssignPaymentTermErrorSummary()
        {
            ctrlPaymentTerm.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignPaymentTermValidationPrerequisites()
        {
            ctrlPaymentTerm.PaymentTermRequiredErrorMessage = _stringResourceService.GetString("payment.aspx.54", true);
            ctrlPaymentTerm.NameOnCardRequiredErrorMessage = _stringResourceService.GetString("payment.aspx.55", true); 
            ctrlPaymentTerm.CardNumberRequiredErrorMessage = _stringResourceService.GetString("payment.aspx.56", true);
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.57", true); 
            ctrlPaymentTerm.CVVRequiredErrorMessage = _stringResourceService.GetString("payment.aspx.58", true);
            ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.59", true); 
            ctrlPaymentTerm.StartMonthInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.60", true);
            ctrlPaymentTerm.StartYearInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.61", true); 
            ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.62", true); 
            ctrlPaymentTerm.UnknownCardTypeErrorMessage = _stringResourceService.GetString("payment.aspx.63", true); 
            ctrlPaymentTerm.NoCardNumberProvidedErrorMessage = _stringResourceService.GetString("payment.aspx.64", true); 
            ctrlPaymentTerm.CardNumberInvalidFormatErrorMessage = _stringResourceService.GetString("payment.aspx.65", true); 
            ctrlPaymentTerm.CardNumberInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.66", true);
            ctrlPaymentTerm.CardNumberInAppropriateNumberOfDigitsErrorMessage = _stringResourceService.GetString("payment.aspx.67", true); 
            ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage = _stringResourceService.GetString("payment.aspx.68", true); 
        }

        private void InitializeTermsAndConditions()
        {
            if (AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout"))
            {
                var t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                string resouce1 = AppLogic.GetString("payment.aspx.69", true); // checkoutpayment.aspx.5

                ctrlPaymentTerm.RequireTermsAndConditions = true;
                ctrlPaymentTerm.RequireTermsAndConditionsPrompt = resouce1;
                ctrlPaymentTerm.TermsAndConditionsHTML = t.Contents;
            }
            else
            {
                ctrlPaymentTerm.RequireTermsAndConditions = false;
            }


        }


        #endregion

        #region Payment Set Us JS Scripts

        private void RegisterControlPayment()
        {
            var script = new StringBuilder();

            script.Append("<script type='text/javascript'> \n");
            script.Append("$(window).load( \n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "payment.aspx.53", AppLogic.GetString("payment.aspx.53", true));
            script.AppendFormat("   ise.Pages.CheckOutPayment.setPaymentTermControlId('{0}');\n", ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutPayment.setForm('{0}');\n", frmPayOnline.ClientID);

            script.Append(" } \n");
            script.Append("); \n");
            script.Append("</script> \n");

            SectionTitle = _stringResourceService.GetString("payment.aspx.24", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/tooltip.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/creditcard.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/paymentterm_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutpayment_ajax.js"));

            var service = new ServiceReference("~/actionservice.asmx");
            service.InlineScript = false;
            manager.Services.Add(service);
        }

        #endregion

        #region Process Payment

        private void ProcessPayment()
        {
            string receiptCode = String.Empty;
            string status = String.Empty;
            string paymentMethodFromInput = ctrlPaymentTerm.PaymentMethod;
            string paymentTermCodeFromInput = ctrlPaymentTerm.PaymentTerm;

            if (paymentMethodFromInput == InterpriseSuiteEcommerceControls.PaymentTermControl2.PAYMENT_METHOD_PAYPALX)
            {

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "onlinepayment";
                _navigationService.NavigateToUrl(PayPalExpress.CheckoutURL(_invoiceCode, Convert.ToDecimal(txtAmount.Text), false));

                return;
            }

            #region Validate Credit Card

            bool isCreditCard = (paymentMethodFromInput == InterpriseSuiteEcommerceControls.PaymentTermControl2.PAYMENT_METHOD_CREDITCARD);

            if (isCreditCard)
            {
                if (!IsValid) return;
                if (!IsValidCreditCardInfo()) return;
            }

            #endregion

            #region Update Billing / Customer Payment Term
            InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, paymentTermCodeFromInput);
            #endregion

            #region Verify Payment Term Code

            UpdateBillingAddress(isCreditCard);

            if (ThisCustomer.PaymentTermCode.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToUrl("payment.aspx?isPaymentTermNull=true");
            }

            #endregion

            #region Process Payment

            Gateway gatewayToUse = null;
            var simplePayment = new SimplePayment();

            try
            {

                #region Load Dataset

                if (simplePayment.InvoiceFacade == null)
                {
                    simplePayment.LoadInvoice(_invoiceCode);
                }

                status = simplePayment.PlacePayment(gatewayToUse, ThisCustomer.PrimaryBillingAddress, ThisCustomer.PrimaryShippingAddress, ref _invoiceCode, ref receiptCode, null, isCreditCard, false, decimal.Parse(txtAmount.Text), true);

                #endregion
            }
            catch (Exception ex)
            {
                simplePayment = null;

                if (ex.Message == "Unable to instantiate Default Credit Card Gateway")
                {
                    _navigationService.NavigateToPageError(_stringResourceService.GetString("payment.aspx.29", true));
                }
                else
                {
                    throw;
                }
            }

            #endregion

            VerifyPaymentsStatus(status, _invoiceCode, true);

            if (status == AppLogic.ro_OK)
            {
                SendMailNotification(simplePayment.ReceiptDate.ToShortDateString(), Convert.ToDecimal(txtAmount.Text), _invoiceCode);
                simplePayment = null;

                _navigationService.NavigateToUrl("t-paymentthankyou.aspx");
            }
           
        }

        #endregion

        #region Validate

        public override void Validate()
        {
            base.Validate();
        }

        private bool IsValidCreditCardInfo()
        {
            var ccValidator = new CreditCardValidator();

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

            if (((!ccValidator.IsValidCardType(cardNumber) || !ccValidator.ValidateCardNumber(cardNumber))))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardNumberInvalidErrorMessage);
                return false;
            }

            return true;
        }


        private void DisplayErrorMessageIfAny()
        {
            string errorMessage = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg", true);
            DisplayErrorMessageIfAny(errorMessage);
        }

        private void DisplayErrorMessageIfAny(string errorMessage)
        {
            if (CommonLogic.IsStringNullOrEmpty(errorMessage)) return;

            if (errorMessage.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (errorMessage == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
            {
               errorMessage = AppLogic.AppConfigBool("ShowGatewayError") ? ThisCustomer.ThisCustomerSession["LastGatewayErrorMessage"] :  _stringResourceService.GetString("payment.aspx.20", true);
            }

            errorSummary.DisplayErrorMessage(errorMessage);
        }

        private void CheckForFailedTransaction()
        {
           if (!CommonLogic.QueryStringBool("hasFailedTransaction")) return;
           errorSummary.DisplayErrorMessage(_stringResourceService.GetString("payment.aspx.19"));
        }

        #endregion

        #region Security

        public static void RequireSecurePage()
        {
            AppLogic.RequireSecurePage();
        }

        private void VerifyPaymentsStatus(string status, string orderNumber, bool isInvoice)
        {

            #region Verify Payments Status 

            if (status == AppLogic.ro_3DSecure)
            { // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                _navigationService.NavigateToUrl("secureform.aspx");
            }
            if (status != AppLogic.ro_OK)
            {
                ThisCustomer.IncrementFailedTransactionCount();

                if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                {
                    ThisCustomer.ResetFailedTransactionCount();
                    _navigationService.NavigateToUrl("orderfailed.aspx");
                }

                ThisCustomer.ClearTransactions(false);

                string errorMessage = status;

                if (errorMessage == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
                {
                    if (AppLogic.AppConfigBool("ShowGatewayError"))
                    {
                        errorMessage = ThisCustomer.LastGatewayErrorMessage;
                    }
                    else
                    {
                        errorMessage = _stringResourceService.GetString("payment.aspx.20");
                    }
                }

                errorSummary.DisplayErrorMessage(errorMessage);
            }

            #endregion

            ClearSessions();

            #region Verify Failed Transactions

            bool hasFailedTransaction = false;

            hasFailedTransaction = isInvoice ? _orderService.IsVoidedCustomerInvoice(orderNumber) : _orderService.IsVoidedCustomerSalesOrder(orderNumber);;

            if (hasFailedTransaction)
            {
                _navigationService.NavigateToUrl("payment.aspx?invoicecode={0}&hasFailedTransaction=true".FormatWith(orderNumber.ToUrlEncode()));
            }
            
            #endregion

        }

        private void ClearSessions()
        {
            Session.Clear();
            Session.Abandon();

            Response.Cookies.Clear();
            Response.Cache.SetNoStore();

            AppLogic.ClearCardNumberInSession(ThisCustomer);
            ThisCustomer.ClearTransactions(true);
            ThisCustomer.ThisCustomerSession.Clear();
        }

        #endregion

        #region Send Notification

        private void SendMailNotification(string date, decimal amount, string transactionID)
        {
            var runtimeParams = new List<XmlPackageParam>();


            runtimeParams.Add(new XmlPackageParam("Date", date));
            runtimeParams.Add(new XmlPackageParam("Amount", amount.ToCustomerCurrency()));
            runtimeParams.Add(new XmlPackageParam("TransactionID", transactionID));
            runtimeParams.Add(new XmlPackageParam("TransactionIDCaption", "InvoiceID"));

            string[] emailacctinfo = InterpriseHelper.GetStoreEmailAccountInfo();

            if (!ThisCustomer.EMail.IsNullOrEmptyTrimmed())
            {
                string messageToCustomer = AppLogic.RunXmlPackage("notification.newpaymentcustomer.xml.config", base.GetParser, ThisCustomer, SkinID, String.Empty, runtimeParams, true, true);
                AppLogic.SendMailRequest("New Payment Received", messageToCustomer, true, emailacctinfo[0], emailacctinfo[1], ThisCustomer.EMail, ThisCustomer.PrimaryBillingAddress.Name, String.Empty, true);
            }

            runtimeParams.Add(new XmlPackageParam("BillToName", _customerTransactionInfo.CustomerName));
            runtimeParams.Add(new XmlPackageParam("TransactionType", _customerTransactionInfo.Type));


            string sendTo = _appConfigService.OnlinePaymentEmailTo;

            if (!sendTo.IsNullOrEmptyTrimmed())
            {
                string messageToAdmin = AppLogic.RunXmlPackage("notification.newpaymentadmin.xml.config", base.GetParser, ThisCustomer, SkinID, String.Empty, runtimeParams, true, true);
                AppLogic.SendMailRequest("New Payment Received", messageToAdmin.ToString(), true, emailacctinfo[0], emailacctinfo[1], sendTo, _appConfigService.OnlinePaymentEmailToName, String.Empty, true);
            }
        }

        #endregion

        public void PaypalCheckout(string orderNumber, decimal amountToPay, bool isShippingNotRequired)
        {
            
            ThisCustomer.PaymentTermCode =  DomainConstants.PAYMENT_METHOD_CREDITCARD;
            var pp = new PayPalExpress();
            var GetPayPalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
          
            var shippingAddress = new Address()
            {
                Name = GetPayPalDetails.PayerInfo.Address.Name,
                Address1 = "{0} {1} {2}".FormatWith(GetPayPalDetails.PayerInfo.Address.Street1, (GetPayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty), GetPayPalDetails.PayerInfo.Address.Street2),
                City = GetPayPalDetails.PayerInfo.Address.CityName,
                State = GetPayPalDetails.PayerInfo.Address.StateOrProvince,
                PostalCode = GetPayPalDetails.PayerInfo.Address.PostalCode,
                Country = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.CountryName.ToString()),
                CountryISOCode = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.Country.ToString()),
                Phone = GetPayPalDetails.PayerInfo.ContactPhone ?? String.Empty
            };

            pp = new PayPalExpress();

            var paypalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
            var doExpressCheckoutResp = pp.DoExpressCheckoutPayment(paypalDetails.Token, paypalDetails.PayerInfo.PayerID, _invoiceCode, amountToPay, isShippingNotRequired);
            
            string result = String.Empty;
            if (doExpressCheckoutResp.Errors != null && !doExpressCheckoutResp.Errors[0].ErrorCode.IsNullOrEmptyTrimmed())
            {
                string errorMessage = AppLogic.AppConfigBool("ShowGatewayError") ? _stringResourceService.GetString("payment.aspx.71").FormatWith(doExpressCheckoutResp.Errors[0].ErrorCode, doExpressCheckoutResp.Errors[0].LongMessage) :
                   paypalStatus.Text = AppLogic.GetString("payment.aspx.70");
                   errorSummary.DisplayErrorMessage(errorMessage);
                   
                   pnlPaymentTerm.Visible = true;
                   pnlPayOnlineButton.Visible = true;

                return;
            }
            else
            {
                var payPalResp = new GatewayResponse(String.Empty)
                {
                    AuthorizationCode = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                    TransactionResponse = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                    Details = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                    AuthorizationTransID = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID
                };

                 Gateway gatewayToUse = null;

                 var simplePayment = new SimplePayment();

                 string status = AppLogic.ro_OK;
                 string receiptCode = string.Empty;

                try
                {
                    var billingAddress = ThisCustomer.PrimaryBillingAddress;

                    #region Load Dataset

                    if (simplePayment.InvoiceFacade == null)
                    {
                        simplePayment.LoadInvoice(_invoiceCode);
                    }
                   
                    simplePayment.PlacePayment(gatewayToUse, billingAddress, shippingAddress, ref _invoiceCode, ref receiptCode, payPalResp, IsPayPalCheckout, false, amountToPay, true);
                 

                    #endregion
                }
                catch (Exception ex)
                {
                    simplePayment = null;

                    if (ex.Message == "Unable to instantiate Default Credit Card Gateway")
                    {
                       _navigationService.NavigateToPageError(_stringResourceService.GetString("payment.aspx.27"));
                    }
                    else
                    {
                        throw;
                    }
                }

                VerifyPaymentsStatus(status, _invoiceCode, true);

                if (status == AppLogic.ro_OK)
                {
                    SendMailNotification(simplePayment.ReceiptDate.ToShortDateString(), Convert.ToDecimal(txtAmount.Text), _invoiceCode);
                    simplePayment = null;
             
                    _navigationService.NavigateToUrl("t-paymentthankyou.aspx");

                    return;
                }
            
            }
            
            }

        }
    }
