$.template(
        "MainRatingTemplate",
        "<div>" +
            "<div class='section-wrapper' id='productRating'>" +
                "<div class='header'>${Caption.HEADER}</div>" +
                "<br />" +
                "<table class='rating-info'>" +
                    "<tr class='caption'>" +
                        "<td class='my-rating'>" +
                            "{{if Config.HAS_RATING}}" +
                                "${Caption.HAS_RATING}" +
                            "{{/if}}" +
                        "</td>" +
                        "<td class='average-rating text-right'>" +
                            "${Caption.AVE_RATING}" +
                        "</td>" +
                    "</tr>" +
                    "<tr >" +
                        "<td>" +
                            "{{if Config.HAS_RATING}}" +
                                "<span>{{html OwnRatingImage}}</span>" +
                            "{{/if}}" +

                            "{{if Config.IsRegistered || Config.AllowAnonToRate == 'true'}}" +

                                "<span class='total-votes'>" +
                                    "<a id='rateIt' href='javascript:void(0)'>${Caption.CLICK_HERE}</a>" +

                                    "{{if Count == 0}}" +
                                        "<span>${Caption.TO_BE_THE_FIRST}</span>" +
                                    "{{else}}" +

                                        "{{if Config.HAS_RATING}}" +
                                            "${Caption.CHANGE_YOUR_RATING}" +
                                        "{{else}}" +
                                            "${Caption.TO_RATE_THIS_PRODUCT}" +
                                        "{{/if}}" +

                                    "{{/if}}" +

                                "</span>" +

                            "{{else}}" +
                                "<span>${Caption.ONLY_REG_CAN_RATE}</span>" +
                            "{{/if}}" +

                        "</td>" +
                         "<td class='text-right'>" +
                            "<span class='average-rating'>" +
                                "{{html AverageRatingImage}}" +
                            "</span>" +
                            "<span class='total-votes'>" +
                                "${Caption.OUT_OF} ${Count} ${Caption.VOTES}" +
                            "</span>" +
                        "</td>" +
                    "</tr>" +
                 "</table>" +
                 "<br />" +
                 "<table class='rating-summary'>" +
                    "<tr>" +
                        "<th></th>" +
                        "<th class='min'>0%</th>" +
                        "<th class='mid'>50%</th>" +
                        "<th class='max'>100%</th>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='caption'>${Caption.GREAT}</td>" +
                        "<td class='bar' colspan='3'>" +
                            "{{if GreatCount > 0}}" +
                                "<img src='skins/skin_${Config.SkinId}/images/pollimage.gif' class='vote-image-graph' style='width: ${GreatCount}%' />" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='caption'>${Caption.GOOD}</td>" +
                        "<td class='bar' colspan='3'>" +
                             "{{if GoodCount > 0}}" +
                                "<img src='skins/skin_${Config.SkinId}/images/pollimage.gif' class='vote-image-graph' style='width: ${GoodCount}%' />" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='caption'>${Caption.OK}</td>" +
                        "<td class='bar' colspan='3'>" +
                             "{{if OkCount > 0}}" +
                                "<img src='skins/skin_${Config.SkinId}/images/pollimage.gif' class='vote-image-graph' style='width: ${OkCount}%' />" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='caption'>${Caption.BAD}</td>" +
                        "<td class='bar' colspan='3'>" +
                             "{{if BadCount > 0}}" +
                                "<img src='skins/skin_${Config.SkinId}/images/pollimage.gif' class='vote-image-graph' style='width: ${BadCount}%' />" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td class='caption'>${Caption.TERRIBLE}</td>" +
                        "<td class='bar' colspan='3'>" +
                             "{{if TerribleCount > 0}}" +
                                "<img src='skins/skin_${Config.SkinId}/images/pollimage.gif' class='vote-image-graph' style='width: ${TerribleCount}%' />" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan='2'>" +
                            "<br />" +
                            "<a href='javascript:void(0);' id='lnkViewComments'>${Caption.VIEW_COMMENTS}</a>" +
                        "</td>" +
                    "</tr>" +
                 "</table>" +
                 "<br/>" +
                 "<div id='comment-wrapper'>" +
                 "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "CommentsTemplate",
        "<div>" +
            "<table class='comments' id='comment-container'>" +
                "<tr class='sort-header'>" +
                    "<td>" +
                        "<span class='sort-header-text'>${Caption.SORT_HEADER}</span>" +
                        "<select name='RatingSortOrder' size='1' id='ddlRating-${Config.PluginID}'>" +
                            "<option value='1'>${Caption.HELPFUL}</option>" +
                            "<option value='2'>${Caption.LESS_HELPFUL}</option>" +
                            "<option selected='' value='4'>${Caption.NEW_OLD}</option>" +
                            "<option value='8'>${Caption.OLD_NEW}</option>" +
                            "<option value='16'>${Caption.HIGH_LOW}</option>" +
                            "<option value='32'>${Caption.LOW_HIGH}</option>" +
                        "</select>" +
                    "</td>" +
                "</tr>" +
                "<tr>" +
                    "<td  id='ratings-container-${Config.PluginID}'>" +
                    "</td>" +
                "</tr>" +
                "<tr>" +
                    "<td colspan='2'>" +
                        "<span id='spinner-${Config.PluginID}' style='display:none'><i class='icon-spinner icon-spin icon-2x'/></span>" +
                        "<a href='javascript:void(0);' id='lnkSeeMore-${Config.PluginID}'>See More</a>" +
                        "<div id='tempContainer-${Config.PluginID}' style='display:none'></div>" +
                    "</td>" +
                "</tr>" +
            "</table>" +
        "</div>"
);

$.template(
        "RatingsTemplate",
        "<div>" +
            "<table class='person-comment'>" +
                "{{each Ratings}}" +
                    "<tr>" +
                        "<td>" +
                            "<i class='icon-user'></i><span class='name'>${ContactSalutationCode} ${FirstName} ${LastName}</span><br />" +
                            "<span class='date-on'>${CreatedOn}</span>" +
                        "</td>" +
                        "<td>" +
                            "<span class='star right'>{{html RatingStar}}</span>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan='2' class='comment-text'>" +
                            "{{if Comment != '' }}" +
                                "<span>${Comment}</span>" +
                            "{{else}}" +
                                "<span>${Caption.NO_COMMENT}</span>" +
                            "{{/if}}" +
                        "</td>" +
                    "</tr>" +
                    "<tr class='separator'>" +
                        "<td colspan='2' class='voting'>" +
                            "<span>Was this comment helpful?</span>" +
                            "<div id='voteYes-${CustomerId}' data-vote='YES' data-customerid='${CustomerId}' data-contactcode='${ContactId}' data-hasrate='${HasRate}' " +

                                "{{if !Config.IsRegistered || Config.CurrentContactCode == ContactId || HasRate}}" +
                                    "disabled='disabled'" + " class='rate-it spacer disabled'" +
                                "{{else}}" +
                                    "class='rate-it spacer'" +
                                "{{/if}}" +

                            ">" +
                                "<i class='icon-thumbs-up'></i><span class='thumbs-count'>${FoundHelpful}</span>" +
                            "</div>" +
                            "<div id='voteNo-${CustomerId}' data-vote='NO' data-customerid='${CustomerId}' data-contactcode='${ContactId}' data-hasrate='${HasRate}'" +

                                "{{if !Config.IsRegistered || Config.CurrentContactCode == ContactId || HasRate}}" +
                                    "disabled='disabled'" + " class='rate-it disabled'" +
                                "{{else}}" +
                                    "class='rate-it'" +
                                "{{/if}}" +

                            ">" +
                                "<i class='icon-thumbs-down'></i><span class='thumbs-count'>${NotHelpful}</span>" +
                            "</div>" +
                            "<br/>" +
                        "</td>" +
                    "</tr>" +
                "{{/each}}" +
            "</table>"
);


$.template(
        "RatingStarTemplate",
        "<div>" +
            "{{if (Rating < 0.25) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 0.25 && Rating < 0.75) }}" +
                
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" + 
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 0.75 && Rating < 1.25) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 1.25 && Rating < 1.75) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 1.75 && Rating < 2.25) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 2.25 && Rating < 2.75) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 2.75 && Rating < 3.25) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 3.25 && Rating < 3.75) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 3.75 && Rating < 4.25) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/stare.gif' />" +

            "{{else (Rating >= 4.25 && Rating < 4.75) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starh.gif' />" +

            "{{else (Rating >= 4.75) }}" +

                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +
                "<img align='absmiddle' src='skins/skin_${SkinId}/images/starf.gif' />" +

            "{{/if}}" +
        "</div>"

);