using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_ShipToAddressControl : System.Web.UI.UserControl
{
    private Customer m_thisCustomer;

    public string ShippingMethodControlId { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack && this.Page.Request.Params.Get("__EVENTTARGET") == drpShipToAddress.UniqueID)
        {
            ThisCustomer.SelectedShippingAddressID = drpShipToAddress.SelectedValue;
            Cart.SetCartShippingAddressID(drpShipToAddress.SelectedValue);
            Response.Redirect(Request.Url.ToString(), true);
        }
    }

    public Customer ThisCustomer
    {
        get
        {
            return m_thisCustomer;
        }
        set 
        {
            m_thisCustomer = value;
            InitializeControl();
        }
    }

    public InterpriseShoppingCart Cart { get; set; }

    private void InitializeControl()
    {
        var shipToAddressList = ServiceFactory.GetInstance<ICustomerService>().GetCustomerAddress(ThisCustomer.CustomerID, (int)AddressTypes.Shipping, ThisCustomer.ContactCode);
        foreach(var shipToAddress in shipToAddressList)
        {
            var completeAddress = new StringBuilder();
            completeAddress.Append(shipToAddress.Name + " - ");
            if (!string.IsNullOrWhiteSpace(shipToAddress.Address)) completeAddress.Append(shipToAddress.Address);
            if (!string.IsNullOrWhiteSpace(shipToAddress.CityStateZip) && completeAddress.Length > 0) completeAddress.Append(", " + shipToAddress.CityStateZip);
            if (!string.IsNullOrWhiteSpace(shipToAddress.Country) && completeAddress.Length > 0) completeAddress.Append(", " + shipToAddress.Country);
            if (!string.IsNullOrWhiteSpace(shipToAddress.County) && completeAddress.Length > 0) completeAddress.Append(", " + shipToAddress.County);

            var addressItem = new ListItem(completeAddress.ToString(), shipToAddress.AddressID);
            drpShipToAddress.Items.Add(addressItem);
        }
        if (!IsPostBack)
        {
            drpShipToAddress.SelectedValue = (string.IsNullOrWhiteSpace(ThisCustomer.SelectedShippingAddressID) ? ThisCustomer.PrimaryShippingAddressID:ThisCustomer.SelectedShippingAddressID);
        }
    }

    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        string queryString = Request.QueryString.ToString();
        string returnURL = "checkoutShipping.aspx";
        if (!string.IsNullOrWhiteSpace(queryString)) returnURL += ("?" + queryString);
        Response.Redirect("selectaddress.aspx?add=true&addressType=Shipping&Checkout=True&noBack=True&returnURL=" + Convert.ToBase64String(Encoding.Unicode.GetBytes(returnURL)), true);
    }
    
}