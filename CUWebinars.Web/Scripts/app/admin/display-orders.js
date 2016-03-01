/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

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
//function getDiscountHtml() {

//    var html = "";

//    $.ajax({
//        async: false,
//        url: "/admin/geteditdiscountdropdownhtml",
//        dataType: "json",
//        type: "POST",
//        success: function (data) {

//            html = data.html;
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            alert("geteditdiscountdropdownhtml" + textStatus);
//        }
//    });

//    return html;
//}

function getOrderStatusHtml() {
    // this seems a little slower than I'd like...
    var html = "";
    html = html + '<form class=\"form-compact compact-order-status-edit-form\" novalidate=\"novalidate\">';
    html = html + '    <input type=\"hidden\" name=\"Id\" />';
    html = html + '    <input type=\"hidden\" name=\"DisplayRowPriceViewModel.OrderStatus\" />';
    html = html + '    <div class=\"dropdown\">';
    html = html + '        <a class=\"dropdown-toggle btn btn-mini\" role=\"button\" href=\"#\" data-toggle=\"dropdown\">[ORDERSTATUS]&nbsp;<b class=\"caret\"></b></a>';
    html = html + '        <ul class=\"dropdown-menu\" role=\"menu\">';
    html = html + '                    <li role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\" onclick=\"updateOrderStatus(this, [ORDERID], \'Submitted\'); return false;\">Submitted</a></li>';
    html = html + '                    <li role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\" onclick=\"updateOrderStatus(this, [ORDERID], \'Billed\'); return false;\">Billed</a></li>';
    html = html + '                    <li role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\" onclick=\"updateOrderStatus(this, [ORDERID], \'Paid\'); return false;\">Paid</a></li>';
    html = html + '                    <li role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\" onclick=\"updateOrderStatus(this, [ORDERID], \'Canceled\'); return false;\">Canceled</a></li>';
    html = html + '                    <li role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\" onclick=\"updateOrderStatus(this, [ORDERID], \'AwaitingVerification\'); return false;\">AwaitingVerification</a></li>';
    html = html + '        </ul>';
    html = html + '    </div>';
    html = html + '</form>';
    //$.ajax({
    //    async: false,
    //    url: "/admin/geteditorderstatusdropdownhtml",
    //    dataType: "json",
    //    type: "POST",
    //    success: function (data) {

    //        html = data.html;
    //    },
    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
    //        alert(textStatus);
    //    }
    //});
    return html;
}

//function getRegTypeDropDownHtml() {
//    // this seems a little slower than I'd like...
//    var html = "";

//    $.ajax({
//        async: false,
//        url: "/admin/GetRegTypeDropdownHtml",
//        dataType: "json",
//        type: "POST",
//        success: function (data) {
//            html = data.html;
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            alert(textStatus);
//        }
//    });

//    return html;
//}

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.primeDomVariables = function () {
        DO.ordersTable = $('#ordersTable');
        DO.royaltiesTable = $('#royaltiesTable');
        DO.connInfoTable = $('#connInfoTable');
        DO.webinarIdDiv = $('#webinarIdDiv');
        //defined in parent page
        DO.affiliateId = affiliateId;

        DO.baseOrderStatusHtml = getOrderStatusHtml();  // 
        //DO.baseDiscountHtml = getDiscountHtml();  // 

        DO.orderStatusFilters = [];


    };

    ns.getBillingCellHtml = function (discount, regTypeLabel, total) {
        //debugger;

        var showDiscount = discount;

        var billingHtml = regTypeLabel
                            + (showDiscount + "")
                            + "<br />Total: $" + (total + "").replace(".00", "");

        return billingHtml;

    };
    ns.wireUpHandlers = function () {
        //var mouseX;
        //var mouseY;
        //$(document).mousemove(function (e) {
        //    mouseX = e.pageX;
        //    mouseY = e.pageY;
        //}); http://stackoverflow.com/questions/4666367/how-do-i-position-a-div-relative-to-the-mouse-pointer-using-jquery

        $('.dataTable').on("mouseover", ".edit-user-name-email", function () {
            $('#hdrCaption').text("Edit Contact");
            //            $('#hdrCaption').css({ 'top': mouseY, 'left': mouseX }).fadeIn('slow');
        });
        $('.dataTable').on("mouseout", ".edit-user-name-email", function () {
            $('#hdrCaption').text("");
        });

        $('.dataTable').on("mouseover", ".edit-institution", function () {
            $('#hdrCaption').text("Institution Details");
        });
        $('.dataTable').on("mouseout", ".edit-institution", function () {
            $('#hdrCaption').text("");
        });

        $('.dataTable').on("mouseover", ".edit-billing", function () {
            $('#hdrCaption').text("Edit Order Details");
        });
        $('.dataTable').on("mouseout", ".edit-billing", function () {
            $('#hdrCaption').text("");
        });

        $('.dataTable').on("mouseover", ".edit-resends", function () {
            $('#hdrCaption').text("Links for User's Media");
        });
        $('.dataTable').on("mouseout", ".edit-resends", function () {
            $('#hdrCaption').text("");
        });

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

        $('.dataTable').on("click", ".aff-revenue-summary", function () {

            var self = this;

            var payload = { idWebinar: this.getAttribute('data-w'), aff: this.getAttribute('data-a') };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/BuildAffiliateInvoice',
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                }
            }).done(function (data) {
                alert("hit");
                if (result.Result === 'Success') {
                    $('#InputFormFields').append(successScreenMessage);
                } else if (result.Result === 'Fail') {
                    $('#InputFormFields').append(noOrderScreenMessage);
                }
                $('#spinnerLabel').remove();
            }).fail(function (result) {
                ns.formatAffRevTable(result);
            }).always(function () {
                $('#loadingSpinner').remove();
            });
        });
        ns.formatAffRevTable = function (data) {
            alert(data.InvoiceId);
        };

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

        // setup radio button group filters
        $("#filter-buttons").on("click", "button", function (e) {

            e.stopImmediatePropagation(); // the bootstrap buttons plugin doesn't toggle the clicked on button soon enough so do it ourselves https://github.com/twbs/bootstrap/issues/2380
            $(this).button('toggle'); // flip the state of the clicked on button
            $(this).blur(); // the focused look is very similar to the active look, which is confusing when deselecting buttons, blur causes the button to look unselected, as it should

            var $button = $(this);

            if ($button.hasClass("active")) {
                // they are "selecting" a button, we want to "uncheck" Active or Inactive IF the user manually clicked a button in the opposite group
                if ($button.hasClass("os-active") && // this button is in the "active" category
                    $(".os-inactive-group-control").hasClass("active")) { // the inactive category is also selected

                    $("#order-status-filter-container button.active.os-inactive").removeClass("active"); // virtually "uncheck" the inactive buttons
                    $(".os-inactive-group-control").removeClass("active"); // virtually "uncheck" the inactive category

                } else if ($button.hasClass("os-inactive") && // this button is in the "inactive" category
                    $(".os-active-group-control").hasClass("active")) { // the active category is also selected

                    $("#order-status-filter-container button.active.os-active").removeClass("active"); // virtually "uncheck" the active buttons
                    $(".os-active-group-control").removeClass("active"); // virtually "uncheck" the active category

                }
            } else {
                // they are deselecting a button, uncheck Active or Inactive category when there aren't any others left selected
                if ($button.hasClass("os-active") && // this button is in the "active" category
                    $("button.os-active.active").length == 0) { // there are no other "active" buttons selected

                    $(".os-active-group-control").removeClass("active"); // virtually "uncheck" the active category

                } else if ($button.hasClass("os-inactive") && // this button is in the "inactive" category
                    $("button.os-inactive.active").length == 0) { // there are no other "inactive" buttons selected

                    $(".os-inactive-group-control").removeClass("active"); // virtually "uncheck" the inactive category
                }
            }

            // collect all "active" filters
            var selectedButtons = [];
            $("#order-status-filter-container button.active").each(function () {
                // inspect the actual value of the clicked buttons
                switch (this.value) {
                    case "Active":
                        break; // do nothing, don't want the word Active included, all statuses that are categorized as active have an actual button on the screen

                    case "Inactive":
                        // the following statuses don't have actual buttons on the screen but we still want to include them when Inactive is clicked
                        //  also, we don't want the word Inactive included
                        selectedButtons.push("Error");
                        selectedButtons.push("Abandoned");
                        selectedButtons.push("Canceled");
                        selectedButtons.push("Unknown");
                        break;

                    default:
                        selectedButtons.push(this.value); // not the buttons Active or Inactive, include it
                        break;
                }
            });

            DO.orderStatusFilters = selectedButtons;

            // trigger search with existing textbox value, additional params will be constructed on that event handler
            var currentSearchVal = $("input[type='search']", "#ordersTable_filter").val();

            DO.ordersTable.DataTable().search(currentSearchVal).draw();

        });

        // manually deal with the UI for Active/Inactive button clicks
        $("#order-status-filter-container").on("click", "button", function (e) {

            var selector = "#order-status-filter-container button.os-" + $(this).val().toLowerCase(); // see the CUWebinars.Web\Views\Admin\Partials\_ShowOrders.cshtml file

            // check whether they are selecting or deselecting the Active/Inactive button
            // NOTE: it looks "backwards" because the active class hasn't been set yet for this click
            if ($(this).hasClass("active")) // looks backwards because the active class hasn't been set yet for this click
            {
                $(selector).removeClass("active"); // "deselect" the buttons
            } else {
                $(selector).addClass("active"); // "select" the buttons
            }

        });
    };


    ns.wireUpDataTable = function () {

        DO.ordersTable.dataTable({
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": '/admin/OrdersDataHandler',
                "contentType": 'application/json; charset=utf-8',
                'data': function (data) {

                    // additional custom filters
                    var showAllEvents = ($("#filter-buttons #showAllOrders button.active").val() == "showAllEvents");
                    var includeOrderStatuses = DO.orderStatusFilters;

                    data.webinarId = parseInt(DO.webinarIdDiv.text());
                    data.affiliateId = DO.affiliateId;
                    data.showAllEvents = showAllEvents;
                    data.selectedOrderStatuses = includeOrderStatuses;
                    return data = JSON.stringify(data);
                }
            },

            "dom": '<ilf<t>ip>',
            "pageLength": 10,
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
                { 'data': 'RegistrationTypeString', 'class': 'details-control edit-billing' },
                {
                    'data': 'Affiliate_ttsDomain',
                    'visible': showAffiliateColumn,
                    'class': 'details-control '
                },
                { 'data': 'OrderDate', 'class': 'details-control edit-resends' },
                { 'data': 'OrderStatusString', 'class': 'edit-status' }
            ],
            "order": [0, "asc"]

            , // complex columns can be specified / created with mRender
            "aoColumnDefs": [
            {
                // [0] idOrder column is hidden

                "aTargets": [1], // User column  -- triggers EditUser_Compact.cshtml
                "mData": "",
                "mRender": function (data, type, full) {
                    return full.LastName + ", " + full.FirstName + "<br>" + full.BillingEmail;
                    //return "<a href='/account/ordersbyuser/" + full.idUser + "' />" + full.LastName + ", " + full.FirstName + "</a><br>" + full.BillingEmail;
                }
                // link on user name should implement 'orders by user' current contorl: byUserWrapper
            },
            {
                "aTargets": [2], // institution column  -- triggers EditInstitution.chtml
                "mData": "",
                "mRender": function (data, type, full) {
                    return full.Institution + "</br>";
                    //return "<a href='/account/editinstitution/" + full.idUser + "' target='_new' />" + full.Institution + "</a>";
                }
            },
           {
               "aTargets": [3], // Billing column  -- triggers EditOrder_Compact.cshtml and EditRegType_DropDown.cshtml
               "mData": "RegistrationType",
               "mRender": function (data, type, full) {

                   var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;

                   var flatOff = 0;
                   var percentOff = 0;

                   if (full.Discount != null) {
                       flatOff = full.Discount.FlatOff;
                       percentOff = full.Discount.PercentOff;

                   };
                   var showDiscount = "";
                   if (flatOff > 0) {
                       showDiscount = "<br><span class=\"DisplayDiscount\">Discounted by: $" + flatOff + "</span>";
                   }

                   if (percentOff > 0) {
                       showDiscount = "<br><span class=\"DisplayDiscount\">Discounted by: " + percentOff + "%</span>";
                   }


                   var newBillingHtml = ns.getBillingCellHtml(showDiscount,
                                                           full.RegistrationType.OptionLabelShort,
                                                           full.Total);

                   return newBillingHtml;

               }
           },
           // [4] Affiliate Column
            {
                "aTargets": [4], //
                "mData": "Affiliate_ttsDomain",
                "mRender": function (data, type, full) {

                    var royaltyHtml = full.Royalty;
                    return "<div class=\"aff-revenue-summary\" data-w=" + parseInt(DO.webinarIdDiv.text()) + " data-a='" + full.Affiliate_ttsDomain + "' style=\"text-align: center\">" + full.Affiliate_ttsDomain + "</br>" + royaltyHtml + "</div>";
                }
            },

           // [5] Resends Column
            {

                "aTargets": [5], // OrderDate column
                "mData": "",
                "mRender": function (data, type, full) {

                    var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;
                    var statusHtml = full.OrderDateString;
                    var resendMsg = "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendOrderConfirmationButton btn btn-mini\">Confirmation</button>";
                    if (full.Webinar_IsActive) {
                        resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendConnectionInfoButton btn btn-mini\">Connection Info</button>";
                    }
                    //if (full.Webinar_IsRecorded) {
                    //    resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendPostEventMaterialButton btn btn-mini\">PostEvent Material</button>";
                    //}
                    return "<div style=\"text-align: center\">" + statusHtml + "</br>" + resendMsg + "</div>";
                }
            },
            {

                "aTargets": [6], // Status column
                "mData": "",
                "mRender": function (data, type, full) {

                    // setup a Bootstrap dropdown (http://getbootstrap.com/2.3.2/javascript.html#dropdowns) with the current order status "selected"

                    var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;

                    // the dropdown's HTML gets built one time on the server via ajax via a partial view and RenderViewToString
                    //  magic strings [ORDERSTATUS] and [ORDERID] are hand-/hard-coded in the partial View
                    var dd_html = DO.baseOrderStatusHtml.replace(/\[ORDERSTATUS\]/gi, full.OrderStatusString).replace(/\[ORDERID\]/gi, orderToEdit);

                    return orderToEdit + dd_html;
                }
            }]
        });
    };


})(DO);
