// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutpayment : SkinBase
    {
        InterpriseShoppingCart _cart = null;
        private bool _weShouldRequirePayment = true;
        private IEnumerable<PaymentTermDTO> _paymentTermOptions = null;

        #region DomainServices

        IAppConfigService _appConfigService = null;
        ICustomerService _customerService = null;
        IShoppingCartService _shoppingCartService = null;
        INavigationService _navigationService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            ctrlPaymentTerm.ThisCustomer = ThisCustomer;
			//for mobile button user control
            btnCompletePurchase.Click += btnCompletePurchase_Click;
            btnCompletePurchase2.Click += btnCompletePurchase_Click;

            InitializeDomainServices();
            base.OnInit(e);

            this.PageNoCache();
            RequireSecurePage();
            RequireCustomerRecord();

            InitializeShoppingCart();
            PerformPageAccessLogic();
            DisplayCheckOutStepsImage();
            DisplayErrorMessageIfAny();
            InitializePaymentTermControl();
            DisplayOrderSummary();
            AssignCheckOutButtonCaption();
            CheckIfWeShouldRequirePayment();
            
        }

        private void DisplayErrorMessageIfAny()
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length == 0) return;

            string errorMessage = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg", true);
            if (errorMessage.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (errorMessage == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
            {
                if (AppLogic.AppConfigBool("ShowGatewayError"))
                {
                    errorMessage = ThisCustomer.ThisCustomerSession["LastGatewayErrorMessage"];
                }
                else
                {
                    errorMessage = AppLogic.GetString("checkoutpayment.aspx.cs.1");
                }
            }
            errorSummary.DisplayErrorMessage(errorMessage);
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            _cart.BuildSalesOrderDetails();
        }

        private void DisplayOrderSummary()
        {
            if (_cart.HasMultipleShippingAddresses())
            {
                var splittedCarts = _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
                splittedCarts.ForEach(splitCart => 
                {
                    splitCart.BuildSalesOrderDetails();
                    OrderSummary.Text += splitCart.RenderHTMLLiteral(new MobileCheckOutPaymentPageLiteralRenderer());
                });
            }
            else
            {
                OrderSummary.Text = _cart.RenderHTMLLiteral(new MobileCheckOutPaymentPageLiteralRenderer());
            }
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_4.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                var rhs = checkoutheadergraphic.HotSpots[i] as RectangleHotSpot;
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.2");
                if (rhs.NavigateUrl.IndexOf("account") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.3");
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.4");
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

            _customerService.DoMobileIsNotRegisteredChecking();

            if (ThisCustomer.IsCreditOnHold)
            {
                Response.Redirect("shoppingcart.aspx");
            }
            
            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required")));
            }

            if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.1")));
            }

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
            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }
        }

        private void AssignCheckOutButtonCaption()
        {
            string value = AppLogic.GetString("checkoutpayment.aspx.6");
            btnCompletePurchase.Text = value;
            btnCompletePurchase2.Text = value;
        }

        private void InitializePaymentTermControl()
        {
            string baseTermOnThisCustomer = ThisCustomer.ContactCode;

            if (ThisCustomer.IsNotRegistered)
            {
                baseTermOnThisCustomer = ThisCustomer.AnonymousCustomerCode;
            }

            bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());
            ctrlPaymentTerm.ShowPaypalPaymentOption = hidePaypalOptionIfMultiShipAndHasGiftRegistry;

            _paymentTermOptions = PaymentTermDTO.GetAllForGroup(baseTermOnThisCustomer, ThisCustomer.PrimaryShippingAddress); //availableTerms;
            if (ServiceFactory.GetInstance<IAppConfigService>().AllowMultipleShippingAddressPerOrder || _cart.HasRegistryItems())
            {
                _paymentTermOptions = ServiceFactory.GetInstance<IPaymentTermService>().GetPaymentTermOptionsWithoutSagePay(_paymentTermOptions);
            }
            ctrlPaymentTerm.PaymentTermOptions = _paymentTermOptions;
            
            var paymentTermOptionSelected = _paymentTermOptions.FirstOrDefault(item => item.IsSelected);
            if (paymentTermOptionSelected != null)
            {
                ctrlPaymentTerm.PaymentMethod = paymentTermOptionSelected.PaymentMethod;
            }

            ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");
			//added header text instead of image
            lblCheckOutPaymentHeaderText.Text = AppLogic.GetString("mobile.checkoutpayment.aspx.1");

            AssignPaymentTermDatasources();
            InitializePaymentTermControlValues();
            AssignPaymentTermCaptions();
            AssignPaymentTermErrorSummary();
            AssignPaymentTermValidationPrerequisites();
            InitializeTermsAndConditions();
        }

        private void InitializeTermsAndConditions()
        {
            if(AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout"))
            {
                ctrlPaymentTerm.RequireTermsAndConditions = true;
                ctrlPaymentTerm.RequireTermsAndConditionsPrompt = AppLogic.GetString("checkoutpayment.aspx.5");

                var t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                ctrlPaymentTerm.TermsAndConditionsHTML = t.Contents;
            }
            else
            {
                ctrlPaymentTerm.RequireTermsAndConditions = false;
            }            
        }

        private void InitializePaymentTermControlValues()
        {
            if (!ThisCustomer.IsRegistered) return;
            
            ctrlPaymentTerm.NameOnCard = ThisCustomer.PrimaryBillingAddress.CardName;
        }

        private void AssignPaymentTermDatasources()
        {
            var cardTypes = new List<string>();
            cardTypes.Add(AppLogic.GetString("address.cs.5"));

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT CreditCardType FROM CustomerCreditCardType with (NOLOCK) WHERE IsActive = 1"))
                {
                    while (reader.Read())
                    {
                        cardTypes.Add(DB.RSField(reader, "CreditCardType"));
                    }
                }
            }

            ctrlPaymentTerm.CardTypeDataSource = cardTypes;

            ////---------------------------------
            int currentYear = DateTime.Now.Year;

            var startYears = new List<string>();
            var expirationYears = new List<string>();

            startYears.Add(AppLogic.GetString("address.cs.8"));
            expirationYears.Add(AppLogic.GetString("address.cs.8"));
            for (int offsetYear = 0; offsetYear <= 10; offsetYear++)
            {
                startYears.Add((currentYear - offsetYear).ToString());
                expirationYears.Add((currentYear + offsetYear).ToString());
            }

            ctrlPaymentTerm.StartYearDataSource = startYears;
            ctrlPaymentTerm.ExpiryYearDataSource = expirationYears;

            var months = new List<string>();
            months.Add(AppLogic.GetString("address.cs.7"));
            for (int month = 1; month <= 12; month++)
            {
                months.Add(month.ToString().PadLeft(2, '0'));
            }
            ctrlPaymentTerm.StartMonthDataSource = months;
            ctrlPaymentTerm.ExpiryMonthDataSource = months;
        }

        private void AssignPaymentTermValidationPrerequisites()
        {
            ctrlPaymentTerm.PaymentTermRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.10");
            ctrlPaymentTerm.NameOnCardRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.11");
            ctrlPaymentTerm.CardNumberRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.12");
            ctrlPaymentTerm.CVVRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.13");
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.14");
            ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.15");
            ctrlPaymentTerm.StartMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.25");
            ctrlPaymentTerm.StartYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.26");
            ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.16");
            ctrlPaymentTerm.UnknownCardTypeErrorMessage = AppLogic.GetString("checkout1.aspx.17");
            ctrlPaymentTerm.NoCardNumberProvidedErrorMessage = AppLogic.GetString("checkout1.aspx.18");
            ctrlPaymentTerm.CardNumberInvalidFormatErrorMessage = AppLogic.GetString("checkout1.aspx.19");
            ctrlPaymentTerm.CardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.20");
            ctrlPaymentTerm.CardNumberInAppropriateNumberOfDigitsErrorMessage = AppLogic.GetString("checkout1.aspx.21");
            ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.27");
        }

        private void CheckIfWeShouldRequirePayment()
        {
            if (_cart.GetOrderBalance() == System.Decimal.Zero &&
                AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                ctrlPaymentTerm.NoPaymentRequired = true;
                _weShouldRequirePayment = false;
                _cart.MakePaymentTermNotRequired();
            }
            else
            {
                ctrlPaymentTerm.NoPaymentRequired = false;
                _weShouldRequirePayment = true;
            }
        }

        private void AssignPaymentTermErrorSummary()
        {
            ctrlPaymentTerm.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignPaymentTermCaptions()
        {
            var resource = ResourceProvider.GetMobilePaymentTermControlDefaultResources();
            resource.NameOnCardCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.2");
            resource.NoPaymentRequiredCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.15");
            resource.CardNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.3");
            resource.CVVCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.4");
            resource.WhatIsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.11");
            resource.CardTypeCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.5");
            resource.CardStartDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.6");
            resource.ExpirationDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.7");
            resource.CardIssueNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.8");
            resource.CardIssueNumberInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.9");
            resource.SaveCardAsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.10");
            resource.SaveThisCreditCardInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.12");
            resource.PONumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.13");
            ctrlPaymentTerm.LoadStringResources(resource);
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/tooltip.js"));
            manager.Scripts.Add(new ScriptReference("js/creditcard.js"));
            manager.Scripts.Add(new ScriptReference("js/paymentterm_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/checkoutpayment_ajax.js"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = new StringBuilder();

            script.Append("<script type='text/javascript' > \n");

            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");
            script.AppendFormat(String.Format(" ise.Configuration.registerConfig('SagePay.PaymentTerm', '{0}');\n", ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "mobile.checkoutpayment.aspx.14", AppLogic.GetString("mobile.checkoutpayment.aspx.14"));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "mobile.checkoutpayment.aspx.16", AppLogic.GetString("mobile.checkoutpayment.aspx.16"));
            script.Append(" }\n");
            script.Append(");\n");

            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutPayment.setPaymentTermControlId('{0}');\n", this.ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutPayment.setForm('{0}');\n", this.frmCheckOutPayment.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            SectionTitle = AppLogic.GetString("checkoutpayment.aspx.9");
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) return;

            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);
            }

            if (_weShouldRequirePayment)
            {
                if (ctrlPaymentTerm.PaymentTerm.ToString().Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase))
                {
                    ThisCustomer.ThisCustomerSession.SetVal("PONumber", ctrlPaymentTerm.PONumber);
                    if (DisplayClearOtherPaymentOptionsWarning()) { return; }
                }
                else if (ctrlPaymentTerm.PaymentTerm.ToString().Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (DisplayClearOtherPaymentOptionsWarning()) { return; }
                }
                else if (ctrlPaymentTerm.PaymentMethod == DomainConstants.PAYMENT_METHOD_PAYPALX)
                {
                    ThisCustomer.ThisCustomerSession["paypalfrom"] = "checkoutpayment";
                    Response.Redirect(PayPalExpress.CheckoutURL(_cart));
                }
                else if (ctrlPaymentTerm.PaymentTerm == ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm)
                {
                    Response.Redirect(SagePayPayment.SetSagePayServerPaymentRequest(_cart));
                }
                else if (ctrlPaymentTerm.PaymentMethod == DomainConstants.PAYMENT_METHOD_CREDITCARD)
                {
                    //Validate Card Number
                    bool blnCcInvalid = false;
                    string cardNumber;
                    string cardNumberInvalidErrorMessage;

                    var ccValidator = new CreditCardValidator();
                    ccValidator.AcceptedCardTypes = ctrlPaymentTerm.CardType;
                    if (ccValidator.AcceptedCardTypes.Contains("0"))
                    {
                        ctrlPaymentTerm.CardTypeInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.14");
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardTypeInvalidErrorMessage);
                        return;
                    }

                        //See if we should use the card number on file.
                        //We also want to see if the card number starts with an *.
                        //If it doesn't it probably means the user entered a new number.
                        if (ctrlPaymentTerm.CardNumber.StartsWith("*"))
                        {
                            //Get the stored card number.
                            cardNumber = ThisCustomer.PrimaryBillingAddress.CardNumber;
                            cardNumberInvalidErrorMessage = ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage;
                        }
                        else
                        {
                            //Get the card number the user entered.
                            cardNumber = ctrlPaymentTerm.CardNumber;
                            cardNumberInvalidErrorMessage = ctrlPaymentTerm.CardNumberInvalidErrorMessage;
                        }

                    if (!ccValidator.IsValidCardType(cardNumber) || !ccValidator.ValidateCardNumber(cardNumber))
                    {
                        errorSummary.DisplayErrorMessage(cardNumberInvalidErrorMessage);
                        blnCcInvalid = true;
                    }

                    //Validate Expiration Date
                    if (!ccValidator.IsValidExpirationDate(string.Concat(ctrlPaymentTerm.CardExpiryYear, ctrlPaymentTerm.CardExpiryMonth)))
                    {
                        ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.15");
                        ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.16");
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage);
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationYearInvalidErrorMessage);
                        blnCcInvalid = true;
                    }

                    //If an error was found display them
                    if (blnCcInvalid)
                    {
                        return;
                    }

                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    billingAddress.CardNumber = cardNumber;

                    billingAddress.CardName = ctrlPaymentTerm.NameOnCard;
                    billingAddress.CardType = ctrlPaymentTerm.CardType;
                    billingAddress.CardExpirationMonth = ctrlPaymentTerm.CardExpiryMonth;
                    billingAddress.CardExpirationYear = ctrlPaymentTerm.CardExpiryYear;

                    if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                    {
                        //Some CCs do not have StartDate, so here we should provide Default if none was supplied.
                        string defaultCardStartMonth = DateTime.Now.Month.ToString();
                        string defaultCardStartYear = DateTime.Now.Year.ToString();

                        billingAddress.CardStartMonth = CommonLogic.IIF(ctrlPaymentTerm.CardStartMonth != "MONTH", ctrlPaymentTerm.CardStartMonth, defaultCardStartMonth);
                        billingAddress.CardStartYear = CommonLogic.IIF(ctrlPaymentTerm.CardStartYear != "YEAR", ctrlPaymentTerm.CardStartYear, defaultCardStartYear);

                        billingAddress.CardIssueNumber = ctrlPaymentTerm.CardIssueNumber;
                    }

                    AppLogic.StoreCardExtraCodeInSession(ThisCustomer, ctrlPaymentTerm.CVV);

                    //Capture the credit card number from the payment page and encrypt it so that the gateway can capture from that credit card
                    string salt = null;
                    string iv = null;
                    string cardNumberEnc = AppLogic.EncryptCardNumber(cardNumber, ref salt, ref iv);
                    AppLogic.StoreCardNumberInSession(ThisCustomer, cardNumberEnc, salt, iv);

                    Address.Update(ThisCustomer, billingAddress);
                }

                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, ctrlPaymentTerm.PaymentTerm);

            }
            Response.Redirect("checkoutreview.aspx");
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

        private bool DisplayClearOtherPaymentOptionsWarning()
        {
            if (ThisCustomer.ThisCustomerSession[DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS].IsNullOrEmptyTrimmed())
            {
                if (_cart.HasCoupon())
                {
                    errorSummary.DisplayErrorMessage(AppLogic.GetString("checkoutpayment.aspx.53"));
                    ThisCustomer.ThisCustomerSession.SetVal(DomainConstants.CLEAR_OTHER_PAYMENT_OPTIONS, true.ToString());
                    return true;
                }
            }
            return false;
        }
    }   

}
