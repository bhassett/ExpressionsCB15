<%@ Page Language="C#"  CodeFile="leadform.aspx.cs" Inherits="InterpriseSuiteEcommerce.LeadForm" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls"TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
     <title></title>
     <style>
      #passwords-wrapper{display:none}
      #account-name-wrapper{display:none}
      #support-security-code {width: 211px;}
      #support-captcha-wrapper {padding-right: 51px;}
      #captcha-label {padding-right: 9px;}
      .accounts-clear{display:none}
     </style>
</head>
<body>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
    <div class="clear-both" id="error-summary-clear"></div>
    <form id="frmLeadForm" runat="server">
    <asp:Panel ID="pnlPageContentWrapper" runat="server" CssClass="row">
      <div class="col-md-6 col-md-offset-3">
        <div class="section-header section-header-top"><asp:Literal ID="Literal2" runat="server">(!leadform.aspx.22!)</asp:Literal></div>
      
        <div class="section-content-wrapper">
            <div class="clear-both height-12"></div>
            <div id="lead-form-logo"></div>

        <div id="divFormContainer">
           <%-- lead form left content starts here --%>
            <div id="divFormLeft" class="float-left">
                <span class="form-section">
                    <asp:Literal ID="LtrYourProfileSectionHeader_Caption" runat="server">(!leadform.aspx.31!)</asp:Literal>
                </span>

               <div class="clear-both height-12"></div>

               <uc:ProfileControl id="ProfileControl"  runat="server" />
      
               <div class="clear-both height-12"></div>

               <!-- Your Profile Section Ends Here -->

               <!-- Your Address Section Starts Here -->

               <span class="form-section">
                    <asp:Literal ID="LtrYourAddressHeader_Caption" runat="server">(!leadform.aspx.35!)</asp:Literal>
               </span>
               <div class="clear-both height-12"></div>
              
               <uc:AddressControl id="AddressControl"  runat="server" />
               
               <div class="clear-both height-12"></div>

               <!-- Your Address Section Ends Here -->

               <!-- Your Message Section Starts Here -->

               <div class="clear-both height-5"></div>

               <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                       <label id="lblMessage"  class="form-field-label">
                             <asp:Literal ID="litMessage" runat="server">(!leadform.aspx.13!)</asp:Literal>
                       </label>
                         <asp:TextBox  ID="txtMessage" rows="5" TextMode="MultiLine" runat="server" class="light-style-input" Columns="55"></asp:TextBox>
                    </span>

                </div>
                
               <!-- Your Message Section Ends Here -->
               <asp:Panel runat="server" ID="pnlSecurityCode">
                <div class="clear-both height-5"></div>

                <div class="form-controls-place-holder">

                    <span class="form-controls-span custom-font-style capitalize-text" id="lead-form-captcha-label">
                        <asp:Literal ID="LtrEnterSecurityCodeBelow_Caption" runat="server">(!customersupport.aspx.12!)</asp:Literal>
                    </span>

                     <span class="form-controls-span">
                       <label id="lblCaptcha" class="form-field-label">
                            <asp:Literal ID="litCaptcha" runat="server">(!leadform.aspx.34!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtCaptcha" runat="server" class="light-style-input"></asp:TextBox>
                    </span>

                </div>
                  <div class="clear-both height-5"></div>
                 <div class="form-controls-place-holder">

                   <div id="lead-form-captcha-wrapper">
                        <div id="captcha-image">
                            <img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/>
                         </div>
                         <div id="captcha-refresh">
                            <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a>
                         </div>
                    </div>
                </div>

                <div class="clear-both height-5"></div>
                </asp:Panel>
            </div>

            <%-- lead form left content ends here --%>

            <%-- lead form  right content starts here --%>

            <div id="divFormRight" class="float-right">
                <ise:Topic ID="LeadFormHelpfulTipsTopic" runat="server" TopicName="LeadFormHelpfulTips" />
            </div>
           
            <%-- lead form  right content ends here --%>

        </div>
        <div class="clear-both height-5"></div>
        <div id="lead-form-button-place-holder" class="button-place-holder">
             <div id="save-lead-button">
                <div id="save-lead-loader"></div>
                    <div id="save-lead-button-place-holder">
                        <input type="button" id="btnSubmitLF" value="<%=AppLogic.GetString("leadform.aspx.15", true)%>" class="btn btn-default btn-block content" data-contentKey="leadform.aspx.15" data-contentType="string resource" data-contentValue="<%=AppLogic.GetString("leadform.aspx.15", true)%>" />
                    </div>
               </div>
         </div>
            <div class="clear-both height-5"></div>
         </div>
        </div>
    </asp:Panel>

    <%-- 
        do not touch the following html script, the following elements are used in overriding postal listing dialog assignment of values to city and zip
        see jscripts/jquery/address.verification.js updateAddressInputValues function
    --%>
    <div class="display-none">
         <asp:Button ID="btnSaveLead" runat="server" Text="" OnClick="btnSaveLead_Click" />
         <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>
         <input type="hidden" id="process-status"/>             
    </div>
    <div style="display:none;margin:auto" title="Address Verification"  id="ise-address-verification"></div>
    <%-- do not touch <-- --%>
    <script type="text/javascript" src="jscripts/minified/address.control.js"></script>
    <script type="text/javascript" src="jscripts/minified/address.verification.js"></script>
    <script type="text/javascript" src="jscripts/minified/leads.js"></script>
    <script type="text/javascript">
    <!-- reference path : /component/address-verificatio/real-time-address-verification-plugin -->
    $(window).load(function () {
        var basePlugin = new jqueryBasePlugin();
        basePlugin.downloadPlugin('components/address-verification/setup.js', function () {

            var loader = new realtimeAddressVerificationPluginLoader();
            loader.start(function (config) {

                var $plugin = $.fn.RealTimeAddressVerification;

                config.submitButtonID = "btnSaveLead";
                config.addressMatchDialogContainerID = "ise-address-verification";
                config.errorContainerId = "errorSummary";
                config.progressContainterId = "save-lead-loader";
                config.buttonContainerId = "save-lead-button-place-holder";
                config.isWithShippingAddress = false;
                config.isAllowShipping = false;
                config.billingInputID = {
                    POSTAL_CODE: "AddressControl_txtPostal",
                    CITY: "AddressControl_txtCity",
                    STATE: "AddressControl_txtState",
                    COUNTRY: "AddressControl_drpCountry",
                    STREET_ADDRESS: "AddressControl_txtStreet",
                    CITY_STATE_SELECTOR: "city-states"
                };

                config.billingLabelID = {
                    POSTAL_CODE: "AddressControl_lblStreet",
                    CITY: "AddressControl_lblCity",
                    STATE: "AddressControl_lblState",
                    STREET_ADDRESS: "AddressControl_lblPostal"
                };

                config.shippingInputID = {
                    POSTAL_CODE: "",
                    CITY: "",
                    STATE: "",
                    COUNTRY: "",
                    STREET_ADDRESS: "",
                    RESIDENCE_TYPE: "AddressControl_drpType",
                    CITY_STATE_SELECTOR: ""
                };


                var realTimeAddressVerificationPluginStringKeys = new Object();

                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "leadform.aspx.37";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "leadform.aspx.38";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "leadform.aspx.39";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "leadform.aspx.40";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "leadform.aspx.41";
                realTimeAddressVerificationPluginStringKeys.progressText = "leadform.aspx.42";

                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;

                $plugin.setup(config);
            });
        });
    });
    </script>
    </form>
</body>
</html>