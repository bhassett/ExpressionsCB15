<%@ Page Language="C#" AutoEventWireup="true" CodeFile="payment.aspx.cs" Inherits="InterpriseSuiteEcommerce.payment"%>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="PaymentTermControl" Src="~/UserControls/PaymentTermControl.ascx" %>
<%@ Register TagPrefix="cbe" TagName="Topic" Src="TopicControl.ascx" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ OutputCache Location="None" NoStore="true" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error" runat="server" Register="False" />
    <form id="frmPayOnline" runat="server">
     <div id="content-wrapper">
        <div id="content-header">
            <div class="float-left" id="content-header-icon"></div>
            <div class="float-left" id="content-header-text" style="font-size:14px;font-weight:bold;"><asp:Literal ID="litPageTitle" runat="server"></asp:Literal></div>
            <div class="clear-both"></div>
            <div><asp:Literal ID="litIfYouHaveQuestions" runat="server">(!payment.aspx.16!)</asp:Literal>&nbsp;<a href="contactus.aspx"><asp:Literal ID="litContactUs" runat="server">(!payment.aspx.17!)</asp:Literal></a>.</div>
            <div class="clear-both height-20"></div>
        </div>
        <div id="payment-body">
            <div class="float-left" id="divPaymentBodyLeft">
                <div style="width:400px;">
                    <div class="float-left" id="divDateCaption"><asp:Literal ID="litDateCaption" runat="server"></asp:Literal></div>
                    <div class="float-left" id="divInvoiceDate"><asp:Literal ID="litInvoiceDate" runat="server"></asp:Literal></div>
                    <div class="clear-both"></div>
                    <div class="float-left" id="divAmountCaption">
                        <asp:Literal ID="litAmountCaption" runat="server"></asp:Literal>
                    </div>
                    <div class="float-left" id="divInvoiceAmount">
                        <asp:Literal ID="litInvoiceAmount" runat="server"></asp:Literal>
                    </div>

                    <asp:Panel ID="pnlBalanceDetails" runat="server">
                        <div class="clear-both height-20"></div>
                        <div class="float-left" id="divBalanceDue">
                            <asp:Literal ID="litBalanceDueCaption" runat="server">(!payment.aspx.7!)</asp:Literal>
                        </div>
                        <div class="float-left">
                            <span style="margin-right:29px;"><asp:Literal ID="litBalanceDue" runat="server"></asp:Literal></span>
                            <span>
                             <asp:Literal ID="litDueByCaption" runat="server">(!payment.aspx.8!)</asp:Literal>
                             <asp:Literal ID="litBalanceDueDate" runat="server"></asp:Literal></span>
                        </div> 
                    </asp:Panel>
        
                </div>

                 <div class="clear-both height-12 border-top-solid"></div>
                 <div style="font-size:14px;font-weight:bold;">
                    <asp:Literal ID="litSelectPaymentMethod" runat="server"></asp:Literal>
                 </div>
                 <asp:Panel ID="pnlPaymentTerm" runat="server" HorizontalAlign="Center">
                       <uc:PaymentTermControl ID="ctrlPaymentTerm" runat="server"></uc:PaymentTermControl>
                 </asp:Panel> 
                 
                 <div style="font-size:14px;font-weight: bold;margin: 5px;">
                    <asp:Literal ID="paypalStatus" runat="server" Visible="false">(!payment.aspx.83!)</asp:Literal>
                 </div>

                 <asp:Panel runat="server" ID ="pnlNoAvailablePaymentStatus" Visible="false">
                    <asp:Literal ID="litTransactionStatusMessage" runat="server"></asp:Literal> <a href="contactus.aspx"><asp:Literal ID="litContactUsLink" runat="server">(!payment.aspx.17!)</asp:Literal></a>.
                 </asp:Panel>
                
                 <asp:Panel ID="pnlPayOnlineButton" runat="server">
                 <div class="clear-both height-12 border-top-solid"></div>
                 <div class="float-right">
                    <div class="float-left" id="divEnterAmountCaption">
                        <asp:Literal ID="litEnterAmountCaption" runat="server">(!payment.aspx.10!)</asp:Literal>
                    </div>
                    <div class="float-left">
                        <asp:TextBox ID="txtAmount" runat="server"></asp:TextBox> 
                        <asp:Button ID="btnProcessPayment" runat="server" CssClass="btn btn-default content" Text="(!payment.aspx.18!)" data-contentKey="payment.aspx.18" data-contentType="string resource" data-contentValue="(!payment.aspx.18!)"/>
                    </div>
                    <div id="progress" class="display-none">
                        <div style='float:left;width:12px;'><i class="fa fa-spinner fa-spin"></i></div> 
                        <div id='loader-container'><asp:Literal ID="litProgessMessage" runat="server">(!payment.aspx.25!)</asp:Literal></div>
                    </div>
                 </div>
                 </asp:Panel>
            </div>

            <div class='float-left' id="divPaymentBodyRight"><cbe:Topic ID="PaymentHelpfulTipsTopic" runat="server" TopicName="PaymentHelpfulTipsTopic" /></div>

            <div class="clear-both height-20"></div>
             <asp:Literal ID="litViewReceipt" runat="server"></asp:Literal>
            <div class="clear-both height-5"></div>
            <asp:Literal ID="litReceipt" runat="server"></asp:Literal>
            <div><asp:Literal ID="litReceiptDescription" runat="server"></asp:Literal></div>
        </div>
        <div class='clear-both'></div>
     </div>
     <script type="text/javascript">
           $(document).ready(function () {

               showSavingProgress(false);

               $("#btnProcessPayment").click(function () {
                   if (ise.Pages.CheckOutPayment.paymentTermControl.validate()) {
                       showSavingProgress(true);
                   } else {
                       showSavingProgress(false);
                   }
               });

           });

           function showSavingProgress(show) {
               var displayNone = "display-none";
               var $divEnterAmountCaption = $("#divEnterAmountCaption");
               var $txtAmount = $("#txtAmount");
               var $btnProcessPayment = $("#btnProcessPayment");
               var $progress = $("#progress");

               if (show) {
                   $divEnterAmountCaption.addClass(displayNone);
                   $txtAmount.addClass(displayNone);
                   $btnProcessPayment.addClass(displayNone);
                   $progress.removeClass(displayNone);
               } else {
                   $divEnterAmountCaption.removeClass(displayNone);
                   $txtAmount.removeClass(displayNone);
                   $btnProcessPayment.removeClass(displayNone);
                   $progress.addClass(displayNone);
               }
           }
    </script>
    </form>
</body>
</html>
