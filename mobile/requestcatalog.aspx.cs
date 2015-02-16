// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;

using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for requestcatalog.
	/// </summary>
	public partial class requestcatalog : SkinBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("requestcatalog.aspx.1");

            reqFName.ErrorMessage = AppLogic.GetString("requestcatalog.aspx.7");
            reqLName.ErrorMessage = AppLogic.GetString("requestcatalog.aspx.9");
            reqAddr1.ErrorMessage = AppLogic.GetString("requestcatalog.aspx.12");
            reqCity.ErrorMessage = AppLogic.GetString("requestcatalog.aspx.16");
            reqZip.ErrorMessage = AppLogic.GetString("requestcatalog.aspx.20");
            
            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }


        public void btnContinue_OnClick(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                string FormInput = "Customer Name: " + txtFirstName.Text + " " + txtLastName.Text + "<br>\n";
                FormInput += "Company: " + txtCompany.Text + "<br>\n";
                FormInput += "Residence Type: " + ddlShippingResidenceType.SelectedItem.Text + "<br>\n";
                FormInput += "Address1: " + txtAddr1.Text + "<br>\n";
                FormInput += "Address2: " + txtAddr2.Text + "<br>\n";
                FormInput += "Suite: " + txtSuite.Text + "<br>\n";
                FormInput += "City: " + txtCity.Text + "<br>\n";
                FormInput += "State: " + ddlState.SelectedValue + "<br>\n";
                FormInput += "Country: " + ddlCountry.SelectedValue + "<br>\n";
                FormInput += "ZIP: " + txtZip.Text + "<br>\n";
                String Body = "<b>" + AppLogic.GetString("requestcatalog.aspx.2") + "</b><br><br>" + FormInput;
                AppLogic.SendMail(AppLogic.GetString("requestcatalog.aspx.3"), Body, true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), AppLogic.AppConfig("MailMe_ToAddress"), AppLogic.AppConfig("MailMe_ToName"), "", AppLogic.AppConfig("MailMe_Server"));
                lblSuccess.Text = String.Format(AppLogic.GetString("requestcatalog.aspx.4"), AppLogic.AppConfig("SE_MetaTitle"));
                pnlCatalogRequest.Visible = false;
                pnlSuccess.Visible = true;
            }
            else
            {
                InitializePageContent();
            }
        }

        public void reqState_OnServerValidate(Object sender, ServerValidateEventArgs args)
        {
            if (args.Value == "0")
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }


        private void InitializePageContent()
        {
            requestcatalog_aspx_7.Text = AppLogic.GetString("requestcatalog.aspx.5");
            requestcatalog_aspx_8.Text = AppLogic.GetString("requestcatalog.aspx.6");
            requestcatalog_aspx_10.Text = AppLogic.GetString("requestcatalog.aspx.8");
            requestcatalog_aspx_12.Text = AppLogic.GetString("requestcatalog.aspx.10");
            address_cs_58.Text = AppLogic.GetString("address.cs.15");
            requestcatalog_aspx_13.Text = AppLogic.GetString("requestcatalog.aspx.11");
            requestcatalog_aspx_15.Text = AppLogic.GetString("requestcatalog.aspx.13");
            requestcatalog_aspx_16.Text = AppLogic.GetString("requestcatalog.aspx.14");
            requestcatalog_aspx_17.Text = AppLogic.GetString("requestcatalog.aspx.15");
            requestcatalog_aspx_19.Text = AppLogic.GetString("requestcatalog.aspx.17");
            requestcatalog_aspx_21.Text = AppLogic.GetString("requestcatalog.aspx.19");
            requestcatalog_aspx_24.Text = AppLogic.GetString("requestcatalog.aspx.21");
            btnContinue.Text = AppLogic.GetString("requestcatalog.aspx.22");
            AppLogic.GetButtonDisable(btnContinue);

			Address ShippingAddress = new Address();
            ShippingAddress.LoadByCustomer(ThisCustomer, AddressTypes.Shipping, ThisCustomer.PrimaryBillingAddressID);

            txtFirstName.Text = ShippingAddress.FirstName;
            txtLastName.Text = ShippingAddress.LastName;
            txtCompany.Text = ShippingAddress.Company;
            txtAddr1.Text = ShippingAddress.Address1;
            txtSuite.Text = ShippingAddress.Suite;
            txtCity.Text = ShippingAddress.City;
            txtZip.Text = ShippingAddress.PostalCode;

            ddlShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.12"), ((int)ResidenceTypes.Unknown).ToString()));
            ddlShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.13"), ((int)ResidenceTypes.Residential).ToString()));
            ddlShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.14"), ((int)ResidenceTypes.Commercial).ToString()));
            ddlShippingResidenceType.SelectedValue = ((int)ShippingAddress.ResidenceType).ToString();
            
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader dr = DB.GetRSFormat(con, "select * from State with (NOLOCK) order by DisplayOrder,Name"))
                {
                    ddlState.DataSource = dr;
                    ddlState.DataTextField = "name";
                    ddlState.DataValueField = "Abbreviation";
                    ddlState.DataBind();
                    ddlState.Items.Insert(0, new ListItem(AppLogic.GetString("requestcatalog.aspx.18"), "0"));
                    ddlState.SelectedValue = ShippingAddress.State;
                }
            }


            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader dr2 = DB.GetRSFormat(con, "select * from Country with (NOLOCK) order by DisplayOrder,Name"))
                {
                    ddlCountry.DataSource = dr2;
                    ddlCountry.DataTextField = "Name";
                    ddlCountry.DataValueField = "Name";
                    ddlCountry.DataBind();
                    ddlCountry.Items.Insert(0, new ListItem(AppLogic.GetString("requestcatalog.aspx.18"), "0"));
                    ddlCountry.SelectedValue = ShippingAddress.Country;
                }
            }
        }



	}
}
