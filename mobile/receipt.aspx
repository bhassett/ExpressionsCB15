<%@ Page language="c#" AutoEventWireup="true" Inherits="InterpriseSuiteEcommerce.receipt" CodeFile="receipt.aspx.cs"  ValidateRequest="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Receipt</title>
    <link href="mvwres:1-DevExpress.Web.ASPxEditors.Css.Default.css" rel="stylesheet" type="text/css" />
    <link href="mvwres:1-DevExpress.Web.ASPxEditors.Css.Default.css" rel="stylesheet" type="text/css" />
    <link href="mvwres:1-DevExpress.Web.ASPxEditors.Css.Default.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div  style="text-align:center;">
        <div>
            <dxwc:ReportToolbar ID="ReportToolbar1" runat="server" ReportViewer="<%# ViewerReport %>"
                    ShowDefaultButtons="False">
                    <Items>
                        <dxwc:ReportToolbarButton ImageUrl="~/images/icon.png" Name="Home" />
                        <dxwc:ReportToolbarSeparator />
                        <dxwc:ReportToolbarButton ItemKind="Search" ToolTip="Display the search window" />
                        <dxwc:ReportToolbarSeparator />
                        <dxwc:ReportToolbarButton ItemKind="PrintReport" ToolTip="Print the report" />
                        <dxwc:ReportToolbarButton ItemKind="PrintPage" ToolTip="Print the current page" />
                        <dxwc:ReportToolbarSeparator />
                        <dxwc:ReportToolbarButton Enabled="False" ItemKind="FirstPage" ToolTip="First Page" />
                        <dxwc:ReportToolbarButton Enabled="False" ItemKind="PreviousPage" ToolTip="Previous Page" />
                        <dxwc:ReportToolbarLabel Text="Page" />
                        <dxwc:ReportToolbarComboBox ItemKind="PageNumber">
                        </dxwc:ReportToolbarComboBox>
                        <dxwc:ReportToolbarLabel Text="of" />
                        <dxwc:ReportToolbarTextBox ItemKind="PageCount" Width="35px" />
                        <dxwc:ReportToolbarSeparator />
                        <dxwc:ReportToolbarButton ItemKind="NextPage" ToolTip="Next Page" />
                        <dxwc:ReportToolbarButton ItemKind="LastPage" ToolTip="Last Page" />
                        <dxwc:ReportToolbarSeparator />
                        <dxwc:ReportToolbarButton ItemKind="SaveToDisk" ToolTip="Export a report and save it to the disk" />
                        <dxwc:ReportToolbarButton ItemKind="SaveToWindow" ToolTip="Export a report and show it in a new window" />
                        <dxwc:ReportToolbarComboBox ItemKind="SaveFormat">
                            <elements>
                                <dxwc:ListElement Text="Pdf" Value="pdf" />
                                <dxwc:ListElement Text="Xls" Value="xls" />
                                <dxwc:ListElement Text="Rtf" Value="rtf" />
                                <dxwc:ListElement Text="Mht" Value="mht" />
                                <dxwc:ListElement Text="Text" Value="txt" />
                                <dxwc:ListElement Text="Csv" Value="csv" />
                                <dxwc:ListElement Text="Image" Value="png" />
                            </elements>
                        </dxwc:ReportToolbarComboBox>
                        
                    </Items>
                    <clientsideevents itemclick="function(s, e) {
	                        if(e.item.name =='Home' ) 
                                window.location.href = 'default.aspx';
                                                                }" />
                    <Styles>
                        <LabelStyle>
                            <Margins MarginLeft="3px" MarginRight="3px" />
                        </LabelStyle>
                    </Styles>
                </dxwc:ReportToolbar>
        </div>
        <div style="margin-left: -5px; text-align:center">
            <dxwc:ReportViewer ID="ViewerReport" runat="server">
            </dxwc:ReportViewer>
        </div>
    </div>    
    </form>
</body>
</html>
