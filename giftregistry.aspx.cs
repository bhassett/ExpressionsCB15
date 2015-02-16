using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;

namespace InterpriseSuiteEcommerce
{
    public partial class giftregistry : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("giftregistry.aspx.13", true);

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            if (ThisCustomer.IsNotRegistered)
            {
                string requestedPage = Security.UrlEncode(Request.Url.PathAndQuery);
                Response.Redirect("findgiftregistry.aspx");
            }

            if (!AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                CurrentContext.GoPageNotFound();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            litRegistryHeader.Text = AppLogic.GetString("giftregistry.aspx.1");
            litCreateRegistry.Text = AppLogic.GetString("giftregistry.aspx.2");
            litFindRegistry.Text = AppLogic.GetString("giftregistry.aspx.3");

            GiftRegistryList1.ThisCustomer = ThisCustomer;
            GiftRegistryList1.GiftRegistries = ThisCustomer.GiftRegistries;

            base.OnInit(e);
        }
    }
}