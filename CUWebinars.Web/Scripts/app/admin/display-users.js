/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $('#usersTable').data('aff');

if (DISPLAYUSERS === null || typeof DISPLAYUSERS === 'undefined')
    var DISPLAYUSERS = {}; // create namespace - object to holds all references and methods.

var DU = DISPLAYUSERS; // alias for code brevity

// document.ready function
$(function () {

    DU.primeDomVariables();
    DU.wireUpHandlers();
    DU.wireUpDataTable();

});




// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.primeDomVariables = function () {
        DU.usersTable = $('#usersTable');
        //DU.connInfoTable = $('#connInfoTable');
        DU.webinarIdDiv = $('#webinarIdDiv');
    };
    ns.wireUpHandlers = function () {


    };

    ns.wireUpDataTable = function () {

        DU.usersTable.dataTable({
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": '/admin/UsersDataHandler',
                "contentType": 'application/json; charset=utf-8',
                'data': function (data) {
                    data.affiliateId = DU.affiliateId;
                    return data = JSON.stringify(data);
                }
            },
            // "dom": 'frtiS',
            "dom": '<ilf<t>ip>',
            "pageLength": 10,
            "scrollY": 500,
            "scrollX": true,
            "scrollCollapse": true,
            "scroller": {
                loadingIndicator: false
            },
            "processing": true,
            "paging": true,
            "deferRender": true,
            'columns': [
                { 'data': 'idUser' },
                { 'data': 'email' },
                { 'data': 'Institution' },

            ],
            "order": [0, "asc"]

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [
            {

                "aTargets": [0], // Status column
                "mData": "",
                "mRender": function (data, type, full) {

                    var startOrder = "<a data-iduser=" + full.idUser + " onclick='OCA.AddOrder(this," + full.idUser + ")' class='btn btn-mini' href='#'/>Place Order</a><br/>";

                    return startOrder;
                }
            }
            ]
        });
    };


})(DU);
