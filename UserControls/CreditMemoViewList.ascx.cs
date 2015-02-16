using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_CreditMemoViewList : System.Web.UI.UserControl
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

    public string CreditCodeHeader { get; set; }
    public string BalanceHeader { get; set; }
    public string ViewCreditsCaption { get; set; }
    public string CreditMemosJSON { get; set; }
    public string NotFoundMessage { get; set; }

    #endregion
}