/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $("#ordersTable").data('aff');

if (DISPLAYORDERS === null || typeof DISPLAYORDERS === 'undefined')
    var DISPLAYORDERS = {}; // create namespace - object to holds all references and methods.

var DO = DISPLAYORDERS; // alias for code brevity

// document.ready function
$(function () {

    DO.primeDomVariables();
    DO.wireUpHandlers();
    DO.wireUpDataTable();

});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.primeDomVariables = function() {
        DO.ordersTable = $('#ordersTable');
        DO.webinarIdDiv = $('#webinarIdDiv');
    };

    ns.wireUpHandlers = function() {

    };

    ns.wireUpDataTable = function() {

            DO.ordersTable.dataTable({
                "ajax": {
                    "url": "/admin/GetGridData",
                    "data": {
                        "webinarId": parseInt(DO.webinarIdDiv.text())
        },
                "type": "POST"
            },
            "columns": [
                { "data": "OrderColumn" },
                
                { "data": "UserColumn" },
                { "data": "InstitutionColumn" },
                { "data": "BillingColumn" },
                {
                    "data": "AffiliateColumn",
                    "visible": aff
                },
                { "data": "OrderDateColumn" },
                { "data": "StatusColumn" },
            ]
        });
    };

})(DO);

$.fn.changeOrDelayedKey = function (fn, iKeyDelay, sKeyEvent) {
    var iTimeoutId,
        oEventData;

    // second signature used, update the variables
    if (!$.isFunction(fn)) {
        oEventData = arguments[0];
        fn = arguments[1];
        iKeyDelay = arguments[2];
        sKeyEvent = arguments[3];
    }

    if (!iKeyDelay || 0 > iKeyDelay) {
        iKeyDelay = 500;
    }

    if (!sKeyEvent || !this[sKeyEvent]) {
        sKeyEvent = 'keydown';
    }

    // non-delayed event callback, should clear any timeouts, then
    // call the original callback function
    function fnExecCallback() {
        clearTimeout(iTimeoutId);
        fn.apply(this, arguments);
    }

    // delayed event callback, should call the non-delayed callback
    // after a short interval
    function fnDelayCallback() {
        var that = this,
            args = arguments;
        clearTimeout(iTimeoutId);
        iTimeoutId = setTimeout(function () {
            fnExecCallback.apply(that, args);
        }, iKeyDelay);
    }

    if (oEventData) {
        this.change(oEventData, fnExecCallback);
        this[sKeyEvent](oEventData, fnDelayCallback);
    }
    else {
        this.change(fnExecCallback);
        this[sKeyEvent](fnDelayCallback);
    }

    return this;
};

