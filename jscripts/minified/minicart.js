deleteCookie('cartisopen');
$('#shopping-cart').click(function () {

    if ($(this).hasClass("page-is-on-edit-mode")) return false;

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/BuildMiniCart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (getCookie('cartisopen') == null || getCookie('cartisopen') == "") {
                $("#mini-wishlist").slideUp("fast");
                deleteCookie('wishlistisopen');


                $("#mini-cart").html(result.d);
                $("#mini-cart").slideDown("fast");
                minicartScroller();

                if ($("#hdnVatEnabled").val() == 'true') {
                    if ($("#hdnVatInclusive").val() == 'false') {
                        var divHeight = ($("#SubTotal").height());
                        $("#SubTotal").css("height", divHeight + 10 + "px");
                    }
                }

                $('.minicartImage').bind("click", function () {
                    GetAccessoryItemForMinicart(this.id);
                });

                setCookie('cartisopen', 'open');
                applyScroller();
            }
            else {
                $("#mini-cart").slideUp("fast");
                deleteCookie('cartisopen');
            }

        },
        fail: function (result) {

        }
    });
});

function GetAccessoryItemForMinicart(id) {

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetAccessoryItemForMinicart",
        data: '{"counter":' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#minicart-accessory-panel").html(result.d);
        },
        fail: function (result) {

        }
    });
}

function updateCart(cartitemsArray, accessoryItemsArray) {

    AjaxCallWithSecuritySimplified(
        "UpdateCart",
        { "qtyArray": cartitemsArray, "chkArray": accessoryItemsArray },
        function (result) {
            reloadMinicart();
        }
    );
};

function removeItem(itemCounter) {

    AjaxCallWithSecuritySimplified(
        "RemoveMiniCartItem",
        { "cartRecordID": itemCounter },
        function (result) {
            reloadMinicart();
        }
    );

};

function goToPayPalCheckout() {

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/RedirectToPayPalCheckoutMinicart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            window.location = result.d;
        }
    });
}

function reloadMinicart() {

    $('#mini-cart').children().remove();

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/BuildMiniCart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.d.length != 0) {
                $("#mini-cart").html(result.d);

                $('.minicartImage').bind("click", function () {
                    GetAccessoryItemForMinicart(this.id);
                });

                reloadShoppingCartNumber();
                minicartScroller();
            }
            else {
                reloadShoppingCartNumber();
                $("#mini-cart").hide();
            }
        }
    });
};

function reloadShoppingCartNumber() {
   $('#cartNumItem').text('');

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/ShoppingCartNumber",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
			$('#cartNumItem').text(result.d);
        }
    });
};

function newScroller() {

    var listLimit = Number($("#hdnMinicartItems").val());
    var listCounts = $("ul#minicartItems > li").length;
    if (listCounts < 1) {

        return;
    }

    var parent = $("#miniCartLineItem");
    var firstChild = $("#miniCartLineItem ul#minicartItems > li:first-child");
    var firstChildHeight = $(firstChild).height();

    if (listLimit > listCounts)
        listLimit = listCounts;

    var parentHeight = listLimit * firstChildHeight;

    $(parent).css("height", parentHeight + "px");
}

function applyScroller() {
    $("#miniCartLineItem").mCustomScrollbar(
         {
             theme: "dark-thick",
             scrollInertia: 500,
             mouseWheel: { enable: false },

         });
}

function minicartScroller() {
    newScroller();
    return;

    var listLimit = $("#hdnMinicartItems").val();
    var listCounts = $("ul#minicartItems > li").length;

    var rowHeight = 0;

    if ($("#hdnVatEnabled").val() == 'true') {
        if ($("#hdnVatInclusive").val() == 'false') {
            rowHeight = 120;
        }
        else {
            rowHeight = 99;
        }
    }
    else {
        rowHeight = 99;
    }

    var d = 0;
    var ulTopMargin = 0;

    var divHeight = listLimit * rowHeight;

    var x = listCounts - listLimit;

    if (listCounts > listLimit) {
        $("#mbTm").css("display", "block");
        $("#miniCartLineItem").css("height", divHeight + "px");
    }

    $("#mbTm").bind("click", function () {

        d = d + 1;
        ulTopMargin = ulTopMargin - rowHeight;

        $("#minicartItems").animate({
            marginTop: ulTopMargin
        }, 500);

        if (d == x) {

            $("#mbTm").css("display", "none");

        }

        $("#mbTop").css("display", "block");

    });

    $("#mbTop").bind("click", function () {

        d = d - 1;
        ulTopMargin = ulTopMargin + rowHeight

        $("#minicartItems").animate({
            marginTop: ulTopMargin
        }, 500);

        if (d == 0) {

            $("#mbTop").css("display", "none");

        }

        $("#mbTm").css("display", "block");

        return false;
    });
}

function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function deleteCookie(name) {
    setCookie(name, "");
}

$(document).on("click", "#btnCheckOutNow", function (event) {

    var cartItemsArray = [];

    $('.qtyLineItem').each(function (index) {
        var qty = $('#Quantity_' + this.id).val();
        var minOrderQty = $('#MinOrderQuantity_' + this.id).val();
        var quantityRegularExpression = $('#hdnQuantityRegEx').val();
        var validationMessage = $('#hdnInvalidQuantity').val();
        var isAllowFractional = $('#hdnAllowFractional').val();

        if (isAllowFractional == 'false') {
            minOrderQty = Number(minOrderQty);
        }

        if (qty == "") {
            alert($('#hdnEmptyQuantity').val());
            event.preventDefault();
            return;
        }

        if (qty < 0) {
            event.preventDefault();
            alert($('#hdnLessThanQuantity').val());
            if (minOrderQty > 1) {
                $('#Quantity_' + this.id).val(minOrderQty);
            }
            else {
                $('#Quantity_' + this.id).val(1);
            }
            return;
        }

        if (minOrderQty > 0) {
            if (qty < minOrderQty && qty > 0) {
                alert($('#hdnMinimumOrderQuantityError').val() + ' ' + minOrderQty);
                $('#Quantity_' + this.id).val(minOrderQty);
                event.preventDefault();
                return;
            }
        }

        if (!qty.match(quantityRegularExpression)) {
            if (isAllowFractional == 'true') {
                validationMessage += '\n' + $('#hdnInvalidDecimalDigits').val();
            }
            event.preventDefault();
            alert(validationMessage);
            return;
        }

        cartItemsArray.push(this.id + ':' + qty);
        updateCart(cartItemsArray, []);

    });
});

$(document).on("click", "#btnUpdate", function (event) {

    var cartItemsArray = [];
    var accessoryItemsArray = [];
    var isUpdateQuantity = true;

    $('.qtyLineItem').each(function (index) {
        var qty = $('#Quantity_' + this.id).val();
        var minOrderQty = $('#MinOrderQuantity_' + this.id).val();
        var quantityRegularExpression = $('#hdnQuantityRegEx').val();
        var validationMessage = $('#hdnInvalidQuantity').val();
        var isAllowFractional = $('#hdnAllowFractional').val();

        if (isAllowFractional == 'false') {
            minOrderQty = Number(minOrderQty);
        }

        if (qty == "") {
            isUpdateQuantity = false;
            alert($('#hdnEmptyQuantity').val());
            return;
        }

        if (qty < 0) {
            isUpdateQuantity = false;
            alert($('#hdnLessThanQuantity').val());
            if (minOrderQty > 1) {
                $('#Quantity_' + this.id).val(minOrderQty);
            }
            else {
                $('#Quantity_' + this.id).val(1);
            }
            return;
        }

        if (minOrderQty > 0) {
            if (qty < minOrderQty && qty > 0) {
                isUpdateQuantity = false;
                alert($('#hdnMinimumOrderQuantityError').val() + ' ' + minOrderQty);
                $('#Quantity_' + this.id).val(minOrderQty);
                return;
            }
        }

        if (!qty.match(quantityRegularExpression)) {
            isUpdateQuantity = false;
            if (isAllowFractional == 'true') {
                validationMessage += '\n' + $('#hdnInvalidDecimalDigits').val();
            }
            alert(validationMessage);
            return;
        }

        cartItemsArray.push(this.id + ':' + qty);
    });

    if (isUpdateQuantity) {
        $('.restrictedQtyLineItem').each(function (index) {
            var qty = $('#Quantity_' + this.id).val();
            cartItemsArray.push(this.id + ':' + qty);
        });

        $('.chkAccItem').each(function (index) {
            if ($('#chkcom_' + this.id).attr('checked')) {
                accessoryItemsArray.push(this.id);
            }
        });

        updateCart(cartItemsArray, accessoryItemsArray);
    }

});
//function GetAccessoryItemForMinicart(t) { $.ajax({ type: "POST", url: "ActionService.asmx/GetAccessoryItemForMinicart", data: '{"counter":' + t + "}", contentType: "application/json; charset=utf-8", dataType: "json", success: function (t) { $("#minicart-accessory-panel").html(t.d) }, fail: function (t) { } }) } function updateCart(t, i) { AjaxCallWithSecuritySimplified("UpdateCart", { qtyArray: t, chkArray: i }, function (t) { reloadMinicart() }) } function removeItem(t) { AjaxCallWithSecuritySimplified("RemoveMiniCartItem", { cartRecordID: t }, function (t) { reloadMinicart() }) } function goToPayPalCheckout() { $.ajax({ type: "POST", url: "ActionService.asmx/RedirectToPayPalCheckoutMinicart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (t) { window.location = t.d } }) } function reloadMinicart() { $("#mini-cart").children().remove(), $.ajax({ type: "POST", url: "ActionService.asmx/BuildMiniCart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (t) { 0 != t.d.length ? ($("#mini-cart").html(t.d), $(".minicartImage").bind("click", function () { GetAccessoryItemForMinicart(this.id) }), reloadShoppingCartNumber(), minicartScroller()) : (reloadShoppingCartNumber(), $("#mini-cart").hide()) } }) } function reloadShoppingCartNumber() { $("#shopping-cart").children().remove(), $.ajax({ type: "POST", url: "ActionService.asmx/ShoppingCartNumber", contentType: "application/json; charset=utf-8", dataType: "json", success: function (t) { $("#shopping-cart").html(t.d) } }) } function newScroller() { var t = Number($("#hdnMinicartItems").val()), i = $("ul#minicartItems > li").length; if (!(1 > i)) { var e = $("#miniCartLineItem"), n = $("#miniCartLineItem ul#minicartItems > li:first-child"), a = $(n).height(); t > i && (t = i); var r = t * a; $(e).css("height", r + "px") } } function applyScroller() { $("#miniCartLineItem").mCustomScrollbar({ theme: "dark-thick", scrollInertia: 500, mouseWheel: { enable: !1 } }) } function minicartScroller() { return void newScroller() } function setCookie(t, i) { document.cookie = t + "=" + i + "; path=/" } function getCookie(t) { for (var i = t + "=", e = document.cookie.split(";"), n = 0; n < e.length; n++) { for (var a = e[n]; " " == a.charAt(0) ;) a = a.substring(1, a.length); if (0 == a.indexOf(i)) return a.substring(i.length, a.length) } return null } function deleteCookie(t) { setCookie(t, "") } deleteCookie("cartisopen"), $("#shopping-cart").click(function () { return $(this).hasClass("page-is-on-edit-mode") ? !1 : void $.ajax({ type: "POST", url: "ActionService.asmx/BuildMiniCart", contentType: "application/json; charset=utf-8", dataType: "json", success: function (t) { if (null == getCookie("cartisopen") || "" == getCookie("cartisopen")) { if ($("#mini-wishlist").slideUp("fast"), deleteCookie("wishlistisopen"), $("#mini-cart").html(t.d), $("#mini-cart").slideDown("fast"), minicartScroller(), "true" == $("#hdnVatEnabled").val() && "false" == $("#hdnVatInclusive").val()) { var i = $("#SubTotal").height(); $("#SubTotal").css("height", i + 10 + "px") } $(".minicartImage").bind("click", function () { GetAccessoryItemForMinicart(this.id) }), setCookie("cartisopen", "open"), applyScroller() } else $("#mini-cart").slideUp("fast"), deleteCookie("cartisopen") }, fail: function (t) { } }) }), $(document).on("click", "#btnCheckOutNow", function (t) { var i = []; $(".qtyLineItem").each(function (e) { var n = $("#Quantity_" + this.id).val(), a = $("#MinOrderQuantity_" + this.id).val(), r = $("#hdnQuantityRegEx").val(), c = $("#hdnInvalidQuantity").val(), o = $("#hdnAllowFractional").val(); return "false" == o && (a = Number(a)), "" == n ? (alert($("#hdnEmptyQuantity").val()), void t.preventDefault()) : 0 > n ? (t.preventDefault(), alert($("#hdnLessThanQuantity").val()), void (a > 1 ? $("#Quantity_" + this.id).val(a) : $("#Quantity_" + this.id).val(1))) : a > 0 && a > n && n > 0 ? (alert($("#hdnMinimumOrderQuantityError").val() + " " + a), $("#Quantity_" + this.id).val(a), void t.preventDefault()) : n.match(r) ? (i.push(this.id + ":" + n), void updateCart(i, [])) : ("true" == o && (c += "\n" + $("#hdnInvalidDecimalDigits").val()), t.preventDefault(), void alert(c)) }) }), $(document).on("click", "#btnUpdate", function (t) { var i = [], e = [], n = !0; $(".qtyLineItem").each(function (t) { var e = $("#Quantity_" + this.id).val(), a = $("#MinOrderQuantity_" + this.id).val(), r = $("#hdnQuantityRegEx").val(), c = $("#hdnInvalidQuantity").val(), o = $("#hdnAllowFractional").val(); return "false" == o && (a = Number(a)), "" == e ? (n = !1, void alert($("#hdnEmptyQuantity").val())) : 0 > e ? (n = !1, alert($("#hdnLessThanQuantity").val()), void (a > 1 ? $("#Quantity_" + this.id).val(a) : $("#Quantity_" + this.id).val(1))) : a > 0 && a > e && e > 0 ? (n = !1, alert($("#hdnMinimumOrderQuantityError").val() + " " + a), void $("#Quantity_" + this.id).val(a)) : e.match(r) ? void i.push(this.id + ":" + e) : (n = !1, "true" == o && (c += "\n" + $("#hdnInvalidDecimalDigits").val()), void alert(c)) }), n && ($(".restrictedQtyLineItem").each(function (t) { var e = $("#Quantity_" + this.id).val(); i.push(this.id + ":" + e) }), $(".chkAccItem").each(function (t) { $("#chkcom_" + this.id).attr("checked") && e.push(this.id) }), updateCart(i, e)) });