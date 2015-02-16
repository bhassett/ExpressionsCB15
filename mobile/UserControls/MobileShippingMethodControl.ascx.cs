using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.ComponentModel;
using System.Text;

public partial class UserControls_MobileShippingMethodControl : BaseUserControl
{

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void Render(HtmlTextWriter writer)
    {
        var script = new StringBuilder();
        script.Append("<script type='text/javascript'>\n");
        script.Append("$add_windowLoad(\n");
        script.Append(" function() { \n");

        script.AppendFormat("   var reg = ise.Controls.ShippingMethodController.registerControl('{0}');\n", this.ClientID);

        script.Append("   if(reg) {\n");

        if (this.SkipShipping)
        {
            script.AppendFormat("      reg.setIsSkipShipping({0});\n", true.ToString().ToLowerInvariant());
        }

        if (this.HidePickupStoreLink)
        {
            script.AppendFormat("      reg.setHidePickupStoreLink({0});\n", true.ToString().ToLowerInvariant()); 
        }

        if (this.HideInStorePickUpShippingOption)
        {
            script.AppendFormat("      reg.hideInStorePickUpShippingOption({0});\n", true.ToString().ToLowerInvariant());
        }

        if (this.IsMultipleShipping)
        {
            script.AppendFormat("       reg.setIfMultipleShipping({0});\n", true.ToString().ToLowerInvariant());
        }

        foreach (var validator in this.ProvideValidators())
        {
            script.AppendFormat("      reg.registerValidator({0});\n", validator.RenderInitialization());
        }

        if (null != this.ErrorSummaryControl)
        {
            script.AppendFormat("      reg.setValidationSummary({0});", this.ErrorSummaryControl.RenderInitialization());
            script.AppendLine();
        }

        script.AppendFormat("      reg.requestShippingMethod('{0}');\n", ShippingAddressID);

        if (this.ShowShowAllRatesButton)
        {
            script.AppendFormat("      reg.registerShowAllRatesButton('{0}');\n", this.btnShowAllRates.ClientID);
        }

        script.Append("   }\n");
        script.Append(" }\n");
        script.Append(");\n");
        script.Append("</script>\n");


        writer.Write(script);
        base.Render(writer);
    }

    #endregion

    #region Properties

    public string ShippingAddressID
    {
        get
        {
            object addressID = ViewState["ShippingAddressID"];
            if (null == addressID) { return String.Empty; }

            return addressID.ToString();
        }
        set
        {
            ViewState["ShippingAddressID"] = value;
        }
    }

    protected override List<InputValidator> ProvideValidators()
    {
        var defaultValidators = new List<InputValidator>();
        defaultValidators.Add(MakeRequiredInputValidator(shippingMethod, this.ShippingMethodRequiredErrorMessage));

        return defaultValidators;
    }

    public string ShippingMethodRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState["ShippingMethodRequiredErrorMessage"];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState["ShippingMethodRequiredErrorMessage"] = value;
        }
    }

    public bool SkipShipping
    {
        get
        {
            object booleanValue = ViewState["SkipShipping"];
            if (null == booleanValue) { return false; }

            return booleanValue is bool && (bool)booleanValue;
        }
        set
        {
            ViewState["SkipShipping"] = value;
        }
    }

    public Customer ThisCustomer
    {
        get
        {
            Customer customer = null;
            if (ViewState["ThisCustomer"] != null)
            {
                customer = ViewState["ThisCustomer"] as Customer;
            }
            return customer;
        }
        set
        {
            ViewState["ThisCustomer"] = value;
        }
    }

    public string ShippingMethod
    {
        get { return shippingMethod.Value; }
        set { shippingMethod.Value = value; }
    }

    public string FreightCalculation
    {
        get { return freightCalculation.Value; }
        set { freightCalculation.Value = value; }
    }

    public string Freight
    {
        get { return freight.Value; }
        set { freight.Value = value; }
    }

    public string FreightChargeType
    {
        get { return hdenFreightChargeType.Value; }
        set { hdenFreightChargeType.Value = value; }
    }

    public Guid RealTimeRateGUID
    {
        get { return new Guid(realTimeRateGUID.Value); }
        set { realTimeRateGUID.Value = value.ToString(); }
    }

    public bool HidePickupStoreLink { get; set; }

    public bool HideInStorePickUpShippingOption { get; set; }

    public bool ShowShowAllRatesButton
    {
        get { return btnShowAllRates.Visible; }
        set { btnShowAllRates.Visible = value; }
    }

    public bool IsMultipleShipping { get; set; }

    public string ShowAllRatesButtonText
    {
        get
        {
            return btnShowAllRates.Text;
        }
        set
        {
            btnShowAllRates.Text = value;
        }
    }

    #endregion

}