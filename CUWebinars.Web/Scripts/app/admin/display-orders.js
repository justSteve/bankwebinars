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

    ns.wireUpHandlers = function () {

        $('.dataTable').on("click", ".ResendConnectionInfoButton", function () {
            

            var self = this;

            var orderId =  this.getAttribute('data-orderId');

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
    $('#getHtmlSpinner').remove();


})(DO);
