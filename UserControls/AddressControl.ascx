<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressControl.ascx.cs" Inherits="UserControls_AddressControl" EnableViewState="true" %>
<% if (!IsHideStreetAddressInputBox)
 { %>

<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <label id="lblStreet" class="form-field-label" runat="server">
            <asp:Literal ID="litStreet" runat="server"></asp:Literal>
        </label>
        <asp:TextBox ID="txtStreet"  TextMode="multiline" runat="server" MaxLength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
</div>
<div class="clear-both height-5"></div>
       
<% } %>
                           
<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <asp:DropDownList ID="drpCountry" runat="server"></asp:DropDownList>
    </span>
</div>

<div class="clear-both height-5"></div>

<div class="<%=IdPrefix%>zip-city-other-place-holder">               
    <span class="form-controls-span">
        <label id="lblCity"  class="form-field-label" runat="server">
            <asp:Literal ID="litCity" runat="server"></asp:Literal>
        </label>
        <asp:TextBox ID="txtCity" runat="server" MaxLength = "50" class="light-style-input" type="text" ></asp:TextBox>
    </span>

    <span class="form-controls-span">
        <label  id="lblState" class="form-field-label" runat="server">
            <asp:Literal ID="litState" runat="server"></asp:Literal>
        </label>
        <asp:TextBox ID="txtState" runat="server" MaxLength = "50" class="light-style-input" type="text" ></asp:TextBox>
    </span>
</div>   

<div class="postal-place-holder">
    <span class="form-controls-span">
        <label id="lblPostal" class="form-field-label" runat="server" maxlength="10">
                <asp:Literal ID="litPostal" runat="server"></asp:Literal>
        </label>
        <asp:TextBox ID="txtPostal" runat="server" MaxLength = "30" class="light-style-input" type="text" ></asp:TextBox>
    </span>

    <span class="form-controls-span custom-font-style capitalize-text" id="<%=IdPrefix%>enter-postal-label-place-holder">
        <asp:Literal ID="litEnterPostal" runat="server"></asp:Literal>
    </span>
</div>

<asp:Panel ID="pnlCounty" runat="server" Visible="false">
<div class="clear-both height-5"></div>
 <div class="float-left">
    <span class="form-controls-span">
        <label id="lblCounty" class="form-field-label" runat="server" maxlength="10">
                <asp:Literal ID="litCounty" runat="server"></asp:Literal>
        </label>
        <asp:TextBox ID="txtCounty" runat="server" MaxLength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
 </div>
 </asp:Panel>

<% if (IsShowBusinessTypesSelector)
   { %>
<div class="clear-both height-5"></div>
<div class="form-controls-place-holder">
    <span class="form-controls-span" id="business-type-place-holder">
         <asp:DropDownList ID="drpBusinessType"  runat="server" CssClass="light-style-input"></asp:DropDownList>
    </span>
</div>
<div id="tax-number-place-holder" class="display-none">
    <div class="clear-both height-5"></div>
    <div class="form-controls-place-holder">
      <span class="form-controls-span">
            <label  id="lblTaxNumber" class="form-field-label" runat="server">
                <asp:Literal ID="litTaxtNumber" runat="server"></asp:Literal>
            </label>
            <asp:TextBox ID="txtTaxNumber" runat="server" MaxLength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
    </div>
</div>
<% } %>

<% if (IsShowResidenceTypesSelector)
{ %>
<div class="clear-both height-5"></div>
<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <asp:DropDownList ID="drpType" runat="server" CssClass="light-style-input"></asp:DropDownList>
    </span>
</div>
<% } %>

<div id="adjust-country-width-if-other-option-is-selected" class="display-none"><%=IsCountryWidthAdjustment%></div>