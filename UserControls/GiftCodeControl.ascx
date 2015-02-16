<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftCodeControl.ascx.cs" Inherits="UserControls_GiftCodeControl" EnableViewState="true" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Infrastructure" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel" %>


<div class="section-wrapper" id="giftCode">
    <div style="text-align:left;" class="section-header section-header-top">
        <%= Title %>  <a href="javascript:void(1);" class="icon icon-chevron-left pull-right" id="aLinkGiftCardResults" data-mode="hide" title="Hide Shipping Methods"></a>
    </div>
    <div class="content">
        <table>
            <tr>
                <td style="height:30px;"><%= SerialCodeInputCaption %></td>
                </tr>
            </tr>
            <tr>
                <td><input type="text" maxlength="30" id="txtGiftCode" /></td>
                <td><input type="button" id="btnSaveGiftCode" value="Add Gift Code" class="btn btn-info" title="<%= NewSerialCodeButtonCaption %>" /></td>
                <td>
                </td>
            </tr>
        </table>
                <div class="pull-right" style="height:20px;">
<span id="giftCodeError" style="display:none;" class="error"></span>
<!-- loader -->
<span id="giftCodeLoader" style="display:none;">
    <img src="images/spinner.gif" alt="" />
    <span id="lblValidating"><%= ValidatingCaption %></span>
</span>
        </div>
<!-- error message -->
                <br />
        <div id="giftCodeContainer" style="display:none;">
            <table id="tblGiftCodes" class="giftcode-table">
                <tr>
                    <th><%= TableHeaderCode %></th>
                    <th><%= TableHeaderBalance %></th>
                    <th><%= TableHeaderType %></th>
                    <th></th>
                </tr>
            </table>
        </div>
        <p id="pGiftCardNotes" class="clr">
            <%=GiftCardsOnCartHelpfulTips %>
        </p>
        <asp:HiddenField ID="hidGiftSerialCodes" runat="server" />
    </div>
    <div class="footer" style="display:none;">
        <asp:Button ID="btnUpdateCart" runat="server" Text="Update" CssClass="btn btn-info" />
    </div>

    <script>
        var giftcodeStringResource = {
            EMPTY_GIFTCODE: '<%= ValidationMsgForEmpty %>',
            INVALID_GIFTCODE : '<%= ValidationMsgForInvalid %>',
            ZEROBALANCE_GIFTCODE: '<%= ValidationMsgForZeroBalance%>'
        };

        $(document).ready(function () {

            giftCodeEvents();
            giftCodeTemplates();
            loadExistingGiftCodes();

        });

        function giftCodeEvents() {

            //add giftcode button click event
            $("#btnSaveGiftCode").click(function () {
                var txtGiftCode = $("#txtGiftCode");
                var code = txtGiftCode.val();
                validateGiftCode(code);
            });

            //remove giftcode button click event
            $(".remove-giftcode").live("click", function () {
                var code = $(this).attr("code");
                $("#tblGiftCodes tr[code='" + code + "']").remove();
                updateSelectedGiftCodes();

                var remainingCodes = 0;
                $("#tblGiftCodes tr").each(function (key, val) {
                    if ($(val).attr("code") != null) { remainingCodes += 1; }
                });
                
                if (remainingCodes == 0) {
                  //  $("#giftCodeContainer").hide();
                }
                else {
                   // $("#giftCodeContainer").show();
                }

            });

            //giftcode textbox key up event
            $("#txtGiftCode").keyup(function (event) {
                if (event.which == 13) { $("#btnSaveGiftCode").click(); }
            });

            //giftcode textbox key press event
            $("#txtGiftCode").keypress(function (event) {
                if (event.which == 13) { event.preventDefault(); }
            });

            //aLinkShowGiftCardsListing link key press event
            $("#aLinkShowGiftCardsListing").unbind("click").click(function () {
                showGiftCardsListing();
            });
        }

        function loadExistingGiftCodes() {
            $.ajax({
                type: "POST",
                url: 'ActionService.asmx/GetGiftCodesAppliedToShoppingCart',
                async: true,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    var codes = $.parseJSON(result.d);
                    if (codes.length > 0) {
                        $.each(codes, function (key, val) {
                            addGiftCode(val.SerialCode, val.CreditAvailableFormatted, val.Type);
                            updateSelectedGiftCodes();
                        });

                        $("#pGiftCardNotes").fadeIn("slow");
                        $("#spanGiftCardsCounter").html(codes.length);
                    }

                },
                error: function (result, textStatus, exception) { }
            });
        }

        function validateGiftCode(code) {
            var txtGiftCode = $("#txtGiftCode");
            var btnSaveGiftCode = $("#btnSaveGiftCode");
            var giftCodeLoader = $("#giftCodeLoader");
            var giftCodeError = $("#giftCodeError");

            if (code == "" || code == null)
            {
                giftCodeError.text(giftcodeStringResource.EMPTY_GIFTCODE);
                giftCodeError.show();
                return;
            }

            giftCodeError.hide();

            //check if duplicate
            var exists = isCodeAlreadyExists(code);
            if (exists) { return; }

            //show loader and hide save btn
         //   btnSaveGiftCode.hide();
            giftCodeLoader.show();

            //validate code
            $.ajax({
                type: "POST",
                url: 'ActionService.asmx/IsValidGiftCode',
                data: JSON.stringify({ code : code }),
                async: true,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    isValid = Boolean(result.d);

                    window.setTimeout(function () {
                        if (isValid) {
                            var detail = getCodeDetail(code)[0];
                            if (detail == null) {
                                giftCodeError.text(giftcodeStringResource.INVALID_GIFTCODE);
                                giftCodeError.show();
                                return;
                            }

                            if (detail.CreditAvailable > 0) {
                                addGiftCode(detail.SerialCode, detail.CreditAvailableFormatted, detail.Type);
                                updateSelectedGiftCodes();
                                txtGiftCode.val("");
                                showGiftCardsListing();
                            }
                            else {
                                giftCodeError.text(giftcodeStringResource.ZEROBALANCE_GIFTCODE);
                                giftCodeError.show();
                            }
                        }
                        else {
                            giftCodeError.text(giftcodeStringResource.INVALID_GIFTCODE);
                            giftCodeError.show();
                        }
                        //hide loader and show save btn
                      //  btnSaveGiftCode.show();
                        giftCodeLoader.hide();

                    }, 0);
                },
                error: function (result, textStatus, exception) { }
            });
        }

        function getCodeDetail(code) {
            var detail = null;
            $.ajax({
                type: "POST",
                url: 'ActionService.asmx/GetGiftCodeInfo',
                data: JSON.stringify({ code: code }),
                async: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    detail = $.parseJSON(result.d);
                },
                error: function (result, textStatus, exception) { }
            });
            return detail;
        }

        function isCodeAlreadyExists(code) {
            var exists = false;
            var giftCodes = $("#tblGiftCodes tr");
            $.each(giftCodes, function (key, val) {
                if ($(val).attr("code") != null) {
                    if ($(val).attr("code").toLowerCase() == code.toLowerCase()) { exists = true; }
                }
            });
            return exists;
        }

        function formatBalance(balance) {
            return "<%= RemainingBalanceCaption %>" + balance;
        }

        function addGiftCode(code, balance, type) {
           // $("#giftCodeContainer").show();
            $("#tblGiftCodes tr:last").after(parseTemplate("giftcode-row", { Code: code, Balance: balance, Type : type}));
        }

        function updateSelectedGiftCodes() {
            var hidGiftCodes = $("input[name*='hidGiftSerialCodes']");
            var tblGiftCodes = $("#tblGiftCodes tr");
            var codes = "";

            tblGiftCodes.each(function () {
                var code = $(this).attr("code");
                if (code != null) {
                    codes += code + ",";
                }
            });
            hidGiftCodes.val(codes);
        }

        function giftCodeTemplates() {
            $.template("giftcode-row", "<tr code='${Code}'>" +
                                            "<td class='code'>${Code}</td>" +
                                            "<td class='balance'>${Balance}</td>" +
                                            "<td class='type'>${Type}</td>" +
                                            "<td class='del'><a class='remove-giftcode' code='${Code}' href='javascript:void(0)'><i class='icon-remove' /></a></td>" +
                                       "</tr>");
        }
        
        function parseTemplate(templateId, data) {
            return $.tmpl(templateId, data);
        }

        function showGiftCardsListing() {
            var index = $("#divGiftCodeWrapper").attr("data-index");
            var vdirection = 'left';

            if (index % 2 == 0) {
                vdirection = 'up';
            }

            var $aLinkGiftCardResults = $("#aLinkGiftCardResults");
            var $buttonApplyGiftCards = $("#aLinkApplyGiftCardsUpdateCart");
          
            var $divListing = $("#divGiftCardsListing");
            var $divGiftCardContainer = $("#giftCodeContainer");

            var html = $divListing.html();
            html = $.trim(html);

            var listing = $divGiftCardContainer.html();
            listing = $.trim(listing);

            $("#divGiftCodeWrapper").css("width", "50%");
            var isWidgetAlone = $("#divGiftCodeWrapper").attr("data-widgetAlone") == "true";

            if (html == '' && listing != '') {

                $divListing.html("<div id='giftCodeContainer'>" + listing + "</div>");
                $divGiftCardContainer.remove();

                if(isWidgetAlone==false){
                    showGiftCardSliderDirectionIcon();
                }
            }

            $("#pGiftCardNotes").fadeOut("slow");
            $("#divGiftCardsSlider").show('slide', { direction: vdirection });

            $aLinkGiftCardResults.unbind("click").click(function () {
                $this = $(this);
                var mode = $this.attr("data-mode");
                mode = $.trim(mode);

                var hideIcon = 'icon-chevron-left';
                var showIcon = 'icon-chevron-right';

                if (vdirection == 'up') {
                    hideIcon = 'icon-chevron-up';
                    showIcon = 'icon-chevron-down';
                }

                if (mode == 'show') {
                    $("#divGiftCardsSlider").show('slide', { direction: vdirection });
                    $this.removeClass(showIcon).addClass(hideIcon);
                    $this.attr("data-mode", "hide");
                    $this.attr("title", '<%=GiftCardsOnCartTitleHideTips%>');
                } else {
                    $("#divGiftCardsSlider").hide('slide', { direction: vdirection });
                    $this.removeClass(hideIcon).addClass(showIcon);
                    $this.attr("data-mode", "show");
                    $this.attr("title", '<%=GiftCardsOnCartTitleShowTips%>');
                }

            });

            $buttonApplyGiftCards.unbind("click").click(function () {
                $("#giftCodeControl_btnUpdateCart").trigger("click");
            });

        }

        function showGiftCardSliderDirectionIcon() {

            var index = $("#divGiftCodeWrapper").attr("data-index");
            var icon = "icon-chevron-left";

            if (index % 2 == 0) {
                icon = "icon-chevron-up";
            }
                
            var $aLinkGiftCardResults = $("#aLinkGiftCardResults");
            var mode = $aLinkGiftCardResults.attr("data-mode");
            mode = $.trim(mode);

            var display = $aLinkGiftCardResults.css("display");
            display = $.trim(display);

            $aLinkGiftCardResults.fadeIn("slow");

            $aLinkGiftCardResults.removeClass("icon-chevron-right").addClass(icon);
            $aLinkGiftCardResults.attr("data-mode", "hide");
            $aLinkGiftCardResults.attr("title", '<%=GiftCardsOnCartTitleHideTips%>');


        }
    </script>

</div>