<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutgiftemail.aspx.cs" Inherits="checkoutgiftemail" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Infrastructure" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel" %>

<html>
<head>
    <title></title>
</head>
<body>

<form id="frmGiftItemEmail" runat="server">
    <div class="controls">
        <asp:Button ID="btnCheckoutTop" runat="server" Text="" CssClass="btn btn-info" />
    </div>
    <div class="giftitems-wrapper">
        
        <span class="header"><asp:Label ID="lblHeader" runat="server"></asp:Label></span>
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
                                <br />
                            </span>
                            </td>
                        </tr>
                        <tr>
                            <td><%# ServiceFactory.GetInstance<IStringResourceService>().GetString("checkoutgiftemail.aspx.2") %> </td>
                            <td><asp:TextBox ID="txtEmail" runat="server" CssClass="email" Text='<%# Eval("EmailRecipient") %>'></asp:TextBox></td>
                        </tr>
                    </table>
                </td>
            </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
    </div>
    <div class="controls">
        <asp:Button ID="btnCheckoutBottom" runat="server" Text="" CssClass="btn btn-info" />
    </div>
</form>

</body>
</html>
