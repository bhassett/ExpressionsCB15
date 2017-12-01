<%@ Page Language="C#" CodeFile="giftregistry.aspx.cs" Inherits="InterpriseSuiteEcommerce.giftregistry" %>

<%@ Register Src="UserControls/GiftRegistry/GiftRegistryList.ascx" TagName="GiftRegistryList"
    TagPrefix="ctrl" %>
<html>
<head>
    <title></title>
</head>
<body>
    <asp:Panel ID="pnlMain" runat="server" CssClass="pnlmain">

        <div class="sections-place-holder row">
         <div class="col-xs-12">
            <div class="entity-header"><h1><asp:Literal runat="server" ID="litRegistryHeader"></asp:Literal></h1></div>
            <div>
                <ctrl:GiftRegistryList ID="GiftRegistryList1" runat="server" />
            </div>
        </div>
        </div>
        <div class="row">
        <div class="col-sm-6 col-sm-offset-6 text-right">
            <!-- Create Registry Button -->
            <a href="editgiftregistry.aspx" class="btn btn-primary content" >
                    <asp:Literal runat="server" ID="litCreateRegistry"></asp:Literal>
            </a>
            &nbsp;
            <!-- Find Registry Button -->
            <a href="findgiftregistry.aspx" class="btn btn-default content" >
                    <asp:Literal runat="server" ID="litFindRegistry"></asp:Literal>
            </a>
        </div>
        
    </asp:Panel>
</body>
</html>
