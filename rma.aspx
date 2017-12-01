<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rma.aspx.cs" Inherits="rma" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="formRMA" runat="server">
    <asp:Panel ID="pnlContent" runat="server" CssClass="rma">

        <!-- NOTIFICATION -->
        <div class="notification" style="display:none;">
            <a href="javascript:void(0)"><i class="fa fa-times-circle"></i></a>
            <span id="lblMessage"></span>
        </div>

        <!-- CREATE RMA -->
        <asp:Button ID="btnCreateRMA" runat="server" CssClass="btn btn-info right" />
        <br />
        <br />
        <br />

        <!-- RMA -->
        <div id="rmas" class="hide">
            <table class="simple">
                <thead>
                    <tr>
                        <th><%= AppLogic.GetString("rma.aspx.3") %></th>
                        <th><%= AppLogic.GetString("rma.aspx.4") %></th>
                        <th><%= AppLogic.GetString("rma.aspx.5") %></th>
                        <th><%= AppLogic.GetString("rma.aspx.14") %></th>
                        <th class='text-center'><%= AppLogic.GetString("rma.aspx.15") %></th>
                        <th class='text-center'><%= AppLogic.GetString("rma.aspx.16") %></th>
                        <th><%= AppLogic.GetString("rma.aspx.6") %></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody></tbody>
                 <tfoot>
                    <tr>
                        <td colspan="8" class="paging"><div class="paging"></div></td>
                    </tr>
                </tfoot>
            </table>
        </div>

        <!-- SCRIPTS -->
        <script src="jscripts/jquery/jquery.simplePagination.js"></script>
        <script>
            var _rmas = null;
            var _templates = { RMA_ITEM_ROW: "rmaitemrow" }
            var _basePlugin = new jqueryBasePlugin();
            var _pageSize = 30;
            $(function () {
                initTemplates();
                initData();
                initRMAs();
                setNofification();
                attachEvents();
            });

            function initData() {
                _rmas = $.parseJSON(JSON.stringify(<%= GetRMAsJSON() %>));
                var pageSize = Number(<%= AppLogic.AppConfig("RMAPageSize") %>);
                if (pageSize) { _pageSize = pageSize; }
            };
            function initRMAs() {
                if (_rmas == null || _rmas.length == 0) {
                    $("#btnCreateRMA").removeClass("right");
                    return;
                };
                var pageNumber = getCurrentPage();
                var pageSize = _pageSize;
                
                loadRMAs(_rmas, pageNumber, pageSize);

                if (_rmas.length > pageSize) {
                    setRMasPaging(_rmas.length, pageNumber, pageSize);
                }
                else {
                    $("#rmas table .paging").hide();
                }
            };
            function loadRMAs(data, pageNumber, pageSize) {
                if (data.length == 0) return;

                var pageFloor = ((pageNumber - 1) * pageSize) + 1;
                var pageCeiling = pageNumber * pageSize;
                //console.log(data, "PageNo: " + pageNumber, "PageSize: " +pageSize, "PageFloor: " + pageFloor, "PageCeiling: " + pageCeiling);
                var tblRMAContent = $("#rmas table tbody");
                tblRMAContent.empty();

                $.each(data, function (index, val) {
                    var idx = index + 1;
                    if (idx >= pageFloor && idx <= pageCeiling) {
                        tblRMAContent.append(_basePlugin.parseTemplate(_templates.RMA_ITEM_ROW, val));
                    }
                });

                $("#rmas").show();
            };
            function setRMasPaging(itemCount, pageNum, pageSize) {
                $("#rmas table .paging").pagination({
                    items: itemCount,
                    itemsOnPage: pageSize,
                    currentPage: pageNum,
                    displayedPages: 2,
                    cssStyle: 'light-theme',
                    onPageClick: function (pageNumber, event) {
                        loadRMAs(_rmas, pageNumber, pageSize);
                    }
                });
            };
            function attachEvents() {
                $(".void").click(function () {
                    var confirmMessage = '<%= AppLogic.GetString("rma.aspx.13") %>';
                     if (confirm(confirmMessage)) {
                         var rmaCode = $(this).attr("data-rmacode");
                         if (rmaCode) {
                             $.ajax({
                                 type: "POST",
                                 url: "ActionService.asmx/VoidRMA",
                                 data: JSON.stringify({ rmaCode: rmaCode }),
                                 dataType: "json",
                                 contentType: "application/json;charset=utf-8",
                                 success: function (result) { window.location = "rma.aspx"; },
                                 error: function (result, textStatus, exception) { console.log(exception); }
                             });
                         }
                     }
                });
                $(".notification a").click(function (event) {
                    window.location = "rma.aspx";
                });
            }

            // COMMON
            function initTemplates() {
                $.template(_templates.RMA_ITEM_ROW,
                    "<tr>" +
                        "<td><a href='viewrma.aspx?rmacode=${RMACode}'>${RMACode}</a></td>" +
                        "<td>${formatDate(RMADate)}</td>" +
                        "<td>${SalesOrderCode}</td>" +
                        "<td>${InvoiceCode}</td>" +
                        "<td class='text-center'>${TotalRateFormatted}</td>" +
                        "<td class='text-center'>${TotalQuantityReturn}</td>" +
                        "<td class='status no-right-border'>{{if RMAStatus == 'Open'}}" + "<%= AppLogic.GetString("rma.aspx.8") %>" + "{{else}}" + "<%= AppLogic.GetString("rma.aspx.9") %>" + "{{/if}}</td>" +
                        "<td class='cmd no-left-border text-right'>{{if RMAStatus == 'Open'}}<a data-rmacode='${RMACode}' class='void' href='javascript:void(0)'>" + "<%= AppLogic.GetString("rma.aspx.7") %>" + "</a>{{/if}}</td>" +
                    "</tr>"
                );
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
            function setNofification() {
                var messageType = _basePlugin.getQueryStringParamByName("msgtype");
                var message = _basePlugin.getQueryStringParamByName("msg");

                if (message && message != "") {
                    $(".notification").addClass(messageType);
                    $(".notification #lblMessage").html(message);
                    $(".notification").show();
                }
                else {
                    $(".notification").hide();
                }
            };

            // common
            function stringify(data) { return JSON.stringify(data); }
            function formatDate(datetime) {
                var dateObj = new Date(parseInt(datetime.replace("/Date(", "").replace(")/", ""), 10));
                return dateObj.format("MM/dd/yyyy"); //01/01/2001
            }
            function renderTemplate(templateId, data) { return $.tmpl(templateId, data); }
            
        </script>

    </asp:Panel>
    </form>
</body>
</html>
