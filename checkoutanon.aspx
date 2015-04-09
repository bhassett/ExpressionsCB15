<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutanon" CodeFile="checkoutanon.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head>
</head>
<body>
    <form runat="Server">
    <div align="text-center padding-bottom">
        <asp:Panel ID="HeaderPanel" runat="server">
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
        </asp:Panel>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="center">
            <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label>
            <asp:LinkButton ID="lnkContactUs" runat="server" PostBackUrl="contactus.aspx" Visible="false" />
            <br />
            <br />
        </asp:Panel>
        <asp:Panel ID="FormPanel" runat="server">
            <div class="row" id="table1">
                <div class="small-12 medium-6 columns">
                    <div class="panel text-left">
                        <p><h5><asp:Label ID="Label6" runat="server" Text="(!checkoutanon.aspx.11!)" Font-Bold="true"></asp:Label></h5>
                        <asp:Label ID="Label1" runat="server" Text="(!checkoutanon.aspx.3!)" /></p>
                        
                        <div class="row">
                            <div class="small-12 columns">
                                <label><asp:Label ID="Label2" runat="server" Text="(!checkoutanon.aspx.4!)" Font-Bold="true" />
                                <asp:TextBox ID="EMail" runat="server" ValidationGroup="Group1" MaxLength="100" CausesValidation="True"
                                    AutoCompleteType="Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="EMail"
                                    runat="server" ValidationGroup="Group1" ErrorMessage="!!" Display="Dynamic" Font-Bold="True"
                                    SetFocusOnError="True"></asp:RequiredFieldValidator></label>
                            </div>

                            <div class="small-12 columns">
                                <label><asp:Label ID="Label3" runat="server" Text="(!checkoutanon.aspx.5!)" Font-Bold="true" />
                           
                                <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" MaxLength="50"
                                    CausesValidation="True" TextMode="Password" Width="155px" AutoCompleteType="Disabled" autocomplete="off"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="Password"
                                    runat="server" ErrorMessage="!!" ValidationGroup="Group1" Display="Dynamic" Font-Bold="True"
                                    SetFocusOnError="True"></asp:RequiredFieldValidator></label>
                            </div>

                            <div class="small-12 columns" id="trSecurityCodeText" runat="server" visible="false">
                                <table>
                                    <tr>
                                        <td valign="top" align="left">
                                            <font class="LightCellText">
                                                <asp:Label ID="Label4" runat="server" Text="(!signin.aspx.18!)" Visible="false"></asp:Label></font>
                                        </td>
                                        <td valign="top" align="left">
                                            <asp:TextBox ID="SecurityCode" runat="server" Visible="false" ValidationGroup="Group1"
                                                CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox>
                                            <br />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode"
                                                ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False">
                                            </asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            
                            <div class="small-12 columns" id="trSecurityCodeImage" runat="server" visible="false">
                                <asp:Image ID="SecurityImage" runat="server" Visible="false"></asp:Image>
                            </div>
                            <div class="small-12 columns">
                                <asp:Button ID="btnSignInAndCheckout" CssClass="button small" Text="(!checkoutanon.aspx.12!)" runat="server" ValidationGroup="Group1" CausesValidation="true" />
                                <br />
                                
                                <asp:Label ID="Label5" runat="server" Text="(!checkoutanon.aspx.6!)" Font-Bold="true" />
                                <br />
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="signin.aspx?checkout=true">(!checkoutanon.aspx.7!)</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="small-12 medium-6 columns">
                    <div class="panel text-left">
                        <h5><asp:Label ID="Label7" runat="server" Text="(!checkoutanon.aspx.8!)" Font-Bold="true" /></h5>
                        <asp:Button ID="RegisterAndCheckoutButton" CssClass="button small" Text="(!checkoutanon.aspx.13!)" runat="server" CausesValidation="false" />
                        <ise:Topic runat="server" ID="Teaser" TopicName="CheckoutAnonTeaser" />
                    </div>
                    <div class="panel text-left">
                        <asp:Panel runat="Server" ID="PasswordOptionalPanel" Visible="false">
                        <asp:Label ID="Label8" runat="server" Text="(!checkoutanon.aspx.9!)" Visible="false" />
                        <asp:Label ID="Label9" runat="server" Text="(!checkoutanon.aspx.10!)" /><br />
                        <br />
                        <asp:Button ID="Skipregistration" CssClass="button small" Text="(!checkoutanon.aspx.14!)" runat="server" CausesValidation="false" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="ExecutePanel" runat="server" Width="90%" HorizontalAlign="center" Visible="false">
            <!-- <img src="images/spacer.gif" alt="" width="100%" height="40" /> -->
            <asp:Label ID="SignInExecuteLabel" runat="server" Font-Bold="true"></asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
