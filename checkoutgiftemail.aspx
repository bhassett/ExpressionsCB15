<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutgiftemail.aspx.cs" Inherits="checkoutgiftemail" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Infrastructure" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel" %>

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
    
    <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>
    
    <%--Checkout Shipping--%> 
    <div id="checkoutgiftemail-container">
        <div class="row">
            <div class="col-lg-7 col-md-7 col-sm-12 col-xs-12">
                <form id="frmGiftItemEmail" runat="server">
                    <div class="giftitems-wrapper">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <asp:Label ID="lblHeader" runat="server"></asp:Label>
                            </div>
                            <div class="panel-body">
                                <table class="giftitems">
                                    <asp:Repeater ID="rptGiftItemsEmail" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="icon">
                                                    <img src="<%# AppLogic.LookupImage("product",
                                                    ServiceFactory.GetInstance<IProductService>().GetItemDefaultImageFilenameBySize(Container.DataItemAs<ShoppingCartGiftEmailCustomModel>().ItemCode, ImageSize.Icon), 
                                                    "icon", 
                                                    ThisCustomer.SkinID, 
                                                    ThisCustomer.LocaleSetting) %>" />
                                                </td>
                                                <td class="detail">
                                                    <asp:HiddenField ID="txtGiftID" runat="server" Value='<%# Eval("Counter") %>' />
                                                    <table>
                                                        <tr>
                                                            <td colspan="2">
                                                                <span class="title">
                                                                    <a href='<%# SE.MakeProductLink(Container.DataItemAs<ShoppingCartGiftEmailCustomModel>().ItemCounter.ToString(), Container.DataItemAs<ShoppingCartGiftEmailCustomModel>().ItemDescription)  %>'><%# Eval("ItemDescription")  %></a>
                                                                   
                                                                </span>
                                                                 <br /><br />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div class="form">
                                                                    <label>
                                                                        <%# ServiceFactory.GetInstance<IStringResourceService>().GetString("checkoutgiftemail.aspx.2") %> 
                                                                    </label>
                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="email form-control" Text='<%# Eval("EmailRecipient") %>'></asp:TextBox>
                                                                </div>
                                                            </td>
                                                       <%--     <td></td>
                                                            <td></td>--%>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="checkout-button-container">
                        <asp:Button ID="btnCheckoutBottom" runat="server" Text="" CssClass="btn-primary btn-huge" />
                    </div>
                </form>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-12 col-xs-12">
                <asp:Literal ID="OrderSummaryCardLiteral" runat="server"></asp:Literal>
            </div>
        </div>
     </div>
   



<%--</body>
</html>--%>
