<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreLocatorControl.ascx.cs"
    Inherits="UserControls_StoreLocator_StoreLocatorControl" EnableViewState="true" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script type="text/javascript" src="//maps.googleapis.com/maps/api/js?key=<%= AuthenticationTokenKey %>&sensor=<%= Sensor.ToStringLower() %>&v=<%= Version %>"></script>
<script type="text/javascript" src="jscripts/store_locator/store-locator-plugin-template.js"></script>
<script type="text/javascript" src="jscripts/store_locator/store-locator-plugin.js"></script>
<asp:Panel ID="pnlMain" runat="server" CssClass="locator-main-container" Style="position: relative;">
    <div class="entity-header">
        <h1>
            <%= AppLogic.GetString("menu.StoreLocator")%>
        </h1>
    </div>
    <div class="locator-search-main">
        <div class="locator-search-header">
            <div class="locator-search-header-text">
                <h1>
                    <%= AppLogic.GetString("storelocator.aspx.3")%></h1>
            </div>
            <div class="locator-icon">
                <a href="javascript:void(0);" id="lnkViewWide">
                    <span class="locator-wide-screen-icon"></span>
                </a>
            </div>
            <div class="locator-icon">
                <a href="javascript:void(0);" id="lnkExpanCollapse" class="locator-collapse-icon">
                </a>
            </div>
        </div>
        <div class="clr">
        </div>
        <div class="locator-search-detail well well-sm">
            
<div class="row">
<div class="col-sm-6" style="padding:0px;">
    <label  class="col-sm-2 control-label text-right"> <%= AppLogic.GetString("storelocator.aspx.10", true)%> </label>
    <div class="col-sm-10" style="padding:0px;">
      <input type="text" id="txtInputAddress" class="form-control locator-search-input" />
       <div class="form-input"><i class="fa fa-info-circle"></i> <em class="locator-search-note"> <%= AppLogic.GetString("storelocator.aspx.11", true)%> </em> </div>
    </div>
  </div>
  <div class="col-sm-3" style="padding:0px;">
    <label  class="col-sm-6 control-label text-right"> <%= AppLogic.GetString("storelocator.aspx.4")%> </label>
    <div class="col-sm-6" style="padding:0px;">
      <asp:DropDownList runat="server" ID="dlStoreType" class="storeTypeClass form-control"> </asp:DropDownList>
    </div>
    </div>
    <div class="col-sm-3" style="padding:0px;">
    <label  class="col-sm-8 control-label text-right"> <%= AppLogic.GetString("storelocator.aspx.6")%> <strong>( <%= AppLogic.GetString("storelocator.aspx.21")%> )</strong> </label>
    <div class="col-sm-4" style="padding:0px;">
      <select id="search-miles" class="form-control">
        <option value="ALL">All</option>
        <option value="3">3</option>
        <option value="5">5</option>
        <option value="10">10</option>
        <option value="30">30</option>
        <option value="50">50</option>
        <option value="100">100</option>
        <option value="200">200</option>
        <option value="300">300</option>
        <option value="500">500</option>
        <option value="1000">1000</option>
      </select>
    </div>
  </div>
  
</div>
<div class="row">
  <div class="col-sm-8" style="padding:0px;">
  <div class="direction-input" id="direction-input">
    <label  class="col-sm-4 control-label text-right"> <%= AppLogic.GetString("storelocator.aspx.24", true)%> </label>
    <div class="col-sm-8" style="padding:0px;">
      <input type="text" id="txtAddressDirection" class="locator-search-input form-control" />
      <div> <em class="locator-search-note"> <%= AppLogic.GetString("storelocator.aspx.20", true)%> </em> </div>
    </div>
  </div>
  </div>
  <div class="col-sm-4" style="padding:0px;">
    <div> </div>
    <div class="form-input locator-search-input-adjust-spacetop">
      <input type="button" class="btn btn-primary content" id="btnSearchLocator" 
                        data-contentKey="storelocator.aspx.9"
                        data-contentValue="<%= AppLogic.GetString("storelocator.aspx.9", true)%>"
                        data-contentType="string resource"
                        value="<%= AppLogic.GetString("storelocator.aspx.9", true)%>" />
    </div>
  </div>
</div>

        </div>
    </div>
    <div class="map-wrapper well well-sm" style="padding:0px">
        <div id="map">
        </div>
        <div id="direction-input" class='direction-input'>
            <div class="direction-input-wrapper">
                <span class="locator-search-note"><%= AppLogic.GetString("storelocator.aspx.11")%></span>
                <br /><br />
                <input type="text" id="txtAddressDirection" class="locator-search-input"  />
                <br /><br />
                <input type="button" class="site-button" id="btnGetDirection" value="<%= AppLogic.GetString("storelocator.aspx.16", true)%>" />

            </div>
        </div>
        <div id="dirPanel">
        </div>
    </div>
    <div class="selectors-wrapper well well-sm">
        <div class="header-selectors">
        </div>
        <div class="selectors-body">
            <ul id="store-menu">
            </ul>
        </div>
    </div>
    <h3>
        <asp:Label ID="lblError" runat="server" CssClass="error-message">
        </asp:Label>
    </h3>
</asp:Panel>
<script type="text/javascript">

    var options = {
        searchInputButtonId: 'btnSearchLocator',    
        searchInputId: 'txtInputAddress',
        distanceInputId: 'search-miles',
        storeTypeInputClass: '.storeTypeClass',
        storeMenuId: 'store-menu',
        storeMenuTemplate: 'storeMenuTemplate',
        headerContainerClass: '.header-selectors',
        headerTextTemplate: 'headerTextTemplate',
        infoWindowTemplate: 'infoWindowTemplate',
        addressInfoWindowTemplate: 'addressInfoWindowTemplate',
        directionInputContainerId: 'direction-input',
        directionInputId: 'txtAddressDirection',
        directionlinkClass: 'store-infowindow-direction-link',
        getDirectionButtonId: 'btnGetDirection',
        storeTypeDropdownId: '<%= dlStoreType.ClientID %>',
        storesIcon: 'skins/Skin_<%= ThisCustomer.SkinID %>/images/storelocator/storeicon.png',
        addressIcon: 'skins/Skin_<%= ThisCustomer.SkinID %>/images/storelocator/addressIcon.png',
        wideScreenButtonId: 'lnkViewWide',
        expandCollapseButtonId: 'lnkExpanCollapse',
        mainLocatorContainerClass: '.locator-main-container',
        searchContainerClass: '.locator-search-detail',
        mapContainerClass: '.map-wrapper',
        messages: {
            MESSAGE_VALIDATION_SEARCH_INPUT: '<%= AppLogic.GetString("storelocator.aspx.12", true)%>',
            MESSAGE_VALIDATION_SEARCH_NOT_FOUND: '<%= AppLogic.GetString("storelocator.aspx.13", true)%>',
            MESSAGE_OUTPUT_SEARCH_FOUND: '<%= AppLogic.GetString("storelocator.aspx.14", true)%>',
            MESSAGE_DEFAULT_SEARCH_HEADER_TEXT: '<%= AppLogic.GetString("storelocator.aspx.15", true)%>',
            MESSAGE_DEFAULT_DIRECTION_HEADER_TEXT: '<%= AppLogic.GetString("storelocator.aspx.18", true)%>',
            MESSAGE_DEFAULT_DIRECTION_LINK_TEXT: '<%= AppLogic.GetString("storelocator.aspx.17", true)%>',
            MESSAGE_DEFAULT_WIDESCREEN_BUTTON_TITLE: '<%= AppLogic.GetString("storelocator.aspx.19", true)%>',
            MESSAGE_DEFAULT_COLLAPSE_TITLE: '<%= AppLogic.GetString("storelocator.aspx.22", true)%>',
            MESSAGE_DEFAULT_EXPAND_TITLE: '<%= AppLogic.GetString("storelocator.aspx.23", true)%>'
        }
    }

    function InitializeMap() { $("#map").StoreLocator.initialize(options); }

    google.maps.event.addDomListener(window, 'load', InitializeMap);

</script>
