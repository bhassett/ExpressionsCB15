﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    public partial class pageError : InterpriseSuiteEcommerce.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("pageerror.aspx.1", true);
            Package1.SetContext = this;
            ErrorMessage.Text = Server.UrlDecode(Request.QueryString["Parameter"].ToString());
        }
    }
}
