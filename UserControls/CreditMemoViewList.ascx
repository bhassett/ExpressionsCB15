<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CreditMemoViewList.ascx.cs" Inherits="UserControls_CreditMemoViewList" %>

<div class="content" id="creditMemo">
    <a href="javascript:void(0)" id="btnViewCreditMemos"  class="btn btn-info btn-block">
        <%= ViewCreditsCaption %>
    </a>
    <span id="notFound" style="display:none;">
        <%= NotFoundMessage %>
    </span>
    <table id="tblCreditMemos" class="creditmemo-table" style="display:none;"></table>

    <script>
        var creditMemosJSON = '<%= CreditMemosJSON %>';
        var templates = { ROW_HEADER: "CreditMemosRowHeader", ROW_CONTENT : "CreditMemosRowContent" }
        var stringresources = { HEADER_CREDIT: "<%= CreditCodeHeader%>", HEADER_BALANCE: "<%= BalanceHeader  %>" }
        var hasCreditMemos = false;
        $(document).ready(function () {
            setTemplates();
            loadCreditMemos();
            creditMemosEvents();
        });

        function renderTemplate(id, data) {
            return $.tmpl(id, data);
        }
        function setTemplates() {
            $.template(templates.ROW_HEADER, "<tr>" +
                                               "<th>${HeaderCredit}</th>" +
                                               "<th>${HeaderBalance}</th>" +
                                            "</tr>");
            $.template(templates.ROW_CONTENT, "<tr>" +
                                                "<td>${Description}</td>" +
                                                "<td>${Balance}</td>" +
                                              "</tr>");
        }
        function loadCreditMemos() {
            var basePlugin = new jqueryBasePlugin();
            var creditmemos = basePlugin.ToJsonObject(creditMemosJSON);
            var container = $("#tblCreditMemos");

            if (creditmemos != null) {
                var header = {
                    HeaderCredit: stringresources.HEADER_CREDIT,
                    HeaderBalance: stringresources.HEADER_BALANCE
                };
                $(container).append(renderTemplate(templates.ROW_HEADER, header));
                $.each(creditmemos, function (index, creditmemo) {
                    var content = {
                        Description: creditmemo.CreditCode,
                        Balance: creditmemo.CreditRemainingBalanceFormatted
                    };
                    $(container).append(renderTemplate(templates.ROW_CONTENT, content));
                });
                
                if (creditmemos.length > 0) { hasCreditMemos = true; }
            }
        }
        function creditMemosEvents() {
            $("#btnViewCreditMemos").click(function () {
                var tblCreditMemos = $("#tblCreditMemos");
                var lblNotFound = $("#notFound");
                $(this).hide();
                if (hasCreditMemos) {
                    $(tblCreditMemos).show();
                    $(lblNotFound).hide();
                }
                else {
                    $(tblCreditMemos).hide();
                    $(lblNotFound).show();
                }
            });
        }
        
    </script>

</div>