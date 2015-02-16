﻿function $add_windowLoad(a) {
    if ($.browser.msie && 8 == parseInt($.browser.version.slice(0, 3))) $(window).load(a); else if ($.browser.msie && 10 == parseInt($.browser.version.slice(0, 3))) $(document).ready(a); else if (window.addEventListener) window.addEventListener("load", a, !1); else if (document.addEventListener) document.addEventListener("load", a, !1); else if (window.attachEvent) window.attachEvent("onload", a); else if ("function" == typeof window.onload) { var b = window.onload; window.onload = function () { b(); a() } } else window.onload =
    init
} function $getElement(a) { return "string" == typeof a ? $get(a) : a } function $enableSubmit(a) { __doEnableSubmit(a, !0) } function $disableSubmit(a) { __doEnableSubmit(a, !1) } function __doEnableSubmit(a, b) { var c = $getElement(a); if (c) for (var d = 0; d < c.length; d++) { var f = c[d]; if ("submit" == f.type.toLowerCase() || "reset" == f.type.toLowerCase()) f.disabled = !b } } var parseBool = function (a) { return "string" === typeof a && "true" == a.toLowerCase() ? !0 : 0 < parseInt(a) }, trimString = function (a) { return "undefined" == typeof a ? "" : $.trim(a) }, split;
split = split || function (a) {
    var b = String.prototype.split, c = /()??/.exec("")[1] === a, d; d = function (d, e, g) {
        if ("[object RegExp]" !== Object.prototype.toString.call(e)) return void 0 != d ? b.call(d, e, g) : []; var h = [], l = (e.ignoreCase ? "i" : "") + (e.multiline ? "m" : "") + (e.extended ? "x" : "") + (e.sticky ? "y" : ""), m = 0; e = RegExp(e.source, l + "g"); var n, k, p; d += ""; c || (n = RegExp("^" + e.source + "$(?!\\s)", l)); for (g = g === a ? 4294967295 : g >>> 0; k = e.exec(d) ;) {
            l = k.index + k[0].length; if (l > m && (h.push(d.slice(m, k.index)), !c && 1 < k.length && k[0].replace(n, function () {
            for (var b =
            1; b < arguments.length - 2; b++) arguments[b] === a && (k[b] = a)
            }), 1 < k.length && k.index < d.length && Array.prototype.push.apply(h, k.slice(1)), p = k[0].length, m = l, h.length >= g)) break; e.lastIndex === k.index && e.lastIndex++
        } m === d.length ? !p && e.test("") || h.push("") : h.push(d.slice(m)); return h.length > g ? h.slice(0, g) : h
    }; String.prototype.split = function (a, b) { return d(this, a, b) }; return d
}();
function jqueryHideShow(a, b, c, d) { "none" == $(a).css("display") ? $(a).show("fast", function () { "undefined" != d && "" != d && $(b).find("span").text(d) }) : $(a).hide("fast", function () { "undefined" != c && "" != c && $(b).find("span").text(c) }) } function AjaxCallWithSecuritySimplified(a, b, c, d) { AjaxCallWithSecurity("", "", a, "", "", b, c, d) } function AjaxCallWithSecuritySynchronous(a, b, c, d) { AjaxCallWithSecurity("", "", a, "", "", b, c, d, !1) }
function AjaxCallCommon(a, b, c, d) { b = null != b ? JSON.stringify(b) : null; $.ajax({ type: "POST", url: a, data: b, dataType: "json", contentType: "application/json; charset=utf-8", success: c, error: d }) } function AjaxCallCommonRest(a, b, c, d, f) { c = null != c ? JSON.stringify(c) : null; $.ajax({ type: b, url: a, data: c, dataType: "json", contentType: "application/json; charset=utf-8", success: d, error: f }) }
function AjaxCallWithSecurity(a, b, c, d, f, e, g, h, l) {
    if ("" == b || void 0 == b) b = "POST"; if ("" == d || void 0 == d) d = "json"; if ("" == a || void 0 == a) a = "ActionService.asmx/" + c; if ("" == f || void 0 == f) f = "application/json; charset=utf-8"; var m = ise.Configuration.getConfigValue("Service.Token"); if ("" == m || void 0 == m) alert("Error in security token: Empty"); else {
        if (null == h || void 0 == h) h = ErrorHandler; null == l && (l = !0); a = {
            type: b, url: a, dataType: d, async: l, contentType: f, data: JSON.stringify(e), beforeSend: function (a) {
                a.setRequestHeader("TOKEN",
                m)
            }, success: g, error: h
        }; $.ajax(a)
    }
} function AjaxSecureEmailAddressVerification(a, b, c, d, f) { var e = {}; e.emailAddress = b; e.initializeRequest = a; e.accountType = c; AjaxCallWithSecurity("", "", "ValidateEmailAddress", "", "", e, d, f) } function ErrorHandler(a, b, c) { $.parseJSON(a.responseText) } function IsInArray(a, b) { return 0 <= $.inArray($.trim(b), a) } $.cachedScript = function (a, b) { b = $.extend(b || {}, { dataType: "script", cache: !0, url: a }); return jQuery.ajax(b) };
Array.prototype.insert = function (a) { a = Math.min(a, this.length); 1 < arguments.length && this.splice.apply(this, [a, 0].concat([].pop.call(arguments))) && this.insert.apply(this, arguments); return this };
var jqueryBasePlugin = function () {
    this.selectorChecker = function (a) { if ("" == a) return a; -1 == a.indexOf(".") && (a = "#" + a); return a }; this.getElemBySelChecker = function (a) { return $(this.selectorChecker(a)) }; this.getTimeStamp = function () { return "?timestamp=" + (new Date).getTime() }; this.downloadCss = function (a, b) { 0 == $("head").find("link[href*='" + a + "']").length && $("head").append("<link href='" + a + this.getTimeStamp() + "' rel='stylesheet' type='text/css' />"); "undefined" != typeof b && b() }; this.removeCssReference = function (a,
    b) { "undefined" != typeof a && ($("head").find("link[href*='" + a + "']").remove(), "undefined" != typeof b && b()) }; this.downloadPlugin = function (a, b, c) { "undefined" == typeof a && "undefined" != typeof b && c("not found"); $.getScript(a).done(function (a, c) { "function" === typeof b && b() }).fail(function (a, b, e) { "function" !== typeof c ? alert("unable to load the plugin please check the source. \n\n Error message: " + e) : c(e) }); return this }; this.downloadCachePlugin = function (a, b) {
        $.cachedScript(a).done(function (a, d) {
            "undefined" != typeof b &&
            b()
        }).fail(function (a, b, f) { alert("unable to load the plugin please check the source. \n\n Error message: " + f.message) })
    }; this.parseTemplate = function (a, b) { return $.tmpl(a, b) }; this.parseTemplateReturnHtml = function (a, b) { return $(this.parseTemplate(a, b)).html() }; this.loadStringResource = function (a, b) { var c = []; c.push(a); ise.StringResource.loadResources(c, b) }; this.parseJqueryTemplate = function (a, b) { return $.trim($(this.selectorChecker(a)).tmpl(b).html()) }; this.downloadStringResources = function (a, b) {
        ise.StringResource.loadResources(a,
        b)
    }; this.downloadAppConfigs = function (a, b) { ise.Configuration.loadResources(a, b) }; this.getString = function (a) { return ise.StringResource.getString(a) }; this.getAppConfig = function (a) { return ise.Configuration.getConfigValue(a) }; this.getAppConfigBool = function (a) { return this.toBoolean(ise.Configuration.getConfigValue(a)) }; this.toBoolean = function (a) { return "true" == a.toLowerCase() }; this.getDigitalDateFormat = function (a) { a = new Date(a); return isNaN ? "/Date(" + (new Date).getTime() + ")/" : "/Date(" + a.getTime() + ")/" }; this.ajaxRequest =
    function (a, b, c, d) { AjaxCallCommon(a, b, c, d) }; this.ajaxRequestRest = function (a, b, c, d, f) { AjaxCallCommonRest(a, b, c, d, f) }; this.ajaxSecureRequest = function (a, b, c, d) { AjaxCallWithSecuritySimplified(a, b, c, d) }; this.isEmpty = function (a) { return "undefined" == typeof a || null == a || "" == a || 0 < a.length && !(0 > $.inArray("", a)) ? !0 : !1 }; this.isInputControlUndefined = function (a) { a = typeof $(this.selectorChecker(a)).val(); return "undefined" == a || void 0 == a ? !0 : !1 }; this.clearInputControlValue = function (a) { $(this.selectorChecker(a)).val("") };
    this.setInputControlValue = function (a, b) { $(this.selectorChecker(a)).val($.trim(b)) }; this.getInputControlValue = function (a) { return this.isEmpty(a) ? "" : $.trim($(this.selectorChecker(a)).val()) }; this.setElementHTMLContent = function (a, b) { $(this.selectorChecker(a)).html($.trim(b)) }; this.hideElement = function (a) { $(this.selectorChecker(a)).addClass("display-none") }; this.removeClass = function (a, b) { $(this.selectorChecker(a)).removeClass(b) }; this.addClass = function (a, b) { $(this.selectorChecker(a)).addClass(b) }; this.convertObjectToArray =
    function (a) { return $.map(a, function (a, c) { return [a] }) }; this.showLoader = function () { $("body").data("globalLoader").show() }; this.hideLoader = function (a) { $("body").data("globalLoader").hide(a) }; this.ArrayInsertAt = function (a, b, c) { a.insert(b, c); return a }; this.ToJsonObject = function (a) { try { return $.parseJSON(a) } catch (b) { return null } }; this.CreateJqueryDelegate = function (a, b) { return $.proxy(a, b) }; this.calculateDistance = function (a, b, c, d) {
        var f = 3.1459 / 180, e = parseFloat(a), g = parseFloat(c); b = parseFloat(b); d = parseFloat(d);
        e = (g - e) * f; g = (d - b) * f; a = Math.sin(e / 2) * Math.sin(e / 2) + Math.cos(a * f) * Math.cos(c * f) * Math.sin(g / 2) * Math.sin(g / 2); return 7917.511728464 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
    }; this.getQueryStringParamByName = function (a) { a = a.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]"); a = RegExp("[\\?&]" + a + "=([^&#]*)").exec(location.search); return null == a ? "" : decodeURIComponent(a[1].replace(/\+/g, " ")) }; this.updateQueryStringParams = function (a) {
        for (var b = {}, c = location.search.substring(1), d = /([^&=]+)=([^&]*)/g, f; f = d.exec(c) ;) b[decodeURIComponent(f[1])] =
        decodeURIComponent(f[2]); $.each(a, function (a, c) { b[a] = c }); location.search = $.param(b)
    }; this.generateRandomNumber = function (a, b) { if (a && 0 == a) return a; if (b && 0 == b) return b; for (var c = "", d = 0; d < a; d++) c += Math.floor(Math.random() * b); return c }; this.formatString = function (a) { var b = Array.prototype.slice.call(arguments, 1); return a.replace(/{(\d+)}/g, function (a, d) { return "undefined" != typeof b[d] ? b[d] : a }) }
};
$.browser.msie && 8 > parseInt($.browser.version, 10) && $.getScript("jscripts/jquery/json2.js").done(function (a, b) { }).fail(function (a, b, c) { alert(c) });