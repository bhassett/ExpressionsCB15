<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProfileControl.ascx.cs" Inherits="UserControls_ProfileControl" %>

<div id="profile-section-wrapper">
  <% if (!IsUseFullnameTextbox)
        { %>
  <div class="form-controls-place-holder ">
    <% if (IsShowSalutation)
               { %>
    <div class="form-group" id="salutation-place-holder">
      <asp:DropDownList ID="drpLstSalutation"  class="form-control light-style-input" runat="server"></asp:DropDownList>
    </div>
    <% } %>
    <div class="form-group">
      <label id="lblFirstName" class="form-field-label">
        <asp:Literal ID="litFirstName" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtFirstName"  CssClass="form-control form-control light-style-input" runat="server" MaxLength="50"></asp:TextBox>
    </div>
    <div class="form-group">
      <label id="lblLastName" class="form-field-label">
        <asp:Literal ID="litLastName" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtLastName" CssClass="form-control form-control light-style-input" runat="server" MaxLength="50"></asp:TextBox>
    </div>
  </div>
  <div class="clear-both height-5"></div>
  <div class="form-controls-place-holder">
    <div class="form-group">
      <label  id="lblContactNumber" class="form-field-label">
        <asp:Literal ID="litContactNumber" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtContactNumber" CssClass="form-control form-control light-style-input" Maxlength="50" runat="server"></asp:TextBox>
    </div>
    <div class="form-group" data-accountType="<%=AccountType%>">
      <label  id="lblMobile" class="form-field-label">
        <asp:Literal ID="litMobile" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtMobile" CssClass="form-control light-style-input" Maxlength="50" runat="server"></asp:TextBox>
      <label id="lblAnonymousEmail" class="form-field-label">
        <asp:Literal ID="litAnonymousEmail" runat="server" Visible="false"></asp:Literal>
      </label>
      <asp:TextBox ID="txtAnonymousEmail" CssClass="form-control light-style-input" MaxLength="50" runat="server"  Visible="false"></asp:TextBox>
    </div>
  </div>
  <% } else { %>
  <div class="form-controls-place-holder">
    <div class="form-group">
      <label id="lblShippingContactName" class="form-field-label">
        <asp:Literal ID="litShippingContactName" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtShippingContactName" CssClass="form-control light-style-input" runat="server" MaxLength="100"></asp:TextBox>
    </div>
    <div class="form-group">
      <label  id="lblShippingContactNumber" class="form-field-label">
        <asp:Literal ID="litShippingContactNumber" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtShippingContactNumber" MaxLength="50" CssClass="form-control light-style-input" runat="server"></asp:TextBox>
    </div>
  </div>
  <div class="clear-both height-5"></div>
  <div class="form-controls-place-holder">
    <div class="form-group" id="spanShippingSectionEmail" data-accountType="<%=AccountType%>">
      <label  id="lblShippingEmail" class="form-field-label">
        <asp:Literal ID="litShippingEmail" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtShippingEmail" CssClass="form-control light-style-input" runat="server" Maxlength="50"></asp:TextBox>
    </div>
  </div>
  <% } %>
  <div class="clear-both height-5 accounts-clear"></div>
  <% if (!IsHideAccountNameArea && !IsExcludeAccountName) { %>
  <div class="clear-both height-12"></div>
  <div class="clear-both height-12 accounts-clear"></div>
  <div id="account-name-wrapper" class="form-group form-controls-place-holder" >
    <label class="custom-font-style" id="enter-account-name-place-holder">
      <asp:Literal ID="litAccountName" runat="server"></asp:Literal>
    </label>
    <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control light-style-input" MaxLength="100"></asp:TextBox>
    <br />
    <label id="lblAccountName">
      <asp:Literal ID="litCompanyName" runat="server"></asp:Literal>
    </label>
  </div>
  <div class="clear-both height-5 accounts-clear"></div>
  <% }  %>
  <div class="form-group form-controls-place-holder">
    <label class="custom-font-style" id="spanEmailAddress">
      <asp:Literal ID="litEmailCaption" runat="server"></asp:Literal>
    </label>
    <span class="" data-accountType="<%=AccountType%>">
    <label  id="lblEmail" class="form-field-label" runat="server" visible="false">
      <asp:Literal ID="litEmail" runat="server" Visible="true"></asp:Literal>
    </label>
    <asp:TextBox ID="txtEmail" CssClass="form-control light-style-input" runat="server"  MaxLength="50"></asp:TextBox>
    </span> </div>
  <div class="clear-both height-5 accounts-clear"></div>
  <% if (IsShowEditPasswordArea && !IsExcludeAccountName) {%>
  <div class="clear-both height-12 accounts-clear"></div>
  <div class="clear-both height-5 accounts-clear"></div>
  <div class="form-controls-place-holder">
    <div class="form-group label-outside">
      <input type="checkbox" id="edit-password"/>
      <span class="checkbox-captions custom-font-style">
      <asp:Literal ID="litEditPassword" runat="server"></asp:Literal>
      </span> </div>
  </div>
  <div class="clear-both height-12 accounts-clear"></div>
  <div id="Div1" class="form-controls-place-holder">
    <div class="form-group custom-font-style" id="old-password-label-place-holder">
      <asp:Literal ID="litOldPassword" runat="server"></asp:Literal>
    </div>
    <div class="form-group">
      <label id="old-password-input-label" class="form-field-label custom-font-style">
        <asp:Literal ID="litCurrentPassword" runat="server"></asp:Literal>
      </label>
      <input id="old-password-input" MaxLength="50" class="form-control light-style-input " autocomplete="off" type="password" char="." />
    </div>
  </div>
  <div class="clear-both height-5 accounts-clear"></div>
  <% } %>
  <% if(!IsExcludeAccountName){ %>
  <div id="passwords-wrapper" class="form-controls-place-holder">
    <% if (!IsHideAccountNameArea) { %>
    <%--<div class="form-controls-span custom-font-style" id="password-caption">--%>
    <asp:Literal ID="litAccountSecurity" runat="server" Visible="false"></asp:Literal>
    <%--</div>--%>
    <% } else {  %>
    <%--<div class="form-controls-span custom-font-style" id="new-password-caption">--%>
    <asp:Literal ID="litNewPassword" runat="server"  Visible="false"></asp:Literal>
    <%--</div>--%>
    <% } %>
    <div class="form-group">
      <label id="lblPassword" class="form-field-label custom-font-style">
        <asp:Literal ID="litPassword" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtPassword" MaxLength="50" class="form-control light-style-input" TextMode="Password"  runat="server" AutoCompleteType="Disabled" autocomplete="off"></asp:TextBox>
    </div>
    <div class="form-group">
      <label id="lblConfirmPassword" class="form-field-label custom-font-style">
        <asp:Literal ID="litConfirmPassword" runat="server"></asp:Literal>
      </label>
      <asp:TextBox ID="txtConfirmPassword" MaxLength="50" class="form-control light-style-input" TextMode="Password" runat="server" AutoCompleteType="Disabled"  autocomplete="off"></asp:TextBox>
    </div>
  </div>
  <div class="clear-both height-5"></div>
  <% } %>
</div>
<script>
    $(document).ready(function () {

        var salutation = $("#ProfileControl_drpLstSalutation").val();
        if (typeof (salutation) == "undefined") {
            $("#ProfileControl_txtFirstName").addClass("new-first-name-width");
            $("#ProfileControl_txtLastName").addClass("new-last-name-width");
        } else {
            $("#ProfileControl_txtFirstName").removeClass("new-first-name-width");
            $("#ProfileControl_txtLastName").removeClass("new-last-name-width");
        }

    });
</script>