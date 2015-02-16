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
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for search.
	/// </summary>
	public partial class search : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("search.aspx.1");
		}

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            string xmlPackageName = ServiceFactory.GetInstance<IAppConfigService>().SearchXMLPackage;
            if (xmlPackageName.IsNullOrEmptyTrimmed())
            {
                xmlPackageName = "page.searchwithcompare.xml.config";
            }
            writer.Write(AppLogic.RunXmlPackage(xmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true));
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/productcompare.js"));
            manager.LoadScriptsBeforeUI = false;

        }
	}
}
