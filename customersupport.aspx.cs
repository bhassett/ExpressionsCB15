using System;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.Xml.Linq;

namespace InterpriseSuiteEcommerce
{
    public class Reason
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public partial class CustomerSupport : SkinBase
    {
        #region Variable Declaration

        ICustomerService _customerService = null;
        IStringResourceService _stringResourceService = null;
        INavigationService _navigationService = null;
        IAppConfigService _appconfigService = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            InitAddressControl();
            BindDropReasonData();
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
            _appconfigService = ServiceFactory.GetInstance<IAppConfigService>();
        }

        private void InitPageContent()
        {
            SectionTitle = _stringResourceService.GetString("customersupport.aspx.49", true);
            CaseFormHelpFulTipsTopic.SetContext = this;

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCaseForm")) pnlSecurityCode.Visible = false;

        }

        private void InitAddressControl()
        {

            AddressControl.LabelStreetText = _stringResourceService.GetString("customersupport.aspx.8");
            AddressControl.LabelCityText = _stringResourceService.GetString("customersupport.aspx.50");
            AddressControl.LabelStateText = _stringResourceService.GetString("customersupport.aspx.51");
            AddressControl.LabelPostalText = _stringResourceService.GetString("customersupport.aspx.52");
            AddressControl.LabelEnterPostalText = _stringResourceService.GetString("customersupport.aspx.40");

            if (ThisCustomer.IsRegistered)
            {
                var address = ThisCustomer.PrimaryBillingAddress;
                AddressControl.Street = address.Address1;
                AddressControl.SelectedCountry = address.Country;
                AddressControl.Postal = address.PostalCode;
                AddressControl.City = address.City;

                if (!address.State.IsNullOrEmptyTrimmed())
                {
                    AddressControl.State = address.State;
                }

                txtContactName.Text = "{0} {1}".FormatWith(ThisCustomer.FirstName, ThisCustomer.LastName);
                txtContactNumber.Text = ThisCustomer.Phone;
                txtEmail.Text = ThisCustomer.EMail;

            }
            else
            {
                litMenuCaseHistory.Text = String.Empty;
            }

            AddressControl.IsShowCounty = false;
            AddressControl.BindData();
        }

        private void BindDropReasonData()
        {
            //List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
            //for (int i = 0; i < 5; i++)
            //{
            //       collection.Add(new KeyValuePair<string, string>("Key-" + i.ToString(), "Value-" + i.ToString()));
            //}
            
            //dropReason.DataTextField = "Key";
            //dropReason.DataValueField = "Value";
            //dropReason.DataSource = collection;
            //dropReason.DataBind();
        }

        protected void btnSendCaseForm_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateCaseForm();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected void ValidateCaseForm()
        {
  
            if (!Interprise.Framework.Base.Shared.Common.IsValidEmail(txtEmail.Text))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("customersupport.aspx.16"));
                txtCaptcha.Text = String.Empty;
                return;
            }

            if (!IsSecurityCodeGood(txtCaptcha.Text))
            {
                errorSummary.DisplayErrorMessage(_stringResourceService.GetString("customersupport.aspx.55", true));
               return;   
            }
               
            SubmitCaseForm();
            
        }

        protected void SubmitCaseForm()
        {
            string bCityStates = txtCityStates.Text;
            string city = string.Empty;
            string state = string.Empty;

            if (!string.IsNullOrEmpty(bCityStates))
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

            InterpriseSuiteEcommerceCommon.DTO.CustomerCase customerCase = new InterpriseSuiteEcommerceCommon.DTO.CustomerCase();
            customerCase.ContactName = txtContactName.Text;
            customerCase.EmailAddress = txtEmail.Text;
            customerCase.ContactNo = txtContactNumber.Text;
            customerCase.Country = AddressControl.Country;
            customerCase.State = state;
            customerCase.PostalCode = AddressControl.Postal;
            customerCase.City = city;
            customerCase.Subject = txtSubject.Text;
            customerCase.Address = AddressControl.Street;
            customerCase.Problem = txtCaseDetails.Text;
            //customerCase.ItemName = txtItemName.Text;
            //customerCase.Reason = dropReason.SelectedValue;
            
            if (AppLogic.AppConfigBool("Address.ShowCounty"))
                customerCase.County =  AddressControl.County;
            
            string msg = InterpriseHelper.SaveCaseForm(customerCase);

            if (msg == "True")
            {
                string caseSupportEmailTo = _appconfigService.CustomerSupportEmailTo;
                if (!caseSupportEmailTo.IsNullOrEmptyTrimmed())
                {
                    string[] emailacctinfo = InterpriseHelper.GetStoreEmailAccountInfo();

                    string emailSubject = String.Format(_stringResourceService.GetString("customersupport.aspx.54", true), _appconfigService.StoreName);

                    var param = new XElement(DomainConstants.XML_ROOT_NAME);
                    param.Add(new XElement("MAIL_SUBJECT", txtSubject.Text));
                    param.Add(new XElement("MAIL_BODY", txtCaseDetails.Text));
                    param.Add(new XElement("MAIL_CONTACTNAME", txtContactName.Text));
                    param.Add(new XElement("MAIL_EMAIL", txtEmail.Text));
                    param.Add(new XElement("MAIL_CONTACTNUMBER", txtContactNumber.Text));

                    var package = new XmlPackage2("notification.newcasenotificationadmin.xml.config", param);
                    string message = package.TransformString();

                    AppLogic.SendMailRequest(emailSubject, message, true, emailacctinfo[0], emailacctinfo[1], caseSupportEmailTo, _appconfigService.CustomerSupportEmailToName, String.Empty);
                }

                Response.Redirect("t-CaseFormThankYouPage.aspx");
            }
            else
            {
                errorSummary.DisplayErrorMessage(msg);
            }
        }
        protected bool IsSecurityCodeGood(string code)
        {

           if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCaseForm")) return true;

           if (Session["SecurityCode"] != null){

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