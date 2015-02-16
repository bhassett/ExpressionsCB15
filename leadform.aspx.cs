using System;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
namespace InterpriseSuiteEcommerce
{
    public partial class LeadForm : SkinBase
    {
        #region Variable Declaration

        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        INavigationService _navigationService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            InitAddressControl();
            InitProfileControl();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            InitPageContent();
            base.OnLoad(e);
        }

        private void InitializeDomainServices()
        {
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
        }

        private void InitPageContent()
        {
            SectionTitle = _stringResourceService.GetString("leadform.aspx.30", true);
            LeadFormHelpfulTipsTopic.SetContext = this;
   
            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnLeadForm")) pnlSecurityCode.Visible = false;
        }

        private void InitAddressControl()
        {
            AddressControl.LabelStreetText = _stringResourceService.GetString("leadform.aspx.32");
            AddressControl.LabelCityText = _stringResourceService.GetString("leadform.aspx.11");
            AddressControl.LabelStateText = _stringResourceService.GetString("leadform.aspx.10");
            AddressControl.LabelPostalText = _stringResourceService.GetString("leadform.aspx.28");
            AddressControl.LabelEnterPostalText = _stringResourceService.GetString("leadform.aspx.36");
            AddressControl.IsShowCounty = false;
            AddressControl.BindData();
        }

        private void InitProfileControl()
        {
            ProfileControl.AccountType = Interprise.Framework.Base.Shared.Const.LEADS;
            ProfileControl.DefaultSalutationText = _stringResourceService.GetString("leadform.aspx.3");
            ProfileControl.LabelFirstNameText = _stringResourceService.GetString("leadform.aspx.4");
            ProfileControl.LabelLastNameText = _stringResourceService.GetString("leadform.aspx.6");
            ProfileControl.LabelEmailText = _stringResourceService.GetString("leadform.aspx.8");
            ProfileControl.LabelContactNumberText = _stringResourceService.GetString("leadform.aspx.33");
            ProfileControl.LabelEmailAddressText = _stringResourceService.GetString("leadform.aspx.43");
            ProfileControl.LabelEmailText = _stringResourceService.GetString("leadform.aspx.44");
            ProfileControl.LabelMobileNumberText = _stringResourceService.GetString("leadform.aspx.45");

            ProfileControl.BindData();
        }

        protected void btnSaveLead_Click(object sender, EventArgs e)
        {
            try
            {   
                SaveLead();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected void SaveLead()
        {

            bool infoIsAllGood = true;

            if (InterpriseHelper.IsLeadDuplicate(ProfileControl.FirstName, String.Empty, ProfileControl.LastName))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("leadform.aspx.20"));
                infoIsAllGood = false;
            }

            if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(ProfileControl.Email))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("leadform.aspx.46"));
                infoIsAllGood = false;
            }

            if (InterpriseHelper.IsLeadEmailDuplicate(ProfileControl.Email))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("leadform.aspx.29"));
                infoIsAllGood = false;
            }

            if (!infoIsAllGood)
            {
                txtCaptcha.Text = String.Empty;
            }

            if (infoIsAllGood)
            {

                if (IsSecurityCodeGood(txtCaptcha.Text))
                {
                    string bCityStates = txtCityStates.Text;
                    string city = String.Empty;
                    string state = String.Empty;

                    if (bCityStates.IsNullOrEmptyTrimmed())
                    {
                        state = AddressControl.State;
                        city = AddressControl.City;
                    }
                    else
                    {
                        var _cityState = bCityStates.Split(',');
                        state = _cityState.Length > 1 ? _cityState[0].Trim() : String.Empty;
                        city = _cityState.Length > 1 ? _cityState[1].Trim() : _cityState[0].Trim();

                    }

                    var customerInfo = new CustomerInfo();
                    customerInfo.Salutation = (ProfileControl.Salutation == _stringResourceService.GetString("leadform.aspx.3")) ? String.Empty : ProfileControl.Salutation;
                    customerInfo.FirstName = ProfileControl.FirstName;
                    customerInfo.LastName = ProfileControl.LastName;
                    customerInfo.Phone = ProfileControl.ContactNumber;
                    customerInfo.Mobile = ProfileControl.Mobile;
                    customerInfo.Email = ProfileControl.Email;

                    var address = new Address();
                    address.City = city;
                    address.PostalCode = AddressControl.Postal;
                    address.State = state;
                    address.Country = AddressControl.Country;
                    address.County = AppLogic.AppConfigBool("Address.ShowCounty") == true ? AddressControl.County : String.Empty;
                    address.Address1 = AddressControl.Street;
                    address.EMail = ProfileControl.Email;
                    address.Phone = ProfileControl.ContactNumber;

                    InterpriseHelper.CreateNewLead(ThisCustomer.LocaleSetting, txtMessage.Text, customerInfo, address);

                    _navigationService.NavigateToUrl("t-ConnectedBusinessLeadThankYouPage.aspx");
                }
                else
                {
                    errorSummary.DisplayErrorMessage(_stringResourceService.GetString("leadform.aspx.47"));      
                }
            }
        }

        protected bool IsSecurityCodeGood(string code)
        {

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnLeadForm")) return true;

            if (Session["SecurityCode"] != null)
            {

                string sCode = Session["SecurityCode"].ToString();
                string fCode = code;

                if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                {
                    if (fCode.Equals(sCode)) return true;
                }
                else
                {
                    if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            return true;

        }
    }
}