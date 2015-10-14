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

        $('.dataTable').on("click", ".ResendOrderConfirmationButton", function () {

            var self = this;

            var orderId = this.getAttribute('data-orderId');

            var payload = { orderId: orderId };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/ResendOrderConfirmation',
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                }
            }).done(function (result) {

                if (result.Result === 'Success') {
                    $('#InputFormFields').append(successScreenMessage);
                } else if (result.Result === 'Fail') {
                    $('#InputFormFields').append(noOrderScreenMessage);
                }
                $('#spinnerLabel').remove();
            }).fail(function () {
                alert("Operation Failed. Call Steve!")
            }).always(function () {
                //$('#loadingSpinner').remove();
            });
        });




        $('.dataTable').on("click", ".ResendConnectionInfoButton", function () {


            var self = this;

            var orderId = this.getAttribute('data-orderId');

            //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            //var logStartOperation = toastLogger.getLogFn('ResendConnectionInfo');
            //logStartOperation("Re-sending ConnectionInfo", null, true);

            var payload = { orderId: orderId };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/ResendConnectionInfo',
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                }
            }).done(function (result) {

                if (result.Result === 'Success') {
                    $('#InputFormFields').append(successScreenMessage);
                } else if (result.Result === 'Fail') {
                    $('#InputFormFields').append(noOrderScreenMessage);
                }
                $('#spinnerLabel').remove();

                Rollbar.info({ 'oen-#2': { 'result': result } });

            }).fail(function () {

            }).always(function () {
                //$('#loadingSpinner').remove();
            });
        });


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
