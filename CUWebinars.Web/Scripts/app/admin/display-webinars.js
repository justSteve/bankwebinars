/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

var aff = $('#webinarsTable').data('aff');

if (DISPLAYWEBINARS === null || typeof DISPLAYWEBINARS === 'undefined')
    var DISPLAYWEBINARS = {}; // create namespace - object to holds all references and methods.

var DW = DISPLAYWEBINARS; // alias for code brevity

// document.ready function
$(function () {

    DW.primeDomVariables();
    DW.wireUpHandlers();
    DW.wireUpWebinarsGrid();
});

// self-invoking function for creating methods using Module pattern.
(function (ns) {
    $('#titleOnly').change(function () {
        location.reload();
    });

    ns.primeDomVariables = function () {
        DW.webinarsTable = $('#webinarsTable');
        DW.connInfoTable = $('#connInfoTable');
        DW.searchTermDiv = $('#searchTermDiv');
        DW.titleOnly = "";
        if ($("#titleOnly").prop('checked')) {
            DW.titleOnly = "title:";
        }

    };

    ns.wireUpHandlers = function () {

        AttachDataTableWebinarEvents();

    };
    var shouldShow = false;
    ns.wireUpWebinarsGrid = function () {


        DW.webinarsTable.dataTable({
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": '/webinar/WebinarDataHandler',
                "contentType": 'application/json; charset=utf-8',
                'data': function (data) {
                    data.titleOnly = $("#titleOnly").prop('checked');
                    data.searchTerm = DW.titleOnly + DW.searchTermDiv.text();
                    data.affiliateId = DW.affiliateId;
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
            "oLanguage": {
                "sSearch": "Filter: "
            },
            'columns': [

                { 'data': 'Date', 'class': 'details-control' },
                { 'data': 'Status', 'class': 'details-control edit' },
                { 'data': 'Title', 'class': 'details-control wTitle' },
                { 'data': 'PresenterName', 'class': 'details-control presenter' },
                { 'data': 'RelatedTopicsString', 'class': 'details-control topicsTitles' },
                { 'data': 'Orders',  'visible': false, 'class': 'details-control orders' }
            ],
            "order": [0, "desc"]

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [
            {

                "aTargets": [0], // Date column
                "mData": "",
                "createdCell": function (td, cellData, rowData, row, col) {
                    if (cellData != null) {
                        $(td).tooltip();
                    }
                },
                "mRender": function (data, type, full) {
                    console.log(full);
                    var statusHtml = full.WebinarDateString + "<br><a target='EventDetails' id='goToEventButton' type='button' class='btn btn-mini' href='/webinar/details/"+full.idWebinar+"' />Go to event</a>";

                    return statusHtml;
                }
            },
            {
                "aTargets": [1], // Status column
                "mData": "",
                "visible": shouldShow,
                "mRender": function (data, type, full) {

                    var statusHtml = full.Title;

                    return statusHtml;
                }
            },
            {
                "aTargets": [2], // Titlecolumn
                "mData": "Title",
                "mRender": function (data, type, full) {

                    var statusHtml = full.Title + " <a class='small' href='#'>[more...]</a>";

                    return statusHtml;
                }
            },
            {
                "aTargets": [3], // Titlecolumn
                "mData": "PresenterPresenterName",
                "mRender": function (data, type, full) {

                    var statusHtml = full.PresenterName + " <a class='small'href='#'>[bio...]</a>";

                    return statusHtml;
                }
            },
            {
                "aTargets": [5], // Status column
                "mData": "",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var numOrders = 3;
                    var statusHtml = "<button data-webinarId=\"" + full.idWebinar + "\" class=\"ShowOrdersByWebinar btn btn-mini\">" + numOrders + "</button>";
                    return statusHtml;
                }
            }
            ]
        });
    };


})(DW);
