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
    /// Summary description for editaddress.
    /// </summary>
    public partial class editaddress : SkinBase
    {

        #region Variable Declarations

        bool _checkOutMode = false;

        string _addressId = String.Empty;
        string _addressTypeString = String.Empty;
        string _returnURL = String.Empty;

        AddressTypes _addressType = AddressTypes.Unknown;

        private IStringResourceService _stringResourceService = null;

        #endregion

        #region Initialize Domain Services

        private void InitializeDomainServices()
        {
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
        }

        #endregion

        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            ResolveQueryStrings();
            PerformPageAccessLogic();

            if (!IsPostBack)
            {
                InitializePageContent();
                AddressBookHelpfulTips.SetContext = this;
            }

            InitAddressControl(CommonLogic.QueryStringCanBeDangerousContent("AddressType"));
            base.OnInit(e);
        }

        #endregion

        #region ResolveQueryStrings

        private void ResolveQueryStrings()
        {
            _checkOutMode = CommonLogic.QueryStringBool("checkout");
            _addressId = CommonLogic.QueryStringCanBeDangerousContent("AddressID");
        }

        #endregion

        #region PerformPageAccessLogic

        private void PerformPageAccessLogic()
        {
            if (AppLogic.AppConfigBool("UseSSL"))
            {
                if (AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") != "1")
                {
                    Response.Redirect(AppLogic.GetStoreHTTPLocation(true) + "changeaddress.aspx?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }

            _returnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (_returnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            _addressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            if (_addressTypeString.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            //AddressTypeString is empty if you change language.
            if (_addressTypeString.Length != 0)
            {
                _addressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), _addressTypeString, true);
            }
            if (_addressType == AddressTypes.Unknown)
            {
                _addressType = AddressTypes.Shipping;
                _addressTypeString = "Shipping";
            }
        }

        #endregion

        private void InitAddressControl(string addressType)
        {
            AddressControl.LabelStreetText = _stringResourceService.GetString("editaddress.aspx.5");
            AddressControl.LabelCityText = _stringResourceService.GetString("editaddress.aspx.6");
            AddressControl.LabelStateText = _stringResourceService.GetString("editaddress.aspx.7");
            AddressControl.LabelPostalText = _stringResourceService.GetString("editaddress.aspx.8");
            AddressControl.LabelEnterPostalText = _stringResourceService.GetString("editaddress.aspx.9");
            AddressControl.LabelCountyText = _stringResourceService.GetString("editaddress.aspx.10");

            if (addressType.ToLower() == "shipping")
            {
                AddressControl.IsShowResidenceTypesSelector = true;
                AddressControl.DefaultAddressType = _stringResourceService.GetString("editaddress.aspx.13");
            }
            else
            {

                AddressControl.IsShowResidenceTypesSelector = false;
                
            }

            AddressControl.IsShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            AddressControl.BindData();
        }


        #region InitializePageContent

        private void InitializePageContent()
        {
            RenderAddressDetails();

            pnlCheckoutImage.Visible = _checkOutMode;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");

            btnReturn.Text = AppLogic.GetString("account.aspx.27", true);

            string returnTo = (_checkOutMode && !_returnURL.IsNullOrEmptyTrimmed()) ? String.Format("&ReturnUrl={0}", _returnURL) : String.Empty;
            btnReturn.OnClientClick = String.Format("self.location='selectaddress.aspx?checkout={0}&&AddressType={1}{2}';return false;", _checkOutMode.ToStringLower(), _addressTypeString, returnTo);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnReturn, "account.aspx.27");
            }

        }

        #endregion

        #region RenderAddressDetails

        private void RenderAddressDetails()
        {
            if (!IsPostBack)
            {
                var address = Address.Get(ThisCustomer, _addressType, _addressId);

                txtContactName.Text = address.Name;
                txtPhone.Value = address.Phone;
                txtContactNumber.Text = address.Phone;

                AddressControl.Street = address.Address1;
                AddressControl.City = address.City;
                AddressControl.State = address.State;

                AddressControl.ResidenceTypeSelected = address.ResidenceType.ToString();
                AddressControl.SelectedCountry = address.Country;

                if (AppLogic.AppConfigBool("Address.ShowCounty") && !address.County.IsNullOrEmptyTrimmed())
                {
                    AddressControl.County = address.County;
                }

                AddressControl.Postal = (address.Plus4.IsNullOrEmptyTrimmed()) ? address.PostalCode : String.Format("{0}-{1}", address.PostalCode, address.Plus4);
            }
        }

        #endregion

        #region SaveAddress

        /// <summary>
        /// Saving/Updating of Editted Address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnUpdateAddress_Click(object sender, EventArgs e)
        {
            var address = Address.Get(ThisCustomer, _addressType, _addressId);
            
            address.Name = txtContactName.Text;
            address.Phone = txtContactNumber.Text;

            var cityStateArray = GetCityStateArray();
            address.State = cityStateArray[0];
            address.City = cityStateArray[1];

            address.Address1 = AddressControl.Street;
            address.PostalCode = AddressControl.Postal;
            address.Country = AddressControl.Country;

            if (_addressType == AddressTypes.Billing)
            {
                address.CardName = txtContactName.Text;
            }

            if (_addressType == AddressTypes.Shipping)
            {
                address.ResidenceType = (AddressControl.ResidenceType == ResidenceTypes.Residential.ToString()) ? ResidenceTypes.Residential : address.ResidenceType = ResidenceTypes.Commercial;
            }

            if (AppLogic.AppConfigBool("Address.ShowCounty"))
            {
                address.County = AddressControl.County;
            }

            Address.Update(ThisCustomer, address);
            AppLogic.SavePostalCode(address);

            string returnTo = (_checkOutMode && !_returnURL.IsNullOrEmptyTrimmed()) ? String.Format("&ReturnUrl={0}", _returnURL) : String.Empty;
            Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}{2}", _checkOutMode.ToString(), _addressType, returnTo));

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

            this.PageNoCache();

            RequireSecurePage();

            SectionTitle = AppLogic.GetString("editaddress.aspx.1", true);

        }

        #endregion

        #endregion

        #region Get Address City And State 

        private string[] GetCityStateArray()
        {
            var arrCityState = new string[2];

            if (!txtCityStates.Text.IsNullOrEmptyTrimmed())
            {
                var cityState = txtCityStates.Text.Split(',');
                if (cityState.Length > 1)
                {
                    arrCityState[0] = cityState[0].Trim();
                    arrCityState[1] = cityState[1].Trim();
                }
                else
                {
                    arrCityState[0] = String.Empty;
                    arrCityState[1] = cityState[0].Trim();
                }
            }
            else
            {
                arrCityState[0] = AddressControl.State;
                arrCityState[1] = AddressControl.City;
            }

            return arrCityState;

        }

        #endregion
    }
}




