//  This script correlates with the Details View.
var checkoutConfirm, discount, cartStateManager, shippingAddressRequired, signUpForm, signUpFormContainer, storedHeight;

discount = '';
checkoutConfirm = {};

$(function () {
    signUpForm = $('#SignUpForm');
    signUpFormContainer = $('#SignUpFormContainer'); // The big beige box

    /* This function gets invoked when the 3rd tab is loaded and an existing user is using the cart */
    checkoutConfirm.initialize = function (userId) {
        cartStateManager.setCancelOrderForm($('#cancelOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {
            //Rollbar.info('submitting confirmOrder form');
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());
            var err = new Error('submitting ConfirmOrder' + cartStateManager.getOrderRowId());
            NREUM.noticeError(err);

            var data = $(this).serialize();
            $('#ConfirmRegistrationBillMe').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');

            $.post(self.attr('action'), data, function (result, status) {
                if (result.Result === 'Success') {
                    orderRowID = result.OrderRowID;

                    var err = new Error('Posted Order: ' + orderRowID);
                    NREUM.noticeError(err);
                    $('#orderDetails').empty();
                    $('#orderDetails').append(result.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    $('#ConfirmModal').modal('show');

                    $('#finalLoadingSpinner').remove();
                } else {
                    //TODO: Add code that will provide as much detail to the Failed message as can be obtained from result.
                    var err = new Error('FAILED posting Order: ');
                    NREUM.noticeError(err);

                    $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
                }

                $('#ConfirmRegistrationBillMe').removeAttr('disabled');
            }, constants.JsonDataType);

            $('#ConfirmModal').on('hidden', function (e) {
                var utilities = new Common.Utilities();
                console.log('/webinar/details/' + cartStateManager.getWebinarId());
                utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());
            });
        });

        var cancelOrderForm = cartStateManager.getCancelOrderForm();

        cancelOrderForm.on('submit', function (e) {
            //Rollbar.info('Submitting cancelOrderForm');
            e.preventDefault();

            $('#cancelModalOrderId').val(cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);

            $('#cancelRegistration').on('click', function (e) {
                e.preventDefault();

                // disable button while operation in progress
                $('#cancelRegistration').attr('disabled', 'disabled');

                $.post(self.attr('action'), data, function (response, status, xhr) {
                    if (response.success) {
                        var err = new Error('Suceeded in canceling order');
                        NREUM.noticeError(err);

                        var utilities = new Common.Utilities();
                        console.log('/webinar/details/' + cartStateManager.getWebinarId());
                        utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());
                    } else {
                        var err = new Error('Cancel Order Failure');
                        NREUM.noticeError(err);
                        $('.signupErrors').html('Invalid Data. Try again?');
                        $('#ConfirmModal').modal('hide');
                    }

                    // enable button again upon ending operation.
                    $('#cancelRegistration').removeAttr('disabled');
                }, 'json');

                // unbind event so we don't get them building up each time the user clicks the Cancel Registration button.
                $(this).off('click');
                $('#rtn').off('click');
            });

            $('#rtn').on('click', function (e) {
                e.preventDefault();
                $('#CancelModal').modal('hide');
                $(this).off('click');
                $('#cancelRegistration').off('click');
            });

            $('#CancelModal').modal('show');
        });
    };

    cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tab in razor view Details.cshtml
    cartStateManager.setOrderRowId(orderRowId); // orderRowId is set at top of this file
    cartStateManager.setIsUserLogged(isUserLogged); // isUserLogged is set in a script tab in razor view Details.cshtml
    cartStateManager.setCheckoutInProcess(checkoutInProcess);

    cartStateManager.SetCartState();

    $("[id^='regTypeID_']").on("click", function (oEvent) {
        //TODO: In original code i had intialized the how the cart displayed the price.
        // clearly this takes place elsewhere now adaquately but review and verify that
        // this is impacted by the Discount - an enitity we've yet to dance with - but this
        // provides the perfect opp to introduce this user story:
        //
        // An existing user can posses one or more 'credits' that should be honored (acknowedged) by the shopping
        // cart as soon as the user's identity is known. Specifically, the cart must display the amount
        // of the discount as well as ensuring that the cart's 'Total' field reflects the discount. Point being that
        // that the cart shouldn't depend on the user to supply the discount.
        //
        // and....
        // absent a pre-existing discount code, the cart must suppy a form field to permit an
        // ajax call to the server to validate anything entered by the user on the Confirmation Tab.
        //
        //cartStateManager.BuildPreRegPrice(oEvent, cartStateManager.getOrderRowId());
        cartStateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });

    /* Click event for the big green SignUp button */
    $('#AddToCart').on('click', function () {
        storedHeight = signUpFormContainer.height();
        signUpForm.submit();
    });

    if (!discount == "none") {
        $('#showDiscount').css('display', 'block');
    }

    // TODO: Note I have not touched this handler yet.
    // [dar] this handler is relevant for update/edit/view aspect of cart. Revisit when we address that.
    if (cartStateManager.getOrderRowId() > 0 && cartStateManager.getCheckoutInProcess()) {
        if (shippingAddressRequired && notificationsTesting === false) {
            // Following function lives in the register-during-checkout.js script
            // which will be in memory at this point and thus will have been hoisted.
            hookUpModal($('#UserDetailsModal'));
        }

        // see top of this file
        checkoutConfirm.initialize();

        // The Bill Me button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function (e) {
            e.preventDefault();
            var confirmOrderForm = $('#confirmOrder');
            confirmOrderForm.submit();
            confirmOrderForm.off('submit');
        });

        // The Cancel Registration button on 3rd tab
        $('#Canceller').on('click', function (e) {
            e.preventDefault();
            var cancelOrderForm = cartStateManager.getCancelOrderForm();
            cancelOrderForm.submit();
        });

        // Following 3 functions live in the register-during-checkout.js script
        // which will be in memory at this point and thus will be hoisted
        hookUpApplyDiscountLogic($('#SubmitDiscountCode'));
        hookUpChangeTypeLogic($('#RegType'));
        hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

        beigeFormArea.height($('#confirmation').height() + 30);
    }

    /* Submit event for the big green SignUp button */
    signUpForm.submit(function (e) {
        e.preventDefault();

        var beigeFormArea = signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippindAddressRequired function
        //  valu comes from a hidden impact in the radio btn list next to the relevant radio button (previous-sibling)
        shippingAddressRequired = isShippindAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = signUpForm.serialize();

        $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        var spinner = $('#loadingSpinner');

        // If the user IS NOT LOGGED IN - control moves to the register-during-checkout.js script
        if (!cartStateManager.getIsUserLogged()) {
            var err = new Error('anon user hits signup');
            NREUM.noticeError(err);

            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);
                    $('#contactInfo').load('/Cart/CheckoutContactDetails', function (response, status, xhr) {
                        $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details2/' + result.webinarId);
                        registerDuringCheckout.initialize(result.orderId, result.webinarId, result.orderRowId, shippingAddressRequired, checkoutConfirm.initialize);
                    });

                    $('#contactInfoTab a').tab('show');
                } else if (!data.isSuccessful) {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There were some problems with the form. Please refer to the items in red.</span>');

                    formProcessor.lightUpValidationSummary('valSummarySignUpForm', result);
                }
            }, constants.JsonDataType);
        } else {
            // If the user IS LOGGED IN
            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {
                    //Rollbar.info({ signup: { from: 'EndUser Checkout', orderRowId: result.orderRowId } });
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);

                    var err = new Error('submitting ConfirmOrder' + result.orderId);
                    NREUM.noticeError(err);

                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function (response, status, xhr) {
                        $('#confirmationTab a').tab('show');

                        if (shippingAddressRequired && notificationsTesting === false) {
                            // Following function lives in the register-during-checkout.js script
                            // which will be in memory at this point and thus will have been hoisted.
                            hookUpModal($('#UserDetailsModal'));
                        }

                        // see top of this file
                        checkoutConfirm.initialize();

                        // The Bill Me button on 3rd tab
                        $('#ConfirmRegistrationBillMe').on('click', function (e) {
                            e.preventDefault();
                            var confirmOrderForm = $('#confirmOrder');
                            confirmOrderForm.submit();
                            confirmOrderForm.off('submit');
                        });

                        // The Cancel Registration button on 3rd tab
                        $('#Canceller').on('click', function (e) {
                            e.preventDefault();
                            var cancelOrderForm = cartStateManager.getCancelOrderForm();
                            cancelOrderForm.submit();
                        });

                        // Following 3 functions live in the register-during-checkout.js script
                        // which will be in memory at this point and thus will be hoisted
                        hookUpApplyDiscountLogic($('#SubmitDiscountCode'));
                        hookUpChangeTypeLogic($('#RegType'));
                        hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

                        beigeFormArea.height($('#confirmation').height() + 30);
                    }, constants.HtmlDataType);
                } else if (result.isSuccessful === false) {
                    formProcessor.lightUpValidationSummary('valSummarySignUpForm', result);

                    spinner.remove();
                }
            }, constants.JsonDataType);

            return false;
        }
    });
});

function isShippindAddressRequired(jQueryObject) {
    if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
        return false;
    return true;
}
//# sourceMappingURL=details.js.map
