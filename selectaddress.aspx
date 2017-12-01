<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.selectaddress" CodeFile="selectaddress.aspx.cs" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>


<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <style>#txtContactName { width: 363px; }</style>
</head>
<body>--%>

      <asp:Panel ID="pnlModalOrderSummary" runat="server" HorizontalAlign="Center" Visible="false">
            <!-- Modal -->
     <div class="modal fade" id="order-summary-items-modal" tabindex="-1" role="dialog">
         <div class="modal-dialog" role="document">
             <div class="modal-content">
                 <div class="modal-header">
                     <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                         <i class="fa fa-times"></i>
                     </button>
                     <h4 class="modal-title">
                         <asp:Literal ID="DetailsLit" runat="server">(!itempopup.aspx.2!)</asp:Literal>
                     </h4>
                 </div>
                 <div class="modal-body">
                     <asp:Literal ID="CheckoutOrderSummaryItemsLiteral" runat="server"></asp:Literal>
                 </div>
                 <div class="modal-footer">
                     <a href="shoppingcart.aspx" class="btn btn-default">
                         <asp:Literal ID="EditCartLit" runat="server">(!checkout1.aspx.30!)</asp:Literal>
                     </a>
                 </div>
             </div>
         </div>
     </div>
      </asp:Panel>
     

      <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
          <%--Steps--%>
          <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>
      </asp:Panel>

   

    <div class="select-address-container">
        <div class="row">
            <div class="col-lg-7">
                <form id="frmAddAddress" runat="server">
      
                             
               <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"> <asp:Literal ID="litEditAddress" runat="server">(!selectaddress.aspx.1!)</asp:Literal></div>
      
        <div class="section-content-wrapper">
                   
        <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
            <cc1:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="false" style="width:660px;"></cc1:InputValidatorSummary>
            <div class="clear-both"></div>
            <asp:Table ID="tblAddressList" CellSpacing="0" CellPadding="0" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                   
                        <asp:Table ID="tblAddressListBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                <asp:Panel ID="pnlNewAddress" runat="server" Visible="false">
                                  
                                   <div class="clear-both height-12"></div>
                                   <div class="float-left" style="width: 60%; padding-left: 12px;">
                                       <span class="support-labels custom-font-style"><asp:Literal ID="litAddressTitle" runat="server">(!selectaddress.aspx.4!)</asp:Literal></span>
                                       <div class="clear-both height-5"></div>
                                        <div class="form-controls-place-holder">
                                            <span class="form-controls-span">
                                                <label id="lblContactName" class="form-field-label">
                                                    <asp:Literal ID="litContactName" runat="server">(!selectaddress.aspx.23!)</asp:Literal>
                                                </label>
                                                <asp:Textbox ID="txtContactName" maxlength="100" class="light-style-input" runat="server"></asp:Textbox>
                                            </span>
                                            <span class="form-controls-span">
                                           <label  id="lblContactNumber" class="form-field-label">
                                                <asp:Literal ID="litContactNumber" runat="server">(!selectaddress.aspx.24!)</asp:Literal>
                                           </label>
                                            <asp:TextBox ID="txtContactNumber"   runat="server" MaxLength="50" class="light-style-input"></asp:TextBox>
                                        </span>
                                        </div>
                                       <div class="clear-both height-12 profile-section-clears"></div>
                                       <uc:AddressControl id="AddressControl"  runat="server" />
                                   </div>
                                   <div class="float-left" style="width:35%">
                                         <ise:Topic ID="AddressBookHelpfulTips" runat="server" TopicName="AddressBookHelpfulTips" />
                                   </div>

                                   <div class="clear-both height-12"></div> 
                                   <div class="clear-both height-12"></div> 

                                 </asp:Panel>
                                    <asp:Panel ID="pnlAddressListMain" runat="server" Visible="true">
                                        <br />
                                        <ol>
                                        <asp:Repeater ID="AddressList" runat="server">
                                            <ItemTemplate>
                                                <li class="well">
                                                    <%#  InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "<b>", "")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Name").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Name").ToString()) %>   
                                                            
                                                            &nbsp;&nbsp;
                                                         
                                                            <asp:Button ID="btnEdit" Text="" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' CommandName="edit" runat="server"   class="btn btn-default button-xs" /> 
                                                            &nbsp;&nbsp;
                                                         
                                                            <asp:Button ID="btnMakePrimary" Text="" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' CommandName="makeprimary" runat="server"   class="btn btn-success button-xs" /> 
                                                            
                                                            <br />                                                 
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Address").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Address") + "<br />")%>                                                    
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "CityStateZip").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "CityStateZip") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Country").ToString().Trim() == "" || DataBinder.Eval(Container.DataItem, "IsHomeCountry").ToString() == "1", "", DataBinder.Eval(Container.DataItem, "Country") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Telephone").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Telephone") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "County").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "County") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "</b>", "")%>
                                                    <br />
                                                 </li>
                                            </ItemTemplate>
                                        </asp:Repeater>                                  
                                        <li class="well">
                                            <asp:Panel ID="liAdd" runat="server" Visible="true">
                                                <asp:HyperLink ID="lnkAddAddress" CssClass="btn btn-default" runat="server"></asp:HyperLink>
                                            </asp:Panel>
                                        </li>
                                        </ol>
                                    </asp:Panel>
                              </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                   
                      <div style="padding-right:12px;" class="button-place-holder">
                       <div id="return-address-button">
                            <div id="return-address-button-place-holder">
                                <div class="row">
                                <div class="col-sm-6">
                                    <asp:Panel runat="server" CssClass="btn-sep btn-block">
                                        <asp:Button ID="btnReturn" runat="server" CssClass="content btn btn-default btn-block" />
                                        <asp:Button ID="btnCheckOut" runat="server" Visible="false" CssClass="content btn btn-default btn-block" />
                                
                                </asp:Panel>
                                </div>
                                <div class="col-sm-6">
                                    <asp:Panel ID="pnlSaveAddress" CssClass="btn-sep btn-block" style="padding-right:5px;" runat="server" Visible="false">
                                         <div id="save-address-button">
                                            <div id="save-address-loader"></div>
                                            <div id="save-address-button-place-holder">
                                                <input type="button" class="content btn btn-primary btn-block" id="save-address" 
                                                    data-contentKey="selectaddress.aspx.16"
                                                    data-contentValue="<%=AppLogic.GetString("selectaddress.aspx.16", true)%>"
                                                    data-contentType="string resource"
                                                    Value="<%=AppLogic.GetString("selectaddress.aspx.16", true)%>" 
                                                />
                                            </div>
                                         </div>
                                     </asp:Panel>
                                     </div>
                                </div>
                            </div>
                       </div>
                       <div class="clear-both height-12"></div>
                    </div>

                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
            </div>
                   </div>
        <asp:Panel ID="pnlNoAddresses" runat="server" Visible="false"><asp:Literal ID="litNoAddresses" runat="server" Mode="PassThrough"></asp:Literal></asp:Panel>

       <!-- do not remove or modify / start here -->
        <div class="display-none">
            <input type="hidden" id="hidSkinID" runat="server" />
            <input type="hidden" id="hidLocale" runat="server" />      
            <input type="hidden" id="load-at-page" value="select-address" />
            <asp:Button ID="btnNewAddress" runat="server" CssClass="btn btn-default" OnClick="btnNewAddress_Click" />
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
                config.submitButtonID = "btnNewAddress";
                config.addressMatchDialogContainerID = "ise-address-verification";
                config.errorContainerId = "errorSummary";
                config.progressContainterId = "save-address-loader";
                config.buttonContainerId = "save-address-button-place-holder";
                config.isWithShippingAddress = false;
                config.isAllowShipping = false;
                config.billingInputID = { POSTAL_CODE: "AddressControl_txtPostal", CITY: "AddressControl_txtCity", STATE: "AddressControl_txtState", COUNTRY: "AddressControl_drpCountry", STREET_ADDRESS: "AddressControl_txtStreet", CITY_STATE_SELECTOR: "city-states" };
                config.billingLabelID = { POSTAL_CODE: "AddressControl_lblStreet", CITY: "AddressControl_lblCity", STATE: "AddressControl_lblState", STREET_ADDRESS: "AddressControl_lblPostal" };
                config.shippingInputID = { POSTAL_CODE: "", CITY: "", STATE: "", COUNTRY: "", STREET_ADDRESS: "", RESIDENCE_TYPE: "AddressControl_drpType", CITY_STATE_SELECTOR: "" };
                var realTimeAddressVerificationPluginStringKeys = new Object();
                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "selectaddress.aspx.30";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "selectaddress.aspx.31";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "selectaddress.aspx.32";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "selectaddress.aspx.33";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "selectaddress.aspx.34";
                realTimeAddressVerificationPluginStringKeys.progressText = "selectaddress.aspx.35";
                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;
                $plugin.setup(config);
            });
        });
    });
    </script>
</form>

            </div>
            <div class="col-lg-5">
                 <asp:Panel ID="pnlOrderSummary" runat="server" HorizontalAlign="Center" Visible="false">
                     <asp:Literal ID="OrderSummaryCardLiteral" runat="server"></asp:Literal>
                 </asp:Panel>
            </div>
        </div>

    </div>

 
<%--    
</body>
</html>--%>