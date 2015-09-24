<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.wishlist" CodeFile="wishlist.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <form runat="server" onsubmit="return FormValidator(this)">
    <DIV class=row>
    <DIV class="small-12 columns">
        <b><asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal></b>
        <ise:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
        &nbsp;
        <asp:Literal ID="XmlPackage_WishListPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

        <asp:Literal ID="XmlPackage_WishListPageTopControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlTopControlLines" runat="server" Visible="false">
            <div class="row">
            <div class="small-12 columns small-only-text-center text-right">
                <ul class="button-group radius">
                    <li><asp:Button ID="btnContinueShopping1" runat="server" CssClass="button small secondary" /></li>
                    <li><asp:Button ID="btnUpateWishList1" runat="server" CssClass="button small" /></li>
                </ul>
            </div>
            </div>
        </asp:Panel>       

        <div class="sections-place-holder no-padding">
        <h2><asp:Literal ID="Literal2" runat="server">Wish List</asp:Literal></h2>
      
        <div class="section-content-wrapper">

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
            <div class="row">
            <div class="small-12 columns small-only-text-center text-right">
                <ul class="button-group radius">
                    <li><asp:Button ID="btnContinueShopping2" runat="server" CssClass="button small secondary" /></li>
                    <li><asp:Button ID="btnUpateWishList2" runat="server" CssClass="button small" /></li>
                </ul>
            </div>
            </div>
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
    </DIV>
    </DIV>
    </form>
</body>
</html>
