<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.shipping" CodeFile="shipping.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>

<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register Src="~/UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Register Src="UserControls/ScriptControl.ascx" TagName="ScriptControl" TagPrefix="uc" %>
<%@ Register Src="UserControls/ShipToAddressControl.ascx" TagPrefix="uc" TagName="ShipToAddressControl" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

 
    
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
         
        <uc:ScriptControl ID="thirdPartyScriptManager" runat="server" ShowGoogleMapApi="true" />
        <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
        

        <div id="checkout-shipping-container">
            <div class="row">
                <div class="col-lg-7 col-md-7 col-sm-12 col-xs-12">
                    <form id="frmCheckOutMultiShipping2" runat="server">

                        <div class="panel panel-default panel-shipping">
                            <div class="panel-heading">
                                <h3 class="panel-title">
                                    <asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal>
                                </h3>
                            </div>
                        <div class="panel-body">
                             <div>
                                <uc:ShipToAddressControl runat="server" ID="ctrlShipToAddressControl" />
                            </div>
                            <asp:Repeater ID="rptCartItems" runat="server">
                                <ItemTemplate>
                                    <div class="panel panel-default">
                                          <div class="panel-heading">
                                                 <h3 class="panel-title"><asp:Label runat="server" ID="lblOptionName"></asp:Label></h3>
                                             </div>
                                        <div class="panel-body">
                                            
                                            
                                          <div class="row">
                                              <div class="col-lg-6">
                                                   <asp:Panel runat="server" ID="pnlItemContainer"></asp:Panel>
                                              </div>
                                              <div class="col-lg-6">
                                                    <asp:Panel runat="server" ID="divShippingInfo">
                                                <span>
                                                    <b><%# AppLogic.GetString("shoppingcart.cs.10") %></b>
                                                </span>
                                                <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                                            </asp:Panel>
                                              </div>
                                          </div>
                                      </div>
                                    </div>
                                
                                </ItemTemplate>
              
             </asp:Repeater>
                        </div>
                    </div>
        
   
         
            
            <div class="checkout-button-container">
                  <asp:Panel ID="pnlCompletePurchase" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnCompletePurchase" CssClass="btn btn-primary btn-huge" runat="server" Text="(!checkoutshippingmult.aspx.6!)" OnClick="btnCompletePurchase_Click" />
            </asp:Panel>
            </div>
           
        
        </form>

                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 col-xs-12">
                     <asp:Literal ID="OrderSummaryCardLiteral" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
 