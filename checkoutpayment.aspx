<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutpayment.aspx.cs"  Inherits="InterpriseSuiteEcommerce.checkoutpayment" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls"
    TagPrefix="ise" %>

<%@ Register TagPrefix="uc" TagName="BillingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentTermControl" Src="~/UserControls/PaymentTermControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="OtherPaymentOptionControl" Src="~/UserControls/OtherPaymentOptionControl.ascx" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ OutputCache Location="None" NoStore="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        #save-as-credit-place-holder{display:none;}
        #errorSummary{display:none;}
        .CreditCardPaymentMethodPanel tbody tr td{text-align:left;}
    </style>
</head>
<body>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error" runat="server" Register="False" />

    <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx"
                Top="0" Left="0" Bottom="90" Right="111" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true"
                Top="0" Left="119" Bottom="90" Right="218" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx"
                Top="0" Left="223" Bottom="90" Right="336" />
        </asp:ImageMap>
    </asp:Panel>

    <div class="clear-both height-12"></div>
    
    <form id="frmCheckOutPayment" runat="server">
       
    <asp:Panel ID="pnlPageWrapper" runat="server">

    <!-- GIFT CARD / GIFT CERTIFICATE -->
    <a href="javascript:void(0)" class="new-giftcode" style="display:none;">
        <i class="icon-gift"></i>
        <asp:Label ID="lblNewGiftCode"  runat="server"></asp:Label>
    </a>

    <!-- OTHER PAYMENT CONTROL -->
    <uc:OtherPaymentOptionControl ID="ctrlOtherPaymentOption" runat="server" />

    <!-- Billing Address and Payment Control Wrapper } -->   
      <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="litPaymentDetails" runat="server">(!checkoutpayment.aspx.34!)</asp:Literal></div>
        <!-- Wrapper Padding { -->
        <div class="section-content-wrapper"> 
            <!-- Billing Address Section { -->
            <div id="payment-form-error-container" class="error-place-holder float-left normal-font-style font-size-12"></div>     
            
           <asp:Panel ID="pnlBillingAddressGrid" runat="server">
               <span class="strong-font  custom-font-style">
                   <asp:Literal ID="Literal3">&nbsp;</asp:Literal></span>
               <div id="billing-address-grid">
                   <asp:Literal ID="litBillingAddressGrid" runat="server"></asp:Literal>
               </div>
           </asp:Panel> 
                
           <div id="credit-card-options">
                <asp:Literal ID="LtrCreditCardOptionsRenderer" runat="server"></asp:Literal>
           </div>

            <div  id="billing-details-place-holder" class="normal-font-style font-size-12 float-left">
                <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingContact" runat="server">(!checkoutpayment.aspx.35!)</asp:Literal></span> 
                <div class="clear-both height-5"></div>

                    <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                        <label id="lblBillingContactName" class="form-field-label">
                            <asp:Literal ID="litBillingContactName" runat="server">(!checkoutpayment.aspx.26!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtBillingContactName" MaxLength="100" runat="server" CssClass="light-style-input"></asp:TextBox>
                    </span>

                     <span class="form-controls-span">
                        <label  id="lblBillingContactNumber" class="form-field-label">
                            <asp:Literal ID="litContactNumber" runat="server" >(!checkout1.aspx.66!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtBillingContactNumber" runat="server" CssClass="light-style-input" MaxLength="50"></asp:TextBox>
                     </span>

                    </div>

                    <div class="clear-both height-12"></div>

                    <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingAddress" runat="server">(!checkoutpayment.aspx.36!)</asp:Literal></span>
                    <div class="clear-both height-5"></div>
                    <uc:BillingAddressControl id="BillingAddressControl" IdPrefix="billing-" runat="server" />
                <div class="clear-both height-12"></div>
             </div>

             <!-- Billing Address Section } -->
              <div class="clear-both height-12"></div>
             <div id="move-credit-card-down-if-width-less-or-1000"></div>

             <!-- Payment Control { -->

             <div id="credit-card-details-place-holder-checkout-payment" class="custom-font-style ">      
               
                <span class="strong-font  custom-font-style"><asp:Literal ID="litPaymentsMethod" runat="server">(!checkoutpayment.aspx.37!)</asp:Literal></span>

                <asp:Panel runat="server" ID ="pnlNoAvailablePaymentStatus" Visible="false">
                    <asp:Literal ID="litTransactionStatusMessage" runat="server"></asp:Literal> <a href="contactus.aspx"><asp:Literal ID="litContactUsLink" runat="server">(!checkoutpayment.aspx.64!)</asp:Literal>.</a>
                </asp:Panel>
                 
                 <div class="clear-both height-12"></div>

                <asp:Panel ID="pnlPaymentTerm" runat="server" HorizontalAlign="Center">
                    <uc:PaymentTermControl ID="ctrlPaymentTerm" runat="server"></uc:PaymentTermControl>
                </asp:Panel> 
          
            </div>
            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>
               
            <!-- Payment Control { -->
        </div>
        <!-- Wrapper Padding } -->
    </div>
    <!-- Billing Address and Payment Control Wrapper } -->

    <div class="clear-both height-12"></div>

    <!-- Checkout Button Section { -->
     <asp:Panel ID="pnlCheckoutPaymentButtons" runat="server">
        <div id="billing-method-button-place-holder">
            <div id="billing-method-button">
                <input type="button" value="<%=AppLogic.GetString("checkoutpayment.aspx.6", true)%>" id="checkoutpayment-submit-button" class="site-button float-right content" 
                data-contentKey="checkoutpayment.aspx.6" data-contentType="string resource" data-contentValue="<%=AppLogic.GetString("checkoutpayment.aspx.6", true)%>"/>
            </div>
            <div id="save-billing-method-loader"></div>
        </div>
    </asp:Panel>

    <!-- Checkout Button Section } -->
  
    <!-- Counpon Section { -->
    <div class="clear-both height-5"></div>
    <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
        <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="Literal1" runat="server">(!checkoutpayment.aspx.43!)</asp:Literal></div>
            <div id="divCouponEntered"><asp:Literal ID="Literal2" runat="server">(!checkoutpayment.aspx.44!)</asp:Literal> : <asp:Literal runat="server" ID="litCouponEntered"></asp:Literal></div>
            </div>
    </asp:Panel>
    <!-- Counpon Section } -->
    <div class="clear-both height-12"></div>
    <div class="clear-both height-5"></div>

    <div class="sections-place-holder no-padding">
        <!-- Order Summary Section { -->

        <div class="sections-place-holder">
            <div class="section-header section-header-top"><asp:Literal ID="litItemsToBeShipped" runat="server">(!checkoutpayment.aspx.39!)</asp:Literal></div>
              
            <div class="section-content-wrapper">
            <div id="order-summary-head-text" style="padding-left: 23px;padding-right:12px">
                <span class="one-page-link-right normal-font-style  float-right">
                <a href="shoppingcart.aspx" class="custom-font-style"><asp:Literal ID="litEditCart" runat="server">(!checkoutpayment.aspx.40!)</asp:Literal></a></span>
            </div>

            <div class="clear-both height-12"></div>

            <div id="items-to-be-shipped-place-holder-1">

                <asp:Literal ID="OrderSummary" runat="server"></asp:Literal>

            </div>
           
            <div class="clear-both" id="divCheckoutPaymentFooterClr1"></div>
            <div id='items-to-be-shipped-footer'>
            <asp:Literal runat="server" ID="litOrderSummaryFooter"></asp:Literal>
            </div>
            <div class="clear-both" id="divCheckoutPaymentFooterClr2"></div>
            </div>
        </div>

        <!-- Order Summary Section } -->
    </div>

    <!-- do not remove or modify / start here -->
     <asp:HiddenField ID="hidRecentData" runat="server" EnableViewState="true" />

    <div class="display-none">
         <asp:Button ID="btnDoProcessPayment" runat="server" Text="Complete Purchase" CssClass="site-button" />
        <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>
        <asp:TextBox id="txtCode" runat="server"></asp:TextBox>
        <asp:TextBox ID="hidMaskCardNumber" runat="server"></asp:TextBox>
        <asp:TextBox ID="hidCreditCardCode" runat="server"></asp:TextBox>
        <div id="c-ref-no"></div>
        <div id="isTokenization"><asp:Literal ID="litTokenizationFlag" runat="server"></asp:Literal></div>
        <div id="isRegistered"><asp:Literal ID="litIsRegistered" runat="server"></asp:Literal></div>
    </div>
    <div style="display:none;margin:auto" title="Address Verification"  id="ise-address-verification"></div>
   </asp:Panel>
    <script type="text/javascript" src="jscripts/minified/address.control.js"></script>
    <script type="text/javascript" src="jscripts/minified/address.verification.js"></script>
    <script type="text/javascript" src="jscripts/minified/normal.checkout.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.numeric.js"></script>
        <!-- do not remove or modify / ends here -->
    <script type="text/javascript">
   <!-- reference path : /component/address-verificatio/real-time-address-verification-plugin -->
    $(window).load(function () {
        var basePlugin = new jqueryBasePlugin();
        basePlugin.downloadPlugin('components/address-verification/setup.js', function () {

            var loader = new realtimeAddressVerificationPluginLoader();
            loader.start(function (config) {

                var $plugin = $.fn.RealTimeAddressVerification;

                config.submitButtonID = "btnDoProcessPayment";
                config.addressMatchDialogContainerID = "ise-address-verification";
                config.errorContainerId = "payment-form-error-container";
                config.progressContainterId = "save-billing-method-loader";
                config.buttonContainerId = "billing-method-button";
                config.isWithShippingAddress = false;
                config.isAllowShipping = false;
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
                    POSTAL_CODE: "",
                    CITY: "",
                    STATE: "",
                    COUNTRY: "",
                    STREET_ADDRESS: "",
                    RESIDENCE_TYPE: "",
                    CITY_STATE_SELECTOR: ""
                };

                var realTimeAddressVerificationPluginStringKeys = new Object();

                realTimeAddressVerificationPluginStringKeys.unableToVerifyAddress = "checkoutpayment.aspx.45";
                realTimeAddressVerificationPluginStringKeys.confirmCorrectAddress = "checkoutpayment.aspx.46";
                realTimeAddressVerificationPluginStringKeys.useBillingAddressProvided = "checkoutpayment.aspx.47";
                realTimeAddressVerificationPluginStringKeys.useShippingAddressProvided = "checkoutpayment.aspx.48";
                realTimeAddressVerificationPluginStringKeys.selectMatchingBillingAddress = "checkoutpayment.aspx.49";
                realTimeAddressVerificationPluginStringKeys.selectMatchingShippingAddress = "checkoutpayment.aspx.50";
                realTimeAddressVerificationPluginStringKeys.gatewayErrorText = "checkoutpayment.aspx.51";
                realTimeAddressVerificationPluginStringKeys.progressText = "checkoutpayment.aspx.52";

                config.stringResourceKeys = realTimeAddressVerificationPluginStringKeys;

                $plugin.setup(config);
            });
        });
    });
    </script>
    <script type="text/javascript">
         $(document).ready(function () {
             var classIndex = 0;
             $(".aTaxRateValue").click(function () {
                 var $this = $(this);

                 var $divTaxBreakdown = $this.parent("span").parent("div").children(".divTaxBreakdownWrapper");
                 var $hideDivBorder = $this.parent("span").parent("div").parent("div").children(".hide-on-tax-breakdown-display");

                 var title = $this.attr("title");

                 var mode = $this.attr("data-mode");
                 mode = (typeof (mode) == "undefined") ? "show" : $.trim(mode);

                 if (mode == "show") {

                     $hideDivBorder.css("border-bottom", "1px solid #fff");
                     $divTaxBreakdown.show("slide", { direction: "up" }, function () {
                         $this.attr("data-mode", "hide");
                     });

                 } else {

                     $divTaxBreakdown.hide("slide", { direction: "up" }, function () {
                         $this.attr("data-mode", "show");
                         $hideDivBorder.css("border-bottom", "1px solid #ccc");
                     });
                 }

                 $this.attr("title", $this.attr("data-title"));
                 $this.attr("data-title", title);
             });
         });
     </script>
    </form>
</body>
</html>