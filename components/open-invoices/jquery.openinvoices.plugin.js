(function ($) {
    var thisOpenInvoices;
    var config = {};

    var global = {
        selected: "",
        selector: ""
    };

    var constants = {
        DOT_VALUE: ".",
        EMPTY_VALUE: ""
    };

    var defaults = {
        accountOpenInvoicesLinkId: "accountOpenInvoicesLink",
        openInvoicesContentId: "openInvoicesContent",
        openInvoicesDisplayRangeId: "openInvoicesDisplayRange",
        openInvoicesLinkId: "lnkOpenInvoices",
        openInvoicesPagerId: "invoice-pager",
        openInvoicesPlaceHolderId: "pnlOpenInvoices",
        pagingHeaderId: "pagingHeader",
        resetLinkId: "reset-invoice-listing",
        openInvoicesCssPath: "components/open-invoices/css/index.css",
        stringResourceKeys: ""
    };

    var constantClassName = {
        LINK_REORDER: ".lnkReOrder",
        PAGE_NUMBER: ".open-invoice-page-number"
    };

    var inputControlsId = {
        DISPLAY_PAGE_TEXT: "txtOpenInvoicesPages"
    };

    var serviceMethods = {
        GET_STRING_RESOURCES: "GetStringResources"
    };

    var appConfigs = {
        IS_SHOW_CUSTOMER_SERVICE_NOTES_IN_RECEIPTS: true,
        ORDER_HISTORY_DEFAULT_PAGE_COUNT: 0
    };

    var stringResource = {
        DISPLAY_TEXT: constants.EMPTY_VALUE,
        OF_TEXT: constants.EMPTY_VALUE,
        ON_TEXT: constants.EMPTY_VALUE,
        RESET_TEXT: constants.EMPTY_VALUE,
        INVOICE_DATE: constants.EMPTY_VALUE,
        INVOICE_CODE: constants.EMPTY_VALUE,
        INVOICE_DUEDATE: constants.EMPTY_VALUE,
        INVOICE_DUETOTAL: constants.EMPTY_VALUE,
        INVOICE_PAYMENTS: constants.EMPTY_VALUE,
        INVOICE_BALANCE: constants.EMPTY_VALUE,
        ACTION_TEXT: constants.EMPTY_VALUE,
        PAY_ONLINE_BUTTON_TEXT: constants.EMPTY_VALUE
    };

    var constantsTemplateID = {
        OPEN_INVOICES_CONTENT_TEMPLATE: "openInvoicesContentTemplate",
        OPEN_INVOICES_PAGER_TEMPLATE: "openInvoicesPager",
        OPEN_INVOICES_PAGINATION_TEMPLATE: "openInvoicesPaginationTemplate",
        OPEN_INVOICES_TEMPLATE: "openInvoicesTemplate",
        OPEN_INVOICES_RANGE_TEMPLATE: "openInvoicesRangeTemplate"
    };

    var init = $.prototype.init;
    $.prototype.init = function (selector, context) {
        var r = init.apply(this, arguments);
        if (selector && selector.selector) {
            r.context = selector.context, r.selector = selector.selector;
        }
        if (typeof selector == "string") {
            r.context = context || document, r.selector = selector;
            global.selector = r.selector;
        }
        global.selected = r;
        return r;
    }

    $.prototype.init.prototype = $.prototype;

    $.fn.OpenInvoices = {
        setup: function (options) {
            setConfig($.extend(defaults, options));
            thisOpenInvoices = this;

            thisOpenInvoices.downloadCss(config.openInvoicesCssPath, function () { });

            var callback = function () {
                thisOpenInvoices.setStringResourcesValue(config.stringResourceKeys);
                thisOpenInvoices.setAppConfigsValue();
                thisOpenInvoices.attachEventsListener();
            }

            thisOpenInvoices.downloadAppConfigs(thisOpenInvoices.getRequiredAppConfigKeys(), callback);
            thisOpenInvoices.downloadStringResources(thisOpenInvoices.convertObjectToArray(config.stringResourceKeys), callback);
        },

        getRequiredAppConfigKeys: function () {
            var keys = new Array();

            keys.push("OpenInvoiceDefaultPageCount");

            return keys;
        },

        setStringResourcesValue: function (stringResourceKeys) {
            stringResource.DISPLAY_TEXT = thisOpenInvoices.getString(stringResourceKeys.displayText);
            stringResource.OF_TEXT = thisOpenInvoices.getString(stringResourceKeys.ofText);
            stringResource.ON_TEXT = thisOpenInvoices.getString(stringResourceKeys.onText);
            stringResource.RESET_TEXT = thisOpenInvoices.getString(stringResourceKeys.resetText);
            stringResource.INVOICE_DATE = thisOpenInvoices.getString(stringResourceKeys.invoiceDate);
            stringResource.INVOICE_CODE = thisOpenInvoices.getString(stringResourceKeys.invoiceCode);
            stringResource.INVOICE_DUEDATE = thisOpenInvoices.getString(stringResourceKeys.invoiceDueDate);
            stringResource.INVOICE_DUETOTAL = thisOpenInvoices.getString(stringResourceKeys.invoiceDueTotal);
            stringResource.INVOICE_PAYMENTS = thisOpenInvoices.getString(stringResourceKeys.invoicePayments);
            stringResource.INVOICE_BALANCE = thisOpenInvoices.getString(stringResourceKeys.invoiceBalance);
            stringResource.ACTION_TEXT = thisOpenInvoices.getString(stringResourceKeys.actionText);
            stringResource.PAY_ONLINE_BUTTON_TEXT = thisOpenInvoices.getString(stringResourceKeys.payonlineButtonText);
        },

        setAppConfigsValue: function () {
            var defaultPageCount =  parseInt(ise.Configuration.getConfigValue("OpenInvoiceDefaultPageCount"));
            if(isNaN(defaultPageCount)) defaultPageCount = 10;
            appConfigs.ORDER_HISTORY_DEFAULT_PAGE_COUNT = defaultPageCount;

        },

        renderOpenInvoicesHeader: function () {
            var resources = new Object();

            resources.invoiceDate = stringResource.INVOICE_DATE;
            resources.invoiceCode = stringResource.INVOICE_CODE;
            resources.invoiceDueDate = stringResource.INVOICE_DUEDATE;
            resources.invoiceDueTotal = stringResource.INVOICE_DUETOTAL;
            resources.invoicePayments = stringResource.INVOICE_PAYMENTS;
            resources.invoiceBalance = stringResource.INVOICE_BALANCE;
            resources.actionText = stringResource.ACTION_TEXT;
 
            var content = new Object();
            content.stringResource = resources;

            var openInvoicesHeadersContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_TEMPLATE, content);
            thisOpenInvoices.setElementHTMLContent(defaults.openInvoicesPlaceHolderId, openInvoicesHeadersContent);

            resources = null;
            content = null;
        },

        attachEventsListener: function () {
            var config = getConfig();
            var thisObject = this;
            
            $(thisOpenInvoices.selectorChecker(config.openInvoicesLinkId)).unbind("click").click(function () {
                
                var pageCount = appConfigs.ORDER_HISTORY_DEFAULT_PAGE_COUNT;
                thisOpenInvoices.renderOpenInvoicesContent(pageCount, 1);

    
            });
        },

        renderOpenInvoicesContent: function (pageCount, currentpage) {
            var callBack = function (result) {

                var invoicesData = result.d;

                var openInvoicesDetails = new Object();
                openInvoicesDetails.invoice = invoicesData.invoice;

                var displayText = new Object();
                displayText.onCaption = stringResource.ON_TEXT;
                displayText.payOnlineButtonText = stringResource.PAY_ONLINE_BUTTON_TEXT;
                openInvoicesDetails.stringResource = displayText;
                reOrderString = null;

                thisOpenInvoices.hideAccountOpenInvoicesLink(function () {

                    thisOpenInvoices.renderOpenInvoicesHeader();

                    var openInvoicesContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_CONTENT_TEMPLATE, openInvoicesDetails);

                    thisOpenInvoices.setElementHTMLContent(config.openInvoicesContentId, $(openInvoicesContent).html());
                    thisOpenInvoices.displayOpenInvoicesRange(invoicesData.rows, invoicesData.start, invoicesData.end, invoicesData.current);
                    thisOpenInvoices.displayOpenInvoicesPager(invoicesData.rows, invoicesData.pages, invoicesData.current, pageCount);

                    $(thisOpenInvoices.selectorChecker(config.resetLinkId)).unbind("click").click(function () {
                        var rowToDisplay = $(thisOpenInvoices.selectorChecker(inputControlsId.DISPLAY_PAGE_TEXT)).val();
                        thisOpenInvoices.getOpenInvoices(rowToDisplay, 1, callBack);
                    });

                    $(thisOpenInvoices.selectorChecker(constantClassName.LINK_REORDER)).unbind("click").click(function () {
                        thisOpenInvoices.displayPrompt(this);
                    });

                });

               
            }

            thisOpenInvoices.getOpenInvoices(pageCount, 1, callBack);
        },

        displayPrompt: function (link) {
            var prompt = stringResource.REORDER_PROMPT;
            if (confirm(prompt)) {
                var code = $(link).attr("data-orderNumber");
                $(link).attr("href", 'reorder.aspx?so=' + code);
            }
        },

        displayOpenInvoicesRange: function (rows, start, end, current) {

            var displayRangeContent = new Object();
            displayRangeContent.rows = rows;
            displayRangeContent.start = start;
            displayRangeContent.end = end;
            displayRangeContent.current = current;

            var displayRangeCaptions = new Object();
            displayRangeCaptions.viewing = stringResource.VIEWING;
            displayRangeCaptions.ofText = stringResource.OF_TEXT;

            displayRangeContent.stringResource = displayRangeCaptions;
            displayRangeCaptions = null;

            var orderRangeContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_RANGE_TEMPLATE, displayRangeContent);
            thisOpenInvoices.setElementHTMLContent(config.openInvoicesDisplayRangeId, orderRangeContent);
        },

        displayOpenInvoicesPager: function (allPages, rowsPerPage, current) {

            var pageDisplayContent = new Object()
            pageDisplayContent.count = rowsPerPage;

            var pageCountCaption = new Object();
            pageCountCaption.display = stringResource.DISPLAY_TEXT;
            pageCountCaption.reset = stringResource.RESET_TEXT;

            pageDisplayContent.stringResource = pageCountCaption;
            pageCountCaption = null;

            var pagesContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_PAGER_TEMPLATE, pageDisplayContent);
            thisOpenInvoices.setElementHTMLContent(config.openInvoicesPagerId, pagesContent);
            thisOpenInvoices.pagination(allPages, rowsPerPage, current);

            $(thisOpenInvoices.selectorChecker(constantClassName.PAGE_NUMBER)).unbind("click").click(function () {
                var current = $(this).attr("data-pagenumber");
                var allPages = $(this).attr("data-allPages");

                var callBack = function (result) {

                    var invoicesData = result.d;

                    var openInvoicesDetails = new Object();
                    openInvoicesDetails.invoice = invoicesData.invoice;

                    var displayText = new Object();
                    displayText.onCaption = stringResource.ON_TEXT;
                    displayText.payOnlineButtonText = stringResource.PAY_ONLINE_BUTTON_TEXT;
                    openInvoicesDetails.stringResource = displayText;
                    reOrderString = null;

                    var openInvoicesContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_CONTENT_TEMPLATE, openInvoicesDetails);
                    thisOpenInvoices.setElementHTMLContent(config.openInvoicesContentId, $(openInvoicesContent).html());
                    thisOpenInvoices.displayOpenInvoicesRange(invoicesData.rows, invoicesData.start, invoicesData.end, invoicesData.current);
                }

                thisOpenInvoices.getOpenInvoices(allPages, current, callBack);
            });
        },

        pagination: function (allPages, rowsPerPage, current) {

            var pageNumbers = Math.ceil(allPages / rowsPerPage);
            if (allPages < rowsPerPage) {
                pageNumbers = 1;
            }

            var pagingContainer = new Array();
            var content = new Object();

            for (var i = 1; i <= pageNumbers; i++) {
                var paging = new Object();

                paging.current = i;
                pagingContainer.push(paging);

                paging = null;
            }

            content.paging = pagingContainer;
            content.allPages = rowsPerPage;

            var pagingContent = thisOpenInvoices.parseJqueryTemplate(constantsTemplateID.OPEN_INVOICES_PAGINATION_TEMPLATE, content);
            thisOpenInvoices.setElementHTMLContent(config.pagingHeaderId, pagingContent);
        },

        getOpenInvoices: function (pageCount, current, callBack) {

            var jsonText = { pages: pageCount, current: current }

            var successCallback = function (result) {
                if (result.d) {
                    if (typeof callBack != 'undefined') callBack(result);
                } else {
                    console.log(result.d);
                }
            }

            var failedCallback = function (result) {
                console.log(result.d);
            }

            thisOpenInvoices.ajaxSecureRequest("GetCustomerOpenInvoices", jsonText, successCallback, failedCallback);
        },

        hideAccountOpenInvoicesLink: function (callback) {
            var config = getConfig();
            $(this.selectorChecker(config.accountOpenInvoicesLinkId)).fadeOut("slow", function () {
                callback();
            });
        }
    }

    $.extend($.fn.OpenInvoices, new jqueryBasePlugin());

    function setConfig(value) { config = value; }
    function getConfig() { return config; }

})(jQuery);
