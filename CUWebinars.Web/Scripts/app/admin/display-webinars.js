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

    ns.primeDomVariables = function () {
        DW.webinarsTable = $('#webinarsTable');
        DW.connInfoTable = $('#connInfoTable');
        DW.searchTermDiv = $('#searchTermDiv');
    };

    ns.wireUpHandlers = function () {



    };

    ns.wireUpWebinarsGrid = function () {
        DW.webinarsTable.dataTable({
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": '/webinar/WebinarDataHandler',
                "contentType": 'application/json; charset=utf-8',
                'data': function (data) {
                    data.searchTerm = DW.searchTermDiv.text();
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
            'columns': [
                { 'data': 'idWebinar' },
                { 'data': 'WebinarDateString' },
                { 'data': 'Body' },
                //null,
                //{
                //    'data': 'Affiliate_ttsDomain',
                //    'visible': aff
                //},
                { 'data': 'Status' },
                //{ 'data': null, 'orderable': false }
                { 'data': 'RelatedTopicsString' }
            ],
            "order": [0, "asc"]

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [{
                "aTargets": [0], //[id] column
                "mData": "",
                "mRender": function (data, type, full) {
                    var orderToEdit = (full.idWebinar != 0) ? full.idWebinar : data;
                    return orderToEdit + ", ";
                },
            },
            {
                "aTargets": [2], // Date column
                "mData": "",
                "mRender": function (data, type, full) {
                    var showDiscount = "";
                    var discountHTML = "<br><span class=\"DisplayDiscount\">Discounted by: {0}</span>";
                    

                    var billingHtml = ""
                    ;

                    return billingHtml;
                }
            },
                {
                    "aTargets": [3], // Status column
                    "mData": "",
                    "mRender": function (data, type, full) {
                        var statusHtml = "<a href='/Admin/manageOrder/" + full.status + "' target='_new' />this</a><br/>";
                        
                        return statusHtml;
                    }
                }]
        });
    };


})(DW);
