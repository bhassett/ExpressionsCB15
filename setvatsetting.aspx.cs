// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for setvatsetting.
    /// </summary>
    public partial class setvatsetting : System.Web.UI.Page
    {
        #region Private Members

        private Customer _customer = null;

        #endregion

        #region DomainServices

        IShoppingCartService _shoppingCartService = null;
        INavigationService _navigationService = null;
        IAuthenticationService _authenticationService = null;
        ICustomerService _customerService = null;

        #endregion 

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            SetCacheability();

            InitializeDomainServices();
            InitializeCustomer();
            
            RequireCustomerRecord();

            DoVATSettingChecking();
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region Methods

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }
        private void InitializeDomainServices()
        {
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
            _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
            _customerService = ServiceFactory.GetInstance<ICustomerService>();
        }
        private void InitializeCustomer()
        {
            var customer = _authenticationService.GetCurrentLoggedInCustomer();
            _customer = customer;
        }
        private void RequireCustomerRecord()
        {
            if (!_customer.HasCustomerRecord)
            {
                _customer.RequireCustomerRecord();
            }
        }
        private void DoVATSettingChecking()
        {
            string vatSettingKey = Customer.ValidateVATSetting(CommonLogic.QueryStringCanBeDangerousContent(DomainConstants.QUERY_STRING_KEY_VATSETTING));
            var vatSetting = (VatDefaultSetting)Enum.Parse(typeof(VatDefaultSetting), vatSettingKey);

            if (_customerService.HasChangedVATSetting(vatSetting))
            {
                _customer.VATSettingRaw = vatSetting;
                _shoppingCartService.DoRecomputeCartItemsPrice();
            }

            string message = (vatSetting == VatDefaultSetting.Inclusive) ? AppLogic.GetString("setvatsetting.aspx.2") : AppLogic.GetString("setvatsetting.aspx.3");
            lblMessage.Text = AppLogic.GetString("setvatsetting.aspx.1").FormatWith(message);

            string returnUrl = CommonLogic.QueryStringCanBeDangerousContent(DomainConstants.QUERY_STRING_KEY_RETURNURL);
            
            AppLogic.CheckForScriptTag(returnUrl);
            
            if (returnUrl.Contains("setvatsetting.aspx")) { returnUrl = string.Empty; }
            if (returnUrl.IsNullOrEmptyTrimmed()) { returnUrl = "default.aspx"; }
            
            Response.AddHeader("REFRESH", "1; URL={0}".FormatWith(Security.UrlDecode(returnUrl)));
        }

        #endregion
    }
}

