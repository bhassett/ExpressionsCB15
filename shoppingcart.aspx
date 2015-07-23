<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.ShoppingCartPage" CodeFile="ShoppingCart.aspx.cs" EnableViewState="true" ValidateRequest="false" %>
<%@ Register TagPrefix="ise" Namespace="InterpriseSuiteEcommerceControls" Assembly="InterpriseSuiteEcommerceControls" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script type="text/javascript" src="jscripts/minified/address.control.js"></script>
    <script type="text/javascript" src="jscripts/minified/address.verification.js"></script>
    <script type="text/javascript" src="jscripts/shippingcalculator.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.easing.js"></script>
</head>
<body>
    <asp:Literal ID="ValidationScript" runat="server"></asp:Literal>
    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
    <form id="CartForm" onsubmit="return Cart_Validator(this)" runat="server">
    <b><asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal></b>
    <asp:Panel ID="BodyPanel" runat="server">
        <div class="row">
            <ise:Topic runat="server" ID="HeaderMsg" TopicName="CartPageHeader" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageHeader" runat="server"></asp:Literal>
            
            <div class="small-12 medium-5 columns paddingbottom">
                <div class="panel">
                    <asp:Panel ID="ShippingInformation" runat="server">
                        <asp:Image ID="redarrow1" AlternateText="" runat="server" visible="false" /><a onclick="popuptopicwh('Shipping+Information','shipping',650,550,'yes')"
                            href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx8" runat="server"></asp:Literal></a><br />
                    </asp:Panel>

                    <asp:Image ID="redarrow2" AlternateText="" runat="server" visible="false" /><a onclick="popuptopicwh('Return+Policy+Information','returns',650,550,'yes')"
                        href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx9" Text="(!shoppingcart.aspx.7!)"
                            runat="server"></asp:Literal></a><br />

                    <asp:Image ID="redarrow3" AlternateText="" runat="server" visible="false" /><a onclick="popuptopicwh('Privacy+Information','privacy',650,550,'yes')"
                        href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx10" Text="(!shoppingcart.aspx.8!)"
                            runat="server"></asp:Literal></a><br />

                    <asp:Panel ID="AddresBookLlink" runat="server">
                        <asp:Image ID="redarrow4" AlternateText="" runat="server" visible="false" /><a href="selectaddress.aspx?returnurl=shoppingcart.aspx&AddressType=Shipping"><asp:Literal
                            ID="shoppingcartaspx11" Text="(!shoppingcart.aspx.9!)" runat="server"></asp:Literal></a>
                    </asp:Panel>
                </div>
            </div>

            <div class="small-12 medium-7 columns small-only-text-center text-right">
                <ul class="button-group">
                    <li><asp:Button ID="btnContinueShoppingTop" Text="(!shoppingcart.cs.12!)" CssClass="button small secondary" runat="server" /></li>
                    <li><asp:Button ID="btnCheckOutNowTop" Text="(!shoppingcart.cs.34!)" runat="server" CssClass="button small" /></li>
                </ul>
            </div>

            <div class="small-12 columns small-only-text-center text-right" runat="server" id="AlternativeCheckoutsTop">
                <h5><asp:Label ID="Label3" runat="server" Text="(!shoppingcart.aspx.14!)"></asp:Label></h5>
                <span runat="server" id="PayPalExpressSpanTop" visible="false">
                    <asp:ImageButton ID="btnPayPalExpressCheckoutTop" cms-3rdparty-attr runat="server" OnClick="btnPayPalExpressCheckout_Click" />
                </span>
            </div>
            
            <div class="small-12 columns">
            <asp:Panel ID="pnlCouponError" runat="Server" Visible="false">
                  <asp:Label ID="CouponError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlErrorMsg" runat="Server" Visible="false">
                  <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlRemovePhasedOutItemWithNoStockError" runat="Server" Visible="false">
                  <asp:Label ID="RemovePhasedOutItemWithNoStockError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlInventoryTrimmedError" runat="Server" Visible="false">
                  <asp:Label ID="InventoryTrimmedError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMinimumQuantitiesUpdatedError" runat="Server" Visible="false">
                  <asp:Label ID="MinimumQuantitiesUpdatedError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderAmountError" runat="Server" Visible="false">
                  <asp:Label ID="MeetsMinimumOrderAmountError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderWeightError" runat="Server" Visible="false">
                  <asp:Label ID="MeetsMinimumOrderWeightError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderQuantityError" runat="Server" Visible="false">
                  <asp:Label ID="MeetsMinimumOrderQuantityError" CssClass="alert-box alert" runat="Server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlOversizedShippingMethodNotValid" runat="Server" Visible="false">
                  <asp:Label ID="litOversizedShippingMethodNotValid" runat="Server" CssClass="alert-box alert" Text="(!shoppingcart.aspx.62!)"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMicropay_EnabledError" runat="Server" Visible="false">
                <asp:Literal ID="Micropay_EnabledError" runat="Server"></asp:Literal>
            </asp:Panel>
            </div>
            <div style="clear: both"></div>
            <div class="hidden errorLg" id="required-error">
                <asp:Literal ID="lRequiredError" runat="server" Visible="True" Text="(!leadform.aspx.16!)"></asp:Literal>
            </div>

            <div class="sections-place-holder no-padding">
                <div id="divCartSummaryContentWrapper" class="section-content-wrapper">
                    <asp:Panel ID="pnlCartSummary" runat="server" HorizontalAlign="right" DefaultButton="btnUpdateCart1">
                        <asp:Literal ID="CartItems" runat="server"></asp:Literal>
                        <asp:Panel ID="pnlCartSummarySubTotals" runat="server">
                            <asp:Literal ID="CartSubTotal" runat="server"></asp:Literal>
                        </asp:Panel>
                        <div class="row">
                            <div class="small-11 columns text-right">
                                <asp:Button ID="btnUpdateCart1" CssClass="button small" Text="(!shoppingcart.cs.33!)" runat="server" />
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>

        </div>

           <div id="divInlineSectionsWrapper">
           <%-- Shopping Cart Calculator Section --%>
            <!-- <div class="div-inline-sections" data-panelId="pnlShippingCalculator" id="divShippingCalculator" data-titleHideTips="<%= AppLogic.GetString("shoppingcart.aspx.59")%>" data-titleShowTips="<%= AppLogic.GetString("shoppingcart.aspx.60")%>">
                <div  class="div-section-content-wrapper">
                        <div style="text-align:left;" class="section-header section-header-top">
                            <span><%= AppLogic.GetString("shoppingcart.aspx.20")%></span>
                              <a href="javascript:void(1);" class="icon icon-chevron-left pull-right" id="aLinkShippingResults" data-mode="hide" title="<%= AppLogic.GetString("shoppingcart.aspx.60")%>"></a>
                        </div>
                        <asp:Panel ID="pnlShippingCalculator" runat="Server" Visible="false" >
                                <div id="pnlShippingCalculatorcontainer">
                                    <div class="shipping-calculator-wrapper">
                                        <span class="shipping-calculator-label">
                                            <%= AppLogic.GetString("shoppingcart.aspx.21")%>
                                        </span>
                                        <br />
                                        <uc:AddressControl ID="AddressControl" runat="server" />
                                        <div class="clr"></div>
                                    </div>
                                    <div class="clr"></div>
                                </div>
                                <div class="clr"></div>
                        </asp:Panel>
                      <div class="div-section-content-footer">
                           <a href="javascript:void(1);" style="color:#fff;" class="button small" id="btnCalcShip"><%= AppLogic.GetString("shoppingcart.aspx.26")%></a>
                      </div>
                </div>
            </div> -->

            <div class="row">
            <div class="small-12 medium-6 medium-push-6 columns">

            <!-- Coupon -->
            <div class="row">
            <div class="small-12 columns">
                <asp:Panel ID="pnlCoupon" runat="server" Visible="false" DefaultButton="btnUpdateCart3">
                <h5><%= AppLogic.GetString("checkoutshipping.aspx.14")%></h5>
                <div class="row small-collapse cart-coupon">
                    <div  class="small-8 columns">
                        <asp:TextBox ID="CouponCode" runat="server"></asp:TextBox>
                    </div>
                    <div class="small-4 columns">
                       <asp:Button ID="btnUpdateCart3" runat="server" Text="(!shoppingcart.aspx.51!)" CssClass="button small postfix" />
                    </div>
                </div>
                </asp:Panel>
            </div>
            </div>

            </div>
            <div class="small-12 medium-6 medium-pull-6 columns">

            <!-- Order Notes -->
                <div data-panelId="pnlOrderNotes" id="divOrderNotesWrapper">
                    <asp:Panel ID="pnlOrderNotes" runat="server" Visible="false" DefaultButton="btnUpdateCart4">
                    <h5><%= AppLogic.GetString("mobile.shoppingcart.aspx.20")%> <small>(optional)</small></h5>
                        <asp:TextBox ID="OrderNotes" Columns="90" Rows="4" TextMode="MultiLine" placeholder="Enter any additional information you may want us to know" runat="server"></asp:TextBox>
                    <asp:Button ID="btnUpdateCart4" runat="server" Text="(!shoppingcart.aspx.54!)" CssClass="button small" />
                    </asp:Panel>
                </div>
            </div>

            </div>
            </div>

        </div>
        </div>
        

        <!-- Checkout Options -->
        <asp:Panel ID="pnlOrderOptions" runat="server" Visible="false">
        <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="Literal2" runat="server">Options</asp:Literal></div>
                <div class="section-content-wrapper">
                <div class="clr height-12"></div>

                        <div style="text-align: center; width: 100%;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td align="left"><asp:Label ID="shoppingcartcs27" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.5!)" runat="server"></asp:Label></td>
                                    <td><asp:Label ID="shoppingcartcs121" runat="server" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.37!)"></asp:Label></td>
                                    <td align="center"><asp:Label ID="shoppingcartcs28" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.6!)" runat="server"></asp:Label></td>
                                    <td width="25" align="center"><asp:Label ID="shoppingcartcs29" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.7!)" runat="server"></asp:Label></td>
                                </tr>
                                <tr><td colspan="4" style="height:12px;"></td></tr>
                                <asp:Repeater ID="OrderOptionsList" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td align="left">
                                                <asp:Image ID="OptionImage" runat="server" Visible="false" />
                                                <asp:Label ID="OrderOptionName" CssClass="OrderOptionsName" runat="server"></asp:Label>
                                                <asp:Image ID="helpcircle_gif" runat="server" AlternateText='<%# InterpriseSuiteEcommerceCommon.AppLogic.GetString("shoppingcart.cs.8") %>' Style="cursor: pointer;" />
                                            </td>
                                            <td align="center">
                                                <asp:Label ID="lblUnitMeasureCode" runat="server" Text=""></asp:Label>
                                                <asp:DropDownList ID="cboUnitMeasureCode" runat="server"></asp:DropDownList>
                                            </td>
                                            <td align="center">
                                                <asp:Label ID="OrderOptionPrice" CssClass="OrderOptionsPrice" runat="server"></asp:Label>
                                            </td>
                                            <td align="center">
                                                <asp:HiddenField ID="hfItemCounter" runat="server" />
                                                <ise:DataCheckBox ID="OrderOptions" runat="server" Data='<%# ((System.Xml.XmlNode)Container.DataItem)["ItemCode"].InnerText %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="left" colspan="4">
                                                <div class="clr height-12"></div>
                                                <asp:Label ID="lblNotes" Text="Notes:" runat="server" /><br />
                                                <asp:TextBox ID="txtOrderOptionNotes" TextMode="MultiLine" runat="server" Width="100%" />
                                                <div class="clr height-12"></div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                  
    
                    <div><asp:Button ID="btnUpdateCart2" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="button small" /></div>
                </div>
            </div>
            </asp:Panel>

            <!-- Upsell Products -->
           <asp:Panel ID="pnlUpsellProducts" runat="server" Visible="false">
                <asp:Literal ID="UpsellProducts" runat="server"></asp:Literal>
                <div style="text-align: right;">
                    <asp:Button ID="btnUpdateCart5" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="site-button content"
                        Visible="false" /></div>
            </asp:Panel>

            <div class="row">
            <div class="small-12 columns small-only-text-center text-right">
                <ul class="button-group">
                    <li><asp:Button ID="btnContinueShoppingBottom" Text="(!shoppingcart.cs.12!)" CssClass="button small secondary" runat="server" /></li>
                    <li><asp:Button ID="btnCheckOutNowBottom" Text="(!shoppingcart.cs.34!)" runat="server" CssClass="button small" /></li>
                </ul>
            </div>

            <div class="small-12 columns small-only-text-center text-right" runat="server" id="AlternativeCheckoutsBottom">
                <h5><asp:Label ID="Label1" runat="server" Text="(!shoppingcart.aspx.14!)" Style="margin-right: 7px;"></asp:Label></h5>
                <span runat="server" id="PayPalExpressSpanBottom" visible="false">
                    <asp:ImageButton ID="btnPayPalExpressCheckoutBottom" cms-3rdparty-attr runat="server" OnClick="btnPayPalExpressCheckout_Click" />
                </span>
            </div>
            </div>

            <ise:Topic ID="CartPageFooterTopic" runat="server" TopicName="CartPageFooter" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageFooter" runat="server"></asp:Literal>
            <span id=""></span>
        
        <script>
        var constants = {
            CLASS_SELECTOR: ".",
            EMPTY_VALUE: "",
            UNDEFINED: "undefined",
            ID_SELECTOR: "#"
        };

        var constantID = {
            DIV_INLINE_SECTIONS_WRAPPER: "#divInlineSectionsWrapper",
            DIV_SHIPPING_CALCULATOR: "divShippingCalculator",
            DIV_GIFT_CODE_WRAPPER: "divGiftCodeWrapper",
            DIV_COUPON_WRAPPER: "divCouponWrapper",
            DIV_ORDER_NOTES: "divOrderNotesWrapper"
        }

        var constantClassName = {
            DIV_INLINE_SECTIONS: "div-inline-sections",
            DIV_IS_CLEAR: "is-clear"
        }

        var constantAttribute = {
            ID: "id",
            DATA_INDEX: "data-index",
            WIDGET_ALONE: "data-widgetAlone",
            WIDTH: "width",
            STYLE: "style"
        }

        var constantElement = {
            DIV : "div"
        }

        var stringResource = {
            SHIPPING_CALCULATOR_RESULTS_HEADER_TEXT: "<%= AppLogic.GetString("shoppingcart.aspx.57")%>",
            SHIPPING_CALCULATOR_RESULTS_UPDATE_CART_TEXT: "<%= AppLogic.GetString("shoppingcart.cs.33")%>",
            GIFT_CODES_RESULTS_APPLY_GIFT_CARDS_TEXT: "<%= AppLogic.GetString("shoppingcart.aspx.58")%>"
        };
     
        var shippingSectionsHtml = '<div id="divShippngMethodsSlider" class="section-wrapper">';
           
            shippingSectionsHtml += '<div class="div-section-content-wrapper">';
            shippingSectionsHtml += '<div id="divShippngMethodsSliderHeader" class="section-header section-header-top">' + stringResource.SHIPPING_CALCULATOR_RESULTS_HEADER_TEXT + '</div>';
                        shippingSectionsHtml += '<div id="shippingMethodOpt"></div>';
                        shippingSectionsHtml += '<div class="clr"></div>';
                        shippingSectionsHtml += '</div>';
                        shippingSectionsHtml += '<div class="shipping-calculator-controls div-section-content-footer">';
                        shippingSectionsHtml += '<a href="javascript:void(1);"  class="btn-info site-button content" id="aLinkShippingResultsUpdateCart">' + stringResource.SHIPPING_CALCULATOR_RESULTS_UPDATE_CART_TEXT + '</a>';
                        shippingSectionsHtml += '</div>';
                shippingSectionsHtml += '<div class="clr"></div>';
            shippingSectionsHtml += '<div>';

        var giftCardsSectionsHtml = '<div id="divGiftCardsSlider" class="section-wrapper">';
                giftCardsSectionsHtml += '<div id="divGiftCardsSliderHeader" class="section-header section-header-top">Your Gift Cards</div>';
                giftCardsSectionsHtml += '<div class="div-section-content-wrapper">';
                        giftCardsSectionsHtml += '<div id="divGiftCardsListing"></div>';
                        giftCardsSectionsHtml += '<div class="clr"></div>';
                        giftCardsSectionsHtml += '</div>';
                        giftCardsSectionsHtml += '<div class="shipping-calculator-controls div-section-content-footer">';
                        giftCardsSectionsHtml += '<a href="javascript:void(1);"  class="btn-info site-button content" id="aLinkApplyGiftCardsUpdateCart">' + stringResource.GIFT_CODES_RESULTS_APPLY_GIFT_CARDS_TEXT + '</a>';
                        giftCardsSectionsHtml += '</div>';
                giftCardsSectionsHtml += '<div class="clr"></div>';
            giftCardsSectionsHtml += '<div>';

        function scanShoppingCartInlineSections() {
            var counter = 0;

            $(constantID.DIV_INLINE_SECTIONS_WRAPPER).children(constantElement.DIV).each(function () {

                var $this = $(this);
                if ($this.hasClass(constantClassName.DIV_INLINE_SECTIONS) == false) {
                    return false;
                }

                var sectionContent = $("#" + $.trim($this.attr("data-panelId"))).html();
                sectionContent = $.trim(sectionContent);

                if (typeof(sectionContent) == "undefined" || sectionContent == null || sectionContent == constants.EMPTY_VALUE) {
                    $this.remove();
                }
   
                if (counter % 2 == 1 && sectionContent != constants.EMPTY_VALUE) {
                    $this.after("<div class='is-clear' style='clear:both;heigth:5px;'></div>");
                }

                if (sectionContent != constants.EMPTY_VALUE) {

                    var id = $this.prev().attr(constantAttribute.ID);
                    id = $.trim(id);

                    if (id == constantID.DIV_SHIPPING_CALCULATOR) {
                        $this.prepend(shippingSectionsHtml);
                    }

                    counter++;
                    $this.attr(constantAttribute.DATA_INDEX, counter);
                }

            });

        }

        function prepareGiftCardsResultContainer() {

            $(constants.CLASS_SELECTOR + constantClassName.DIV_INLINE_SECTIONS).each(function () {

                var $this = $(this);
                var id = $this.attr(constantAttribute.ID);

                if (id == constantID.DIV_GIFT_CODE_WRAPPER) {
                    var nextInlineSectionId = $this.next().attr(constantAttribute.ID);
                    var isClearDiv = $this.next().hasClass(constantClassName.DIV_IS_CLEAR);

                    if (isClearDiv) {
                        nextInlineSectionId = $this.next().next().attr(constantAttribute.ID);
                    }
                
                    if (typeof (nextInlineSectionId) != constants.UNDEFINED) {

                        var $o = $(constants.ID_SELECTOR + nextInlineSectionId);
                        var dataIndex = $o.attr(constantAttribute.DATA_INDEX);
                      
                        if (dataIndex % 2 == 1) {
                            var nextId = $o.next().attr(constantAttribute.ID);

                            if (typeof (nextId) != constants.UNDEFINED) {
                                $(constants.ID_SELECTOR + nextId).prepend(giftCardsSectionsHtml);
                            } else {
                                $(constantID.DIV_INLINE_SECTIONS_WRAPPER).append("<div class='div-inline-sections'>" + giftCardsSectionsHtml + "</div>");
                            }
                        } else {
                            $o.prepend(giftCardsSectionsHtml);
                        }

                    } else {

                        var prevId = $this.prev().attr(constantAttribute.ID);
                        prevId = $.trim(prevId);

                        if (prevId == constantID.DIV_SHIPPING_CALCULATOR || prevId == constantID.DIV_COUPON_WRAPPER) {
                            $(constants.ID_SELECTOR + prevId).prepend(giftCardsSectionsHtml);
                        }

                        if (prevId == constants.EMPTY_VALUE || typeof (prevId) == constants.UNDEFINED) {
                            $(constantID.DIV_INLINE_SECTIONS_WRAPPER).append("<div class='div-inline-sections'>" + giftCardsSectionsHtml + "</div>");
                        }
                    } 
                }
            });

        }

        function scanForStandAloneSections() {
            var $divInlineSectionsWrapper = $(constantID.DIV_INLINE_SECTIONS_WRAPPER);
            var $divInlineSections = $(constants.CLASS_SELECTOR + constantClassName.DIV_INLINE_SECTIONS);

            if ($divInlineSections.length == 1) {
                $divInlineSectionsWrapper.children(constantElement.DIV).last().css(constantAttribute.WIDTH, "100%");
                var id = $divInlineSections.attr(constantAttribute.ID);
                id = $.trim(id);

                if (id == constantID.DIV_SHIPPING_CALCULATOR) {
                    $(constants.ID_SELECTOR + id).attr(constantAttribute.WIDGET_ALONE, "true");
                    $divInlineSectionsWrapper.append("<div class='div-inline-sections'>" + shippingSectionsHtml + "</div>");
                }

                if (id == constantID.DIV_GIFT_CODE_WRAPPER) {
                    $(constants.ID_SELECTOR + id).attr(constantAttribute.WIDGET_ALONE, "true");
                    $divInlineSectionsWrapper.append("<div class='div-inline-sections'>" + giftCardsSectionsHtml + "</div>");
                } 
            }


            if ($divInlineSections.length == 5) {
                var id = $divInlineSections.last().attr(constantAttribute.ID);
                id = $.trim(id);
                if (id == constantID.DIV_ORDER_NOTES) {
                    $(constants.ID_SELECTOR + id).attr(constantAttribute.STYLE, "width:100% !important;");
                }
            }

        }

        $(document).ready(function () {
            scanShoppingCartInlineSections();
            prepareGiftCardsResultContainer();
            scanForStandAloneSections();
        });

    </script>
    </asp:Panel>
    </form>
</body>
</html>
