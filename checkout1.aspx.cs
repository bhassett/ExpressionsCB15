using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

namespace InterpriseSuiteEcommerce
{
    public partial class checkout1 : SkinBase
    {
        #region Declaration

        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        private bool isUsingInterpriseGatewayv2 = false;
        private IEnumerable<PaymentTermDTO> paymentTermOptions = null;
        private bool _isRequirePayment = true;
        private bool _skipCreditCardValidation = false;
        private const string PAYMENT_METHOD_CREDITCARD = DomainConstants.PAYMENT_METHOD_CREDITCARD;
        private int _shippingMethodCount = 0;

        private INavigationService _navigationService = null;
        private ICustomerService _customerService = null;
        private IStringResourceService _stringResourceService = null;
        private IAppConfigService _appConfigService = null;
        private IShoppingCartService _shoppingCartService = null;
        private ICurrencyService _currencyService = null;
        private IAuthenticationService _authenticationService = null;
        private bool _IsPayPal = false;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            Initialize();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            DoPassingOfValueWhenTokenizedCreditCard();
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }

            base.OnUnload(e);
        }

        public override void Validate()
        {
            base.Validate();
        }

        #endregion

        #region Methods

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _currencyService = ServiceFactory.GetInstance<ICurrencyService>();
        }

        private void Initialize()
        {
            InitializeDomainServices();
            RegisterAjaxScript();
            PageNoCache();

            RequireSecurePage();
            RequireCustomerRecord();

            InitializeShoppingCart();
            PerformPageAccessLogic();

            ctrlScript.ShowMaxMind = AppLogic.AppConfigBool("MaxMind.Enabled");
            ctrlShippingMethod.ThisCustomer = ThisCustomer;
            ctrlPaymentTerm.ThisCustomer = ThisCustomer;

            _IsPayPal = (CommonLogic.QueryStringCanBeDangerousContent("PayPal") == bool.TrueString && CommonLogic.QueryStringCanBeDangerousContent("token") != null);

            CheckWhetherToRequireShipping();
            InitializeShippingMethodControl();

            isUsingInterpriseGatewayv2 = AppLogic.IsUsingInterpriseGatewayv2();
            InitializePaymentTermControl(IsCreditCardTokenizationEnabled);
            InitializeOtherPaymentOptionControl();
            
            if (IsCreditCardTokenizationEnabled)
            {
                CreditCardOptionsRenderer();
            }

            if (ThisCustomer.IsRegistered)
            {
                pnlSignIn.Visible = false;
                ShippingAddressGridRenderer();
            }

            if (_cart.CartItems.Count > 0)
            {
                CheckIfWeShouldRequirePayment();
                DisplaySelectedShippingMethod();
            }

            InitProfileControl();
            InitAddressControl();
            DisplayErrorMessageIfAny();

            string couponCode = (ThisCustomer.CouponCode.IsNullOrEmptyTrimmed()) ? String.Empty : ThisCustomer.CouponCode;
            OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, "page.checkout1ordersummary.xml.config", couponCode));
            OnePageCheckoutHelpfulTips.SetContext = this;

            //Clear the key since user already processing it's shipping
            _customerService.ClearAddressCheckingKey();

        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutshippingmulti2_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/tooltip.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/creditcard.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/paymentterm_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("~/jscripts/checkout1_ajax.js"));
            manager.LoadScriptsBeforeUI = false;
            
            var service = new ServiceReference("~/actionservice.asmx");
            service.InlineScript = false;
            manager.Services.Add(service);
        }

        private void RegisterAjaxScript()
        {
            var script = new StringBuilder();

            script.Append("<script type='text/javascript'>\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.Checkout1.setPaymentTermControlId('{0}');\n", ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.Checkout1.setForm('{0}');\n", OnePageCheckout.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
            SectionTitle = _stringResourceService.GetString("checkout1.aspx.83", true);
        }

        private void InitializeShoppingCart()
        {
            _cart = _shoppingCartService.New(CartTypeEnum.ShoppingCart, true);
            bool computeVat = _appConfigService.VATIsEnabled;

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(_stringResourceService.GetString("shoppingcart.aspx.1"));
            }

            if (_cart.CartItems.Count > 0)
            {
                try
                {
                    _cart.BuildSalesOrderDetails(true);
                    _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping(ThisCustomer.CouponCode);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message == _stringResourceService.GetString("shoppingcart.cs.35"))
                    {
                        _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");

                    }
                    else { throw ex; }
                }
                catch (Exception ex) { throw ex; }
            }
            else 
            {
                _navigationService.NavigateToShoppingCart();
            }

            if (_cart.HasRegistryItems())
            {
                _navigationService.NavigateToCheckOutPayment();
            }

            string couponCode = String.Empty;
            string error = String.Empty;

            bool hasCoupon = _cart.HasCoupon(ref couponCode);
            if (hasCoupon && _cart.IsCouponValid(ThisCustomer, couponCode, ref error))
            {
                pnlCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                pnlCoupon.Visible = false;
                if (!error.IsNullOrEmptyTrimmed())
                {
                    _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
            }
        }

        private void InitializeShippingMethodControl()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents())
            {
                ctrlShippingMethod.SkipShipping = true;
                pnlShippingMethod.Visible = false;
            }
            else
            {

                if (_cartHasCouponAndIncludesFreeShipping)
                {
                    ctrlShippingMethod.Visible = false;
                }
                else
                {
                    if (AppLogic.EnableAdvancedFreightRateCalculation() && !(_cart.HasHazardousItem() && AppLogic.ApplyHazardousShipping()))
                    {
                        pnlShippingMethod.Visible = false;
                        InitializeCartRepeaterControl();
                    }
                    else
                    {
                        InitializeShippingMethodControlValues();
                        InitializeShippingMethodCaptions();
                    }
                }
            }
        }

        private void InitializeCartRepeaterControl()
        {
            rptCartItems.ItemDataBound += rptCartItems_ItemDataBound;
            InitializeDataSource();
        }

        private List<InterpriseShoppingCart> GetDataSource()
        {
            return _cart.SplitIntoMultipleOrdersBySpecificType();
        }
        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetDataSource();
            rptCartItems.DataBind();
        }


        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is InterpriseShoppingCart)
                {
                    var cart = e.Item.DataItem as InterpriseShoppingCart;
                    foreach (CartItem item in cart.CartItems)
                    {
                        cart = ServiceFactory.GetInstance<IShoppingCartService>()
                                     .New(CartTypeEnum.ShoppingCart, string.Empty, false, true, string.Empty, string.Empty, item.ItemSpecificType, true);
                    }
                    cart.BuildSalesOrderDetails(true);
                    foreach (CartItem item in cart.CartItems)
                    {
                        if (!item.Priority)
                        {
                            var itemContainer = e.Item.FindByParse<Panel>("pnlItemContainer");
                            itemContainer.Controls.Add(new Label() { Text = item.ItemDescription });
                            itemContainer.Controls.Add(new Literal() { Text = " " });
                            itemContainer.Controls.Add(new Label() { Text = AppLogic.GetString("shoppingcart.cs.25") });
                            itemContainer.Controls.Add(new Literal() { Text = " : " });
                            itemContainer.Controls.Add(new Label() { Text = Localization.ParseLocaleDecimal(item.m_Quantity, ThisCustomer.LocaleSetting) });
                            itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                            itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                        }
                    }

                    var lblOptionName = e.Item.FindByParse<Label>("lblOptionName");
                    lblOptionName.Text = cart.OptionName;
                    var mainShipMethodContainer = e.Item.FindByParse<Panel>("divShippingInfo");
                    var lblShipmethodHeader = e.Item.FindByParse<Label>("lblShipmethodHeader");

                    if (!cart.HasShippableComponents())
                    {
                        lblShipmethodHeader.Text = AppLogic.GetString("checkoutshippingmult.aspx.7");
                        lblShipmethodHeader.CssClass = "notificationtext";
                        mainShipMethodContainer.Visible = false;
                    }
                    else
                    {
                        var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, cart.FirstItem().m_ShippingAddressID, cart.FirstItem().GiftRegistryID);
                        var ctrlShippingMethod = e.Item.FindByParse<UserControls_ShippingMethodControl>("ctrlShippingMethod");

                        ctrlShippingMethod.ShippingAddressID = shippingAddress.AddressID;
                        //Set these properties to disable the instore pickup
                        ctrlShippingMethod.HideInStorePickUpShippingOption = true;
                        ctrlShippingMethod.HidePickupStoreLink = true;
                        ctrlShippingMethod.ItemSpecificType = ((cart.CartItems[0].ItemSpecificType.IsNullOrEmpty()) ? "" : cart.CartItems[0].ItemSpecificType);
                        ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
                        ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9");

                        CustomCartItem CustomItem = new CustomCartItem();
                        CustomItem.Counter = cart.CartItems[0].m_ShoppingCartRecordID;
                        CustomItem.IsDownload = cart.CartItems[0].IsDownload;
                        CustomItem.ItemCode = cart.CartItems[0].ItemCode;
                        CustomItem.UnitMeassureCode = cart.CartItems[0].UnitMeasureCode;
                        CustomItem.InStoreWarehouseCode = cart.CartItems[0].InStoreWarehouseCode;
                        CustomItem.KitComposition = cart.CartItems[0].GetKitComposition();
                        ctrlShippingMethod.InstoreCartItem = CustomItem;
                        ctrlShippingMethod.InStoreSelectedWareHouseCode = CustomItem.InStoreWarehouseCode;
                        ctrlShippingMethod.ItemSpecificType = cart.CartItems[0].ItemSpecificType;

                        if (_appConfigService.ShippingRatesOnDemand)
                        {
                            ctrlShippingMethod.ShowShowAllRatesButton = true;
                            ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
                        }
                        ctrlShippingMethod.IsMultipleShipping = true;

                        mainShipMethodContainer.Visible = true;

                        var script = new StringBuilder();
                        script.Append("<script type='text/javascript'>\n");
                        script.Append("$(document).ready( function() { \n");
                        script.AppendFormat("ise.Pages.CheckOutShippingMulti2.registerShippingMethodControlId('{0}');\n", ctrlShippingMethod.ClientID);
                        script.Append("});\n");
                        script.Append("</script>\n");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                        _shippingMethodCount++;
                    }
                }
            }
        }

        private void InitializeShippingMethodControlValues()
        {
            ctrlShippingMethod.ShippingAddressID = (_cart.OnlyShippingAddressIsNotCustomerDefault()) ? _cart.FirstItem().m_ShippingAddressID : ThisCustomer.PrimaryShippingAddressID;
            ctrlShippingMethod.HideInStorePickUpShippingOption = true;
            ctrlShippingMethod.HidePickupStoreLink = true;
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.9", true);
            if (_appConfigService.ShippingRatesOnDemand)
            {
                ctrlShippingMethod.ShowShowAllRatesButton = true;
                ctrlShippingMethod.ShowAllRatesButtonText = _stringResourceService.GetString("checkoutshipping.aspx.16", true);
            }
        }

        private void InitializeShippingMethodCaptions()
        {
            if (!_cart.CartAllowsShippingMethodSelection) return;

            if (_cartHasCouponAndIncludesFreeShipping)
            {
                lblSelectShippingMethod.Text = _stringResourceService.GetString("checkout1.aspx.84");
            }
            else
            {
                if (ThisCustomer.IsRegistered && Shipping.MultiShipEnabled() && _cart.TotalQuantity() > 1
                    && _appConfigService.AllowShipToDifferentThanBillTo)
                {
                    lblSelectShippingMethod.Text = _stringResourceService.GetString("checkout1.aspx.86").FormatWith("checkoutshippingmult.aspx");
                }
                else
                {
                    lblSelectShippingMethod.Text = _stringResourceService.GetString("checkout1.aspx.4");
                }
            }
        }

        private void InitializeOtherPaymentOptionControl()
        {
            var customer = _authenticationService.GetCurrentLoggedInCustomer();
            if (customer.IsNotRegistered) { return; }

            // set stringresources...
            ctrlOtherPaymentOption.Header = AppLogic.GetString("checkout1.aspx.98");
            ctrlOtherPaymentOption.HeaderCreditMemo = AppLogic.GetString("checkout1.aspx.99");
            ctrlOtherPaymentOption.HeaderLoyaltyPoints = AppLogic.GetString("checkout1.aspx.100");
            ctrlOtherPaymentOption.HeaderGiftCode = AppLogic.GetString("checkout1.aspx.101");
            ctrlOtherPaymentOption.HeaderBalanceAvailable = AppLogic.GetString("checkout1.aspx.102");
            ctrlOtherPaymentOption.HeaderApplyAmount = AppLogic.GetString("checkout1.aspx.103");
            ctrlOtherPaymentOption.ButtonApplyCaption = AppLogic.GetString("checkout1.aspx.104");
            ctrlOtherPaymentOption.PointsEarnedText = AppLogic.GetString("checkout1.aspx.111");
            ctrlOtherPaymentOption.ButtonAddGiftCodeTooltip = AppLogic.GetString("checkout1.aspx.112");
            ctrlOtherPaymentOption.GiftCodeText = AppLogic.GetString("checkout1.aspx.113");
            ctrlOtherPaymentOption.ValidationGiftCodeEmpty = AppLogic.GetString("checkout1.aspx.114");
            ctrlOtherPaymentOption.ValidationGiftCodeInvalid = AppLogic.GetString("checkout1.aspx.115");
            ctrlOtherPaymentOption.ValidationGiftCodeZeroBalance = AppLogic.GetString("checkout1.aspx.116");
            ctrlOtherPaymentOption.LoaderText = AppLogic.GetString("checkout1.aspx.117");
            ctrlOtherPaymentOption.ButtonSaveGiftCodeTooltip = AppLogic.GetString("checkout1.aspx.118");
            ctrlOtherPaymentOption.ButtonCancelGiftCodeTooltip = AppLogic.GetString("checkout1.aspx.119");
            ctrlOtherPaymentOption.IsCreditMemoEnabled = _appConfigService.CreditRedemptionIsEnabled;
            ctrlOtherPaymentOption.IsLoyaltyPointsEnabled = _appConfigService.LoyaltyPointsEnabled;
            ctrlOtherPaymentOption.IsGiftCodeEnabled = _appConfigService.GiftCodeEnabled;
            ctrlOtherPaymentOption.RedemptionMultiplier = _customerService.GetRedemptionMultiplier();
            ctrlOtherPaymentOption.CurrencySymbol = _currencyService.GetCurrencySymbol();
            lblNewGiftCode.Text = AppLogic.GetString("checkout1.aspx.120");

            if (_appConfigService.CreditRedemptionIsEnabled)
            {
                // get available creditmemos
                var creditMemos = _customerService.GetCustomerCreditMemosWithRemainingBalance().ToList();
                if (creditMemos.Count > 0) { ctrlOtherPaymentOption.CreditMemosJSON = creditMemos.ToJSON(); }

                // get applied creditmemos
                var creditMemosApplied = _shoppingCartService.GetAppliedCreditMemos().ToList();
                if (creditMemosApplied.Count > 0) { ctrlOtherPaymentOption.CreditMemosAppliedJSON = creditMemosApplied.ToJSON(); }
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
                errorSummary.DisplayErrorMessage(AppLogic.GetString("checkout1.aspx.cs.3"));
            }
        }

        private void InitAddressControl()
        {
            string cityText = _stringResourceService.GetString("checkout1.aspx.71");
            string streetText = _stringResourceService.GetString("checkout1.aspx.62");
            string stateText = _stringResourceService.GetString("checkout1.aspx.72");
            string postalText = _stringResourceService.GetString("checkout1.aspx.70");
            string enterPostalText = _stringResourceService.GetString("checkout1.aspx.76");
            string countyText = _stringResourceService.GetString("checkout1.aspx.77");

            BillingAddressControl.LabelStreetText = streetText;
            BillingAddressControl.LabelCityText = cityText;
            BillingAddressControl.LabelStateText = stateText;
            BillingAddressControl.LabelPostalText = postalText;
            BillingAddressControl.LabelEnterPostalText = enterPostalText;
            BillingAddressControl.LabelCountyText = countyText;
            BillingAddressControl.IsShowBusinessTypesSelector = false;
            BillingAddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
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
            ShippingAddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            ShippingAddressControl.DefaultAddressType = _stringResourceService.GetString("checkout1.aspx.87");
            ShippingAddressControl.BindData();
        }

        private void InitProfileControl()
        {
            ProfileControl.AccountType = Interprise.Framework.Base.Shared.Const.CUSTOMER;
            ProfileControl.IsUseFullnameTextbox = true;
            ProfileControl.IsExcludeAccountName = true;
            ProfileControl.HideDefaulEmailInput = true;

            ProfileControl.LabelShippingContactNameText = _stringResourceService.GetString("checkout1.aspx.79");
            ProfileControl.LabelShippingContactNumberText = _stringResourceService.GetString("checkout1.aspx.66");
            ProfileControl.LabeShippingEmailText = _stringResourceService.GetString("checkout1.aspx.65");
            ProfileControl.BindData();
        }

        private void InitializePaymentTermControl(bool isTokenization)
        {
            AssignCardTypeDataSources();

            bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());
            ctrlPaymentTerm.ShowPaypalPaymentOption = hidePaypalOptionIfMultiShipAndHasGiftRegistry;

            paymentTermOptions = PaymentTermDTO.GetAllForGroup(ThisCustomer.ContactCode, ThisCustomer.PrimaryShippingAddress); //availableTerms;
            if (ServiceFactory.GetInstance<IAppConfigService>().AllowMultipleShippingAddressPerOrder || _cart.HasRegistryItems())
            {
                paymentTermOptions = ServiceFactory.GetInstance<IPaymentTermService>().GetPaymentTermOptionsWithoutSagePay(paymentTermOptions);
            }
            ctrlPaymentTerm.PaymentTermOptions = paymentTermOptions;

            if (paymentTermOptions.Count() > 0) {
                var paymentTermOptionSelected = paymentTermOptions.FirstOrDefault(item => item.IsSelected);
                if (paymentTermOptionSelected != null)
                {
                    ctrlPaymentTerm.PaymentMethod = paymentTermOptionSelected.PaymentMethod;
                }

                ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");
                ctrlPaymentTerm.IsInOnePageCheckOut = _appConfigService.CheckoutUseOnePageCheckout;

                if (isTokenization)
                {
                    ctrlPaymentTerm.IsTokenization = true;
                    ctrlPaymentTerm.IsInOnePageCheckOut = true;
                }

                AssignPaymentTermDatasources();
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
                pnlPaymentSectionWrapper.Visible = false;
                litPaymentsMethod.Visible = false;
                litTransactionStatusMessage.Text = _stringResourceService.GetString("checkout1.aspx.107");
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

            ctrlPaymentTerm.NameOnCard = ThisCustomer.PrimaryBillingAddress.CardName;
            ctrlPaymentTerm.EnabledCreditCardValidation = _appConfigService.ValidateCardEnabled;
        }

        private void AssignPaymentTermCaptions()
        {
            int customerSkinID = Customer.Current.SkinID;
            string customerLocaleSetting = Customer.Current.LocaleSetting;

            var resource = ResourceProvider.GetPaymentTermControlDefaultResources();
            resource.NameOnCardCaption = _stringResourceService.GetString("checkout1.aspx.50");
            resource.NoPaymentRequiredCaption = _stringResourceService.GetString("checkout1.aspx.58");
            resource.CardNumberCaption = _stringResourceService.GetString("checkout1.aspx.51");
            resource.CVVCaption = _stringResourceService.GetString("checkout1.aspx.52");
            resource.WhatIsCaption = _stringResourceService.GetString("checkout1.aspx.55");
            resource.CardTypeCaption = _stringResourceService.GetString("checkout1.aspx.53");
            resource.CardStartDateCaption = _stringResourceService.GetString("checkout1.aspx.22");
            resource.ExpirationDateCaption = _stringResourceService.GetString("checkout1.aspx.54");
            resource.CardIssueNumberCaption = _stringResourceService.GetString("checkout1.aspx.23");
            resource.CardIssueNumberInfoCaption = _stringResourceService.GetString("checkout1.aspx.24");
            resource.SaveCardAsCaption = _stringResourceService.GetString("checkout1.aspx.56");
            resource.SaveThisCreditCardInfoCaption = _stringResourceService.GetString("checkout1.aspx.57");
            resource.PONumberCaption = _stringResourceService.GetString("checkout1.aspx.59");
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
            ctrlPaymentTerm.CVVRequiredErrorMessage = _stringResourceService.GetString("checkout1.aspx.13", true);
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = _stringResourceService.GetString("checkout1.aspx.14", true);
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
                Topic t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                string resouce1 = AppLogic.GetString("checkout1.aspx.85");

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

            if (ThisCustomer.IsRegistered)
            {

                if (!this.IsPostBack)
                {
                    AppLogic.GenerateCreditCardCodeSaltIV(ThisCustomer);
                }

                var creditCards = CreditCardDTO.GetCreditCards(ThisCustomer.CustomerCode);

                StringBuilder _CreditOptionsHTML = new StringBuilder();
                string _default = "---";

                _CreditOptionsHTML.Append("<span class='strong-font'>Saved Credit Card Info</span>");
                _CreditOptionsHTML.Append("<div class='clear-both height-12'></div>");


                _CreditOptionsHTML.Append("<div id='credit-card-options-wrapper'>");

                _CreditOptionsHTML.Append("<div id='credit-card-options-header-wrapper'>");

                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", String.Empty);
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.67"));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.61"));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.62"));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left custom-font-style'>{0}</div>", String.Empty);

                _CreditOptionsHTML.Append("</div>");

                _CreditOptionsHTML.Append("<div class='clear-both'></div>");

                int counter = 1;
                string thisOption = string.Empty;
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

                    string description = string.Empty;

                    if (credit.RefNo > 0)
                    {
                        description = credit.Description;
                    }

                    _CreditOptionsHTML.Append("<div class='opc-credit-card-options-row'>");

                    _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left'>{0}</div>", creditOption);
                    _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left'>{0}{1}</div>", credit.NameOnCard, string.Format("<div class='clr5'></div><div id='{1}-credit-card-description'>{0}</div>", description, counter));

                    if (credit.RefNo > 0)
                    {

                        string lastFour = string.Empty;

                        if (credit.CardNumber.Length > 0)
                        {
                            lastFour = credit.CardNumber.Substring(credit.CardNumber.Length - 4);
                            lastFour = string.Format("&nbsp;<span class=\"MaskNumber\">ending in {0}</span>", lastFour);
                        }

                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left' id='{2}-credit-card-type'>{0} {1}</div>", credit.CardType, lastFour, counter);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'  id='{1}-credit-card-expiry'>{0}</div>", string.Format("{0}/{1}", credit.ExpMonth, credit.ExpYear), counter);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left' id='{1}-credit-card-clear'>{0}</div>", string.Format("<a class='opc-clearcard' id='opc::{0}::{1}' href='javascript:void(1);'>Clear</a>", creditCardCode, counter), counter);

                    }
                    else
                    {

                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left'>{0}</div>", _default);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'>{0}</div>", _default);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left'>{0}</div>", _default);
                    }


                    _CreditOptionsHTML.Append("</div>");

                    _CreditOptionsHTML.Append("<div class='clear-both'></div>");

                    counter++;

                }

                _CreditOptionsHTML.Append("</div>");
                _CreditOptionsHTML.Append("<div class='clear-both height-12'></div>");

                LtrCreditCardOptionsRenderer.Text = _CreditOptionsHTML.ToString();

            }
        }

        private void ShippingAddressGridRenderer()
        {

            var con = DB.NewSqlConnection();

            try
            {
                var gridLayout = new StringBuilder();
                con.Open();

                int counter = 1;
                string thisOption = String.Empty;

                string sql = String.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), 2, DB.SQuote(ThisCustomer.ContactCode));

                var reader = DB.GetRSFormat(con, sql);

                gridLayout.Append("<div id='billing-address-options-wrapper'>");
                gridLayout.Append("<div id='credit-card-options-header-wrapper'>");
                gridLayout.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", String.Empty);
                gridLayout.AppendFormat("<div class='option-billing-account-name-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.67"));
                gridLayout.AppendFormat("<div class='option-billing-country-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.61"));
                gridLayout.AppendFormat("<div class='option-billing-address-place-holder float-left custom-font-style'>{0}</div>", _stringResourceService.GetString("checkout1.aspx.62"));
                gridLayout.Append("</div>");
                gridLayout.Append("<div class='clear-both'></div>");

                while (reader.Read())
                {
                    thisOption = (int)reader["PrimaryAddress"] == 1? "checked": String.Empty;

                    string creditOption = String.Empty;
                    string contactName = String.Empty;
                    string address = String.Empty;

                    creditOption = String.Format("<input type='radio' id='{2}' {1} name='multiple-shipping-address' value = '{0}'/>", reader["AddressID"], thisOption, counter);
                    gridLayout.Append("<div class='multiple-address-options-row'>");
                    gridLayout.AppendFormat("<div class='multiple-address-options-control-column float-left'>{0}</div>", creditOption);
                    gridLayout.AppendFormat("<div class='multiple-address-options-account-name-column float-left custom-font-style'>{0}</div>", reader["Name"].ToString());
                    gridLayout.AppendFormat("<div class='multiple-address-options-country-column float-left custom-font-style'>{0}</div>", reader["Country"].ToString());
                    gridLayout.AppendFormat("<div class='multiple-address-options-street-colum float-left custom-font-style'>{0}</div>", reader["CityStateZip"].ToString());
                    gridLayout.Append("</div>");
                    gridLayout.Append("<div class='clear-both'></div>");

                    counter++;
                }

                gridLayout.Append("</div>");
                gridLayout.Append("<div class='clear-both height-12'></div>");
                litShippingAddressGrid.Text = gridLayout.ToString();

                if ((counter - 1) > 1)
                {
                    litShippingAddressGrid.Visible = true;
                }
                else
                {
                    litShippingAddressGrid.Visible = false;
                }

            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

        }

        protected bool IsCreditCardTokenizationEnabled
        {
            get { return isUsingInterpriseGatewayv2 && ThisCustomer.IsRegistered && AppLogic.AppConfigBool("AllowCreditCardInfoSaving"); }
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
                if (AppLogic.AppConfigBool("ShowGatewayError"))
                {
                    errorMessage = ParseGatewayErrorMessage(ThisCustomer.LastGatewayErrorMessage);
                }
                else
                {
                    errorMessage = _stringResourceService.GetString("checkoutpayment.aspx.cs.1");
                }
            }

            errorSummary.DisplayErrorMessage(errorMessage);
        }

        protected void btnDoProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessPayment();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected void ProcessPayment()
        {
            if (!_cart.IsEmpty())
            {
                _shoppingCartService.CheckStockAvailabilityDuringCheckout(_cart.HasNoStockPhasedOutItem, _cart.HaNoStockAndNoOpenPOItem);

                //check discountinued
                var discontinuedItems = _cart.CartItems.Where(c => c.Status.ToLowerInvariant() == "D".ToLowerInvariant())
                                            .Select(itm => itm.m_ShoppingCartRecordID)
                                            .AsParallel().ToList();
                if (discontinuedItems.Count > 0)
                {
                    discontinuedItems.ForEach(recId => { _cart.RemoveItem(recId); });
                    _navigationService.NavigateToShoppingCartWitErroMessage(AppLogic.GetString("checkoutpayment.aspx.cs.4").ToUrlEncode());
                }
            }
                       

            if (!_isRequirePayment) { 

                _navigationService.NavigateToCheckoutReview();
            
            }

            bool isCustomerRegistered = Customer.Current.IsRegistered;
            bool isCreditCardTokenizationEnabled = IsCreditCardTokenizationEnabled;

            string paymentMethodFromInput = ctrlPaymentTerm.PaymentMethod;
            string paymentTermCodeFromInput = ctrlPaymentTerm.PaymentTerm;

            #region Save Billing Address

            var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);
            var aShippingAddress = ThisCustomer.PrimaryShippingAddress;

            string email = ThisCustomer.IsRegistered ? ThisCustomer.EMail : aShippingAddress.EMail.IsNullOrEmptyTrimmed() ? ThisCustomer.EMail : aShippingAddress.EMail;

            ThisCustomer.EMail = email;
            aBillingAddress.EMail = email;

            string PAYMENT_METHOD_PAYPALX = DomainConstants.PAYMENT_METHOD_PAYPALX;

            if (!copyShippingInfo.Checked)
            {
                aBillingAddress.Address1 = BillingAddressControl.Street;
                aBillingAddress.Country = BillingAddressControl.Country;
                aBillingAddress.PostalCode = BillingAddressControl.Postal;

                string bCityStates = txtCityStates.Text;
                string city = String.Empty;
                string state = String.Empty;

                var cityStateArray = GetCityStateArray();
                aBillingAddress.State = InterpriseHelper.IsWithState(BillingAddressControl.Country) ? cityStateArray[0] : String.Empty;
                aBillingAddress.City = cityStateArray[1];

                aBillingAddress.ResidenceType = aShippingAddress.ThisCustomer.PrimaryShippingAddress.ResidenceType;
                aBillingAddress.Name = txtBillingContactName.Text;
                aBillingAddress.Phone = txtBillingContactNumber.Text;
           
                if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD) aBillingAddress.CardName = ctrlPaymentTerm.NameOnCard;
                if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = BillingAddressControl.County; }
            }
            else
            {
                aBillingAddress.Address1 = aShippingAddress.Address1;
                aBillingAddress.Country = aShippingAddress.Country;
                aBillingAddress.PostalCode = (aShippingAddress.Plus4.IsNullOrEmptyTrimmed()) ? aShippingAddress.PostalCode : String.Format("{0}-{1}", aShippingAddress.PostalCode, aShippingAddress.Plus4);
                aBillingAddress.City = aShippingAddress.City;
                aBillingAddress.State = aShippingAddress.State;
                aBillingAddress.ResidenceType = aShippingAddress.ResidenceType;
                aBillingAddress.Name = aShippingAddress.Name;
                aBillingAddress.Phone = aShippingAddress.Phone;

                if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD) aBillingAddress.CardName = aShippingAddress.Name;

                if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = aShippingAddress.County; }
            }

            AppLogic.SavePostalCode(aBillingAddress);
            UpdateAnonForAge13();

            #endregion

            //Save Anonymous Customer Email Address in Sales Order Note
            ServiceFactory.GetInstance<ICustomerService>()
                          .AssignAnonymousCustomerEmailAddressInSalesOrderNote();

            //Clear the cart warehouse code since user cancel the instore pickup
            ServiceFactory.GetInstance<IShoppingCartService>()
                          .ClearCartWarehouseCodeByCustomer();

            #region Payments

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
                if (ThisCustomer.IsNotRegistered)
                {
                    //for the address information
                    aBillingAddress.Save();
                    ThisCustomer.PrimaryBillingAddress = aBillingAddress;
                }

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "checkoutpayment";
                _cart.BuildSalesOrderDetails(ThisCustomer.CouponCode);
                _navigationService.NavigateToUrl(PayPalExpress.CheckoutURL(_cart));

            }
            else if (paymentTermCodeFromInput == ServiceFactory.GetInstance<IAppConfigService>().SagePayPaymentTerm)
            {
                if (ThisCustomer.IsNotRegistered)
                {
                    aBillingAddress.Save();
                    ThisCustomer.PrimaryBillingAddress = aBillingAddress;
                }
                _cart.BuildSalesOrderDetails(ThisCustomer.CouponCode);
                Response.Redirect(SagePayPayment.SetSagePayServerPaymentRequest(_cart));
            }
            else if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD)
            {
                //Validate first the inputs
                //triggers the input registered validators.

                if (!IsValid) return;

                if (!_skipCreditCardValidation)
                {
                    //credit card validation
                    if (!IsValidCreditCardInfo()) return;
                }

                #region Posted Data (Credit Card Information)

                string nameOnCard = ctrlPaymentTerm.NameOnCard;
                string cardNumberFromInput = ctrlPaymentTerm.CardNumber;
                string cardTypeFromInput = ctrlPaymentTerm.CardType;
                string cardExpiryYearFromInput = ctrlPaymentTerm.CardExpiryYear;
                string cardExpiryMonthFromInput = ctrlPaymentTerm.CardExpiryMonth;
                string cVVFromInput = ctrlPaymentTerm.CVV;
                string saveCreditCardAsFromInput = ctrlPaymentTerm.CardDescription;

                string cardStartMonth = String.Empty;
                string cardStartYear = String.Empty;
                string cardIssueNumber = String.Empty;

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    cardStartMonth = ctrlPaymentTerm.CardStartMonth;
                    cardStartYear = ctrlPaymentTerm.CardStartYear;
                    cardIssueNumber = ctrlPaymentTerm.CardIssueNumber;
                }

                #endregion

                #region Credit Card Info

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

                    if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = BillingAddressControl.County; }

                }

                if (isCreditCardTokenizationEnabled)
                {
                    bool saveCreditCardInfo = (AppLogic.AppConfigBool("ForceCreditCardInfoSaving") || ctrlPaymentTerm.SaveCreditCreditCardInfo);
                    ThisCustomer.ThisCustomerSession["SaveCreditCardChecked"] = saveCreditCardInfo.ToString();
                    if (saveCreditCardInfo) aBillingAddress.CardDescription = saveCreditCardAsFromInput;
                }

                aBillingAddress.AddressID = creditCardCode;
                aBillingAddress.CardNumber = cardNumberFromInput;
                aBillingAddress.CardType = cardTypeFromInput;
                aBillingAddress.CardExpirationMonth = cardExpiryMonthFromInput;
                aBillingAddress.CardExpirationYear = cardExpiryYearFromInput;
                aBillingAddress.CustomerCode = ThisCustomer.CustomerCode;
                aBillingAddress.Save();
                _customerService.UpdateCustomerBillTo(aBillingAddress, true);

                #endregion

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
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

                if (isCreditCardTokenizationEnabled)
                {
                    ServiceFactory.GetInstance<ICustomerService>()
                                  .MakeDefaultAddress(creditCardCode, AddressTypes.Billing, true);

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

                    if (plus4 == 0)
                    {
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4=NULL, ", postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote());
                    }
                    else
                    {
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4={2}, ", postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote(), plus4);
                    }

                    sql.AppendFormat(" ExpMonth={0}, ExpYear={1}, Telephone={2}, ", InterpriseHelper.ToInterpriseExpMonth(aBillingAddress.CardExpirationMonth).ToDbQuote(), aBillingAddress.CardExpirationYear.ToDbQuote(), aBillingAddress.Phone.ToDbQuote());
                    sql.AppendFormat(" CreditCardType = {0}, DateModified=getdate() ", aBillingAddress.CardType.ToDbQuote());

                    sql.AppendFormat(" WHERE CreditCardCode={0} ", creditCardCode.ToDbQuote());

                    DB.ExecuteSQL(sql.ToString());
                    sql.Clear();

                    #endregion

                    DB.ExecuteSQL(@"UPDATE Customer SET Creditcardcode={0} WHERE CustomerCode={1}", DB.SQuote(creditCardCode), DB.SQuote(ThisCustomer.CustomerCode));

                    AppLogic.ClearCreditCardCodeInSession(ThisCustomer);

                }

                AppLogic.StoreCardExtraCodeInSession(ThisCustomer, cVVFromInput);
            }
            else
            {
                //handling of non credit card, Paypal, REQUEST QUOTE, PONumber when tokenization is enabled

                if (isCreditCardTokenizationEnabled && !txtCode.Text.IsNullOrEmptyTrimmed())
                {
                    //txtCode.Text - Customer CreditCard code
                    //Override the credit card code if tokenization
                    //decrypt the credit card code from the rendered hidden text box since it is encrypted.

                    var primariBillingAddress = ThisCustomer.PrimaryBillingAddress;
                    primariBillingAddress.Address1 = BillingAddressControl.Street;
                    primariBillingAddress.Country = BillingAddressControl.Country;
                    primariBillingAddress.PostalCode = BillingAddressControl.Postal;

                    var cityStateArray = GetCityStateArray();
                    primariBillingAddress.State = cityStateArray[0];
                    primariBillingAddress.City = cityStateArray[1];
                    primariBillingAddress.Phone = txtBillingContactNumber.Text;
                    primariBillingAddress.Name = txtBillingContactName.Text.Trim();
                    primariBillingAddress.CardName = txtBillingContactName.Text.Trim();

                    if (AppLogic.AppConfigBool("Address.ShowCounty")) { primariBillingAddress.County = BillingAddressControl.County; }

                    _customerService.UpdateCustomerBillTo(primariBillingAddress, true);
                }
                else
                {
                    aBillingAddress.Save();
                    _customerService.UpdateCustomerBillTo(aBillingAddress, true);
                }

            }

            #endregion

            #region Redirect to Confirmation Page or Do Place Order

            RedirectToConfirmationPage(paymentTermCodeFromInput, aBillingAddress, aShippingAddress);

            #endregion
        }

        private void PerformPageAccessLogic()
        {
            _customerService.DoIsCreditOnHoldChecking();
            _customerService.DoIsOver13Checking(true);

            if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed())
            {
                _navigationService.NavigateToUrl("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
            }

            if (_cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (_cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(_stringResourceService.GetString("shoppingcart.aspx.1", true));
            }

            if (ThisCustomer.IsNotRegistered && !_appConfigService.PasswordIsOptionalDuringCheckout)
            {
                _navigationService.NavigateToCheckoutAnon(true);
            }

            if (_cart.HasOverSizedItemWithPickupShippingMethod() || _cart.HasPickupItem())
            {
                _navigationService.NavigateToCheckOutStore();
            }

            if (!_appConfigService.CheckoutUseOnePageCheckout)
            {
                _navigationService.NavigateToCheckoutShipping();
            }

            string couponCode = String.Empty;
            string couponErrorMessage = String.Empty;

            if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                _navigationService.NavigateToUrl("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!_cart.IsNoShippingRequired() && (_cart.HasMultipleShippingAddresses()) && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.NumItems() > 1)
            {
                _navigationService.NavigateToCheckoutMult();
            }

            if (_cart.HasGiftItems() && _cart.IsGiftEmailNotSet())
            {
                _navigationService.NavigateToCheckoutGiftEmail();
            }
        }

        protected void RedirectToConfirmationPage(string paymentTerm, Address billing, Address shipping)
        {

            InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, paymentTerm);
            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"))
            {
                _navigationService.NavigateToCheckoutReview();
            }
            string salesOrderCode = string.Empty;
            string receiptCode = string.Empty;
            Gateway gatewayToUse = null;

            if (!_cart.IsSalesOrderDetailBuilt)
            {
                _cart.BuildSalesOrderDetails(false);
            }

            string status = _cart.PlaceOrder(gatewayToUse, billing, shipping, ref salesOrderCode, ref receiptCode, true, true, false);

            if (status == AppLogic.ro_3DSecure)
            {
                // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                _navigationService.NavigateToSecureForm();

            }
            if (status == AppLogic.ro_OK)
            {
                ThisCustomer.ClearTransactions(true);

                string PM = AppLogic.CleanPaymentMethod(ThisCustomer.PaymentMethod);
                bool multipleAttachment = false;
                if (salesOrderCode.IndexOf(',') != -1)
                {
                    multipleAttachment = true;
                }

                //Send mail.
                foreach (string salesOrderToEmail in salesOrderCode.Split(','))
                {
                    if (ThisCustomer.PaymentTermCode.ToUpper() != "REQUEST QUOTE" && ThisCustomer.PaymentTermCode.ToUpper() != "PURCHASE ORDER")
                    {
                        AppLogic.SendOrderEMail(ThisCustomer, _cart, salesOrderToEmail, false, PM, true, multipleAttachment);
                    }
                    else
                    {
                        //This will only send email to admin.
                        AppLogic.SendOrderEMail(ThisCustomer, _cart, salesOrderToEmail, false, PM, multipleAttachment);
                    }
                }
                
                _navigationService.NavigateToOrderConfirmation(salesOrderCode);

            }
            else
            {
                ThisCustomer.IncrementFailedTransactionCount();
                if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                {

                    _cart.ClearTransaction();
                    ThisCustomer.ResetFailedTransactionCount();
                    _navigationService.NavigateToOrderFailed();

                }

                if (status == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
                {
                    _navigationService.NavigateToUrl("checkout1.aspx?paymentterm={0}&errormsg={1}".FormatWith(ThisCustomer.PaymentTermCode, status.ToUrlEncode()));
                }

                ThisCustomer.ClearTransactions(false);
                errorSummary.DisplayErrorMessage(status);

            }

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

        private void UpdateAnonForAge13()
        {
            if (ThisCustomer.IsRegistered) return;

            int isupdated = 1;
            string updateAnonRecordIfIsover13checked = string.Format("UPDATE EcommerceCustomer SET IsOver13 = 1, IsUpdated = {0} WHERE CustomerID = {1}", DB.SQuote(isupdated.ToString()), DB.SQuote(ThisCustomer.CustomerID.ToString()));
            DB.ExecuteSQL(updateAnonRecordIfIsover13checked);
            ThisCustomer.Update();
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

        private void CheckWhetherToRequireShipping()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents() || _cart.CouponIncludesFreeShipping(litCouponEntered.Text))
            {
                _cart.MakeShippingNotRequired();

                if (_IsPayPal)
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                    _navigationService.NavigateToUrl("checkoutreview.aspx?PayPal=True&token={0}".FormatWith(Request.QueryString["token"]));
                }
            }
        }

        private string ParseGatewayErrorMessage(string gatewayMessage)
        {
            string returnMessage = String.Empty;

            if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_DIGIT) != -1 ||
                gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_FORMAT) != -1 ||
                gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_FORMAT2) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.20", true);
            }
            else if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_DATE_CHECK_FAILED_EXPIRED) != -1 ||
                     gatewayMessage.IndexOf(GatewayErrorCodes.CARD_DATE_CHECK_FAILED_INVALID) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.cs.2", true);
            }
            else if (gatewayMessage.IndexOf(GatewayErrorCodes.CARD_CHECK_FAILED_TYPE) != -1)
            {
                returnMessage = AppLogic.GetString("checkout1.aspx.14", true);
            }
            else
            {
                returnMessage = gatewayMessage;
            }

            return returnMessage;
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

        private void DisplaySelectedShippingMethod()
        {
            string selectedShippingMethod = (_cart.IsSalesOrderDetailBuilt) ? _cart.SalesOrderDataset.CustomerSalesOrderView[0].ShippingMethod : _cart.GetCartShippingMethodSelected();
            string freightRate = "";
            decimal freight = Decimal.Zero;

            if (AppLogic.AppConfigBool("ShowTaxBreakDown") && !selectedShippingMethod.IsNullOrEmptyTrimmed())
            {
                freight = Decimal.Zero;
                decimal freightTax = Decimal.Zero;

                if (!_cart.IsSalesOrderDetailBuilt)
                {
                    freight = _cart.GetCartFreightRate();
                    freightTax = _cart.GetCartFreightRateTax(_cart.ThisCustomer.CurrencyCode, freight, ThisCustomer.FreightTaxCode, ThisCustomer.PrimaryShippingAddress);
                }
                else
                {
                    freight = _cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightRate;
                    freightTax = _cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightTaxRate;
                }
          
                if (_cart.ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive)
                {
                    freight += freightTax;
                }

                freightRate = (freight == Decimal.Zero) ? AppLogic.GetString("shoppingcart.aspx.13") : freight.ToCustomerCurrency();
                litSelectedShippingMethod.Text = "{0} {1}".FormatWith(selectedShippingMethod, freightRate);
            }
            var splittedCart = _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
            foreach (InterpriseShoppingCart cartList in splittedCart)
            {
                selectedShippingMethod = (_cart.IsSalesOrderDetailBuilt) ? cartList.SalesOrderDataset.CustomerSalesOrderView[0].ShippingMethod : cartList.GetCartShippingMethodSelected();
                freightRate = (freight == Decimal.Zero) ? AppLogic.GetString("shoppingcart.aspx.13") : cartList.GetCartFreightRate().ToCustomerCurrency();
                litSelectedShippingMethod.Text += (litSelectedShippingMethod.Text.IsNullOrEmpty()) ? "" : "<br />" + "{0} {1}".FormatWith(selectedShippingMethod, freightRate);
            }

        }

        #endregion
    }
}
