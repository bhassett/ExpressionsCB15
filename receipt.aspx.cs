// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public partial class receipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ServiceFactory.GetInstance<IRequestCachingService>()
                          .PageNoCache();

            AppLogic.RequireSecurePage();

            Customer thisCustomer = Customer.Current;
            bool blnShowReceipt = false;

            //get the values from the querystring
            string strOrderCodeFromQueryString = CommonLogic.QueryStringCanBeDangerousContent("OrderNumber");
            string strCustGuidFromQueryString = CommonLogic.QueryStringCanBeDangerousContent("CustomerGUID");

            string transactionType = ServiceFactory.GetInstance<ICustomerRepository>().GetTransactionType(strOrderCodeFromQueryString);


            if (thisCustomer.IsNotRegistered)
            {
                string strOrderNumberFromCookie = CommonLogic.CookieCanBeDangerousContent("OrderNumber", true);
                string strCustGuidFromCookie = CommonLogic.CookieCanBeDangerousContent("ContactGUID", true);
                
                // for multiple order number in cookies
                string[] ordernumbers = strOrderNumberFromCookie.ToLowerInvariant().Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries);
                bool orderInCookie = Array.IndexOf(ordernumbers, strOrderCodeFromQueryString.ToLowerInvariant()) > -1;

                //show the receipt only if both the order number and guid match
                blnShowReceipt = strCustGuidFromQueryString.Equals(strCustGuidFromCookie, StringComparison.InvariantCultureIgnoreCase) && orderInCookie;
            }
            else
            {
                //make sure that this customer owns this order to view
                if (thisCustomer.OwnsThisOrder(strOrderCodeFromQueryString, transactionType))
                {
                    blnShowReceipt = true;   
                }
            }

            //show the receipt if it's appropriate to do so
            if (blnShowReceipt && !string.IsNullOrEmpty(strOrderCodeFromQueryString))
            {
                ViewerReport.Report = InterpriseHelper.CreateReport(strOrderCodeFromQueryString, transactionType);
            }
            else
            {
                Response.Redirect(SE.MakeDriverLink("ordernotfound"));
            }
        }
    }
}
