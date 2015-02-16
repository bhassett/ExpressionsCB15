$("document").ready(function () {

    var elements = getQueryString()["el"];
    var aids = getQueryString()["aid"];
    var gids = getQueryString()["gid"];
    var guid = getQueryString()["guid"];

    var defaultpage = getQueryString()["dfp"];

    var pathArray = window.location.pathname.split('/');

    updateAttributeUrls(aids, gids, elements, guid);

    if (aids != null) {

        var counter = 0;
        var size = $(".selected-attributes").size();

        $(".selected-attributes").each(function () {

            var thisId = $(this).attr("id");
            thisId = thisId.split("::");

            var myAttributeId = thisId[1];


            $(this).addClass("selected-" + counter);
            $(this).attr("onclick", "removeThisSelection(" + myAttributeId + ", " + counter + ")");

            counter++;

        });


        if (counter == size && size > 1) {

            $("#remove-all").attr("href", defaultpage + ".aspx");

        } else {

            $("#remove-all").css("display", "none");
        }


    } else {

        $("#selections").css("display", "none");
    }


    var items = $(".attributes a").size();

    if (items == 0) {

        $("#attributes-listing").css("display", "none");

    }

});


function updateAttributeUrls(aids, gids, elements, guids) {

    var attributeIds = "";
    var _gids = "";
    var _guids = "";
    var appendToThisIndex = "";
    var indexes = "";
    var defaultpage = "";
    var imProduct = "";

    if (aids != null) {

        attributeIds = "-" + aids;
        _gids = "-" + gids;
        _guids = "_" + guids;

        defaultpage = getQueryString()["dfp"];

    } else {
        var pathname = location.pathname.slice(1);
        

        var activeAspx = pathname;
        activeBaseAspx = activeAspx;
        defaultpage = activeAspx.split(".");
        defaultpage = defaultpage[0];

        imProduct = defaultpage.split("-");
        imProduct = imProduct[0];

    }


    var gid = "";
    var guid = "";
    var _id = "";
    var aid = "";

    $(".attributes a").each(function () {

        var aspx = $(this).attr("href");
        var thisId = $(this).attr("id");

        var parent = thisId.split("#");

        if (parent[0] != "parent" && parent[0] != "selected") {

            _id = thisId.split("::");
            gid = _id[1];
            guid = _id[2];

            aid = aspx.split("-");

            var thisElementIndex = $(".attributes a").index(this);

            if (elements != null) {

                appendToThisIndex = "-" + elements;
            }
            var newUrl = aspx;

            if (imProduct == "p") {

                defaultpage = getDefaultPageIfProduct(aspx);

            }

            newUrl += "&aid=" + aid[1] + attributeIds;
            newUrl += "&gid=" + gid + _gids;
            newUrl += "&guid=" + guid + _guids;
            newUrl += "&el=" + thisElementIndex + appendToThisIndex;
            newUrl += "&dfp=" + defaultpage;



            $(this).attr("href", newUrl);
            $(this).addClass("attribute-a-" + thisElementIndex);



        } else {

            var thisElementIndex = $(".attributes a").index(this);
            $(this).addClass("attribute-a-" + thisElementIndex);


        }

    });


}

function getDefaultPageIfProduct(aspx) {

    var defaultpage = "";

    var urlParam = aspx.split("?");
    urlParam = urlParam[1];
    urlParam = urlParam.split("&");

    var eName = urlParam[1];
    eName = eName.split("=");
    eName = eName[1];

    var atr = urlParam[2];
    atr = atr.split("=");
    atr = atr[1];

    if (eName == "Category") {

        eName = "c";

    } else {

        eName = "d";
    }

    defaultpage = eName + "-" + atr;

    return defaultpage;
}

function removeThisSelection(attribute, index) {

    var selections = getQueryString()["aid"];
    var gids = getQueryString()["gid"]
    var guids = getQueryString()["guid"];

    var elements = getQueryString()["el"];


    var newSelections = "";
    var newElements = "";
    var newGIDS = "";
    var newGUIDS = "";

    if (elements != null) {

        var ss = selections.split("-");
        var yy = gids.split("-");
        var gg = guids.split("_");
        var xx = elements.split("-");

        var s = []; var y = []; var g = []; var x = [];

        //s.reverse(); y.reverse(); g.reverse(); x.reverse();
        if (ss.length > 1) {

            var sortingIDs = [];


            $(".selected-attributes").each(function () {

                var thisId = $(this).attr("id");
                thisId = thisId.split("::");

                var myAttributeId = thisId[1];

                if (myAttributeId != undefined) {
                    sortingIDs.push(myAttributeId);
                }

            });

            // get sorting refer to element order
            var sortingIndexs = [];
            for (indx = 0; indx < sortingIDs.length; ++indx) {
                var sortindex = ss.indexOf(sortingIDs[indx]);
                if (sortingIDs.length > sortindex) {
                    sortingIndexs.push(sortindex);
                }
            }

            // assign new values base on index
            for (indx = 0; indx < sortingIndexs.length; ++indx) {
                s.push(ss[parseInt(sortingIndexs[indx])]);
                y.push(yy[parseInt(sortingIndexs[indx])]);
                g.push(gg[parseInt(sortingIndexs[indx])]);
                x.push(xx[parseInt(sortingIndexs[indx])]);
            }


        }

        for (var i = 0; i < s.length; i++) {

            if (index != i) {

                newSelections += s[i] + "-";
                newElements += x[i] + "-";
                newGIDS += y[i] + "-";
                newGUIDS += g[i] + "_";
            }
        }

    }


    var imNext = "";

    if (index == 0 && s.length > 1) {

        imNext = $(".selected-1").attr("id");
        imNext = imNext.split("::");
        imNext = imNext[1] + "-" + imNext[2];

    }

    if (index > 0 && index <= s.length) {

        imNext = $(".selected-0").attr("id");
        imNext = imNext.split("::");
        imNext = imNext[1] + "-" + imNext[2]; ;
    }


    var str = location.pathname;
    str = str.split("/");

    var defaultpage = getQueryString()["dfp"];

    var queryString = "?EntityID=" + getQueryString()["EntityID"] + "&EntityName=" + getQueryString()["EntityName"];
    queryString += "&aid=" + newSelections.substr(0, newSelections.length - 1);
    queryString += "&gid=" + newGIDS.substr(0, newGIDS.length - 1);
    queryString += "&guid=" + newGUIDS.substr(0, newGUIDS.length - 1);
    queryString += "&el=" + newElements.substr(0, newElements.length - 1);
    queryString += "&dfp=" + defaultpage;

    var landingPage = "/a-" + imNext + ".aspx" + queryString

    //check if has domain
    if (imNext != '' && str.length == 2) {
        landingPage = "/a-" + imNext + ".aspx" + queryString
    }

    // no more selected attribute
    if ((imNext == '' && s.length == 0) && str.length > 1) {
        var url = defaultpage.split("/");

        var activeAspx = url[url.length - 1];

        landingPage = "/" + activeAspx + ".aspx";
    }

    if ((index == 0 && s.length == 1) && str.length > 1) {
        landingPage = str[1] + "/" + defaultpage + ".aspx";
    }

    //check if has domain and no return attribute
    if (imNext == '' && str.length == 2) {
        landingPage = defaultpage + ".aspx";
    }


    //check if local host
    if (str[1] != null && str.length > 2) {

        var url = defaultpage.split("/");

        var activeAspx = url[url.length - 1];
        window.location = "/" + defaultpage.replace("/" + activeAspx, landingPage);
    } else {
        window.location = landingPage;
    }

}


function getQueryString() {

    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');

    for (var i = 0; i < hashes.length; i++) {

        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];

    }
    return vars;
}