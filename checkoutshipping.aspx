<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutshipping.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkoutshipping" EnableEventValidation="false" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Register Src="UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Register Src="UserControls/ShipToAddressControl.ascx" TagPrefix="uc" TagName="ShipToAddressControl" %>


<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%--<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
 
</head>
 <body>
     --%>
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
     
     <%--Steps--%>
     <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>

     <%--Error Summary--%>
     <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
        
     <%--Checkout Shipping--%> 
     <div id="checkout-shipping-container">
        <div class="row">
            <div class="col-lg-7 col-md-7 col-sm-12 col-xs-12">
                <form id="frmCheckOutShipping" runat="server">
                    
                    <asp:Panel ID="pnlGetFreeShippingMsg" CssClass="FreeShippingThresholdPrompt" Visible="false" runat="server">
                        <asp:Literal ID="GetFreeShippingMsg" runat="server" Mode="PassThrough"></asp:Literal>
                        <div class="clear-both height-12"></div>
                    </asp:Panel>
            
                    <div class="panel panel-default panel-checkoutshippping">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal>
                            </h3>
                        </div>
                        <div class="panel-body">
                            <div>
                                <uc:ShipToAddressControl runat="server" ID="ctrlShipToAddressControl" />
                            </div>
                            <div class="text-left">
                                <asp:Label ID="lblSelectShippingMethod" Text="" runat="server" Font-Bold="true" class="content"/>
                            </div>
                            <div>
                                <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="checkout-button-container">
                         <asp:Button ID="btnCompletePurchase" runat="server" Text="Complete Purchase" CssClass="btn btn-primary btn-huge" />
                    </div>
                    
                    
               
           
        <!-- Counpon Section { -->
            <div class="clear-both height-5"></div>
            <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
                <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top"><asp:Literal ID="Literal1" runat="server">(!checkoutshipping.aspx.14!)</asp:Literal></div>
                    <div id="divCouponEntered" class="section-content-wrapper"><asp:Literal ID="Literal2" runat="server">(!order.cs.12!)</asp:Literal><asp:Literal runat="server" ID="litCouponEntered"></asp:Literal></div>
                    <div class="clear-both height-5"></div>
                </div>
            </asp:Panel>
        <!-- Counpon Section } -->

        <div class="clear-both height-5"></div>
           
        <!-- Order Summary Section { -->

   <%--     <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="litItemsToBeShipped" runat="server">(!checkout1.aspx.43!)</asp:Literal></div>
           
            <div class="section-content-wrapper">
                <div id="order-summary-head-text" style="padding-left: 23px;padding-right:12px">
                    <div class="clear-both height-12"></div>
                    <span class="strong-font  custom-font-style">
                    <asp:Literal ID="litOrderSummary" runat="server"></asp:Literal>
                    </span> 
                    <span class="one-page-link-right normal-font-style  float-right">
                    <a href="shoppingcart.aspx" class="custom-font-style"><asp:Literal ID="litEditCart" runat="server">(!checkout1.aspx.44!)</asp:Literal></a></span>
                </div>

                <div class="clear-both height-12"></div>

                <div id="items-to-be-shipped-place-holder-1">
                    <asp:Literal ID="OrderSummary" runat="server"></asp:Literal>
                </div>
           
                <div class="clear-both height-12"></div>
                <div id='items-to-be-shipped-footer'>
                    <asp:Literal runat="server" ID="litOrderSummaryFooter"></asp:Literal>
                </div>
            </div>
            <div class="clear-both height-12"></div>
        </div>--%>

        <!-- Order Summary Section } -->

        </form>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-12 col-xs-12">
                  <asp:Literal ID="OrderSummaryCardLiteral" runat="server"></asp:Literal>
            </div>
        </div>
     </div>
   
     

    
   
   
     
    
        
<%--        
    </body>
</html>--%>
