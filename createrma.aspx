<%@ Page Language="C#" AutoEventWireup="true" CodeFile="createrma.aspx.cs" Inherits="InterpriseSuiteEcommerce.createrma" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>

<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="formRMA" runat="server">

    <asp:Panel ID="pnlContent" runat="server" CssClass="rma">

    <!-- ORDERS / INVOICES -->
    <div id="invoices" class="hide">
        <table class="simple">
            <thead>
                <tr>
                    <th><%= AppLogic.GetString("createrma.aspx.21") %></th>
                    <th><%= AppLogic.GetString("createrma.aspx.22") %></th>
                    <th><%= AppLogic.GetString("createrma.aspx.23") %></th>
                    <th><%= AppLogic.GetString("createrma.aspx.30") %></th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <!-- orders/invoices row will be loaded here -->
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="4" class="paging"><div class="paging"></div></td>
                </tr>
            </tfoot>
        </table>
        <br />
        <asp:Button ID="btnCancel2" runat="server" CssClass="btn btn-info" />
    </div>

    <!-- INVOICE DETAIL -->
     <% if(CurrentInvoice != null) { %>
        <table class="plain">
            <tr>
                <td><%= AppLogic.GetString("createrma.aspx.2") %></td>
                <td><%= CurrentInvoice.SalesOrderCode %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("createrma.aspx.3") %></td>
                <td><%= CurrentInvoice.SalesOrderDate.ToString("MM/dd/yy") %></td>
            </tr>
              <tr>
                <td><%= AppLogic.GetString("createrma.aspx.31") %></td>
                <td><%= CurrentInvoice.InvoiceCode %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("createrma.aspx.4") %></td>
                <td><%= CurrentInvoice.TotalRateFormatted %></td>
            </tr>
            <tr>
                <td colspan="2" style="padding-top:15px;">
                    <%= AppLogic.GetString("createrma.aspx.5") %>
                </td>
            </tr>
            <tr>
                <td colspan="2" id="items" style="padding-bottom:15px;">
                    <table class="simple">
                        <thead>
                            <tr>
                                <th><%= AppLogic.GetString("createrma.aspx.6") %></th>
                                <th><%= AppLogic.GetString("createrma.aspx.7") %></th>
                                <th><%= AppLogic.GetString("createrma.aspx.8") %></th>
                                <th class="text-center"><%= AppLogic.GetString("createrma.aspx.9") %></th>
                                <th class="text-center"><%= AppLogic.GetString("createrma.aspx.10") %></th>
                            </tr>
                        </thead>
                        <tbody><!-- load invoice items here --></tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("createrma.aspx.11") %></td>
                <td><asp:DropDownList ID="ddlRequestType" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="vertical-top"><%= AppLogic.GetString("createrma.aspx.12") %></td>
                <td><asp:TextBox ID="txtReason" TextMode="MultiLine" MaxLength="240" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-bottom:20px;">
                    <input type="checkbox" id="chkAgree" style="vertical-align:top;" />
                    <span class="agree">
                        <%=AppLogic.GetString("createrma.aspx.15")  %>
                        <a href="t-returns.aspx" target="_blank"><%=AppLogic.GetString("createrma.aspx.16")  %></a>
                        <%=AppLogic.GetString("createrma.aspx.17")  %>
                    </span>
                </td>

            </tr>
            <tr>
                <td></td>
                <td><asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-success" Enabled="false" />
                    <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-info" OnClientClick="return confirm(getCancelRMAConfirmMessage());"  />
                    <asp:Button ID="btnBackToOrderSelection" runat="server" CssClass="btn btn-info" OnClientClick="return confirm(getGoBackToSelectionConfirmMessage());"  />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnInvoiceItems" runat="server" />
    <% } %>
        
    <!-- SCRIPTS -->
    <script src="jscripts/jquery/jquery.simplePagination.js"></script>
    <script>
        var _invoices = null;
        var _invoiceItems = null;
        var _pageSize = 30;
        var _basePlugin = new jqueryBasePlugin();
        var _templates = { INVOICE_ROW: "invoicerow", INVOICE_ITEM_ROW : "invoiceitemrow" };

        $(function () {
            initTemplates();
            initData();
            initInvoices();
            initInvoiceItems();
            attachEvents();
        });
        function initData () {
            _invoices = $.parseJSON(JSON.stringify(<%= GetCustomerInvoicesJSON() %>));
            _invoiceItems = $.parseJSON(JSON.stringify(<%= GetCustomerInvoiceItemsJSON() %>));
            var pageSize = Number(<%= AppLogic.AppConfig("RMAPageSize") %>);
            if (pageSize) { _pageSize = pageSize; }
        };
        function initInvoices() {
            if (_invoices == null || _invoices.length == 0) return;
            var pageNumber = getCurrentPage();
            var pageSize = _pageSize;
            loadInvoices(_invoices, pageNumber, pageSize);
            if (_invoices.length > pageSize) {
                setInvoicesPaging(_invoices.length, pageNumber, pageSize);
            }
            else {
                $("#invoices table .paging").hide();
            }
        };
        function loadInvoices(data, pageNumber, pageSize) {
            if (data.length == 0) return;

            var pageFloor = ((pageNumber - 1) * pageSize) + 1;
            var pageCeiling = pageNumber * pageSize;
            //console.log(data, "PageNo: " + pageNumber, "PageSize: " +pageSize, "PageFloor: " + pageFloor, "PageCeiling: " + pageCeiling);
            var tblInvoiceContent = $("#invoices table tbody");
            tblInvoiceContent.empty();

            $.each(data, function (index, val) {
                var idx = index + 1;
                if (idx >= pageFloor && idx <= pageCeiling) {
                    tblInvoiceContent.append(_basePlugin.parseTemplate(_templates.INVOICE_ROW, val));
                }
            });

            $("#invoices").show();
        };
        function setInvoicesPaging(itemCount, pageNum, pageSize) {
            $("#invoices table .paging").pagination({
                items: itemCount,
                itemsOnPage: pageSize,
                currentPage: pageNum,
                displayedPages: 2,
                cssStyle: 'light-theme',
                onPageClick: function (pageNumber, event) {
                    //event.preventDefault();
                    loadInvoices(_invoices, pageNumber, pageSize);
                }
            });
        };
        function initInvoiceItems() {
            if (_invoiceItems == null || _invoiceItems.length == 0) return;
            loadInvoiceItems(_invoiceItems);
        };
        function loadInvoiceItems(data) {
            if (data.length == 0) return;
            var tblItems = $("#items table tbody");
            tblItems.empty();
            $.each(data, function (index, val) {
                tblItems.append(_basePlugin.parseTemplate(_templates.INVOICE_ITEM_ROW, val));
            });
        };
        function validate() {
            var isAgreeToTermsChecked = $("#chkAgree").prop("checked");
            var isValid = checkQuanties() && isAgreeToTermsChecked;
            var btnSubmit = $("#<%= btnSubmit.ClientID %>");

            if (isValid) {
                $(btnSubmit).removeAttr("disabled");
                saveItems();
            }
            else {
                $(btnSubmit).attr("disabled", "disabled");
            }
        };
        function checkQuanties() {
            var isValid = true;
            var totalQty = 0;

            if (_invoiceItems == null || _invoiceItems.length == 0) return;
                
            $.each(_invoiceItems, function (index, invoice) {
                var row = $("#items table").find("tr[data-rownum='" + invoice.LineNum + "']");
                if (row.length > 0) {
                    var input = $(row).find("input");
                    if (input) {
                        var qty = Number(input.val());
                        var autoUpdateQty;

                        if (qty > invoice.QuantityAvailable) {
                            input.addClass("invalid");
                            input.attr("title", "invalid return quantity");
                            isValid = false;

                            autoUpdateQty = setTimeout(function () {
                                input.val(invoice.QuantityAvailable);
                                validate();
                            }, 0);
                        }
                        else {
                            input.removeClass("invalid");
                            input.attr("title", "");
                            totalQty += qty;

                            clearTimeout(autoUpdateQty);
                        }
                    }
                }
            });
            return isValid && (totalQty > 0);
        };
        function saveItems() {
            var items = [];
            $.each(_invoiceItems, function (index, invoice) {
                var row = $("#items table").find("tr[data-rownum='" + invoice.LineNum + "']");
                if (row.length > 0) {
                    var input = $(row).find("input");
                    if (input) {
                        var qty = Number(input.val());
                        items.push({
                            LineNum: invoice.LineNum,
                            QuantityToReturn: qty
                        });
                    }
                }
            });

            if (items.length > 0) {
                $("#hdnInvoiceItems").val(JSON.stringify(items));
            }
        };
        function attachEvents() {
            var txtRetQty = $("#items table tr td input");
            var chkAgree = $("#chkAgree");

            $(chkAgree).change(function () {
                validate();
            });

            $(txtRetQty).live("keydown", function (event) {
                preventNonNumericKey(event);
                preventEnterKey(event);
            });

            $(txtRetQty).live("keyup", function (event) {
                validate();
            });
        };
        function getCancelRMAConfirmMessage() {
            return "<%= AppLogic.GetString("createrma.aspx.28") %>";
        };
        function getGoBackToSelectionConfirmMessage() {
            return "<%= AppLogic.GetString("createrma.aspx.29") %>";
        };
        // COMMON
        function initTemplates() {
            $.template(_templates.INVOICE_ROW, 
                "<tr>" +
                    "<td>${SalesOrderCode}</td>" +
                    "<td>${formatDate(SalesOrderDate)}</td>" +
                    "<td class='no-right-border'>${InvoiceCode}</td>" +
                    "<td class='no-right-border'>${TotalRateFormatted}</td>" +
                    "<td class='text-right no-left-border'><a href='createrma.aspx?invoicecode=${InvoiceCode}'>" + "<%= AppLogic.GetString("createrma.aspx.24") %>" + "</a></td>" +
                "</tr>"
            );
            $.template(_templates.INVOICE_ITEM_ROW,
                   "<tr data-rownum='${LineNum}'>" +
                       "<td>${ItemDescription}</td>" +
                       "<td>${UPCCode}</td>" +
                       "<td>${UnitMeasureCode}</td>" +
                       "<td class='text-center'>${QuantityAvailable}</td>" +
                       "<td class='text-center'><input type='text' class='ret' value='${QuantityAvailable}' /></td>" +
                "</tr>"
            );
        };
        function formatDate(datetime) {
            var dateObj = new Date(parseInt(datetime.replace("/Date(", "").replace(")/", ""), 10));
            return dateObj.format("MM/dd/yyyy"); //01/01/2001
        };
        function preventNonNumericKey(e) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
                // Allow: Ctrl+A
                (e.keyCode == 65 && e.ctrlKey === true) ||
                // Allow: home, end, left, right
                (e.keyCode >= 35 && e.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        };
        function preventEnterKey(event) {
            if (event.which == 13) { event.preventDefault(); }
        };
        function getCurrentPage() {
            var pageNum = 1;
            var hash = window.location.hash;
            if (hash && hash != "") {
                var arr = hash.split("-");
                return Number(arr[1]);
            }
            return pageNum;
        };

    </script>

    </asp:Panel>
    </form>
</body>
</html>
