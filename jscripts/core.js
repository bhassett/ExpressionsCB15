function $add_windowLoad(handler) 
{
    if ($.browser.msie && parseInt($.browser.version.slice(0, 3)) == 8) {
        $(window).load(handler);
    }
    else if ($.browser.msie && parseInt($.browser.version.slice(0, 3)) == 10) {
        $(document).ready(handler);
    } else {
        if (window.addEventListener) {
            window.addEventListener('load', handler, false);
        }
        else if (document.addEventListener) {
            document.addEventListener('load', handler, false);
        }
        else if (window.attachEvent) {
            window.attachEvent('onload', handler);
        }
        else {
            if (typeof window.onload == 'function') {
                var oldload = window.onload;
                window.onload = function () {
                    oldload();
                    handler();
                }
            }
            else { window.onload = init; }
        }
    }
}

function $getElement(el) {
    return typeof (el) == 'string' ? $get(el) : el;
}

function $enableSubmit(frm) {
    __doEnableSubmit(frm, true);
}

function $disableSubmit(frm) {
    __doEnableSubmit(frm, false);
}

function __doEnableSubmit(frm, enable) {
    var form = $getElement(frm);
    if (form) {
        for (var ctr = 0; ctr < form.length; ctr++) {
            var inpt = form[ctr];
            if (inpt.type.toLowerCase() == 'submit' ||
               inpt.type.toLowerCase() == 'reset') {
                inpt.disabled = !enable;
            }
        }
    }
}

var parseBool = function (str) {
    if (typeof str === 'string' && str.toLowerCase() == 'true')
        return true;

    return (parseInt(str) > 0);
}

var trimString = function (str) {
    return (typeof (str) == "undefined") ? "" : $.trim(str);
}

/*!
* Cross-Browser Split 1.1.1
* Copyright 2007-2012 Steven Levithan <stevenlevithan.com>
* Available under the MIT License
* ECMAScript compliant, uniform cross-browser split method
*/
/**
* Splits a string into an array of strings using a regex or string separator. Matches of the
* separator are not included in the result array. However, if `separator` is a regex that contains
* capturing groups, backreferences are spliced into the result each time `separator` is matched.
* Fixes browser bugs compared to the native `String.prototype.split` and can be used reliably
* cross-browser.
* @param {String} str String to split.
* @param {RegExp|String} separator Regex or string to use for separating the string.
* @param {Number} [limit] Maximum number of items to include in the result array.
* @returns {Array} Array of substrings.
* @example
*
* // Basic use
* split('a b c d', ' ');
* // -> ['a', 'b', 'c', 'd']
*
* // With limit
* split('a b c d', ' ', 2);
* // -> ['a', 'b']
*
* // Backreferences in result array
* split('..word1 word2..', /([a-z]+)(\d+)/i);
* // -> ['..', 'word', '1', ' ', 'word', '2', '..']
*/
var split;
// Avoid running twice; that would break the `nativeSplit` reference
split = split || function (undef) {
    var nativeSplit = String.prototype.split,
compliantExecNpcg = /()??/.exec("")[1] === undef, // NPCG: nonparticipating capturing group
self;
    self = function (str, separator, limit) {
        // If `separator` is not a regex, use `nativeSplit`
        if (Object.prototype.toString.call(separator) !== "[object RegExp]") {
            if (str != undefined) {
                return nativeSplit.call(str, separator, limit);
            }
            return []
        }
        var output = [],
flags = (separator.ignoreCase ? "i" : "") +
(separator.multiline ? "m" : "") +
(separator.extended ? "x" : "") + // Proposed for ES6
(separator.sticky ? "y" : ""), // Firefox 3+
lastLastIndex = 0,
        // Make `global` and avoid `lastIndex` issues by working with a copy
separator = new RegExp(separator.source, flags + "g"),
separator2, match, lastIndex, lastLength;
        str += ""; // Type-convert
        if (!compliantExecNpcg) {
            // Doesn't need flags gy, but they don't hurt
            separator2 = new RegExp("^" + separator.source + "$(?!\\s)", flags);
        }
        /* Values for `limit`, per the spec:
        * If undefined: 4294967295 // Math.pow(2, 32) - 1
        * If 0, Infinity, or NaN: 0
        * If positive number: limit = Math.floor(limit); if (limit > 4294967295) limit -= 4294967296;
        * If negative number: 4294967296 - Math.floor(Math.abs(limit))
        * If other: Type-convert, then use the above rules
        */
        limit = limit === undef ?
-1 >>> 0 : // Math.pow(2, 32) - 1
limit >>> 0; // ToUint32(limit)
        while (match = separator.exec(str)) {
            // `separator.lastIndex` is not reliable cross-browser
            lastIndex = match.index + match[0].length;
            if (lastIndex > lastLastIndex) {
                output.push(str.slice(lastLastIndex, match.index));
                // Fix browsers whose `exec` methods don't consistently return `undefined` for
                // nonparticipating capturing groups
                if (!compliantExecNpcg && match.length > 1) {
                    match[0].replace(separator2, function () {
                        for (var i = 1; i < arguments.length - 2; i++) {
                            if (arguments[i] === undef) {
                                match[i] = undef;
                            }
                        }
                    });
                }
                if (match.length > 1 && match.index < str.length) {
                    Array.prototype.push.apply(output, match.slice(1));
                }
                lastLength = match[0].length;
                lastLastIndex = lastIndex;
                if (output.length >= limit) {
                    break;
                }
            }
            if (separator.lastIndex === match.index) {
                separator.lastIndex++; // Avoid an infinite loop
            }
        }
        if (lastLastIndex === str.length) {
            if (lastLength || !separator.test("")) {
                output.push("");
            }
        } else {
            output.push(str.slice(lastLastIndex));
        }
        return output.length > limit ? output.slice(0, limit) : output;
    };
    // For convenience
    String.prototype.split = function (separator, limit) {
        return self(this, separator, limit);
    };
    return self;
} ();

function jqueryHideShow(id, linkId, showText, hideText) {
    if ($(id).css("display") == 'none') {
        $(id).show('fast', function () {
            if (hideText != 'undefined' && hideText != '') {
                $(linkId).find('span').text(hideText)
            }
        });
    } else {
        $(id).hide('fast', function () {
            if (showText != 'undefined' && showText != '') {
                $(linkId).find('span').text(showText);
            }
        })
    }
}

function AjaxCallWithSecuritySimplified(methodName, data, successFunction, errorFunction) {
    AjaxCallWithSecurity('', '', methodName, '', '', data, successFunction, errorFunction);
}

function AjaxCallWithSecuritySynchronous(methodName, data, successFunction, errorFunction) {
    AjaxCallWithSecurity('', '', methodName, '', '', data, successFunction, errorFunction, false);
}

function AjaxCallCommon(methodName, data, successFunction, errorFunction) {

    var param = (data != null) ? JSON.stringify(data) : null;

    $.ajax({
        type: "POST",
        url: methodName,
        data: param,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: successFunction,
        error: errorFunction
    });

}

function AjaxCallCommonRest(methodName, type, data, successFunction, errorFunction) {

    var param = (data != null) ? JSON.stringify(data) : null;

    $.ajax({
        type: type,
        url: methodName,
        data: param,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: successFunction,
        error: errorFunction
    });

}

function AjaxCallWithSecurity(url, type, methodName, dataType, contentType, data, successFunction, errorFunction, asynchronous) {

    if (type == '' || type == undefined) { type = 'POST'; }

    if (dataType == '' || dataType == undefined) { dataType = 'json'; }

    if (url == '' || url == undefined) {
        url = 'ActionService.asmx/' + methodName;
    }

    if (contentType == '' || contentType == undefined) {
        contentType = "application/json; charset=utf-8";
    }

    var token = ise.Configuration.getConfigValue("Service.Token");
    if (token == '' || token == undefined) {
        alert('Error in security token: ' + 'Empty');
        return;
    }

    if (errorFunction == null || errorFunction == undefined) {
        errorFunction = ErrorHandler;
    }

    if (asynchronous == null) {
        asynchronous = true;
    }

    var ajaxOptions = {
        type: type,
        url: url,
        dataType: dataType,
        async: asynchronous,
        contentType: contentType,
        data: JSON.stringify(data),
        beforeSend: function (req) {
            req.setRequestHeader("TOKEN", token);
        },
        success: successFunction,
        error: errorFunction
    };

    $.ajax(ajaxOptions);
}

function AjaxSecureEmailAddressVerification(initializeRequest, emailAddress, accountType, successFunction, errorFunction) {

    var data = new Object();

    data.emailAddress = emailAddress;
    data.initializeRequest = initializeRequest;
    data.accountType = accountType;

    AjaxCallWithSecurity("", "", "ValidateEmailAddress", "", "", data, successFunction, errorFunction);

    data = null;

}

function ErrorHandler(result, textStatus, errorThrown) {
    var errorObject = $.parseJSON(result.responseText);
    //alert(errorObject.Message);
}

function IsInArray(strArray, strValue) {
    return $.inArray($.trim(strValue), strArray) >= 0;
}

$.cachedScript = function (url, options) {
    // allow user to set any option except for dataType, cache, and url
    options = $.extend(options || {}, {
        dataType: "script",
        cache: true,
        url: url
    });
    // Use $.ajax() since it is more flexible than $.getScript
    // Return the jqXHR object so we can chain callbacks
    return jQuery.ajax(options);
};

Array.prototype.insert = function (index) {
    index = Math.min(index, this.length);
    arguments.length > 1
        && this.splice.apply(this, [index, 0].concat([].pop.call(arguments)))
        && this.insert.apply(this, arguments);
    return this;
};

var jqueryBasePlugin = function () {

    this.toggleMenu = function (elem, tglClass) {
        $(elem).toggleClass(tglClass)
          .siblings(".collapse")
          .collapse("toggle");
    },

    this.selectorChecker = function (selector) {
        if (selector == '') return selector;
        if (selector.indexOf(".") == -1) {
            selector = "#" + selector;
        }
        return selector;
    },

    this.getElemBySelChecker = function (selector)
    {
        return $(this.selectorChecker(selector));
    },

    this.getTimeStamp = function () {
        return "?timestamp=" + new Date().getTime();
    },

    this.downloadCss = function (cssPath, callbackkSucess) {

        if ($('head').find("link[href*='" + cssPath + "']").length == 0) {
            $('head').append("<link href='" + cssPath + this.getTimeStamp() + "' rel='stylesheet' type='text/css' />");
        }

        if (typeof callbackkSucess != 'undefined') callbackkSucess();
    },

    this.removeCssReference = function (cssPath, callbackkSucess) {

        if (typeof cssPath == 'undefined') return;

        $('head').find("link[href*='" + cssPath + "']").remove();
        if (typeof callbackkSucess != 'undefined') callbackkSucess();
    },

    this.downloadPlugin = function (pluginPath, callbackkSucess, callbackError) {

        if (typeof pluginPath == 'undefined') {
            var message = "not found";
            if (typeof callbackkSucess != 'undefined') callbackError(message);
        }

        $.getScript(pluginPath)
            .done(function (script, textStatus) { //execute callback after the download done

                if (typeof callbackkSucess === "function") callbackkSucess();

            }).fail(function (jqxhr, settings, exception) { //execute callback after the download failed 

                if (typeof callbackError !== "function") {
                    alert('unable to load the plugin please check the source. \n\n Error message: ' + exception);
                }
                else {
                    callbackError(exception);
                }
                
            });

        return this;
    },

    this.downloadCachePlugin = function (pluginPath, callbackkSucess) {
        $.cachedScript(pluginPath)
            .done(function (script, textStatus) { //execute callback after the download done
                if (typeof callbackkSucess != 'undefined') callbackkSucess();
            }).fail(function (jqxhr, settings, exception) { //execute callback after the download failed 
                alert('unable to load the plugin please check the source. \n\n Error message: ' + exception.message);
            });
    },

    this.parseTemplate = function (templateId, data) {
        return $.tmpl(templateId, data);
    },

    this.parseTemplateReturnHtml = function (templateId, data) {
        return $(this.parseTemplate(templateId, data)).html();
    },

    this.loadStringResource = function (key, callBack) {
        var keys = new Array();
        keys.push(key);
        ise.StringResource.loadResources(keys, callBack);
    },

    this.parseJqueryTemplate = function (templateId, data) {
        return $.trim($(this.selectorChecker(templateId)).tmpl(data).html());
    },

    this.downloadStringResources = function (keys, callBack) {
        ise.StringResource.loadResources(keys, callBack);
    },

    this.downloadAppConfigs = function (keys, callBack) {
        ise.Configuration.loadResources(keys, callBack);
    },

    this.getString = function (key) {
        return ise.StringResource.getString(key);
    },

    this.getAppConfig = function (key) {
        return ise.Configuration.getConfigValue(key);
    },

    this.getAppConfigBool = function (key) {
        return this.toBoolean(ise.Configuration.getConfigValue(key));
    },

    this.toBoolean = function (value) {
        return (value.toLowerCase() == "true");
    },

    this.getDigitalDateFormat = function (date) {

        var d = new Date(date);
        if (isNaN) {
            return '\/Date(' + new Date().getTime() + ')\/';
        }

        return '\/Date(' + d.getTime() + ')\/';

    },

    this.ajaxRequest = function (methodName, data, callBack, failedCallBack) {
        AjaxCallCommon(methodName, data, callBack, failedCallBack);
    },

    this.ajaxRequestRest = function (methodName, type, data, callBack, failedCallBack) {
        AjaxCallCommonRest(methodName, type, data, callBack, failedCallBack);
    },

    this.ajaxSecureRequest = function (methodName, data, callBack, failedCallBack) {
        AjaxCallWithSecuritySimplified(methodName, data, callBack, failedCallBack);
    },

    this.isEmpty = function (value) {

        if (typeof (value) == "undefined" || value == null || value == "") return true;

        if (value.length > 0) {
            if ($.inArray("", value) < 0) {
                return false;
            } else {
                return true;
            }
        }

        return false;
    },

    this.isInputControlUndefined = function (selector) {
        var value = typeof ($(this.selectorChecker(selector)).val());
        if (value == "undefined" || value == undefined) return true;
        return false;
    },

    this.clearInputControlValue = function (selector) {
        $(this.selectorChecker(selector)).val("");
    },

    this.setInputControlValue = function (selector, value) {
         $(this.selectorChecker(selector)).val($.trim(value));
     },

    this.getInputControlValue = function (selector) {

        if (this.isEmpty(selector)) {
            return '';
        }

        return $.trim($(this.selectorChecker(selector)).val());

     },

    this.setElementHTMLContent = function (selector, content) {
         $(this.selectorChecker(selector)).html($.trim(content));
     },

    this.hideElement = function (selector) {
         $(this.selectorChecker(selector)).addClass("display-none");
     },

    this.removeClass = function (selector, className) {
         $(this.selectorChecker(selector)).removeClass(className);
     },

    this.addClass = function (selector, className) {
         $(this.selectorChecker(selector)).addClass(className);
     },

    this.convertObjectToArray = function (object) {
        return $.map(object, function (k, v) { return [k]; });
    },

    this.showLoader = function () {
        $("body").data("globalLoader").show();
    }

    this.hideLoader = function (callback) {
        $("body").data("globalLoader").hide(callback);
    },

    this.ArrayInsertAt = function(array, index, args) {
        array.insert(index, args);
        return array;
    },

    this.ToJsonObject = function (jsonString) {
        try {
            return $.parseJSON(jsonString);
        } catch (e) {
            return null;
        }
    },

    this.CreateJqueryDelegate = function (evnt, thisObject) {
        return $.proxy(evnt, thisObject);
    },

    this.calculateDistance = function (lat1, lon1, lat2, lon2) {
        
        var radius = 6371;
        var radiance = 3.1459 / 180;
        var inMiles = 0.621371192;

        var latitude1 = parseFloat(lat1);
        var latitude2 = parseFloat(lat2);

        var longtitude1 = parseFloat(lon1);
        var longtitude2 = parseFloat(lon2);

        var dLat = (latitude2 - latitude1) * radiance;
        var dLon = (longtitude2 - longtitude1) * radiance;

        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(lat1 * radiance) *
                Math.cos(lat2 * radiance) * Math.sin(dLon / 2) * Math.sin(dLon / 2);

        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

        return radius * c * inMiles;

    },

    this.getQueryStringParamByName = function(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    },

    this.updateQueryStringParams = function (params) {
        var queryParams = {};
        var queryString = location.search.substring(1);
        var regex = /([^&=]+)=([^&]*)/g;
        var m;

        //map querystring parameters
        while (m = regex.exec(queryString)) {
            queryParams[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
        }

        //add or update parameters
        $.each(params, function (key, value) {
            queryParams[key] = value;
        });

        //reload page
        location.search = $.param(queryParams); 
    },

    this.generateRandomNumber = function (length, combination) {
        
        if (length && length == 0) return length;
        if (combination && combination == 0) return combination;

        var result = '';
        for (var i = 0; i < length; i++) {
            result = result + Math.floor(Math.random() * combination);
        }

        return result;
    },

    this.formatString = function(format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    }

}

//JSO2.js // for IE 7 and below
if ($.browser.msie && parseInt($.browser.version, 10) < 8) {
    $.getScript("jscripts/jquery/json2.js").done(function (script, textStatus) {
    }).fail(function (jqxhr, settings, exception) { alert(exception); });

}