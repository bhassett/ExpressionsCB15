//How to use
//var bundleProduct1 = new BundleProduct();
//var bundleProduct2 = new BundleProduct();

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
                var selectedAttributes = this.items[i].callBackGetSelectedElementsAttributes();
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

        returnObj["isValid"] = originalPrice >= currentPrice && checkItemCounter == this.items.length ;//&& itemsWithZeroStock.length < 1;
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
                var selectedAttributes = this.items[i].callBackGetSelectedElementsAttributes();
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
        $.each(this.items, function( index, item ){
            if (item.ItemType == 'Matrix Group') {
                var selectedAttributes = item.callBackGetSelectedElementsAttributes();
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