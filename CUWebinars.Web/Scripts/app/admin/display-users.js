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
        DU.webinarIdDiv = $('#webinarIdDiv');
    };

    ns.wireUpHandlers = function () {

    ns.wireUpDataTable = function () {
        
        DU.usersTable.dataTable({
            "dom": '<ilf<t>ip>',
            "processing": true,
            "serverSide": true,
            "paging": true,
            //'dom': 'T<"clear">lfrtip',
            'ajax': {
                'url': '/admin/GetGridUserData',
                'data': {
                    'affiliateId': 19
                },
                'type': 'POST'
            },
            'columns': [
                { 'data': 'OrderColumn' },
                { 'data': 'UserColumn' },
                { 'data': 'InstitutionColumn' },
                { 'data': 'BillingColumn' },
                //{
                //    'data': 'AffiliateColumn',
                //    'visible': aff
                //},
                { 'data': 'OrderDateColumn' },
                { 'data': 'StatusColumn' }
            ]
        });
    };
    $('#getHtmlSpinner').remove();

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


})(DU);
