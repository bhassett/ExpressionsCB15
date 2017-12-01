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
        
        <!-- Facebook Conversion Code for Leads -->
        <script>(function() {
        var _fbq = window._fbq || (window._fbq = []);
        if (!_fbq.loaded) {
        var fbds = document.createElement('script');
        fbds.async = true;
        fbds.src = '//connect.facebook.net/en_US/fbds.js';
        var s = document.getElementsByTagName('script')[0];
        s.parentNode.insertBefore(fbds, s);
        _fbq.loaded = true;
        }
        })();
        window._fbq = window._fbq || [];
        window._fbq.push(['track', '6024881871249', {'value':'0.00','currency':'USD'}]);
        </script>
        <noscript><img height="1" width="1" alt="" style="display:none" src="https://www.facebook.com/tr?ev=6024881871249&amp;cd[value]=0.00&amp;cd[currency]=USD&amp;noscript=1" /></noscript>
        
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
            <a href="/"><h1 class="title">Expressions</h1></a>
          </section>

          <section class="right-small tab-bar-section">
            <a href="shoppingcart.aspx"><i class="fi-shopping-bag"></i></a>
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
          <div class="black-wrapper">
          <div class="row show-for-medium-up">
            <div class="medium-12 columns text-center"><a itemprop="brand" itemscope itemtype="http://schema.org/Brand" href="/"><img itemprop="logo" class="header-logo" src="skins/Skin_(!SKINID!)/images/global/expressions-logo.png" /></a></div>
          </div>
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
                  <li><a href="/">Home</a></li>
                  <li><a href="c-2-accessories.aspx">Accessories</a></li>
                  <li><a href="c-4-wall-decor.aspx">Wall Decor</a></li>
                  <li><a href="c-5-lighting.aspx">Lighting</a></li>
                  <li><a href="c-6-furniture.aspx">Accent Furniture</a></li>
<!--                  <li><a href="c-52-exclusive.aspx">Exclusive</a></li>-->
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
            </span>
        </div>
    </div>
    <!-- END OF CONTENT -->

    <!-- FOOTER -->

          <div class="grey-wrapper content-bottom">
            <div class="row pagefooterbar">
              <!-- (!LIVECHAT!) -->
              <div class="livechat small-6 medium-6 columns">
                <script type="text/javascript" src="https://support.expressionsdecor.com/CuteSoft_Client/CuteChat/Support-Image-Button.js.aspx"></script>
                <script type="text/javascript" src="https://support.expressionsdecor.com/CuteSoft_Client/CuteChat/Support-Visitor-monitor-crossdomain.js.aspx"
                id="LiveSupportVisitorMonitorScript"></script>
              </div>
                
              <div class="small-6 medium-6 columns text-right"><img src="skins/Skin_(!SKINID!)/images/global/billmethod-icons.png" /></div>
            </div>
          </div>

          <div class="black-wrapper">
            <div class="row pagefooter">
              <div class="small-12 medium-6 medium-push-6 columns small-text-center medium-text-right paddingbottom">
                
                <h6><small><a href="t-about.aspx">About Us</a> <span>|</span> <a href="t-contact.aspx">Contact Us</a></small></h6>
                
<!--                <h6><small><a href="t-about.aspx">About Us</a> <span>|</span> <a href="sitemap2.aspx">Site Map</a> <span>|</span> <a href="t-contact.aspx">Contact Us</a></small></h6>-->

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
              <div class="small-12 medium-6 medium-pull-6 columns small-text-center medium-text-left copyright"><h6><small itemprop="brand" itemscope itemtype="http://schema.org/Brand">&#169; 2017 <span itemprop="name">Expressions by Decor &#38; More, Inc.</span> All rights reserved.<br><a href="t-termsandconditions.aspx">Terms of Use</a> <span>|</span> <a href="t-privacy.aspx">Privacy Policy</a></small></h6></div>
            </div>
              
            <div class="row show-for-large-up"><a href="#" class="back-to-top"><img src="/skins/Skin_(!SKINID!)/images/back-to-top.png" /></a></div>

          </div>

        </section>

      <a class="exit-off-canvas"></a>

      </div>
    </div>
    
    <script>            
        jQuery(document).ready(function() {
            var offset = 220;
            var duration = 500;
            jQuery(window).scroll(function() {
                if (jQuery(this).scrollTop() > offset) {
                    jQuery('.back-to-top').fadeIn(duration);
                } else {
                    jQuery('.back-to-top').fadeOut(duration);
                }
            });

            jQuery('.back-to-top').click(function(event) {
                event.preventDefault();
                jQuery('html, body').animate({scrollTop: 0}, duration);
                return false;
            })
        });
    </script>


    <!-- END OF FOOTER -->

    <!-- BUBBLE MESSAGE -->
    <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div> 
            
    <!-- ADDRESS VERIFICATION -->
    (!ADDRESS_VERIFICATION_DIALOG_LISTING!)

    <!-- GOOGLE ANALYTICS -->
    <script>
      (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
      (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
      m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
      })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

      ga('create', 'UA-23764710-1', 'auto');
      ga('send', 'pageview');

    </script>
                
    <!-- AddThis -->
    <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50dc8ab51b6823ca" async="async"></script>

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