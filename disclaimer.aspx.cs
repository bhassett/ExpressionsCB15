// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for disclaimer.
    /// </summary>
    public partial class disclaimer : System.Web.UI.Page
    {

        #region Variable Declaration
        INavigationService _navigationService = null;
        #endregion

        protected override void OnInit(EventArgs e)
        {
            PageNoCache();
            InitializeDomainServices();
            InitPageContent();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);

                if (!CommonLogic.CookieCanBeDangerousContent("SiteDisclaimerAccepted", true).IsNullOrEmptyTrimmed())
                {
                    _navigationService.NavigateToUrl(ReturnURL.Text);
                }

            }else{

                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", CommonLogic.GetNewGUID());
                AppLogic.SetSessionCookie("SiteDisclaimerDisagree", String.Empty);

                pnlDisclaimer.Visible = false;
                Response.AddHeader("REFRESH", "1; URL=" + ReturnURL.Text);
            }

            base.OnLoad(e);
        }

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
        }

        protected void PageNoCache()
        {
            ServiceFactory.GetInstance<IRequestCachingService>().PageNoCache();
        }

        protected void InitPageContent()
        {
             DisclaimerContents.Text = new Topic("SiteDisclaimer", 1).Contents;

             ReturnURL.Text = CommonLogic.QueryStringCanBeDangerousContent("returnToUrl", true);
             AppLogic.CheckForScriptTag(ReturnURL.Text);

             string returnUrl = ReturnURL.Text.IsNullOrEmptyTrimmed() ? AppLogic.AppConfig("SiteDisclaimerAgreedPage") : ReturnURL.Text;

             if (returnUrl.IsNullOrEmptyTrimmed())
             {
                 returnUrl = CommonLogic.QueryStringBool("checkout") ? "shoppingcart.aspx?checkout=true" : "default.aspx";
             }

             ReturnURL.Text = returnUrl;

        }

        protected void DoNotAgreeButton_Click(object sender, EventArgs e)
        {
            AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);
            AppLogic.SetSessionCookie("SiteDisclaimerDisagree", CommonLogic.GetNewGUID());

            _navigationService.NavigateToUrl(AppLogic.AppConfig("SiteDisclaimerNotAgreedURL"));
        }

    }
}