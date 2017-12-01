<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutanon" CodeFile="checkoutanon.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head>
</head>
<body>
    <form runat="Server">
    <div align="center">

     <%--   <asp:Panel ID="HeaderPanel" runat="server" Width="90%">
            <div align="center" style="text-align: center">
                <asp:Panel ID="CheckoutPanel" runat="server">
                    <div id="CheckoutSequence" align="center">
                        <asp:ImageMap ID="CheckoutMap" runat="server">
                            <asp:RectangleHotSpot Bottom="90" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true"
                                Right="119" />
                        </asp:ImageMap><br />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>--%>

        <asp:Literal ID="CheckoutStepLiteral" runat="server"></asp:Literal>
        
        
        <asp:Panel ID="FormPanel" runat="server">
            <div id="checkoutanon-container">
                <div class="row">
                    <div class="col-lg-12 no-padding">
                        <div class="checkoutanon-container-well">
                            <div class="row">
                                <div class="col-lg-6">
                                    <div class="checkoutanon-container-registered panel panel-default">
                                        <div class="checkoutanon-container-header to-upper panel-heading">
                                            <%--Registered User Header--%>
                                             <asp:Label ID="Label6" runat="server" Text="(!checkoutanon.aspx.11!)" Font-Bold="true"></asp:Label><br />
                                        </div>
                                        <div class="panel-body text-left">
                                        <div class="checkoutanon-container-registered-details">
                                            <p>
                                                <%--Registered User Detail--%>
                                                <asp:Label ID="Label1" runat="server" Text="(!checkoutanon.aspx.3!)" />
                                            </p>
                                        </div>
                                        <br>

                                          <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="center">
                                              <div class="alert alert-danger" role="alert">
                                                  <strong>
                                                         <asp:Label ID="ErrorMsgLabel" CssClass=" " runat="server"></asp:Label>
                                                  </strong>
                                                  <asp:LinkButton ID="lnkContactUs" runat="server" PostBackUrl="contactus.aspx" Visible="true" />
                                              </div>
                                          </asp:Panel>

                                        <div class="form-group">
                                            <label id="txtCheckoutAnonEmailLabel" class="">
                                                <%--Email Label--%>
                                                <asp:Label ID="Label2" runat="server" Text="(!checkoutanon.aspx.4!)"  />
                                            </label>
                                            <%--Email Input--%>
                                           <asp:TextBox ID="txtCheckoutAnonEmail" runat="server" ValidationGroup="Group1" MaxLength="100" CausesValidation="True" AutoCompleteType="Email" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtCheckoutAnonEmail" runat="server" ValidationGroup="Group1" ErrorMessage="!!" Display="Dynamic" Font-Bold="True" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group" style="margin-bottom: 0px;" >
                                            <label id="txtCheckoutAnonPasswordLabel" class="">
                                                <%--Password Label--%>
                                                 <asp:Label ID="Label3" runat="server" Text="(!checkoutanon.aspx.5!)" />
                                            </label>
                                        </div>
                                            <div class="form-inline">
                                            <div class="form-group">
                                                    <%--Password Input--%>
                                                    <asp:TextBox ID="txtCheckoutAnonPassword" runat="server" ValidationGroup="Group1" MaxLength="50" CausesValidation="True" TextMode="Password"  AutoCompleteType="Disabled" autocomplete="off" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtCheckoutAnonPassword" runat="server" ErrorMessage="!!" ValidationGroup="Group1" Display="Dynamic" Font-Bold="True" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                                </div>

                                                    <%--SignIn&Checkout Button--%>
                                                    <asp:Button ID="btnSignInAndCheckout" CssClass="btn btn-primary content to-upper" Text="(!checkoutanon.aspx.12!)" runat="server" ValidationGroup="Group1" CausesValidation="true" />

                                            
                                      
                                      
                                          
                                      
                                        </div>
                                    
                                        <%--Security Code--%>
                                        <asp:Panel ID="panelSecurityCode" runat="server">
                                               <div class="checkoutanon-container-security-code">
                                            <div class="form-group">
                                                <label>
                                                      <asp:Label ID="Label4" runat="server" Text="(!signin.aspx.18!)" Visible="false"></asp:Label>
                                                </label>
                                                <asp:TextBox ID="SecurityCode" runat="server" Visible="false" ValidationGroup="Group1"   CausesValidation="True"  EnableViewState="False" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode" ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False"></asp:RequiredFieldValidator>
                                                <asp:Image ID="SecurityImage" runat="server" Visible="false"></asp:Image>
                                            </div>
                                        
                                        </div>
                                        </asp:Panel>
                                 

                                            <%--Forgot Password--%>
                                        <div class="checkoutanon-container-forgot-password">
                                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="signin.aspx?checkout=true">(!checkoutanon.aspx.6!)</asp:HyperLink>
                                        </div>
                                        </div>
                                 
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="checkoutanon-container-nonregistered">

                                        <%--Register and Checkout--%>
                                        <div class="checkoutanon-container-nonregistered-reg-checkout panel panel-default">
                                            <div class="checkoutanon-container-header to-upper panel-heading">
                                                <asp:Label ID="Label7" runat="server" Text="(!checkoutanon.aspx.8!)" Font-Bold="true" /><br />
                                            </div>
                                            <div class="panel-body">
                                            <asp:Button ID="RegisterAndCheckoutButton" CssClass="btn btn-primary content to-upper" Text="(!checkoutanon.aspx.13!)" runat="server" CausesValidation="false" />
                                        </div>
                                        </div>

                                        <%--Skip Login and Checkout--%>
                                        <div class="checkoutanon-container-nonregistered-skip-checkout">
                                            <ise:Topic runat="server" ID="Teaser" TopicName="CheckoutAnonTeaser" />
                                            <asp:Panel runat="Server" ID="PasswordOptionalPanel" Visible="false">
                                                <div class="checkoutanon-container-header to-upper">
                                                    <asp:Label ID="Label8" runat="server" Text="(!checkoutanon.aspx.9!)" Font-Bold="true" /><br />
                                                </div>
                                                
                                                <asp:Label ID="Label9" runat="server" Text="(!checkoutanon.aspx.10!)" /><br />
                                                 <br />
                                                <asp:Button ID="Skipregistration" CssClass="site-button content to-upper" Text="(!checkoutanon.aspx.14!)" runat="server" CausesValidation="false" />
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                </div>
            </div>


        </div>

      
            
        </asp:Panel>

        <asp:Panel ID="ExecutePanel" runat="server" Width="90%" HorizontalAlign="center" Visible="false">
            <img src="images/spacer.gif" alt="" width="100%" height="40" />
            <asp:Label ID="SignInExecuteLabel" runat="server" Font-Bold="true"></asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
