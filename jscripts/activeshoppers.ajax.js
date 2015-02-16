$(".generate-link").click(function () {
    InitRequestCode();
});
function InitRequestCode() {
    $(".request-generator-content").hide();
    $("#imgLoader").show();
    $(".generate-link").hide();
    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GenerateRequestCode",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            $(".request-code").text(result.d);
            $(".request-generator-content").show();
            $("#imgLoader").hide();
            $(".generate-link").show();
            $('.copy-link').zclip({
                path: 'images/ZeroClipboard.swf',
                copy: function () { return $(".request-code").text() }
            });
        },
        error: function (result, textStatus, exception) {
        }
    });
}
InitRequestCode();