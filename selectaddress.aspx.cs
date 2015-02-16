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
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for selectaddress.
    /// </summary>
    public partial class selectaddress : SkinBase
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

        private INavigationService _navigationService = null;
        private ICustomerService _customerService = null;
        private IStringResourceService _stringResourceService = null;

        #endregion

        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            InitializeDomainServices();

            ResolveQueryStrings();
            PerformPageAccessLogic();
            InitializePageContent();
            DisplayAddressList();

            AddressBookHelpfulTips.SetContext = this;

            BindControls();
            base.OnInit(e);
        }

        private void BindControls()
        {
            if (checkOutMode)
            {
                btnCheckOut.Visible = checkOutMode;
                btnCheckOut.Text = AppLogic.GetString("account.aspx.24");
                btnCheckOut.Click += btnCheckOut_Click;
            }
        }

        #endregion

        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        }

        #region InitiliazeComponent

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddressList.ItemDataBound += AddressList_ItemDataBound;
            this.AddressList.ItemCommand += AddressList_ItemCommand;
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

            if (CommonLogic.QueryStringBool("editaddress") && !ThisCustomer.IsRegistered)
            {
                string url = CommonLogic.IIF(AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"), "checkout1.aspx", String.Format("createaccount.aspx?checkout=true&skipreg=true&editaddress=true"));
                Response.Redirect(url);
            }

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

        private void InitAddressControl(string addressType)
        {

            AddressControl.LabelStreetText =  _stringResourceService.GetString("selectaddress.aspx.17");
            AddressControl.LabelCityText =  _stringResourceService.GetString("selectaddress.aspx.18");
            AddressControl.LabelStateText = _stringResourceService.GetString("selectaddress.aspx.19");
            AddressControl.LabelPostalText = _stringResourceService.GetString("selectaddress.aspx.20");
            AddressControl.LabelEnterPostalText = _stringResourceService.GetString("selectaddress.aspx.21");
            AddressControl.LabelCountyText = _stringResourceService.GetString("selectaddress.aspx.22");
      
            if(addressType.ToLower() == "shipping"){

                AddressControl.IsShowResidenceTypesSelector = true;
                AddressControl.DefaultAddressType = _stringResourceService.GetString("selectaddress.aspx.25");

            }else{
                AddressControl.IsShowResidenceTypesSelector = false;
            }

            AddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            AddressControl.BindData();
        }

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

            string addressType = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            pnlSaveAddress.Visible = addMode;
            InitAddressControl(addressType);
        }

        #endregion

        #region DisplayAddressList

        private void DisplayAddressList()
        {
            LoadAddresses();
        }

        #endregion

        #region LoadAddresses

        private void LoadAddresses()
        {
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), (int)AddressType, DB.SQuote(ThisCustomer.ContactCode))))
                {
                    AddressList.DataSource = reader;
                    AddressList.DataBind();
                    reader.Close();
                    btnReturn.Text = AppLogic.GetString("account.aspx.25", true);
                    btnReturn.OnClientClick = "self.location='account.aspx?checkout=" + checkOutMode.ToString() + "';return false";
                    if (ThisCustomer.IsInEditingMode())
                    {
                        AppLogic.EnableButtonCaptionEditing(btnReturn, "account.aspx.25");
                        AppLogic.EnableButtonCaptionEditing(btnCheckOut, "account.aspx.24");
                    }
                }
            }
        }

        #endregion

        #region AddressList ItemDataBound

        void AddressList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var MakePrimaryBtn = (ImageButton)e.Item.FindControl("btnMakePrimary");
                var EditBtn = (ImageButton)e.Item.FindControl("btnEdit");

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

                    ServiceFactory.GetInstance<ICustomerService>()
                                  .MakeDefaultAddress(e.CommandArgument.ToString(), AddressType);

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
        
        #region NewAddress

        public void btnNewAddress_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) return;

            AddressTypes AddressType = AddressTypeString.TryParseEnum<AddressTypes>();
            int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

            if (!AllowShipToDifferentThanBillTo)
            {
                //Shipping and Billing address must be the same so save both
                AddressType = AddressTypes.Shared;
            }

            var thisAddress = new Address()
            {
                ThisCustomer = ThisCustomer,
                CustomerCode = ThisCustomer.CustomerCode,
                Name = txtContactName.Text,
                Address1 = AddressControl.Street,
                PostalCode = AddressControl.Postal,
                Country = AddressControl.Country,
                Phone = txtContactNumber.Text
            };

            string bCityStates = txtCityStates.Text;
            string city = String.Empty;
            string state = String.Empty;

            if (!bCityStates.IsNullOrEmptyTrimmed())
            {
                var _cityState = bCityStates.Split(',');

                if (_cityState.Length > 1)
                {
                    state = _cityState[0].Trim();
                    city = _cityState[1].Trim();
                }
                else
                {
                    city = _cityState[0].Trim();
                    state = string.Empty;
                }

            }
            else
            {
                state = AddressControl.State;
                city = AddressControl.City;
            }

            thisAddress.City = city;
            thisAddress.State = state;

            if (AppLogic.AppConfigBool("Address.ShowCounty")) thisAddress.County = AddressControl.County;

            switch (AddressType)
            {
                case AddressTypes.Shared:

                    thisAddress.ResidenceType = ResidenceTypes.Residential;

                    InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                    InterpriseHelper.AddCustomerShipTo(thisAddress);

                    break;
                case AddressTypes.Billing:

                    thisAddress.ResidenceType = ResidenceTypes.Residential;
                    InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);

                    break;
                case AddressTypes.Shipping:

                    if (AddressControl.ResidenceType == ResidenceTypes.Residential.ToString())
                    {
                        thisAddress.ResidenceType = ResidenceTypes.Residential;
                    }
                    else
                    {
                        thisAddress.ResidenceType = ResidenceTypes.Commercial;
                    }

                    InterpriseHelper.AddCustomerShipTo(thisAddress);

                    break;
            }

            AppLogic.SavePostalCode(thisAddress);
            Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
        }

        #endregion

        #region Event Handlers

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
            PageNoCache();

            RequireSecurePage();

            SectionTitle = String.Format(AppLogic.GetString("selectaddress.aspx.1"), AddressTypeString);

        }

        void btnCheckOut_Click(object sender, EventArgs e)
        {
            string redirecTo = (checkOutMode && ReturnURL.IsNullOrEmpty()) ? "checkoutshipping.aspx" : ReturnURL;
            if (checkOutMode && AppLogic.AppConfigBool("Checkout.UseOnePageCheckout")) redirecTo = "checkout1.aspx";

            _navigationService.NavigateToUrl(redirecTo);
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/address_ajax.js"));            
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));  
        }

        #endregion
        
    }
}