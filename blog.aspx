<%@ Page Language="C#" AutoEventWireup="true" CodeFile="blog.aspx.cs" Inherits="blog" %>

<html>
<head>
</head>
<body>
<form runat="server" id="formblog">
    <div class="row">
        <div class="col-md-8">
            <asp:Literal ID="BlogPosts" runat="server"></asp:Literal>  
            <asp:Literal ID="BlogPostDetail" runat="server"></asp:Literal>
        </div>
        <div class="col-md-4">
            <asp:Literal ID="BlogCategories" runat="server"></asp:Literal>
        </div>
    </div>
    
</form>
</body>
</html>

