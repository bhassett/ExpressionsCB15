(function (a) {
    a.globalLoader = function (e, m) {
        var n = { delay: 1E3, background: "#FFFFFF", image: "ajax-loader.gif", zIndex: 9999999, autoHide: !0, opacity: 0.3, text: "loading..." }, b = this; b.settings = {}; a(e); var k = "", c = ""; b.init = function () { b.settings = a.extend({}, n, m); k = "divGlobalMask"; c = "divGlobalLoader"; var d = a("<div />", { id: k }), h = a("<div />", { id: c }); a("body").append(d); a("body").append(h); a(g(c)).append("<img src='" + b.settings.image + "' />"); a(g(c)).append("<span>" + b.settings.text + "</span>"); a(g(c)).hide() }; b.show =
        function () { var d = a(document).height(), h = a(window).width(); a(g(k)).css({ width: h, height: d, background: b.settings.background, position: "absolute", left: 0, top: 0, display: "inline-block", "z-index": b.settings.zIndex }).fadeTo("fast", b.settings.opacity); var f = a(window).height(), d = a(window).width(), h = a(g(c)), f = f / 2 - h.height() / 2, d = d / 2 - h.width() / 2; a(g(c)).css({ top: f, left: d, position: "fixed", "z-index": b.settings.zIndex - 1 }).show(); l() }; b.hide = function (d) {
            a(g(c)).fadeOut(); a(g(k)).fadeOut("", function () {
                "function" === typeof d &&
                d()
            })
        }; var l = function () { b.settings.autoHide && window.setTimeout(function () { b.hide() }, b.settings.delay) }, g = function (a) { if ("" == a) return a; -1 == a.indexOf(".") && (a = "#" + a); return a }; b.init()
    }; a.fn.globalLoader = function (e) { return this.each(function () { if (void 0 == a(this).data("globalLoader")) { var m = new a.globalLoader(this, e); a(this).data("globalLoader", m) } }) }
})(jQuery);
(function (a) {
    a.contentLoader = function (e, m) {
        var n = { delay: 1E3, background: "#FFFFFF", image: "ajax-loader.gif", zIndex: 99, autoHide: !0, opacity: 0.3, text: "loading..." }, b = this; b.settings = {}; a(e); var k = "", c = ""; b.init = function () { b.settings = a.extend({}, n, m); k = "divGlobalMask"; c = "divContentLoader"; var d = a("<div />", { id: k }), h = a("<div />", { id: c }); a("body").append(d); a("body").append(h); a(l(c)).append("<img src='" + b.settings.image + "' />"); a(l(c)).append("<span>" + b.settings.text + "</span>") }; b.show = function () {
            var d = a(e).height(),
            h = a(e).width(), f = a(e).position(); a(l(k)).css({ width: h, height: d, background: b.settings.background, position: "absolute", left: f.left, top: f.top, display: "inline-block", "z-index": b.settings.zIndex }).fadeTo("fast", b.settings.opacity); g(a(l(c)), a(e)); a(l(c)).show(); b.settings.autoHide && window.setTimeout(function () { b.hide() }, b.settings.delay)
        }; b.hide = function () { a(l(c)).fadeOut(); a(l(k)).fadeOut() }; var l = function (a) { if ("" == a) return a; -1 == a.indexOf(".") && (a = "#" + a); return a }, g = function (b, c) {
            var f = a(c).position(),
            e = a(c).height() / 2, g = a(c).width() / 2; f.left += g; f.top += e - a(b).height() / 2; a(b).css({ position: "absolute", left: f.left, top: f.top, "z-index": 100 })
        }; b.init()
    }; a.fn.contentLoader = function (e) { return this.each(function () { if (void 0 == a(this).data("contentLoader")) { var m = new a.contentLoader(this, e); a(this).data("contentLoader", m) } }) }
})(jQuery);