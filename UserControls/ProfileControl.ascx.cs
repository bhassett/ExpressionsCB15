using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.UI.Extensions;

public partial class UserControls_ProfileControl : System.Web.UI.UserControl
{
    public bool _isAnonymousCheckout = false;

    public string AccountType { get; set; }
    public bool IsExcludeAccountName { get; set; }
    public bool IsUseFullnameTextbox { get; set; }
    public bool IsShowEditPasswordArea { get; set; }
    public bool IsShowSalutation { get; set; }
    public bool IsHideAccountNameArea { get; set; }
    public bool IsAnonymousCheckout {
        get
        {
            return _isAnonymousCheckout;
        }
        set
        {
            _isAnonymousCheckout = value;
        }
    }

    public string SelectedSalutation { get; set; }
    public string DefaultSalutationText { get; set; }

    #region Label Controls

    public string LabelFirstNameText
    {
        get { return litFirstName.Text; }
        set { litFirstName.Text = value; }
    }

    public string LabelLastNameText
    {
        get { return litLastName.Text; }
        set { litLastName.Text = value; }
    }

    public string LabelEmailText
    {
        get { return litEmail.Text; }
        set { litEmail.Text = value; }
    }

    public string LabelAnonymousEmailText
    {
        get { return litAnonymousEmail.Text; }
        set { litAnonymousEmail.Text = value; }
    }

    public string LabelEmailAddressText
    {
        get { return litEmailCaption.Text; }
        set { litEmailCaption.Text = value; }
    }

    public string LabelContactNumberText
    {
        get { return litContactNumber.Text; }
        set { litContactNumber.Text = value; }
    }

    public string LabelMobileNumberText
    {
        get { return litMobile.Text;  }
        set { litMobile.Text = value; }

    }
    public string LabelShippingContactNameText
    {
        get { return litShippingContactName.Text; }
        set { litShippingContactName.Text = value; }
    }

    public string LabelShippingContactNumberText
    {
        get { return litShippingContactNumber.Text; }
        set { litShippingContactNumber.Text = value; }
    }

    public string LabeShippingEmailText
    {
        get { return litShippingEmail.Text; }
        set { litShippingEmail.Text = value; }
    }

    public string LabeEditPasswordText
    {
        get { return litEditPassword.Text; }
        set { litEditPassword.Text = value; }
    }

    public string LabelOldPasswordText
    {
        get { return litOldPassword.Text; }
        set { litOldPassword.Text = value; }
    }

    public string LabelCurrentPasswordText
    {
        get { return litCurrentPassword.Text; }
        set { litCurrentPassword.Text = value; }
    }

    public string LabelAccountNameText
    {
        get { return litAccountName.Text; }
        set { litAccountName.Text = value; }
    }

    public string LabelCompanyNameText
    {
        get { return litCompanyName.Text; }
        set { litCompanyName.Text = value; }
    }

    public string LabelSecurityAccountText
    {
        get { return litAccountSecurity.Text; }
        set { litAccountSecurity.Text = value; }
    }

    public string LabelNewPasswordText
    {
        get { return litNewPassword.Text; }
        set { litNewPassword.Text = value; }
    }

    public string LabelPasswordText
    {
        get { return litPassword.Text; }
        set { litPassword.Text = value; }
    }

    public string LabelConfirmPasswordText
    {
        get { return litConfirmPassword.Text; }
        set { litConfirmPassword.Text = value; }
    }

    public bool HideDefaulEmailInput { get; set; }
    #endregion

    #region Input Controls

    public string Salutation
    {
        get
        {
            return drpLstSalutation.Text;
        }
        set
        {
            drpLstSalutation.Text = value;
        }
    }

    public string FirstName
    {
        get
        {
            return txtFirstName.Text;
        }
        set
        {
            txtFirstName.Text = value;
        }
    }

    public string LastName
    {
        get
        {
            return txtLastName.Text;
        }
        set
        {
            txtLastName.Text = value;
        }
    }

    public string ContactNumber
    {
        get
        {
            return txtContactNumber.Text;
        }
        set
        {
            txtContactNumber.Text = value;
        }
    }

    public string Mobile
    {
        get
        {
            return txtMobile.Text;
        }
        set
        {
            txtMobile.Text = value;
        }
    }

    public string Email
    {
        get
        {
            return txtEmail.Text;
        }
        set
        {
            txtEmail.Text = value;
        }
    }

    public string AnonymousEmail
    {
        get
        {
            return txtAnonymousEmail.Text;
        }
        set
        {
            txtAnonymousEmail.Text = value;
        }
    }

    public string AccountName
    {
        get
        {
            return txtAccountName.Text;
        }
        set
        {
            txtAccountName.Text = value;
        }
    }

    public string Password
    {
        get
        {
            return txtPassword.Text;
        }
        set
        {
            txtPassword.Text = value;
        }
    }

    #endregion

    public void BindData()
    {
        IsShowSalutation = AppLogic.AppConfigBool("Address.ShowSalutation");

        if (IsAnonymousCheckout)
        {
            txtMobile.Visible = false;
            litMobile.Text = String.Empty;

            litEmailCaption.Text = String.Empty;
            txtEmail.Visible = false;
            litEmail.Text = String.Empty;

            txtAnonymousEmail.Visible = true;
            litAnonymousEmail.Visible = true;

            IsShowSalutation = false;

        }

        if (!IsUseFullnameTextbox && IsShowSalutation)
        {
            drpLstSalutation.Items.Clear();
            AppLogic.AddItemsToDropDownList(ref drpLstSalutation, "Salutation", false, DefaultSalutationText);
        }

        drpLstSalutation.SelectedIndex = drpLstSalutation.ToSelectedIndexByText(SelectedSalutation);

        if(HideDefaulEmailInput){
            txtEmail.Visible = false;
        }

     
       
    }



}