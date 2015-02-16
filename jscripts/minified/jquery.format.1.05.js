var email = { tldn: RegExp("^[^@]+@[^@]+.(A[C-GL-OQ-UWXZ]|B[ABD-JM-OR-TVWYZ]|C[ACDF-IK-ORUVX-Z]|D[EJKMOZ]|E[CEGR-U]|F[I-KMOR]|G[ABD-IL-NP-UWY]|H[KMNRTU]|I[DEL-OQ-T]|J[EMOP]|K[EG-IMNPRWYZ]|L[A-CIKR-VY]|M[AC-EGHK-Z]|N[ACE-GILOPRUZ]|OM|P[AE-HKL-NR-TWY]|QA|R[EOSUW]|S[A-EG-ORT-VYZ]|T[CDF-HJ-PRTVWZ]|U[AGKMSYZ]|V[ACEGINU]|W[FS]|XN|Y[ETU]|Z[AMW]|AERO|ARPA|ASIA|BIZ|CAT|COM|COOP|EDU|GOV|INFO|INT|JOBS|MIL|MOBI|MUSEUM|NAME|NET|ORG|PRO|TEL|TRAVEL)$", "i") };
(function (d) {
    d.extend(d.expr[":"], { regex: function (b, c, d) { c = RegExp(d[3], "g"); b = "text" === b.type ? b.value : b.innerHTML; return "" == b ? !0 : c.exec(b) } }); d.fn.output = function (b) { return "undefined" == typeof b ? this.is(":text") ? this.val() : this.html() : this.is(":text") ? this.val(b) : this.html(b) }; formatter = {
        getRegex: function (b) {
            b = d.extend({ type: "decimal", precision: 5, decimal: ".", allow_negative: !0 }, b); var c = ""; "decimal" == b.type ? (c = b.allow_negative ? "-?" : "", c = 0 < b.precision ? "^" + c + "\\d+$|^" + c + "\\d*" + b.decimal + "\\d{1," + b.precision +
            "}$" : "^" + c + "\\d+$") : "phone-number" == b.type ? c = "^\\d[\\d\\-]*\\d$" : "alphabet" == b.type && (c = "^[A-Za-z]+$"); return c
        }, isEmail: function (b) { b = d(b).output(); var c = !0, c = /[s~!#$%^&*+=()[]{}<>\/;:,?|]+/; return null != b.match(c) || null != b.match(/((\.\.)|(\.\-)|(\.\@)|(\-\.)|(\-\-)|(\-\@)|(\@\.)|(\@\-)|(\@\@))+/) || -1 != b.indexOf("'") || -1 != b.indexOf('"') || email.tldn && null == b.match(email.tldn) ? !1 : c }, formatString: function (b, c) {
            c = d.extend({ type: "decimal", precision: 5, decimal: ".", allow_negative: !0 }, c); var f = d(b).output(),
            e = f; if ("decimal" == c.type) { if ("" != e) { var a; a = c.allow_negative ? "\\-" : ""; var g = "\\" + c.decimal; a = RegExp("[^\\d" + a + g + "]+", "g"); e = e.replace(a, ""); a = c.allow_negative ? "\\-?" : ""; a = 0 < c.precision ? RegExp("^(" + a + "\\d*" + g + "\\d{1," + c.precision + "}).*") : RegExp("^(" + a + "\\d+).*"); e = e.replace(a, "$1") } } else "phone-number" == c.type ? e = e.replace(/[^\-\d]+/g, "").replace(/^\-+/, "").replace(/\-+/, "-") : "alphabet" == c.type && (e = e.replace(/[^A-Za-z]+/g, "")); e != f && d(b).output(e)
        }
    }; d.fn.format = function (b, c) {
        b = d.extend({
            type: "decimal",
            precision: 5, decimal: ".", allow_negative: !0, autofix: !1
        }, b); var f = b.decimal; c = "function" == typeof c ? c : function () { }; this.keypress(function (c) {
            d(this).data("old-value", d(this).val()); var a = c.charCode ? c.charCode : c.keyCode ? c.keyCode : 0; if (13 == a && "input" != this.nodeName.toLowerCase()) return !1; if (c.ctrlKey && (97 == a || 65 == a || 120 == a || 88 == a || 99 == a || 67 == a || 122 == a || 90 == a || 118 == a || 86 == a || 45 == a) || 46 == a && null != c.which && 0 == c.which) return !0; if (48 > a || 57 < a) {
                if ("decimal" == b.type) return b.allow_negative && 45 == a && 0 == this.value.length ?
                !0 : a == f.charCodeAt(0) ? 0 < b.precision && -1 == this.value.indexOf(f) ? !0 : !1 : 8 != a && 9 != a && 13 != a && 35 != a && 36 != a && 37 != a && 39 != a ? !1 : !0; if ("email" == b.type) return 8 == a || 9 == a || 13 == a || 34 < a && 38 > a || 39 == a || 45 == a || 46 == a || 64 < a && 91 > a || 96 < a && 123 > a || 95 == a || 64 == a && -1 == this.value.indexOf("@") ? !0 : !1; if ("phone-number" == b.type) return 45 == a && 0 == this.value.length ? !1 : 8 == a || 9 == a || 13 == a || 34 < a && 38 > a || 39 == a || 45 == a ? !0 : !1; if ("alphabet" == b.type) { if (8 == a || 9 == a || 13 == a || 34 < a && 38 > a || 39 == a || 64 < a && 91 > a || 96 < a && 123 > a) return !0 } else return !1
            } else return "alphabet" ==
            b.type ? !1 : !0
        }).blur(function () { "email" == b.type ? formatter.isEmail(this) || c.apply(this) : d(this).is(":regex(" + formatter.getRegex(b) + ")") || c.apply(this) }).focus(function () { d(this).select() }); b.autofix && this.keyup(function (c) { d(this).data("old-value") != d(this).val() && formatter.formatString(this, b) }); return this
    }
})(jQuery);