using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.CustomModel;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class rma : SkinBase
{
    #region DomainServices

    IAuthenticationService _authenticationServie = null;
    INavigationService _navigationService = null;
    IOrderService _orderService = null;
    ICryptographyService _cryptoService = null;

    #endregion

    #region Initialize

    protected override void OnInit(EventArgs e)
    {
        InitializeDomainServices();
        PageNoCache();
        PerformPageAccessLogic();
        BindControls();
        InitializeContent();

        base.OnInit(e);
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion

    #region Private Methods

    private void InitializeDomainServices()
    {
        _authenticationServie = ServiceFactory.GetInstance<IAuthenticationService>();
        _navigationService = ServiceFactory.GetInstance<INavigationService>();
        _orderService = ServiceFactory.GetInstance<IOrderService>();
        _cryptoService = ServiceFactory.GetInstance<ICryptographyService>();
    }
    private void PerformPageAccessLogic()
    {
        RequireSecurePage();

        // check if logged in customer
        string returnUrl = CommonLogic.GetThisPageName(false);
        string queryStrings = CommonLogic.ServerVariables("QUERY_STRING");
        if (!queryStrings.IsNullOrEmptyTrimmed()) { returnUrl = returnUrl + "?" + queryStrings; }
        RequiresLogin(returnUrl);
    }
    private void InitializeContent()
    {
        // set string resources
        SectionTitle = AppLogic.GetString("rma.aspx.1");
        btnCreateRMA.Text = AppLogic.GetString("rma.aspx.2");
    }
    private void BindControls()
    {
        btnCreateRMA.Click += (sender, e) => _navigationService.NavigateToCreateRMA();
    }

    #endregion

    #region Public Methods

    public string GetRMAsJSON()
    {
        var rmas = _orderService.GetCustomerRMAs()
                                .OrderByDescending(x => x.RMACode)
                                .Select(x =>
                                {
                                    x.SalesOrderDate = x.RMADate;
                                    //x.RMAStatus = (x.RMAStatus.EqualsIgnoreCase("open")) ? AppLogic.GetString("rma.aspx.8") : AppLogic.GetString("rma.aspx.9"); 
                                    return x;
                                })
                                .ToList();
        return _cryptoService.SerializeToJson<List<CustomerRMACustomModel>>(rmas);
    }

    #endregion
}