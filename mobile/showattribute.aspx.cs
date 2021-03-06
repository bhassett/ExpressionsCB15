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
	/// Summary description for showcategory.
	/// </summary>
	public partial class showcategory : SkinBase
	{
        ShowEntityPage m_EP;
		protected void Page_Load(object sender, System.EventArgs e)
		{
            m_EP = new ShowEntityPage(EntityDefinitions.readonly_AttributeEntitySpecs, this);
            m_EP.Page_Load(sender, e);
		}

        protected override bool EnableScriptGlobalization
        {
            get { return true; }
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/product_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/kitProduct_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            m_EP.RenderContents(writer);
        }

        override protected void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

	}
}