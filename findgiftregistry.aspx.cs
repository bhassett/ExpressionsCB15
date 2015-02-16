﻿using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;

namespace InterpriseSuiteEcommerce
{
    public partial class findgiftregistry : SkinBase
    {
        #region Initializer

        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("giftregistry.aspx.13", true);

            this.PageNoCache();

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            if (!AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                CurrentContext.GoPageNotFound();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
            {
            }

            litRegistryHeader.Text = AppLogic.GetString("findregistry.aspx.aspx.1");
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        #endregion
    }
}