using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.Linq;

public partial class UserControls_GiftCodeControl : System.Web.UI.UserControl
{
    #region EventHandler

    public event EventHandler UpdateGiftCodes;

    #endregion

    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        btnUpdateCart.Click += btnUpdateCart_Click;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion

    #region Events 
    
    protected void btnUpdateCart_Click(object sender, EventArgs e)
    {
        if (UpdateGiftCodes != null) { UpdateGiftCodes(this, EventArgs.Empty); }
    }

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

    public string SerialCodeInputCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["SerialCodeInputCaption"] != null) { caption = ViewState["SerialCodeInputCaption"].ToString(); }
            return caption;
        }
        set { ViewState["SerialCodeInputCaption"] = value; }
    }

    public string UpdateCartButtonCaption
    {
        get { return btnUpdateCart.Text; }
        set { btnUpdateCart.Text = value; }
    }

    public string NewSerialCodeButtonCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["NewSerialCodeButtonCaption"] != null) { caption = ViewState["NewSerialCodeButtonCaption"].ToString(); }
            return caption;
        }
        set { ViewState["NewSerialCodeButtonCaption"] = value; }
    }

    public string ValidatingCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["ValidatingCaption"] != null) { caption = ViewState["ValidatingCaption"].ToString(); }
            return caption;
        }
        set { ViewState["ValidatingCaption"] = value; }
    }

    public string RemainingBalanceCaption
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["RemainingBalanceCaption"] != null) { caption = ViewState["RemainingBalanceCaption"].ToString(); }
            return caption;
        }
        set { ViewState["RemainingBalanceCaption"] = value; }
    }

    public string[] GiftCodes 
    { 
        get 
        {
            string serialCodes = hidGiftSerialCodes.Value;
            if (!serialCodes.IsNullOrEmptyTrimmed())
            {
                string[] codes = serialCodes.Split(new char[] { DomainConstants.COMMA_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
                return codes;
            }
            else { return null; }
        } 
    }

    public string Note
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["Note"] != null) { caption = ViewState["Note"].ToString(); }
            return caption;
        }
        set { ViewState["Note"] = value; }
    }

    public string ValidationMsgForEmpty
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["ValidationMsgForEmpty"] != null) { caption = ViewState["ValidationMsgForEmpty"].ToString(); }
            return caption;
        }
        set { ViewState["ValidationMsgForEmpty"] = value; }
    }

    public string ValidationMsgForInvalid
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["ValidationMsgForInvalid"] != null) { caption = ViewState["ValidationMsgForInvalid"].ToString(); }
            return caption;
        }
        set { ViewState["ValidationMsgForInvalid"] = value; }
    }

    public string ValidationMsgForZeroBalance
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["ValidationMsgForZeroBalance"] != null) { caption = ViewState["ValidationMsgForZeroBalance"].ToString(); }
            return caption;
        }
        set { ViewState["ValidationMsgForZeroBalance"] = value; }
    }

    public string TableHeaderCode
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["TableHeaderCode"] != null) { caption = ViewState["TableHeaderCode"].ToString(); }
            return caption;
        }
        set { ViewState["TableHeaderCode"] = value; }
    }

    public string TableHeaderBalance
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["TableHeaderBalance"] != null) { caption = ViewState["TableHeaderBalance"].ToString(); }
            return caption;
        }
        set { ViewState["TableHeaderBalance"] = value; }
    }

    public string TableHeaderType
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["TableHeaderType"] != null) { caption = ViewState["TableHeaderType"].ToString(); }
            return caption;
        }
        set { ViewState["TableHeaderType"] = value; }
    }

    public string GiftCardsOnCartHelpfulTips
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["GiftCardsOnCartHelpfulTips"] != null) { caption = ViewState["GiftCardsOnCartHelpfulTips"].ToString(); }
            return caption;
        }
        set { ViewState["GiftCardsOnCartHelpfulTips"] = value; }
    }

    public string GiftCardsOnCartTitleHideTips
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["GiftCardsOnCartTitleHideTips"] != null) { caption = ViewState["GiftCardsOnCartTitleHideTips"].ToString(); }
            return caption;
        }
        set { ViewState["GiftCardsOnCartTitleHideTips"] = value; }
    }

    public string GiftCardsOnCartTitleShowTips
    {
        get
        {
            string caption = String.Empty;
            if (ViewState["GiftCardsOnCartTitleShowTips"] != null) { caption = ViewState["GiftCardsOnCartTitleShowTips"].ToString(); }
            return caption;
        }
        set { ViewState["GiftCardsOnCartTitleShowTips"] = value; }
    }
    #endregion
}