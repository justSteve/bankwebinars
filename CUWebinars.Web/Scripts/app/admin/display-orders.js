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


function getOrderStatusHtml() {
    // this seems a little slower than I'd like...
    var html = "";

    $.ajax({
        async: false,
        url: "/admin/geteditorderstatusdropdownhtml",
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        }
    });

    return html;
}

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.primeDomVariables = function () {
        DO.ordersTable = $('#ordersTable');
        DO.connInfoTable = $('#connInfoTable');
        DO.webinarIdDiv = $('#webinarIdDiv');
        DO.baseOrderStatusHtml = getOrderStatusHtml();  // this seems a little slower than I'd like...
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
                alert("Operation Failed. Call Steve!");
            }).always(function () {
                $('#loadingSpinner').remove();
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

                //Rollbar.info({ 'oen-#2': { 'result': result } });

            }).fail(function () {

            }).always(function () {
                //$('#loadingSpinner').remove();
            });
        });


        $('.dataTable').on("click", ".ResendPostEventMaterialButton", function () {


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
                url: '/Admin/ResendPostEventMaterial',
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

                //Rollbar.info({ 'oen-#2': { 'result': result } });

            }).fail(function () {

            }).always(function () {
                //$('#loadingSpinner').remove();
            });
        });

        AttachDataTableEditEvents(); // edit-forms-in-child-rows.js

    };

    ns.wireUpDataTable = function () {

        DO.ordersTable.dataTable({
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": '/admin/OrdersDataHandler',
                "contentType": 'application/json; charset=utf-8',
                'data': function (data) {
                    data.webinarId = parseInt(DO.webinarIdDiv.text());
                    data.affiliateId = DO.affiliateId;
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
                { 'data': 'idOrder' },
                { 'data': 'LastName', 'class': 'details-control edit-user-name-email' },
                { 'data': 'Institution', 'class': 'details-control' },
                { 'data': null, 'class': 'details-control' },
                { 'data': 'Discount', 'class': 'details-control' },

                {
                    'data': 'Affiliate_ttsDomain',
                    'visible': aff,
                    'class': 'details-control'
                },
                { 'data': 'OrderDateString', 'class': 'details-control edit-date' },
                { 'data':'OrderStatusString' }

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
                    return "<a href='/account/edituser/" + full.idUser + "' target='_new' />" + full.LastName + ", " + full.FirstName + "</a><br>" + full.BillingEmail;
                }
            },
            {
                "aTargets": [3], // Billing column
                "mData": "",
                "mRender": function (data, type, full) {
                    var showDiscount = "";
                    var discountHTML = "<br><span class=\"DisplayDiscount\">Discounted by: {0}</span>";
                    if (full.Discount != null) {
                        if (full.Discount.FlatOff > 0) {
                            showDiscount = discountHTML.replace("{0}", "$" + (full.Discount.FlatOff + "").replace(".00", ""));
                        }
                        else if (full.Discount.PercentOff > 0) {
                            showDiscount = discountHTML.replace("{0}", full.Discount.PercentOff + "%");
                        }
                    }

                    var billingHtml = full.RegistrationType.OptionLabel
                                            .replace(" and Hardcopy Handouts", "")
                                            .replace("Plus Five", "")
                                            + showDiscount
                                            + "<br />Total: $" + (full.Total + "").replace(".00", "")
                    ;

                    return billingHtml;
                }
            },
            {
                "aTargets": [4], // Discount column
                "mData": "",
                "mRender": function (data, type, full) {
                    var discountCode = "";
                    if (full.Discount != null) {

                        discountCode = full.Discount.DiscountCode;
                    }


                    var showDiscount = "";
                    var discountHtml = "<span class=\"EditDiscount\">" + discountCode + "</span>";

                    return discountHtml;
                }
            },
            {

                "aTargets": [7], // Status column
                "mData": "",
                "mRender": function (data, type, full) {
                    var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;

                    // setup a Bootstrap dropdown (http://getbootstrap.com/2.3.2/javascript.html#dropdowns) with the current 
                    //  order status selected, fire custom ajax when it changes
               
                    // we need to build this one time on the server via ajax...
                    var dd_html = DO.baseOrderStatusHtml.replace("[ORDERSTATUS]", full.OrderStatusString).replace("[ORDERID]", orderToEdit);
                    // find and set the dropdown to our status
                    

                    //// added a hook into Edit Order in the dropdown...
                    //var statusHtml = "<a href='/Admin/manageOrder/" + orderToEdit + "' target='_new' />" + full.OrderStatusString + "</a><br/>";


                    var resendMsg = "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendOrderConfirmationButton btn btn-mini\">Send Confirmation</button>";
                    if (full.Webinar_IsActive) {
                        resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendConnectionInfoButton btn btn-mini\">Connection Info</button>";
                    }

                    //if (full.Webinar_IsRecorded) {
                    //    resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendPostEventMaterialButton btn btn-mini\">PostEvent Material</button>";
                    //}

                    return dd_html + resendMsg;
                }
            }]
        });
    };


})(DO);
