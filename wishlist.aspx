<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.wishlist" CodeFile="wishlist.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <form runat="server" onsubmit="return FormValidator(this)">
        <b><asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal></b>
        <ise:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
        &nbsp;
        <asp:Literal ID="XmlPackage_WishListPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

        <asp:Literal ID="XmlPackage_WishListPageTopControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlTopControlLines" runat="server" Visible="false">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table1">
                <tr>
                    <td valign="bottom" align="right">
                        <asp:Button ID="btnContinueShopping1" runat="server" CssClass="site-button content btn btn-success" />
                        <asp:Button ID="btnUpateWishList1" runat="server" CssClass="site-button content btn btn-success" />
                    </td>
                </tr>
            </table>
        </asp:Panel>       
         <div class="clr height-12"></div>
         <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="Literal2" runat="server">Wish List</asp:Literal></div>
      
        <div class="section-content-wrapper">
        <div class="clr height-12"></div>

        <asp:Panel ID="Panel1" runat="server" DefaultButton="btnUpateWishList1" class="wishlist-wrapper">
            <asp:Table ID="tblWishList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Table ID="tblWishListBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server" CssClass="wishlist">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Literal ID="CartItems" runat="server" Mode="PassThrough"></asp:Literal>                                
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        </div>
             </div>

        <asp:Literal ID="Xml_WishListPageBottomControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlBottomControlLines" runat="server">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table2">
                <tr>
                    <td valign="bottom" align="right">
                        <br />
                        <asp:Button ID="btnContinueShopping2" runat="server" CssClass="site-button content btn" />
                        <asp:Button ID="btnUpateWishList2" runat="server" CssClass="site-button content btn" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        
        <ise:Topic runat="server" ID="TopicWishListPageFooter" TopicName="WishListPageFooter" />
        <asp:Literal ID="Xml_WishListPageFooter" runat="server" Mode="PassThrough"></asp:Literal>

         <script type="text/javascript">
             $(document).ready(function () {
                 $(".move-to-shopping-cart").unbind("click").click(function () {

                     $this = $(this);

                     var elementId = $this.attr("data-elementId");
                     var cartId = $this.attr("data-cartId");
                     var quantityRegEx = $this.attr("data-quantityRegEx");

                     var quantityInvalidMessage = $this.attr("data-messageQuantityInvalid");
                     var quantiryEmptyMessage = $this.attr("data-messageQuantityEmpty");

                     var quantity = $.trim($("#" + elementId).val());

                     if (quantity == "" || quantity == 0) {
                         alert(quantiryEmptyMessage);
                         return false;
                     }

                     if (quantity.match(quantityRegEx)) {
                         self.location = "wishlist.aspx?movetocartid=" + cartId + "&movetocartqty=" + quantity;
                     } else {
                         alert(quantityInvalidMessage);
                         return false;
                     }

                 });
             });
         </script>
    </form>
</body>
</html>
