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
    DO.wireUpConnInfoDataTable();

});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.primeDomVariables = function() {
        DO.ordersTable = $('#ordersTable');
        DO.connInfoTable = $('#connInfoTable');
        DO.webinarIdDiv = $('#webinarIdDiv');
    };

    ns.wireUpHandlers = function () {

        $('.dataTable').on("click", ".ResendOrderConfirmationButton", function () {
            
            var self = this;

            var orderId =  this.getAttribute('data-orderId');

            var payload = { orderId: orderId };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/ResendOrderConfirmation',
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function() {
                    $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                }
            }).done(function(result) {

                if (result.Result === 'Success') {
                    $('#InputFormFields').append(successScreenMessage);
                } else if (result.Result === 'Fail') {
                    $('#InputFormFields').append(noOrderScreenMessage);
                }
                $('#spinnerLabel').remove();
            }).fail(function() {
                alert("Operation Failed. Call Steve!")
            }).always(function() {
                //$('#loadingSpinner').remove();
            });
        });




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
                "serverSide": true,
                "ajax": {
                    "type": "POST",
                    "url": '/admin/OrderDataHandler',
                    "contentType": 'application/json; charset=utf-8',
                    'data': function (data) {
                        data.webinarId = parseInt(DO.webinarIdDiv.text());
                        data.affiliateId = DO.affiliateId;
                        return data = JSON.stringify(data);
                    }
                },
                // "dom": 'frtiS',
                "dom": '<ilf<t>ip>',
                "pageLength" : 10,
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
                    { 'data': 'idOrder' },
                    { 'data': 'idUser' },
                    { 'data': 'Institution' },
                    null,
                    {
                        'data': 'Affiliate_ttsDomain',
                        'visible': aff
                    },
                    { 'data': 'OrderDateString' },
                    { 'data': null, 'orderable': false }
                ],
                "order": [0, "asc"]            

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [{
                "aTargets": [0], // Order [id] column
                "mData": "",
                "mRender": function (data, type, full) {
                    var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : data;
                    return orderToEdit + ", " + full.TtsJoinUrl;
                },
            },
            {
                "aTargets": [1], // User column
                "mData": "",
                "mRender": function (data, type, full) {
                    return "<a href='/account/edituser/" + data + "' target='_new' />" + full.LastName + ", " + full.FirstName + "</a><br>" + full.BillingEmail;
                }
            },
            {
                "aTargets": [3], // Billing column
                "mData": "",
                "mRender": function (data, type, full) {
                    var showDiscount = "";
                    var discountHTML = "<br><span class=\"DisplayDiscount\">Discounted by: {0}</span>";
                    if (full.Discount != null)
                    {
                        if (full.Discount.FlatOff > 0)
                        {
                            showDiscount = discountHTML.replace("{0}", "$" + (full.Discount.FlatOff + "").replace(".00", ""));
                        }
                        else if (full.Discount.PercentOff > 0) 
                        {
                            showDiscount = discountHTML.replace("{0}", full.Discount.PercentOff + "%");
                        }
                    }

                    var billingHtml = full.RegistrationType.OptionLabel
                                            .replace(" and Hardcopy Handouts", "")
                                            .replace("Plus Five", "Only")
                                            + showDiscount
                                            + "<br />Total: $" + (full.Total + "").replace(".00", "")
                                            ;

                    return billingHtml;
                }
            },
            {
                "aTargets": [6], // Status column
                "mData": "",
                "mRender": function (data, type, full) {
                    var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;
                    var statusHtml = "<a href='/Admin/manageOrder/" + orderToEdit + "' target='_new' />" + full.OrderStatusString + "</a><br/>";
                    var resendMsg = "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendOrderConfirmationButton btn btn-mini\">Send Confirmation</button>";
                    if (full.Webinar_IsActive) {
                        resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendConnectionInfoButton btn btn-mini\">Connection Info</button>";
                    }

                    return statusHtml + resendMsg;
                }
            }]
        });
    };

    ns.wireUpConnInfoDataTable = function() {
        
        DO.connInfoTable.dataTable({
            "dom": '<ilf<t>ip>',
            //'dom': 'T<"clear">lfrtip',
            buttons: [
              'copy',
              'excel',
              'csv',
              'pdf'
            ],
        
            'ajax': {
                'url': '/admin/GetGridDataForConnInfo',
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
