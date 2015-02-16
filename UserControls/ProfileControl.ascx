<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProfileControl.ascx.cs" Inherits="UserControls_ProfileControl" %>
<div id="profile-section-wrapper">
    
    <% if (!IsUseFullnameTextbox)
        { %>
        <div class="form-controls-place-holder ">
            
            <% if (IsShowSalutation)
               { %>
            <span class="form-controls-span" id="salutation-place-holder">
                <asp:DropDownList ID="drpLstSalutation"  class="light-style-input" runat="server"></asp:DropDownList>
            </span>
            <% } %>

            <span class="form-controls-span">
                <label id="lblFirstName" class="form-field-label">
                    <asp:Literal ID="litFirstName" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtFirstName"  CssClass="light-style-input" runat="server" MaxLength="50"></asp:TextBox>
            </span>
        
            <span class="form-controls-span">
                <label id="lblLastName" class="form-field-label">
                    <asp:Literal ID="litLastName" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtLastName" CssClass="light-style-input" runat="server" MaxLength="50"></asp:TextBox>
            </span>
        </div>
        <div class="clear-both height-5"></div>

        <div class="form-controls-place-holder">

            <span class="form-controls-span">
                <label  id="lblContactNumber" class="form-field-label">
                <asp:Literal ID="litContactNumber" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtContactNumber" CssClass="light-style-input" Maxlength="50" runat="server"></asp:TextBox>
            </span>

            <span class="form-controls-span" data-accountType="<%=AccountType%>">
                <label  id="lblMobile" class="form-field-label">
                    <asp:Literal ID="litMobile" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtMobile" CssClass="light-style-input" Maxlength="50" runat="server"></asp:TextBox>

                <label id="lblAnonymousEmail" class="form-field-label">
                    <asp:Literal ID="litAnonymousEmail" runat="server" Visible="false"></asp:Literal>
                </label>
                <asp:TextBox ID="txtAnonymousEmail" CssClass="light-style-input" MaxLength="50" runat="server"  Visible="false"></asp:TextBox>
            </span>


   
        </div>

    <% } else { %>

    <div class="form-controls-place-holder">
        <span class="form-controls-span">
            <label id="lblShippingContactName" class="form-field-label">
                <asp:Literal ID="litShippingContactName" runat="server"></asp:Literal>
            </label>
            <asp:TextBox ID="txtShippingContactName" CssClass="light-style-input" runat="server" MaxLength="100"></asp:TextBox>
        </span>

       <span class="form-controls-span">
                <label  id="lblShippingContactNumber" class="form-field-label">
                <asp:Literal ID="litShippingContactNumber" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtShippingContactNumber" MaxLength="50" CssClass="light-style-input" runat="server"></asp:TextBox>
       </span>


    </div>
    <div class="clear-both height-5"></div>

     <div class="form-controls-place-holder">
         <span class="form-controls-span" id="spanShippingSectionEmail" data-accountType="<%=AccountType%>">
            <label  id="lblShippingEmail" class="form-field-label">
                <asp:Literal ID="litShippingEmail" runat="server"></asp:Literal>
            </label>
            <asp:TextBox ID="txtShippingEmail" CssClass="light-style-input" runat="server" Maxlength="50"></asp:TextBox>
        </span>
    </div>
                 
    <% } %>

     <div class="clear-both height-5 accounts-clear"></div>


    <% if (!IsHideAccountNameArea && !IsExcludeAccountName) { %>

    <div class="clear-both height-12"></div>
    <div class="clear-both height-12 accounts-clear"></div>

    <div id ="account-name-wrapper" class="form-controls-place-holder">
             <span class="form-controls-span custom-font-style" id="enter-account-name-place-holder">
                <asp:Literal ID="litAccountName" runat="server"></asp:Literal>
            </span>
            <span class="form-controls-span">
                <label id="lblAccountName" class="form-field-label">
                    <asp:Literal ID="litCompanyName" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtAccountName" runat="server" class="light-style-input" MaxLength="100"></asp:TextBox>
            </span>
    </div>
    <div class="clear-both height-5 accounts-clear"></div>

    <% }  %>
    
    
    <div class="form-controls-place-holder">
        
        <span class="form-controls-span custom-font-style" id="spanEmailAddress">
               <asp:Literal ID="litEmailCaption" runat="server"></asp:Literal>
         </span>
         <span class="form-controls-span" data-accountType="<%=AccountType%>">
                <label  id="lblEmail" class="form-field-label">
                    <asp:Literal ID="litEmail" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtEmail" CssClass="light-style-input" runat="server"  MaxLength="50"></asp:TextBox>
          </span>
    </div>

    <div class="clear-both height-5 accounts-clear"></div>

    
    <% if (IsShowEditPasswordArea && !IsExcludeAccountName) {%>

    <div class="clear-both height-12 accounts-clear"></div>
    <div class="clear-both height-5 accounts-clear"></div>

    <div class="form-controls-place-holder">
        <span class="form-controls-span label-outside">
            <input type="checkbox" id="edit-password"/> <span class="checkbox-captions custom-font-style">
            
            <asp:Literal ID="litEditPassword" runat="server"></asp:Literal>
            
            </span>
        </span>
    </div>
    <div class="clear-both height-12 accounts-clear"></div>
    <div id="Div1" class="form-controls-place-holder">
            <span class="form-controls-span custom-font-style" id="old-password-label-place-holder">
                 <asp:Literal ID="litOldPassword" runat="server"></asp:Literal>
            </span>
            <span class="form-controls-span">
                <label id="old-password-input-label" class="form-field-label custom-font-style">
                    <asp:Literal ID="litCurrentPassword" runat="server"></asp:Literal>
                </label>
                <input id="old-password-input" MaxLength="50" class="light-style-input " autocomplete="off" type="password" char="." /> 
            </span>
    </div>
     <div class="clear-both height-5 accounts-clear"></div>

    
    <% } %>

    <% if(!IsExcludeAccountName){ %>

    <div id="passwords-wrapper" class="form-controls-place-holder">
           <% if (!IsHideAccountNameArea) { %>
             <span class="form-controls-span custom-font-style" id="password-caption">
               <asp:Literal ID="litAccountSecurity" runat="server"></asp:Literal>
            </span>
           <% } else {  %>
             <span class="form-controls-span custom-font-style" id="new-password-caption">
                <asp:Literal ID="litNewPassword" runat="server"></asp:Literal>
            </span>
           <% } %>


            <span class="form-controls-span">
                <label id="lblPassword" class="form-field-label custom-font-style">
                    <asp:Literal ID="litPassword" runat="server"></asp:Literal>
                </label>
                <asp:TextBox ID="txtPassword" MaxLength="50" class="light-style-input" TextMode="Password"  runat="server" AutoCompleteType="Disabled" autocomplete="off"></asp:TextBox>
            </span>
    
            <span class="form-controls-span">
                <label id="lblConfirmPassword" class="form-field-label custom-font-style">
                    <asp:Literal ID="litConfirmPassword" runat="server"></asp:Literal>
                </label>
               <asp:TextBox ID="txtConfirmPassword" MaxLength="50" class="light-style-input" TextMode="Password" runat="server" AutoCompleteType="Disabled"  autocomplete="off"></asp:TextBox>
            </span>
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