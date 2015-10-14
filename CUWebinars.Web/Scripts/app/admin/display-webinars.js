/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $('#webinarsSearchTable').data('aff');

if (DISPLAYWEBINARS === null || typeof DISPLAYWEBINARS === 'undefined')
    var DISPLAYWEBINARS = {}; // create namespace - object to holds all references and methods.

var DW = DISPLAYWEBINARS; // alias for code brevity

// document.ready function
$(function () {

    DW.primeDomVariables();
    DW.wireUpHandlers();
    DW.wireUpDataTable();



});

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.primeDomVariables = function () {
        DW.webinarsSearchTable = $('#webinarsSearchTable');
        //DW.connInfoTable = $('#connInfoTable');
        DW.searchTerm = $('#searchTerm');
    };

    ns.wireUpHandlers = function () {

    };

    ns.wireUpDataTable = function () {

        DW.webinarsSearchTable.dataTable({
            "dom": '<ilf<t>ip>',
            //'dom': 'T<"clear">lfrtip',

            'ajax': {
                'url': '/admin/GetWebinarSearchGridData',
                'data': {
                    'searchTerm': DW.searchTerm.text(),
                    'affiliateId': DW.affiliateId
                },
                'type': 'POST'
            },
            "columnDefs": [
                {
                    "render": function(data, type, row) {
                        return data + ' (' + row[1] + ')';
                    },
                    "targets": 0
                }
            ],
            'columns': [
                { 'data': 'WebinarID' },
                { 'data': 'OrdersColumn' },
                { 'data': 'PresenterColumn' },
                { 'data': 'TopicsColumn' },
                { 'data': 'AddToCartColumn' },
                { 'data': 'MediaColumn' },
                { 'data': 'ShareColumn' },
                { 'data': 'StatusColumn' }
            ]
        });
    };


})(DW);
