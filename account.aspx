<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.account" CodeFile="account.aspx.cs" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="CreditMemoViewList" Src="~/UserControls/CreditMemoViewList.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head runat="server">
    <title></title>
    <script type="text/javascript" src="jscripts/minified/customer.js"></script>
    <script type="text/javascript">
        var orderHistoryPluginStringKeys = new Object();
        orderHistoryPluginStringKeys.displayText = "account.aspx.43";
        orderHistoryPluginStringKeys.ofText = "account.aspx.29";
        orderHistoryPluginStringKeys.onText = "account.aspx.19";
        orderHistoryPluginStringKeys.orderDate = "account.aspx.12";
        orderHistoryPluginStringKeys.orderNotes = "account.aspx.16";
        orderHistoryPluginStringKeys.orderNumber = "account.aspx.11";
        orderHistoryPluginStringKeys.orderTotal = "account.aspx.15";
        orderHistoryPluginStringKeys.paymentMethod = "account.aspx.17";
        orderHistoryPluginStringKeys.paymentStatus = "account.aspx.13";
        orderHistoryPluginStringKeys.reorder = "account.aspx.22";
        orderHistoryPluginStringKeys.reorderPrompt = "account.aspx.26";
        orderHistoryPluginStringKeys.resetText = "account.aspx.44";
        orderHistoryPluginStringKeys.shippedText = "account.aspx.18";
        orderHistoryPluginStringKeys.shippingStatus = "account.aspx.14";
        orderHistoryPluginStringKeys.trackingNumber = "account.aspx.32";
        orderHistoryPluginStringKeys.viewing = "account.aspx.28";


        var openInvoicesPluginStringKeys = new Object();
        openInvoicesPluginStringKeys.displayText = "account.aspx.43";
        openInvoicesPluginStringKeys.ofText = "account.aspx.29";
        openInvoicesPluginStringKeys.onText = "account.aspx.19";
        openInvoicesPluginStringKeys.resetText = "account.aspx.44";
        openInvoicesPluginStringKeys.invoiceDate = "account.aspx.72";
        openInvoicesPluginStringKeys.invoiceCode = "account.aspx.73";
        openInvoicesPluginStringKeys.invoiceDueDate = "account.aspx.74";
        openInvoicesPluginStringKeys.invoiceDueTotal = "account.aspx.75";
        openInvoicesPluginStringKeys.invoicePayments = "account.aspx.76";
        openInvoicesPluginStringKeys.invoiceBalance = "account.aspx.77";
        openInvoicesPluginStringKeys.actionText = "account.aspx.78";
        openInvoicesPluginStringKeys.payonlineButtonText = "account.aspx.81";
    </script>

    <script type="text/javascript" src="components/order-history/setup.js"></script>
    <script type="text/javascript" src="components/open-invoices/setup.js"></script>
    <style>
        .section-header-top{margin-top:0 !important;}
        #pnlLoyaltyPoints, #pnlGiftCodes, #pnlPageContentWrapper,  #pnlLoyaltyPoints .content, #pnlGiftCodes .content{border:none;box-shadow:none;}
        #pnlPageContentWrapper{padding:0!important;}
    </style>
</head>
<body>
    <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="111" Bottom="90" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
    </asp:Panel>
    
<div class="row">
<div class="small-12 columns">
    
    <asp:Label ID="unknownerrormsg" runat="server" style="color:#FF0000;"></asp:Label>
    <asp:Label ID="ErrorMsgLabel" runat="server" style="color:#FF0000;"></asp:Label>
    
</div>
</div>
    
<div class="row">
<div class="small-12 columns">
    <asp:Panel ID="pnlAccountUpdated" runat="server" HorizontalAlign="left">
        <asp:Label ID="lblAcctUpdateMsg" runat="server" style="font-weight:bold;color:#FF0000;"></asp:Label>
        <br/><br/>
    </asp:Panel>

</div>
</div>
    
<div class="row">
<div class="small-12 columns">
    
    <div id="profile-error-place-holder" class="error float-left display-none"></div> 
    <div class="clear-both height-5"></div>

    <asp:Panel ID="pnlNotCheckOutButtons" runat="server" HorizontalAlign="left">
        <asp:Image ID="redarrow1" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="accountaspx4" NavigateUrl="#OrderHistory" Text="(!account.aspx.3!)"></asp:HyperLink></b>
        <div class="clear-both height-5"></div>
        <asp:Panel ID="pnlShowWishButton" runat="server"><asp:Image ID="redarrow2" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="ShowWishListButton" NavigateUrl="~/wishlist.aspx" Text="(!account.aspx.23!)"></asp:HyperLink></b></asp:Panel>
    </asp:Panel>
     <div class="clear-both height-12"></div>

    <ise:Topic runat="server" ID="HeaderMsg" TopicName="AccountPageHeader" />
    
    <div class="error"><cc1:InputValidatorSummary ID="errorSummary" runat="server" Register="false"></cc1:InputValidatorSummary></div>  

</div>
</div>   

    <form id="AccountForm" runat="server">
     <asp:Panel ID="pnlPageContentWrapper" runat="server">
         <div class="row">
         <div class="small-12 columns"> 
          <div class="clear-both height-12"></div>
           
           <!-- profile section starts here !-->
            <%-- div section for address book starts here --%>

        <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="Literal2" runat="server">(!account.aspx.62!)</asp:Literal></div>
      
        <div class="section-content-wrapper">
                <div>

                    <div class="clear-both"></div>
                    <div id="profile-info-wrapper" style="padding:12px;">
                                            
                        <d  iv id="profile-account-info-place-holder" class="float-left">
                            <span class="form-section">
                                <asp:Literal ID="litProfileInfo" runat="server">(!account.aspx.63!)</asp:Literal>
                            </span>
        
                            <div class="clear-both height-12 profile-section-clears"></div>
                            <uc:ProfileControl id="ProfileControl" runat="server" />
                            <div class="clear-both height-12 profile-section-clears"></div>

                            <div class="clear-both height-5"></div>
                                                
                            <div id="account-section-wrapper">
                                <span class="form-section custom-font-style">
                                    <asp:Literal ID="litAdditionalInfo" runat="server">(!account.aspx.64!)</asp:Literal>
                                </span>
                                <div class="clear-both height-12"></div>

                                <div class="form-controls-place-holder">
                                    <span class="form-controls-span">
                                        <asp:CheckBox ID="chkIsOkToEmail" runat="server"/>
                                        <span class="checkbox-captions custom-font-style">
                                           <asp:Literal ID="Literal3" runat="server">(!account.aspx.65!)</asp:Literal>
                                       </span>
                                    </span>
                                </div>

                                <div class="clear-both height-5"></div>

                                <div class="form-controls-place-holder">
                                    <span class="form-controls-span label-outside" id="age-13-place-holder">
                                         <asp:CheckBox ID="chkIsOver13Checked" runat="server"/>
                                         <span class="checkbox-captions custom-font-style">
                                             <asp:Literal ID="litOver13" runat="server">(!account.aspx.66!)</asp:Literal>
                                        </span>
                                    </span>
                                </div>
                            </div>

                            <div class="clear-both height-5"></div>
             
              <!-- Captcha Section Starts Here -->
                <div class="form-controls-place-holder captcha-section">
                    <span class="form-controls-span custom-font-style" id="captcha-label" style="padding-right:17px !important;">
                        <asp:Literal ID="LtrEnterSecurityCodeBelow_Caption" runat="server">(!customersupport.aspx.12!)</asp:Literal>:
                    </span>

                    <span class="form-controls-span">
                       <label id="lblCaptcha" class="form-field-label">
                            <asp:Literal ID="litCaptcha" runat="server">(!customersupport.aspx.13!)</asp:Literal>
                        </label>
                        <input id="txtCaptcha" class="light-style-input" type="text" /> 
                    </span>
                </div>

                <div class="clear-both height-5  captcha-section"></div>

                <div class="form-controls-place-holder  captcha-section">

                   <div id="account-captcha-wrapper" class="float-right">
                        <div id="captcha-image">
                            <img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/>
                         </div>
                         <div id="captcha-refresh">
                            <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a>
                         </div>
                    </div>

                </div>

               <div class="clear-both height-5  captcha-section"></div>
               <!-- Captcha Section Ends Here -->
                        </div>
                        <div id="divProfileHelpfulTips" class="float-left">
                            <ise:Topic ID="ProfileHelpfulTips" runat="server" TopicName="ProfileHelpfulTips" />
                        </div>
                    </div>
                </div>
                    
            <div class="clear-both"></div>

            <div id="profile-info-button-place-holder">
               <div id="save-profile-button">
                     <div id="save-profile-loader"></div>
                     <div id="save-profile-button-place-holder">
                         <input type="button" class="site-button content" id="update-profile" 
                         data-contentType="string resource"
                         data-contentKey="account.aspx.6"
                         data-contentValue="<%=AppLogic.GetString("account.aspx.6", true)%>"
                         value="<%=AppLogic.GetString("account.aspx.6", true)%>" />
                         <asp:Button ID="btnContinueToCheckOut" CssClass="site-button" Text="(!account.aspx.24!)" runat="server" CausesValidation="false" OnClick="btnContinueToCheckOut_Click" />
                     </div>
               </div>
            </div>
            <div class="clear-both"></div>
          </div>
            </div>
                
                </div>
             </div>
       </asp:Panel>
        
       <!-- profile section ends here !-->

    <div class="row">
<div class="small-12 columns"> 
       <div class="clear-both height-5"></div>

       <%-- div section for address book starts here --%>

        <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="Literal7" runat="server">(!account.aspx.67!)</asp:Literal></div>
      
        <div class="section-content-wrapper">
 
                <div id="Div2">
                    <div id="Div3" style="padding:10px;">
                                            
                        <div style="float:left;width:50%">
                        <b><asp:Label ID="accountaspx30" runat="server" Text="(!account.aspx.8!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                                    <asp:HyperLink ID="lnkChangeBilling" runat="server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                                    <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>
                        </div>
                        <div style="float:left;width:50%;">
                        <asp:Panel ID="pnlShipping" runat="server">
                                        <b><asp:Label ID="accountaspx32" runat="server" Text="(!account.aspx.10!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                                        <asp:HyperLink ID="lnkChangeShipping" runat="Server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                                        <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                                    </asp:Panel>
                        </div>
                    </div>
                </div>  
                <b><asp:HyperLink ID="lnkAddShippingAddress" runat="server"></asp:HyperLink></b>
                <div class="clear-both height-12"></div>
          </div>
       </div>

       <div class="clear-both height-5"></div>

        <asp:ValidationSummary DisplayMode="List" ID="ValSummary" ShowMessageBox="false" runat="server" ShowSummary="true" ValidationGroup="account" ForeColor="red" Font-Bold="true"/>
       
       <%-- gift code section --%>
        <asp:Panel ID="pnlGiftCodes" runat="server">
            <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top"><%= AppLogic.GetString("account.aspx.45") %></div>
                <div class="section-content-wrapper">
                    <div class="content">
                        <a href="javascript:void(0)" id="btnViewGiftCodes"><%= AppLogic.GetString("account.aspx.46") %></a>
                        <div id="giftCode" style="display:none;">
                            <span id="lblNoGiftCodesFound">
                                <%= AppLogic.GetString("account.aspx.47") %>
                            </span>
                            <table id="tblGiftCodes" class="giftcode-table">
                                <tr>
                                    <th><%= AppLogic.GetString("account.aspx.48") %></th>
                                    <th><%= AppLogic.GetString("account.aspx.49") %></th>
                                    <th><%= AppLogic.GetString("account.aspx.50") %></th>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <script>
                        $(document).ready(function () {
                            giftcodeTemplate();
                            giftcodeEvents();
                            giftcodeLoadData();
                        });
                        function giftcodeLoadData() {
                            var giftcodes = $.parseJSON('<%= GetGiftCodesJSON() %>');
                            $.each(giftcodes, function (key, code) {
                                var visible = true;
                                if (code.Type == "Gift Certificate" && code.CreditAvailable <= 0) { visible = false; }
                                if (visible) {
                                    $("#tblGiftCodes tr:last").after($.tmpl("giftcode-row", { Code: code.SerialCode, Balance: code.CreditAvailableFormatted, Type: code.Type }));
                                }
                            });
                        }
                        function giftcodeEvents() {
                            $("#btnViewGiftCodes").click(function () {
                                var giftcodesNotFoundLabel = $("#lblNoGiftCodesFound");
                                var giftcodesTable = $("#tblGiftCodes");
                                var giftcodesTableRow = $("#tblGiftCodes tr");
                                var giftcodesContainer = $("#giftCode");

                                if (giftcodesTableRow.length > 1) {
                                    giftcodesNotFoundLabel.hide();
                                    giftcodesTable.show();
                                }
                                else {
                                    giftcodesNotFoundLabel.show();
                                    giftcodesTable.hide();
                                }
                                giftcodesContainer.show();
                                $(this).hide();
                            });
                        }
                        function giftcodeTemplate() {
                            $.template("giftcode-row", "<tr>" +
                                                          "<td class='code'>${Code}</td>" +
                                                          "<td class='balance'>${Balance}</td>" +
                                                          "<td class='type'>${Type}</td>" +
                                                       "</tr>");
                        }
                    </script>
                </div>
            </div>
            <div class="clear-both height-5"></div>
        </asp:Panel>

        <%-- loyalty points section --%>
        <asp:Panel ID="pnlLoyaltyPoints" runat="server">
            <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top"><%= AppLogic.GetString("account.aspx.52") %></div>
                <div class="section-content-wrapper">
                    <div class="content" id="loyaltyPoints">
                        <table>
                            <tr>
                                <td class="caption"><%= AppLogic.GetString("account.aspx.53") %></td>
                                <td>
                                    <asp:Label ID="lblPoints" runat="server" CssClass="points"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="caption"><%= AppLogic.GetString("account.aspx.54") %></td>
                                <td>
                                    <asp:Label ID="lblMonetaryValue" runat="server" CssClass="value"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="clear-both height-5"></div>
        </asp:Panel>

        <!-- credit memos section -->
        <asp:Panel ID="pnlCreditMemos" runat="server">
             <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top"><%= AppLogic.GetString("account.aspx.82") %></div>
                <div class="section-content-wrapper">
                    <uc:CreditMemoViewList ID="creditMemoList" runat="server" />
                </div>
            </div>
            <div class="clear-both height-5"></div>
        </asp:Panel>

        <%-- div section for Open Invoices starts here --%>

        <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="Literal4" runat="server">(!account.aspx.79!)</asp:Literal></div>
      
        <div class="section-content-wrapper">
                <a name="OpenInvoices"></a>
                <div id="AccountOpenInvoices" style="border: none !important">
                    <div id="accountOpenInvoicesLink"  runat="server"><a id="lnkOpenInvoices" href="javascript:void(0);"><%=AppLogic.GetString("account.aspx.80", true)%></a></div>
                    <div id="pnlOpenInvoices"></div>
                </div>
            </div>
        </div>

       <div class="clear-both height-5"></div>

       <%-- div section for Order History starts here --%>

       <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="litOrderHistory" runat="server">(!account.aspx.68!)</asp:Literal></div>
      
            <div class="section-content-wrapper">
                <a name="OrderHistory"></a>
                <div id="AccountOrderHistory" style="border: none !important">
                    <div id="accountOrderHistoryLink" runat="server" ><a id="lnkOrderHistory" href="javascript:void(0)"><%=AppLogic.GetString("account.aspx.30", true)%></a></div>
                    <div id="pnlOrderHistory"></div>
                </div>
           </div>

        </div>
           
           </div>
           </div>

     <!-- do not remove --><input type="hidden" id="load-at-page" value="edit-profile" /><!-- do not remove -->

    </form>

</div>
</div>
    
</body>
</html>
