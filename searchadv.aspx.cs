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
	/// Summary description for searchadv.
	/// </summary>
	public partial class searchadv : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}
			SectionTitle =  AppLogic.GetString("searchadv.aspx.1");
		}

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            string xmlPackageName = ServiceFactory.GetInstance<IAppConfigService>().SearchAdvXMLPackage;
            if (xmlPackageName.IsNullOrEmptyTrimmed())
            {
                xmlPackageName = "page.searchadv.xml.config";
            }
            writer.Write(AppLogic.RunXmlPackage(xmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true));
        }
        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));
            manager.LoadScriptsBeforeUI = false;
        }
	}
}
