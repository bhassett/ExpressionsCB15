<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.editaddress" CodeFile="editaddress.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="frmEditAddress" runat="server">
        <asp:Literal ID="litswitchformat" runat="server" Mode="PassThrough"></asp:Literal>
        <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
            </asp:ImageMap>
            <br />
            <br />
        </asp:Panel>
        <cc1:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="false" Style="width: 660px;"></cc1:InputValidatorSummary>
        <div class="clear-both"></div>
                    <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"> <asp:Literal ID="litEditAddress" runat="server">(!editaddress.aspx.14!)</asp:Literal></div>
      
        <div class="section-content-wrapper">


        <asp:Panel ID="pnlAddress" runat="server" Visible="true">  
            <table cellpadding="0" id="tblAccountInfo" runat="server" cellspacing="0" width="100%">
                <tr>
                    <td>

                        <table cellpadding="2" id="tblAccountInfoBox" runat="server" cellspacing="0" width="100%">
                            <tr>
                                <td>

                                    <div class="clear-both height-12"></div>
                                    <div class="float-left" style="width: 60%; padding-left: 12px;">
                                        <span class="support-labels custom-font-style">
                                            <asp:Literal ID="litAddressTitle" runat="server">(!editaddress.aspx.15!)</asp:Literal></span>
                                        <div class="clear-both height-5"></div>
                                        <div class="form-controls-place-holder">
                                            <span class="form-controls-span">
                                                <label id="lblContactName" class="form-field-label">
                                                    <asp:Literal ID="litContactName" runat="server">(!editaddress.aspx.11!)</asp:Literal>
                                                </label>
                                                <asp:TextBox ID="txtContactName" MaxLength="100" class="light-style-input" runat="server"></asp:TextBox>
                                            </span>

                                            <span class="form-controls-span">
                                                <label id="lblContactNumber" class="form-field-label">
                                                    <asp:Literal ID="litContactNumber" runat="server">(!editaddress.aspx.12!)</asp:Literal>
                                                </label>
                                                <asp:TextBox ID="txtContactNumber" runat="server" MaxLength="50" class="light-style-input"></asp:TextBox>
                                            </span>
                                        </div>
                                        <div class="clear-both height-5"></div>

                                        <div class="clear-both height-12 profile-section-clears"></div>
                                        <uc:AddressControl ID="AddressControl" runat="server" />
                                    </div>
                                    <div class="float-left" style="width: 35%">
                                        <ise:Topic ID="AddressBookHelpfulTips" runat="server" TopicName="AddressBookHelpfulTips" />
                                    </div>

                                    <div class="clear-both height-12"></div>
                                    <div class="clear-both height-12"></div>

                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td>

                        <div style="padding-right: 12px;" class="button-place-holder">
                            <div id="return-address-button">
                                <div id="return-address-button-place-holder">
                                    <div class="row">
                                    <div class="col-sm-6">
                                        <asp:Panel ID="Panel1" runat="server" CssClass="btn-sep btn-block">
                                            <asp:Button ID="btnReturn" runat="server" CssClass="btn btn-default content btn-block" />
                                            <asp:Button ID="btnCheckOut" runat="server" Visible="false" CssClass="btn btn-default btn-block" />
                                        </asp:Panel>
                                        </div>
                                    <div class="col-sm-6">
                                        <asp:Panel ID="pnlUpdasteAddress" CssClass="btn-sep btn-block" Style="padding-right: 5px;" runat="server">
                                            <div id="update-address-button">
                                                <div id="update-address-loader"></div>
                                                <div id="update-address-button-place-holder">
                                                    <input type="button" id="save-address" class="btn btn-primary content btn-block"
                                                        data-contentkey="editaddress.aspx.3"
                                                        data-contentvalue="<%=AppLogic.GetString("editaddress.aspx.3", true)%>"
                                                        data-contenttype="string resource"
                                                        value="<%=AppLogic.GetString("editaddress.aspx.3", true)%>" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        </div>
                                        
                                    </div>
                                </div>

                            </div>
                            <div class="clear-both height-12"></div>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
            </div>
                        </div>
        
       <!-- do not remove or modify / start here -->
        <div class="display-none">
            <input type="text" id="hidSkinID" runat="server" />
            <input type="text" id="hidLocale" runat="server" />
            <input type="text" id="txtPhone" runat="server" />
            <input type="text" id="load-at-page" value="edit-address" />
            <asp:Button ID="btnUpdateAddress" runat="server" OnClick="btnUpdateAddress_Click"/>
            <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>
        </div>
        <div style="display:none;margin:auto" title="Address Verification"  id="ise-address-verification"></div>
        <!-- do not remove or modify / ends here -->
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

                config.submitButtonID = "btnUpdateAddress";
                config.addressMatchDialogContainerID = "ise-address-verification";
                config.errorContainerId = "errorSummary";
                config.progressContainterId = "update-address-loader";
                config.buttonContainerId = "update-address-button-place-holder";
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

                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "editaddress.aspx.18";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "editaddress.aspx.19";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "editaddress.aspx.20";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "editaddress.aspx.21";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "editaddress.aspx.22";
                realTimeAddressVerificationPluginStringKeys.progressText = "editaddress.aspx.23";

                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;

                $plugin.setup(config);
            });
        });
    });
    </script>
   </form>
</body>
</html>
