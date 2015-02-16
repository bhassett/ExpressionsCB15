/// <reference path="../core.js" />
(function ($) {

	var config = {};
	var thisObject;

	var pluginConstants = {
		EMPTY_VALUE: '',
		DOT_VALUE: '.',
		ZERO_VALUE: 0
	}

	var defaults = {
	    rateTemplateID: 'RatingStarTemplate',
	    SkinId: 99,
        Rating: 0
	};

	var global = {
		selected: '',
		selector: '',
		originalElem: ''
	};

	var pluginId = '';

	var init = $.prototype.init;
	$.prototype.init = function (selector, context) {

		var r = init.apply(this, arguments);

		if (selector && selector.selector) {
			r.context = selector.context, r.selector = selector.selector;
		}

		if (typeof selector == 'string') {
			r.context = context || document, r.selector = selector;
			global.selector = r.selector;
		}

		global.selected = r;
		return r;
	}

	$.prototype.init.prototype = $.prototype;

	$.fn.RateStar = {

		setup: function (optionalConfig) {

		    global.originalElem = global.selected;
		    thisObject = this;

			if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

			var config = getConfig();

			$(global.originalElem).each(function () {

			    thisObject.Display(this);

			});
		  
		},

		Display : function(thisContainer) {
			
			var config = getConfig();
			var param = new Object();
			param.Rating = config.Rating;
			param.SkinId = config.SkinId;

			var html = thisObject.parseTemplateReturnHtml(config.rateTemplateID, param);
			$(thisContainer).append(html);
			
    		return this;
		},

		clear: function () {
			return this;
		},

		config: function (args) {
			setConfig($.extend(defaults, args));
			return (getConfig());
		}

	};

	//jqueryBasePlugin is located inside core.js
	$.extend($.fn.RateStar, new jqueryBasePlugin());

	function setConfig(value) {
		config = value;
	}

	function getConfig() {
		return config;
	}

})(jQuery);

$.template(
        "RatingStarTemplate",
        "<div>" +
            "{{if (Rating < 0.25) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 0.25 && Rating < 0.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 0.75 && Rating < 1.25) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 1.25 && Rating < 1.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 1.75 && Rating < 2.25) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 2.25 && Rating < 2.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 2.75 && Rating < 3.25) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 3.25 && Rating < 3.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 3.75 && Rating < 4.25) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 4.25 && Rating < 4.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +

            "{{else (Rating >= 4.75) }}" +

                "<img data-rate='1' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='2' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='3' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='4' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img data-rate='5' align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +

            "{{/if}}" +
        "</div>"

    );