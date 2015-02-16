$(document).ready(function () {

    // load all required templates
    $.get("components/order-history/templates/jquery.orderhistory.tmpl.min.html", function (data, textStatus, XMLHttpRequest) {
        $("body").append(data);

        // load order history plugin cs and call the set up function
        $.getScript("components/order-history/jquery.orderhistory.plugin.min.js").done(function (script, textStatus) {
            $(this).OrderHistory.setup({ "stringResourceKeys": orderHistoryPluginStringKeys });
        }).fail(function (jqxhr, settings, exception) { alert(exception); });
    });

});