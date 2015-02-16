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
	    pluginTemplate: 'components/rateit/skin/template.js',
	    pluginTemplateCss: 'components/rateit/skin/index.css',
	    basePluginTemplate: 'components/rateit/skin/template.js', //if pluginTemplate is overridden but does not exist use the base which is the original path
	    basePluginTemplateCss: 'components/rateit/skin/index.css', //if pluginTemplateCss is overridden but does not exist use the base which is the original path
	    mainRateitDialogId: 'rateItDialogContainerId',
	    ItemCode: '',
	    onRateItCallback: null,
		Captions: {
		    HEADER: 'You are rating',
		    SELECT_RATING: 'Please select a rating',
            ENTER_COMMENT: 'Please enter a comment'
   	    },
		Config : {
			SkinId: 99,
			AllowAnonToRate: true
		},
		DialogWidth: '425',
		DialogHeight: '300',
		messages: {
		}
	};

	var global = {
		selected: '',
		selector: '',
		originalElem: ''
	};

	var pluginId = '';
	var cachedRatings;
	var selectedRate = 0;

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

	$.fn.RateIt = {

		setup: function (optionalConfig) {
			
            pluginId = this.generateRandomNumber(5, 10);

			thisObject = this;

			if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

			var config = getConfig();

			this.downloadPluginSkin(function () {

                thisObject.Display();

			});
		  
		},

		Display : function() {
			
			var config = getConfig();
			var param = new Object();
			param.itemCode = config.ItemCode;

			this.showLoading();

			this.ajaxRequest("ActionService.asmx/GetCustomerCurrentRatingJson", param, function (response) {

			    thisObject.hideLoading();

				if (!response) return;

				var yourRating = thisObject.ToJsonObject(response.d);

				var headerObject = new Object();
				headerObject.Caption = new Object();
				headerObject.Config = new Object();
				headerObject.RatingModel = Object();

				if (yourRating != null) {
				    headerObject.RatingModel = yourRating;
				    selectedRate = yourRating.Rating;
				}
				else {
				    selectedRate = 0;
				}

				var html = thisObject.parseTemplateReturnHtml("rateItDialogContainerTemplateId", headerObject);
				$('body').append(html);

                //support placeholder for ie10
				$("#txtComment").val("");
				if (yourRating.Comment != null) {
				    $("#txtComment").val(yourRating.Comment);
				}


                //just to make sure the container is ready
				var timeid = setInterval(function () {

				    if ($(thisObject.getElemBySelChecker(config.mainRateitDialogId)).length > 0)
				    {
				        thisObject.showInModal()
                                  .attachEvents();
                        
				        $('#ratingImage').RateStar
                                         .setup({ SkinId: config.Config.SkinId, Rating: headerObject.RatingModel.Rating });

				        $('#ratingImage').find("img[data-rate]")
                                         .unbind('click')
                                         .click(function () { thisObject.clickStar(this); })

				        clearInterval(timeid);
				    }

				}, 300);

			});

			return this;
		},

		clickStar : function (starElement) {
            
		    selectedRate = $(starElement).attr('data-rate');

		    $('#ratingImage').find('img').remove();
		    $('#ratingImage').RateStar.setup({ SkinId: config.Config.SkinId, Rating: selectedRate });

		    $('#ratingImage').find("img[data-rate]")
                             .unbind('click')
                             .click(function () { thisObject.clickStar(this); });

		},

		showInModal: function () {
		    var config = getConfig();

		    var curentDocWidth = config.DialogWidth; //($(document).width() - $(document).width() * .5);
		    var curentDocHeight = config.DialogHeight; //($(document).height() - $(document).height() * .63);

		    thisObject.getElemBySelChecker(config.mainRateitDialogId).dialog({
		        autoOpen: false,
		        modal: true,
		        position: "center",
		        resizable: false,
		        zIndex: 99999,
		        width: curentDocWidth,
		        height: curentDocHeight,
		        open: function (event, ui) {
		            $(this).parent().hide().fadeIn("slow", function () { });
		        },
		        close: function () {
		            var thisDialog = this;
		            $(this).parent().show().fadeOut("slow", function () {
		                $(thisDialog).remove();
		                $('.ui-effects-wrapper').remove();
		            });
		        },
		        beforeClose: function () {
		            thisObject.clear();
		        }
		    });

		    thisObject.getElemBySelChecker(config.mainRateitDialogId).dialog('open');
		    thisObject.autoCenterDialogOnResizeAndScroll();

		    return this;
		},

		autoCenterDialogOnResizeAndScroll: function () {
		    $(window).resize(function () {
		        thisObject.centerDialog();
		    });
		    $(window).scroll(function () {
		        thisObject.centerDialog();
		    });
		},

		centerDialog: function () {
		    var config = getConfig();
		    this.getElemBySelChecker(config.mainRateitDialogId + pluginId)
                .dialog("option", "position", "center");
		},

		attachEvents: function () {

		    $("#btnSubmit").unbind('click')
						   .bind('click', this.CreateJqueryDelegate(this.RateIt, this));

		    $("#btnCancel").unbind('click')
                           .bind('click', this.CreateJqueryDelegate(this.Cancel, this));

			return this;
		},

		saveWoohaData: function() {
		    
		    var productReviewModel = new ProductRatingModel();
		    productReviewModel.Comments = 'test';
		    productReviewModel.CompanyId = 244;
		    productReviewModel.CreatedOn = this.getDigitalDateFormat();
		    productReviewModel.CustomerEmailAddress = 'jj@j.com';
		    productReviewModel.CustomerName = 'jaysoncenteno';
		    productReviewModel.FoundHelpful = 1;
		    productReviewModel.FoundNotHelpful = 0;
		    productReviewModel.HasComments = true;
		    productReviewModel.Helpful = 0;
		    productReviewModel.InventoryItemId = 22686;
		    productReviewModel.IsActive = true;
		    productReviewModel.IsFilthy = false;
		    productReviewModel.IsRotd = false;
		    productReviewModel.MaskedWord = '';
		    productReviewModel.Rating = 0;
		    productReviewModel.RatingCustomerEmailAddress = '';
		    productReviewModel.RatingId = 0;
		    productReviewModel.RowIndex = 0;
		    productReviewModel.VotingCustomerEmailAddress = '';
		    productReviewModel.Word = '';
            
            this.ajaxRequestRest("http://devservice.woohaa.net/Upload/SubmitProductReview", 'PUT', productReviewModel, function (data) {

		    })

		},

		Cancel: function () {
		    this.getElemBySelChecker(config.mainRateitDialogId).dialog('close');
		},

		RateIt: function () {

		    var config = getConfig();

		    var param = new Object();
		    param.itemCode = config.ItemCode;
		    param.rating = selectedRate;
		    param.comment = this.getComment();

		    if (param.rating == 0) {
		        alert(defaults.Captions.SELECT_RATING);
		        return;
		    }

		    if (param.comment == "") {
		        alert(defaults.Captions.ENTER_COMMENT);
		        return;
		    }

		    this.ajaxRequest("ActionService.asmx/SaveRating", param, function (response) {

                if (!response) return;

                thisObject.getElemBySelChecker(config.mainRateitDialogId).dialog('close');

                if (typeof config.onRateItCallback === 'function') {
                    config.onRateItCallback();
                }

		    });

			return this;
		},

		showLoading : function() {
			this.showLoader();
		},

		hideLoading: function () {
			this.hideLoader();
		},

		getComment: function() {
		    return $.trim($('#txtComment').val());
		},

		downloadPluginSkin: function (callback) {

			var config = getConfig();
			this.downloadPlugin(config.pluginTemplate, function () {

				thisObject.downloadCss(config.pluginTemplateCss, function () {

					if (typeof callback === 'function') callback();

				});

			}, function (message) {

				if (typeof message == 'undefined' || (message && message.toLowerCase() == "not found")) {

					thisObject.downloadPlugin(config.basePluginTemplate, function () {

						thisObject.downloadCss(config.basePluginTemplateCss, function () {

							if (typeof callback === 'function') callback();

						});

					}, function (message) {

						alert('unable to load the plugin please check the source. \n\n Error message: ' + message);
						if (typeof callback === 'function') callback();

					});

				}
				else {
					alert('Unable to parse the script file: Error Message - ' + message);
				}

			});

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
	$.extend($.fn.RateIt, new jqueryBasePlugin());

	function setConfig(value) {
		config = value;
	}

	function getConfig() {
		return config;
	}

})(jQuery);