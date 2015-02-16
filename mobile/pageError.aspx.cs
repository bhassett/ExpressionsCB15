using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class pageError : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("pageerror.aspx.1");
            Package1.SetContext = this;
            ErrorMessage.Text = "Parameter".ToQueryString().ToUrlDecode();
        }
    }
}