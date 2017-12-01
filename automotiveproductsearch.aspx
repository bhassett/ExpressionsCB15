<%@ Page language="C#" CodeFile="automotiveproductsearch.aspx.cs" Inherits="automotiveproductsearch" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head runat="server">
    <title>Untitled Page</title>
     <style>
        #itempopup-loader{
            display:none;
            top: 50%; left: 50%; width: 100px; position: absolute; display: none; z-index: 999999; padding: 0px; font-size: 12pt; color: #000; background-color: #fff; text-align: center; border-radius: 7px 7px 7px 7px; -webkit-border-radius: 7px 7px 7px 7px; -moz-border-radius: 7px 7px 7px 7px; padding: 10px; border: solid 1px #ccc; 
        }
        #itempopup-mask{
            display:none;
        }
        img.content {
            width : auto; 
        }
        
    </style>
</head>
<body>
    <div id="webcontent" runat="server">
        <br />
    </div>
    <div id="webContent2" runat="server"></div>
   
</body>


</html>

