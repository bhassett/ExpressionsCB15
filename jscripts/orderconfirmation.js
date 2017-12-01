




var OrderConfirmationScript = function (salesOrderCode) {

    var self = {};
    self.salesOrderCode = salesOrderCode;

    self.init = function () {
        if (typeof (self.salesOrderCode) === 'undefined' || self.salesOrderCode === null || self.salesOrderCode === '') { return; }

     
        getSalesOrderDetails(self.salesOrderCode, function (data) {
           
            ga('require', 'ecommerce');

            var transaction = data.transaction;
            transaction = createTransaction(transaction.salesOrderCode, transaction.website, transaction.subTotal, transaction.freight, transaction.tax);

            ga('ecommerce:addTransaction', transaction);

            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];
                var ecommerceItem = createItem(transaction.id, item.itemDescription, item.itemName, item.category, item.price, item.quantity);
                ga('ecommerce:addItem', ecommerceItem);
            }

            ga('ecommerce:send');
           
         
          
             
        });
    };

    function sendGoogleEcommerceTracker(transaction, items) {


    }

    function createTransaction(salesOrderCode, storeName, total, shipping, tax) {
        return {
            'id': salesOrderCode,                     // Transaction ID. Required.
            'affiliation': storeName,   // Affiliation or store name.
            'revenue': total,               // Grand Total.
            'shipping': shipping,                  // Shipping.
            'tax': tax                 // Quantity.
        }
    }

    function createItem(salesOrderCode, itemDescription, itemName, categoryCode, price, quantity) {
        return {
            'id': salesOrderCode,                     // Transaction ID. Required.
            'name': itemDescription,    // Product name. Required.
            'sku': itemName,                 // SKU/code.
            'category': categoryCode,         // Category or variation.
            'price': price,                 // Unit price.
            'quantity': quantity                   // Quantity.
        }
    }

    function getSalesOrderDetails(salesOrderCode, callBack) {

        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetCustomerSalesOrderForTracking",
            data: JSON.stringify({ salesOrderCode: salesOrderCode }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                var data = JSON.parse(response.d);
                if (data.ok === true) {
                    callBack(data);
                } else {
                    console.log('GetCustomerSalesOrderForTracking: ' + data.message);
                }
            },
            error: function () { console.log('GetCustomerSalesOrderForTracking: Unable to fetch sales order details.'); }
        });
    }

    return self;
};