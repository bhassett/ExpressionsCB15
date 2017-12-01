<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult" CodeFile="checkoutshippingmult.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
 <body>--%>
      
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
     
     <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />


       <div id="checkout-multi-shipping-container">
            <div class="row">
                <div class="col-lg-7 col-md-7 col-sm-12 col-xs-12">
                     <p>
            <asp:Label ID="lblHeader1" runat="server" Text="(!checkoutshippingmult.aspx.5!)" Font-Bold="true"></asp:Label>
        </p>

                    <form id="frmCheckOutMultiShipping" runat="server">
        <div class="clear-both heigh-5"></div>
            <p> 
                <asp:Label ID="lblHeader2" runat="server" Text="To add or edit shipping address" ></asp:Label>
 
                <asp:HyperLink ID="lnkEditAddress" runat="server" NavigateUrl="selectaddress.aspx?checkout=true&AddressType=shipping&returnURL=checkoutshippingmult.aspx">click here</asp:HyperLink>
                <div class="clear-both heigh-5"></div>
                
                <asp:Label ID="lblHeader3" runat="server" Text="To ship all items to your primary shipping address " >
                </asp:Label>
                <asp:LinkButton ID="lnkShipAllItemsToPrimaryShippingAddress" runat="server" Text="click here" OnClick="lnkShipAllItemsToPrimaryShippingAddress_Click"  ></asp:LinkButton>    
                
            </p>   
               
            <asp:Repeater ID="rptCartItems" runat="server">
                 <HeaderTemplate>
                    <table width="100%" cellpadding="0" cellspacing="0"  id="divMultiShippingItems">
                </HeaderTemplate>
                 <FooterTemplate>
                    </table>
                 </FooterTemplate>
             </asp:Repeater>
             
             <hr />
             
                        <div class="checkout-button-container">
                              <asp:Panel ID="pnlCompletePurchase" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnCompletePurchase" runat="server" OnClick="btnCompletePurchase_Click" CssClass="btn btn-primary btn-huge" />
            </asp:Panel>
                        </div>
          
        
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
