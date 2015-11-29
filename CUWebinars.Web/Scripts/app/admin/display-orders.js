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


function getResendInfoHtml() {
    // this seems a little slower than I'd like...
    var html = "";

    $.ajax({
        async: false,
        url: "/admin/getresendinfohtml",
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
                // class names function as trigger - createChildRow
                { 'data': 'idOrder', 'visible': false },
                { 'data': 'LastName', 'class': 'details-control edit-user-name-email' },
                { 'data': 'Institution', 'class': 'details-control edit-institution' },
                { 'data': null, 'class': 'details-control edit-billing' },
                {
                    'data': 'Affiliate_ttsDomain',
                    'visible': aff,
                    'class': 'details-control'
                },
                { 'data': 'OrderDateString', 'class': 'details-control edit-resends' },
                { 'data': 'OrderStatusString' }

            ],
            "order": [0, "asc"]

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [
            {
                // [0] idOrder column is hidden

                "aTargets": [1], // User column  -- triggers EditUser_Compact.cshtml
                "mData": "",
                "mRender": function (data, type, full) {
                    return full.LastName + ", " + full.FirstName + " [add OrdersByUser]<br>" + full.BillingEmail;
                    //return "<a href='/account/ordersbyuser/" + full.idUser + "' />" + full.LastName + ", " + full.FirstName + "</a><br>" + full.BillingEmail;
                }
                // link on user name should implement 'orders by user' current contorl: byUserWrapper
            },
            {
                "aTargets": [2], // institution column  -- triggers EditInstitution.chtml
                "mData": "",
                "mRender": function (data, type, full) {
                    return "<a href='/account/editinstitution/" + full.idUser + "' target='_new' />" + full.Institution + "</a>";
                }
            },
           {
               "aTargets": [3], // Billing column  -- triggers EditOrder_Compact.cshtml
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
           // [4] Affiliate Column
           // [5] Resends Column
            {

                "aTargets": [6], // Status column
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

                    return dd_html;
                }
            }]
        });
    };


})(DO);
