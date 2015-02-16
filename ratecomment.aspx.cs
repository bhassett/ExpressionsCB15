// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Text;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using System.Data.SqlClient;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for ratecomment.
	/// </summary>
	public partial class ratecomment : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            ServiceFactory.GetInstance<IRequestCachingService>()
                          .PageNoCache();

            var ThisCustomer = ServiceFactory.GetInstance<IAuthenticationService>()
                                             .GetCurrentLoggedInCustomer();

            ThisCustomer.RequireCustomerRecord();

            string ItemCode = "ProductID".ToQueryString();

            //Comment owner to evaluate
            string CustomerID = "CustomerID".ToQueryString();
            string ContactID = "ContactID".ToQueryString();

            string MyVote = "MyVote".ToQueryString()
                                    .ToUpperInvariant();

            int HelpfulVal = (MyVote == "YES").ToBit();

            CustomerDA.RateComment(ItemCode, CustomerID, ContactID, HelpfulVal);

			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<title>Rate Comment</title>\n");
			Response.Write("</head>\n");
			Response.Write("<body>\n");
			Response.Write("<!-- INVOCATION: " + CommonLogic.PageInvocation() + " -->\n");
			Response.Write("</body>\n");
			Response.Write("</html>\n");

		}

	}
}






