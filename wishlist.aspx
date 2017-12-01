<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.wishlist" CodeFile="wishlist.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
<form runat="server" onSubmit="return FormValidator(this)">
  <b>
  <asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal>
  </b>
  <ise:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
  &nbsp;
  <asp:Literal ID="XmlPackage_WishListPageHeader" runat="server" Mode="PassThrough"></asp:Literal>
  <asp:Literal ID="XmlPackage_WishListPageTopControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
  <div class="row">
    <div class="entity-header col-md-6">
      <h1>
        <asp:Literal ID="Literal2" runat="server">Wish List</asp:Literal>
      </h1>
    </div>
  <asp:Panel ID="pnlTopControlLines" runat="server" Visible="false" CssClass="col-md-6 text-right">
      <div class="btn-sep">
        <asp:Button ID="btnContinueShopping1" runat="server" CssClass="btn content btn btn-default" />
      </div>
      <div class="btn-sep">
        <asp:Button ID="btnUpateWishList1" runat="server" CssClass="btn content btn btn-primary" />
    </div>
  </asp:Panel>
  </div>
  <div class="section-content-wrapper row" style="margin-top: 20px;">
  <asp:Panel ID="Panel1" runat="server" DefaultButton="btnUpateWishList1" class="wishlist-wrapper">
    <asp:Panel ID="tblWishList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
      <asp:Panel ID="tblWishListBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server" CssClass="wishlist">
        <asp:Literal ID="CartItems" runat="server" Mode="PassThrough"></asp:Literal>
      </asp:Panel>
    </asp:Panel>
  </asp:Panel>
</div>
  <asp:Literal ID="Xml_WishListPageBottomControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
  <asp:Panel ID="pnlBottomControlLines" runat="server" CssClass="row">
    <div class="col-sm-12 text-right">
      <div class="btn-sep">
        <asp:Button ID="btnContinueShopping2" runat="server" CssClass="btn btn-default content btn" />
      </div>
      <div class="btn-sep">
        <asp:Button ID="btnUpateWishList2" runat="server" CssClass="btn btn-primary content btn" />
      </div>
    </div>
  </asp:Panel>
  <ise:Topic runat="server" ID="TopicWishListPageFooter" TopicName="WishListPageFooter" />
  <asp:Literal ID="Xml_WishListPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
  <script type="text/javascript">
             $(document).ready(function () {
				 $(".wishlist table").addClass("table table-hover table-bordered");
				 $(".wishlist table tr:first-child").css("display", "none");
				 $(".wishlist table select").addClass("form-control");
				 $(".wishlist table input[type='text']").addClass("form-control");
				 $(".wishlist table tr td:first-child a").addClass("thumbnail");
				 $(".wishlist table tr td:first-child a").css("margin-bottom", "0px");
				 $(".wishlist table tr td input[type='button']").removeClass("move-to-shopping-cart site-button");
				 $(".wishlist table tr td input[type='button']").addClass("btn btn-success btn-block move-to-cart");
				 $(".wishlist table tr td input[name='bt_Delete']").removeClass("site-button");
				 $(".wishlist table tr td input[name='bt_Delete']").addClass("btn btn-danger btn-block");
                 $(".move-to-cart").unbind("click").click(function () {

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
