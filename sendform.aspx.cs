// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for sendform.
    /// </summary>
    public partial class sendform : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            SectionTitle = AppLogic.GetString("sendform.aspx.1", true);
            // DOS attack prevention:
            if (AppLogic.OnLiveServer() && (Request.UrlReferrer == null || Request.UrlReferrer.Authority != Request.Url.Authority))
            {
                Response.Redirect("default.aspx", true);
                return;
            }

            var appConfigService = ServiceFactory.GetInstance<IAppConfigService>();

            string formContents = String.Empty;
            if (CommonLogic.FormCanBeDangerousContent("AsXml").Length == 0)
            {
                formContents = CommonLogic.GetFormInput(true, "<br/>");
                formContents = formContents + AppLogic.AppConfig("MailFooter");
            }
            else
            {
                formContents = CommonLogic.GetFormInputAsXml(true, "root");
            }

            string subject = CommonLogic.FormCanBeDangerousContent("Subject");
            if (subject.Length == 0)
            {
                subject = AppLogic.GetString("sendform.aspx.2", true);
            }

            string[] emailacctinfo = InterpriseHelper.GetStoreEmailAccountInfo();

            string sendTo = CommonLogic.FormCanBeDangerousContent("SendTo");
            if (sendTo.Length == 0)
            {
                sendTo = appConfigService.OrderFailedEmailTo;
            }
            else
            {
                sendTo += "," + appConfigService.OrderFailedEmailTo;
            }

            foreach (string s in sendTo.Split(','))
            {
                AppLogic.SendMail(subject, formContents, true, emailacctinfo[0], emailacctinfo[1], s, s, "", String.Empty);
            }
            Label1.Text = AppLogic.GetString("sendform.aspx.3");
        }


    }
}
