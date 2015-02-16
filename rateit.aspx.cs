// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.Data.SqlClient;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for rateit.
	/// </summary>
	public partial class rateit : System.Web.UI.Page
	{
        int ProductID;
        String ItemCode;
        String ReturnURL = String.Empty;
        int TheirCurrentRating = 0;
        String TheirCurrentComment = String.Empty;
        bool Editing = false;
        bool HasBadWords = false;
        Customer ThisCustomer;
        int _SkinID = 1;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();
            _SkinID = CommonLogic.CookieUSInt(SkinBase.ro_SkinCookieName);
            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            ItemCode = InterpriseHelper.GetInventoryItemCode(ProductID);
            bool? nopostback = "NoPostback".ToQueryStringDecode().TryParseBool();

            NoPostback = nopostback.HasValue ? nopostback.Value : false;

            String ProductName = HttpContext.Current.Server.HtmlEncode(AppLogic.GetProductName(ProductID.ToString(), ThisCustomer.LocaleSetting));
            String ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");

           
                if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader rs = DB.GetRSFormat(con, String.Format("SELECT * FROM EcommerceRating with (NOLOCK) WHERE CustomerCode={0} AND ItemCode={1} AND WebsiteCode={2} AND ContactCode={3}",
                                                                                    DB.SQuote(ThisCustomer.CustomerCode), DB.SQuote(ItemCode), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode), DB.SQuote(ThisCustomer.ContactCode))))
                    {
                        if (rs.Read())
                        {
                            TheirCurrentRating = DB.RSFieldInt(rs, "Rating");
                            TheirCurrentComment = DB.RSField(rs, "Comments").ToHtmlDecode();
                            Editing = true;
                        }
                    }
                }

                if (!IsPostBack)
                {
                    InitializePageContent();
                }

            }
        
		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {

            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.6"), "0"));
            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.7"), "1"));
            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.8"), "2"));
            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.9"), "3"));
            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.10"), "4"));
            rating.Items.Add(new ListItem(AppLogic.GetString("rateit.aspx.11"), "5"));

            InitializeComponent();
            base.OnInit(e);
        }
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
		}

		#endregion

        void btnSubmit_Click(object sender, EventArgs e)
        {
			var sql = new StringBuilder(2500);
			string theCmts = CommonLogic.Left(Comments.Text.ToHtmlEncode(),5000);
			string theRating = rating.SelectedValue;

            ThisCustomer.ThisCustomerSession.SetVal("LastCommentEntered", theCmts, System.DateTime.Now.AddMinutes(AppLogic.SessionTimeout())); // instead of passing via querystring due to length
            ThisCustomer.ThisCustomerSession.SetVal("LastRatingEntered", theRating, System.DateTime.Now.AddMinutes(AppLogic.SessionTimeout()));

            HasBadWords = false;
            if (!HasBadWords)
            {
                if (!Editing)
                {
                    sql.Append("INSERT INTO EcommerceRating(ItemCode, IsFilthy, CustomerCode, CreatedOn, Rating, HasComment, WebSiteCode,Comments, ContactCode) VALUES(");
                    sql.Append(ItemCode.ToDbQuote() + ",");
                    sql.Append(HasBadWords.ToBit().ToString() + ",");
                    sql.Append(ThisCustomer.CustomerCode.ToDbQuote() + ",");
                    sql.Append(DB.DateQuote(Localization.ToDBDateTimeString(DateTime.Now)) + ",");
                    sql.Append(theRating + ",");
                    sql.Append(CommonLogic.IIF(theCmts.Length != 0, "1", "0") + ",");
                    sql.Append(InterpriseHelper.ConfigInstance.WebSiteCode.ToDbQuote() + ",");

                    if (theCmts.Length != 0)
                    {
                        sql.Append(DB.SQuote(theCmts) + ",");
                    }
                    else
                    {
                        sql.Append("NULL" + ",");
                    }
                    sql.Append(ThisCustomer.ContactCode.ToDbQuote());
                    sql.Append(")");
                }
                else
                {
                    sql.Append("UPDATE EcommerceRating SET ");
                    sql.Append("IsFilthy=" + HasBadWords.ToBit().ToString() + ",");
                    sql.Append("Rating=" + theRating + ",");
                    sql.Append("CreatedOn=getdate(),");
                    sql.Append("HasComment=" + (theCmts.Length != 0).ToBit().ToString() + ",");

                    if (theCmts.Length != 0)
                    {
                        sql.Append("Comments=" + DB.SQuote(theCmts));
                    }
                    else
                    {
                        sql.Append("Comments=NULL");
                    }
                    sql.Append(" where ItemCode=" + ItemCode.ToDbQuote() +
                               " and CustomerCode=" + ThisCustomer.CustomerCode.ToDbQuote() +
                               " and ContactCode=" + ThisCustomer.ContactCode.ToDbQuote());
                }

                DB.ExecuteSQL(sql.ToString());

                TheirCurrentRating = Convert.ToInt32(theRating);
                TheirCurrentComment = theCmts;

                var rcollection = new Rating();
                rcollection.SynchronizeHelpfulCount(ItemCode);
            }

            var s = new StringBuilder("");
            if (!HasBadWords)
            {
                s.Append("<script type='text/javascript'>\n");

                if (!NoPostback || HttpContext.Current.Request.Browser.Browser == "InternetExplorer") { s.Append("opener.window.location.reload();"); }

                s.Append("self.close();");
                s.Append("</script>\n");
                ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), s.ToString());
            }
            else
            {
                InitializePageContent();
            }
        }
        
        private void InitializePageContent()
        {

            rateit_aspx_4.Visible = HasBadWords;

            rateit_aspx_3.Text = AppLogic.GetString("rateit.aspx.3");
            rateit_aspx_4.Text = AppLogic.GetString("rateit.aspx.4");
            rateit_aspx_5.Text = AppLogic.GetString("rateit.aspx.5");
            rateit_aspx_12.Text = AppLogic.GetString("rateit.aspx.12");
            rateit_aspx_13.Text = AppLogic.GetString("rateit.aspx.13");

             // Check if ItemCode is part of the Query String or parameter
            if (String.IsNullOrEmpty(ItemCode))
            {
                this.btnSubmit.Enabled = false;
                this.btnCancel.Enabled = false;
                this.rating.Enabled = false;
                this.Comments.Enabled = false;
                lblProductName.Text = AppLogic.GetString("rateit.aspx.16");
                this.Star1.Visible = false;
                this.Star2.Visible = false;
                this.Star3.Visible = false;
                this.Star4.Visible = false;
                this.Star5.Visible = false;
            }
            else
            {
                this.btnSubmit.Enabled = true;
                this.btnCancel.Enabled = true;
                this.rating.Enabled = true;
                this.Comments.Enabled = true;
                lblProductName.Text = HttpContext.Current.Server.HtmlEncode(AppLogic.GetProductName(ItemCode, ThisCustomer.LocaleSetting));
                this.Star1.Visible = true;
                this.Star2.Visible = true;
                this.Star3.Visible = true;
                this.Star4.Visible = true;
                this.Star5.Visible = true;
            }
            string img1 = AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif");
            string img2 = AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-blu.gif");
            Star1.ImageUrl = CommonLogic.IIF(TheirCurrentRating >= 1, img2, img1);
            Star2.ImageUrl = CommonLogic.IIF(TheirCurrentRating >= 2, img2, img1);
            Star3.ImageUrl = CommonLogic.IIF(TheirCurrentRating >= 3, img2, img1);
            Star4.ImageUrl = CommonLogic.IIF(TheirCurrentRating >= 4, img2, img1);
            Star5.ImageUrl = CommonLogic.IIF(TheirCurrentRating >= 5, img2, img1);

            rating.SelectedValue = TheirCurrentRating.ToString();
            Comments.Text = TheirCurrentComment;

            btnSubmit.Text = AppLogic.GetString("rateit.aspx.14");
            btnCancel.Text = AppLogic.GetString("rateit.aspx.15");

            GetJSFunctions();
        }

        private void GetJSFunctions()
        {
            StringBuilder s = new StringBuilder("");
            s.Append("<script type=\"text/javascript\" Language=\"JavaScript\">\n");
            s.Append(" document.onreadystatechange=document_onreadystatechange;\n");
            s.Append("function FormValidator()\n");
            s.Append("	{\n");
            s.Append("	if (document.getElementById(\"rating\").selectedIndex < 1)\n");
            s.Append("	{\n");
            s.Append("		alert(\"" + AppLogic.GetString("rateit.aspx.1") + "\");\n");
            s.Append("		document.getElementById(\"rating\").focus();\n");
            s.Append("		return (false);\n");
            s.Append("    }\n");
            s.Append("	if (document.getElementById(\"Comments\").value.length > 5000)\n");
            s.Append("	{\n");
            s.Append("		alert(\"" + AppLogic.GetString("rateit.aspx.2") + "\");\n");
            s.Append("		document.getElementById(\"Comments\").focus();\n");
            s.Append("		return (false);\n");
            s.Append("    }\n");
            s.Append("	return (true);\n");
            s.Append("	}\n");
            s.Append("\n");
            s.Append("	var ImgArray = new Array(new Image(),new Image())\n");
            s.Append("	ImgArray[0].src = \"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-blu.gif") + "\"\n");
            s.Append("	ImgArray[1].src = \"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif") + "\"\n");
            s.Append("	\n");
            s.Append("	function document_onreadystatechange()\n");
            s.Append("	{\n");
            s.Append("		newRatingEntered(document.getElementById(\"rating\").selectedIndex);\n");
            s.Append("	}\n");
            s.Append("	\n");
            s.Append("	function newRatingEntered(RV)\n");
            s.Append("	{\n");
            s.Append("		if (RV >= 1)\n");
            s.Append("			{document.getElementById(\"" + Star1.ClientID + "\").src = ImgArray[0].src}\n");
            s.Append("		else\n");
            s.Append("			{document.getElementById(\"" + Star1.ClientID + "\").src = ImgArray[1].src}\n");
            s.Append("		if (RV >= 2)\n");
            s.Append("			{document.getElementById(\"" + Star2.ClientID + "\").src = ImgArray[0].src}\n");
            s.Append("		else\n");
            s.Append("			{document.getElementById(\"" + Star2.ClientID + "\").src = \"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif") + "\"}\n");
            s.Append("		if (RV >= 3)\n");
            s.Append("			{document.getElementById(\"" + Star3.ClientID + "\").src = ImgArray[0].src}\n");
            s.Append("		else\n");
            s.Append("			{document.getElementById(\"" + Star3.ClientID + "\").src = \"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif") + "\"}\n");
            s.Append("		if (RV >= 4)\n");
            s.Append("			{document.getElementById(\"" + Star4.ClientID + "\").src = ImgArray[0].src}\n");
            s.Append("		else\n");
            s.Append("			{document.getElementById(\"" + Star4.ClientID + "\").src = \"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif") + "\"}\n");
            s.Append("		if (RV >= 5)\n");
            s.Append("			{document.getElementById(\"" + Star5.ClientID + "\").src = ImgArray[0].src}\n");
            s.Append("		else\n");
            s.Append("			{document.getElementById(\"" + Star5.ClientID + "\").src =\"" + AppLogic.LocateImageURL("skins/skin_" + _SkinID.ToString() + "/images/bigstar-whi.gif") + "\"}\n");
            s.Append("		document.getElementById(\"rating\").selectedIndex = RV;\n");
            s.Append("		return false;\n");
            s.Append("	}\n");
            s.Append("</script>\n");

            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), s.ToString());

        }

        public bool NoPostback { get; set; }
    }
}