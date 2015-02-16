using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterpriseSuiteEcommerceCommon;

/// <summary>
/// Summary description for ResourceProvider
/// </summary>
public static class ResourceProvider
{
    public static PaymentTermControlResource GetPaymentTermControlDefaultResources()
    {
        var resource = new PaymentTermControlResource()
        {
            NameOnCardCaption = AppLogic.GetString("checkoutpayment.aspx.15"),
            NoPaymentRequiredCaption = AppLogic.GetString("checkoutpayment.aspx.8"),
            CardNumberCaption = AppLogic.GetString("checkoutpayment.aspx.16"),
            CVVCaption = AppLogic.GetString("checkoutpayment.aspx.17"),
            WhatIsCaption = AppLogic.GetString("checkoutpayment.aspx.23"),
            CardTypeCaption = AppLogic.GetString("checkoutpayment.aspx.18"),
            CardStartDateCaption = AppLogic.GetString("checkoutpayment.aspx.19"),
            ExpirationDateCaption = AppLogic.GetString("checkoutpayment.aspx.20"),
            CardIssueNumberCaption = AppLogic.GetString("checkoutpayment.aspx.21"),
            CardIssueNumberInfoCaption = AppLogic.GetString("checkoutpayment.aspx.22"),
            SaveCardAsCaption = AppLogic.GetString("checkoutpayment.aspx.13"),
            SaveThisCreditCardInfoCaption = AppLogic.GetString("checkoutpayment.aspx.14"),
            PONumberCaption = AppLogic.GetString("checkoutpayment.aspx.24"),
            ExternalCaption = AppLogic.GetString("checkoutpayment.aspx.25")
        };
        return resource;
    }

    public static PaymentTermControlResource GetMobilePaymentTermControlDefaultResources()
    {
        var resource = new PaymentTermControlResource()
        {
            NameOnCardCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.2"),
            NoPaymentRequiredCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.15"),
            CardNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.3"),
            CVVCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.4"),
            WhatIsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.11"),
            CardTypeCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.5"),
            CardStartDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.6"),
            ExpirationDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.7"),
            CardIssueNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.8"),
            CardIssueNumberInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.9"),
            SaveCardAsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.10"),
            SaveThisCreditCardInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.12"),
            PONumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.13"),
            ExternalCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.14")
        };
        return resource;
    }
}