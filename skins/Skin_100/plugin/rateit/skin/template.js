$.template(
        "rateItDialogContainerTemplateId",
        "<div>" +
            "<div id='rateItDialogContainerId' class='itempopup itempopup-override'>" +
                "<div class='header'>" +
                    "<div class='header-col-a'>" +
                        "<div class='imageBox'><img src='${RatingModel.ImageURL}' /></div>" +
                    "</div>" +
                    "<div class='header-col-b'>" +
                        "<span>${RatingModel.ItemDescription}</span>" +
                        "<br />" +
                        "<br />" +
                        "<span id='ratingImage'></span>" +
                    "</div>" +
                "</div>" +
                "<div class='detail'>" +
                    "<div id='starContainer'>" +
                    "</div>" +
                    "<div id='commentContainer' class='content-curv'>" +
                        "<textarea id='txtComment' placeholder='Enter your comment' cols='40' rows='6' name='txtComment'>" +
                             '${RatingModel.Comment}' +
                        "</textarea>" +
                    "</div>" +
                    "<div id='commandContainer' class='content-curv'>" +
                        "<input id='btnSubmit' value='Submit' class='site-button content btn btn-info' ></input>" +
                        "<input id='btnCancel' value='Cancel' class='site-button content btn btn-info' ></input>" +
                    "</div>" +
                "</div>" +
            "</div>" +
        "</div>"
);