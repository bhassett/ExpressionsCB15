<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!--[if lt IE 7]><html class="lt-ie9 lt-ie8 lt-ie7" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 7]><html class="lt-ie9 lt-ie8" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 8]><html class="lt-ie9" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if gt IE 8]><!-->
<html xmlns="http://www.w3.org/1999/xhtml"><!--<![endif]-->
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
    <!-- PAGE INVOCATION: '(!INVOCATION!)' -->
    <!-- PAGE REFERRER: '(!REFERRER!)' -->
    <!-- STORE LOCALE: '(!STORELOCALE!)' -->
    <!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
    <!-- STORE VERSION: '(!STORE_VERSION!)' -->
    <!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
   

    <header class="hidden-sm hidden-xs">
        <div class="row">
            <div class="col-md-6">
                <a href="default.aspx"><img class="logo-main" src="skins/Skin_(!SKINID!)/images/celcom-logo.png" alt="" /></a>
            </div>
            <div class="col-md-6 text-right">
                (!USERNAME!) &nbsp; <a href="(!SIGNINOUT_LINK!)" id="signInOutLink"><i class="icon-key"></i>(!SIGNINOUT_TEXT!)</a> &nbsp;
                <a id="shopping-cart" href="javascript:void(0)"><i class="icon-shopping-cart"></i>(!CARTPROMPT!)((!NUM_CART_ITEMS!))</a>
                 <div id="mini-cart"></div>
            </div>
            
        </div>
    </header>

    <nav class="navbar-inverse" role="navigation">

        <!-- mobile menu -->
        <div class="hidden-md hidden-lg">(!XmlPackage Name="rev.menumobile"!)</div>

        <div class="row">
            <div class="col-md-9">
                <!-- desktop menu -->
                <div id="menu_container" runat="server" class="hidden-sm hidden-xs"></div>
            </div>
            <div class="col-md-3">(!XmlPackage Name="rev.search"!)</div>
        </div>
    </nav>

    <div id="subnav">
      <div class="row">
        <div class="col-md-9">
            <ul>
                <li><a href="bestsellers.aspx"><i class="icon-tags"></i> Bestsellers</a></li>
                <li><a href="recentadditions.aspx"><i class="icon-plus-sign"></i> Recent Addition</a></li>
                <span runat="server">
                    <% if (InterpriseSuiteEcommerceCommon.AppLogic.AppConfigBool("GiftRegistry.Enabled"))
                       { %>
                            <li><a href="giftregistry.aspx"><i class="icon-gift"></i>Gift Registry</a></li>
                    <% } %>
                </span>
                <li><a href="storelocator.aspx"><i class="icon-map-marker"></i> Store Locator</a></li>
                <li><a href="wishlist.aspx"><i class="icon-star"></i> Wishlist</a></li>
            </ul>
        </div>
        <div class="col-md-3 livechat">
            (!LIVECHAT!)
            <img src="skins/Skin_(!SKINID!)/images/online.gif" />
        </div>
      </div>
    </div>

    <div class="row">
        <div class="col-md-3 hidden-sm hidden-xs">
            <!-- category menu -->
            <div class="sidebox panel panel-default">
                <div class="panel-heading">Browse Categories</div>
                <div class="panel-body">(!XmlPackage Name="rev.categorymenu"!)</div>
            </div>
            <!-- department menu -->
            <div class="sidebox panel panel-default">
                <div class="panel-heading">Browse Departments</div>
                <div class="panel-body">(!XmlPackage Name="rev.departmentmenu"!)</div>
            </div>
            <!-- manufacturer menu -->
            <div class="sidebox panel panel-default">
                <div class="panel-heading">Browse Manufacturers</div>
                <div class="panel-body">(!XmlPackage Name="rev.manufacturermenu"!)</div>
            </div>
        </div>
        <div class="col-md-9 main-container">
            <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
        </div>
    </div>

    <div class="row" id="subFooter">
        <div class="col-md-4">
            <a href="t-shipping.aspx" class="btn btn-block btn-large">
                <span class="fa fa-plane"></span> Shipping Policy <br />
                <span class="tiny">Free Delivery on Certain Areas</span>
            </a>
        </div>
        <div class="col-md-4">
            <a href="news.aspx" class="btn btn-block btn-large">
                <span class="fa fa-rss-square"></span> Latest News<br />
                <span class="tiny">Get a regular feed of news and events</span>
            </a>
        </div>
        <div class="col-md-4">
            <a href="casehistory.aspx" class="btn btn-block btn-large">
                <i class="fa fa-phone"></i> Our Support <br />
                <span class="tiny">Call us: 1800-000-555</span>
            </a>
        </div>
    </div>

    <footer>
        <div class="row">
            <div class="col-md-3 col-xs-6">
                <div class="header">Navigation</div>
                <ul>
                    <li><a href="default.aspx">Home</a></li>
                    <li><a href="contactus.aspx">Contact Us</a></li>
                    <li><a href="casehistory.aspx">Customer Support</a></li>
                    <li><a href="sitemap2.aspx">Sitemap</a></li>
                </ul>
            </div>
            <div class="col-md-3 col-xs-6">
                <div class="header">Header</div>
                <ul>
                    <li><a href="t-faq.aspx">FAQs</a></li>
                    <li><a href="t-returns.aspx">Return Policy</a></li>
                    <li><a href="t-privacy.aspx">Privacy Policy</a></li>
                    <li><a href="t-security.aspx">Security Policy</a></li>
                    <li><a href="t-shipping.aspx">Shipping Info</a></li>
                </ul>
            </div>
            <div class="col-md-3 col-xs-6">
                <div class="header">Services</div>
                <ul>
                    <li><a href="storelocator.aspx">Store Locator</a></li>
                    <li><a href="giftregistry.aspx">Gift Registry</a></li>
                    <li><a href="wishlist.aspx">Wishlist</a></li>
                    <li><a href="leadform.aspx">Lead Form</a></li>
                </ul>
                        </div>
            <div class="col-md-3 col-xs-6">
                <div class="header">Follow Us</div>
            
                <div class="social-media">
                    <div class="socialmedia_subscribebox">
                  <a href="http://facebook.com" class="facebook" target="_blank" title="follow us on facebook"></a>
                  <a href="http://twitter.com" class="twitter" target="_blank" title="follow us on twitter"></a>
                  <a href="http://digg.com" class="digg" target="_blank" title="follow us on digg"></a>
              </div>
                </div>
                <!-- CUSTOMER SUPPORT -->
                <div class="customer-support">
                    <div id="request-container">
                        <span>(!stringresource name="main.content.1"!)</span><br />
                        <span class="request-generator-content request-code" >---------------</span>
                        <span title="Refresh"><a href="javascript:void(0)" class="generate-link"><i class="fa fa-refresh"></i></a></span>
                        <span title="Copy to Clipboard"><a href="javascript:void(0)" class="copy-link"><i class="fa fa-files-o"></i></a></span>
                    </div>
                </div>
                <!-- END OF CUSTOMER SUPPORT -->
            </div>
        </div>
        <div class="row copyright">
            Copyright &copy; 2014. All Rights Reserved.
        </div>

         <!-- BUBBLE MESSAGE -->
        <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div>

        <!-- ADDRESS VERIFICATION -->
        (!ADDRESS_VERIFICATION_DIALOG_LISTING!)
    </footer>
</body>
</html>