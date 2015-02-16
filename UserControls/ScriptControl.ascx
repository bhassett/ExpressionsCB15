<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScriptControl.ascx.cs" Inherits="UserControls_ScriptControl" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<%-- Start MaxMind Api Script --%>

    <% if (ShowMaxMind == true) { %>

    <script type="text/javascript">

        maxmind_user_id = <%= AppLogic.AppConfig("MaxMind.UserID") %>;
        (function () {
            var mt = document.createElement('script'); mt.type = 'text/javascript'; mt.async = true;
            mt.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'device.maxmind.com/js/device.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(mt, s);
        })();

    </script>

    <% }  %>

<%-- End MaxMind Api Script --%>


<%-- Start Google Map Api Script --%>

    <% if (ShowGoogleMapApi == true) { %>

    <script type="text/javascript" src="//maps.googleapis.com/maps/api/js?key=<%= AppLogic.AppConfig("Storelocator.GoogleAPIKey") %>&sensor=false&v=3.8"></script>

    <% }  %>

<%-- End Google Map Api Script --%>