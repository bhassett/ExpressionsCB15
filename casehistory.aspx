<%@ Page Language="C#" ClientTarget="UpLevel" CodeFile="casehistory.aspx.cs" Inherits="InterpriseSuiteEcommerce.CaseHistory" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>
</head>
<body>
<form id="frmCaseHistory" runat="server">
  <asp:Panel ID="pnlPageContentWrapper" runat="server">
    <div class="sections-place-holder no-padding row">
      <div class="section-content-wrapper col-md-6 col-md-offset-3">
        <div class="section-header section-header-top">
          <asp:Literal ID="Literal3" runat="server">(!customersupport.aspx.42!)</asp:Literal>
        </div>
      </div>
      <div class="section-content-wrapper col-md-6 col-md-offset-3">
        <hr />
        <asp:Literal ID="Literal1" runat="server">(!customersupport.aspx.2!)</asp:Literal>
        &nbsp; <a class="btn btn-default btn-xs" href="contactus.aspx">
        <asp:Literal ID="ltrMenuContact" runat="server">(!customersupport.aspx.53!)</asp:Literal>
        </a>
        <hr />
        <div id="support-contact-form-place-holder">
          <div id="support-form-ajax-process-message">
            <asp:Literal ID="Literal11" runat="server">(!customersupport.aspx.3!)</asp:Literal>
          </div>
          <div class="btn-sep">
          <input type="button" id="open-case" class="btn btn-primary content" data-contentKey="customersupport.aspx.37" data-contentValue="<%=AppLogic.GetString("customersupport.aspx.37", true)%>" data-contentType="string resource" value="<%=AppLogic.GetString("customersupport.aspx.37", true)%>"/>
          </div>
          <hr />
          <div id="support-grid-wrapper">
            <div class="case-history-sections-head">
              <asp:Literal ID="lTrResolutionCases" runat="server">(!customersupport.aspx.26!)</asp:Literal>
            </div>
            <div id="support-header-controls">
              <div class="float-left custom-font-style form-group" id="support-select-view-place-holder">
                <label><asp:Literal ID="LtrView" runat="server">(!customersupport.aspx.27!)</asp:Literal></label>
                <select id="support-select-view" class="form-control">
                  <asp:Literal ID="ActivityStats" runat="server"></asp:Literal>
                </select>
              </div>
              <div class="float-left custom-font-style form-group" id="support-select-period-place-holder">
                <label><asp:Literal ID="ltrPeriod" runat="server">(!customersupport.aspx.28!)</asp:Literal></label>
                <select id="support-select-period" class="form-control">
                  <option value="30-days">
                  <asp:Literal ID="ltrLast30Days" runat="server">(!customersupport.aspx.29!)</asp:Literal>
                  </option>
                  <option value="6-months">
                  <asp:Literal ID="ltrLast6Months" runat="server">(!customersupport.aspx.30!)</asp:Literal>
                  </option>
                  <option value="12-months">
                  <asp:Literal ID="ltrLast12Months" runat="server">(!customersupport.aspx.31!)</asp:Literal>
                  </option>
                </select>
              </div>
              <div class="float-left custom-font-style" id="support-select-search-place-holder">
                <div id="support-search-caption" class="custom-font-style float-left form-group">
                  <label><asp:Literal ID="LtrSearch" runat="server">(!customersupport.aspx.32!)</asp:Literal></label>
                </div>
                <div id="support-search-text-container" class="float-left">
                  <input type="text" id="support-search-text" class="form-control" />
                  <a id="support-search-go" href="javascript:void(1);" class="btn btn-default"><i class="fa fa-search"></i> Search</a></div>
              </div>
            </div>
            <div id="support-header-fields">
              <div class="float-left custom-font-style" id="support-date-place-holder">
                <asp:Literal ID="LtrDate" runat="server">(!customersupport.aspx.33!)</asp:Literal>
              </div>
              <div class="float-left custom-font-style" id="support-subject-place-holder">
                <asp:Literal ID="LtrSubject" runat="server">(!customersupport.aspx.34!)</asp:Literal>
              </div>
              <div class="float-left custom-font-style" id="support-status-place-holder">
                <asp:Literal ID="LtrStatus" runat="server">(!customersupport.aspx.35!)</asp:Literal>
              </div>
              <div class="clear-both height-5"></div>
            </div>
            <hr />
            <div id="support-grid-details" class="well well-sm"></div>
            <div class="case-history-sections-head" style="height: 1px"></div>
          </div>
        </div>
        <div class="clear-both height-5"></div>
      </div>
    </div>
    </asp:Panel>
  <div id="_DEFAULT_MESSAGE" style="display: none;">
    <asp:Literal ID="Literal19" runat="server">(!customersupport.aspx.3!)</asp:Literal>
  </div>
  <div id="_DOWNLOAD_CASE_MESSAGE" style="display: none;">
    <asp:Literal ID="Literal2" runat="server">(!customersupport.aspx.36!)</asp:Literal>
  </div>
  <script type="text/javascript" src="jscripts/minified/case.history.js"></script>
</form>
</body>
</html>
