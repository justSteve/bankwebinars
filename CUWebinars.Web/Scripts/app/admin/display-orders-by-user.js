/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $("#ordersTable").data('aff');

if (DISPLAYORDERSBU === null || typeof DISPLAYORDERSBU === 'undefined')
    var DISPLAYORDERSBU = {}; // create namespace - object to holds all references and methods.

var DOBU = DISPLAYORDERSBU; // alias for code brevity

// document.ready function
$(function () {

    DOBU.primeDomVariables();
    DOBU.wireUpHandlers();
    DOBU.wireUpDataTable();

});

// self-invoking function for creating methods using Module pattern.
// 
(function (ns1) {

    ns1.primeDomVariables = function () {
        DOBU.ordersTable = $('#ordersTable');
        DOBU.userEmailDiv = $('#userEmailDiv');
    };

    ns1.wireUpHandlers = function () {

    };

    ns1.wireUpDataTable = function () {

        DOBU.ordersTable.dataTable({
            "dom": 'T<"clear">lfrtip'
            ,
            "tableTools": {
                "sSwfPath": "/swf/copy_csv_xls_pdf.swf"
            },
            "ajax": {
                "url": "/admin/GetOrdersByUser",
                "data": {
                    "email": DOBU.userEmailDiv.text()
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

})(DOBU);

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

