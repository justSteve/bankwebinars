/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $('#ordersTable').data('aff');

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
            "dom": '<ilf<t>ip>',
            //'dom': 'T<"clear">lfrtip',
            'tableTools': {
                'sSwfPath': '/Content/DataTables/swf/copy_csv_xls_pdf.swf'
            },
            'ajax': {
                'url': '/admin/GetGridData',
                'data': {
                    'webinarId': parseInt(DO.webinarIdDiv.text()),
                    'affiliateId': DO.affiliateId
                },
                'type': 'POST'
            },
            'columns': [
                { 'data': 'OrderColumn' },
                
                { 'data': 'UserColumn' },
                { 'data': 'InstitutionColumn' },
                { 'data': 'BillingColumn' },
                {
                    'data': 'AffiliateColumn',
                    'visible': aff
                },
                { 'data': 'OrderDateColumn' },
                { 'data': 'StatusColumn' }
            ]
        });
    };

})(DO);
