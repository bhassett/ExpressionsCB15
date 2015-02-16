// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for showmanufacturer.
	/// </summary>
	public partial class showmanufacturer : SkinBase
	{
		ShowEntityPage m_EP;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_EP = new ShowEntityPage(EntityDefinitions.readonly_ManufacturerEntitySpecs,this);
			m_EP.Page_Load(sender,e);
		}

        protected override bool EnableScriptGlobalization
        {
            get { return true; }
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/imagezoom.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/productcompare.js"));
            manager.CompositeScript.Scripts.Add(new ScriptReference("jscripts/formatting/accounting.min.js"));
            manager.LoadScriptsBeforeUI = false;

            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            m_EP.RenderContents(writer);
        }

        override protected void OnPreInit(EventArgs e)
        {
            if (AppLogic.AppConfigBool("TemplateSwitching.Enabled"))
            {
                string HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_ManufacturerEntitySpecs.m_EntityName);
                SetTemplate(HT);
            }
            base.OnPreInit(e);
        }

	}
}
