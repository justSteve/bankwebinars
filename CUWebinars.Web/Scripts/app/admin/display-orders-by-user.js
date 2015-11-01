/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $('#ordersTable').data('aff');

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
            "dom": '<lif<t>ip>',
            //"dom": 'T<"clear">lfrtip',
            'tableTools': {
                'sSwfPath': '/Content/DataTables/swf/copy_csv_xls_pdf.swf'
            },
            'ajax': {
                'url': '/admin/GetOrdersByUser',
                'data': {
                    'email': DOBU.userEmailDiv.text()
                },
                'type': 'POST'
            },
            'columns': [
                { 'data': 'OrderColumn' },

                { 'data': 'UserColumn' },
                { 'data': 'InstitutionColumn' },
                { 'data': 'BillingColumn' },
                { 'data': 'DiscountColumn' },
                {
                    'data': 'AffiliateColumn',
                    'visible': aff
                },
                { 'data': 'OrderDateColumn' },
                { 'data': 'StatusColumn' }
            ]
        });

    };

})(DOBU);
