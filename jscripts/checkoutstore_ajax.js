// register namespace
Type.registerNamespace('ise.Pages');

ise.Pages.CheckouStore = {
    initialize: function () {
        this.shippingMethodControls = new Array();
        this.form = null;
        var thisObject = this;
        $(document).ready(function () {
            thisObject.showLoader();
            thisObject.initializeLoaderListener();
        });
    },
    
    addShippingMethodControlId : function(id) {
        var del = Function.createDelegate(this, this.addShippingMethodControl);
        this.loadControl(id, ise.Controls.ShippingMethodController, del);
    },
    
    addShippingMethodControl : function(control) {
        if (control) {
            this.shippingMethodControls.push(control);
            var del = Function.createDelegate(this, this.shippingOptionsLoadedEventHandler);
            control.addShippingOptionsLoadedEventHandler(del);
        }
    },
   
    loadControl : function(id, controller, setControlDelegate) {
        var control = controller.getControl(id);
        if(control) {
            setControlDelegate(control);
        }
        else {
            var observer = { 
                notify : function(control) {
                    if(control) {
                        if(control.id == id) { setControlDelegate(control); }
                    }
                }
            }
            controller.addObserver(observer);
        }
    },

    showLoader: function () { $("body").data("globalLoader").show(); },

    initializeLoaderListener: function () {
        var thisObject = this;
        $(window).load(function () {
            var checkerID = 0;
            var checker = function () {
                var shippingMethodControls = thisObject.shippingMethodControls;
                if (shippingMethodControls != null) {
                    if (shippingMethodControls.length > 0)
                    {
                        if (shippingMethodControls[0].isShippingOnDemand == "true") {
                            if (thisObject.isShippingMethodsDoneGettingRates()) {
                                $("body").data("globalLoader").hide();
                                clearInterval(checkerID);
                            }
                        }
                        else {
                            if (thisObject.isShippingMethodsAlreadyLoaded()) {
                                $("body").data("globalLoader").hide();
                                clearInterval(checkerID);
                            }
                        }
                    }
                }
                // Added to support Free Shipping - no shipping method control
                else {
                    $("body").data("globalLoader").hide();
                    clearInterval(checkerID);
                }
            }
            checkerID = setInterval(checker, 200);
        });
    },

    isShippingMethodsDoneGettingRates: function () {
        var allDone = false;
        if (this.shippingMethodControls[0].isShippingOnDemand == 'true') {
            for (var i = 0; i < this.shippingMethodControls.length; i++) {
                if (this.shippingMethodControls[i].isAutoClickOfOption) {
                    allDone = false;
                    break;
                }
                allDone = true;
            }
        }
        else {
            allDone = true;
        }
        return allDone;
    },

    isShippingMethodsAlreadyLoaded: function () {
        var allDone = false;
        for (var i = 0; i < this.shippingMethodControls.length; i++) {
            if (!this.shippingMethodControls[i].shippingMethodLoadingComplete) {
                allDone = false;
                break;
            }
            allDone = true;
        }
        return allDone;
    },

    setForm : function(id) {
        this.form = $getElement(id);
        if(this.form) {
            this.form.onsubmit = Function.createDelegate(this, this.validate);           
            $disableSubmit(this.form);
        }
    },
       
    shippingOptionsLoadedEventHandler : function() { $enableSubmit(this.form); },
    
    clearValidations: function () {
        for (var i = 0; i < this.shippingMethodControls.length; i++) {
            this.shippingMethodControls[i].clearValidationSummary();
        }
    },
    
    validate: function () {
        var isValid = true;
        var clear = false;
        this.clearValidations();
        for (var i = 0; i < this.shippingMethodControls.length; i++) {
            var result = this.shippingMethodControls[i].validate(clear);
            if (!result) { isValid = false; }
        }
        return isValid;
    }
}
ise.Pages.CheckouStore.initialize();
