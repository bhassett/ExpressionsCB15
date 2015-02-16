using InterpriseSuiteEcommerceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using InterpriseSuiteEcommerceCommon.Extensions;

/// <summary>
/// Summary description for LoginControl
/// </summary>
public class LoginControl
{ 
    private static LoginControl _instance;

    public void Initialize()
    {
        if (_instance == null)
        {
            _instance = new LoginControl();
        }
    }
    public static LoginControl Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new LoginControl();
            }
            return _instance;
        }
    }

    public string GetUserLoginControl()
    {
        string returnstring = String.Empty;
        if (Customer.Current.IsRegistered)
        {
            var root = new XElement(DomainConstants.XML_ROOT_NAME);
            root.Add(new XElement("DisplayType", "Login"));
            root.Add(new XElement("LoginStatusText", AppLogic.GetString("skinbase.cs.1")));
            root.Add(new XElement("CustomerURL", "account.aspx"));
            root.Add(new XElement("CustomerFullName", Customer.Current.FullName.ToHtmlEncode()));
            root.Add(new XElement("SignOutURL", "signout.aspx"));
            root.Add(new XElement("SignOutText", AppLogic.GetString("skinbase.cs.5")));

            returnstring = new XmlPackage2("account.login.xml.config", root).TransformString();
        }
        else
        {
            string currentPage = CommonLogic.PageInvocation();
            string[] hideFromPages = new string[] { "createaccount.aspx", "signin.aspx" };
            for (int i = 0; i < hideFromPages.Length; i++)
            {
                if (currentPage.ToLower().Contains(hideFromPages[i]))
                {
                    return string.Empty;
                }
            }

            var root = new XElement(DomainConstants.XML_ROOT_NAME);
            var error = CommonLogic.QueryStringCanBeDangerousContent("error");

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                root.Add(new XElement("ShowCaptcha", true));
                root.Add(new XElement("CaptchaText", AppLogic.GetString("signin.aspx.18")));
                root.Add(new XElement("CaptchaImage", "Captcha.ashx?id=1"));
                root.Add(new XElement("CaptchaValidationMessage", AppLogic.GetString("signin.aspx.17")));
                root.Add(new XElement("CaptchaShowValidationMessage", false));
            }
            else
            {
                root.Add(new XElement("ShowCaptcha", false));
            }

            var errorContainerXElement = new XElement("ShowErrorContainer", false);
            var emailShowValidation = new XElement("EmailShowValidationMessage", false);
            var emailValidationMessage = new XElement("EmailValidationMessage", AppLogic.GetString("signin.aspx.3"));
         
            switch (error)
            {
                case "captcha":
                    {
                        errorContainerXElement.SetValue(true);
                        root.Add(new XElement("ErrorMessage", AppLogic.GetString("signin.aspx.22", true).FormatWith(String.Empty, String.Empty)));
                    } break;
                case "expiredaccount":
                    {
                        errorContainerXElement.SetValue(true);
                        root.Add(new XElement("ErrorMessage", AppLogic.GetString("signin.aspx.message.1", true)));
                        root.Add(new XElement("ContactUsText", AppLogic.GetString("menu.Contact", true)));

                        //this.pErrorMessage.InnerText = _stringResourceService.GetString("signin.aspx.message.1", true);
                        //this.lnkContactUs.InnerText = _stringResourceService.GetString("menu.Contact", true);
                    } break;
                case "invalidlogin":
                    {
                        errorContainerXElement.SetValue(true);
                        root.Add(new XElement("ErrorMessage", AppLogic.GetString("signin.aspx.20", true)));
                    } break;
                case "invalidemail":
                    {
                        emailShowValidation.SetValue(true);
                        emailValidationMessage.SetValue(AppLogic.GetString("signin.aspx.21"));
                   
                    } break;
            }
            root.Add(errorContainerXElement);

            root.Add(new XElement("EmailText", AppLogic.GetString("signin.aspx.9")));
            root.Add(emailShowValidation);
            root.Add(emailValidationMessage);
            
            root.Add(new XElement("PasswordText", AppLogic.GetString("signin.aspx.10")));
            root.Add(new XElement("PasswordValidationMessage", AppLogic.GetString("signin.aspx.4")));
            root.Add(new XElement("PasswordShowValidationMessage", false));
            
            root.Add(new XElement("RememberText", AppLogic.GetString("signin.aspx.11")));
            root.Add(new XElement("RememberChecked", false));
            
            root.Add(new XElement("LoginButtonText", AppLogic.GetString("signin.aspx.16")));
            root.Add(new XElement("RegisterButtonText", AppLogic.GetString("signin.aspx.6")));
            
            returnstring = new XmlPackage2("account.login.xml.config", root).TransformString();
        }
        return returnstring;
    }
     
}