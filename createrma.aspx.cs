using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterpriseSuiteEcommerce
{
    public partial class createrma : SkinBase
    {
        #region DomainServices

        IAuthenticationService _authenticationServie = null;
        INavigationService _navigationService = null;
        IOrderService _orderService = null;
        ICryptographyService _cryptographyService = null;

        #endregion

        #region Private Members

        private InterpriseShoppingCart cart = null;

        #endregion

        #region Properties

        public CustomerInvoiceCustomModel CurrentInvoice = null;

        #endregion

        #region Initialize

        protected override void OnInit(EventArgs e)
        {
            InitializeDomainServices();
            PerformPageAccessLogic();
            PageNoCache();
            RequireSecurePage();
            BindControls();
            InitializeContent();
            
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion

        #region Private Method

        private void InitializeDomainServices()
        {
            _authenticationServie = ServiceFactory.GetInstance<IAuthenticationService>();
            _navigationService = ServiceFactory.GetInstance<INavigationService>();
            _orderService = ServiceFactory.GetInstance<IOrderService>();
            _cryptographyService = ServiceFactory.GetInstance<ICryptographyService>();
        }
        private void PerformPageAccessLogic()
        {
            // check if logged in customer
            string returnUrl = CommonLogic.GetThisPageName(false);
            string queryStrings = CommonLogic.ServerVariables("QUERY_STRING");
            if (!queryStrings.IsNullOrEmptyTrimmed()) { returnUrl = returnUrl + "?" + queryStrings; }
            RequiresLogin(returnUrl);
        }
        private void BindControls()
        {
            btnSubmit.Click += (sender, ex) => SaveRMA();
            btnCancel.Click += (sender, ex) => _navigationService.NavigateToRMA();
            btnCancel2.Click += (sender, ex) => _navigationService.NavigateToRMA();
            btnBackToOrderSelection.Click += (sender, ex) => _navigationService.NavigateToCreateRMA();
        }
        private void InitializeContent()
        {
            // set string resources
            SectionTitle = AppLogic.GetString("createrma.aspx.1");
            btnSubmit.Text = AppLogic.GetString("createrma.aspx.13");
            btnCancel.Text = AppLogic.GetString("createrma.aspx.14");
            btnCancel2.Text = AppLogic.GetString("createrma.aspx.26");
            btnBackToOrderSelection.Text = AppLogic.GetString("createrma.aspx.25");

            // load request type, invoice detail, invoices
            LoadRequestType();
            LoadCustomerInvoice();

            // initialize shopping cart
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
        }
        private void LoadCustomerInvoice()
        {
            string invoiceCode = CommonLogic.QueryStringCanBeDangerousContent("invoicecode");
            if(!invoiceCode.IsNullOrEmptyTrimmed())
            {
                CurrentInvoice = _orderService.GetCustomerInvoice(invoiceCode);
            }
        }

        private void LoadRequestType()
        {
            var requestTypes = AppLogic.AppConfig("RMARequestTypes")
                                            .Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Trim());

            ddlRequestType.DataSource = requestTypes;
            ddlRequestType.DataBind();
        }
        private void SaveRMA()
        {
            string invoiceCode = CommonLogic.QueryStringCanBeDangerousContent("invoicecode");
            string rmaNotes = GetRMANotes();
            var rmaItems = GetRMAItems().ToList();

            if (!invoiceCode.IsNullOrEmptyTrimmed() && rmaItems.Count > 0)
            {
                bool isTrimmed = false;
                string errorMessage = String.Empty;
                string message = String.Empty;

                if (cart.CreateNewRMA(invoiceCode, rmaItems, rmaNotes, ref errorMessage, ref isTrimmed))
                {
                    message = AppLogic.GetString("createrma.aspx.18");

                    if(isTrimmed)
                    {
                        message += AppLogic.GetString("createrma.aspx.20");
                        _navigationService.NavigateToRMA(NotificationStatus.Warning, message);
                    }
                    _navigationService.NavigateToRMA(NotificationStatus.Success, message);
                }
                else
                {
                    if(errorMessage == DomainConstants.RMA_NOITEM_AVAILABLE_ERROR)
                    {
                        message = AppLogic.GetString("createrma.aspx.19");
                    }
                    _navigationService.NavigateToRMA(NotificationStatus.Error, message);
                }
            }
        }
        private string GetRMANotes()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{0} {1}".FormatWith(AppLogic.GetString("createrma.aspx.11"), ddlRequestType.SelectedValue));
            sb.AppendLine("{0} {1}".FormatWith(AppLogic.GetString("createrma.aspx.12"), txtReason.Text));
            return sb.ToString();
        }
        private IEnumerable<CustomerInvoiceItemCustomModel> GetRMAItems()
        {
            string json = hdnInvoiceItems.Value;
            if (!json.IsNullOrEmptyTrimmed())
            {
                return _cryptographyService.DeserializeJson<List<CustomerInvoiceItemCustomModel>>(json);
            }
            return null;
        }

        #endregion

        #region Public Methods

        public string GetCustomerInvoiceItemsJSON()
        {
            string json = String.Empty;
            string invoiceCode = CommonLogic.QueryStringCanBeDangerousContent("invoicecode");
            if (!invoiceCode.IsNullOrEmptyTrimmed())
            {
                var items = _orderService.GetCustomerInvoiceItems(invoiceCode).OrderBy(x => x.LineNum).ToList();
                json = _cryptographyService.SerializeToJson<List<CustomerInvoiceItemCustomModel>>(items);
            }
            return json;
        }
        public string GetCustomerInvoicesJSON()
        {
            string json = String.Empty;
            string invoiceCode = CommonLogic.QueryStringCanBeDangerousContent("invoicecode");
            if (invoiceCode.IsNullOrEmptyTrimmed())
            {
                var invoices = _orderService.GetCustomerInvoices().OrderByDescending(x => x.SalesOrderCode).ToList();

                if (invoices.Count == 0)
                {
                    // no orders found
                    _navigationService.NavigateToRMA(NotificationStatus.Error, AppLogic.GetString("createrma.aspx.27"));
                }

                json = _cryptographyService.SerializeToJson<List<CustomerInvoiceCustomModel>>(invoices);
            }
            return json;
        }

        #endregion
    }
}