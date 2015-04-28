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
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/ui.custom.min.css" type="text/css" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" type="text/css" />
        <script type="text/javascript" src="jscripts/jquery/jquery.min.v1.7.2.js"></script>
        <script type="text/javascript" src="jscripts/jquery/jquery-ui-1.8.16.custom.min.js"></script>
        <script type="text/javascript" src="jscripts/jquery/jquery.tmpl.min.js"></script>
        <script type="text/javascript" src="jscripts/minified/core.js"></script>
        <script type="text/javascript" src="jscripts/minified/menu.js"></script>
        <script type="text/javascript" src="jscripts/minified/attribute.selectors.js"></script>
        <script type="text/javascript" src="jscripts/minified/jquery.format.1.05.js"></script>
        <script type="text/javascript" src="jscripts/minified/address.dialog.js"></script>
        <script type="text/javascript" src="jscripts/minified/bubble.message.js"></script>
        <script type="text/javascript" src="jscripts/minified/jquery.loader.js"></script>
        
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

    <!-- mobile menu wrap -->
    <div class="off-canvas-wrap" data-offcanvas>
      <div class="inner-wrap">
        <nav class="tab-bar show-for-small-only">
          <section class="left-small">
            <a class="left-off-canvas-toggle menu-icon" href="#"><span></span></a>
          </section>

          <section class="middle tab-bar-section">
            <h1 class="title">Expressions</h1>
          </section>

          <section class="right-small tab-bar-section">
            <a href="shopping-cart.shtml"><i class="fi-shopping-bag"></i></a>
          </section>
        </nav>

        <form>
        <aside class="left-off-canvas-menu">
          <ul class="off-canvas-list">
            <li><label>(!USERNAME!)</label></li>
            <li><a href="(!SIGNINOUT_LINK!)"><small>(!SIGNINOUT_TEXT!)</small></a></li>
            <li><label>Customer Service</label></li>
            <li><a href="account.aspx">My Account<br><small>Address Book, Order History</small></a></li>
            <span runat="server">
                <% if (InterpriseSuiteEcommerceCommon.AppLogic.AppConfigBool("GiftRegistry.Enabled"))
                   { %>
                        <li><a href="giftregistry.aspx">Gift Registry</a></li>
                <% } %>
            </span>
            <li><a href="wishlist.aspx">Wishlist</a></li>
            <li><a href="account.aspx">Order Status<br><small>Check your order status</small></a></li>
            <li><a href="t-shipping.aspx">Shipping Info<br><small>Estimates delivery times</small></a></li>
            <li><a href="t-returns.aspx">Returns &amp; Exchanges<br><small>How to return items</small></a></li>
            <li><a href="t-contact.aspx">Contact Us <i class="fi-telephone"></i></a></li>
          </ul>
        </aside>
        </form>

        <!-- page content -->
        <section class="main-section">
          
          <!-- Header -->
          <div class="row show-for-medium-up">
            <div class="medium-12 columns text-center"><a href="/Staging/"><img src="skins/Skin_(!SKINID!)/images/global/expressions-logo.png" /></a></div>
          </div>

          <div class="grey-wrapper">
            <div class="row small-collapse toppanel">
              <div class="medium-3 columns show-for-medium-up account-info">
                <h5><small><a class="username" href="account.aspx">(!USERNAME!)</a><br><a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a> | <a href="account.aspx">My Account</a></small></h5>
              </div>
              <div class="small-12 medium-6 columns">
                (!XmlPackage Name="rev.search"!)
              </div>
              <div class="medium-2 columns show-for-medium-up text-center">
                <div class="customer-service-icon">
                  <a data-dropdown="drop1" data-options="align:bottom"><i class="fi-torsos"></i><label>Customer Service</label></a>
                  <ul id="drop1" class="medium f-dropdown" data-dropdown-content>
                    <li><a href="account.aspx">My Account<br><small>Address Book, Order History</small></a></li>
                    <span runat="server">
                        <% if (InterpriseSuiteEcommerceCommon.AppLogic.AppConfigBool("GiftRegistry.Enabled"))
                           { %>
                                <li><a href="giftregistry.aspx">Gift Registry</a></li>
                        <% } %>
                    </span>
                    <li><a href="wishlist.aspx">Wishlist</a></li>
                    <li><a href="account.aspx">Order Status<br><small>Check your order status</small></a></li>
                    <li><a href="t-shipping.aspx">Shipping Info<br><small>Estimates delivery times</small></a></li>
                    <li><a href="t-returns.aspx">Returns &amp; Exchanges<br><small>How to return items</small></a></li>
                    <li><a href="t-contact.aspx">Contact Us <i class="fi-telephone"></i></a></li>
                  </ul>
                </div>
              </div>
              <div class="medium-1 columns show-for-medium-up text-right">
                <div class="shopping-cart-icon">
                  <a href="shoppingcart.aspx"><i class="fi-shopping-cart"></i>
                  <div class="shopping-cart-number">(!NUM_CART_ITEMS!)</div></a>
                </div>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <div class="contain-to-grid">
            <nav class="top-bar" data-topbar role="navigation">
              <ul class="title-area">
                <li class="name"></li>
                <li class="toggle-topbar menu-icon"><a href="#"><span>Browse</span></a></li>
              </ul>

              <section class="top-bar-section">
                 <ul class="left">
                  <li><a href="/Staging/">Home</a></li>
                  <li class="has-dropdown">
                    <a href="c-2-accessories.aspx">Accessories</a>
                    <ul class="dropdown">
                      <li><a href="c-12-decorative-accents.aspx">Decorative Accents</a></li>
                      <li><a href="c-14-candleholders.aspx">Candleholders</a></li>
                      <li><a href="c-15-baskets-boxes.aspx">Baskets &amp; Boxes</a></li>
                      <li><a href="c-10-picture-frames.aspx">Picture Frames</a></li>
                      <li><a href="c-13-clocks.aspx">Clocks</a></li>
                      <li><a href="c-9-urns-jars.aspx">Urns &amp; Jars</a></li>
                      <li><a href="c-11-decorative-bowls.aspx">Decorative Bowls</a></li>
                      <li><a href="c-48-decorative-plates-chargers.aspx">Plates &amp; Chargers</a></li>
                      <li><a href="c-8-vases.aspx">Vases</a></li>
                      <li><a href="c-29-trays.aspx">Trays</a></li>
                      <li><a href="c-31-planters.aspx">Planters</a></li>
                      <li><a href="c-32-sculpture.aspx">Sculpture</a></li>
                      <li><a href="c-45-floral.aspx">Floral</a></li>
                    </ul>
                  </li>
                  <li class="has-dropdown">
                    <a href="c-4-wall-decor.aspx">Wall Decor</a>
                    <ul class="dropdown">
                      <li><a href="c-20-racks-shelving.aspx">Racks &amp; Shelving</a></li>
                      <li><a href="c-21-sconces.aspx">Sconces</a></li>
                      <li><a href="c-22-wall-clocks.aspx">Wall Clocks</a></li>
                      <li><a href="c-23-wall-art.aspx">Wall Art</a></li>
                      <li><a href="c-26-mirrors.aspx">Mirrors</a></li>
                      <li><a href="c-28-hanging-picture-frames.aspx">Hanging Frames</a></li>
                    </ul>
                  </li>
                  <li class="has-dropdown">
                    <a href="c-5-lighting.aspx">Lighting</a>
                    <ul class="dropdown">
                      <li><a href="c-24-floor-lamps.aspx">Floor Lamps</a></li>
                      <li><a href="c-25-table-lamps.aspx">Table Lamps</a></li>
                    </ul>
                  </li>
                  <li class="has-dropdown">
                    <a href="c-6-furniture.aspx">Accent Furniture</a>
                    <ul class="dropdown">
                      <li><a href="c-34-chairs.aspx">Chairs</a></li>
                      <li><a href="c-35-benches-ottomans.aspx">Benches &amp; Ottomans</a></li>
                      <li><a href="c-40-tables-desks.aspx">Tables &amp; Desks</a></li>
                      <li><a href="c-41-chests-cabinets.aspx">Chests &amp; Cabinets</a></li>
                      <li><a href="c-44-accent-furniture.aspx">Other Accent Furniture</a></li>
                    </ul>
                  </li>
                  <li><a href="c-52-exclusive.aspx">Exclusive</a></li>
                </ul> 
              </section>
            </nav>
          </div>

          <div class="row">
            <div class="small-12 columns">
              <ul class="breadcrumbs">
                <li class="current">(!SECTION_TITLE!)</li>
              </ul>
            </div>
          </div>

    <!-- CONTENT -->
    <div class="row">
    <div class="small-12 columns">
    <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
    </div>
    </div>
    <!-- END OF CONTENT -->

    <!-- FOOTER -->

              <div class="grey-wrapper content-bottom">
            <div class="row pagefooterbar">
              <div class="small-12 columns text-center"><h4><small>We Accept <img src="skins/Skin_(!SKINID!)/images/global/billmethod-icons.png" /></small></h4></div>
            </div>
          </div>

          <div class="black-wrapper">
            <div class="row pagefooter">
              <div class="small-12 medium-6 medium-push-6 columns small-text-center medium-text-right paddingbottom">
                
                <h6><small><a href="t-about.aspx">About Us</a> <span>|</span> <a href="sitemap2.aspx">Site Map</a> <span>|</span> <a href="t-contact.aspx">Contact Us</a></small></h6>

                <!-- AddThis Follow BEGIN -->
                <div class="addthis_toolbox addthis_32x32_style addthis_default_style small-text-center medium-text-right">
                <a addthis:userid="expressionsdecor"><i class="fi-social-facebook"></i></a>
                <a addthis:userid="ExpressionsDeco"><i class="fi-social-twitter"></i></a>
                <a addthis:userid="expressionsdeco"><i class="fi-social-pinterest"></i></a>
                <a addthis:userid="expressionsdecor"><i class="fi-social-instagram"></i></a>
                <a addthis:userid="117223946102304177432"><i class="fi-social-google-plus"></i></a>
                <a addthis:userid="117223946102304177432" addthis:usertype="company"><i class="fi-social-linkedin"></i></a>
                </div>
                <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50dc8ab51b6823ca"></script>
                <!-- AddThis Follow END -->
                
              </div>
              <div class="small-12 medium-6 medium-pull-6 columns small-text-center medium-text-left copyright"><h6><small>&#169; 2015 Expressions by Decor &#38; More, Inc. All rights reserved.<br><a href="t-termsandconditions.aspx">Terms of Use</a> <span>|</span> <a href="t-privacy.aspx">Privacy Policy</a></small></h6></div>
            </div>
          </div>

        </section>

      <a class="exit-off-canvas"></a>

      </div>
    </div>


    <!-- END OF FOOTER -->

    <!-- BUBBLE MESSAGE -->
    <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div> 
            
    <!-- ADDRESS VERIFICATION -->
    (!ADDRESS_VERIFICATION_DIALOG_LISTING!)

     <!-- GOOGLE ANALYTICS -->
    <script type="text/javascript">
        $(window).load(function () {
            var pageTracking = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.PageTracking"));
            var ecTracking = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.ConversionTracking"));
            var gaAccount = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.TrackingCode"));
            var orderNumber = getQueryString()["ordernumber"]; // get ordernumber query string
            var imAtConfirmOrderPage = false;

            if (typeof orderNumber != "undefined" && ecTracking == 'true') imAtConfirmOrderPage = true;

            if ((pageTracking == 'true' || imAtConfirmOrderPage) && gaAccount != "GoogleAnalytics.TrackingCode") {
                var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
            }
        });
    </script>

<!--    <script src="skins/Skin_(!SKINID!)/bower_components/jquery/dist/jquery.min.js"></script>-->
    <script src="skins/Skin_(!SKINID!)/bower_components/foundation/js/foundation.min.js"></script>
    <script src="skins/Skin_(!SKINID!)/js/app.js"></script>
    <script>
        $(document).ready(function () {
            $('.flexslider').flexslider({
                animation: 'fade',
                controlsContainer: '.flexslider'
            });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            setupControls();
        });
        function setupControls() {
            //buttons
            $('.site-button').addClass('btn btn-info');
            $('#btnCheckOutNowTop').removeClass('btn-info');
            $('#btnCheckOutNowTop').addClass('btn-success');
            $('#btnCheckOutNowBottom').removeClass('btn-info');
            $('#btnCheckOutNowBottom').addClass ('btn-success');
            $('#btnUpateWishList1').removeClass('btn-info');
            $('#btnUpdateWishList1').addClass('btn-success');
            $('#btnUpateWishList2').removeClass('btn-info');
            $('#btnUpateWishList2').addClass('btn-success');
            $('input.addto').addClass('btn btn-info');
            //store language
            var showLanguage = false;
            var languageVisibility = '(!COUNTRYDIVVISIBILITY!)';
            var languageContainer = $('#storeLanguage');
            if (languageVisibility == 'visible') { showLanguage = true; }
            if (showLanguage) { languageContainer.show(); }
            //store vat
            var showVat = false;
            var vatVisibility = '(!VATDIVVISIBILITY!)';
            var vatContainer = $('#storeVAT');
            if (vatVisibility == 'visible') { showVat = true; }
            if (showVat) { vatContainer.show(); }
        }
    </script>

</body>
</html>