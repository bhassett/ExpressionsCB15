using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using Interprise.Framework.ECommerce.DatasetComponent;
using Interprise.Framework.ECommerce.DatasetGateway;
using Interprise.Facade.ECommerce;
using Interprise.Facade.Base;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class savenotification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            ltMessage.Text = AppLogic.GetString("savenotification.aspx.cs.1");
            int notificationType = Int32.Parse("notificationType".ToQueryString());
            string itemCode = "itemCode".ToQueryString();
            string itemType = "itemType".ToQueryString();
            
            string productURL = String.Empty;
            if (itemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_MATRIX_GROUP)
            {
                var matrixInfo = ServiceFactory.GetInstance<IProductService>()
                                               .GetMatrixItemInfo(itemCode);

                productURL = CurrentContext.FullyQualifiedApplicationPath() + InterpriseHelper.MakeItemLink(matrixInfo.ItemCode);
                productURL = CommonLogic.QueryStringSetParam(productURL, DomainConstants.QUERY_STRING_KEY_MATRIX_ID, matrixInfo.Counter.ToString());
            }
            else
            {
                productURL = CurrentContext.FullyQualifiedApplicationPath() + InterpriseHelper.MakeItemLink(itemCode);
            }

            var ruleloaddataset = new string[][] { new string[] {"ECOMMERCENOTIFICATION", "READECOMMERCENOTIFICATION", "@ContactCode", Customer.Current.ContactCode,
                                                      "@WebsiteCode", InterpriseHelper.ConfigInstance.WebSiteCode, "@ItemCode", itemCode, "@EmailAddress", Customer.Current.EMail}};

            var ruleDatasetContainer = new EcommerceNotificationDatasetGateway();

            if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.LoadDataSet(
                InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, ruleloaddataset, ruleDatasetContainer))
            {
                EcommerceNotificationDatasetGateway.EcommerceNotificationRow ruleDatasetContainernewRow;

                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification.NewEcommerceNotificationRow();
                else
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification[0];

                bool onPriceDrop = ServiceFactory.GetInstance<ICustomerService>().IsCustomerSubscribeToProductNotification(itemCode, 1);
                bool onItemAvail = ServiceFactory.GetInstance<ICustomerService>().IsCustomerSubscribeToProductNotification(itemCode, 0);

                if (notificationType == 1)
                {
                    onPriceDrop = true;
                }
                else
                {
                    onItemAvail = true;
                }

                ruleDatasetContainernewRow.BeginEdit();
                ruleDatasetContainernewRow.WebSiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;
                ruleDatasetContainernewRow.ItemCode = itemCode;
                ruleDatasetContainernewRow.ContactCode = Customer.Current.ContactCode;
                ruleDatasetContainernewRow.EmailAddress = Customer.Current.EMail;
                ruleDatasetContainernewRow.NotifyOnPriceDrop = onPriceDrop;
                ruleDatasetContainernewRow.NotifyOnItemAvail = onItemAvail;
                ruleDatasetContainernewRow.ProductURL = productURL;

                byte[] salt = InterpriseHelper.GenerateSalt();
                byte[] iv = InterpriseHelper.GenerateVector();
                string contactCodeCypher = InterpriseHelper.Encryption(Customer.Current.ContactCode, salt, iv);
                string emailAddressCypher = InterpriseHelper.Encryption(Customer.Current.EMail, salt, iv);

                ruleDatasetContainernewRow.EncryptedContactCode = contactCodeCypher + "|" + Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(iv);
                ruleDatasetContainernewRow.EncryptedEmailAddress = emailAddressCypher + "|" + Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(iv);

                ruleDatasetContainernewRow.EndEdit();

                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                    ruleDatasetContainer.EcommerceNotification.AddEcommerceNotificationRow(ruleDatasetContainernewRow);

                var rulecommandset = new string[][] { new string[] { ruleDatasetContainer.EcommerceNotification.TableName, "CREATEECOMMERCENOTIFICATION",
                                                                        "UPDATEECOMMERCENOTIFICATION", "DELETEECOMMERCENOTIFICATION"} };

                if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.UpdateDataset(
                    InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, rulecommandset, ruleDatasetContainer))
                {
                    ltMessage.Text = AppLogic.GetString("savenotification.aspx.cs.2");
                    Response.Write("<script 'type=text/javascript'>window.top.close();</script>");
                }
            }
        }
    }
}
 

      