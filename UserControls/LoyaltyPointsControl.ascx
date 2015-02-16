<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoyaltyPointsControl.ascx.cs" Inherits="UserControls_LoyaltyPointsControl" EnableViewState="true" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

<div class="section-wrapper" id="loyaltyPoints">
     <div style="text-align:left;" class="section-header section-header-top">
        <%= Title %>  
     </div>
        <div class="content">
            <table>
                <tr>
                    <td class="caption"><%= PointsCaption %></td>
                    <td>
                        <asp:Label ID="lblPoints" runat="server" CssClass="points"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="caption"><%= PointsValueCaption %></td>
                    <td>
                        <asp:Label ID="lblMonetaryValue" runat="server" CssClass="value"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="caption"><%= RedeemCaption %></td>
                    <td><%= CurrencySymbol %> <input type="text" id="txtRedemptionAmount"  class="redeem" autocomplete="off" />
                        <input type="hidden" id="hidLoyaltyPoints" runat="server" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <span id="loyaltyPointsError" style="display:none;" class="error"></span>
                    </td>
                </tr>
            </table>
        </div>

    <script type="text/javascript" src="jscripts/jquery/jquery.maskmoney.js"></script>
    <script>
        var LOYALTYPOINTS_STRING_RESOURCE = {
            INVALID_AMOUNT : '<%= ValidationMsgForInvalidAmount %>'
        };
        var LOYALTYPOINTS_CONSTANTS = { 
            REDEMPTION_MULTIPLIER : Number(<%= RedemptionMultipler %>)
        };

        $(document).ready(function () {
            loadExistingPoints();
            loyaltyPointsEvents();

            var numberFormatterInfo = '<%= SystemCurrencyJSON %>';
            var basePlugin = new jqueryBasePlugin();
            var numberFormatterInfoObject = basePlugin.ToJsonObject(numberFormatterInfo);

            if (numberFormatterInfoObject != null) {
                $("#txtRedemptionAmount").maskMoney({
                    thousands: numberFormatterInfoObject.CurrencyGroupSeparator,
                    decimal: numberFormatterInfoObject.CurrencyDecimalSeparator,
                    precision:numberFormatterInfoObject.CurrencyDecimalDigits
                });
            }
            else {
                $("#txtRedemptionAmount").maskMoney();
            }
        });
        
        function loadExistingPoints() {
            var existingPoints = $("#<%= hidLoyaltyPoints.ClientID %>").val();
            var txtRedemptionAmount = $("#txtRedemptionAmount");
            if (existingPoints != null && existingPoints != "") {
                var monetize = existingPoints * LOYALTYPOINTS_CONSTANTS.REDEMPTION_MULTIPLIER;
                txtRedemptionAmount.val(monetize);
            }
        }
        function toNumber(amount) {
            return Number(amount.toString().replace(/[^0-9\.]+/g, ""));
        }
        function loyaltyPointsEvents() {
            $("#txtRedemptionAmount").keyup(function () {
                var inputAmount = toNumber($(this).val());
                var maxAmount = toNumber("<%= MonetaryValue %>");
                var hidPoints = $("#<%= hidLoyaltyPoints.ClientID %>");
                var errorMessage = $("#loyaltyPointsError");
                if (inputAmount != "" && inputAmount != null) {
                    var points = inputAmount / LOYALTYPOINTS_CONSTANTS.REDEMPTION_MULTIPLIER;
                    hidPoints.val( toNumber(points));
                    if (inputAmount <= maxAmount) {
                        errorMessage.hide();
                    }
                    else {
                        errorMessage.html(LOYALTYPOINTS_STRING_RESOURCE.INVALID_AMOUNT);
                        errorMessage.show();
                    }
                }
                else {
                    hidPoints.val("");
                }
            });

            //prevent enter on textbox
            $("#txtRedemptionAmount").keypress(function (event) {
                if (event.which == 13) { event.preventDefault(); }
            });
        }

    </script>
       <div class="div-section-content-footer">
            <asp:Button ID="btnUpdateCart" runat="server" Text="Update" CssClass="btn btn-info" />
        </div>
</div>