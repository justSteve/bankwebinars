/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
var OrderRegistration;
(function (OrderRegistration) {
    //  todo: duplicate in Registration module
    var Constants = (function () {
        function Constants() {
        }
        Constants.BillingAddressFields = '#RegisterFields_BillingAddress';
        Constants.City = '_City';
        Constants.Country = '_Country';
        Constants.ConfirmDeleteShippingAddressdialog = '#ConfirmDeleteShippingAddressdialog';
        Constants.FormPostContentType = 'application/x-www-form-urlencoded';
        Constants.JsonContentType = 'application/json; charset=utf-8';
        Constants.JsonDataType = 'json';
        Constants.HtmlDataType = 'html';
        Constants.Phone = '_Phone';
        Constants.ShippingAddressFields = '#RegisterFields_ShippingAddress';
        Constants.ShippingAddressContainer = '#ShippingAddressContainer';
        Constants.State = '_State';
        Constants.AddShippingAddressLink = '#AddShippingAddressLink';
        Constants.HideAddShippingAddressLink = '#HideAddShippingAddressLink';
        Constants.StreetAddress = '_StreetAddress';
        Constants.StreetAddress2 = '_StreetAddress2';
        Constants.TypeofAddressBilling = 'Billing';
        Constants.TypeofAddressShipping = 'Shipping';
        Constants.Zip = '_Zip';
        return Constants;
    })();
    OrderRegistration.Constants = Constants;
    ;

    var StateManager = (function () {
        function StateManager() {
        }
        StateManager.prototype.getCancelOrderForm = function () {
            return this.cancelOrderForm;
        };

        StateManager.prototype.getConfirmOrderForm = function () {
            return this.confirmOrderForm;
        };

        StateManager.prototype.getIsUserLogged = function () {
            return this.isUserLogged;
        };

        StateManager.prototype.getOrderId = function () {
            return this.orderId;
        };

        StateManager.prototype.getOrderRowId = function () {
            return this.orderRowID;
        };

        StateManager.prototype.getWebinarId = function () {
            return this.idWebinar;
        };

        StateManager.prototype.setCheckoutInProcess = function (val) {
            this.checkoutInProcess = val;
        };

        StateManager.prototype.setCancelOrderForm = function (form) {
            this.cancelOrderForm = form;
        };

        StateManager.prototype.setConfirmOrderForm = function (form) {
            this.confirmOrderForm = form;
        };

        StateManager.prototype.setIsUserLogged = function (val) {
            this.isUserLogged = val;
        };

        StateManager.prototype.setOrderId = function (num) {
            this.orderId = num;
        };

        StateManager.prototype.setOrderRowId = function (num) {
            this.orderRowID = num;
        };

        StateManager.prototype.setWebinarId = function (id) {
            this.idWebinar = id;
        };

        StateManager.prototype.BuildPreRegPrice = function (oEvent, orderRowId) {
            //permits a 'preReg' pricing scheme to handle
            //computation of discounts and addl locations prior
            //to stepping to confirmation.
            var $form = $("#BuildPrice");

            oEvent.preventDefault();
            $("#ProgressDialogBS").modal('show');
            $.ajax({
                url: '/cart/CheckoutDisplayRowPrice/' + orderRowId,
                type: "POST",
                //data: $form.serialize(),
                success: function (data) {
                    $("#regTypeID_" + data.orderRowID).prop('checked', true);
                },
                error: function () {
                },
                complete: function () {
                    $("#ProgressDialogBS").modal('hide');
                }
            });
        };

        StateManager.prototype.CheckIfAddLocShouldHide = function (optionID) {
            //don't show AdditionalEmails when RegType
            // can't support them. (ex: recorded only)
            $.ajax({
                url: "/cart/CheckIfAddLocShouldHide?optionID=" + optionID,
                type: "GET",
                cache: false,
                dataType: Constants.JsonDataType,
                beforeSend: function () {
                    //console.log('beforeSend CheckIfAddLocShouldHide');
                    // no loading image needed
                }
            }).done(function (data) {
                //console.log('done CheckIfAddLocShouldHide');
                if (data.shouldShow === 'Yes') {
                    //console.log('show CheckIfAddLocShouldHide');
                    $("#displayAddLoc").show('slow');
                } else if (data.shouldShow === 'No') {
                    //console.log('hide  CheckIfAddLocShouldHide');
                    $("#displayAddLoc").hide(1000);
                }
            }).fail(function (data) {
                //console.log('CheckIfAddLocShouldHide failed!!! ');
            });
        };

        StateManager.prototype.PlaceOrder = function () {
            $("#Step2_BillMeFormReferred").val($("#tbReferred").val());
            this.confirmOrderForm.submit();
        };

        StateManager.prototype.SetCartState = function () {
            $("#connectionsCount").val(0);
            $('#collectAdditionalLocation').html('');
            $("#confirmationTab").hide();
            $("#signUpTab").hide();
            $("#contactInfoTab").hide();
            $('#AddToCart').attr({ disabled: false, value: 'SignUp' });
            $('#AddToCart1').attr({ disabled: false, value: 'SignUp' });
        };
        return StateManager;
    })();
    OrderRegistration.StateManager = StateManager;
    ;
})(OrderRegistration || (OrderRegistration = {}));
//function setPath() {
//    var indexOfHome = location.href.indexOf('Account');
//    var path = '';
//    if (indexOfHome > -1)
//        path = location.href.substr(0, location.href.indexOf('Account') - 1);
//    else
//        path = location.href;
//    //  IE is a rubbish browser!
//    if (path === '')
//        path = $(location).attr('href');
//    return path;
//}
//function isValidEmailAddress(emailAddress : string) {
//    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
//    return pattern.test(emailAddress);
//};
//# sourceMappingURL=create-order-new.js.map
