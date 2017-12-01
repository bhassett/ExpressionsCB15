<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShipToAddressControl.ascx.cs" Inherits="UserControls_ShipToAddressControl" EnableViewState="true" ViewStateMode="Enabled" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="col-lg-8">
            <asp:DropDownList ID="drpShipToAddress" runat="server" CssClass="form-control" EnableViewState ="true" ClientIDMode="Static" AutoPostBack="True" ViewStateMode="Enabled" onchange="setSkipValidation()">
            </asp:DropDownList>
        </div>
        <div class="col-lg-4">
            <asp:Button ID="btnAddNew" runat="server" CssClass="btn btn-default btn-lg" Text=" Add New " OnClick="btnAddNew_Click" OnClientClick="setSkipValidation()" />
        </div>
    </div>
</div>
<div class="clear-both height-22"></div>
<script>
    function setSkipValidation() {
        var shippingMethodControl = ise.Controls.ShippingMethodController.getControl('<%=ShippingMethodControlId%>');
        shippingMethodControl.setSkipValidation();
    };
</script>


