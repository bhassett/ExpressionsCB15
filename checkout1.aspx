<%@ Page Language="C#"  CodeFile="checkout1.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkout1"%>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls"TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="BillingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentTermControl" Src="~/UserControls/PaymentTermControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShippingMethodControl" Src="~/UserControls/ShippingMethodControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ScriptControl" Src="~/UserControls/ScriptControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="OtherPaymentOptionControl" Src="~/UserControls/OtherPaymentOptionControl.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">#errorSummary{display:none;}.CreditCardPaymentMethodPanel tbody tr td{text-align:left;padding-left:22px;}</style>
</head>
<body>
<uc:ScriptControl ID="ctrlScript" runat="server"/>
<ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
    <form id="OnePageCheckout" runat="server">
    <asp:Panel ID="pnlSignIn" runat="server">
        <p><asp:Literal ID="litIfYouHaveAlreadyAnAccount" runat="server">(!checkout1.aspx.88!)</asp:Literal>&nbsp;<a href="signin.aspx"><asp:Literal ID="litSignIn" runat="server">(!checkout1.aspx.89!)</asp:Literal></a></p>
        <div class="clear-both height-12"></div>
    </asp:Panel>   
    <!-- GIFT CARD / GIFT CERTIFICATE -->
    <a href="javascript:void(0)" class="new-giftcode" style="display:none;">
        <i class="icon-gift"></i>
        <asp:Label ID="lblNewGiftCode" runat="server"></asp:Label>
    </a>
    <!-- OTHER PAYMENT CONTROL -->
    <uc:OtherPaymentOptionControl ID="ctrlOtherPaymentOption" runat="server" />
    <%--one page checkout sections starts here --%>
    <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="litShippingDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal></div>
        <%-- div section wrapper starts here --%>
        <div class="section-content-wrapper">
            <%-- A. Shipping section starts here --%>
            <div id="shipping-details-wrapper" class="padding-left-12 padding-right-12">
                <div class="clear-both height-5"></div>
                <div id="step-1-error-place-holder" class="error-place-holder float-left normal-font-style font-size-12"></div> 
                <%-- section for address listing for multiple shipping addresss starts here --%>
                <div><div class="clear-both height-5"></div><asp:Literal ID = "litShippingAddressGrid" runat="server"></asp:Literal></div>
                <%-- section for address listing for multiple shipping addresss ends here --%>
                <div class="clear-both height-12"></div>
                <%-- section for profile and address control starts here --%>
                <div class="shipping-details-place-holder font-normal-style  custom-font-style float-left">
                    <span class="strong-font  custom-font-style"><asp:Literal ID="litShippingContact" runat="server">(!checkout1.aspx.31!)</asp:Literal></span>
                    <div class="clear-both height-5"></div>
                    <uc:ProfileControl id="ProfileControl" runat="server" />
                    <div class="clear-both height-5"></div>
                    <div class="clear-both height-12"></div>
                    <span class="strong-font  custom-font-style"><asp:Literal ID="litShippingAddress" runat="server">(!checkout1.aspx.32!)</asp:Literal></span>
                    <div class="clear-both height-5"></div>
                    <uc:AddressControl id="ShippingAddressControl" IdPrefix="shipping-" runat="server" />
                </div>
                <%-- section for profile and address control ends here --%>
                <div id="shipping-helpful-tips-place-holder" class="font-normal-style  custom-font-style float-left"><ise:Topic ID="OnePageCheckoutHelpfulTips" runat="server" TopicName="OnePageCheckoutHelpfulTips" /></div>
                <div class="clear-both height-5"></div>
                <div class="clear-both height-12"></div>
                <%-- section for continue button starts here --%>
                <div id="shipping-info-button-place-holder"><div id="save-shipping-button"><div id="save-shipping-loader"></div><div id="save-shipping-button-place-holder"><a href="javascript:void(1);"  id="opc-submit-step-1" class="site-button"><asp:Literal ID="litStep1" runat="server">(!checkout1.aspx.82!)</asp:Literal></a></div></div></div>
                <%-- section for continue button ends here --%>
                <div class="clear-both height-5"></div>
                <div class="clear-both height-12"></div>
            </div>
            <%-- shipping section ends here --%>
            <%-- shipping details (hidden on edit shipping) starts here --%>
            <div id="shipping-details-wrapper-hidden" class="padding-left-12 padding-right-12">
                <div class="clear-both height-12"></div>
	            <div class="shipping-details-place-holder font-normal-style  custom-font-style float-left">
                    <%-- details left section --%>
		            <div class="float-left width-half">
			            <span class="strong-font  custom-font-style"><asp:Literal ID="litShippingContactHidden" runat="server">(!checkout1.aspx.31!)</asp:Literal></span>
			            <div class="clear-both height-5"></div>
			            <ul class="ul-list-no-style no-margin padding-left-12"><li id="li-contact"></li><li id="li-phone"></li><li id="li-email"></li></ul>
		            </div>
                    <%-- details right section --%>
		            <div class="float-left width-half">
			            <span class="strong-font  custom-font-style"><asp:Literal ID="litShippingAddressHidden" runat="server">(!checkout1.aspx.32!)</asp:Literal></span>
			            <div class="clear-both height-5"></div>
			            <ul class="ul-list-no-style no-margin padding-left-12"><li id="li-address"></li><li id="li-city-state-postal"><span id="li-span-city"></span>,<span id="li-span-state"></span><span id="li-span-postal"></span></li><li id="li-country"></li><li class="display-none" id="li-county"></li><li id="li-residence"></li></ul>
		            </div>
		            <div class="clear-both height-5"></div>
	            </div>
	            <%-- edit shipping link --%>
                <div><span class="one-page-link-right normal-font-style  float-right custom-font-style"><a  id="edit-shipping" href="javascript:void(1);"><asp:Literal ID="litEditShipping" runat="server">(!checkout1.aspx.33!)</asp:Literal></a></span><div class="clear-both height-5"></div></div>   
                <div class="clear-both height-5"></div>
                <div class="clear-both height-12"></div>
            </div>
            <%-- shipping details (hidden on edit shipping) ends here --%>
            <div id="coupon-free-shipping-text"></div>
            <%-- B. Shipping methods section starts here --%>
            <div id="shipping-methods-wrapper" class="padding-left-12 padding-right-12">
                <div class="clear-both height-12"></div>
                <div id="step-2-error-place-holder" class="error-place-holder float-left normal-font-style font-size-12"></div> 
                <div class="clear-both height-12"></div>
                <span class="strong-font  custom-font-style"><asp:Literal ID="litShippingMethod" runat="server">(!checkout1.aspx.34!)</asp:Literal></span> 
                <span class="one-page-link-right normal-font-style  float-right custom-font-style"><a href="javascript:void(1)" id="edit-shipping-method"><asp:Literal ID="litEditShippingMethod" runat="server">(!checkout1.aspx.35!)</asp:Literal></a></span>
                <div class="clear-both height-12"></div>
                <div id="selected-shipping-method" class="padding-left-12"><asp:Literal ID="litSelectedShippingMethod" runat="server"></asp:Literal></div>
                <div id="available-shipping-methods" class="width-full">
                    <%-- shipping method control starts here --%>
                    <asp:Panel ID="pnlShippingMethod" runat="server"><asp:Label ID="lblSelectShippingMethod" Text="" runat="server" Font-Bold="true" /><br /><uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" /></asp:Panel>
                     <%-- shipping method control ends here --%>
                    <div class="clear-both height-12"></div>
                    <%-- shipping method control continue button starts here --%>
                    <div id="shipping-method-button-place-holder">
                       <div id="save-shipping-method-button"><a href="javascript:void(1);"  id="opc-submit-step-2"" class="site-button"><asp:Literal ID="litStep2" runat="server">(!checkout1.aspx.82!)</asp:Literal></a></div>
                       <div id="save-shipping-method-loader"></div>
                    </div>
                    <%-- shipping method control continue button ends here --%>
                    <div class="clear-both height-12"></div>
                </div>
                <div class="clear-both"></div>
              </div>
              <%-- shipping methods section ends here --%>
              <div class="clear-both"></div>
              <div class="clear-both height-12"></div>
          </div>
          <%-- div section wrapper ends here --%>
          <%-- C. Payments section starts here --%>
          <div class="section-header"><asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.36!)</asp:Literal></div>
          <%-- div section wrapper starts here --%>
          <div class="section-content-wrapper">
           <div class="clear-both"></div>
           <div id="billing-details-wrapper" class="padding-left-12 padding-right-12">
           <div id="step-3-error-place-holder" class="error-place-holder float-left normal-font-style font-size-12"></div>
           <div class="clear-both"></div>
           <asp:Panel runat="server" ID ="pnlNoAvailablePaymentStatus" Visible="false">
               <asp:Literal ID="litTransactionStatusMessage" runat="server"></asp:Literal> <a href="contactus.aspx"><asp:Literal ID="litContactUsLink" runat="server">(!checkout1.aspx.108!)</asp:Literal>.</a>
           </asp:Panel>
           <asp:Panel ID="pnlPaymentSectionWrapper" runat="server">
           <%-- section for address listing for multiple billing addresss and tokenization starts here --%>   
           <asp:Panel ID="pnlBillingAddressGrid" runat="server">
              <span class="strong-font  custom-font-style"><asp:Literal ID="Literal3" runat="server">&nbsp;</asp:Literal></span> 
              <div id="billing-address-grid"><asp:Literal ID="litBillingAddressGrid" runat="server"></asp:Literal></div>
           </asp:Panel> 
           <div id="credit-card-options"> <asp:Literal ID="LtrCreditCardOptionsRenderer" runat="server"></asp:Literal></div>
            <%-- section for address listing for multiple billing addresss and tokenization ends here --%>   

            <%-- section for address / contact form starts here --%>   
            <div  id="billing-details-place-holder">
               <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingContact" runat="server">(!checkout1.aspx.37!)</asp:Literal></span> 
               <span id="copy-shipping-info-place-holder" class="float-right">
                   <asp:CheckBox ID="copyShippingInfo" runat="server"/><span class="checkbox-captions custom-font-style"><asp:Literal ID="litSameAsShippingInfo" runat="server">(!checkout1.aspx.78!)</asp:Literal></span>
               </span>
               <div class="clear-both height-5"></div>

                <%-- section for contact name, area code, and phone controls starts here --%>   
               <div class="form-controls-place-holder">
                    <span class="form-controls-span">
                        <label id="lblBillingContactName" class="form-field-label">
                            <asp:Literal ID="litBillingContactName" runat="server">(!checkout1.aspx.79!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtBillingContactName" MaxLength="100" runat="server" CssClass = "light-style-input"></asp:TextBox>
                    </span>
                     <span class="form-controls-span">
                         <label  id="lblBillingContactNumber" class="form-field-label">
                            <asp:Literal ID="litBillingContactNumber" runat="server">(!checkout1.aspx.66!)</asp:Literal>
                         </label>
                         <asp:TextBox ID="txtBillingContactNumber" MaxLength="50" runat="server" CssClass = "light-style-input"></asp:TextBox>
                     </span>
                </div>
                <%-- section for contact name, area code, and phone controls ends here --%>   

                <div class="clear-both height-12"></div>
                <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingAddress" runat="server">(!checkout1.aspx.38!)</asp:Literal></span>
                <div class="clear-both height-5"></div>
                   
                <%-- section for billing address control starts here --%>    
                <uc:BillingAddressControl id="BillingAddressControl" IdPrefix="billing-" runat="server" />
                <div class="clear-both height-12"></div>

                <div class="form-controls-place-holder">
                    <span class="form-controls-span label-outside" id="age-13-place-holder">
                        <input type="checkbox" id="im-over-13-years-of-age"/> <span class="checkbox-captions"><asp:Literal ID="litImOver13" runat="server">(!checkout1.aspx.39!)</asp:Literal></span>
                    </span>
                </div>
                <%-- section for billing address control ends here --%>    

                <div class="clear-both height-12"></div>
             </div>
             <%-- section for address / contact form ends here --%>   

             <div id="move-credit-card-down-if-width-less-or-1000"></div>
            
             <%-- section for payment methods control starts here --%>   
             <div id="credit-card-details-place-holder normal-font-style ">
             
                <span class="strong-font  custom-font-style"><asp:Literal ID="litPaymentsMethod" runat="server">(!checkout1.aspx.40!)</asp:Literal></span>
                <div class="clear-both height-5"></div>
                <asp:Panel ID="pnlPaymentTerm" runat="server" HorizontalAlign="Center">
                    <uc:PaymentTermControl ID="ctrlPaymentTerm" runat="server"/>
                </asp:Panel> 
             </div>
             <%-- section for payment methods control ends here --%>   
               
             <div class="clear-both height-5"></div>
             <div class="clear-both height-12"></div>

             <%-- payment section continue button starts here --%>   
             <div id="billing-method-button-place-holder">
                <div id="billing-method-button">
                    <a href="javascript:void(1);"  id="opc-submit-step-3" class="site-button"><asp:Literal ID="litStep3" runat="server">(!checkout1.aspx.82!)</asp:Literal></a>
                </div>
                <div id="save-billing-method-loader"></div>
             </div>
              <%-- payment section continue button ends here --%>   
            </asp:Panel> 
              <div class="clear-both height-12"></div>
           </div>

          <%-- billing details section starts here (hidden on edit billing)--%>   
          <div id="billing-details-wrapper-hidden" class="padding-left-12 padding-right-12">
            <div class="clear-both height-12"></div>
            <div class="shipping-details-place-holder font-normal-style  custom-font-style float-left">

            <div class="float-left width-half">
	            <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingContactHidden" runat="server">(!checkout1.aspx.37!)</asp:Literal></span>
	            <div class="clear-both height-5"></div>

	            <ul class="ul-list-no-style no-margin float-left padding-left-12">
		            <li id="li-billing-contact"></li>
		            <li id="li-billing-phone"></li>
	            </ul>
            </div>

            <div class="float-left width-half">
	            <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingAddressHidden" runat="server">(!checkout1.aspx.38!)</asp:Literal></span>
	            <div class="clear-both height-5"></div>

	            <ul class="ul-list-no-style no-margin padding-left-12">
		            <li id="li-billing-address"></li>
		            <li><span id="li-billing-city"></span>,<span id="li-billing-state"></span><span id="li-billing-postal"></span></li>
		            <li id="li-billing-country"></li>
                    <li class="display-none" id="li-billing-county"></li>
	            </ul>
            </div>

            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>

            <div class="strong-font  custom-font-style"><asp:Literal ID="litPaymentsMethodHiddent" runat="server">(!checkout1.aspx.40!)</asp:Literal>:</div>
            <div class="clear-both height-5"></div>
            <ul class="ul-list-no-style no-margin padding-left-12">
                <li id="selected-payments-method"></li>
            </ul>
            </div>
	
            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>
        </div>
       <%-- billing details section ends here --%>   

      </div>
      <%-- div section wrapper ends here --%>

      <div class="clear-both height-12"></div>
       
    </div>
   <%--one page checkout sections ends here --%>

   <%--coupon sections ends here --%>
    <div class="clear-both height-12"></div>
    <asp:Panel ID="pnlCoupon" class="no-margin no-padding" runat="server">
        <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="Literal1" runat="server">(!checkout1.aspx.80!)</asp:Literal></div>
            <div id="divCouponEntered"><asp:Literal ID="Literal2" runat="server">(!checkout1.aspx.81!)</asp:Literal> : <asp:Literal runat="server" ID="litCouponEntered"></asp:Literal></div>
            </div>
    </asp:Panel>
    <%--coupon sections ends here --%>
     
    <div class="clear-both height-12"></div>
    
    <%--cart items and order summary details starts here --%>

    <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="litItemsToBeShipped" runat="server">(!checkout1.aspx.43!)</asp:Literal></div>
          
         <%-- div section wrapper starts here --%>
        <div class="section-content-wrapper">
            <div id="order-summary-head-text" style="padding-left: 23px;padding-right:12px">
                <span class="one-page-link-right normal-font-style  float-right">
                <a href="shoppingcart.aspx" class="custom-font-style"><asp:Literal ID="litEditCart" runat="server">(!checkout1.aspx.44!)</asp:Literal></a></span>
            </div>

            <div class="clear-both height-12"></div>
            <div><asp:Literal ID="OrderSummary" runat="server"></asp:Literal></div>
           
            <div class="clear-both height-12"></div>
            <div id='items-to-be-shipped-footer'><asp:Literal runat="server" ID="litOrderSummaryFooter"></asp:Literal></div>
            <div class="clear-both height-12"></div>
       </div>
        <%-- div section wrapper ends here --%>

    </div>

    <%--cart items and order summary details ends here --%>

    <%-- do not touch--%>

<div class="display-none">
    <asp:Button ID="btnDoProcessPayment" runat="server" Text="" OnClick="btnDoProcessPayment_Click" />       
    <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>
    <asp:TextBox id="txtCode" runat="server"></asp:TextBox>
    <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
    <asp:CheckBox ID="chkSaveCreditInfo" runat="server" Checked="false" />
    <asp:TextBox ID="hidMaskCardNumber" runat="server"></asp:TextBox>
    <asp:TextBox ID="hidCreditCardCode" runat="server"></asp:TextBox>
</div>
<div style="display:none;margin:auto" title="Address Verification"  id="ise-address-verification"></div>
<script type="text/javascript" src="jscripts/minified/onepage.checkout.js"></script>
<script type="text/javascript" src="jscripts/minified/address.control.js"></script>
<script type="text/javascript" src="jscripts/minified/address.verification.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.numeric.js"></script>
<script type="text/javascript">
<!-- reference path : /component/address-verificatio/real-time-address-verification-plugin -->
    $(window).load(function () {
        var basePlugin = new jqueryBasePlugin();
        basePlugin.downloadPlugin('components/address-verification/setup.js', function () {
            var loader = new realtimeAddressVerificationPluginLoader();
            loader.start(function (config) {
                var $plugin = $.fn.RealTimeAddressVerification;
                config.submitButtonID = "btnDoProcessPayment";
                config.isAllowShipping = false;
                config.addressMatchDialogContainerID = "ise-address-verification";
                config.errorContainerId = "payment-form-error-container";
                config.progressContainterId = "save-account-loader";
                config.buttonContainerId = "save-account-button-place-holder";
                config.isWithShippingAddress = false;
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

                config.onepagecheckout = {
                    isTrue: true,
                    buttonShippingSectionID: "",
                    buttonBillingSectionID: ""
                }

                var realTimeAddressVerificationPluginStringKeys = new Object();

                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "checkout1.aspx.90";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "checkout1.aspx.91";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "checkout1.aspx.92";
                realTimeAddressVerificationPluginStringKeys.useShippingAddressProvided = "checkout1.aspx.93";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "checkout1.aspx.94";
                realTimeAddressVerificationPluginStringKeys.selectMatchingShippingAddress = "checkout1.aspx.95";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "checkout1.aspx.96";
                realTimeAddressVerificationPluginStringKeys.progressText = "checkout1.aspx.97";
                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;
                $plugin.setup(config);
            });
        });
    });
  </script>
</form>
</body>
</html>
