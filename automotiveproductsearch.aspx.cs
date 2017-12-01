using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;
using InterpriseSuiteEcommerceCommon.DTO;

public partial class automotiveproductsearch : SkinBase
{

    #region Variables
    private string m_connectionID;

    #endregion

    #region Properties

    #region ConnectionID

    private string ConnectionID
    {
        get
        {
            return m_connectionID;
        }
        set
        {
            m_connectionID = value;
        }
    }
    #endregion

    #endregion

    #region Method

    #region GetProductDetails
    public static List<AutomotiveProductInfo> GetProductDetails(string make, string model, string type, string year, string category)
    {

        AutomotiveProductInfoGroup productInfo = new AutomotiveProductInfoGroup();
        productInfo.Items = new List<AutomotiveProductInfo>();

        string script = string.Format("EXEC GetAutomotiveProduct_DEV004765 @Maker='{0}', @Model='{1}', @Type='{2}', @Year='{3}', @Category='{4}'", make, model, type, year, category);

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();

            using (var dataSet = DB.GetDS(script, false))
            {
                if (dataSet.Tables[0].Rows.Count < 1)
                {
                    return null;
                }



                foreach (System.Data.DataRow row in dataSet.Tables[0].Rows)
                {
                    var item = new AutomotiveProductInfo();
                    item.Counter = Convert.ToInt16(row["Counter"]);
                    item.ItemCode = row["ItemCode_DEV004765"].ToString();
                    item.Make = row["Make_DEV004765"].ToString();
                    item.Model = row["Model_DEV004765"].ToString();
                    item.Type = row["Type_DEV004765"].ToString();
                    item.Year = row["Year_DEV004765"].ToString();
                    item.ItemDescription = row["ItemDescription"].ToString();
                    item.Qty = 0;
                    item.ItemCounter = Convert.ToInt16(row["ItemCounter"]);

                   // var image = ProductImage.LocateDefaultImage("product", item.ItemCode, "icon", "English - United States");

                   // item.ImgSrc = image.src;

                    string imageFilename = ServiceFactory.GetInstance<InterpriseSuiteEcommerceCommon.Domain.Infrastructure.IProductService>().GetItemDefaultImageFilenameBySize(item.ItemCode, InterpriseSuiteEcommerceCommon.ImageSize.Icon);
                    string imagePath = "images/product/icon/" + imageFilename;
                    
                    item.ImgSrc = imagePath;

                    productInfo.Items.Add(item as AutomotiveProductInfo);
                }
            }
        }

        return productInfo.Items;
    }
    #endregion

    #region GetHeaderXML
    private System.Xml.Linq.XElement GetHeaderXML()
    {
        //List<AutomotiveProductInfo> item = GetProductDetails("null", "null", "null", "null");

        var root = new System.Xml.Linq.XElement(DomainConstants.XML_ROOT_NAME);


        //foreach (var row in item)
        //{
        //    var product = new System.Xml.Linq.XElement("AutomotiveProduct");
        //    product.Add(new System.Xml.Linq.XElement("Counter", row.Counter));
        //    product.Add(new System.Xml.Linq.XElement("ItemCode_DEV004765", row.ItemCode));
        //    product.Add(new System.Xml.Linq.XElement("Make_DEV004765", row.Make));
        //    product.Add(new System.Xml.Linq.XElement("Model_DEV004765", row.Model));
        //    product.Add(new System.Xml.Linq.XElement("Type_DEV004765", row.Type));
        //    product.Add(new System.Xml.Linq.XElement("Year_DEV004765", row.Year));
        //    product.Add(new System.Xml.Linq.XElement("ImageSource", row.ImgSrc));
        //    product.Add(new System.Xml.Linq.XElement("ItemDescription", row.ItemDescription));
        //    root.Add(product);
        //}

        System.Data.DataSet dt = GetDistinctClassification();

        System.Xml.Linq.XElement classification = null;
        for (var x = 0; x <= dt.Tables.Count - 1; x++)
        {
            if (dt.Tables[x].TableName == "WebserviceURL")
            {
                if (dt.Tables[x].Rows.Count != 0)
                {
                    classification = new System.Xml.Linq.XElement("WebserviceURL");
                    classification.Add(new System.Xml.Linq.XElement("ConfigValue", dt.Tables[x].Rows[0]["ConfigValue"].ToString()));
                }
            }

            else
            {
                classification = new System.Xml.Linq.XElement(dt.Tables[x].TableName + "Classification");
                foreach (System.Data.DataRow row in dt.Tables[x].Rows)
                {
                    classification.Add(new System.Xml.Linq.XElement(dt.Tables[x].TableName, row[0]));
                }

            }

            root.Add(classification);
        }

        if (m_connectionID != null)
        {
            System.Xml.Linq.XElement connID = null;

            connID = new System.Xml.Linq.XElement("ConnID");
            connID.Add(new System.Xml.Linq.XElement("ID", m_connectionID));
            root.Add(connID);
        }


        return root;
    }
    #endregion

    #region GetProductXML
    private static System.Xml.Linq.XElement GetProductXML(string Year, string Maker, string Model, string Type, string Category, int pageNum)
    {
        List<AutomotiveProductInfo> item = GetProductDetails(Maker, Model, Type, Year, Category);

        int totalCount = GetTotalCount(Year, Maker, Model, Type, Category);

        if (item == null)
        {
            return null;
        }

        var root = new System.Xml.Linq.XElement(DomainConstants.XML_ROOT_NAME);


        //ITEMS
        foreach (var row in item)
        {
            var product = new System.Xml.Linq.XElement("AutomotiveProduct");
            product.Add(new System.Xml.Linq.XElement("Counter", row.Counter));
            product.Add(new System.Xml.Linq.XElement("ItemCode_DEV004765", row.ItemCode));
            product.Add(new System.Xml.Linq.XElement("Make_DEV004765", row.Make));
            product.Add(new System.Xml.Linq.XElement("Model_DEV004765", row.Model));
            product.Add(new System.Xml.Linq.XElement("Type_DEV004765", row.Type));
            product.Add(new System.Xml.Linq.XElement("Year_DEV004765", row.Year));
            product.Add(new System.Xml.Linq.XElement("ImageSource", row.ImgSrc));
            product.Add(new System.Xml.Linq.XElement("ItemDescription", row.ItemDescription));
            product.Add(new System.Xml.Linq.XElement("Qty", row.Qty));
            product.Add(new System.Xml.Linq.XElement("ItemCounter", row.ItemCounter));
            root.Add(product);
        }

        int pages = totalCount / 2;

        for (int x = 1; x <= pages; x++)
        {
            var page = new System.Xml.Linq.XElement("Page");
            page.Add(new System.Xml.Linq.XElement("Pages", x));
            if (pageNum == x)
            {
                page.Add(new System.Xml.Linq.XElement("IsActive", 1));
            }
            else
            {
                page.Add(new System.Xml.Linq.XElement("IsActive", 0));
            }
            root.Add(page);
        }


        return root;
    }
    #endregion

    #region GetDistinctClassification
    private System.Data.DataSet GetDistinctClassification()
    {

        

        System.Data.DataSet types;
        string script = string.Format("SELECT DISTINCT(Make_DEV004765) FROM InventoryItemClassification_DEV004765 " +
        "SELECT DISTINCT(Model_DEV004765) FROM InventoryItemClassification_DEV004765 " +
        "SELECT DISTINCT(Type_DEV004765) FROM InventoryItemClassification_DEV004765 UNION ALL SELECT 'All' AS  Type_DEV004765 " +
        "SELECT DISTINCT(Year_DEV004765) FROM InventoryItemClassification_DEV004765 " +
        "SELECT CategoryCode FROM SystemCategory " +
        "SELECT * FROM EcommerceAppConfig WHERE Name='{0}' ", "WCF-URL");

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            types = DB.GetDS(script, false);
        }

        types.Tables[0].TableName = "Make";
        types.Tables[1].TableName = "Model";
        types.Tables[2].TableName = "Type";
        types.Tables[3].TableName = "Year";
        types.Tables[4].TableName = "Category";
        types.Tables[5].TableName = "WebserviceURL";

        var url = types.Tables[5].Rows[0]["ConfigValue"].ToString();

        types.Tables[5].Rows[0]["ConfigValue"] = ValidateWCFurl(url);

        return types;
    }
    #endregion

    #region ValidateWCFurl
    private string ValidateWCFurl(string url)
    {
        if (url == "") return null;
        var lastString = url.Substring(url.Length - 1);
        if (lastString == "/")
        {
           return url.Remove(url.Length - 1);
        }
        return url;
    }


    #endregion

    #region OnSubmit
    [WebMethod, System.Web.Script.Services.ScriptMethod]
    public static string OnSubmit(string Year, string Maker, string Model, string Type, string Category, int pageNum = 1)
    {
        
        string xmlPackage = "automotive.product.search.content.xml.config";

        string content = new XmlPackage2(xmlPackage, Customer.Current, Customer.Current.SkinID, string.Empty, null, string.Empty, true, GetProductXML(Year, Maker, Model, Type, Category, pageNum)).TransformString();

        return content;


    }
    #endregion

    #region GetTotalCount

    private static int GetTotalCount(string Year, string Maker, string Model, string Type, string Category)
    {
        string script = string.Format("SELECT COUNT(*) FROM InventoryItemClassification_DEV004765 A WHERE Make_DEV004765 = '{0}'" +
                                       "AND Model_DEV004765 = '{1}' AND Type_DEV004765 = '{2}' AND Year_DEV004765 = '{3}'" +
                                       "AND (SELECT CategoryCode FROM InventoryCategory C WHERE C.ItemCode = A.ItemCode_DEV004765) = '{4}'", Maker, Model, Type, Year, Category);

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();

            using (var dataSet = DB.GetDS(script, false))
            {
                if (dataSet.Tables[0].Rows.Count < 1)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt16(dataSet.Tables[0].Rows[0][0]);
                }
            }

        }

       // return 0;
    }


    #endregion

    #endregion

    #region Events

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // OnOwinStart();

        if (Request.QueryString["Id"] != null)
        {
            m_connectionID = Request.QueryString["Id"].ToString();
        }
        

        System.Data.DataSet dtset;
        string query;
        
        query = string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('{0}') AND type in ('{1}')) SELECT 1 ELSE SELECT 0 " , "[dbo].[InventoryItemClassification_DEV004765]", "U");


       using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            dtset = DB.GetDS(query, false);
        }

        //do not proceed if table does not yet exists
       if (Convert.ToInt16(dtset.Tables[0].Rows[0][0]) == 1)
       {
           string xmlPackage = "automotive.product.search.xml.config";

           string content = new XmlPackage2(xmlPackage, Customer.Current, Customer.Current.SkinID, string.Empty, null, string.Empty, true, GetHeaderXML()).TransformString();
           webcontent.InnerHtml = content;
       }
       else
       {
           string msg = "<p>Please register the Automotive plugin to access this page.</p>";
           webcontent.InnerHtml = msg;
       }

      

    }

    #endregion

    #region OnOwinStart
    [WebMethod]
    public static void OnOwinStart()
    {
        //OnOwinStop();
        //SelfhostServer.Start();
    }

    #endregion

    #region OnOwinStop
    [WebMethod]
    public static void OnOwinStop()
    {
        //SelfhostServer.Stop();
    }

    #endregion

    #endregion


}