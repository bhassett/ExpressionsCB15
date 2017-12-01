<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult2" CodeFile="checkoutshippingmult2.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register Src="~/UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

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
                    <form id="frmCheckOutMultiShipping2" runat="server">
        
            <asp:Repeater ID="rptCartItems" runat="server">
                 <HeaderTemplate>
                    <table width="100%" cellpadding="0" cellspacing="0" >
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color:#DDDDDD;">
                        <td style="height:15px;">
                            <span>
                                <b>
                                    <%=String.Format(AppLogic.GetString("shoppingcart.cs.29"), ++ShippingGroupCounter)%>
                                </b>
                            </span>
                        </td>
                        <td style="height:15px;">
                            <span>
                                <b>
                                    <%=AppLogic.GetString("shoppingcart.cs.28", true)%>
                                </b>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding: 2px;">
                            <br />
                                <asp:Panel runat="server" ID="pnlItemContainer"></asp:Panel>
                            <br />
                        </td>
                        <td valign="top">
                            <br />
                            <b>
                                <asp:Label runat="server" ID="lblShipmethodHeader"></asp:Label>
                            </b>
                            <asp:Panel runat="server" ID="divShippingInfo">
                                <asp:Label runat="server" ID="lblShippingAddressString"></asp:Label>
                                <br />
                                <span>
                                    <b><%# AppLogic.GetString("shoppingcart.cs.10") %></b>
                                </span>
                                <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                </ItemTemplate>
                 <FooterTemplate>
                    </table>
                 </FooterTemplate>
             </asp:Repeater>
            
            <hr />
            
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
        
        
  <%--  </body>
</html>--%>
