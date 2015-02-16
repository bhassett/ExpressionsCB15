<%@ Page Language="C#" CodeFile="checkoutstore.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkoutstore" EnableEventValidation="false" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Domain.Infrastructure" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<%@ Register Src="UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Register Src="UserControls/ScriptControl.ascx" TagName="ScriptControl" TagPrefix="uc" %>

<body>

    <uc:ScriptControl ID="thirdPartyScriptManager" runat="server" ShowGoogleMapApi="true" />
    <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />

    <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
        </asp:ImageMap>
    </asp:Panel>

    <div class="height-12"></div>

    <form id="frmcheckoutstore" runat="server">
        <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
            <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top">
                    <asp:Literal ID="Literal1" runat="server">(!checkoutshipping.aspx.14!)</asp:Literal>
                </div>
                <div id="divCouponEntered" class="section-content-wrapper">
                    <asp:Literal ID="Literal2" runat="server">(!order.cs.12!)</asp:Literal>
                    <asp:Literal runat="server" ID="litCouponEntered"></asp:Literal>
                </div>
                <div class="clear-both height-5">
                </div>
            </div>
        </asp:Panel>
        <div class="clear-both height-12"></div>
        <div class="float-right">
            <asp:Button ID="btnContinueCheckOutTop" runat="server" Text="(!checkoutpayment.aspx.6!)" CausesValidation="true" CssClass="site-button content btn btn-info" />
        </div>
        <div class="clear-both height-12"></div>

        <div class="sections-place-holder no-padding">

            <div class="section-header section-header-top">
                <asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal>
            </div>

            <div id="order-summary-head-text" style="padding-left: 23px; padding-right: 12px">
                <div class="clear-both height-12"></div>
                <span class="strong-font custom-font-style">Order Summary </span>
                <span class="one-page-link-right normal-font-style float-right">
                    <a class="custom-font-style" href="shoppingcart.aspx">Edit Cart</a>
                </span>
            </div>

            <div class="section-content-wrapper">

                <div class="clear-both"></div>

                <table style="width: 100%">
                    <tbody>
                        <asp:Repeater runat="server" ID="rptCartItems">
                            <ItemTemplate>
                                <tr>
                                    <td class="section-grp-header"><%# AppLogic.GetString("checkoutstore.aspx.32")%> <%# Container.DataItemAs<CustomCartItem>().GroupID.ToString() %></td>
                                    <td class="section-grp-header"><%# AppLogic.GetString("shoppingcart.cs.28")%></td>
                                </tr>
                                <tr>
                                    <td class="section-grp-content">
                                        <span><%# Container.DataItemAs<CustomCartItem>().ItemDescription.ToHtmlDecode() %></span>
                                        <div style="margin-top: 5px;">
                                            <span><%# AppLogic.GetString("shoppingcart.cs.25")%> : </span>
                                            <span><%# (ServiceFactory.GetInstance<ILocalizationService>()
                                                                     .FormatDecimal(Container.DataItemAs<CustomCartItem>().Quantity, 0)) %>
                                            </span>
                                        </div>
                                    </td>
                                    <td class="section-grp-content">
                                        <asp:Literal runat="server" ID="litNoShippingMethodText" Visible="false" Text='<%# AppLogic.GetString("checkoutshippingmult.aspx.7") %>'></asp:Literal>
                                        <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

            </div>

        </div>

        <div class="clear-both height-12"></div>

        <div class="float-right">
            <asp:Button ID="btnContinueCheckOut" runat="server" Text="(!checkoutpayment.aspx.6!)" CausesValidation="true" CssClass="site-button content btn btn-info" />
        </div>

    </form>

</body>
