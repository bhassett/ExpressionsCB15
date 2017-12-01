<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!--[if lt IE 7]><html class="lt-ie9 lt-ie8 lt-ie7" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 7]><html class="lt-ie9 lt-ie8" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 8]><html class="lt-ie9" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if gt IE 8]><!-->
<html xmlns="http://www.w3.org/1999/xhtml"><!--<![endif]-->
<head>
  <meta http-equiv="X-UA-Compatible" content="IE=10" />
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta name="viewport" content="width=device-width, minimum-scale=1.0, maximum-scale=1.0" />
  <title>(!METATITLE!)</title>
  <meta name="description" content="(!METADESCRIPTION!)" />
  <meta name="keywords" content="(!METAKEYWORDS!)" />
  <link rel="stylesheet" href="skins/Skin_(!SKINID!)/bootstrap/css/bootstrap.min.css" media="screen" />
  <link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" media="screen" />
  <link rel="stylesheet" href="skins/Skin_(!SKINID!)/font-awesome/css/font-awesome.min.css" type="text/css" />
  <script type="text/javascript" src="jscripts/jquery/jquery.min.v1.7.2.js"></script>
  <script type="text/javascript" src="jscripts/minified/bubble.message.js"></script>
  <script type="text/javascript" src="jscripts/minified/jquery.loader.js"></script>
  <script type="text/javascript" src="skins/Skin_(!SKINID!)/bootstrap/js/bootstrap.min.js"></script>
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
<asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
</body>
</html>