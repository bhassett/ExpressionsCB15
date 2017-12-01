
var BundleProduct = function () {
    //*****Start Properties

    var bundle = null;
    var settings = null;
    var items = [];

    //*****End Properties



    //*****Start Functions
    function addItem(item) {
        this.items.push(item);
        item["getFreeStock"] = Function.createDelegate(this, this.getFreeStock);

        if (item.ItemType == 'Matrix Group') {
            if (item.LoadAttributes != undefined) { item.LoadAttributes(); }
            item["getMatrixItemBySelectedAttributes"] = Function.createDelegate(this, this.getMatrixItemBySelectedAttributes);
        }

    }

    function getMatrixItemAttributes(item) {
        var defaultSelectedMatrixItemCode = item.DefaultSelectedMatrixItemCode;
        for (var i = 0; i < item.MatrixItems.length; i++) {
            if (defaultSelectedMatrixItemCode == item.MatrixItems[i].MatrixItemCode) {
                var attributes = this.getAttributes(item.MatrixItems[i]);
                return { item: item.MatrixItems[i], attributes: attributes };
            }
        }
        return null;
    }

    function getAttributes(matrixItem) {
        var attributes = [];
        if (matrixItem.Attribute1 != '' && matrixItem.AttributeCode1 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute1, AttributeCode: matrixItem.AttributeCode1 })
        }
        if (matrixItem.Attribute2 != '' && matrixItem.AttributeCode2 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute2, AttributeCode: matrixItem.AttributeCode2 })
        }
        if (matrixItem.Attribute3 != '' && matrixItem.AttributeCode3 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute3, AttributeCode: matrixItem.AttributeCode3 })
        }
        if (matrixItem.Attribute4 != '' && matrixItem.AttributeCode4 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute4, AttributeCode: matrixItem.AttributeCode4 })
        }
        if (matrixItem.Attribute5 != '' && matrixItem.AttributeCode5 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute5, AttributeCode: matrixItem.AttributeCode5 })
        }
        if (matrixItem.Attribute6 != '' && matrixItem.AttributeCode6 != '') {
            attributes.push({ AttributeValueCode: matrixItem.Attribute6, AttributeCode: matrixItem.AttributeCode6 })
        }
        return attributes;

    }

    function getMatrixItemBySelectedAttributes(item, attributes) {
        function attributesHaveMatch(currentItem) {
            var foundCounter = 0;
            var foundInMatrixItem = 0;
            if (currentItem.AttributeCode1 != '') foundInMatrixItem++;
            if (currentItem.AttributeCode2 != '') foundInMatrixItem++;
            if (currentItem.AttributeCode3 != '') foundInMatrixItem++;
            if (currentItem.AttributeCode4 != '') foundInMatrixItem++;
            if (currentItem.AttributeCode5 != '') foundInMatrixItem++;
            if (currentItem.AttributeCode6 != '') foundInMatrixItem++;


            for (var i = 0; i < attributes.length; i++) {
                if (currentItem.AttributeCode1 == attributes[i].AttributeCode && currentItem.Attribute1 == attributes[i].AttributeValueCode) { foundCounter++ }
                if (currentItem.AttributeCode2 == attributes[i].AttributeCode && currentItem.Attribute2 == attributes[i].AttributeValueCode) { foundCounter++ }
                if (currentItem.AttributeCode3 == attributes[i].AttributeCode && currentItem.Attribute3 == attributes[i].AttributeValueCode) { foundCounter++ }
                if (currentItem.AttributeCode4 == attributes[i].AttributeCode && currentItem.Attribute4 == attributes[i].AttributeValueCode) { foundCounter++ }
                if (currentItem.AttributeCode5 == attributes[i].AttributeCode && currentItem.Attribute5 == attributes[i].AttributeValueCode) { foundCounter++ }
                if (currentItem.AttributeCode6 == attributes[i].AttributeCode && currentItem.Attribute6 == attributes[i].AttributeValueCode) { foundCounter++ }
            }
            return foundCounter == attributes.length && foundInMatrixItem == attributes.length;
        }
        for (var i = 0; i < item.MatrixItems.length; i++) {
            var currentItem = item.MatrixItems[i];
            if (attributesHaveMatch(currentItem)) {
                return currentItem;
            }
        }
        return null;
    }

    function getItemByLineNum(lineNum) {
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i].LineNum == lineNum) {
                return this.items[i];
            }
        }
        return null;
    }

    function generateMatrixGroupAttributes(item) {
        if (item.LoadAttributes == undefined) { return; }
        var attributes = [];
        for (var i = 0; i < item.Attributes.length; i++) {
            var foundAttribute = null;
            for (var c = 0; c < attributes.length; c++) {
                if (attributes[c].AttributeCode == item.Attributes[i].AttributeCode) {
                    foundAttribute = c;
                    break;
                }
            }
            if (foundAttribute != null) {
                attributes[foundAttribute].Attributes.push(
                    {
                        AttributeValueCode: item.Attributes[i].AttributeValueCode,
                        AttributeValueDescription: item.Attributes[i].AttributeValueDescription
                    });
            } else {
                var children = [];
                children.push(
                    {
                        AttributeValueCode: item.Attributes[i].AttributeValueCode,
                        AttributeValueDescription: item.Attributes[i].AttributeValueDescription
                    });
                attributes.push({
                    AttributeCode: item.Attributes[i].AttributeCode,
                    AttributeDescription: item.Attributes[i].AttributeDescription,
                    Attributes: children
                });
            }
        }
        item.LoadAttributes(attributes, item);
    }

    function getSelectedMatrixItem(matrixItems, itemCode) {
        for (var i = 0; i < matrixItems.length; i++) {
            if (matrixItems[i].MatrixItemCode == itemCode) {
                return matrixItems[i];
            }
        }
        return null;
    }

    function getFreeStock(itemCode, unitMeasureCode) {
        var freeStock = 0;
        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetInventoryFreeStock",
            data: JSON.stringify({ itemCode: itemCode, unitMeasureCode: unitMeasureCode }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                freeStock = result.d;
            },
            fail: function (result) {

            }
        });
        return freeStock;

    }

    function validate() {
        var returnObj = {};
        returnObj["isValid"] = false;

        var originalPrice = this.bundle.TotalPrice;
        var currentPrice = 0;
        var checkItemCounter = 0;
        var itemsWithZeroStock = [];
        var selectedItems = [];

        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i].ItemType == 'Matrix Group') {
                var selectedAttributes = this.items[i].CallBackGetSelectedElementsAttributes();
                var selectedMatrixItem = this.getMatrixItemBySelectedAttributes(this.items[i], selectedAttributes);
                // find matrix item with selectedAttributes
                if (selectedMatrixItem != null) {
                    //original matrix item
                    if (this.items[i].DefaultSelectedMatrixItemCode == selectedMatrixItem.MatrixItemCode) {
                        currentPrice += this.items[i].SalesPrice * this.items[i].Quantity;
                        //selectedItems.push(this.items[i].DefaultSelectedMatrixItemCode);

                        if (this.items[i].ShoppingCartRecID == undefined) {
                            selectedItems.push({ ItemCode: this.items[i].DefaultSelectedMatrixItemCode, AlternateItemCode: this.items[i].DefaultSelectedMatrixItemCode, MatrixGroup: this.items[i].ItemCode });
                        }
                        else {
                            selectedItems.push({ ItemCode: this.items[i].DefaultSelectedMatrixItemCode, AlternateItemCode: this.items[i].DefaultSelectedMatrixItemCode, MatrixGroup: this.items[i].ItemCode, ShoppingCartRecID: this.items[i].ShoppingCartRecID });
                        }

                        if (this.getFreeStock(this.items[i].DefaultSelectedMatrixItemCode, this.items[i].UnitMeasureCode) < 1) {
                            itemsWithZeroStock.push({ ItemType: this.items[i].ItemType, ItemCode: this.items[i].ItemCode, MatrixItemCode: this.items[i].DefaultSelectedMatrixItemCode });
                        }
                    } else {
                        //find the price of other matrix items
                        currentPrice += this.getItemSalesPrice(selectedMatrixItem.Counter, selectedMatrixItem.MatrixItemCode, this.items[i].Quantity);
                        //selectedItems.push(selectedMatrixItem.MatrixItemCode);


                        if (this.items[i].ShoppingCartRecID == undefined) {
                            selectedItems.push({ ItemCode: this.items[i].DefaultSelectedMatrixItemCode, AlternateItemCode: selectedMatrixItem.MatrixItemCode, MatrixGroup: this.items[i].ItemCode });
                        }
                        else {
                            selectedItems.push({ ItemCode: this.items[i].DefaultSelectedMatrixItemCode, AlternateItemCode: selectedMatrixItem.MatrixItemCode, MatrixGroup: this.items[i].ItemCode, ShoppingCartRecID: this.items[i].ShoppingCartRecID });
                        }

                        if (this.getFreeStock(selectedMatrixItem.MatrixItemCode, this.items[i].UnitMeasureCode) < 1) {
                            itemsWithZeroStock.push({ ItemType: this.items[i].ItemType, ItemCode: this.items[i].ItemCode, MatrixItemCode: selectedMatrixItem.MatrixItemCode });
                        }
                    }
                    checkItemCounter++;
                }
                else {
                    //this item (combination of attributes) does not exist
                    //todo: show outofstock/does not exist ??
                    if (this.items[i].ElementErrorContainer != undefined) {
                        this.items[i].ElementErrorContainer.element.attr(this.items[i].ElementErrorContainer.property, this.settings.MessageMatrixItemNotFound);
                        switch (this.items[i].ElementErrorContainer.property.toLowerCase()) {
                            case 'text': {
                                this.items[i].ElementErrorContainer.element.text(this.settings.MessageMatrixItemNotFound);
                            } break;
                            case 'val': {
                                this.items[i].ElementErrorContainer.element.val(this.settings.MessageMatrixItemNotFound);
                            } break;
                            case 'value': {
                                this.items[i].ElementErrorContainer.element.val(this.settings.MessageMatrixItemNotFound);
                            } break;
                        }
                        this.items[i].ElementErrorContainer.element.show();
                    }
                }
            } else {
                currentPrice += this.items[i].SalesPrice * this.items[i].Quantity;
                selectedItems.push({ ItemCode: this.items[i].ItemCode });


                if (this.getFreeStock(this.items[i].ItemCode, this.items[i].UnitMeasureCode) < 1) {
                    itemsWithZeroStock.push({ ItemType: this.items[i].ItemType, ItemCode: this.items[i].ItemCode });
                }
                checkItemCounter++;
            }
        }
        currentPrice = Math.round(currentPrice * 100) / 100;

        returnObj["isValid"] = originalPrice >= currentPrice && checkItemCounter == this.items.length;//&& itemsWithZeroStock.length < 1;
        returnObj["items"] = selectedItems;
        returnObj["bundleCode"] = this.bundle.BundleCode;
        return returnObj;
    }

    function getItemSalesPrice(counter, itemCode, quantity) {
        var price = 0;
        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetItemSalesPrice",
            data: JSON.stringify({ counter: counter, itemCode: itemCode, quantity: quantity }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                price = result.d;
            },
            fail: function (result) { }
        });
        return price;
    }

    function doesBundleHasStock() {
        var itemStockCounter = 0;
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i].ItemType == 'Matrix Group') {
                var selectedAttributes = this.items[i].CallBackGetSelectedElementsAttributes();
                var selectedMatrixItem = this.getMatrixItemBySelectedAttributes(this.items[i], selectedAttributes);

                if (selectedMatrixItem != null) {

                    if (this.items[i].DefaultSelectedMatrixItemCode == selectedMatrixItem.MatrixItemCode) {

                        if (this.getFreeStock(this.items[i].DefaultSelectedMatrixItemCode, this.items[i].UnitMeasureCode) > 0) {
                            itemStockCounter++;
                        }
                    } else {
                        if (this.getFreeStock(selectedMatrixItem.MatrixItemCode, this.items[i].UnitMeasureCode) > 0) {
                            itemStockCounter++;
                        }
                    }
                }

            } else {
                if (this.getFreeStock(this.items[i].ItemCode, this.items[i].UnitMeasureCode) > 0) {
                    itemStockCounter++;
                }
            }
        }
        return itemStockCounter === this.items.length;
    }

    function doesMatrixItemHasAttribute(matrixItem, attributes) {
        var count = 0;

        for (var i = 0; i < attributes.length; i++) {
            if (matrixItem.AttributeCode1 == attributes[i].AttributeCode && matrixItem.Attribute1 == attributes[i].AttributeValueCode) {
                count++;
            }
            else if (matrixItem.AttributeCode2 == attributes[i].AttributeCode && matrixItem.Attribute2 == attributes[i].AttributeValueCode) {
                count++;
            }
            else if (matrixItem.AttributeCode3 == attributes[i].AttributeCode && matrixItem.Attribute3 == attributes[i].AttributeValueCode) {
                count++;
            }
            else if (matrixItem.AttributeCode4 == attributes[i].AttributeCode && matrixItem.Attribute4 == attributes[i].AttributeValueCode) {
                count++;
            }
            else if (matrixItem.AttributeCode5 == attributes[i].AttributeCode && matrixItem.Attribute5 == attributes[i].AttributeValueCode) {
                count++;
            }
            else if (matrixItem.AttributeCode6 == attributes[i].AttributeCode && matrixItem.Attribute6 == attributes[i].AttributeValueCode) {
                count++;
            }
        }
        return count == attributes.length;
    }

    function findMatrixItems(matrixItems, itemCode) {
        for (var i = 0; i < matrixItems.length; i++) {
            if (matrixItems[i].MatrixItemCode == itemCode) {
                return matrixItems[i];
            }
        }
        return null;
    }

    function getCurrentBundleTotalPrice() {
        var totalPrice = 0;
        $.each(this.items, function (index, item) {
            if (item.ItemType == 'Matrix Group') {
                var selectedAttributes = item.CallBackGetSelectedElementsAttributes();
                var selectedMatrixItem = getMatrixItemBySelectedAttributes(item, selectedAttributes);
                if (selectedMatrixItem != null) {
                    totalPrice += selectedMatrixItem.FinalPrice;
                } else {
                    totalPrice += item.SalesPrice;
                }
            } else {
                totalPrice += item.SalesPrice;
            }
        });
        return { value: totalPrice, display: getAmountCustomerCurrencyFormat(totalPrice) };
    }

    function getAmountCustomerCurrencyFormat(amount) {
        var value = '';
        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetAmountCustomerCurrencyFormat",
            data: JSON.stringify({ amount: amount }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                value = result.d;
            },
            fail: function (result) { }
        });
        return value;
    }

    //*****End Functions



    return {
        bundle: bundle,
        settings: settings,
        items: items,

        addItem: addItem,
        getMatrixItemAttributes: getMatrixItemAttributes,
        getAttributes: getAttributes,
        getMatrixItemBySelectedAttributes: getMatrixItemBySelectedAttributes,
        getItemByLineNum: getItemByLineNum,
        generateMatrixGroupAttributes: generateMatrixGroupAttributes,
        getSelectedMatrixItem: getSelectedMatrixItem,
        getFreeStock: getFreeStock,
        validate: validate,
        getItemSalesPrice: getItemSalesPrice,
        doesBundleHasStock: doesBundleHasStock,
        doesMatrixItemHasAttribute: doesMatrixItemHasAttribute,
        findMatrixItems: findMatrixItems,
        getCurrentBundleTotalPrice: getCurrentBundleTotalPrice,
        getAmountCustomerCurrencyFormat: getAmountCustomerCurrencyFormat

    }
};


var BundlePage = function () {
    //*********Private
    var _that = this,
        _this = {},
        bundle = new BundleProduct(),
        items = [],
        shoppingCartMatrixItems = [],
        wishlistMatrixItems = [],
        modalDetails = null,
        modalDetailsButtons = [],
        shoppingCartObjects = {},
        wishlistObjects = {},
        productPageModalSource = null
    ;

    //*********Product Page
    function setupProductPage() {

        _this.settings.TargetBundlePrice = $('#bundle-product-price-' + _this.item.counter);
        _this.settings.TargetBundleQuantity = $('#Quantity' + _this.item.counter);

        bundle.settings = _this.settings;
        bundle.bundle =
            {
                Guid: _this.item.guid,
                Counter: _this.item.counter,
                BundleCode: _this.item.bundleCode,
                TotalPrice: _this.item.totalPrice,
                DisplayTotalPrice: _this.item.displayTotalPrice,
                MiniCartImageSrc: _this.item.miniCartImageSrc,
                IconImageSrc: _this.item.iconImageSrc,
                MediumImageSrc: _this.item.mediumImageSrc
            };

        //modal
        _this.item.bundleModalID = '#bundle-product-modal-' + _this.item.counter;
        setupModalDetails();

        $('#btn-bundle-view-details-' + _this.item.counter).click(function (e) {
            productPageModalSource = null;
            modalDetails.dialog('open');
        });

        var lblProductPrice = $('span#lblPrice_' + _this.item.counter);
        lblProductPrice.empty();
        lblProductPrice.text(_this.item.displayTotalPrice);
        lblProductPrice.load(function () {
            lblProductPrice.text(_this.item.displayTotalPrice);
        });

        var formAddToCart = $('#AddToCartForm_' + _this.item.counter);
        var formAction = formAddToCart.attr('action');
        formAction = formAction.replace("addtocart.aspx", "addbundletocart.aspx");
        formAddToCart.attr('action', formAction);

        var buttonAddToCart = $('#AddToCart_' + _this.item.counter);
        buttonAddToCart[0].type = "button";
        buttonAddToCart[0].onclick = null;
        buttonAddToCart.click(function (e) {
            var source = 'addtocart';
            productPageModalSource = source;
            productPageSubmit(source, formAddToCart);
        });

        var buttonAddToWishList = $('#AddToWishList_' + _this.item.counter);
        buttonAddToWishList[0].type = "button";
        buttonAddToWishList[0].onclick = null;
        buttonAddToWishList.click(function (e) {
            var source = 'addtowishlist';
            productPageModalSource = source;
            productPageSubmit(source, formAddToCart);
        });


        modalDetailsButtons.push(
            {
                text: "Close",
                "class": 'btn btn-default',
                click: function () {
                    if (productPageModalSource != null) {
                        productPageSubmit(productPageModalSource, formAddToCart);
                    }
                    else {
                        $(this).dialog('close');
                    }
                }
            }
        );
        modalDetails.dialog('option', { buttons: modalDetailsButtons });
        displayMatrixItemsAttribute();
        setMatrixGroupErrorContainer();
        updateCurrentBundlePrice(bundle.getCurrentBundleTotalPrice());
    }
    function productPageSubmit(source, form) {
        var formAction = form.attr('action');
        var validate = bundle.validate();

        if (validate.isValid) {
            var quantity = $('#Quantity' + _this.item.counter);
            var params = { Items: validate.items, BundleCode: validate.bundleCode };
            params = JSON.stringify(params);
            var decodedParams = decodeURIComponent(params);
            formAction = decodeURIComponent(formAction);

            if (formAction.indexOf("Items[]") > -1) {
                formAction = formAction.substring(0, formAction.indexOf("Items[]"));
            }

            if (formAction[formAction.length - 1] != "&") {
                formAction += "&";
            }

            formAction = formAction.replace("addtocart.aspx", "addbundletocart.aspx");
            formAction += 'Bundle=' + decodedParams;
            formAction += '&Source=' + _this.pageSource;
            formAction += '&Quantity=' + quantity.val();
            formAction += '&ClickSource=' + source;
            form.attr('action', formAction);
            form.submit();
        }
        else {
            if (modalDetails.dialog("isOpen") == true) {
                modalDetails.dialog('close');
            } else {
                modalDetails.dialog('open');
            }

        }
    }

    function isAttributesHasSingleValues(item) {
        var hasSingleValue = true;
        for (var ctr = 0; ctr < item.Attributes.length; ctr++) {
            if (item.Attributes[ctr].Values.length > 1) {
                hasSingleValue = false;
                break;
            }
        }
        return hasSingleValue;
    }

    function displayAttributeProductPage(item) {
        var hasSingleValue = isAttributesHasSingleValues(item);
        var listAttributeElements = [];

        for (var ctr = 0; ctr < item.Attributes.length; ctr++) {
            var attribute = item.Attributes[ctr];
            var positionID = attribute.PositionID;
            var id = 'select-matrix-attribute-' + _this.item.counter;
            var select = $('<select/>').attr(
            {
                id: id + '-' + item.LineNum + '-' + positionID + '-' + item.UniqueID,
                customDataPositionID: positionID,
                customDataAttributeCode: attribute.AttributeCode,
                customDataTarget: ctr == item.Attributes.length - 1 ? null : id + '-' + item.LineNum + '-' + (positionID + 1) + '-' + item.UniqueID,
                customDataByPassChange: false
            });
            var optionDefault = $('<option/>').text('Select ' + attribute.AttributeDescription).val(attribute.AttributeCode);
            select.append(optionDefault);
            item.ElementsAttributes.push(select);

            if (ctr > 0) {
                // select.prop('disabled', 'disabled');
            }
            else {
                for (var valueIndex = 0; valueIndex < attribute.Values.length; valueIndex++) {
                    var option = $('<option/>').text(attribute.Values[valueIndex].AttributeValueDescription).val(attribute.Values[valueIndex].AttributeValueCode);
                    select.append(option);
                }
            }

            select.bind("change", function (e) { selectMatrixGroupChanged($(this), item.LineNum, item, false); });
            $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.LineNum + '-' + item.UniqueID).append($("<div class='bundle-matrix-group-attribute-elements-container' />").append(select));

            listAttributeElements.push(select);
        }

        listAttributeElements.forEach(function (select) {
            $('#' + select.attr('id') + ' option:eq(1)').attr('selected', 'selected');
            select.change();
        });



    }

    //*********ShoppingCart Page
    function setupShoppingCartPage() {

        _this.settings.TargetBundlePrice = $('#bundle-total-price-' + _this.item.bundleCode + '-' + _this.item.bundleHeaderID);
        _this.settings.TargetBundlePriceExt = $('#bundle-total-price-ext-' + _this.item.bundleCode + '-' + _this.item.bundleHeaderID);
        _this.settings.TargetBundleQuantity = $('#Quantity_' + _this.item.bundleCode + '_' + _this.item.bundleHeaderID);

        _this.item.bundleModalID = '#bundle-product-modal-' + _this.item.counter + '-' + _this.item.bundleHeaderID;

        bundle.settings = _this.settings;
        bundle.bundle =
            {
                Guid: _this.item.guid,
                Counter: _this.item.counter,
                BundleCode: _this.item.bundleCode,
                TotalPrice: _this.item.totalPrice,
                DisplayTotalPrice: _this.item.displayTotalPrice,
                MiniCartImageSrc: _this.item.miniCartImageSrc,
                IconImageSrc: _this.item.iconImageSrc,
                MediumImageSrc: _this.item.mediumImageSrc
            };

        setupModalDetails();

        if (hasMatrixItems()) {
            modalDetailsButtons.push({
                text: "Update",
                "class": 'btn btn-default',
                click: shoppingCartPageSubmit
            });
        }

        modalDetailsButtons.push({
            text: "Close",
            "class": 'btn btn-default',
            click: function () { $(this).dialog('close'); }
        });

        $(_this.item.bundleModalID).dialog('option', { buttons: modalDetailsButtons });

        $('#btn-bundle-view-details-' + _this.item.counter + '-' + _this.item.bundleHeaderID).click(function (e) {

            modalDetails.dialog('open');
        });



        shoppingCartObjects.bundleModalFormUpdateID = '#form-bundle-update-' + _this.item.counter + '-' + _this.item.bundleHeaderID;
        shoppingCartObjects.shoppingCartDisplayedMatrixGroups = [];



        //var formUpdate = $('<form/>').attr({ 'id': shoppingCartObjects.bundleModalFormUpdateID.replace('#', ''), 'method': 'post' });
        //formUpdate.appendTo($(shoppingCartObjects.bundleModalFormUpdateID));

        displayMatrixItemsAttribute();
        setMatrixGroupErrorContainer();
    }
    function shoppingCartPageSubmit() {
        var validate = bundle.validate();
        if (validate.isValid) {
            var quantity = $('#Quantity_' + _this.item.bundleCode + '_' + _this.item.bundleHeaderID);
            var formAction = "addbundletocart.aspx?ReturnUrl=shoppingCart.aspx&ProductID=" + _this.item.counter;
            var params = { Items: validate.items, BundleCode: validate.bundleCode };
            params = JSON.stringify(params);
            var decodedParams = decodeURIComponent(params);
            formAction = decodeURIComponent(formAction);

            if (formAction.indexOf("Items[]") > -1) {
                formAction = formAction.substring(0, formAction.indexOf("Items[]"));
            }

            if (formAction[formAction.length - 1] != "&") {
                formAction += "&";
            }

            formAction += 'Bundle=' + decodedParams;
            formAction += '&Source=' + _this.pageSource;
            formAction += '&Quantity=' + quantity.val();
            formAction += '&BundleHeaderID=' + _this.item.bundleHeaderID;

            var form = $('<form/>');
            form.attr({ action: formAction, method: 'POST' });
            form.appendTo('body').submit();
            // $(this).dialog('close');
        }
    }
    function addShoppingCartItem(item) {
        if (item) {
            shoppingCartMatrixItems.push(item);
        }
    }

    //*********Wishlist Page
    function setupWishlistPage() {
        _this.item.bundleModalID = '#bundle-product-modal-' + _this.item.counter + '-' + _this.item.bundleHeaderID;
        bundle.settings = _this.settings;
        bundle.bundle =
            {
                Guid: _this.item.guid,
                Counter: _this.item.counter,
                BundleCode: _this.item.bundleCode,
                TotalPrice: _this.item.totalPrice,
                DisplayTotalPrice: _this.item.displayTotalPrice,
                MiniCartImageSrc: _this.item.miniCartImageSrc,
                IconImageSrc: _this.item.iconImageSrc,
                MediumImageSrc: _this.item.mediumImageSrc
            };

        setupModalDetails();

        if (hasMatrixItems()) {
            modalDetailsButtons.push({
                text: "Update",
                "class": 'btn btn-default',
                click: wishlistPageSubmit
            });
        }

        modalDetailsButtons.push({
            text: "Close",
            "class": 'btn btn-default',
            click: function () { $(this).dialog('close'); }
        });

        $(_this.item.bundleModalID).dialog('option', { buttons: modalDetailsButtons });

        $('#btn-bundle-view-details-' + _this.item.counter + '-' + _this.item.bundleHeaderID).click(function (e) {
            modalDetails.dialog('open');
        });

        displayMatrixItemsAttribute();
        setMatrixGroupErrorContainer();
    }
    function wishlistPageSubmit() {
        var validate = bundle.validate();
        if (validate.isValid) {
            var quantity = $('#Quantity_' + _this.item.bundleCode + '_' + _this.item.bundleHeaderID);
            var formAction = "addbundletocart.aspx?ReturnUrl=shoppingCart.aspx&ProductID=" + _this.item.counter;
            var params = { Items: validate.items, BundleCode: validate.bundleCode };
            params = JSON.stringify(params);
            var decodedParams = decodeURIComponent(params);
            formAction = decodeURIComponent(formAction);

            if (formAction.indexOf("Items[]") > -1) {
                formAction = formAction.substring(0, formAction.indexOf("Items[]"));
            }

            if (formAction[formAction.length - 1] != "&") {
                formAction += "&";
            }

            formAction += 'Bundle=' + decodedParams;
            formAction += '&Source=' + _this.pageSource;
            formAction += '&Quantity=' + quantity.val();
            formAction += '&BundleHeaderID=' + _this.item.bundleHeaderID;

            var form = $('<form/>');
            form.attr({ action: formAction, method: 'POST' });
            form.appendTo('body').submit();
            //  $(this).dialog('close');
        }
    }
    function addWishlistItem(item) {
        if (item) {
            wishlistMatrixItems.push(item);
        }
    }
    function displayAttributeWishlistPage(item) {
        var listSelect = $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID + ' select');
        for (var ctr = 0; ctr < item.Attributes.length; ctr++) {
            var attribute = item.Attributes[ctr];
            var positionID = attribute.PositionID;
            var id = 'select-matrix-attribute-' + _this.item.counter;
            var customDataTargetID = ctr == item.Attributes.length - 1 ? null : id + '-' + _this.item.bundleHeaderID + '-' + item.Counter + '-' + (positionID + 1) + '-' + item.UniqueID;
            var select = $('<select/>').attr(
            {
                id: id + '-' + _this.item.bundleHeaderID + '-' + item.Counter + '-' + positionID + '-' + item.UniqueID,
                customDataPositionID: positionID,
                customDataAttributeCode: attribute.AttributeCode,
                customDataTarget: customDataTargetID,
                customDataByPassChange: false
            });

            var optionDefault = $('<option/>').text('Select ' + attribute.AttributeDescription).val(attribute.AttributeCode);
            select.append(optionDefault);
            item.ElementsAttributes.push(select);

            var selectedFound = false;
            for (var valueIndex = 0; valueIndex < attribute.Values.length; valueIndex++) {
                var option = $('<option/>').text(attribute.Values[valueIndex].AttributeValueDescription).val(attribute.Values[valueIndex].AttributeValueCode);
                select.append(option);
            }

            select.bind("change", function (e) { selectMatrixGroupChanged($(this), item.LineNum, item, false); });
            if (item.Attributes.length - 1 == ctr) {
                selectMatrixGroupChanged(select, item.LineNum, item, true);
            }

            //bundle-matrix-group-attribute-container-{../../Counter}-{Counter}-{../../BundleHeaderID}-{UniqueID}
            $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID)
                .append($("<div class='bundle-matrix-group-attribute-elements-container' />").append(select));
        }

        var shoppingCartRecID = null;
        for (var elementIndex = 0; elementIndex < item.ElementsAttributes.length; elementIndex++) {
            var select = item.ElementsAttributes[elementIndex];
            for (var cartIndex = 0; cartIndex < wishlistMatrixItems.length; cartIndex++) {
                if (wishlistMatrixItems[cartIndex].itemCode == item.ItemCode && wishlistMatrixItems[cartIndex].isDisplayed == false) {
                    if (shoppingCartRecID == null) {
                        shoppingCartRecID = wishlistMatrixItems[cartIndex].shoppingCartRecID;
                    }

                    var matrixItemFound = bundle.findMatrixItems(item.MatrixItems, wishlistMatrixItems[cartIndex].bundleMatrixItemSelected);
                    if (matrixItemFound != null) {
                        var matrixItemAttributesFound = bundle.getAttributes(matrixItemFound);
                        for (var matrixItemAttributesFoundIndex = 0; matrixItemAttributesFoundIndex < matrixItemAttributesFound.length; matrixItemAttributesFoundIndex++) {
                            if (select.attr('value') != matrixItemAttributesFound[matrixItemAttributesFoundIndex].AttributeCode) continue;
                            select.val(matrixItemAttributesFound[matrixItemAttributesFoundIndex].AttributeValueCode);
                        }
                    }
                    break;
                }
            }
            if (item.ElementsAttributes.length - 1 == elementIndex) {
                selectMatrixGroupChanged(select, item.LineNum, item, true);
            }
        }

        if (shoppingCartRecID != null) {
            for (var cartIndex = 0; cartIndex < wishlistMatrixItems.length; cartIndex++) {
                if (shoppingCartRecID == wishlistMatrixItems[cartIndex].shoppingCartRecID) {
                    wishlistMatrixItems[cartIndex].isDisplayed = true;
                    item["ShoppingCartRecID"] = wishlistMatrixItems[cartIndex].shoppingCartRecID;
                    break;
                }
            }
        }
    }


    function getPageSourceTypes() {
        return { product: function () { return "productpage"; }, shoppingCart: function () { return "shoppingcart"; }, wishlist: function () { return "wishlist"; } };
    }

    function addItem(item) {
        if (item) {
            item.CallBackGetSelectedElementsAttributes = getSelectedElementsAttributes;
            items.push(item);
            bundle.addItem(item);
        }
    }

    function setMatrixGroupErrorContainer() {
        for (var ctr = 0; ctr < items.length; ctr++) {
            if (items[ctr].ItemType == "Matrix Group") {
                if (_this.pageSource == getPageSourceTypes().product()) {
                    items[ctr].ElementErrorContainer = { element: $('#error-message-container-' + _this.item.counter + '-' + items[ctr].LineNum), property: 'text' };
                } else if (_this.pageSource == getPageSourceTypes().shoppingCart()) {
                    items[ctr].ElementErrorContainer = { element: $('#error-message-container-' + _this.item.counter + '-' + items[ctr].Counter + _this.item.bundleHeaderID), property: 'text' };
                } else if (_this.pageSource == getPageSourceTypes().wishlist()) {
                    items[ctr].ElementErrorContainer = { element: $('#error-message-container-' + _this.item.counter + '-' + items[ctr].Counter + _this.item.bundleHeaderID), property: 'text' };
                }
            }
        }

    }

    function getSelectedElementsAttributes() {
        var selectedAttributes = [];
        for (var i = 0; i < this.ElementsAttributes.length; i++) {
            var code = $(this.ElementsAttributes[i].children()[0]).val();
            var value = this.ElementsAttributes[i].find(":selected").val();
            if (code == value) { continue; }
            selectedAttributes.push({ AttributeCode: code, AttributeValueCode: value });
        }
        return selectedAttributes;
    }

    function getItems() {
        return items;
    }

    function hasMatrixItems() {
        for (var ctr = 0; ctr < items.length; ctr++) {
            if (items[ctr].ItemType == "Matrix Group") {
                return true;
            }
        }
    }

    function displayMatrixItemsAttribute() {
        for (var ctr = 0; ctr < items.length; ctr++) {
            if (items[ctr].ItemType == "Matrix Group") {
                if (_this.pageSource == getPageSourceTypes().product()) {
                    displayAttributeProductPage(items[ctr]);
                } else if (_this.pageSource == getPageSourceTypes().shoppingCart()) {
                    displayAttributeShoppingCartPage(items[ctr]);
                } else if (_this.pageSource == getPageSourceTypes().wishlist()) {
                    displayAttributeWishlistPage(items[ctr]);
                }
            }
        }
    }

    function displayAttributeShoppingCartPage(item) {
        var listSelect = $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID + ' select');
        for (var ctr = 0; ctr < item.Attributes.length; ctr++) {
            var attribute = item.Attributes[ctr];
            var positionID = attribute.PositionID;
            var id = 'select-matrix-attribute-' + _this.item.counter;
            var customDataTargetID = ctr == item.Attributes.length - 1 ? null : id + '-' + _this.item.bundleHeaderID + '-' + item.Counter + '-' + (positionID + 1) + '-' + item.UniqueID;
            var select = $('<select/>').attr(
            {
                id: id + '-' + _this.item.bundleHeaderID + '-' + item.Counter + '-' + positionID + '-' + item.UniqueID,
                customDataPositionID: positionID,
                customDataAttributeCode: attribute.AttributeCode,
                customDataTarget: customDataTargetID,
                customDataByPassChange: false
            });

            var optionDefault = $('<option/>').text('Select ' + attribute.AttributeDescription).val(attribute.AttributeCode);
            select.append(optionDefault);
            item.ElementsAttributes.push(select);

            var selectedFound = false;
            for (var valueIndex = 0; valueIndex < attribute.Values.length; valueIndex++) {
                var option = $('<option/>').text(attribute.Values[valueIndex].AttributeValueDescription).val(attribute.Values[valueIndex].AttributeValueCode);
                select.append(option);
            }

            select.bind("change", function (e) { selectMatrixGroupChanged($(this), item.LineNum, item, false); });
            if (item.Attributes.length - 1 == ctr) {
                selectMatrixGroupChanged(select, item.LineNum, item, true);
            }

            //bundle-matrix-group-attribute-container-{../../Counter}-{Counter}-{../../BundleHeaderID}-{UniqueID}
            $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID)
                .append($("<div class='bundle-matrix-group-attribute-elements-container' />").append(select));
        }

        var shoppingCartRecID = null;
        for (var elementIndex = 0; elementIndex < item.ElementsAttributes.length; elementIndex++) {
            var select = item.ElementsAttributes[elementIndex];
            for (var cartIndex = 0; cartIndex < shoppingCartMatrixItems.length; cartIndex++) {
                if (shoppingCartMatrixItems[cartIndex].itemCode == item.ItemCode && shoppingCartMatrixItems[cartIndex].isDisplayed == false) {
                    if (shoppingCartRecID == null) {
                        shoppingCartRecID = shoppingCartMatrixItems[cartIndex].shoppingCartRecID;
                    }

                    var matrixItemFound = bundle.findMatrixItems(item.MatrixItems, shoppingCartMatrixItems[cartIndex].bundleMatrixItemSelected);
                    if (matrixItemFound != null) {
                        var matrixItemAttributesFound = bundle.getAttributes(matrixItemFound);
                        for (var matrixItemAttributesFoundIndex = 0; matrixItemAttributesFoundIndex < matrixItemAttributesFound.length; matrixItemAttributesFoundIndex++) {
                            if (select.attr('value') != matrixItemAttributesFound[matrixItemAttributesFoundIndex].AttributeCode) continue;
                            select.val(matrixItemAttributesFound[matrixItemAttributesFoundIndex].AttributeValueCode);
                        }
                    }
                    break;
                }
            }
            if (item.ElementsAttributes.length - 1 == elementIndex) {
                selectMatrixGroupChanged(select, item.LineNum, item, true);
            }
        }

        if (shoppingCartRecID != null) {
            for (var cartIndex = 0; cartIndex < shoppingCartMatrixItems.length; cartIndex++) {
                if (shoppingCartRecID == shoppingCartMatrixItems[cartIndex].shoppingCartRecID) {
                    shoppingCartMatrixItems[cartIndex].isDisplayed = true;
                    item["ShoppingCartRecID"] = shoppingCartMatrixItems[cartIndex].shoppingCartRecID;
                    break;
                }
            }
        }
    }

    function resetSelectMatrixGroup(element) {
        var target = element.attr('customDataTarget');

        if (target != null) {
            $('#' + target).prop('disabled', 'disabled');
            $('#' + target).find("option:gt(0)").remove();
            resetSelectMatrixGroup($('#' + target));
        }
    }

    function selectMatrixGroupChanged(element, lineNum, item, includeLastElement) {
        if (element.attr('customDataByPassChange') == true) { return; }
        if (item.ElementErrorContainer != undefined) {
            item.ElementErrorContainer.element.hide();
        }
        var selectedValue = element.val();
        var targetElement = $('#' + element.attr('customDataTarget'));
        var listSelect = null;

        if (_this.pageSource == getPageSourceTypes().product()) {
            listSelect = $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + lineNum + '-' + item.UniqueID + ' select:not([disabled])');
        } else if (_this.pageSource == getPageSourceTypes().shoppingCart()) {
            listSelect = $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID + ' select:not([disabled])');
        } else if (_this.pageSource == getPageSourceTypes().wishlist()) {
            listSelect = $(_this.item.bundleModalID + ' #bundle-matrix-group-attribute-container-' + _this.item.counter + '-' + item.Counter + '-' + _this.item.bundleHeaderID + '-' + item.UniqueID + ' select:not([disabled])');
        }

        if (listSelect == null)
            return;

        resetSelectMatrixGroup(element);

        if (element.attr('customDataAttributeCode') == element.val())
            return;

        var selectedAttributes = [];

        for (var ctr = 0; ctr < listSelect.length; ctr++) {
            var attributeCode = $(listSelect[ctr]).attr('customDataAttributeCode');
            var attributeValueCode = $(listSelect[ctr]).val();
            if (attributeCode != attributeValueCode) {
                selectedAttributes.push({ AttributeCode: attributeCode, AttributeValueCode: $(listSelect[ctr]).val() });
            }
        }

        if (includeLastElement == true)
            selectedAttributes.push({ AttributeCode: element.attr('customDataAttributeCode'), AttributeValueCode: element.val() });

        var positionID = element.attr('customDataPositionID');

        var hasSingleValue = isAttributesHasSingleValues(item);

        if (positionID < item.Attributes.length && positionID != 0) {
            var attributeCode = item.Attributes[positionID].AttributeCode;
            for (var valueIndex = 0; valueIndex < item.Attributes[positionID].Values.length; valueIndex++) {
                var valueCode = item.Attributes[positionID].Values[valueIndex].AttributeValueCode;
                var valueDesc = item.Attributes[positionID].Values[valueIndex].AttributeValueDescription;
                selectedAttributes.push({ AttributeCode: attributeCode, AttributeValueCode: valueCode });

                for (var matrixItemIndex = 0; matrixItemIndex < item.MatrixItems.length; matrixItemIndex++) {
                    if (bundle.doesMatrixItemHasAttribute(item.MatrixItems[matrixItemIndex], selectedAttributes)) {
                        var option = $('<option/>').text(valueDesc).val(valueCode);
                        $(targetElement).append(option);
                        if (hasSingleValue === true && matrixItemIndex === 0) {
                            option.attr('selected', 'selected');
                        }
                        break;
                    }
                }
                selectedAttributes.splice(selectedAttributes.length - 1, 1);
            }
        }
        else {
            for (var matrixItemIndex = 0; matrixItemIndex < item.MatrixItems.length; matrixItemIndex++) {
                if (bundle.doesMatrixItemHasAttribute(item.MatrixItems[matrixItemIndex], selectedAttributes)) {
                    if (bundle.doesMatrixItemHasAttribute(item.MatrixItems[matrixItemIndex], selectedAttributes)) {
                        var matrixItem = item.MatrixItems[matrixItemIndex];

                        var freeStock = bundle.getFreeStock(matrixItem.MatrixItemCode, item.UnitMeasureCode);

                        $(_this.item.bundleModalID + ' #img-bundle-sub-product-image-modal-' + _this.item.counter + '-' + lineNum + '-' + item.UniqueID).attr({ src: matrixItem.IconImageSrc });
                        $(_this.item.bundleModalID + ' #img-bundle-sub-product-stockhint-' + _this.item.counter + '-' + lineNum + '-' + item.UniqueID).attr({ src: freeStock > 0 ? bundle.settings.StockHint.InStockSrc : bundle.settings.StockHint.OutOfStockSrc });
                        $(_this.item.bundleModalID + ' #final-price-' + item.Counter + '-' + item.UniqueID).text(matrixItem.DisplayFinalPrice);
                    }
                    break;
                }
            }
        }

        var currentTotalPrice = bundle.getCurrentBundleTotalPrice();
        updateCurrentBundlePrice(currentTotalPrice);


        if (hasSingleValue === true) {
            $(targetElement).removeAttr('disabled');
            $(targetElement).change();
        } else {
            $(targetElement).prop('disabled', null);
        }
    }

    function updateCurrentBundlePrice(currentTotalPrice) {

        if (_this.pageSource == getPageSourceTypes().product()) {
            bundle.settings.TargetBundlePrice.text(currentTotalPrice.display);
        } else if (_this.pageSource == getPageSourceTypes().shoppingCart()) {
            bundle.settings.TargetBundlePrice.text(currentTotalPrice.display);
            var quantity = parseInt(bundle.settings.TargetBundleQuantity.val());
            bundle.settings.TargetBundlePriceExt.text(bundle.getAmountCustomerCurrencyFormat(currentTotalPrice.value * quantity));
        }
    }

    function setupModalDetails() {
        modalDetails = $(_this.item.bundleModalID).dialog({
            width: 800,
            autoOpen: false,
            modal: true,
            draggable: false,
            resizable: true,
            title: "<h1 style='margin: 0px;'>Bundle Items</h1>",
        });
    }

    function init(pageSource) {
        _this.pageSource = pageSource;

        if (_this.pageSource == getPageSourceTypes().product()) {
            setupProductPage();
        } else if (_this.pageSource == getPageSourceTypes().shoppingCart()) {
            setupShoppingCartPage();
        } else if (_this.pageSource == getPageSourceTypes().wishlist()) {
            setupWishlistPage();
        }
        else {
            throw new Error("pageSource is invalid.");
        }
    }

    //*********Public
    _this.init = init;
    _this.pageSource = null;
    _this.item = {};
    _this.item.counter = null;
    _this.item.bundleCode = null;
    _this.item.guid = null;
    _this.item.totalPrice = null;
    _this.item.displayTotalPrice = null;
    _this.item.miniCartImageSrc = null;
    _this.item.iconImageSrc = null;
    _this.item.mediumImageSrc = null;
    _this.item.bundleModalID = null;
    _this.item.bundleHeaderID = null;
    _this.item.add = addItem;
    _this.item.addShoppingCartItem = addShoppingCartItem;
    _this.item.addWishlistItem = addWishlistItem;
    _this.settings = {};
    return _this;
};