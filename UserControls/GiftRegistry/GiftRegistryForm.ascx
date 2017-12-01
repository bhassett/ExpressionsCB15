<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryForm.ascx.cs" Inherits="UserControls_GiftRegistry_GiftRegistryForm" EnableViewState="true" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<%@ Register Src="~/UserControls/DatePickerControl.ascx" TagName="DatePickerControl" TagPrefix="ctrl" %>
<%@ Register Src="~/UserControls/WebEditorControl.ascx" TagName="WebEditorControl" TagPrefix="ctrl" %>

<div class="gift-registry-form">
    <div class="gift-registry-form-wrapper">
        <div class="row">
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.3") %> </label>
    <ctrl:DatePickerControl ID="ctrlDatePickerStartDate" runat="server" CssClass="registry-input form-control" />
    <span class="required-marker">*</span> </div>
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.4") %> </label>
    <ctrl:DatePickerControl ID="ctrlDatePickerEndDate" runat="server" CssClass="registry-input form-control" />
    <span class="required-marker">*</span> </div>
  <div id="registry-sublabel" class="form-input"> <span class="registry-sublabel"> <%= AppLogic.GetString("editgiftregistry.aspx.5") %> </span> </div>
</div>
<!--row-->

<hr class="registry-divider" />
<div class="row">
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.6") %> </label>
    <asp:TextBox runat="server" ID="txtTitle" CssClass="registry-input form-control" ></asp:TextBox>
    <span class="required-marker">*</span> </div>
  <div id="webeditor-label" class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.7") %> </label>
    <ctrl:WebEditorControl ID="WebEditorControl" DisplayMode="Simple" runat="server" />
  </div>
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.8") %> </label>
    <asp:FileUpload runat="server" ID="txtFileUpload" CssClass="registry-input form-control"/>
  </div>
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.9") %> </label>
    <div class="radio text-left">
    <asp:RadioButton runat="server" ID="rdPublic" Text="Public" Checked="true" GroupName="btn" />
    </div>
    <div class="radio text-left">
    <asp:RadioButton runat="server" ID="rdPrivate" Text="Private" GroupName="btn" />
    </div>
  </div>
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.17")%> </label>
    <asp:TextBox runat="server" MaxLength="5" ID="txtGuestPassword" CssClass="registry-input form-control" Enabled="false"></asp:TextBox>
  </div>
</div>
<hr class="registry-divider" />
<div class="row">
  <div id="registry-sublabel" class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.15")%> <%= AppLogic.GetString("editgiftregistry.aspx.33")%> </label>
  </div>
  <div class="form-group">
    <label> <%= AppLogic.GetString("editgiftregistry.aspx.12") %> </label>
    <asp:Label runat="server" id="lblprimaryurl" style="float:left; line-height:26px"></asp:Label>
    <asp:TextBox runat="server" ID="txtCustomURL" CssClass="registry-input form-control" ></asp:TextBox>
    <span class="required-marker adjust-padding">*</span> </div>
  <a runat="server" target="_blank" id="lnkPreView" class="btn btn-default registry-form-link content" Visible="false" /> </div>

    </div>
</div>

<script type="text/javascript" >

    $(document).ready(function () {

        $('#<%=rdPublic.ClientID%>').click(function () {
            $('#<%=txtGuestPassword.ClientID%>').val('');
            $('#<%=txtGuestPassword.ClientID%>').attr("disabled", "disabled");
        })

        $('#<%=rdPrivate.ClientID%>').click(function () {
            $('#<%=txtGuestPassword.ClientID%>').removeAttr("disabled");
        })

    });
    
</script>
