﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MobileShippingMethodControl.ascx.cs" Inherits="UserControls_MobileShippingMethodControl" %>

<asp:Panel runat="server" ID="content" HorizontalAlign="Left"></asp:Panel>            

<div id="pnlHiddenField">
    <asp:HiddenField ID="shippingMethod" runat="server" />
    <asp:HiddenField ID="freight" runat="server" />
    <asp:HiddenField ID="freightCalculation" runat="server" />
    <asp:HiddenField ID="realTimeRateGUID" runat="server" />
    <asp:HiddenField ID="hdenFreightChargeType" runat="server" />
</div>

<asp:Button id="btnShowAllRates" runat="server" CssClass="site-button content" OnClientClick="return false;" Visible="false"/>
