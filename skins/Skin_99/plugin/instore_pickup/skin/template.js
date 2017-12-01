$.template(
        "storeDialogContainerTemplateId",
        "<div>" +
            "<div id='storeDialogContainerId' class='itempopup itempopup-override'>" +
                "<div class='header'>" +
                    "<div class='header-row'>" +
                        "<div class='header-col-a'>" +
                            "<div class='imageBox'>" +
                                "<img src='${ProductInfo.ImageURL}' >" +
                                "</img>" +
                            "</div>" +
                        "</div>" +
                        "<div class='header-col-b'>" +
                            "<div class='titleBox'>" +
                                "{{if ProductInfo.ItemDescription!=''}}" +
                                    "${ProductInfo.ItemDescription}" +
                                "{{else}}" +
                                    "${ProductInfo.ItemName} " +
                                "{{/if}}" +
                            "</div>" +
                            "<div class='menuBox'>" +
                                "<div>" +
                                    "{{html ProductInfo.RatingHtml}}" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                        "<div class='header-col-c' id='storeInfoHeaderContainerId'>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
                "<div class='store-dialog-body'>" +
                    "<div id='storePickupSideMenuId' class='content-curv box'>" +
                    "</div>" +
                    "<div id='storePickupMapContainerWrapperId' class='content-curv box'>" +
                    "</div>" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "storeInfoHeaderContainerTemplateId",
        "<div>" +
            "<div class='store-info'>" +
                "<span class='store-title-format'>${Caption.StoreInfoTitleText}</span><br/>" +
                "<span class='store-name'>${Warehouse.WareHouseDescription}</span><br/>" +
                "<span class='store-address-description'>" +
                    "${Warehouse.Address.Address}, ${Warehouse.Address.City} <br/>" +
                    "${Warehouse.Address.State} ${Warehouse.Address.PostalCode} <br/>" +
                    "${Warehouse.Address.Country}" +
                     "{{if Warehouse.Telephone!=''}}" +
                        "<br/> Telephone: ${Warehouse.Telephone}" +
                     "{{/if}}" +
                     "{{if Warehouse.Fax!=''}}" +
                        "<br/> Fax: ${Warehouse.Fax}" +
                     "{{/if}}" +
                     //"{{if Warehouse.Email!=''}}" +
                     //   "<br/> Email: ${Warehouse.Email}" +
                     //"{{/if}}" +
                     //"{{if Warehouse.Website!=''}}" +
                     //   "<br/> Website: <a href='http://${Warehouse.Website}/' target='_blank'>${Warehouse.Website}</a>" +
                     //"{{/if}}" +
                "</span>" +
            "</div>" +
            "<div class='store-hours'>" +
                "<span class='store-title-format'>${Caption.StoreHoursTitleText}</span><br/>" +
                "{{each WorkingHours}}" +
                    "<span class='store-name'>${Day} : </span>" +
                    "{{if WorkingHour.Close}}" +
                        "<span>${Caption.StoreCloseText}</span>" +
                    "{{else}}" +
                        "<span>${WorkingHour.Opening} - ${WorkingHour.Closing}</span>" +
                    "{{/if}}" +
                    "</br>" +
                "{{/each}}" +
            "</div>" +
            "<div class='store-holidays'>" +
                "<span class='store-title-format'>${Caption.StoreHolidayTitleText}</span><br/>" +
                "{{each Holidays}}" +
                    "<span class='store-name'>${Name} (${Date}): </span>" +
                    "{{if WorkingDay.Close}}" +
                        "<span>${Caption.StoreCloseText}</span>" +
                    "{{else}}" +
                        "<span>${WorkingDay.Opening} - ${WorkingDay.Closing}</span>" +
                    "{{/if}}" +
                    "</br>" +
                "{{/each}}" +
            "</div>" +
        "</div>"
);

$.template(
        "storeDialogMenuContainerTemplateId",
        "<div>" +
            "<div id='storeWhereMenuWrapperId'>" +
                "<div class='header content-header content-curv header-format'>" +
                    "<span>${Caption.MenuHeaderText}</span>" +
                "</div>" +
                "<div class='store-menu-body'>" +
                    "<span>${Caption.PickupWhereText}</span>" +
                    "<div class='field-item'>" +
                        "<input type='text' id='searchWherePostalId' placeholder='${Caption.ZipText}' value='${Address.PostalCode}' />" +
                    "</div>" +
                    "<div class='field-item'>or</div>" +
                    "<div class='field-item'>" +
                        "<input type='text' id='searchWhereCityId' placeholder='${Caption.CiyText}' title='${Caption.CiyText}' />" +
                    "</div>" +
                    "<div class='field-item'>" +
                        "<input type='text' id='searchWhereStateId' placeholder='${Caption.StateText}' title='${Caption.StateText}' />" +
                    "</div>" +
                    "<div class='field-item'>" +
                            "<select id='ddlInstoreCountryId' class='store-country-limit'>" +
                            "{{each Countries}}" +
                                '<option> ${CountryCode} </option>' +
                            "{{/each}}" +
                        "</select>" +
                    "</div>" +
                    "<div class='field-item'>" +
                        "<input type='button' class='site-button content btn btn-info' id='btnInstoreSearchId' value='${Caption.ButtonSearchText}' title='${Caption.ButtonSearchText}' />" +
                    "</div>" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "storeListContainerTemplateId",
        "<div>" +
            "<div id='storeListContainerId'>" +
                "<div class='header content-header content-curv header-format'>" +
                    "<span>${Caption.StoreListHeaderText}</span>" +
                "</div>" +
                "<div id='storeListContainerScrollerId'>" +
                    "<table id='storeListTableContainerId'>" +
                        "<tr>" +
                            "<td class='store-item-header'>" +
                                "${Caption.StoreListStoreHeaderText}" +
                            "</td>" +
                            "<td class='store-item-header'>" +
                                "${Caption.StoreListStockAvailabilityHeaderText}" +
                            "</td>" +
                            "{{if AppConfig.ShowActualStock}}" +
                                "<td class='store-item-header store-item-stock'>" +
                                    "${Caption.StoreListStockText} (${Caption.UnitMeasureText})" +
                                "</td>" +
                            "{{/if}}" +
                            "<td id='distanceHeaderId' class='store-item-header store-item-distance' title='Sort by Distance'>DISTANCE <br/><span>(in Miles)</span><i class='icon-sort'></td>" +
                            "<td class='store-item-header'></td>" +
                        "</tr>" +
                    "</table>" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "storeListDataTemplate",
        "<table>" +
            "{{each WarehouseList}}" +
                "<tr class='store-item-wrapper' data-store-item-attr >" +
                    "<td class='store-item store-item-info'>" +
                        "<span class='store-name'>" +
                            "${WareHouseDescription}" +
                        "</span>" +
                        "<br/>" +
                        "<span class='store-address-description'>" +
                            "${Address.Address}, ${Address.City}, ${Address.State} ${Address.PostalCode} <br/>" +
                            "${Address.Country} " +
                        "</span>" +
                        "<br/>" +
                        "<a href='javascript:void(0)' class='site-link map-link' data-storeid='${WareHouseCode}'>" +
                            "<i class='icon-map-marker' data-storeid='${WareHouseCode}'></i>${Caption.StoreListDirectionLinkText}" +
                        "</a>" +
                    "</td>" +
                    "<td class='store-item store-item-availability'>" +
                        "{{if FreeStock > 0}}" +
                            "<img src='images/instock.png' al='' ></img>" +
                        "{{else}}" +
                            "<img src='images/outofstock.png' al='' ></img>" +
                        "{{/if}}" +
                    "</td>" +
                    "{{if AppConfig.ShowActualStock}}" +
                        "<td class='store-item store-item-stock'>" +
                            "<span>${FreeStock}</span> " +
                        "</td>" +
                    "{{/if}}" +
                    "<td class='store-item store-item-distance'>" +
                        "<span id='distance-${RowNumber}' style='display:none'></span>" +
                        "<i id='spinner-${RowNumber}' class='icon-spinner icon-spin icon-2x'></i>" +
                    "</td>" +
                    "<td class='store-item store-item-input'>" +
                        "{{if FreeStock > 0}}" +
                            "{{if !AppConfig.ShowInAddToCart }}" +
                                "<input type='button' class='site-button content btn btn-info store-select' value='${Caption.StoreListSelectButtonText}' data-storeid='${WareHouseCode}' data-stock='${FreeStock}'>" +
                                "</input>" +
                            "{{else}}" +
                                "<input type='button' class='site-button content btn btn-info' id='btnAddToCart_${WareHouseCode}' value='${Caption.AddToCartButtonText}' data-storeid='${WareHouseCode}' data-stock='${FreeStock}'>" +
                                "</input>" +
                            "{{/if}}" +
                        "{{else}}" +
                            "---" +
                        "{{/if}}" +
                    "</td>" +
                "</tr>" +
            "{{/each}}" +
        "</table>"
);



$.template(
        "storeListEmptySearchTemplateId",
        "<div>" +
            "<div id='storeListContainerId'>" +
                "<div class='header content-header content-curv header-format'>" +
                    "${Caption.StoreListHeaderText}" +
                "</div>" +
                "<div id='storeListContainerScrollerId' class='store-list-empty'>" +
                    "${Caption.NoRecordsFoundMessage}" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "storeMapContainerTemplateId",
        "<div>" +
            "<div id='storeMapContainerId'>" +
                "<div class='storeMapCloseContainerClass'>" +
                    "<a href='javascript:void(0)' id='lnkMapCloseId'><i class='icon-remove'>${Caption.MapCloseText}</i></a>" +
                "</div>" +
                "<div id='storeMapId'>" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "storeMapMenuContainerTemplateId",
        "<div>" +
            "<div id='storeHowMenuWrapperId'>" +
                "<div class='header content-header content-curv header-format'>" +
                    "${Caption.MenuHeaderText}" +
                "</div>" +
                "<div class='store-menu-body'>" +
                    "<span>${Caption.PickupHowText}</span>" +
                    "<div class='field-item'>" +
                        "<input type='text' id='txtSearchHowAddressId' placeholder='${Caption.AddressText}' value='${Address.PostalCode}' />" +
                    "</div>" +
                    "<div class='field-item how-sample-address-text'>" +
                        "<span>(eg. ${Caption.AddressText})</span>" +
                    "</div>" +
                    "<div class='field-item'>" +
                        "<input type='button' class='site-button content btn btn-info' id='btnHowSearchId' value='${Caption.ButtonSearchText}' title='${Caption.ButtonSearchText}' />" +
                    "</div>" +
                "</div>" +
                "<div class='storeMapDirectionWrapperClass'>" +
                    "<div class='header content-header content-curv header-format'>" +
                         "<span>${Caption.DirectionText}</span>" +
                    "</div>" +
                    "<div id='storeMapDirectionContainerId'>" +
                    "</div>" +
                "</div>" +
            "</div>" +
        "</div>"
);

$.template(
        "selectedWarehouseShippingMethodTemplate",
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