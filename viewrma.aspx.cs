using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class viewrma : SkinBase
{
    #region DomainServices

    IAuthenticationService _authenticationServie = null;
    INavigationService _navigationService = null;
    IOrderService _orderService = null;
    ICryptographyService _cryptographyService = null;

    #endregion

    #region Properties

    public CustomerRMACustomModel CurrentRMA = null;
    public List<CustomerRMAItemCustomModel> CurrentRMAItems = null;

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

    #region Method

    private void BindControls()
    {
        btnBack.Click += (sender, ex) => _navigationService.NavigateToRMA();
    }
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
    private void InitializeContent()
    {
        SectionTitle = AppLogic.GetString("viewrma.aspx.1");
        btnBack.Text = AppLogic.GetString("viewrma.aspx.2");

        LoadRMA();
    }
    private void LoadRMA()
    {
        string rmaCode = CommonLogic.QueryStringCanBeDangerousContent("rmacode");
        if(!rmaCode.IsNullOrEmptyTrimmed())
        {
            CurrentRMA = _orderService.GetCustomerRMA(rmaCode);
            CurrentRMAItems = _orderService.GetCustomerRMAItems(rmaCode).ToList();

            if(CurrentRMA == null)
            {
                _navigationService.NavigateToRMA(NotificationStatus.Error, "Invalid RMA");
            }
        }
    }

    public string GetAlternateStatus(string rmaStatus)
    {
        if (rmaStatus.EqualsIgnoreCase("open"))
        {
            return AppLogic.GetString("rma.aspx.8");
        }
        return AppLogic.GetString("rma.aspx.9");
    }

    #endregion
}