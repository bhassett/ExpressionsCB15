$(document).ready(function () {

    // load all required templates
    $.get("components/open-invoices/templates/jquery.openinvoices.tmpl.min.html", function (data, textStatus, XMLHttpRequest) {
        $("body").append(data);

        // load order history plugin cs and call the set up function
        $.getScript("components/open-invoices/jquery.openinvoices.plugin.min.js").done(function (script, textStatus) {
            $(this).OpenInvoices.setup({ "stringResourceKeys": openInvoicesPluginStringKeys });
        }).fail(function (jqxhr, settings, exception) { alert(exception); });
    });

});