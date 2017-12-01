<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryList.ascx.cs" Inherits="UserControls_GiftRegistry_GiftRegistryList" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<asp:Label runat="server" ID="lblNoRecord" CssClass="registry-norecord-found" ></asp:Label>

<div class="registry-table">
    <table class="table table-hover table-bordered">
        <asp:Repeater ID="rptGiftRegistries" runat="server">
            <ItemTemplate>
                <tr id="row_<%# Container.DataItemAs<GiftRegistry>().Counter%>" >
                    <td></td>
                    <td>
                        <a class="registry-link" href="editgiftregistry.aspx?<%# DomainConstants.GIFTREGISTRYPARAMCHAR %>=<%# Container.DataItemAs<GiftRegistry>().RegistryID.ToString().ToLower() %>">
                                <%# Container.DataItemAs<GiftRegistry>().Title%></a>
                        <br />
                        <span class="registry-date">
                            <%# AppLogic.GetString("giftregistry.aspx.4")%> : 
                        </span>
                        <span class="registry-date">
                            <strong>
                                <%# Container.DataItemAs<GiftRegistry>().StartDate.Value.ToLongDateString()%>
                            </strong>
                        </span>
                    </td>
                    <td>
                        <a class="btn btn-default btn-block" href="editgiftregistry.aspx?<%# DomainConstants.GIFTREGISTRYPARAMCHAR %>=<%# Container.DataItemAs<GiftRegistry>().RegistryID.ToString().ToLower() %>">
                             <i class="fa fa-pencil"></i> <%= AppLogic.GetString("giftregistry.aspx.7")%>
                        </a>
                    </td>
                    <%--<td>
                        <a class="btn btn-default btn-block" href="#">Disable</a>
                    </td>--%>
                    <td>
                        <a class="btn btn-danger btn-block" href="javascript:void(0);" id="ctrDelete_<%# Container.DataItemAs<GiftRegistry>().Counter%>">
                            <i class="fa fa-trash-o"></i> <%= AppLogic.GetString("giftregistry.aspx.8")%>
                        </a>

                        <img style="display:none" id="ctrLoader_<%# Container.DataItemAs<GiftRegistry>().Counter%>" 
                                src="skins/skin_<%= ThisCustomer.SkinID %>/images/spinner.gif" alt="" />

                        <script type="text/javascript" >

                            $(document).ready(function () {

                                    var lnkDeleteId = '#ctrDelete_<%# Container.DataItemAs<GiftRegistry>().Counter%>';
                                    var currentRegistry = '<%# Container.DataItemAs<GiftRegistry>().RegistryID %>'
                                    $(lnkDeleteId).click(function () {
                                    
                                    if(confirm("<%= AppLogic.GetString("giftregistry.aspx.9", true)%>")) {
                                        
                                         $('#ctrLoader_<%# Container.DataItemAs<GiftRegistry>().Counter%>').show();
                                         $(lnkDeleteId).hide();

                                         AjaxCallWithSecuritySimplified(
                                            "DeleteGiftRegistry",
                                            { "giftRegistryID": currentRegistry },
                                            function (result) {
                                                $('#row_<%# Container.DataItemAs<GiftRegistry>().Counter%>').hide("fast");
                                            }
                                        );

                                    }
                                                                         
                                });

                            });
                            
                        </script>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
