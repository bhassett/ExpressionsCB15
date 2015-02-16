// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutanon : SkinBase
    {
        #region Declaration

        private string PaymentMethod = String.Empty;
        private InterpriseShoppingCart cart;
        private string _checkoutType = String.Empty;
        private IAuthenticationService _authenticationService = null;
        private IStringResourceService _stringResourceService = null;
        private INavigationService _navigationService = null;
        private IAppConfigService _appConfigService = null;

        #endregion

        #region Methods

        private void RegisterDomainServices()
        {
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>(); 
        }

        private void InitializePageContent()
        {
            CheckoutMap.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");

            btnSignInAndCheckout.Text = _stringResourceService.GetString("checkoutanon.aspx.12", true);
            RegisterAndCheckoutButton.Text = _stringResourceService.GetString("checkoutanon.aspx.13", true);
            Skipregistration.Text = _stringResourceService.GetString("checkoutanon.aspx.14", true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnSignInAndCheckout, "checkoutanon.aspx.12");
                AppLogic.EnableButtonCaptionEditing(RegisterAndCheckoutButton, "checkoutanon.aspx.13");
                AppLogic.EnableButtonCaptionEditing(Skipregistration, "checkoutanon.aspx.14");
            }
        }

        private void BindControls()
        { 
            btnSignInAndCheckout.Click += btnSignInAndCheckout_Click;
            RegisterAndCheckoutButton.Click += RegisterAndCheckoutButton_Click;
            Skipregistration.Click += Skipregistration_Click;
        }

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            BindControls();
            RegisterDomainServices();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.PageNoCache();
            this.RequireSecurePage();
            this.ThisCustomer.RequireCustomerRecord();

            _checkoutType = "checkoutType".ToQueryString();
            SectionTitle = _stringResourceService.GetString("checkoutanon.aspx.1", true);

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
            if (cart.IsEmpty())
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (cart.InventoryTrimmed)
            {
                _navigationService.NavigateToShoppingCartRestLinkBackWithErroMessage(_stringResourceService.GetString("shoppingcart.aspx.1", true));
            }

            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!cart.MeetsMinimumOrderWeight(AppLogic.AppConfigUSDecimal("MinOrderWeight")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                _navigationService.NavigateToShoppingCartRestLinkBack();
            }

            CheckoutMap.HotSpots[0].AlternateText = _stringResourceService.GetString("checkoutanon.aspx.2", true);

            Teaser.SetContext = this;

            if (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                PasswordOptionalPanel.Visible = true;
            }

            ErrorMsgLabel.Text = String.Empty;
            if (!IsPostBack)
            {
                InitializePageContent();
            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityImage.Visible = true;
                SecurityCode.Visible = true;

                trSecurityCodeText.Visible = true;
                trSecurityCodeImage.Visible = true;

                Label4.Visible = true;
                if (!IsPostBack)
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                else
                    SecurityImage.ImageUrl = "Captcha.ashx?id=2";
            }
        }

        protected void btnSignInAndCheckout_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (!Page.IsValid) return;

            string EMailField = EMail.Text.ToLower();
            string PasswordField = Password.Text;

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                string sCode = Session["SecurityCode"].ToString();
                string fCode = SecurityCode.Text;

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
                    ErrorMsgLabel.Text = _stringResourceService.GetString("signin.aspx.22", true)
                                                               .FormatWith(String.Empty, String.Empty);
                    ErrorPanel.Visible = true;                        
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                    return;
                }
            }

            var status = _authenticationService.Login(EMail.Text, PasswordField, true);
            if (!status.IsValid)
            {
                if (status.IsAccountExpired)
                {
                    ErrorMsgLabel.Text = _stringResourceService.GetString("signin.aspx.message.1", true);
                    lnkContactUs.Text = _stringResourceService.GetString("menu.Contact", true);
                    lnkContactUs.Visible = true;
                }
                else 
                {
                    ErrorMsgLabel.Text = _stringResourceService.GetString("signin.aspx.20", true);
                }
                
                ErrorPanel.Visible = true;
                return;
            }

            var customerWithValidLogin = _authenticationService.GetCurrentLoggedInCustomer();
            if (_checkoutType == "pp")
            {
                if (!cart.IsSalesOrderDetailBuilt)
                {
                    cart.BuildSalesOrderDetails();
                }
                customerWithValidLogin.ThisCustomerSession["paypalFrom"] = "checkoutanon";
                _navigationService.NavigateToUrl(PayPalExpress.CheckoutURL(cart));
            }
            else
            {
                FormPanel.Visible = false;
                HeaderPanel.Visible = false;
                ExecutePanel.Visible = true;
                SignInExecuteLabel.Text = _stringResourceService.GetString("signin.aspx.2", true);
                _navigationService.NavigateToShoppingCart() ;
            }
        }

        protected void RegisterAndCheckoutButton_Click(object sender, EventArgs e)
        {
            if (_checkoutType == "pp")
            {
                _navigationService.NavigateToUrl("createaccount.aspx?checkout=true&isAnonPayPal=true");
            }
            else
            {
                _navigationService.NavigateToUrl("createaccount.aspx?checkout=true");
            }
        }

        protected void Skipregistration_Click(object sender, EventArgs e)
        {
            if (_appConfigService.CheckoutUseOnePageCheckout && _appConfigService.PasswordIsOptionalDuringCheckout)
            {
                if (ThisCustomer.IsNotRegistered && !_appConfigService.AllowShipToDifferentThanBillTo)
                {
                    _navigationService.NavigateToUrl("createaccount.aspx?checkout=true&skipreg=true");
                }
                else
                {
                    _navigationService.NavigateToCheckout1();
                }
            }
            else
            {
                if (_checkoutType == "pp")
                {
                    _navigationService.NavigateToUrl("createaccount.aspx?checkout=true&skipreg=true&isAnonPayPal=true");
                }
                else
                {
                    _navigationService.NavigateToUrl("createaccount.aspx?checkout=true&skipreg=true");
                }
            }
        }

        #endregion
    }
}
