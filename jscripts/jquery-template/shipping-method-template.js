$.template(
        "shippingMethodOptionTemplateID",
        "<div>" +
            "<input id='${id}' type='radio' name='${name}' value='${description}' ${isDefault} ${disabled}>" +
            "<span>" +
                "<span id='shipping-description$" + "${id}' class='shipping-description' style='cursor: pointer;'> ${description} </span>" +
                "<span> ${freight} </span>" +
                "{{if InStoreCaption}}" +
                    "<a id='instore-map-link_${ItemCounter}' href='javascript:void(0)'><i class='icon-map-marker'></i>${InStoreCaption}</a>" +
                    "<div id='selectedInStoreInfoContainer_${ItemCounter}'>" +
                    "</div>" +
                "{{/if}}" +
            "</span>" +
        "</div>"
    );

$.template(
        "selectedWarehouseTemplateId",
        "<div>" +
            "<div id='selectedWarehouseShippingMethodwrapper' class='box' style='padding: 5px; border: solid 1px #777; background-color:#F2E9E9; font-weight: 9pt; color:#555; margin:5px 0px; 5px 5px;'>" +
                "<b><span>${WareHouseDescription}</span></b><br/>" +
                "<span>" +
                    "${Address.Address}, ${Address.City} <br/>" +
                    "${Address.State} ${Address.PostalCode} <br/>" +
                    "${Address.Country} " +
                "</span>" +
            "</div>" +
        "</div>"
);