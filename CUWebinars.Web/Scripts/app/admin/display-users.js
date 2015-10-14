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


    $('#datatab tfoot th').each(function () {
        $(this).html('<input type="text" />');
    });

    //DU.usersTable.dataTable.columns().every(function () {
    //    var that = this;

    //    $('input', this.footer()).on('keyup change', function () {
    //        that
    //            .search(this.value)
    //            .draw();
    //    });
    //});


});

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.primeDomVariables = function () {
        DU.usersTable = $('#usersTable');
        DU.webinarIdDiv = $('#webinarIdDiv');
    };

    ns.wireUpHandlers = function () {

        ns.wireUpDataTable = function () {

            DU.usersTable.dataTable({
                "dom": '<ilf<t>ip>',


                "serverSide": true,
                "ajax": {
                    "type": "POST",
                    'url': '/admin/GetGridUserData',
                    "contentType": 'application/json; charset=utf-8',
                    'data': function (data) { return data = JSON.stringify(data); }


                },
                "scrollY": 500,
                "scrollX": true,
                "scrollCollapse": true,
                "scroller": {
                    loadingIndicator: false
                },
                "processing": true,
                "paging": true,
                "deferRender": true,
                "columns": [
               { "data": "Name" },
               { "data": "City" },
               { "data": "Postal" },
               { "data": "Email" },
               { "data": "Company" },
               { "data": "Account" },
               {
                   "className": 'details-control',
                   "orderable": false,
                   "data": null,
                   "defaultContent": ''
               }
                ],
                "order": [0, "asc"]

            });
        };


        $('#getHtmlSpinner').remove();

    };


})(DU);
