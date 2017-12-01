<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="createaccount.aspx.cs" Inherits="InterpriseSuiteEcommerce.createaccount" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShippingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="BillingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>
</head>
<body>
<ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
<form id="createAccount" runat="server">
  <div class="clear-both"></div>
  <%--  <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="119" Bottom="90" HotSpotMode="Navigate"
                NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
       <div class="clear-both height-5"></div>
   </asp:Panel>--%>
  <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>
  <asp:Panel ID="pnlPageContentWrapper" runat="server">
    <div class="sections-place-holder no-padding">
      <div class="section-header section-header-top">
        <asp:Literal ID="Literal2" runat="server">(!createaccount.aspx.105!)</asp:Literal>
      </div>
      <div class="section-content-wrapper">
        <div class="clr height-12"></div>
        <asp:Panel ID="pnlPageHeaderPlaceHolder" runat="server">
          <asp:Panel  id="pnlPageTitle" runat="server">
            <p id="pCreateAccountPageTips">
              <asp:Literal ID="litCreateAccountPageTips" runat="server">(!createaccount.aspx.106!)</asp:Literal>
            </p>
            </asp:Panel>
          <div>
            <div class="clear-both height-12"></div>
            <p>
              <asp:Literal ID="litIfYouHaveAlreadyAnAccount" runat="server">(!createaccount.aspx.107!)</asp:Literal>
              &nbsp; <a href="signin.aspx" class="btn btn-default btn-sm">
              <asp:Literal ID="litSignIn" runat="server">(!createaccount.aspx.144!)</asp:Literal>
              </a></p>
          </div>
          <div class="clear-both height-12"></div>
          </asp:Panel>
        <asp:Panel ID="pnlResetForm" runat="server" Visible=false>
          <div>
            <p>
              <asp:Literal ID="litResetString1" runat="server">(!createaccount.aspx.146!)</asp:Literal>
              &nbsp; <a href="javascript:void(1);" id="linkResetSignUpForm">
              <asp:Literal ID="litResetString2" runat="server">(!createaccount.aspx.147!)</asp:Literal>
              </a>
              <asp:Literal ID="litResetString3" runat="server">(!createaccount.aspx.148!)</asp:Literal>
            </p>
          </div>
          </asp:Panel>
        <div id="divFormWrapper" class="row">
          <div id="divFormContainer">
            <%-- create account form left content starts here --%>
            <div id="divFormLeft" class="float-left"> 
            <div class="col-md-6">
            <span class="form-section">
              <asp:Literal ID="LitYourProfileInto" runat="server">(!createaccount.aspx.108!)</asp:Literal>
              </span> 
              <!-- Profile Info Section Starts Here -->
              <div class="clear-both height-12 profile-section-clears"></div>
              <uc:ProfileControl id="ProfileControl"  runat="server" />
              <div class="clear-both height-12 profile-section-clears"></div>
              <!-- Profile Info Section Ends Here --> 
              </div>
              <div class="col-md-6">
              <!-- Billing Info Section Starts Here --> 
              <span class="form-section">
              <asp:Literal ID="litBillingInfo" runat="server">(!createaccount.aspx.109!)</asp:Literal>
              </span>
              <div class="clear-both height-12"></div>
              <uc:BillingAddressControl id="BillingAddressControl" IdPrefix="billing-" runat="server" />
              <div class="clear-both height-12"></div>
              
              <!-- Billing Info Section Ends Here -->
              
              <div class="clear-both height-12 shipping-section-clears"></div>
              
              <!-- Shipping Info Section Starts Here -->
              
              <div class="form-section" id="shipping-section-head-place-holder"> <span id="lit-shipping-info"  class="float-left custom-font-style">
                <asp:Literal ID="litShippingInfo" runat="server">(!createaccount.aspx.110!)</asp:Literal>
                </span> <span class="float-right" id="copy-billing-info-place-holder">
                <asp:CheckBox ID="copyBillingInfo" runat="server"/>
                <span class="checkbox-captions custom-font-style">
                <asp:Literal ID="litSameAsBillingInfo" runat="server">(!createaccount.aspx.111!)</asp:Literal>
                </span> </span> </div>
              <div class="clear-both height-12 shipping-section-clears"></div>
              <div id="shipping-info-place-holder">
                <uc:ShippingAddressControl id="ShippingAddressControl" IdPrefix="shipping-" runat="server" />
              </div>
              <div class="clear-both height-12 shipping-section-clears"></div>
              <!-- Shipping Info Section Ends Here --> 
              </div>
              <div class="col-xs-12">
              <!-- Account Info Section Starts Here -->
              
              <div class="clear-both height-12 account-info-sections-clears"></div>
              <div id="account-section-wrapper"> <span class="form-section">
                <asp:Literal ID="litAdditionaInfo" runat="server">(!createaccount.aspx.112!)</asp:Literal>
                </span>
                <div class="clear-both height-12"></div>
                <asp:Panel ID="pnlProductUpdates" runat="server" Visible="false">
                  <div class="form-controls-place-holder"> <span class="form-controls-span label-outside">
                    <asp:CheckBox ID="chkProductUpdates" runat="server" Checked="true"/>
                    <span class="checkbox-captions custom-font-style">
                    <asp:Literal ID="litProductUpdates" runat="server">(!createaccount.aspx.113!)</asp:Literal>
                    </span> </span> </div>
                  </asp:Panel>
                <div class="clear-both height-5"></div>
                <div class="form-controls-place-holder" style="display:none;"> <span class="form-controls-span label-outside" id="age-13-place-holder">
                  <asp:CheckBox ID="chkOver13" runat="server"/>
                  <span class="checkbox-captions custom-font-style">
                  <asp:Literal ID="litImOver13" runat="server">(!createaccount.aspx.143!)</asp:Literal>
                  </span> </span> </div>
              </div>
              <div class="clear-both height-12 account-info-sections-clears"></div>
              
              <!-- Account Info Section Ends Here -->
              
              <div class="clear-both height-5"></div>
              
              <!-- Captcha Section Starts Here -->
              
              <div class="form-controls-place-holder captcha-section"> <span class="form-controls-span custom-font-style capitalize-text" id="create-account-captcha-label">
                <asp:Literal ID="LitEnterSecurityCodeBelow" runat="server">(!createaccount.aspx.149!)</asp:Literal>
                </span> <span class="form-controls-span">
                <label id="lblCaptcha" class="form-field-label">
                  <asp:Literal ID="litCaptcha" runat="server">(!createaccount.aspx.150!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtCaptcha" runat="server" class="light-style-input"></asp:TextBox>
                </span> </div>
              <div class="clear-both height-5  captcha-section"></div>
              <div class="form-controls-place-holder  captcha-section">
                <div id="create-account-captcha-wrapper" class="float-right">
                  <div id="captcha-image"> <img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/> </div>
                  <div id="captcha-refresh"> <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a> </div>
                </div>
              </div>
              <div class="clear-both height-5  captcha-section"></div>
              
              <!-- Captcha Section Ends Here --> 
              
            </div>
            <%-- case form left content ends here --%>
            <%-- case form  right content starts here --%>
            <div id="divFormRight" class="float-right">
              <ise:Topic ID="CreateAccountHelpfulTips" runat="server" TopicName="CreateAccountHelpfulTips" />
            </div>
            <%-- case form  right content ends here --%>
          </div>
          </div>
          <div class="clear-both height-5"></div>
          <div class="clear-both height-12"></div>
          <div class="col-xs-12">
          <div id="account-form-button-place-holder" class="button-place-holder">
            <div id="save-account-button">
              <div id="save-account-loader"></div>
              <div id="save-account-button-place-holder">
              <div class="btn-sep">
                <input type="button"  id="create-customer-account" 
                        class="btn btn-success content" 
                        data-contentKey="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  "createaccount.aspx.25",  "createaccount.aspx.24")%>" 
                        data-contentType="string resource"
                        data-contentValue="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  AppLogic.GetString("createaccount.aspx.25", true),  
                                            AppLogic.GetString("createaccount.aspx.24", true))%>"
                        value="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  AppLogic.GetString("createaccount.aspx.25", true),  
                                            AppLogic.GetString("createaccount.aspx.24", true))%>"/>
              </div>
              </div>
            </div>
          </div>
          </div>
          <div class="display-none">
            <asp:Button ID="btnCreateAccount" runat="server" Text="" OnClick="btnCreateAccount_Click" />
            <asp:TextBox ID="billingTxtCityStates" runat="server"></asp:TextBox>
            <asp:TextBox ID="shippingTxtCityStates" runat="server"></asp:TextBox>
          </div>
        </div>
        <div class="clr height-5"></div>
      </div>
    </div>
    </asp:Panel>
  <%-- 
        do not touch the following html script, the following elements are used in overriding postal listing dialog assignment of values to city and zip
        see jscripts/jquery.cbe.address.verification.js updateAddressInputValues function
        
    --%>
  <div id="submit-case-caption" style="display:none;">
    <asp:Literal ID="LtrCreateAccount_Caption" runat="server"></asp:Literal>
  </div>
  <div style="display:none;margin:auto" title="Address Verification" id="ise-address-verification-for-create-account"></div>
  <input type="hidden" id="load-at-page" value="create-account" />
  <script type="text/javascript" src="jscripts/minified/address.control.js"></script> 
  <script type="text/javascript" src="jscripts/minified/address.verification.js"></script> 
  <script type="text/javascript" src="jscripts/minified/customer.js"></script> 
  <script type="text/javascript">
    <!-- reference path : /component/address-verificatio/real-time-address-verification-plugin -->
    $(window).load(function () {
        var basePlugin = new jqueryBasePlugin();
        basePlugin.downloadPlugin('components/address-verification/setup.js', function () {

            var loader = new realtimeAddressVerificationPluginLoader();
            loader.start(function (config) {

                var $plugin = $.fn.RealTimeAddressVerification;

                config.submitButtonID = "btnCreateAccount";
                config.isAllowShipping = $plugin.toBoolean(ise.Configuration.getConfigValue("AllowShipToDifferentThanBillTo"));
                config.addressMatchDialogContainerID = "ise-address-verification-for-create-account";
                config.errorContainerId = "errorSummary";
                config.progressContainterId = "save-account-loader";
                config.buttonContainerId = "save-account-button-place-holder";
                config.isWithShippingAddress = $plugin.toBoolean(ise.Configuration.getConfigValue("AllowShipToDifferentThanBillTo"));
                config.billingInputID = {
                    POSTAL_CODE: "BillingAddressControl_txtPostal",
                    CITY: "BillingAddressControl_txtCity",
                    STATE: "BillingAddressControl_txtState",
                    COUNTRY: "BillingAddressControl_drpCountry",
                    STREET_ADDRESS: "BillingAddressControl_txtStreet",
                    CITY_STATE_SELECTOR: "billing-city-states"
                };

                config.billingLabelID = {
                    POSTAL_CODE: "BillingAddressControl_lblStreet",
                    CITY: "BillingAddressControl_lblCity",
                    STATE: "BillingAddressControl_lblState",
                    STREET_ADDRESS: "BillingAddressControl_lblPostal"
                };

                config.shippingInputID = {
                    POSTAL_CODE: "ShippingAddressControl_txtPostal",
                    CITY: "ShippingAddressControl_txtCity",
                    STATE: "ShippingAddressControl_txtState",
                    COUNTRY: "ShippingAddressControl_drpCountry",
                    STREET_ADDRESS: "ShippingAddressControl_txtStreet",
                    RESIDENCE_TYPE: "ShippingAddressControl_drpType",
                    CITY_STATE_SELECTOR: "shipping-city-states"
                };

                config.shippingLabelID = {
                    POSTAL_CODE: "ShippingAddressControl_lblStreet",
                    CITY: "ShippingAddressControl_lblCity",
                    STATE: "ShippingAddressControl_lblState",
                    STREET_ADDRESS: "ShippingAddressControl_lblPostal"
                };



                var realTimeAddressVerificationPluginStringKeys = new Object();

                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "createaccount.aspx.114";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "createaccount.aspx.115";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "createaccount.aspx.116";
                realTimeAddressVerificationPluginStringKeys.useShippingAddressProvided = "createaccount.aspx.117";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "createaccount.aspx.118";
                realTimeAddressVerificationPluginStringKeys.selectMatchingShippingAddress = "createaccount.aspx.119";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "createaccount.aspx.152";
                realTimeAddressVerificationPluginStringKeys.progressText = "createaccount.aspx.153";

                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;

                $plugin.setup(config);
            });
        });
    });
   </script>
  <%-- do not touch <-- --%>
</form>
</body>
</html>