using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.UI.Extensions;

public partial class UserControls_AddressControl : System.Web.UI.UserControl
{

    #region Properties

    public bool IsShowResidenceTypesSelector { get; set; }
    public bool IsShowBusinessTypesSelector { get; set; }
    public bool IsShowCounty { get; set; }
    public bool IsHideStreetAddressInputBox { get; set; }
    public int IsCountryWidthAdjustment { get; set; }
    public string IdPrefix { get; set; }
    public string ResidenceTypeSelected { get; set; }
    public string SelectedCountry { get; set; }
    public string DefaultPostalText { get; set; }
    public string DefaultAddressType { get; set; }
    public string DefaultBusinessTypeText { get; set; }
    public string DefaultCountryText { get; set; }

    #endregion

    #region Label Controls

    public string LabelStreetText
    {
        get { return litStreet.Text; }
        set { litStreet.Text = value; }
    }

    public string LabelCityText
    {
        get { return litCity.Text; }
        set { litCity.Text = value; }
    }

    public string LabelStateText
    {
        get { return litState.Text; }
        set { litState.Text = value; }
    }

    public string LabelPostalText
    {
        get { return litPostal.Text; }
        set { litPostal.Text = value; }
    }

    public string LabelEnterPostalText
    {
        get { return litEnterPostal.Text; }
        set { litEnterPostal.Text = value; }
    }

    public string LabelCountyText
    {
        get { return litCounty.Text; }
        set { litCounty.Text = value; }
    }

    public string LabelTaxText
    {
        get { return litTaxtNumber.Text; }
        set { litTaxtNumber.Text = value; }
    }

    #endregion

    #region Input Controls

    public string Street
    {
        get
        {
            return txtStreet.Text;
        }
        set
        {
            txtStreet.Text = value;
        }
    }

    public string Country
    {

        get
        {
            return drpCountry.Text;
        }
        set
        {
            drpCountry.Text = value;
        }

    }

    public string Postal
    {
        get
        {
            return txtPostal.Text;
        }
        set
        {
            txtPostal.Text = value;
        }
    }

    public string City
    {
        get
        {
            return txtCity.Text;
        }
        set
        {
            txtCity.Text = value;

        }
    }

    public string State
    {
        get
        {
            return txtState.Text;
        }
        set
        {
            txtState.Text = value;
        }
    }

    public string County
    {
        get
        {
            return txtCounty.Text;
        }
        set
        {
            txtCounty.Text = value;
        }
    }

    public string ResidenceType
    {
        get
        {
            return drpType.Text;
        }
        set
        {
            drpType.Text = value;
        }
    }

    public string BusinessType
    {
        get
        {
            return drpBusinessType.Text;
        }
        set
        {
            drpBusinessType.Text = value;
        }
    }

    public string TaxNumber
    {
        get
        {
            return txtTaxNumber.Text;
        }
        set
        {
            txtTaxNumber.Text = value;
        }
    }

    #endregion

    #region Methods

    public void BindData()
    {
        drpCountry.Items.Clear();
        AppLogic.AddItemsToDropDownList(ref drpCountry, "Country", true, DefaultCountryText);

        pnlCounty.Visible = IsShowCounty;

        if (IsShowResidenceTypesSelector)
        {
            drpType.Items.Add(this.DefaultAddressType);
            drpType.Items.Add(ResidenceTypes.Residential.ToString());
            drpType.Items.Add(ResidenceTypes.Commercial.ToString());
            drpType.SelectedIndex = drpType.ToSelectedIndexByText(ResidenceTypeSelected);
        }

        if (IsShowBusinessTypesSelector)
        {
            drpBusinessType.Items.Add(DefaultBusinessTypeText);
            drpBusinessType.Items.Add(Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_WHOLESALE);
            drpBusinessType.Items.Add(Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_RETAIL);
        }

        txtPostal.Attributes.Add("data-postalIsOptionalIn", AppLogic.AppConfig("PostalCodeNotRequiredCountries"));
        txtPostal.Attributes.Add("data-labelText", LabelPostalText);
        drpCountry.SelectedIndex = drpCountry.ToSelectedIndexByText(SelectedCountry);
    }

    #endregion
}