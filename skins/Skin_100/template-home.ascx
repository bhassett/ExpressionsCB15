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
        <meta name="google-site-verification" content="wfwmP0kKubQHIicagLLVMBF2Y1nS0YgB_u-7KDoROtI" />
        <meta name="p:domain_verify" content="c4f97936a90b44b833477f21a85b990a"/>
        
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
<!--
              <section class="top-bar-section">
                 <ul class="left">
                  <li><a href="/">Home</a></li>
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
-->
            </nav>
          </div>


    <!-- CONTENT -->
          <DIV class=row>
              <DIV class="small-12 columns">
                  <ul class="small-block-grid-1">
                      <li><IMG src="skins/Skin_(!SKINID!)/images/categories/category-home.jpg"></li>
                  </ul>
              </DIV>
          </DIV>
          
          <DIV class=grey-band-wrapper>
            <DIV class=row>
            <DIV class="small-12 columns">
            <H2 class=text-center>Popular Picks</H2>
            <UL class="small-block-grid-2 medium-block-grid-3 large-block-grid-4 text-center draw-border">
                <LI id="feature1"><a href="p-28579-turquoise-distress-glazed-victorian-style-ceramic-covered-jar.aspx"><IMG src="images/product/medium/UMA71231.jpg"><BR>Turquoise Victorian Ceramic Jar</a></LI>
                <LI id="feature2"><a href="p-28561-contemporary-black-metal-wall-clock-with-decorative-circular-mirrors.aspx"><IMG src="images/product/medium/item-67103-7.gif"><BR>Metal Wall Clock with Mirrors</a></LI>
                <LI id="feature3"><a href="p-29254-set-of-3-ceramic-chrome-mosiac-orbs.aspx"><IMG src="images/product/medium/UMA16156.jpg"><BR>Chrome Mosiac Orbs</a></LI>
                <LI id="feature4"><a href="p-29265-colorful-horse-canvas-wall-art.aspx"><IMG src="images/product/medium/UMA38515.jpg"><BR>Horse Canvas Wall Art</a></LI>
                <LI id="feature5"><a href="p-28189-blue-wood-cabinet-with-wicker-baskets.aspx"><IMG src="images/product/medium/UMA96181.jpg"><BR>Wood Cabinet With Wicker Baskets</a></LI>
                <LI id="feature6"><a href="p-27587-large-peacock-feather-painting-on-canvas-wall-art.aspx"><IMG src="images/product/medium/UMA92734.jpg"><BR>Peacock Feather Painting</a></LI>
                <LI id="feature7"><a href="p-28937-turquoise-glass-metal-contemporary-retro-style-table-lamp-30-h.aspx"><IMG src="images/product/medium/UMA40196.jpg"><BR>Turquoise Glass Retro Style Table</a></LI>
                <LI id="feature8"><a href="p-28566-modern-chrome-silver-glazed-ceramic-lantern-candle-holder.aspx"><IMG src="images/product/medium/UMA71700.jpg"><BR>Silver Glazed Ceramic Lantern</a></LI>
                <LI id="feature9" class="show-for-medium-only"><a href="p-9176-lime-green-psychedelic-ceramic-table-lamp-19-.aspx"><IMG src="images/product/medium/UMA40057.jpg"><BR>Lime Green Ceramic Table Lamp</a></LI>
            </UL></DIV></DIV>
            
            
            <DIV class=row>
              <DIV class="small-12 columns">
                  <ul class="small-block-grid-1 text-center">
                      <LI><IMG data-interchange="[skins/Skin_(!SKINID!)/images/home/free-shipping-480.jpg, (default)], [skins/Skin_(!SKINID!)/images/home/free-shipping-960.jpg, (medium)]"></LI>
                  </ul>
                  <hr />
              </DIV>
            </DIV>
            
<!--
            <DIV class=row>
                <DIV class="small-12 large-8 columns">
                    <DIV class=row>
                        <DIV class="small-12 columns text-center">
                            <UL class=small-block-grid-1>
                                <LI><IMG data-interchange="[http://placehold.it/380x220&amp;text=380x220, (default)], [http://placehold.it/640x435&amp;text=medium 640x435, (medium)]"></LI>
                            </UL>
                            <UL class="small-block-grid-1 medium-block-grid-2">
                                <LI><IMG src="http://placehold.it/380x220"></LI>
                                <LI><IMG src="http://placehold.it/380x220"></LI>
                            </UL>
                        </DIV>
                    </DIV>
                </DIV>
                <DIV class="large-4 columns show-for-large-up">
                    <h2>everything is blue</h2>
                </DIV>
            </DIV>
-->
                
            <DIV class="row paddingbottom">
                <DIV class="small-12 columns text-center">
<!--                    <H2 class=text-center>Popular Categories</H2>-->
                    <UL class="small-block-grid-2 medium-block-grid-4 text-center">
                        <LI><a href="c-12-decorative-accents.aspx"><IMG src="skins/Skin_(!SKINID!)/images/home/home-popcat-decorativeaccents.jpg"></a></LI>
                        <LI><a href="c-23-wall-art.aspx"><IMG src="skins/Skin_(!SKINID!)/images/home/home-popcat-wallart.jpg"></a></LI>
                        <LI><a href="c-8-vases.aspx"><IMG src="skins/Skin_(!SKINID!)/images/home/home-popcat-vases.jpg"></a></LI>
                        <LI><a href="c-25-table-lamps.aspx"><IMG src="skins/Skin_(!SKINID!)/images/home/home-popcat-lamps.jpg"></a></LI>
                    </UL>
                    <hr />
                </DIV>
            </DIV>
          
<!--
            <DIV class="row show-for-medium-up paddingbottom">
                <DIV class="medium-12 columns">
                    <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50dc8ab51b6823ca" async="async"></script>
                    <div class="addthis_recommended_horizontal"></div>

                    <ul class="small-block-grid-2 text-center">
                        <li><IMG src="http://placehold.it/456x316"></li>
                        <li><IMG src="http://placehold.it/456x316"></li>
                    </ul>
                </DIV>
            </DIV>
-->
          
<!--
            <DIV class=greybar-wrapper>
                <DIV class=row>
                    <DIV class="small-12 medium-4 columns">
                        <BLOCKQUOTE>
                        <H3>Gift &amp; Wedding Registry</H3>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam
                        </BLOCKQUOTE>
                    </DIV>
                    <DIV class="small-12 medium-4 columns">
                        <BLOCKQUOTE>
                        <H3>Trade Program</H3>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip
                        </BLOCKQUOTE>
                    </DIV>
                    <DIV class="small-12 medium-4 columns">
                        <BLOCKQUOTE>
                        <H3>Interior Design &amp; Merchandising Services</H3>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore</BLOCKQUOTE>
                    </DIV>    
                </DIV>
            </DIV>
-->
                            
            <div class="row">
                <div class="small-10 small-offset-1 medium-6 medium-offset-3 columns end">
                    <!-- Begin MailChimp Signup Form -->
                    <div id="mc_embed_signup" class="center">
                    <form action="//expressionsdecor.us12.list-manage.com/subscribe/post?u=8b087b547d0c16606c8e6893f&amp;id=1daa9b1596" method="post" id="mc-embedded-subscribe-form" name="mc-embedded-subscribe-form" class="validate" target="_blank" novalidate>
                        <h4>Subscribe to our mailing list</h4>
<!--                        <div class="indicates-required"><span class="asterisk">*</span> indicates required</div>-->
                        <div id="mc_embed_signup_scroll" class="row small-collapse search">
                            <div class="mc-field-group small-8 columns">
<!--                                <label for="mce-EMAIL">  <span class="asterisk">*</span></label>-->
                                <input type="email" value="" placeholder="Email Address *" name="EMAIL" class="required email" id="mce-EMAIL">
                            </div>
                            <div class="clear small-4 columns"><input type="submit" value="Subscribe" name="subscribe" id="mc-embedded-subscribe" class="button postfix"></div>
                            
                            <div id="mce-responses" class="clear">
                                <div class="response" id="mce-error-response" style="display:none"></div>
                                <div class="response" id="mce-success-response" style="display:none"></div>
                            </div>    <!-- real people should not fill this in and expect good things - do not remove this or risk form bot signups-->
                            <div style="position: absolute; left: -5000px;" aria-hidden="true"><input type="text" name="b_8b087b547d0c16606c8e6893f_1daa9b1596" tabindex="-1" value=""></div>
                        </div>
                    </form>
                    </div>
                    <!--End mc_embed_signup-->
                </div>
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
              <div class="small-12 medium-6 medium-pull-6 columns small-text-center medium-text-left copyright"><h6><small itemprop="brand" itemscope itemtype="http://schema.org/Brand">&#169; 2015 <span itemprop="name">Expressions by Decor &#38; More, Inc.</span> All rights reserved.<br><a href="t-termsandconditions.aspx">Terms of Use</a> <span>|</span> <a href="t-privacy.aspx">Privacy Policy</a></small></h6></div>
            </div>
              
            <div class="row show-for-large-up"><a href="#" class="back-to-top"><img src="/skins/Skin_(!SKINID!)/images/back-to-top.png" /></a></div>

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