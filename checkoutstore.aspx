<%@ Page Language="C#" CodeFile="checkoutstore.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkoutstore" EnableEventValidation="false" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Infrastructure" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<%@ Register Src="UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Register Src="UserControls/ScriptControl.ascx" TagName="ScriptControl" TagPrefix="uc" %>

<body>

    <uc:ScriptControl ID="thirdPartyScriptManager" runat="server" ShowGoogleMapApi="true" />
   
 
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

     <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>
     <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />

    <div class="height-12"></div>

     <div id="checkout-shipping-container">
        <div class="row">
            <div class="col-lg-7 col-md-7 col-sm-12 col-xs-12">
                <form id="frmcheckoutstore" runat="server">
        <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
            <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top">
                    <asp:Literal ID="Literal1" runat="server">(!checkoutshipping.aspx.14!)</asp:Literal>
                </div>
                <div id="divCouponEntered" class="section-content-wrapper">
                    <asp:Literal ID="Literal2" runat="server">(!order.cs.12!)</asp:Literal>
                    <asp:Literal runat="server" ID="litCouponEntered"></asp:Literal>
                </div>
                <div class="clear-both height-5">
                </div>
            </div>
        </asp:Panel>
        <div class="clear-both height-12"></div>
        <div class="float-right">
            <%--<asp:Button ID="btnContinueCheckOutTop" runat="server" Text="(!checkoutpayment.aspx.6!)" CausesValidation="true" CssClass="site-button content btn btn-info" />--%>
        </div>
        <div class="clear-both height-12"></div>
        
         <div class="panel panel-checkoutstore">
             <div class="panel-heading">
                <h3 class="panel-title">
                      <asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal>
                </h3>
             </div>
             <div class="panel-body">
                 
                 <div class="shipping-cart-item-container">
                     <asp:Repeater runat="server" ID="rptCartItems">
                         <ItemTemplate>
                             <div class="panel panel-default">
                                 <div class="panel-body">
                                     <div class="row shipping-cart-item">
                                 <div class="col-lg-6">
                                     <asp:Label runat="server" ID="lblItemDescription"></asp:Label>
                                     <div style="margin-top: 5px;">
                                         <span><%# AppLogic.GetString("shoppingcart.cs.25")%> : </span>
                                         <asp:Label runat="server" ID="lblQuantity"></asp:Label>
                                     </div>
                                 </div>
                                 <div class="col-lg-6">
                                     <asp:Literal runat="server" ID="litNoShippingMethodText" Visible="false" Text='<%# AppLogic.GetString("checkoutshippingmult.aspx.7") %>'></asp:Literal>
                                    <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                                 </div>
                             </div>
                                 </div>
                             </div>
                             
                         
                        </ItemTemplate>
                     </asp:Repeater>
                 </div>
                
             </div>
         </div>

        <div class="checkout-button-container">
            <asp:Button ID="btnContinueCheckOut" runat="server" Text="(!checkoutpayment.aspx.6!)" CausesValidation="true" CssClass="btn btn-primary btn-huge" />
        </div>
   

      

    </form>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-12 col-xs-12">
                <asp:Literal ID="OrderSummaryCardLiteral" runat="server"></asp:Literal>
            </div>
        </div>
     </div>

    

</body>
