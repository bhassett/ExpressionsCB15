<%@ Page Language="C#" AutoEventWireup="true" CodeFile="viewrma.aspx.cs" Inherits="viewrma" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="formRMA" runat="server">

        <asp:Panel runat="server" ID="pnlContent" CssClass="rma">
        <table class="plain">
            <tr>
                <td style="width:150px;"><%= AppLogic.GetString("viewrma.aspx.3") %></td>
                <td><%= CurrentRMA.RMACode %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("viewrma.aspx.4") %></td>
                <td><%= GetAlternateStatus(CurrentRMA.RMAStatus) %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("viewrma.aspx.5") %></td>
                <td><%= CurrentRMA.RMADate.ToString("MM/dd/yy") %></td>
            </tr>
            <tr>
                <td style="padding-top:15px;"><%= AppLogic.GetString("viewrma.aspx.6") %></td>
                <td style="padding-top:15px;"><%= CurrentRMA.SalesOrderCode %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("viewrma.aspx.7") %></td>
                <td><%= CurrentRMA.SalesOrderDate.ToString("MM/dd/yy") %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("viewrma.aspx.16") %></td>
                <td><%= CurrentRMA.InvoiceCode %></td>
            </tr>
            <tr>
                <td><%= AppLogic.GetString("viewrma.aspx.8") %></td>
                <td><%= CurrentRMA.TotalRateFormatted %></td>
            </tr>
            <tr>
                <td style="padding-top:15px;"><%= AppLogic.GetString("viewrma.aspx.9") %></td>
                <td></td>
            </tr>
            <tr>
                <td colspan="2">
                    <table class="simple">
                        <thead>
                            <tr>
                                <th><%= AppLogic.GetString("viewrma.aspx.11") %></th>
                                <th><%= AppLogic.GetString("viewrma.aspx.12") %></th>
                                <th><%= AppLogic.GetString("viewrma.aspx.13") %></th>
                                <th class="text-center"><%= AppLogic.GetString("viewrma.aspx.14") %></th>
                                <th class="text-center"><%= AppLogic.GetString("viewrma.aspx.15") %></th>
                            </tr>
                        </thead>
                        <tbody>
                            <% foreach(var item in CurrentRMAItems) { %>
                            <tr>
                                <td><%= item.ItemDescription %></td>
                                <td><%= item.UPCCode %></td>
                                <td><%= item.UnitMeasureCode %></td>
                                <td class="text-center"><%= item.QuantityShipped.ToNumberFormat() %></td>
                                <td class="text-center"><%= item.QuantityOrdered.ToNumberFormat() %></td>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>

                </td>
            </tr>
            <tr>
                <td style="vertical-align:top;padding-top:15px;">Notes:</td>
                <td style="padding-top:15px;">
                    <textarea disabled="disabled"><%= CurrentRMA.Notes %></textarea>
                </td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-top:15px;">
                    <asp:Button ID="btnBack" runat="server" CssClass="btn btn-info" /></td>
            </tr>
        </table>
        </asp:Panel>
    </form>
</body>
</html>
