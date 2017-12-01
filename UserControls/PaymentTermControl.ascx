<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentTermControl.ascx.cs" Inherits="UserControls_PaymentTermControl" EnableViewState="true" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<asp:Panel runat="server" ID="pnlNoPayment" Visible="false">
    <span>
        <asp:Literal ID="litNoPaymentRequired" runat="server"></asp:Literal>
    </span>
</asp:Panel>

<div style="width: 100%">
    <asp:Panel ID="pnlPaymentTermOptions" runat="server">
       
            <asp:Repeater ID="rptPaymentOptions" runat="server">
                <ItemTemplate>
                    
                        <div class="payment-option">
                            <input id="<%# this.ClientID + "_" + this.ClientID + "_" + Container.DataItemAs<PaymentTermDTO>().Counter %>"
                                type="radio"
                                name="<%# this.ClientID %>$"
                                <%# Container.DataItemAs<PaymentTermDTO>().IsSelected ? " checked=\"checked\" " : "" %>
                                <%# CreatePaymentMethodAttribute(Container.DataItemAs<PaymentTermDTO>()) %>
                                <%# CreatePaymentTermAttribute(Container.DataItemAs<PaymentTermDTO>()) %> />
                       
                            
                                  <strong><%# Container.DataItemAs<PaymentTermDTO>().PaymentTermCode %></strong> - <%# Container.DataItemAs<PaymentTermDTO>().Description %>
                            
                          
                          
                            <span runat="server" id="collapseCreditCardImagesButton" visible="false">
                                <a role="button" data-toggle="collapse" href="#credit-card-images-container" aria-expanded="false">
                                    <i class="fa fa-question-circle"></i>
                                </a>
                                
                            </span>
                            <div class="collapse" id="credit-card-images-container">
                                <asp:Label runat="server" ID="litCreditCardImages" Visible="false" ></asp:Label>
                            </div>
                        </div>
                    
                         <div id="paypalRow" runat="server" visible="false" class="payment-option">

                            <input id="<%# this.ClientID + "_" + this.ClientID + "_PaypalOption" %>"
                                type="radio"
                                name="<%# this.ClientID %>$"
                                pm="<%# DomainConstants.PAYMENT_METHOD_PAYPALX %>"
                                pr="<%# DomainConstants.PAYMENT_METHOD_PAYPALX %>" />
                            <a href="#" onclick="javascript:window.open('https://www.paypal.com/us/cgi-bin/webscr?cmd=xpt/Marketing/popup/OLCWhatIsPayPal-outside','olcwhatispaypal','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes,resizable=yes,width=400,height=350');">
                                <%--<img style="vertical-align: middle" src="https://www.paypal.com/en_US/i/logo/PayPal_mark_37x23.gif" border="0" alt="Acceptance Mark">--%>

                            </a>
                             <strong> <%= AppLogic.GetString("pm.paypal.display", true)%></strong> - <%= AppLogic.GetString("pm.paypaldescription.display", true) %>
                        </div>
                    
              
                </ItemTemplate>
            </asp:Repeater>
       
    </asp:Panel>
    
    <asp:Panel ID="pnlCreditCardInfo" runat="server">
        <div id="<%= this.ClientID + "_"%>cardFormRow" style="display: none">
            <div class="payment-method-option">
                <div class="credit-card-payment-method-panel">
                 
                        <div class="form-group">
                            <label>
                                 <asp:Literal ID="litNameOnCard" runat="server"></asp:Literal>
                            </label>
                               <asp:TextBox ID="nameOnCard" class="form-control" type="text" size="50" maxlength="100" name="NameOnCard" runat="server"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                           <label>
                               <asp:Literal ID="litCardNumber" runat="server"></asp:Literal>
                           </label>
                            <asp:TextBox ID="cardNumber" autocomplete="off" class="form-control" type="text" size="50" maxlength="100" name="CardNumber" runat="server"></asp:TextBox>
                       </div>
                        
                        <asp:Panel ID="pnlStartDate" runat="server" Visible="false">
                            <div class="form-group">
                                <label>
                                     <asp:Literal ID="litCardStartDate" runat="server"></asp:Literal>
                                </label>
                                <div class="form-inline">
                                    <div class="form-group">
                                         <asp:DropDownList ID="startMonth"  class="light-style-input"  name="CardStartMonth" runat="server"></asp:DropDownList>
                                        / 
                                   
                                    </div>
                                    <div class="form-group">
                                         <asp:DropDownList ID="startYear"  class="light-style-input"  name="CardStartYear" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                          </asp:Panel>
                        
                        <div class="form-group">
                            <label>
                                  <asp:Literal ID="litExpirationDate" runat="server"></asp:Literal>
                            </label>
                            <div class="row">
                                <div class="col-lg-6">
                                    <asp:DropDownList ID="expirationMonth"  class="form-control"  name="ExpirationMonth" runat="server"></asp:DropDownList>
                                </div>
                                 <div class="col-lg-6">
                                     <asp:DropDownList ID="expirationYear"  class="form-control"  name="ExpirationYear" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        
                         <div class="form-group">
                             <label>
                                 <asp:Literal ID="litCVV" runat="server"></asp:Literal>
                                 <span>
                                       <asp:LinkButton ID="lnkWhatIsCvv" runat="server" href="javascript:void(0);">
                                 <i class="fa fa-question-circle"></i>
                             </asp:LinkButton>
                                 </span>
                             </label>
                             <asp:TextBox ID="cvv" class="form-control" type="text" autocomplete="off" size="5" maxlength="10" name="CVV" runat="server"></asp:TextBox>
                           
                         </div>
                        
                        <div class="form-group">
                            <label>
                                  <asp:Literal ID="litCardType" runat="server"></asp:Literal>
                            </label>
                            <asp:DropDownList ID="cardType" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                        
                        <asp:Panel ID="pnlCardIssueNumber" runat="server" Visible="false">
                            <div class="form-group">
                                <label>
                                      <asp:Literal ID="litCardIssueNumber" runat="server"></asp:Literal>
                                </label>
                                  <asp:TextBox ID="cardIssueNumber" class="light-style-input" type="text" autocomplete="off" size="2" maxlength="2" name="CardIssueNumber" runat="server"></asp:TextBox>
                                    &nbsp;
                                    <span>
                                        <asp:Literal ID="litCardIssueNumberInfo" runat="server"></asp:Literal>
                                    </span>
                            </div>
                            
                        </asp:Panel>

                        <asp:Panel ID="pnlTokenization" runat="server" Visible="false">
                            <div class="form-group">
                                <label>
                                    <asp:Literal ID="litSaveCardAs" runat="server"></asp:Literal>
                                </label>
                                  <asp:TextBox ID="cardDescription" runat="server"></asp:TextBox>
                            </div>
                           <div class="form-group">
                               <label>
                                   <asp:CheckBox ID="chkSaveCreditCardInfo" runat="server"/>
                               </label>
                                 <asp:Literal ID="litSaveThisCreditCardInfo" runat="server"></asp:Literal>
                           </div>
                           
                        </asp:Panel>
                     
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlPONumberInfo" runat="server">
        <div id="<%= this.ClientID + "_"%>poNumberRow" style="display:none;"">
        <div class="payment-method-option">
            <table class="purchase-order-payment-method-panel">
                <tr>
                    <td>
                        <span>
                            <asp:Literal ID="litPONumber" runat="server"></asp:Literal>
                        </span>
                    </td>
                    <td>
                        <asp:TextBox ID="poNumber" class="light-style-input" type="text" autocomplete="off" size="30" maxlength="30" name="ctl12" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </asp:Panel>
   
    <asp:Panel ID="pnlRedirectInfo" runat="server">
        <div id="<%= this.ClientID + "_"%>externalRow" style="display:none;">
            <div class="payment-method-option">
                <table class="redirect-payment-method-panel">
                <tr>
                    <td>
                        <span id="lblRedirectCaption" class="strong-font">
                            <asp:Literal ID="litExternal" runat="server"></asp:Literal>
                        </span>
                    </td>
                </tr>
                </table>
            </div>
        </div>
   </asp:Panel>

    <asp:Panel ID="pnlTerms" runat="server" Visible="false">
        <div>
            <div class="payment-method-option">
                <div>
                    <asp:CheckBox ID="termsAndConditionsChecked" runat="server" name="TermsAndConditionsChecked"/>
                    <asp:Literal ID="litTermsAndConditionsHTML" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </asp:Panel>
</div>

<div id="pnlHiddenField">
    <input type="hidden" id="paymentTerm" runat="server" />
    <input type="hidden" id="paymentMethod" runat="server" />
    <input type="hidden" id="hidNoPaymentRequired" runat="server" />
</div>