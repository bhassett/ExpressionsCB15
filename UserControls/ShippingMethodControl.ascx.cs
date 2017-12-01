using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls.Validators.Special;
using InterpriseSuiteEcommerceControls.Validators;
using System.Text;

public partial class UserControls_ShippingMethodControl : BaseUserControl
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

        if (InstoreCartItem != null)
        {
            var item = InstoreCartItem;
            script.Append("     var param = new Object(); \n");
            script.AppendFormat("      param.ItemCode = '{0}'\n", item.ItemCode);
            script.AppendFormat("      param.UnitMeassureCode = '{0}'\n", item.UnitMeassureCode);
            script.AppendFormat("      param.Counter = '{0}'\n", item.Counter);
            script.AppendFormat("      param.ShippingMethod = '{0}'\n", item.ShippingMethod);
            script.AppendFormat("      param.Quantity = '{0}'\n", item.Quantity);

            if (item.KitComposition != null)
            {
                var compositionArray = item.KitComposition.Compositions.Select(c => { return "'0+{0}'".FormatWith(c.ItemKitCounter.ToString()); });
                string composition = "[{0}]".FormatWith(String.Join(DomainConstants.COMMA_DELIMITER
                                                                                   .ToString(), compositionArray.ToArray()));
                script.AppendFormat("      param.kitComposition = {0}\n", composition);
            }

            script.Append("            reg.setInstoreCartItem(param);\n");
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

        script.AppendFormat("      reg.requestShippingMethod('{0}', '{1}');\n", ShippingAddressID, ItemSpecificType);

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

        InStorePickupShppingMethodValidator customValidator = null;

        //if hide the pickup link then pass only the normal checking
        if (this.HidePickupStoreLink)
        { 
            customValidator = new InStorePickupShppingMethodValidator(shippingMethod, null, null, String.Empty, this.ShippingMethodRequiredErrorMessage);
        }
        else
        {
            customValidator = new InStorePickupShppingMethodValidator(shippingMethod, hdenFreightChargeType, hdenInStoreSelectedWarehouseCode, this.InStoreNoSelectedWarehouseErrorMessage, this.ShippingMethodRequiredErrorMessage);
        }

        defaultValidators.Add(customValidator);

        HandleValidationErrorEvent(customValidator);

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

    public string InStoreNoSelectedWarehouseErrorMessage
    {
        get
        {
            object savedValue = ViewState["InStoreNoSelectedWarehouseErrorMessage"];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState["InStoreNoSelectedWarehouseErrorMessage"] = value;
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

    public string InStoreSelectedWareHouseCode
    {
        get { return hdenInStoreSelectedWarehouseCode.Value; }
        set { hdenInStoreSelectedWarehouseCode.Value = value; }
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

    public bool HidePickupStoreLink { get; set; }

    public bool HideInStorePickUpShippingOption { get; set; }

    public Guid RealTimeRateGUID
    {
        get { return new Guid(realTimeRateGUID.Value); }
        set { realTimeRateGUID.Value = value.ToString(); }
    }

    public bool RedirectOnlyWhenPickupOption { get; set; }

    public CustomCartItem InstoreCartItem { get; set; }

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

    public string ItemSpecificType
    {
        get { return itemSpecificType.Value; }
        set { itemSpecificType.Value = value; }
        //get {return specificTypeCode.Value;} set; 
    }

    #endregion
}   