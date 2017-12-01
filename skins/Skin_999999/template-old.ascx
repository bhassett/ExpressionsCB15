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

  <link rel="stylesheet" href="skins/Skin_(!SKINID!)/additionalcss.css" type="text/css" />
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
    $('#Logout').find('.toshow').addClass('tohide');
     });
    </script>

    <script type="text/javascript">
    $(document).ready(function(){

      $(window).on('scroll',function() {
        var scrolltop = $(this).scrollTop();
     
        if(scrolltop >= 215) {
          $('#headertop').fadeIn(250);
          $('#headertop').addClass('navbar-fixed-top');
        }
        
        else if(scrolltop <= 210) {
          $('#headertop').removeClass('navbar-fixed-top');
        }

        if(scrolltop >= 700) {
          $('#bringtotop').addClass('toshow');
        }
        
        else if(scrolltop <= 700) {
           $('#bringtotop').removeClass('toshow');
        }
      });


      $(".grid .pix").hover(function(){
          $('.addtocart .btn').addClass('tohide');
          }, 
          function(){
          $('.addtocart .btn').addclass('tohide');
      }); 



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
   
<div id="(!SIGNINOUT_TEXT!)">
<header class="">
  <nav id="headertop" class="navbar-inverse ">
    <div id="defaultbutton" class="row  hidden-xs">
      <div id="toplogo" class="col-md-3 col-sm-3">
        <a href="default.aspx"><img class="logo-main" src="skins/Skin_(!SKINID!)/images/logo.png" alt="" /></a>
      </div>
       
      <div id="topnav" class="col-md-7 col-sm-8">
      (!XmlPackage Name="rev.topmenu"!)<!--<div id="menu_container" runat="server" class="hidden-sm hidden-xs"></div>--></div>

      <div id="sideicon" class="col-md-2 col-sm-1 text-right">
         <ul>
            <li>
                <button id="searchcollapse" class="btn btn-primary" type="button" data-toggle="collapse" data-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample"><i class="fa fa-search"></i></button>
                
            </li>
            <li><a href="(!SIGNINOUT_LINK!)" id="signInOutLink"><i class="icon-key"></i> <!--(!SIGNINOUT_TEXT!) --> </a> </li>
            <li><a id="shopping-cart" href="javascript:void(0)"><i class="icon-shopping-cart"></i> <span class="badge">(!NUM_CART_ITEMS!)</span></a>
                <div id="mini-cart"></div>
            </li>
         </ul>
         <div class="collapse" id="collapseExample">
            <div class="well">
                  (!XmlPackage Name="rev.search"!)
            </div>
          </div>                     
      </div>       
    </div>

    <div id="mobilenav" class="hidden-md hidden-lg hidden-sm col-xs-12">(!XmlPackage Name="rev.menumobile"!)(!XmlPackage Name="rev.search"!)</div>

    <div id="headerbottom">
      <div class="row">
        <div id="subnav" class="col-md-9 col-sm-8 col-xs-12">
          <ul>
            <li><a href="bestsellers.aspx"><i class="icon-tags"></i> <span class="labelhide">Bestsellers</span></a></li>
            <li><a href="recentadditions.aspx"><i class="icon-plus-sign"></i> <span class="labelhide">Recent Addition</span></a></li>
            <span runat="server">
                <% if (InterpriseSuiteEcommerceCommon.AppLogic.AppConfigBool("GiftRegistry.Enabled"))
                   { %>
                <li><a href="giftregistry.aspx"><i class="icon-gift"></i> <span class="labelhide">Gift Registry</span></a></li>
                <% } %>
            </span>
            <li><a href="storelocator.aspx"><i class="icon-map-marker"></i> <span class="labelhide">Store Locator</span></a></li>
            <li><a href="wishlist.aspx"><i class="icon-star"></i> <span class="labelhide">Wishlist</span></a></li>
          </ul>
        </div>
        <div class="col-md-3 col-sm-4 col-xs-12 usernametop">
          <div class="toshow">Start shopping when you Login</div>
          (!USERNAME!)
        </div>
      </div>
    </div>
  </nav>
</header>
</div>



  <div id="bread" >
    <div class="row">
      <ol class="breadcrumb">
        <li><a href="default.aspx"><i class="fa fa-home"></i></a></li>
        <li class="active">(!SECTION_TITLE!)</li>
      </ol>
    </div>
  </div>

  

  <div class="row">

  <div class="col-md-12 col-sm-12 main-container">    
    <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
  </div>
</div>



<footer>
  <div id="footertop">
    <div class="row">
      <div id="footerlinks" class="col-md-4 col-sm-4 col-xs-12">
        <div class="col-md-6 col-sm-6 col-xs-6">
          <div class="header">Information</div>
          <ul>
              <li><a href="default.aspx">Home</a></li>
              <li><a href="contactus.aspx">Contact Us</a></li>
              <li><a href="casehistory.aspx">Customer Support</a></li>
              <li><a href="sitemap2.aspx">Sitemap</a></li>
              <li><a href="t-faq.aspx">FAQs</a></li>
              <li><a href="t-returns.aspx">Return Policy</a></li>
              <li><a href="t-privacy.aspx">Privacy Policy</a></li>
              <li><a href="t-security.aspx">Security Policy</a></li>
              <li><a href="t-shipping.aspx">Shipping Info</a></li>
          </ul>
        </div>
        <div class="col-md-6 col-sm-6 col-xs-6">
          <div class="header">Services</div>
          <ul>
              <li><a href="storelocator.aspx">Store Locator</a></li>
              <li><a href="giftregistry.aspx">Gift Registry</a></li>
              <li><a href="wishlist.aspx">Wishlist</a></li>
              <li><a href="leadform.aspx">Lead Form</a></li>
          </ul>
        </div>
      </div>
      <div id="footersocial" class="col-md-4 col-sm-4 col-xs-12">
        <div id="socialmedia" class="col-md-12 col-sm-12 col-xs-12">
          <div class="header">Follow US</div>
          <ul>
              <li><a href="www.facebook.com" title="Connect with us on Facebook"><i class="fa fa-facebook"></i></a></li>
              <li><a href="www.twitter.com" title="Connect with us on Twitter"><i class="fa fa-twitter"></i></a></li>
              <li><a href="www.instagram.com" title="Connect with us on Instagram"><i class="fa fa-instagram"></i></a></li>
              <li><a href="www.pinterest.com" title="Connect with us on Pinterest"><i class="fa fa-pinterest"></i></a></li>
              <li><a href="www.rss.com" title="Connect with us on RSS"><i class="fa fa-rss"></i></a></li>
          </ul>
        </div>
        <div id="payment" class="col-md-12 col-sm-12 col-xs-12">
          <div class="header">Payment</div>
          <ul>
              <li><a href="/shoppingcart.aspx" title="Shop With Us"><img class="paymentmethod" src="skins/Skin_(!SKINID!)/images/payment.png" alt="" /></a></li>
          </ul>
        </div>
      </div>
      <div class="col-md-4 col-sm-4 col-xs-12">
        <div class="header">We'd love to hear from you</div>          
        <div class="contactinfo">
          <ul>
              <li>If you have inquiry or question please don’t hesitate to send us an email or simply contact us.  We are glad to help you.</li>
              <li id="iconcontant"><a href="callto://+123456789"><i class="fa fa-phone"></i> 123.456.789</li>
              <li id="iconcontant"><a href="mailto:sales@connectedbusiness.com?Subject=Product%20Inquiry" target="_top"><i class="fa fa-envelope"></i> sales@connectedbusiness.com</a></li>
              <li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.</li>
        </div>      
      </div>
    </div>
  </div>  
  <div id="footerbottom">
    <div class="row" id="copyright">Copyright &copy; (!COPYRIGHTYEARS!). All Rights Reserved.</DIV>
  </div>

 <!-- BUBBLE MESSAGE -->
  <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div>
  <!-- ADDRESS VERIFICATION -->
  (!ADDRESS_VERIFICATION_DIALOG_LISTING!)
</footer>

<div id="bringtotop" ><a href="#"><i class="fa fa-angle-up"></i></a></div>



</body>
</html>