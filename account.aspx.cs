// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interprise.Facade.Customer;
using Interprise.Framework.Customer.DatasetGateway;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for account.    
    /// </summary>
    public partial class account : SkinBase
    {
        #region Private Members

        bool AccountUpdated = false;
        bool Checkout = false;
        public CultureInfo SqlServerCulture = new System.Globalization.CultureInfo(CommonLogic.Application("DBSQLServerLocaleSetting")); // qualification needed for vb.net (not sure why)
        public string m_StoreLoc = AppLogic.GetStoreHTTPLocation(true);
        string SkinImagePath = string.Empty;

        #endregion

        #region Domain Services

        IAppConfigService _appConfigService = null;
        IStringResourceService _stringResourceService = null;
        ILocalizationService _localizationService = null;
        ICustomerService _customerService = null;

        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitializeDomainServices();

            CheckFirefoxChecking();

            RequireCustomerRecord();
            PerformPageAccessLogic();

            InitializeControls();

            InitializeLoyaltyPoints();
            InitializeGiftCodes();
            InitializeCreditMemos();

            ProfileHelpfulTips.SetContext = this;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            RefreshPage();

            StringBuilder script = new StringBuilder();

            script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "account.aspx.26", AppLogic.GetString("account.aspx.26", true));
            script.AppendFormat("   ise.Pages.Account.setForm('{0}');\n", this.AccountForm.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            this.HeaderMsg.SetContext = this;
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        public void btnContinueToCheckOut_Click(object sender, EventArgs e)
        {
            Response.Redirect("checkoutshipping.aspx");
        }

        public void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) { return; }

            ThisCustomer.Update();
            AccountUpdated = true;

            RefreshPage();
        }

        #endregion

        #region Private Methods

        private void InitProfileControl()
        {

            ProfileControl.AccountType = Interprise.Framework.Base.Shared.Const.CUSTOMER;

            ProfileControl.IsShowEditPasswordArea = true;
            ProfileControl.IsHideAccountNameArea = true;

            ProfileControl.LabelFirstNameText = _stringResourceService.GetString("account.aspx.55");
            ProfileControl.LabelLastNameText = _stringResourceService.GetString("account.aspx.56");
            ProfileControl.LabelContactNumberText = _stringResourceService.GetString("account.aspx.58");

            ProfileControl.LabelOldPasswordText = _stringResourceService.GetString("account.aspx.35");
            ProfileControl.LabelNewPasswordText = _stringResourceService.GetString("account.aspx.41");
            ProfileControl.LabeEditPasswordText = _stringResourceService.GetString("account.aspx.34");
            ProfileControl.LabelCurrentPasswordText = _stringResourceService.GetString("account.aspx.36");

            ProfileControl.LabelPasswordText = _stringResourceService.GetString("account.aspx.59");
            ProfileControl.LabelConfirmPasswordText = _stringResourceService.GetString("account.aspx.60");
            ProfileControl.DefaultSalutationText = _stringResourceService.GetString("account.aspx.61");

            ProfileControl.LabelEmailAddressText = _stringResourceService.GetString("account.aspx.87");
            ProfileControl.LabelEmailText = _stringResourceService.GetString("account.aspx.88");
            ProfileControl.LabelMobileNumberText = _stringResourceService.GetString("account.aspx.89");

            ProfileControl.FirstName = ThisCustomer.FirstName;
            ProfileControl.LastName = ThisCustomer.LastName;
            ProfileControl.Email = ThisCustomer.EMail;
            ProfileControl.ContactNumber = ThisCustomer.Phone;
            ProfileControl.Mobile = ThisCustomer.Mobile;
            ProfileControl.SelectedSalutation = ThisCustomer.Salutation;
            

            var address = ThisCustomer.BillingAddresses;

            chkIsOkToEmail.Checked = ThisCustomer.OKToEMail;
            chkIsOver13Checked.Checked = ThisCustomer.IsOver13;

            ProfileControl.BindData();

        }
        private void CheckFirefoxChecking()
        {
            if (!Request.UserAgent.Contains("Mozilla")) return;

            var outputCacheSettings = new OutputCacheParameters()
            {
                Duration = 0,
                Location = OutputCacheLocation.None,
                NoStore = true
            };

            InitOutputCache(outputCacheSettings);
        }

        private void PerformPageAccessLogic()
        {

            SectionTitle = AppLogic.GetString("account.aspx.42", true);

            RequireSecurePage();
            RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));


            //If current user came from IS, chances are it has no Primary Billing Info!
            if (ThisCustomer.PrimaryBillingAddressID == String.Empty)
            {

                // IS Facade implementation
                string[][] commandSet;
                string[][] parameters;

                var customerGateway = new CustomerDetailDatasetGateway();
                var iseCustomer = new CustomerDetailFacade(customerGateway);

                // Specific view and stored procedure
                commandSet = new string[][] { new string[] { "CustomerView", "ReadCustomer" } };
                parameters = new string[][] { new string[] { "@CustomerCode", null } };

                // Load it at defined Dataset
                iseCustomer.LoadDataSet(commandSet, parameters, Interprise.Framework.Base.Shared.Enum.ClearType.Specific, Interprise.Framework.Base.Shared.Enum.ConnectionStringType.Online);

                // Retrieve data information, store it at DataRow array
                // Filter the data information based on the Customer Code
                DataRow[] rowsCustomer = customerGateway.Tables["CustomerView"].Select(string.Concat("CustomerCode = '", ThisCustomer.CustomerCode, "'"));

                // It should return 1 record from filtered dataset.
                if (rowsCustomer.Length > 0)
                {
                    // By Default 
                    string ResidenceType = "Residential";

                    // Address Class to store address information 
                    // retrieve via DataRow array
                    var thisAddress = new Address()
                    {
                        CustomerCode = ThisCustomer.CustomerCode,
                        Name = rowsCustomer[0]["CustomerName"].ToString(),
                        Address1 = rowsCustomer[0]["Address"].ToString(),
                        City = rowsCustomer[0]["City"].ToString(),
                        State = rowsCustomer[0]["State"].ToString(),
                        PostalCode = rowsCustomer[0]["PostalCode"].ToString(),
                        Country = rowsCustomer[0]["Country"].ToString(),
                        Phone = rowsCustomer[0]["Telephone"].ToString(),
                        County = rowsCustomer[0]["County"].ToString()
                    };

                    // set default if null
                    if (!string.IsNullOrEmpty(rowsCustomer[0]["ResidenceType"].ToString()))
                    {
                        ResidenceType = rowsCustomer[0]["ResidenceType"].ToString();
                    }

                    thisAddress.ResidenceType = (ResidenceTypes)Enum.Parse(typeof(ResidenceTypes), ResidenceType);

                    // Adding of Customer Billing Address Information
                    InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, true);
                }
                else
                {
                    // If no Record is found
                    // Redirects to address information entry
                    Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
                }

            }

            ThisCustomer.ValidatePrimaryAddresses();
            SkinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/account_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private void InitializeDomainServices()
        {
            _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
            _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
            _localizationService = ServiceFactory.GetInstance<ILocalizationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
        }

        private void InitializeControls()
        {
            InitProfileControl();
        }

        #endregion

        #region Public Methods

        public void RefreshPage()
        {
            Address BillingAddress = ThisCustomer.PrimaryBillingAddress;
            Address ShippingAddress = ThisCustomer.PrimaryShippingAddress;

            Checkout = CommonLogic.QueryStringBool("checkout");

            if (Checkout)
            {
                pnlCheckoutImage.Visible = true;
                CheckoutImage.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "step_2.gif");
                if (ThisCustomer.PrimaryBillingAddressID == string.Empty || ThisCustomer.PrimaryShippingAddressID == string.Empty || !ThisCustomer.HasAtLeastOneAddress())
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("account.aspx.33", true) + "&nbsp;&nbsp;";
                }
            }

            string XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            pnlCheckoutImage.Visible = Checkout;
            unknownerrormsg.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("unknownerror"));
            ErrorMsgLabel.Text += Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));
            pnlAccountUpdated.Visible = AccountUpdated;
            if (AccountUpdated)
            {
                lblAcctUpdateMsg.Text = AppLogic.GetString("account.aspx.1", true);
            }
            else
            {
                lblAcctUpdateMsg.Text = AppLogic.GetString("account.aspx.2", true);
            }

            pnlNotCheckOutButtons.Visible = !Checkout;
            pnlShowWishButton.Visible = AppLogic.AppConfigBool("ShowWishListButton");
            btnContinueToCheckOut.Visible = Checkout;

           // lnkChangeBilling.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "change.gif");
            lnkChangeBilling.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=billing&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";

            litBillingAddress.Text = BillingAddress.DisplayHTML(Checkout);

            if (!AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                pnlShipping.Visible = false;
            }
            else
            {

                lnkChangeShipping.Text = _stringResourceService.GetString("account.aspx.91");
                lnkChangeShipping.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=shipping&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";

                lnkAddShippingAddress.NavigateUrl = "selectaddress.aspx?add=true&addressType=Shipping&Checkout=" + Checkout.ToString() + "&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString());
                litShippingAddress.Text = ShippingAddress.DisplayHTML(Checkout);
            }

            if (BillingAddress.PaymentMethod.Length == 0)
            {
                litBillingAddress.Text += "<b>" + AppLogic.GetString("account.aspx.9", true) + "</b><br/>";
                litBillingAddress.Text += BillingAddress.DisplayPaymentMethod(ThisCustomer);
            }

            // btnOrderHistory.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "orderhistory.gif");
            accountOrderHistoryLink.Visible = true;
        }

        public string GetGiftCodesJSON()
        {
            return ServiceFactory.GetInstance<ICustomerService>()
                                 .GetCustomerGiftCodes()
                                 .ToList()
                                 .ToJSON();
        }

        private void InitializeLoyaltyPoints()
        {
            pnlLoyaltyPoints.Visible = _appConfigService.LoyaltyPointsEnabled;

            var currencyService = ServiceFactory.GetInstance<ICurrencyService>();
            var customerLoyaltyPoints = ServiceFactory.GetInstance<ICustomerService>().GetLoyaltyPoints();
            decimal loyaltyPoints = Decimal.Zero;
            decimal loyaltyPointsMonetized = Decimal.Zero;

            if (customerLoyaltyPoints != null)
            {
                loyaltyPoints = customerLoyaltyPoints.RemainingPoints;
                loyaltyPointsMonetized = customerLoyaltyPoints.MonetizedRemainingPoints.ToCustomerRoundedMonetary();
            }

            lblPoints.Text = _localizationService.FormatDecimalToNumber(loyaltyPoints, 0);
            lblMonetaryValue.Text = currencyService.FormatCurrency(loyaltyPointsMonetized);
        }

        private void InitializeCreditMemos()
        {
            pnlCreditMemos.Visible = _appConfigService.CreditRedemptionIsEnabled;
            if (_appConfigService.CreditRedemptionIsEnabled)
            {
                creditMemoList.CreditCodeHeader = AppLogic.GetString("account.aspx.83");
                creditMemoList.BalanceHeader = AppLogic.GetString("account.aspx.84");
                creditMemoList.ViewCreditsCaption = AppLogic.GetString("account.aspx.85");
                creditMemoList.NotFoundMessage = AppLogic.GetString("account.aspx.86");

                var creditMemosApplied = _customerService.GetCustomerCreditMemosWithRemainingBalance().ToList();
                if (creditMemosApplied.Count > 0) 
                {
                    creditMemoList.CreditMemosJSON = creditMemosApplied.ToJSON();
                }
            }
        }

        private void InitializeGiftCodes()
        {
            pnlGiftCodes.Visible = _appConfigService.GiftCodeEnabled;
        }

        #endregion
    }

}