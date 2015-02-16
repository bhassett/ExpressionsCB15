

;(function ($, window, document, undefined) {
    var pluginName = "OtherPaymentOptions";
    var defaults = {
        CreditMemos: null,
        LoyaltyPoints: null,
        GiftCodes: null,
        CreditMemosApplied: null,
        LoyaltyPointsApplied: null,
        GiftCodesApplied: null,
        IsCreditMemoEnabled: false,
        IsLoyaltyPointsEnabled: false,
        IsGiftCodeEnabled: false,
        BtnApplyID: '',
        BtnEnterNewGiftCodeID : '',
        RedemptionMultiplier: 0
    };
    var templates = {
        ROW_HEADER: "RowHeader",
        ROW_CONTENT: "RowContent",
        ROW_CONTENT_WITH_REMOVEITEM: "RowContentWithRemoveItem",
        ROW_ADDGIFTCODE: "RowAddGiftCode",
        STATUS_VALID: "StatusValid",
        STATUS_INVALID: "StatusInvalid",
        BTN_ADDGIFTCODE: "BtnAddGiftCode"
    };
    var stringresources = {
        EMPTY: "",
        COMMA: ",",
        PIPELINE: "|",
        VALIDATING: " validating..."
    };
    var codetype = {
        CREDIT_MEMO: "creditmemo",
        LOYALTY_POINTS: "loyaltypoints",
        GIFT_CODE: "giftcode"
    }

    var constants = {
        LOYALTYPTS_CODE : "loyalty-pts"
    }
    
    function Plugin(element, options) {
        var showOtherPayment = false;
        var showNewGiftCodeButton = false;

        this.element = element;
        this.options = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
        this.setTemplates();

        if (defaults.IsCreditMemoEnabled) {
            this.loadCreditMemos();
            this.loadAppliedCreditMemos();

            if (defaults.CreditMemos != null && defaults.CreditMemos.length > 0) { showOtherPayment = true; }
        }

        if (defaults.IsLoyaltyPointsEnabled) {
            this.loadLoyaltyPoints();
            this.loadAppliedLoyaltyPoints();

            if (defaults.LoyaltyPoints != null && defaults.LoyaltyPoints.length > 0) { showOtherPayment = true; }
        }

        if (defaults.IsGiftCodeEnabled) {
            this.loadGiftCodes();
            this.loadAppliedGiftCodes();

            if (defaults.GiftCodes != null && defaults.GiftCodes.length > 0) { showOtherPayment = true; }
            else { showNewGiftCodeButton = true; }
        }

        this.setPanelOtherPaymentVisibility(showOtherPayment);
        this.setBtnNewGiftCodeVisibility(showNewGiftCodeButton);

        this.initEvents();
    }

    Plugin.prototype = {
        init: function () {
            var basePlugin = new jqueryBasePlugin();
            
            defaults.CreditMemos = basePlugin.ToJsonObject(this.options.CreditMemosJSON);
            defaults.CreditMemosApplied = basePlugin.ToJsonObject(this.options.CreditMemosAppliedJSON);
            defaults.LoyaltyPoints = basePlugin.ToJsonObject(this.options.LoyaltyPointsJSON);
            defaults.LoyaltyPointsApplied = this.options.LoyaltyPointsApplied;
            defaults.GiftCodes = basePlugin.ToJsonObject(this.options.GiftCodesJSON);
            defaults.GiftCodesApplied = basePlugin.ToJsonObject(this.options.GiftCodesAppliedJSON);
            defaults.IsCreditMemoEnabled = this.options.IsCreditMemoEnabled;
            defaults.IsLoyaltyPointsEnabled = this.options.IsLoyaltyPointsEnabled;
            defaults.IsGiftCodeEnabled = this.options.IsGiftCodeEnabled;
            defaults.BtnApplyID = this.options.BtnApplyID;
            defaults.BtnEnterNewGiftCodeID = this.options.BtnEnterNewGiftCodeID;
            defaults.RedemptionMultiplier = this.options.RedemptionMultiplier;
            stringresources = $.extend({}, stringresources, this.options.StringResources);
        },
        setTemplates: function () {
            $.template(templates.ROW_HEADER, "<tr class='head-row'>" +
                                                "<th>${HeaderType} {{html ButtonAdd}}</th>" +
                                                "<th>${HeaderBalance}</th>" +
                                                "<th>${HeaderApplyAmount}</th>" +
                                             "</tr>");
            $.template(templates.ROW_CONTENT, "<tr data-code='${Code}' data-codetype='${CodeType}'>" +
                                                    "<td class='desc'>${Description}</td>" +
                                                    "<td class='balance'>${Balance}</td>" +
                                                    "<td class='apply'>${CurrencySymbol} <input data-type='payment' type='text' autocomplete='off' /><span class='status' /></td>" +
                                                  "</tr>");
            $.template(templates.ROW_CONTENT_WITH_REMOVEITEM, "<tr data-code='${Code}' data-codetype='${CodeType}'>" +
                                                    "<td class='desc'>${Description} <a href='javascript:void(0)' title='click to remove' class='remove-item'><i class='icon-trash'></i></a></td>" +
                                                    "<td class='balance'>${Balance}</td>" +
                                                    "<td class='apply'>${CurrencySymbol} <input data-type='payment' type='text' autocomplete='off' /><span class='status' /></td>" +
                                                  "</tr>");
            $.template(templates.STATUS_VALID, "&nbsp;<icon class='icon-ok' />");
            $.template(templates.STATUS_INVALID, "&nbsp;<icon class='icon-remove' />");
            $.template(templates.BTN_ADDGIFTCODE, "<a id='btnAddGiftCode' href='javascript:void(0)' title='${AddGiftCodeTooltip}'><i class='icon-plus-sign-alt' /></a>");
            $.template(templates.ROW_ADDGIFTCODE, "<tr class='new-giftcode'><td colspan='3'>" +
                                                        "<span class='caption'>${Description}</span>" +
                                                        "<input id='txtGiftCode' type='text' />" +
                                                        "<a id='btnSaveGiftCode' href='javascript:void(0)' title='${SaveGiftCodeTooltip}'><i class='icon-ok-sign' /></a>" +
                                                        "<a id='btnCancelGiftCode' href='javascript:void(0)' title='${CancelGiftCodeTooltip}'><i class='icon-remove-sign'></i></a>" +
                                                        "<span class='loader' style='display:none'><img src='images/spinner.gif' alt='' />${LoaderMessage}</span>" +
                                                        "<span class='error-msg'></span>" +
                                                  "</td></tr>")
        },
        renderTemplate: function (id, data) {
            return $.tmpl(id, data);
        },
        loadCreditMemos: function () {
            var self = this;
            var container = $(this.element).find(".creditmemos");
            var creditmemos = defaults.CreditMemos;
            if (creditmemos != null) {
                var header = {
                    HeaderType: stringresources.HEADER_CREDITMEMO,
                    HeaderBalance: stringresources.HEADER_BALANCE_AVAILABLE,
                    HeaderApplyAmount: stringresources.HEADER_APPLY_AMOUNT
                };
                $(container).append(self.renderTemplate(templates.ROW_HEADER, header));

                $.each(creditmemos, function (index, creditmemo) {
                    var content = {
                        Description: creditmemo.CreditCode,
                        Balance: creditmemo.CreditRemainingBalanceFormatted,
                        CurrencySymbol: stringresources.CURRENCY_SYMBOL,
                        Code: creditmemo.CreditCode,
                        CodeType: codetype.CREDIT_MEMO
                    };
                    $(container).append(self.renderTemplate(templates.ROW_CONTENT, content));
                });
            }
        },
        loadAppliedCreditMemos: function () {
            var self = this;
            var container = SelectorChecker(this.element.id);
            var creditmemos = defaults.CreditMemosApplied;
            if (creditmemos != null) {
                $.each(creditmemos, function (index, creditmemo) {
                    var applied = creditmemo.CreditAppliedInShoppingCart;
                    if (applied != "") {
                        var row = $(container).find("tr[data-code='" + creditmemo.CreditCode + "']");
                        var input = $(row).find("input[data-type='payment']")
                        $(input).val(applied);
                        self.checkAmount(input);
                    }
                });
            }
        },
        loadLoyaltyPoints: function () {
            var self = this;
            var container = $(this.element).find(".loyaltypoints");
            var loyaltypoints = defaults.LoyaltyPoints;
            
            if (loyaltypoints != null) {

                // hide loyalty points redemption if zero
                if (loyaltypoints[0] != null && loyaltypoints[0].MonetizedRemainingPoints == 0) { return; }

                var header = {
                    HeaderType: stringresources.HEADER_LOYALTYPOINTS,
                    HeaderBalance: stringresources.HEADER_BALANCE_AVAILABLE,
                    HeaderApplyAmount: stringresources.HEADER_APPLY_AMOUNT
                };
                $(container).append(self.renderTemplate(templates.ROW_HEADER, header));
                $.each(loyaltypoints, function (index, loyaltypoint) {
                    var content = {
                        Description: stringresources.POINTSEARNED_TEXT + loyaltypoint.RemainingPointsFormatted,
                        Balance: loyaltypoint.MonetizedRemainingPointsFormatted,
                        CurrencySymbol: stringresources.CURRENCY_SYMBOL,
                        Code: constants.LOYALTYPTS_CODE,
                        CodeType: codetype.LOYALTY_POINTS
                    };
                    $(container).append(self.renderTemplate(templates.ROW_CONTENT, content));
                });

            }

        },
        loadAppliedLoyaltyPoints: function () {
            var container = SelectorChecker(this.element.id);
            var loyaltypoints = defaults.LoyaltyPointsApplied;
            var self = this;
            
            if (loyaltypoints != null) {
                var applied = loyaltypoints * defaults.RedemptionMultiplier;
                if (applied > 0) {
                    var row = $(container).find("tr[data-code='" + constants.LOYALTYPTS_CODE + "']");
                    var input = $(row).find("input[data-type='payment']")
                    $(input).val(applied);
                    self.checkAmount(input);
                }
            }
        },
        loadGiftCodes: function () {
            var self = this;
            var container = $(this.element).find(".giftcodes");
            var giftcodes = defaults.GiftCodes;
            if (giftcodes != null) {
                var htmlBtnAdd = self.renderTemplate(templates.BTN_ADDGIFTCODE, { AddGiftCodeTooltip: stringresources.BTN_ADDGIFTCODE_TOOLTIP })[0];
                if (htmlBtnAdd != null) { htmlBtnAdd = htmlBtnAdd.outerHTML; }
                var header = {
                    HeaderType: stringresources.HEADER_GIFTCODE,
                    ButtonAdd: htmlBtnAdd,
                    HeaderBalance: stringresources.HEADER_BALANCE_AVAILABLE,
                    HeaderApplyAmount: stringresources.HEADER_APPLY_AMOUNT
                };
                $(container).append(self.renderTemplate(templates.ROW_HEADER, header));

                $.each(giftcodes, function (index, giftcode) {

                    if (giftcode.CreditAvailable == 0) { return }; // do not display giftcode with no creditavailable

                    var templateID = templates.ROW_CONTENT;
                    if (!giftcode.IsOwned) { templateID = templates.ROW_CONTENT_WITH_REMOVEITEM; }

                    var content = {
                        Description: giftcode.Type + ": " + giftcode.SerialCode,
                        Balance: giftcode.CreditAvailableFormatted,
                        CurrencySymbol: stringresources.CURRENCY_SYMBOL,
                        Code: giftcode.SerialCode,
                        CodeType: codetype.GIFT_CODE
                    };
                    $(container).append(self.renderTemplate(templateID, content));
                });
            }
        },
        loadAppliedGiftCodes: function () {
            var self = this;
            var container = SelectorChecker(this.element.id);
            var giftcodes = defaults.GiftCodesApplied;
            if (giftcodes != null) {
                $.each(giftcodes, function (index, giftcode) {
                    var applied = giftcode.AmountApplied;
                    if (applied != "") {
                        var row = $(container).find("tr[data-code='" + giftcode.SerialCode + "']");
                        var input = $(row).find("input[data-type='payment']")
                        if (input != null) {
                            $(input).val(applied);
                            self.checkAmount(input);
                        }
                    }
                });
            }
        },
        checkAmount: function (input) {
            var amount = Number($(input).val());
            var row = $(input).closest("tr");
            var code = $(row).data("code");
            var codetype = $(row).data("codetype");
            var status = $(row).find(".status");
            
            if (amount > 0) {
                var isvalid = this.isValidAmount(amount, code, codetype);
                if (isvalid) {
                    $(status).html(this.renderTemplate(templates.STATUS_VALID));
                }
                else {
                    $(status).html(this.renderTemplate(templates.STATUS_INVALID));
                }
            }
            else {
                $(status).html(stringresources.EMPTY);
            }
        },
        checkAmounts: function () {
            var applybutton = SelectorChecker(defaults.BtnApplyID);
            var container = SelectorChecker(this.element.id);
            var isvalid = true;

            if (defaults.CreditMemos != null) {
                $.each(defaults.CreditMemos, function (index, creditmemo) {
                    var row = $(container).find("tr[data-code='" + creditmemo.CreditCode + "']");
                    var amount = Number($(row).find("input[data-type='payment']").val());
                    if (amount > creditmemo.CreditRemainingBalance) { isvalid = false; }
                });
            }
            
            if (defaults.LoyaltyPoints != null) {
                $.each(defaults.LoyaltyPoints, function (index, loyaltypoint) {
                    var row = $(container).find("tr[data-code='" + constants.LOYALTYPTS_CODE + "']");
                    var amount = Number($(row).find("input[data-type='payment']").val());
                    if (amount > loyaltypoint.MonetizedRemainingPoints) { isvalid = false; }
                });
            }
            
            if (defaults.GiftCodes != null) {
                $.each(defaults.GiftCodes, function (index, giftcode) {
                    var row = $(container).find("tr[data-code='" + giftcode.SerialCode + "']");
                    var amount = Number($(row).find("input[data-type='payment']").val());
                    if (amount > giftcode.CreditAvailable) { isvalid = false; }
                });
            }

            if (isvalid) {
                $(applybutton).removeAttr("disabled");
            }
            else {
                $(applybutton).attr("disabled", "disabled");
            }
        },
        isValidAmount: function (amount, code, type) {
            var matches = null;
            var available = 0;

            if (type == codetype.CREDIT_MEMO) {
                matches = $.grep(defaults.CreditMemos, function (e) { return e.CreditCode == code });
                $.each(matches, function (index, val) {
                    available += val.CreditRemainingBalance;
                });
            }

            if (type == codetype.LOYALTY_POINTS) {
                $.each(defaults.LoyaltyPoints, function (index, val) {
                    available += val.MonetizedRemainingPoints;
                });
            }

            if (type == codetype.GIFT_CODE) {
                matches = $.grep(defaults.GiftCodes, function (e) { return e.SerialCode == code });
                $.each(matches, function (index, val) {
                    available += val.CreditAvailable;
                });
            }

            if (amount <= available) { return true; }
            return false;
        },
        getTotal: function () {
            var amounttotal = 0;
            var payments = $(this.element).find("input[data-type='payment']");
            $.each(payments, function (index, payment) {
                var amount = $(payment).val();
                if (amount != null) {
                    amounttotal += Number(amount);
                }
            });
            return amounttotal;
        },
        saveCredits: function () {
            var payments = $(this.element).find("input[data-type='payment']");
            var creditmemos = [];
            var giftcodes = [];
            var loyaltypoints = null;
            var reloadpage = false;

            $.each(payments, function (index, payment) {
                var row = $(payment).closest("tr");
                var type = $(row).data("codetype");
                var code = $(row).attr("data-code");
                var amount = $(payment).val();

                if (amount == stringresources.EMPTY) { amount = 0; }

                if (type == codetype.CREDIT_MEMO) {
                    creditmemos.push({
                        CreditCode: code,
                        CreditAppliedInShoppingCart: amount
                    });
                }

                if (type == codetype.LOYALTY_POINTS) {
                    loyaltypoints = amount / defaults.RedemptionMultiplier;
                }

                if (type == codetype.GIFT_CODE) {
                    giftcodes.push({
                        SerialCode: code,
                        AmountApplied: amount
                    });
                }
            });

            if (creditmemos.length > 0) {
                var data = { "jsonData": JSON.stringify(creditmemos) };
                this.ajaxSecured("ApplyCreditMemos", data, function () { reloadpage = true; });
            }

            if (loyaltypoints != null) {
                var data = { "points": loyaltypoints };
                this.ajaxSecured("ApplyLoyaltyPoints", data, function () { reloadpage = true; });
            }

            if (giftcodes != null) {
                var data = { "jsonData": JSON.stringify(giftcodes) };
                this.ajaxSecured("ApplyGiftCodes", data, function () { reloadpage = true; });
            }
            
            if (reloadpage) {
                location.reload();
            }
        },
        addGiftCode: function () {
            var self = this;
            var container = $(self.element).find(".giftcodes");

            // check if giftcode input is already populated
            var hasAddGiftCodeRow = ($(container).find(".new-giftcode").length == 1);
            if (!hasAddGiftCodeRow) {
                var content = {
                    Description: stringresources.GIFTCODE_TEXT,
                    LoaderMessage: stringresources.LOADER_TEXT,
                    SaveGiftCodeTooltip: stringresources.BTN_SAVEGIFTCODE_TOOLTIP,
                    CancelGiftCodeTooltip: stringresources.BTN_CANCELGIFTCODE_TOOLTIP
                };
                $(container).append(self.renderTemplate(templates.ROW_ADDGIFTCODE, content));
                $(container).find(SelectorChecker("txtGiftCode")).focus();
            }
        },
        cancelGiftCode: function () {
            var self = this;
            var container = $(self.element).find(".giftcodes");
            $(container).find("tr.new-giftcode").remove();
        },
        saveGiftCode: function () {
            var self = this;
            var container = $(self.element).find(".giftcodes");
            var txtgiftcode = $(container).find("#txtGiftCode");
            var errormessage = $(container).find(".error-msg");
            var loader = $(container).find(".loader");
            var btncancelgiftcode = $(container).find("#btnCancelGiftCode");
            var btnsavegiftcode = $(container).find("#btnSaveGiftCode");

            var giftcode = $(txtgiftcode).val();

            // initialize
            errormessage.empty();

            // check if empty 
            if (giftcode == stringresources.EMPTY) {
                errormessage.text(stringresources.GIFTCODE_EMPTY);
                return;
            }

            // check if existing
            var matches = $.grep(defaults.GiftCodes, function (e) { return e.SerialCode == giftcode });
            if (matches.length > 0) {
                errormessage.text(stringresources.GIFTCODE_INVALID);
                return;
            }

            // check if valid
            var isValid = self.isValidGiftCode(giftcode);
            if (!isValid) {
                errormessage.text(stringresources.GIFTCODE_INVALID);
                return;
            }

            // check if already owned
            var isOwned = self.isGiftCodeOwned(giftcode);
            if (isOwned) {
                // we'll just reload the page, no need to further 
                // add the giftcode as this is already owned by customer
                location.reload();
                return;
            }

            // save giftcode
            var data = { "code" : giftcode };
            self.ajaxSecured("AddGiftCode", data, function () {
                location.reload();
            });
        },
        removeGiftCode: function (giftcode) {
            // save giftcode
            var data = { "code": giftcode };
            this.ajaxSecured("RemoveGiftCode", data, function () { });
        },
        isValidGiftCode: function (serialcode) {
            var valid = false;
            var data = { "code": serialcode };
            this.ajaxSecured("VerifyGiftCode", data, function (result) { valid = Boolean(result.d); });
            return valid;
        },
        isGiftCodeOwned: function (serialcode) {
            var owned = false;
            var data = { "code": serialcode };
            this.ajaxSecured("CheckIfGiftCodeIsOwned", data, function (result) { owned = Boolean(result.d); });
            return owned;
        },
        setBtnNewGiftCodeVisibility: function (show) {
            if (show) {
                $(SelectorChecker(defaults.BtnEnterNewGiftCodeID)).show();
            }
            else {
                $(SelectorChecker(defaults.BtnEnterNewGiftCodeID)).hide();
            }
        },
        setPanelOtherPaymentVisibility: function (show) {
            if (show) {
                $(SelectorChecker(this.element.id)).show();
            }
            else {
                $(SelectorChecker(this.element.id)).hide();
            }
        },
        initEvents: function () {
            var self = this;
            var inputamount = SelectorChecker(self.element.id) + " input[data-type='payment']";
            var btnapply = SelectorChecker(defaults.BtnApplyID);
            var btnaddgiftcode = SelectorChecker("btnAddGiftCode");
            var btncancelgiftcode = SelectorChecker("btnCancelGiftCode");
            var btnsavegiftcode = SelectorChecker("btnSaveGiftCode");
            var btnenternewgiftcode = SelectorChecker(defaults.BtnEnterNewGiftCodeID);
            var btnremoveitem = SelectorChecker(".remove-item");

            $(inputamount).keyup(function (event) {
                self.checkAmount(this);
                self.checkAmounts();
            });
            $(inputamount).keypress(function (event) {
                PreventEnterKey(event);
            });
            $(btnapply).click(function (event) {
                self.saveCredits();
            })
            $(btnaddgiftcode).click(function (event) {
                self.addGiftCode();
            });
            $(btncancelgiftcode).live("click", function (event) {
                self.cancelGiftCode();
            });
            $(btnsavegiftcode).live("click", function (event) {
                self.saveGiftCode();
            });
            $(btnenternewgiftcode).click(function (event) {
                self.setBtnNewGiftCodeVisibility(false);
                self.setPanelOtherPaymentVisibility(true);
                $(btnaddgiftcode).click();
            });
            $(btnremoveitem).click(function (event) {
                var row = $(this).closest("tr");
                var giftCode = $(row).attr("data-code");
                row.remove();
                self.removeGiftCode(giftCode);
                self.saveCredits();
            });
        },
        ajaxSecured: function (method, data, success, async) {
            if (async) { AjaxCallWithSecuritySimplified(method, data, success); }
            else { AjaxCallWithSecuritySynchronous(method, data, success); }
        }
    };

    function PreventNonNumericKey(event) {
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 || (event.keyCode == 65 && event.ctrlKey === true) || (event.keyCode >= 35 && event.keyCode <= 39)) { return; }
        else {
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) { event.preventDefault(); }
        }
    }

    function PreventEnterKey(event) {
        if (event.which == 13) { event.preventDefault(); }
    }

    function SelectorChecker(selector) {
        if (selector == "") return selector;

        if (selector.indexOf(".") == -1) {
            selector = "#" + selector;
        }
        return selector;
    }

    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName,
                new Plugin(this, options));
            }
        });
    };
})(jQuery, window, document);