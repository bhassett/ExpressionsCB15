<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!--[if lt IE 7]><html class="lt-ie9 lt-ie8 lt-ie7" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 7]><html class="lt-ie9 lt-ie8" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 8]><html class="lt-ie9" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if gt IE 8]><!-->
<html xmlns="http://www.w3.org/1999/xhtml">
<!--<![endif]-->
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, minimum-scale=1.0, maximum-scale=1.0" />
<title>(!METATITLE!)</title>
<meta name="description" content="(!METADESCRIPTION!)" />
<meta name="keywords" content="(!METAKEYWORDS!)" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/bootstrap/css/bootstrap.min.css" media="screen" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/font-awesome/css/font-awesome.min.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/font-awesome-old/css/font-awesome.min.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/ui.custom.min.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/content.css" type="text/css" />
<link href="skins/Skin_(!SKINID!)/ionicons/css/ionicons.min.css" rel="stylesheet" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/gridder-master/dist/css/jquery.gridder.min.css" type="text/css" />

<script type="text/javascript" src="jscripts/jquery/jquery.min.v1.7.2.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery-ui-1.8.16.custom.min.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.tmpl.min.js"></script>
<script type="text/javascript" src="jscripts/core.js"></script>
<script type="text/javascript" src="jscripts/minified/menu.js"></script>
<script type="text/javascript" src="jscripts/minified/attribute.selectors.js"></script>
<script type="text/javascript" src="jscripts/minified/jquery.format.1.05.js"></script>
<script type="text/javascript" src="jscripts/minified/address.dialog.js"></script>
<script type="text/javascript" src="jscripts/minified/bubble.message.js"></script>
<script type="text/javascript" src="jscripts/minified/jquery.loader.js"></script>
<script type="text/javascript" src="skins/Skin_(!SKINID!)/bootstrap/js/bootstrap.min.js"></script>
<script src="jscripts/jquery/malihu-custom-scrollbar/jquery.mCustomScrollbar.concat.min.js"></script>

<!-- Google Analytics -->
<script type="text/javascript">
    var _gaq = _gaq || [];
    $(window).load(function () {
    var pageTracking = ise.Configuration.getConfigValue("GoogleAnalytics.PageTracking");
    var ecTracking = ise.Configuration.getConfigValue("GoogleAnalytics.ConversionTracking");
    var gaAccount = ise.Configuration.getConfigValue("GoogleAnalytics.TrackingCode");
    var orderNumber = getQueryString()["ordernumber"]; // get ordernumber query string
    var imAtConfirmOrderPage = false;

    if (typeof orderNumber != "undefined" && ecTracking == 'true') { // verify if ordernumber query string is defined and ecTracking app config is  true
    imAtConfirmOrderPage = true;
    }

    if ((pageTracking == 'true' || imAtConfirmOrderPage) && gaAccount != "") { // verify if pageTracking app config is true OR current page is orderconfirmation.aspx
    //  uncomment to test your Google Analytics on localhost:
    // _gaq.push(['_setDomainName', 'none']); 
    _gaq.push(['_setAccount', gaAccount]); // set google analytics account (see app config GoogleAnalytics.TrackingCode
    _gaq.push(['_trackPageview']); // request page tracking
    }
    });
     </script>
<!-- Global Loader -->
<script type="text/javascript">
    $(document).ready(function () {
    $("body").globalLoader({autoHide: false,image: 'images/ajax-loader.gif',opacity: 0.3,text: 'loading...'});
    //sample implementation to display the loader
    //$("body").data("globalLoader").show();
  });
  </script>
(!JAVASCRIPT_INCLUDES!)
</head>
<body>
<asp:Panel ID="pnlForm" runat="server" Visible="false" />
<!-- PAGE INVOCATION: '(!INVOCATION' --> 
<!-- PAGE REFERRER: '(!REFERRER' --> 
<!-- STORE LOCALE: '(!STORELOCALE' --> 
<!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE' --> 
<!-- STORE VERSION: '(!STORE_VERSION' --> 
<!-- CACHE MENUS: '(!AppConfig name="CacheMenus"' -->
<div class="row clearfix"><div class="column full"><div class="display">
(!Topic Name="sitenav"!)
</div></div></div><!--row clearfix-->
<div class="row clearfix"><div class="column full"><div class="display">

<div class="livechat-float"> (!LIVECHAT!) <!--<img src="skins/Skin_(!SKINID!)/images/online.gif" />--> </div>
<div class="row main-content ltd-width">

  <div class="col-md-3 hidden-sm hidden-xs hidden-md hidden-lg"> 
    <!-- search and filter -->
    <div class="sidebox panel panel-default">
      <div class="panel-heading">Search</div>
      <div class="panel-body">(!XmlPackage Name="rev.search2"!)</div>
    </div>
    <!-- category menu -->
    <div class="sidebox panel panel-default">
      <div class="panel-heading">Browse Categories</div>
      <div class="panel-body">(!XmlPackage Name="rev.categorymenuside"!)</div>
    </div>
    <!-- department menu -->
    <div class="sidebox panel panel-default">
      <div class="panel-heading">Browse Departments</div>
      <div class="panel-body">(!XmlPackage Name="rev.departmentmenuside"!)</div>
    </div>
    <!-- manufacturer menu -->
    <div class="sidebox panel panel-default">
      <div class="panel-heading">Browse Manufacturers</div>
      <div class="panel-body">(!XmlPackage Name="rev.manufacturermenuside"!)</div>
    </div>
  </div>
    (!XmlPackage Name="rev.attributes" IsForAttributes="true"!)
  <div class="col-md-12 col-sm-12 main-container" style="min-height:700px;">
    <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
  </div>
</div>
</div></div></div><!--row clearfix-->
</div></div></div><!--row clearfix-->
<div class="row clearfix"><div class="column full"><div class="display" style="margin-bottom: 0px;">
(!Topic Name="sitefooter"!)
  </div></div></div><!--row clearfix--><!--row clearfix-->
 <!-- ADDRESS VERIFICATION --> 
  (!ADDRESS_VERIFICATION_DIALOG_LISTING!) 
     <script>
 var currentYear = (new Date()).getFullYear();
$(document).ready(function() {
$("#year").text(currentYear);
});
</script>
</body>
</html>