// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for selectaddress.
    /// </summary>
    public partial class selectaddress1 : SkinBase
    {

        #region Variable Declarations

        bool addMode = false;
        bool checkOutMode = false;
        public AddressTypes AddressType = AddressTypes.Unknown;
        public Addresses custAddresses;
        public Boolean setPrimary = false;
        public String AddressTypeString = String.Empty;
        public String ButtonImage = String.Empty;
        public String PaymentMethodPrompt = String.Empty;
        public String Prompt = String.Empty;
        public String ReturnURL = String.Empty;

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();

            ResolveQueryStrings();
            PerformPageAccessLogic();
            InitializePageContent();
            InitializeAddressControls();
            DisplayAddressList();
        }

        #endregion

        #region InitiliazeComponent

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddressList.ItemDataBound += new RepeaterItemEventHandler(AddressList_ItemDataBound);
            this.AddressList.ItemCommand += new RepeaterCommandEventHandler(AddressList_ItemCommand);
        }

        #endregion

        #region ResolveQueryStrings

        private void ResolveQueryStrings()
        {
            addMode = CommonLogic.QueryStringBool("add");
            checkOutMode = CommonLogic.QueryStringBool("checkout");
            setPrimary = CommonLogic.QueryStringBool("SetPrimary");
        }

        #endregion
        
        #region CheckWhichCountriesWeDontRequirePostalCodes

        private void CheckWhichCountriesWeDontRequirePostalCodes()
        {
            string postalCodeNotRequiredCountries = AppLogic.AppConfig("PostalCodeNotRequiredCountries");
            string[] countriesThatDontRequirePostalCodes = postalCodeNotRequiredCountries.Split(',');
            foreach (string country in countriesThatDontRequirePostalCodes)
            {
                ctrlAddress.PostalCodeOptionalCountryCodes.Add(country.Trim());
            }
        }

        #endregion

        #region PerformPageAccessLogic

        private void PerformPageAccessLogic()
        {
            ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            if (AddressTypeString.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            ThisCustomer.RequireCustomerRecord();
            if (!Shipping.MultiShipEnabled())
            {
                RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }

            if (AddressTypeString.Length != 0)
            {
                AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);
            }
            if (AddressType == AddressTypes.Unknown)
            {
                AddressType = AddressTypes.Shipping;
                AddressTypeString = "Shipping";
            }

            custAddresses = new Addresses();
            custAddresses.LoadCustomer(ThisCustomer, AddressType);

            if (AddressType == AddressTypes.Shipping)
            {
                ButtonImage = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/usethisshippingaddress.gif", ThisCustomer.LocaleSetting);
            }
            else
            {
                ButtonImage = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/usethisbillingaddress.gif", ThisCustomer.LocaleSetting);
            }
        }

        #endregion

        #region InitializePageContent

        private void InitializePageContent()
        {
            pnlCheckoutImage.Visible = checkOutMode;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");

            pnlAddressList.Visible = (custAddresses.Count > 0 || addMode);
            pnlAddressListMain.Visible = (!addMode);
            pnlNewAddress.Visible = (addMode);

            lnkAddAddress.Text = AppLogic.GetString("selectaddress.aspx.3");
            lnkAddAddress.NavigateUrl = "selectaddress.aspx?add=true&checkout=" + checkOutMode.ToString() + "&addressType=" + AddressType.ToString() + "&returnURL=" + Server.UrlEncode(ReturnURL);
            lnkAddAddress.Visible = (!addMode);
            liAdd.Visible = (!addMode);

            if (addMode)
            {
                ctrlAddress.Visible = true;
                btnNewAddress.Text = AppLogic.GetString("selectaddress.aspx.2");
            }
        }

        #endregion

        #region InitializeAddressControls

        private void InitializeAddressControls()
        {
            LoadAllAvailableCountriesAndAssignRegistriesForAddresses();
            LoadAvailableSalutationsForBillingAddress();
            ApplyAddressStyles();
            CheckWhichCountriesWeDontRequirePostalCodes();
            InitializeAddressDisplay();
            AssignAddressValidatorPrerequisites();
            AssignHostFormForAddresses();
            AssignErrorSummaryDisplayControlForAddresses();
        }

        #endregion

        #region DisplayAddressList

        private void DisplayAddressList()
        {
            LoadAddresses();
        }

        #endregion

        #region LoadAvailableSalutationsForBillingAddress

        private void LoadAvailableSalutationsForBillingAddress()
        {
            List<KeyValuePair<string, string>> salutations = new List<KeyValuePair<string, string>>();

            salutations.Add(new KeyValuePair<string, string>("Select One", string.Empty));
            
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, "SELECT SalutationDescription FROM SystemSalutation with (NOLOCK) WHERE IsActive = 1"))
                {
                    while (reader.Read())
                    {
                        salutations.Add(new KeyValuePair<string, string>(DB.RSField(reader, "SalutationDescription"), DB.RSField(reader, "SalutationDescription")));
                    }
                }
            }

            ctrlAddress.SetSalutations(salutations);
        }

        #endregion

        #region LoadAllAvailableCountriesAndAssignRegistriesForAddresses

        private void LoadAllAvailableCountriesAndAssignRegistriesForAddresses()
        {
            List<CountryAddressDTO> countries = CountryAddressDTO.GetAllCountries();

            ctrlAddress.Countries = countries;
            ctrlAddress.RegisterCountries = true;
        }

        #endregion

        #region ApplyAddressStyles

        private void ApplyAddressStyles()
        {
            addressbook_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/addressbook.gif", ThisCustomer.LocaleSetting);
            tblAddressList.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblAddressListBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
        }

        #endregion

        #region InitializeAddressDisplay

        private void InitializeAddressDisplay()
        {
            if (AddressType == AddressTypes.Shipping)
            {
                InitShippingAddress();
            }
            else
            {
                InitBillingAddress();
            }
        }

        #endregion

        #region AssignAddressValidatorPrerequisites

        private void AssignAddressValidatorPrerequisites()
        {
            if (AddressType == AddressTypes.Shipping)
            {
                AssignShippingAddressValidatorPrerequisites();
            }
            else
            {
                AssignBillingAddressValidatorPrerequisites();
            }
        }

        #endregion

        #region AssignHostFormForAddresses

        private void AssignHostFormForAddresses()
        {
            ctrlAddress.HostForm = frmAddAddress;
        }

        #endregion

        #region AssignErrorSummaryDisplayControlForAddresses

        private void AssignErrorSummaryDisplayControlForAddresses()
        {
            ctrlAddress.ErrorSummaryControl = this.InputValidatorySummary1;
        }

        #endregion

        #region InitBillingAddress

        private void InitBillingAddress()
        {
            ctrlAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.34");
            ctrlAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35");
            ctrlAddress.AddressCaption = AppLogic.GetString("createaccount.aspx.17");
            ctrlAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15");
            ctrlAddress.BusinessTypeCaption = AppLogic.GetString("address.cs.18");
            ctrlAddress.TaxNumberCaption = AppLogic.GetString("address.cs.17");
            ctrlAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.18");
            ctrlAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6");
            ctrlAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7");
            ctrlAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16");
            ctrlAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33");

            bool _postalCodeOptionalCountry = false;
            foreach (string country in ctrlAddress.PostalCodeOptionalCountryCodes)
            {
                if (country == ctrlAddress.CountryCode) { _postalCodeOptionalCountry = true; }
            }
            if (_postalCodeOptionalCountry)
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.30");
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.29");
            }
            else
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.77");
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31");
            }
            
            ctrlAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32");

            ctrlAddress.ShowResidenceType = false;
            ctrlAddress.RequireSalutation = false;
            ctrlAddress.ShowFirstName = false;
            ctrlAddress.ShowLastName = false;

            ctrlAddress.RequireEmail = false;
            ctrlAddress.RequirePassword = false;
            ctrlAddress.RequireOkToEmail = false;
            ctrlAddress.RequireOver13 = false;
            ctrlAddress.RequireBusinessType = false; 
            ctrlAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
        }

        #endregion

        #region InitShippingAddress

        private void InitShippingAddress()
        {
            ctrlAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.34");
            ctrlAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35");
            ctrlAddress.AddressCaption = AppLogic.GetString("createaccount.aspx.17");
            ctrlAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15");
            ctrlAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.18");
            ctrlAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6");
            ctrlAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7");
            ctrlAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16");
            ctrlAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33");

            bool _postalCodeOptionalCountry = false;
            foreach (string country in ctrlAddress.PostalCodeOptionalCountryCodes)
            {
                if (country == ctrlAddress.CountryCode) { _postalCodeOptionalCountry = true; }
            }
            if (_postalCodeOptionalCountry)
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.30");
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.29");
            }
            else
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.77");
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31");
            }

            ctrlAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32");

            ctrlAddress.RequireSalutation = false;
            ctrlAddress.ShowFirstName = false;
            ctrlAddress.ShowLastName = false;

            ctrlAddress.RequireEmail = false;
            ctrlAddress.RequirePassword = false;
            ctrlAddress.RequireOkToEmail = false;
            ctrlAddress.RequireOver13 = false;
            ctrlAddress.RequireBusinessType = false;
            ctrlAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
        }

        #endregion

        #region AssignBillingAddressValidatorPrerequisites

        private void AssignBillingAddressValidatorPrerequisites()
        {
            ctrlAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.37");
            ctrlAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.38");
            ctrlAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.39");
            ctrlAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.40");
            ctrlAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.41");

            ctrlAddress.FirstNameMaximumCharacterLength = 50;
            ctrlAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.42");
            ctrlAddress.LastNameMaximumCharacterLength = 50;
            ctrlAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.43");
            ctrlAddress.AccountNameMaximumCharacterLength = 100;
            ctrlAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.44");
            ctrlAddress.AddressMaximumCharacterLength = 200;
            ctrlAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.45");
            ctrlAddress.PhoneMaximumCharacterLength = 50;
            ctrlAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.46");
            ctrlAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.69");
            ctrlAddress.CityMaximumCharacterLength = 50;
            ctrlAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.73");
            ctrlAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.70");
            ctrlAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.74");
            ctrlAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.19");
        }

        #endregion

        #region AssignShippingAddressValidatorPrerequisites

        private void AssignShippingAddressValidatorPrerequisites()
        {
            ctrlAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.56");
            ctrlAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.57");
            ctrlAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.58");
            ctrlAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.59");
            ctrlAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.60");
            ctrlAddress.FirstNameMaximumCharacterLength = 50;
            ctrlAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.61");
            ctrlAddress.LastNameMaximumCharacterLength = 50;
            ctrlAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.62");
            ctrlAddress.AccountNameMaximumCharacterLength = 100;
            ctrlAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.63");
            ctrlAddress.AddressMaximumCharacterLength = 200;
            ctrlAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.64");
            ctrlAddress.PhoneMaximumCharacterLength = 50;
            ctrlAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.65");
            ctrlAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.71");
            ctrlAddress.CityMaximumCharacterLength = 50;
            ctrlAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.75");
            ctrlAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.72");
            ctrlAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.76");
            ctrlAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.20");
        }

        #endregion

        #region LoadAddresses

        private void LoadAddresses()
        {
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), (int)AddressType, DB.SQuote(ThisCustomer.ContactCode))))
                {
                    AddressList.DataSource = reader;
                    AddressList.DataBind();
                    reader.Close();
                    btnReturn.Text = AppLogic.GetString("account.aspx.25");
                    btnReturn.OnClientClick = "self.location='account.aspx?checkout=" + checkOutMode.ToString() + "';return false";
                    btnCheckOut.Visible = checkOutMode;
                    btnCheckOut.Text = AppLogic.GetString("account.aspx.24");
                    btnCheckOut.OnClientClick = "self.location='checkoutshipping.aspx';return false;";
                }
            }
        }

        #endregion

        #region AddressList ItemDataBound

        void AddressList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ImageButton MakePrimaryBtn = (ImageButton)e.Item.FindControl("btnMakePrimary");
                ImageButton EditBtn = (ImageButton)e.Item.FindControl("btnEdit");

                MakePrimaryBtn.Visible = (((DbDataRecord)e.Item.DataItem)["PrimaryAddress"].ToString() == "0");

                MakePrimaryBtn.ImageUrl = ButtonImage;
                EditBtn.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/edit.gif");
            }

        }

        #endregion

        #region AddressList ItemCommand

        void AddressList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    Response.Redirect(String.Format("editaddress.aspx?Checkout={0}&AddressType={1}&AddressID={2}&ReturnURL={3}", checkOutMode.ToString(), AddressType, e.CommandArgument, ReturnURL));
                    break;
                case "makeprimary":
                    InterpriseHelper.MakeDefaultAddress(ThisCustomer.ContactCode, e.CommandArgument.ToString(), AddressType);
                    //Update customer default address.
                    if (AddressType == AddressTypes.Shipping)
                    {
                        ThisCustomer.PrimaryShippingAddressID = e.CommandArgument.ToString();
                    }
                    else
                    {
                        ThisCustomer.PrimaryBillingAddressID = e.CommandArgument.ToString();
                    }
                    Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Checked Address Information based on Address Type
        /// And Validate
        /// </summary>
        /// <param name="thisAddress">Address Information</param>
        /// <param name="type">[Enum] Address Type</param>
        /// <returns>bool: true/false</returns>
        public bool CheckToValidate(Address thisAddress, AddressTypes type)
        {
            // For AddressType Objects
            Address billingAddress = null;
            Address shippingAddress = null;

            bool retCheck = false;

                if (type == AddressTypes.Billing)
                {
                    thisAddress.CardName = ctrlAddress.AccountName;
                    this.hidBillCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlBillingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    CountryAddressDTO billCountry = CountryAddressDTO.Find(thisAddress.Country);
                    billingAddress = thisAddress;
                    this.hidBillCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, billCountry);
                    this.hidBlnWithState.Value = billCountry.withState.ToString();
                }
                else
                {
                    this.hidShipCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlShippingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    CountryAddressDTO shipCountry = CountryAddressDTO.Find(thisAddress.Country);
                    shippingAddress = thisAddress;
                    this.hidShipCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, shipCountry);
                    this.hidShpWithState.Value = shipCountry.withState.ToString();
                }

                // Checked if either Billing Address or Shipping Address is Correct
                if (!string.IsNullOrEmpty(this.hidBillCheck.Value) || !string.IsNullOrEmpty(this.hidShipCheck.Value))
                {
                    // Set the PopUp window to Show Up
                    hidValid.Value = "false";
                    retCheck = true;

                    hidSkinID.Value = ThisCustomer.SkinID.ToString();
                    hidLocale.Value = ThisCustomer.LocaleSetting.ToString();

                    if (AddressType == AddressTypes.Billing)
                    {
                        this.hidBillTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.87"));
                        
                        this.hidBlnState.Value = billingAddress.State;
                        this.hidBlnPostalCode.Value = billingAddress.PostalCode;
                        this.hidBlnCountry.Value = billingAddress.Country;
                        this.hidBlnCity.Value = billingAddress.City;
                    }
                    else
                    {
                        this.hidShipTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.88"));
                        
                        this.hidShpState.Value = shippingAddress.State;
                        this.hidShpPostalCode.Value = shippingAddress.PostalCode;
                        this.hidShpCountry.Value = shippingAddress.Country;
                        this.hidShpCity.Value = shippingAddress.City;     
                    }
                }

            return retCheck;
        }

        #region NewAddress

        public void btnNewAddress_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                var AddressType = AddressTypeString.TryParseEnum<AddressTypes>();
                int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
                bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

                if (!AllowShipToDifferentThanBillTo)
                {
                    //Shipping and Billing address must be the same so save both
                    AddressType = AddressTypes.Shared;
                }

                Address thisAddress = new Address();

                thisAddress.ThisCustomer = ThisCustomer;
                thisAddress.CustomerCode = ThisCustomer.CustomerCode;

                thisAddress.Name = ctrlAddress.AccountName;
                thisAddress.Address1 = ctrlAddress.Address;
                thisAddress.City = ctrlAddress.City;
                thisAddress.State = ctrlAddress.State;
                thisAddress.PostalCode = ctrlAddress.PostalCode;
                thisAddress.Country = ctrlAddress.CountryCode;
                thisAddress.Phone = ctrlAddress.PhoneNumber;
                thisAddress.County = ctrlAddress.County;
                thisAddress.ResidenceType = ctrlAddress.ResidenceType;

                if (!CheckToValidate(thisAddress, AddressType))
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Shared:

                            InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                            InterpriseHelper.AddCustomerShipTo(thisAddress);
                            break;
                        case AddressTypes.Billing:

                            InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                            break;
                        case AddressTypes.Shipping:

                            InterpriseHelper.AddCustomerShipTo(thisAddress);
                            break;
                    }

                    Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
                }
            }
        }

        #endregion

        #region Event Handlers

        #region Page_Load

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            SectionTitle = String.Format(AppLogic.GetString("selectaddress.aspx.1"), AddressTypeString);

            if (addMode)
            {
                var script = new StringBuilder();

                script.Append("<script type='text/javascript' >\n");
                script.Append("Sys.Application.add_load(\n");                
                script.Append(" function() { \n");

                script.AppendFormat("   var form = $getElement('{0}');\n", this.frmAddAddress.ClientID);
                script.AppendFormat("   var addressId = '{0}';\n", this.ctrlAddress.ClientID);

                script.Append("   var delAttachHandler = function(ctrl){ form.onsubmit = function(){ return ctrl.validate(true); }}\n");

                script.Append("   var ctrl = ise.Controls.AddressController.getControl(addressId);\n");
                
                script.Append("   if(ctrl) {\n");
                script.Append("       delAttachHandler(ctrl);\n");
                script.Append("   }\n");
                script.Append("   else {\n");
                script.Append("       var observer = {\n");
                script.Append("           notify : function(ctrl) {\n");
                script.Append("               if(ctrl.id == addressId) {\n");
                script.Append("                   delAttachHandler(ctrl);\n");
                script.Append("               }\n");
                script.Append("           }\n");
                script.Append("       }\n");
                script.Append("       ise.Controls.AddressController.addObserver(observer);\n");
                script.Append("   }\n");

                script.Append(" }\n");
                script.Append(");\n");
                script.Append("</script>\n");

                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
            }
        }

        #endregion

        #region OnRenderHeader

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/address_ajax.js"));            
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));  
        }

        #endregion

        #endregion
        
    }
}