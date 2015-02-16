using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_OtherPaymentOptionControl : System.Web.UI.UserControl
{
    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion

    #region Methods

    #endregion

    #region Properties

    public string Header { get; set; }
    public string HeaderCreditMemo { get; set; }
    public string HeaderLoyaltyPoints { get; set; }
    public string HeaderGiftCode { get; set; }
    public string HeaderBalanceAvailable { get; set; }
    public string HeaderApplyAmount { get; set; }
    public string CurrencySymbol { get; set; }
    public string ButtonApplyCaption { get; set; }
    public string PointsEarnedText { get; set; }
    public string ButtonAddGiftCodeTooltip { get; set; }
    public string ButtonSaveGiftCodeTooltip { get; set; }
    public string ButtonCancelGiftCodeTooltip { get; set; }
    public string GiftCodeText { get; set; }
    public string ValidationGiftCodeEmpty { get; set; }
    public string ValidationGiftCodeZeroBalance { get; set; }
    public string ValidationGiftCodeInvalid { get; set; }
    public string LoaderText { get; set; }


    public string CreditMemosJSON { get; set; }
    public string CreditMemosAppliedJSON { get; set; }

    public string LoyaltyPointsJSON { get; set; }
    public decimal LoyaltyPointsApplied { get; set; }

    public string GiftCodesJSON { get; set; }
    public string GiftCodesAppliedJSON { get; set; }

    public bool IsCreditMemoEnabled { get; set; }
    public bool IsLoyaltyPointsEnabled { get; set; }
    public bool IsGiftCodeEnabled { get; set; }

    public decimal RedemptionMultiplier { get; set; }

    #endregion
}