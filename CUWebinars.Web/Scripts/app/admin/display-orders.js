/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

if (DISPLAYORDERS === null || typeof DISPLAYORDERS === 'undefined')
    var DISPLAYORDERS = {}; // create namespace - object to holds all references and methods.

var DO = DISPLAYORDERS; // alias for code brevity

// document.ready function
$(function () {

    DO.primeDomVariables();
    DO.wireUpHandlers();

    $('#ordersTable').dataTable({
        "processing": true,
        "serverSide": true,
        "pagingType":"full_numbers",
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
});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.primeDomVariables = function() {

    };

    ns.wireUpHandlers = function() {

    };

})(DO);