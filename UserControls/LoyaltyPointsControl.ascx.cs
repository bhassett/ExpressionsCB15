using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon.Extensions;

public partial class UserControls_LoyaltyPointsControl : System.Web.UI.UserControl
{
    #region EventHandler

    public event EventHandler UpdateLoyaltyPoints;

    #endregion

    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        btnUpdateCart.Click += btnUpdateCart_Click;
    }

    protected void btnUpdateCart_Click(object sender, EventArgs e)
    {
        if (UpdateLoyaltyPoints != null) { UpdateLoyaltyPoints(this, EventArgs.Empty); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion

    #region Events

    #endregion

    #region Properties

    public string Title
    {
        get
        {
            string title = String.Empty;
            if (ViewState["Title"] != null) { title = ViewState["Title"].ToString(); }
            return title;
        }
        set { ViewState["Title"] = value; }
    }

    public string PointsCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["PointsCaption"] != null) { caption = ViewState["PointsCaption"].ToString(); }
            return caption;
        }
        set { ViewState["PointsCaption"] = value; }
    }

    public string PointsValueCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["PointsValueCaption"] != null) { caption = ViewState["PointsValueCaption"].ToString(); }
            return caption;
        }
        set { ViewState["PointsValueCaption"] = value; }
    }

    public string RedeemCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["RedeemCaption"] != null) { caption = ViewState["RedeemCaption"].ToString(); }
            return caption;
        }
        set { ViewState["RedeemCaption"] = value; }
    }

    public string UpdateCartButtonCaption
    {
        get { return btnUpdateCart.Text; }
        set { btnUpdateCart.Text = value; }
    }

    public string ValidationMsgForInvalidAmount
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["ValidationMsgForInvalidAmount"] != null) { caption = ViewState["ValidationMsgForInvalidAmount"].ToString(); }
            return caption;
        }
        set { ViewState["ValidationMsgForInvalidAmount"] = value; }
    }

    public string LoyaltyPoints
    {
        get { return lblPoints.Text; }
        set { lblPoints.Text = value; }
    }

    public decimal MonetaryValue { get; set; }

    public string MonetaryValueFormatted
    {
        get { return lblMonetaryValue.Text; }
        set { lblMonetaryValue.Text = value; }
    }

    public string LoyaltyPointsToBeApplied
    {
        get { return hidLoyaltyPoints.Value; }
        set { hidLoyaltyPoints.Value = value; }
    }

    public string CurrencySymbol
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["CurrencySymbol"] != null) { caption = ViewState["CurrencySymbol"].ToString(); }
            return caption;
        }
        set { ViewState["CurrencySymbol"] = value; }
    }

    public decimal RedemptionMultipler { get; set; }

    public string SystemCurrencyJSON { get; set; }

    #endregion
}