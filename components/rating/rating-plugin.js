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
	    pluginTemplate: 'components/rating/skin/template.js',
		pluginTemplateCss: 'components/rating/skin/index.css',
		basePluginTemplate: 'components/rating/skin/template.js', //if pluginTemplate is overridden but does not exist use the base which is the original path
		basePluginTemplateCss: 'components/rating/skin/index.css', //if pluginTemplateCss is overridden but does not exist use the base which is the original path
		ItemCode: '',
		ItemCounter: 0,
		Captions: {
			HEADER: 'Product Ratings',
			HAS_RATING: 'Your Rating',
			AVE_RATING: 'Ave Rating',
			CLICK_HERE: 'Click Here',
			ONLY_REG_CAN_RATE: '(Only registered customers can rate)',
			TO_BE_THE_FIRST: 'to be the first one to share your opinion!',
			TO_RATE_THIS_PRODUCT: 'to be the first to rate this product',
			CHANGE_YOUR_RATING: 'To change your rating',
			OUT_OF: 'Out of',
			VOTES: 'Votes',
			GREAT: '5-Great',
			GOOD: '4-Good',
			OK: '3-OK',
			BAD: '2-BAD',
			TERRIBLE: '1-Terrible',
			SORT_HEADER: 'Sort',
			NO_COMMENT: 'There are no comments by this customer.',

			VIEW_COMMENTS: 'View Comments',
			HELPFUL: 'Helpful to Less Helpful',
			LESS_HELPFUL: 'Less Helpful to Helpful',
			NEW_OLD: 'New to Old',
			OLD_NEW: 'Old to New',
			HIGH_LOW: 'High to Low Rating',
			LOW_HIGH: 'Low to High Rating'

		},
		Config : {
			SkinId: 99,
			AllowAnonToRate: true,
			RateItUrl: 'RateIt.aspx',
		    RatingPageSize: 10
		},
		messages: {
		}
	};

	var ProductRatingModel = function() {
	    this.Comments = ''
	    this.CompanyId = 0
	    this.CreatedOn = ''
	    this.CustomerEmailAddress = ''
	    this.CustomerName = ''
	    this.FoundHelpful = 0
	    this.FoundNotHelpful = 0
	    this.HasComments = false
	    this.Helpful = 0
	    this.InventoryItemId = 0
	    this.IsActive = false
	    this.IsFilthy = false
	    this.IsRotd = false
	    this.MaskedWord = ''
	    this.Rating = 0
	    this.RatingCustomerEmailAddress = ''
	    this.RatingId = 0
	    this.RowIndex = 0
	    this.VotingCustomerEmailAddress = ''
	    this.Word = ''
	};

	var global = {
		selected: '',
		selector: '',
		originalElem: ''
	};

	var isRegistered = false;
	var currentContactCode = '';
	var pluginId = '';
	var cachedRatings;

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

	$.fn.Rating = {

		setup: function (optionalConfig) {
			
		    global.originalElem = global.selected;
            pluginId = this.generateRandomNumber(5, 10);

			thisObject = this;

			if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

			var config = getConfig();

			this.downloadPluginSkin(function () {

				$(global.originalElem).each(function() {
					
					thisObject.Display(this);
					
				});

			});
		  
		},

		Display : function(targetElement) {
			
			var config = getConfig();

			var param = new Object();
			param.itemCode = config.ItemCode;

			this.ajaxRequest("ActionService.asmx/GetProductRatingSummary", param, function (response) {

				if (!response) return;

				var summary = thisObject.ToJsonObject(response.d);

				var headerObject = new Object();
				headerObject.Caption = new Object();
				headerObject.Config = new Object();

				headerObject.Caption.HEADER = config.Captions.HEADER;
				headerObject.Caption.HAS_RATING = config.Captions.HAS_RATING;
				headerObject.Caption.AVE_RATING = config.Captions.AVE_RATING;
				headerObject.Caption.CLICK_HERE = config.Captions.CLICK_HERE;
				headerObject.Caption.ONLY_REG_CAN_RATE = config.Captions.ONLY_REG_CAN_RATE;
				headerObject.Caption.TO_BE_THE_FIRST = config.Captions.TO_BE_THE_FIRST;
				headerObject.Caption.TO_RATE_THIS_PRODUCT = config.Captions.TO_RATE_THIS_PRODUCT;
				headerObject.Caption.CHANGE_YOUR_RATING = config.Captions.CHANGE_YOUR_RATING;
				headerObject.Caption.OUT_OF = config.Captions.OUT_OF;
				headerObject.Caption.VOTES = config.Captions.VOTES;
				headerObject.Caption.GREAT = config.Captions.GREAT;
				headerObject.Caption.GOOD = config.Captions.GOOD;
				headerObject.Caption.OK = config.Captions.OK;
				headerObject.Caption.BAD = config.Captions.BAD;
				headerObject.Caption.TERRIBLE = config.Captions.TERRIBLE;
				headerObject.Caption.VIEW_COMMENTS = config.Captions.VIEW_COMMENTS;

				headerObject.Config.HAS_RATING = summary.HasRating;
				headerObject.Config.IsRegistered = summary.IsRegistered;

				isRegistered = summary.IsRegistered;
				currentContactCode = summary.ContactCode;

				headerObject.Config.AllowAnonToRate = config.Config.AllowAnonToRate;
				headerObject.Config.SkinId = config.Config.SkinId;

				headerObject.Count = summary.Count;
				headerObject.GreatCount = summary.Great;
				headerObject.GoodCount = summary.Good;
				headerObject.OkCount = summary.OK;
				headerObject.BadCount = summary.BAD;
				headerObject.TerribleCount = summary.TERRIBLE;

				$(targetElement).html(null)
								.append(thisObject.parseTemplateReturnHtml("MainRatingTemplate", headerObject));

				var timeout = setTimeout(function () {

				    if ($('#ownRating').length > 0 && $('#average-rating').length > 0) {

				        $('#ownRating').RateStar.setup({ SkinId: config.Config.SkinId, Rating: summary.Rate });
				        $('#average-rating').RateStar.setup({ SkinId: config.Config.SkinId, Rating: summary.Average });

				        clearTimeout(timeout);
				    }

				}, 200)

				//headerObject.OwnRatingImage = thisObject.getRatingImage(summary.Rate);
				//headerObject.AverageRatingImage = thisObject.getRatingImage(summary.Average);

				thisObject.attachEvents();

			});

			return this;
		},

		attachEvents: function () {

			$("#rateIt").unbind('click')
						.bind('click', this.CreateJqueryDelegate(this.RateIt, this));

		    $("#lnkViewComments").unbind('click')
						         .bind('click', this.CreateJqueryDelegate(this.ViewComments, this));

		},

		attachEventRating: function () {

		    $("div[id^='voteYes-']").unbind('click')
                                    .bind('click', thisObject.Vote);

		    $("div[id^='voteNo-']").unbind('click')
                                   .bind('click', thisObject.Vote);

		    $("#lnkSeeMore-" + pluginId).unbind('click')
                                        .bind('click', thisObject.CreateJqueryDelegate(thisObject.SeeMore, thisObject));

		},

		attachDropDownSorterEvent: function () {
		    
		    $('#ddlRating-' + pluginId).unbind('change')
                                       .bind('change', this.CreateJqueryDelegate(this.Sort, this))

		},

		Sort: function () {
		    $('#ratings-container-' + pluginId).html(null);
		    this.clear();
		    this.getNextRatings();
		},

		SeeMore: function () {

		    $("#spinner-" + pluginId).show();
		    $("#lnkSeeMore-" + pluginId).hide();

		    this.getNextRatings(function (newRecordLength) {
                
		        $("#spinner-" + pluginId).hide();
		        if (newRecordLength == 0) {
		            $("#lnkSeeMore-" + pluginId).hide();
		        } else {
		            $("#lnkSeeMore-" + pluginId).show();
		        }

		    });

		},

		Vote: function () {

		    var config = getConfig();

		    var value = $(this).attr('data-vote');
		    var customerid = $(this).attr('data-customerid');
		    var contactCode = $(this).attr('data-contactcode');
		    var hasRate = $(this).attr('data-hasrate');

		    var currentRatingButton = $(this);

		    var param = new Object();
		    param.itemCode = config.ItemCode;
		    param.customerId = customerid;
		    param.contactId = contactCode;
		    param.vote = value;
		    
		    if (!isRegistered || currentContactCode == contactCode || hasRate == 'true') {
		        $(this).unbind('click');
		        return;
		    }

		    thisObject.ajaxRequest("ActionService.asmx/RateComment", param, function (response) {

		        var currentValue = $(currentRatingButton).find('.thumbs-count').html();
		        $(currentRatingButton).find('.thumbs-count').html(parseInt(currentValue) + 1);
		        thisObject.SetRateFlag();
		        thisObject.DisbleRateButtons(customerid);

		    });

		},

		DisbleRateButtons: function (customerid) {
		    $("#voteYes-" + customerid).unbind('click').attr('disabled', 'disabled').addClass('disabled');
		    $("#voteNo-" + customerid).unbind('click').attr('disabled', 'disabled').addClass('disabled');
		},

		SetRateFlag: function (customerid) {

		    $("#voteYes-" + customerid).attr('data-hasrate', true);
		    $("#voteNo-" + customerid).attr('data-hasrate', true);

		    return this;
		},

		rateItRefreshCallback: function () {
            
		    thisObject.showLoading();
		    var id = setTimeout(function () {

		    	$(global.originalElem).each(function () {

		    		thisObject.Display(this)
		    				  .hideLoading();

		    		clearTimeout(id);
		    	});

		    }, 300);

		},

		RateIt: function () {

			var config = getConfig();
			
			var basePlugin = new jqueryBasePlugin();
			basePlugin.downloadPlugin('components/rateIt/setup.js', function () {
			    var loader = new instorePluginLoader();
			    loader.start(function (newConfig) {
			        $.fn.RateIt.setup({ "onRateItCallback": thisObject.rateItRefreshCallback, "ItemCode": config.ItemCode });
			    });
			});

			//var returnURL = location.href;
			//var url = config.Config.RateItUrl + "?" +
			//		  "productid=" + config.ItemCounter +
			//		  "&NoPostback=1" +
			//		  "&refresh=no" +
			//		  "&returnurl=" + returnURL;

			//var specs = "height=450;width=440;status=no;toolbar=no;menubar=no;scrollbars=yes;location=no;resizable=no;center=yes;maximize=no";
			//var ret = window.open(url, this.getTimeStamp(), specs);

			//ret.onbeforeunload = function () {

			//	thisObject.showLoading();
			//	var id = setTimeout(function () {

			//		$(global.originalElem).each(function () {

			//			thisObject.Display(this)
			//					  .hideLoading();

			//			clearTimeout(id);
			//		});

			//	}, 300);

			//};

			return this;
		},

		ViewComments: function () {

		    var config = getConfig();

		    this.clear();

		    var param = new Object();
		    param.itemCode = config.ItemCode;
		    param.nextRecord = this.getNextRecordNumber();
		    param.ratingPageSize = config.Config.RatingPageSize;

		    var commentParam = new Object();
		    commentParam.Caption = new Object();
		    commentParam.Config = new Object();
		    commentParam.Caption.SORT_HEADER = config.Captions.SORT_HEADER;
		    commentParam.Caption.HELPFUL = config.Captions.HELPFUL;
		    commentParam.Caption.LESS_HELPFUL = config.Captions.LESS_HELPFUL;
		    commentParam.Caption.NEW_OLD = config.Captions.NEW_OLD;
		    commentParam.Caption.OLD_NEW = config.Captions.OLD_NEW;
		    commentParam.Caption.HIGH_LOW = config.Captions.HIGH_LOW;
		    commentParam.Caption.LOW_HIGH = config.Captions.LOW_HIGH;
		    commentParam.Config.PluginID = pluginId;

		    $("#comment-wrapper").html(null)
                                 .append(thisObject.parseTemplateReturnHtml("CommentsTemplate", commentParam));

		    this.attachDropDownSorterEvent();

		    this.getNextRatings();

		    return this;

		},

		GetCurrentSortBy: function() {
		    return $('#ddlRating-' + pluginId).val();
		},

		getNextRatings: function(callback) {

		    var config = getConfig();

		    var param = new Object();
		    param.itemCode = config.ItemCode;
		    param.nextRecord = this.getNextRecordNumber();
		    param.ratingPageSize = config.Config.RatingPageSize;

		    param.sortBy = this.GetCurrentSortBy();
		    
		    this.ajaxRequest("ActionService.asmx/GetProductRatings", param, function (response) {

		        var ratings = thisObject.ToJsonObject(response.d);

		        $(ratings).each(function () {
		            this.RatingStar = thisObject.getRatingImage(this.Rating);
		        });

		        var bindingParam = new Object();
                bindingParam.Caption = new Object();
		        bindingParam.Config = new Object();

		        bindingParam.Caption.NO_COMMENT = config.Captions.NO_COMMENT;
		        bindingParam.Config.IsRegistered = isRegistered;
		        bindingParam.Config.CurrentContactCode = currentContactCode;
		        bindingParam.Config.PluginID = pluginId;

		        thisObject.appendToRatings(ratings);
		        bindingParam.Ratings = ratings;

		        var newRatings = thisObject.parseTemplateReturnHtml("RatingsTemplate", bindingParam);

		        $("#ratings-container-" + pluginId).append(newRatings)
                                                   .find('table:last')
                                                   .hide()
                                                   .fadeIn(2000);

		        //need some delay to make sure the design is properly rendered
		        setTimeout(thisObject.attachEventRating, 200);

		        if (typeof callback === 'function') callback(ratings.length);

		    });

		},

		getNextRecordNumber : function () {
		    
		    if (cachedRatings != null && cachedRatings.length > 0)
		    {
		        //thisObject.sortByRowNumber(cachedRatings);

		        return cachedRatings[cachedRatings.length - 1].Row;

		    }

		    return pluginConstants.ZERO_VALUE;
		},

		appendToRatings: function (newRatings) {

		    if (cachedRatings != null) {
		        cachedRatings = $.merge(cachedRatings, newRatings);
		    }
		    else {
		        cachedRatings = newRatings;
		    }
		    return cachedRatings;

		},

		showLoading : function() {
			this.showLoader();
		},

		hideLoading: function () {
			this.hideLoader();
		},

		getRatingImage : function(rating) {

			var config = getConfig();

			var param = new Object();
			param.SkinId = config.Config.SkinId;
			param.Rating = rating;

			return thisObject.parseTemplateReturnHtml("RatingStarTemplate", param);
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

		    cachedRatings = null;

			return this;
		},

		config: function (args) {
			setConfig($.extend(defaults, args));
			return (getConfig());
		}

	};

	//jqueryBasePlugin is located inside core.js
	$.extend($.fn.Rating, new jqueryBasePlugin());

	function setConfig(value) {
		config = value;
	}

	function getConfig() {
		return config;
	}

})(jQuery);