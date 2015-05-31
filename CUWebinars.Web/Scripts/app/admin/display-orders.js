/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

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
    };

    ns.wireUpHandlers = function() {

    };

    ns.wireUpDataTable = function() {

        DO.ordersTable.dataTable({
            "processing": true,
            "serverSide": true,
            "pagingType": "full_numbers",
            "ajax": {
                "url": "GetGridData",
                "type": "POST"
            },
            "columns": [
                { "data": "OrderId" },
                { "data": "FirstName" },
                { "data": "LastName" },
                { "data": "Institution" },
                { "data": "BillingPhone" },
                { "data": "BillingEmail" },
                { "data": "BillingAddress" },
                { "data": "BillingAddress2" },
                { "data": "BillingCity" },
                { "data": "BillingState" },
                { "data": "BillingZip" },
                { "data": "ShippingFirstName" },
                { "data": "ShippingLastName" },
                { "data": "ShippingPhone" },
                { "data": "ShippingAddress" },
                { "data": "ShippingAddress2" },
                { "data": "ShippingCity" },
                { "data": "ShippingState" },
                { "data": "ShippingZip" }
            ]
        });

        DO.ordersTable.find('tfoot th:first').each(function () {
            var title = DO.ordersTable.find('thead th').eq($(this).index()).text();
            $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = DO.ordersTable.DataTable(); // note capital D - used to access DataTables API.

        table.columns(0).each(function() {

            var self = this;

            //DO.ordersTable.find('input', self.footer()).changeOrDelayedKey(table, function(e) {
            //    self.search(self.value).draw();
            //}, 500, 'keyup');
            DO.ordersTable.find('input:first', this.footer()).on('keyup change', function () {
                self.search(this.value).draw();
            });
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

