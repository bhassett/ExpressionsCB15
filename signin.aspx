<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.signin" CodeFile="signin.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <form runat="Server" method="POST" id="SigninForm" name="SigninForm">
     <div>
         <div align="center">
            <asp:Panel ID="FormPanel" runat="server" Width="100%">
                <asp:Panel ID="CheckoutPanel" runat="server">
                    <div id="CheckoutSequence" align="center">
                        <asp:ImageMap ID="CheckoutMap" runat="server" ImageUrl="(!skins/skin_(!SKINID!)/images/step_2.gif!)">
                            <asp:RectangleHotSpot Bottom="54" HotSpotMode="Navigate"
                                NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true" Right="87" />
                        </asp:ImageMap>
                        </div>
                </asp:Panel>
                <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="Left">
                            <asp:Label CssClass="errorLg" ID="ErrorMsgLabel" runat="server"></asp:Label>
                            <asp:LinkButton ID="lnkContactUs" runat="server" PostBackUrl="contactus.aspx" Visible="false" />
                </asp:Panel>
                <ise:Topic runat="server" ID="HeaderMsg" TopicName="SigninPageHeader"  />
                <p align="left"><b><asp:Label ID="Label11" runat="server" Text="(!signin.aspx.5!)" Visible ="false"></asp:Label>&nbsp;&nbsp;</b></p>
                
                <div class="row">
                    <div class="col-sm-offset-3 col-sm-6">
                        
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <asp:Literal ID="LtrPageWelcomeHeader_Caption" runat="server">(!signin.aspx.7!)</asp:Literal>
                            </div>
                            <div class="panel-body text-left">
                                <b>
                                    <asp:Label ID="Label3" runat="server" Text="(!signin.aspx.9!)" for="EMail"></asp:Label>
                                </b>
                                <asp:TextBox ID="EMail" runat="server" ValidationGroup="Group1" Columns="30" MaxLength="100" CausesValidation="True"
                                    AutoCompleteType="Email" cssclass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Group1"
                                    ErrorMessage="(!signin.aspx.3!)" ControlToValidate="EMail"></asp:RequiredFieldValidator>
                                <b><asp:Label ID="Label2" runat="server" Text="(!signin.aspx.10!)" for="Password"></asp:Label></b>
                                <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" Columns="30" MaxLength="50"
                                    CausesValidation="True" TextMode="Password" AutoCompleteType="Disabled" autocomplete="off" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Group1"
                                    ErrorMessage="(!signin.aspx.4!)" ControlToValidate="Password"></asp:RequiredFieldValidator>
                                    <div runat="server" style="display: inline-block;" visible="false">
                                    <font class="LightCellText"><asp:Label ID="Label1" runat="server" Text="(!signin.aspx.18!)" Visible="False" for="SecurityCode"></asp:Label></font>
                                    <asp:Image ID="SecurityImage" runat="server" Visible="False"></asp:Image>
                                    <asp:TextBox ID="SecurityCode" runat="server" Visible="False" ValidationGroup="Group1"
                                                                CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox><asp:RequiredFieldValidator
                                                                    ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode"
                                                                    ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False"></asp:RequiredFieldValidator>
                                    </div>
                                    <asp:Label ID="Label7" runat="server" Text="(!signin.aspx.11!)" for="PersistLogin"></asp:Label>
                                    <asp:CheckBox ID="PersistLogin" runat="server" CssClass="form-Control"></asp:CheckBox>
                                <br />  
                                <br />  
                                <div class="row">
                                    <div class="col-sm-6">
                                        <asp:Button ID="LoginButton" runat="server" CssClass="btn btn-success btn-block content form-control" ValidationGroup="Group1">
                                        </asp:Button>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:HyperLink ID="SignUpLink" runat="server" CssClass="btn btn-default btn-block content form-control">(!signin.aspx.6!)</asp:HyperLink>
                                    </div>
                                </div>
                                <br /> 
                                <div class="row">
                                    <div class="col-sm-6">
                                        <a href="#" data-toggle="modal" data-target="#forgotPassword">
                                            Forgot Password?
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="forgotPassword" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
                  <div class="modal-dialog" role="document">
                    <div class="modal-content">
                      <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><i class="fa fa-times"></i></button>
                        <br />
                      </div>
                      <div class="modal-body">
                         <!--Request Password Section Starts Here -->
                          <div class="panel panel-default">
                              <div class="panel-heading">
                                  <div runat="server" style="display: inline-block;" visible="false">
                                      <p align="left"><asp:Label ID="Label8" runat="server" Text="(!signin.aspx.13!)" Visible="false"></asp:Label></p>
                                      <div class="clr height-12"></div>
                                      <asp:Literal ID="Literal1" runat="server" Visible="false">(!signin.aspx.14!)</asp:Literal>
                                  </div>
                                  <asp:Label ID="Label6" runat="server" Text="(!signin.aspx.12!)"></asp:Label>
                              </div>
                              <div class="panel-body text-left">
                                <div class="row">
                                    <div class="col-sm-4">
                                        <font class="LightCellText">&nbsp;<asp:Label ID="Label12" runat="server" Text="(!signin.aspx.9!)"></asp:Label></font>
                                    </div>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="ForgotEMail" runat="server" ValidationGroup="Group2" CausesValidation="True"
                                            AutoCompleteType="Email" Columns="30"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ForgotEMail" ErrorMessage="(!signin.aspx.3!)" ValidationGroup="Group2"></asp:RequiredFieldValidator>    
                                    </div>
                                </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:Button ID="RequestPassword" runat="server" CssClass="btn btn-default content" ValidationGroup="Group2"></asp:Button>
                                        </div>
                                    </div>
                              </div>
                           </div>
                           <!--Request Password Section Ends Here -->
                      </div>
                    </div>
                  </div>
                </div>
                    <div class="clr height-5"></div>
                  </div>
             </div>
             <!--Log In Form Section Ends Here -->

             <div class="clr height-12"></div>

             


            </asp:Panel>
                           </div>
            </div>
            <asp:Panel ID="ExecutePanel" runat="server" Width="90%">
                <div align="center">
                    <img src="images/spacer.gif" alt="" width="100%" height="40" />
                    <b>
                        <asp:Literal ID="SignInExecuteLabel" runat="server"></asp:Literal></b></div>
            </asp:Panel>
            <asp:CheckBox ID="DoingCheckout" runat="server" Visible="False" />
            <asp:Label ID="ReturnURL" runat="server" Text="default.aspx" Visible="False" />
        </div>
    </form>
</body>
</html>
