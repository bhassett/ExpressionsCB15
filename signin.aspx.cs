// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public partial class signin : SkinBase
    {
        #region Declaration

        InputValidator _EmailValidator = null;

        #endregion

        #region Domain Services

        private IStringResourceService _stringResourceService = null;
        private INavigationService _navigationService = null;
        private IAuthenticationService _authenticationService = null;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            RegisterDomainServices();
            RegisterEvents();
            Initialize();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            
            if (ThisCustomer.IsRegistered)
            {
                _navigationService.NavigateToAccountPage();
            }

            LoginButton.Text = _stringResourceService.GetString("signin.aspx.16", true);
            RequestPassword.Text = _stringResourceService.GetString("signin.aspx.15", true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(LoginButton, "signin.aspx.16");
                AppLogic.EnableButtonCaptionEditing(RequestPassword, "signin.aspx.15");
            }

            ReturnURL.Text = "ReturnURL".ToQueryString();

            if (ReturnURL.Text.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (HttpContext.Current.Request.Browser.Browser.Equals("Firefox", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorMsgLabel.Text = String.Empty;
            }

            string errorMsg = "ErrorMsg".ToQueryString();
            if (errorMsg.Trim().Length != 0)
            {
                ErrorMsgLabel.Text = errorMsg;
                ErrorPanel.Visible = true;
            }

            RequireSecurePage();
            SectionTitle = _stringResourceService.GetString("signin.aspx.1", true);
            if (!Page.IsPostBack)
            {
                if (ReturnURL.Text.Length == 0)
                {
                    if (CommonLogic.QueryStringBool("checkout"))
                    {
                        ReturnURL.Text = "shoppingcart.aspx?checkout=true";
                    }
                    else
                    {
                        ReturnURL.Text = "default.aspx";
                    }
                }

                var rememberMeCustomer = _authenticationService.GetRememberMeInfo();
                if (rememberMeCustomer != null)
                {
                    EMail.Text = rememberMeCustomer.Email;
                    this.Password.Attributes.Add("value", rememberMeCustomer.DecryptedPassword);
                    this.PersistLogin.Checked = true;
                }
                else
                {
                    EMail.Text = String.Empty;
                    Password.Text = String.Empty;
                }

                CheckoutMap.HotSpots[0].AlternateText = _stringResourceService.GetString("checkoutanon.aspx.2", true);
            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityImage.Visible = true;
                SecurityCode.Visible = true;
                RequiredFieldValidator4.Enabled = true;
                Label1.Visible = true;
                SecurityImage.ImageUrl = "Captcha.ashx?id=1";
            }
            HeaderMsg.SetContext = this;
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string emailField = EMail.Text.ToLower();
            string passwordField = Password.Text;

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                string errorMessage = _stringResourceService.GetString("signin.aspx.22", true)
                                                            .FormatWith(String.Empty, String.Empty);

                if (Session["SecurityCode"] != null)
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
                        ErrorMsgLabel.Text = errorMessage;
                        ErrorPanel.Visible = true;
                        SecurityCode.Text = String.Empty;
                        SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                        return;
                    }
                }
                else
                {
                    ErrorMsgLabel.Text = errorMessage;
                    ErrorPanel.Visible = true;
                    SecurityCode.Text = String.Empty;
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                    return;
                }
            }

            if (emailField.IsNullOrEmptyTrimmed() || passwordField.IsNullOrEmptyTrimmed())
            {
                DisplayInvalidLogin();
                return;
            }

            if (CheckValidEmail())
            {
                var status = _authenticationService.Login(EMail.Text, passwordField, PersistLogin.Checked);

                if (!status.IsValid)
                {
                    if (status.IsAccountExpired)
                    {
                        DisplayExpiredAccount();
                    }
                    else
                    {
                        DisplayInvalidLogin();
                    }
                    return;
                }

                FormPanel.Visible = false;
                ExecutePanel.Visible = true;
                SignInExecuteLabel.Text = _stringResourceService.GetString("signin.aspx.2");

                var customerWithValidLogin = _authenticationService.GetCurrentLoggedInCustomer();
                string sReturnURL = _authenticationService.GetRedirectUrl(customerWithValidLogin.ContactGUID.ToString(), PersistLogin.Checked);
                if (sReturnURL.Length == 0)
                {
                    sReturnURL = ReturnURL.Text;
                }
                if (sReturnURL.Length == 0)
                {
                    if (DoingCheckout.Checked)
                    {
                        sReturnURL = "shoppingcart.aspx";
                    }
                    else
                    {
                        sReturnURL = "default.aspx";
                    }
                }
                if (sReturnURL.Contains("default.aspx"))
                {
                    sReturnURL = sReturnURL.Replace("default", "account");
                }

                if (sReturnURL.Contains("download.aspx"))
                {
                    sReturnURL = sReturnURL + "&sid=" + "sid".ToQueryString();
                }

                _navigationService.NavigateToUrl(sReturnURL.ToUrlDecode());
            }

        }

        protected void RequestPassword_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = true; // that is where the status msg goes, in all cases in this routine

            //FireFox does not validate RequiredFieldValidator1.
            //This code will double check forgotemail has value.
            if (ForgotEMail.Text.Trim() == string.Empty)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.3", true);
                return;
            }

            //Decrypt connectionstring using salt & vector scheme implemented by Interprise.
            ErrorMsgLabel.Text = String.Empty;
            string PWD = String.Empty;
            bool passwordValid = true;
            string customerCode = String.Empty;
            string contactCode = String.Empty;
            bool exists = false;

            string sql = string.Format("SELECT EntityCode, cc.ContactCode, Password,PasswordSalt,PasswordIV FROM CRMContact cc WITH (NOLOCK) INNER JOIN EcommerceCustomerActiveSites ecas ON cc.ContactCode = ecas.ContactCode WHERE IsAllowWebAccess=1 AND UserName= {0} AND ecas.WebSiteCode = {1} AND ecas.IsEnabled = 1", DB.SQuote(ForgotEMail.Text.ToLower()), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode));
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var rs = DB.GetRSFormat(con, sql))
                {
                    exists = rs.Read();
                    if (exists)
                    {
                        string pwdCypher = DB.RSField(rs, "Password");
                        string salt = DB.RSField(rs, "PasswordSalt");
                        string iv = DB.RSField(rs, "PasswordIV");
                        customerCode = DB.RSField(rs, "EntityCode");
                        contactCode = DB.RSField(rs, "ContactCode");

                        try
                        {
                            var tmpCrypto = new Interprise.Licensing.Base.Services.CryptoServiceProvider();
                            PWD = tmpCrypto.Decrypt(Convert.FromBase64String(pwdCypher),
                                                    Convert.FromBase64String(salt),
                                                    Convert.FromBase64String(iv));
                        }
                        catch
                        {
                            passwordValid = false;
                        }
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", true);
                        return;
                    }
                }
            }

            if (exists && !passwordValid)
            {
                byte[] salt = InterpriseHelper.GenerateSalt();
                byte[] iv = InterpriseHelper.GenerateVector();

                string newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                string newPasswordCypher = InterpriseHelper.Encryption(newPassword, salt, iv);

                string saltBase64 = Convert.ToBase64String(salt);
                string ivBase64 = Convert.ToBase64String(iv);

                DB.ExecuteSQL("UPDATE CRMContact SET Password = {0}, PasswordSalt = {1}, PasswordIV = {2} WHERE EntityCode = {3} AND ContactCode = {4}", DB.SQuote(newPasswordCypher), DB.SQuote(saltBase64), DB.SQuote(ivBase64), DB.SQuote(customerCode), DB.SQuote(contactCode));

                PWD = newPassword;
            }

            if (PWD.Length != 0)
            {
                string FromEMail = AppLogic.AppConfig("MailMe_FromAddress");
                string EMail = ForgotEMail.Text;
                bool SendWasOk = false;
                try
                {
                    string WhoisRequestingThePassword = "\r\n" + ThisCustomer.LastIPAddress + "\r\n" + DateTime.Now.ToString();
                    string MsgBody = string.Empty;

                    MsgBody = InterpriseHelper.GetPasswordEmailTemplate(EMail);
                    if (MsgBody.Length > 0)
                    {
                        AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " " + AppLogic.GetString("lostpassword.aspx.5", true), MsgBody, true, FromEMail, FromEMail, EMail, EMail, "", AppLogic.AppConfig("MailMe_Server"));
                        SendWasOk = true;
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", true);
                    }
                }
                catch { }
                if (SendWasOk)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.2", true);
                }
                else
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.3", true);
                }
            }
            else
            {
                ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", true);
            }
        }

        #endregion

        #region Methods

        private void RegisterEvents()
        {
            LoginButton.Click += LoginButton_Click;
            RequestPassword.Click += RequestPassword_Click;
        }

        private void RegisterDomainServices()
        {
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        }

        private void DisplayInvalidLogin()
        {
            ErrorMsgLabel.Text = _stringResourceService.GetString("signin.aspx.20", true);
            ErrorPanel.Visible = true;
        }

        private void DisplayExpiredAccount()
        {
            ErrorMsgLabel.Text = _stringResourceService.GetString("signin.aspx.message.1", true);
            lnkContactUs.Text = _stringResourceService.GetString("menu.Contact", true);
            lnkContactUs.Visible = true;
            ErrorPanel.Visible = true;
        }

        private bool CheckValidEmail()
        {
            ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.21", true);
            ErrorPanel.Visible = true;

            _EmailValidator = new RegularExpressionInputValidator(EMail, DomainConstants.EmailRegExValidator, ErrorMsgLabel.Text.ToString());
            _EmailValidator.Validate();
            return (_EmailValidator.IsValid);
        }

        private void Initialize()
        {
            bool isCheckout = CommonLogic.QueryStringBool("checkout");
            DoingCheckout.Checked = isCheckout;
            CheckoutPanel.Visible = isCheckout;
            SignUpLink.NavigateUrl = "createaccount.aspx?checkout={0}".FormatWith(isCheckout.ToString());
        }

        #endregion
    }
}
