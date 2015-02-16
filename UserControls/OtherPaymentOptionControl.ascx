<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OtherPaymentOptionControl.ascx.cs" Inherits="UserControls_OtherPaymentOptionControl" %>
<asp:Panel ID="pnlOtherPaymentOptions" runat="server">
    <div id="otherPayment" class="sections-place-holder no-padding">
        <div class="header section-header section-header-top"><%= Header %></div>
        <div class="content section-content-wrapper">
            <table class="payments" border="1">
                <tbody class="giftcodes"></tbody>
                <tbody class="loyaltypoints"></tbody>
                <tbody class="creditmemos"></tbody>
            </table>
        </div>
        <div class="footer">
            <input type="button" id="btnApplyCreditPayment" value="<%= ButtonApplyCaption %>" disabled="disabled" class="btn" />
            
        </div>
    </div>
    <script src="components/other-payment/setup.js"></script>
    <script>
        $(document).ready(function () {
            var stringresources = {
                HEADER_CREDITMEMO: '<%= HeaderCreditMemo %>',
                HEADER_LOYALTYPOINTS: '<%= HeaderLoyaltyPoints %>',
                HEADER_GIFTCODE: '<%= HeaderGiftCode %>',
                HEADER_BALANCE_AVAILABLE: '<%= HeaderBalanceAvailable %>',
                HEADER_APPLY_AMOUNT: '<%= HeaderApplyAmount %>',
                CURRENCY_SYMBOL: '<%= CurrencySymbol %>',
                POINTSEARNED_TEXT: '<%= PointsEarnedText %>',
                BTN_ADDGIFTCODE_TOOLTIP: '<%= ButtonAddGiftCodeTooltip %>',
                BTN_SAVEGIFTCODE_TOOLTIP: '<%= ButtonSaveGiftCodeTooltip %>',
                BTN_CANCELGIFTCODE_TOOLTIP: '<%= ButtonCancelGiftCodeTooltip %>',
                GIFTCODE_TEXT: '<%= GiftCodeText %>',
                GIFTCODE_EMPTY: '<%= ValidationGiftCodeEmpty %>',
                GIFTCODE_INVALID: '<%= ValidationGiftCodeInvalid %>',
                GIFTCODE_ZEROBALANCE: '<%= ValidationGiftCodeZeroBalance %>',
                LOADER_TEXT: '<%= LoaderText%>'
            };

            var options = {
                CreditMemosJSON: '<%= CreditMemosJSON %>',
                CreditMemosAppliedJSON: '<%= CreditMemosAppliedJSON %>',
                LoyaltyPointsJSON: '<%= LoyaltyPointsJSON %>',
                LoyaltyPointsApplied: Number('<%= LoyaltyPointsApplied %>'),
                GiftCodesJSON: '<%= GiftCodesJSON %>',
                GiftCodesAppliedJSON: '<%= GiftCodesAppliedJSON %>',
                IsCreditMemoEnabled: ToBoolean('<%= IsCreditMemoEnabled %>'),
                IsLoyaltyPointsEnabled: ToBoolean('<%= IsLoyaltyPointsEnabled %>'),
                IsGiftCodeEnabled: ToBoolean('<%= IsGiftCodeEnabled %>'),
                StringResources: stringresources,
                BtnApplyID: 'btnApplyCreditPayment',
                BtnEnterNewGiftCodeID: '.new-giftcode',
                RedemptionMultiplier : Number('<%= RedemptionMultiplier %>')
            };
            $("#otherPayment").OtherPaymentOptions(options);
            $("#otherPayment input[data-type='payment']").numeric();
        });
        function ToBoolean(val) {
            if (val.toLowerCase() == "true") {
                return true;
            }
            return false;
        }
    </script>

</asp:Panel>
    