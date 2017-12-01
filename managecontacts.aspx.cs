using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

namespace InterpriseSuiteEcommerce
{
    public partial class managecontacts : SkinBase
    {
        protected override void OnInit(EventArgs e)
        {
            Initialize();
            base.OnInit(e);
        }

        private void Initialize()
        {
            this.AllowDefaultContactOnly();
            base.SectionTitle = "Manage Contacts";
            PackageManageContact.SetContext = this;
        }

        protected void AllowDefaultContactOnly()
        {
            if (!CustomerContactDTO.IsCustomerContactDefault(this.ThisCustomer))
            {
                Context.Response.Redirect("default.aspx");
            }
        }
    }
}



 

