deleteCookie("cartisopen");
$("#shopping-cart").click(function () {
    if ($(this).hasClass("page-is-on-edit-mode")) return !1; $.ajax({
        type: "POST", url: "ActionService.asmx/BuildMiniCart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (a) {
            null == getCookie("cartisopen") || "" == getCookie("cartisopen") ? ($("#mini-cart").html(a.d), $("#mini-cart").slideDown("fast"), minicartScroller(), "true" == $("#hdnVatEnabled").val() && "false" == $("#hdnVatInclusive").val() && (a = $("#SubTotal").height(), $("#SubTotal").css("height", a + 10 +
            "px")), $(".minicartImage").bind("click", function () { GetAccessoryItemForMinicart(this.id) }), setCookie("cartisopen", "open")) : ($("#mini-cart").slideUp("fast"), deleteCookie("cartisopen"))
        }, fail: function (a) { }
    })
}); function GetAccessoryItemForMinicart(a) { $.ajax({ type: "POST", url: "ActionService.asmx/GetAccessoryItemForMinicart", data: '{"counter":' + a + "}", contentType: "application/json; charset=utf-8", dataType: "json", success: function (a) { $("#minicart-accessory-panel").html(a.d) }, fail: function (a) { } }) }
function updateCart(a, d) { AjaxCallWithSecuritySimplified("UpdateCart", { qtyArray: a, chkArray: d }, function (a) { reloadMinicart() }) } function removeItem(a) { AjaxCallWithSecuritySimplified("RemoveMiniCartItem", { cartRecordID: a }, function (a) { reloadMinicart() }) } function goToPayPalCheckout() { $.ajax({ type: "POST", url: "ActionService.asmx/RedirectToPayPalCheckoutMinicart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (a) { window.location = a.d } }) }
function reloadMinicart() { $("#mini-cart").children().remove(); $.ajax({ type: "POST", url: "ActionService.asmx/BuildMiniCart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (a) { 0 != a.d.length ? ($("#mini-cart").html(a.d), $(".minicartImage").bind("click", function () { GetAccessoryItemForMinicart(this.id) }), reloadShoppingCartNumber(), minicartScroller()) : (reloadShoppingCartNumber(), $("#mini-cart").hide()) } }) }
function reloadShoppingCartNumber() { $("#shopping-cart").children().remove(); $.ajax({ type: "POST", url: "ActionService.asmx/ShoppingCartNumber", contentType: "application/json; charset=utf-8", dataType: "json", success: function (a) { $("#shopping-cart").html(a.d) } }) }
function minicartScroller() {
    var a = $("#hdnMinicartItems").val(), d = $("ul#minicartItems > li").length, c = 0, c = "true" == $("#hdnVatEnabled").val() ? "false" == $("#hdnVatInclusive").val() ? 120 : 99 : 99, b = 0, e = 0, g = a * c, f = d - a; d > a && ($("#mbTm").css("display", "block"), $("#miniCartLineItem").css("height", g + "px")); $("#mbTm").bind("click", function () { b += 1; e -= c; $("#minicartItems").animate({ marginTop: e }, 500); b == f && $("#mbTm").css("display", "none"); $("#mbTop").css("display", "block") }); $("#mbTop").bind("click", function () {
        b -= 1; e +=
        c; $("#minicartItems").animate({ marginTop: e }, 500); 0 == b && $("#mbTop").css("display", "none"); $("#mbTm").css("display", "block"); return !1
    })
} function setCookie(a, d) { document.cookie = a + "=" + d + "; path=/" } function getCookie(a) { a += "="; for (var d = document.cookie.split(";"), c = 0; c < d.length; c++) { for (var b = d[c]; " " == b.charAt(0) ;) b = b.substring(1, b.length); if (0 == b.indexOf(a)) return b.substring(a.length, b.length) } return null } function deleteCookie(a) { setCookie(a, "") }
$(document).on("click", "#btnCheckOutNow", function (a) {
    var d = []; $(".qtyLineItem").each(function (c) {
        c = $("#Quantity_" + this.id).val(); var b = $("#MinOrderQuantity_" + this.id).val(), e = $("#hdnQuantityRegEx").val(), g = $("#hdnInvalidQuantity").val(), f = $("#hdnAllowFractional").val(); "false" == f && (b = Number(b)); "" == c ? (alert($("#hdnEmptyQuantity").val()), a.preventDefault()) : 0 > c ? (a.preventDefault(), alert($("#hdnLessThanQuantity").val()), 1 < b ? $("#Quantity_" + this.id).val(b) : $("#Quantity_" + this.id).val(1)) : 0 < b && c < b && 0 <
        c ? (alert($("#hdnMinimumOrderQuantityError").val() + " " + b), $("#Quantity_" + this.id).val(b), a.preventDefault()) : c.match(e) ? (d.push(this.id + ":" + c), updateCart(d, [])) : ("true" == f && (g += "\n" + $("#hdnInvalidDecimalDigits").val()), a.preventDefault(), alert(g))
    })
});
$(document).on("click", "#btnUpdate", function (a) {
    var d = [], c = [], b = !0; $(".qtyLineItem").each(function (a) {
        a = $("#Quantity_" + this.id).val(); var c = $("#MinOrderQuantity_" + this.id).val(), f = $("#hdnQuantityRegEx").val(), h = $("#hdnInvalidQuantity").val(), k = $("#hdnAllowFractional").val(); "false" == k && (c = Number(c)); "" == a ? (b = !1, alert($("#hdnEmptyQuantity").val())) : 0 > a ? (b = !1, alert($("#hdnLessThanQuantity").val()), 1 < c ? $("#Quantity_" + this.id).val(c) : $("#Quantity_" + this.id).val(1)) : 0 < c && a < c && 0 < a ? (b = !1, alert($("#hdnMinimumOrderQuantityError").val() +
        " " + c), $("#Quantity_" + this.id).val(c)) : a.match(f) ? d.push(this.id + ":" + a) : (b = !1, "true" == k && (h += "\n" + $("#hdnInvalidDecimalDigits").val()), alert(h))
    }); b && ($(".restrictedQtyLineItem").each(function (a) { a = $("#Quantity_" + this.id).val(); d.push(this.id + ":" + a) }), $(".chkAccItem").each(function (a) { $("#chkcom_" + this.id).attr("checked") && c.push(this.id) }), updateCart(d, c))
});