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

        console.log('initialize hit');

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {
            //Rollbar.info('submitting confirmOrder form');
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());
            var err = new Error('submitting ConfirmOrder' + cartStateManager.getOrderRowId());
            //NREUM.noticeError(err);

            var data = $(this).serialize();
            $('#ConfirmRegistrationBillMe').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');

            $.ajax({
                type: 'POST',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: self.attr('action'),
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: data,
                beforeSend: function () {
                    $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');
                    //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
                }
            }).done(function (data) {
                if (data.Result === 'Success') {
                    orderRowId = data.OrderRowId;

                    var err = new Error('Posted Order: ' + orderRowId);
                    //NREUM.noticeError(err);
                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    $('#ConfirmModal').modal('show');

                } else {

                    var err = new Error('FAILED posting Order: ');
                    //NREUM.noticeError(err);

                    $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                $('#ConfirmRegistrationBillMe').removeAttr('disabled');
                $('#signUpSpinner').remove();
            }).fail(function(jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                $('#ConfirmRegistrationBillMe').removeAttr('disabled');
                $('#signUpSpinner').remove();
                $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Transport error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
            });

            $('#ConfirmModal').on('hidden', function (e) {
                var utilities = new Common.Utilities();
                console.log('/webinar/details/' + cartStateManager.getWebinarId());
                utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());
            });
        });

        var cancelOrderForm = cartStateManager.getCancelOrderForm();

        cancelOrderForm.on('submit', function (e) {

            console.log('cancelOrderForm submit hit');
            //Rollbar.info('Submitting cancelOrderForm');
            e.preventDefault();
            e.stopImmediatePropagation();

            $('#cancelModalOrderId').val(cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);

            $('#cancelRegistration').on('click', function (e) {
                e.preventDefault();

                // disable button while operation in progress
                $('#cancelRegistration').attr('disabled', 'disabled');

                $.post(self.attr('action'), data, function (response, status, xhr) {

                    if (status !== 'error') {
                        if (xhr.responseJSON['success']) {
                            var err = new Error('Suceeded in cancelling order');
                            //NREUM.noticeError(err);

                            var utilities = new Common.Utilities();
                            console.log('/webinar/details/' + cartStateManager.getWebinarId());
                            utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());
                        } else {
                            var err = new Error('Cancel Order Failure');
                            //NREUM.noticeError(err);

                            $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                        // enable button again upon ending operation.
                        //$('#cancelRegistration').removeAttr('disabled');  // [dar] NO. On staging, redirect is slow and button enabled again. User could have clicked it again.

                    } else {
                        $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Server Error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                        $('#CancelModal').modal('hide');
                    }
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
    cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view Details.cshtml
    cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tab in razor view Details.cshtml
    cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tab in razor view Details.cshtml

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
        $(this).prepend('<i id="signUpSpinner" class="icon-spinner icon-spin"></i>').attr('disabled', 'disabled');
        signUpForm.submit();
    });

    if (discount !== 'none') {
        $('#showDiscount').css('display', 'block');
    }

    //  flow goes inside this block where the order exists and is in process e.g. previously abandoned before finializing
    if (cartStateManager.getOrderRowId() > 0 && cartStateManager.getCheckoutInProcess()) {

        if (shippingAddressRequired && notificationsTesting === false) {
            // Following function lives in the register-during-checkout.js script
            // which will be in memory at this point and thus will have been hoisted.
            hookUpModal($('#UserDetailsModal'));
        }

        cartStateManager.setOrderId(orderId);

        // see top of this file
        checkoutConfirm.initialize();

         //The Bill Me button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function (e) {
            e.preventDefault();
            var confirmOrderForm = $('#confirmOrder');
            confirmOrderForm.submit();
        });

        // The Cancel Registration button on 3rd tab
        $('#Canceller').on('click', function (e) {
            e.preventDefault();
            var cancelOrderForm = $('#cancelOrder');
            cancelOrderForm.submit();
        });

        setUpEditButtons();

        // Following 3 functions live in the register-during-checkout.js script
        // which will be in memory at this point and thus will be hoisted
        hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
        hookUpChangeTypeLogic($('#RegType'));
        hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);
    }

    /* Submit event for the big green SignUp button */
    signUpForm.on('submit', function (e) {
        e.preventDefault();

        var beigeFormArea = signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippindAddressRequired function
        //  valu comes from a hidden impact in the radio btn list next to the relevant radio button (previous-sibling)
        shippingAddressRequired = isShippindAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = signUpForm.serialize();

        $('#SignUpForm > div');
        var spinner = $('#signUpSpinner');

        // If the user IS NOT LOGGED IN - control moves to the register-during-checkout.js script
        if (!cartStateManager.getIsUserLoggedIn()) {
            var err = new Error('anon user hits signup');
            //NREUM.noticeError(err);

            $.post(signUpForm.attr('action'), data, function (response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {
                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                        $('#contactInfo').load('/Cart/CheckoutContactDetails', function (response, status, xhr) {
                            if (status !== 'error') {
                                $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details/' + cartStateManager.getWebinarId());
                                registerDuringCheckout.initialize(cartStateManager.getOrderId(), cartStateManager.getWebinarId(), cartStateManager.getOrderRowId(), shippingAddressRequired, checkoutConfirm.initialize);
                            } else {
                                $('#labelEmail').html('<span class="label label-important">&nbsp;Server error. Try again or call 800-831-0678 ext 706 for immediate assistance!</span>');
                            }
                        });

                        $('#contactInfoTab a').tab('show');
                    } else if (!xhr.responseJSON['isSuccessful']) {
                        $('#labelEmail').html('<span class="label label-important">&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);
                    }
                } else {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Server error. Try again or call 800-831-0678 ext 706 for immediate assistance!</span>');
                }
            }, constants.JsonDataType);
        } else {
            // If the user IS LOGGED IN
            $.post(signUpForm.attr('action'), data, function (response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {
                        //Rollbar.info({ signup: { from: 'EndUser Checkout', orderRowId: data.orderRowId } });
                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);

                        var err = new Error('submitting ConfirmOrder' + xhr.responseJSON['orderId']);
                        //NREUM.noticeError(err);

                        $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function(response, status, xhr) {

                            if (status === 'error') {
                                $(this).html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                                $('#loadingSpinner').remove();
                                $('#confirmationTab a').tab('show');
                            } else {

                                $('#confirmationTab a').tab('show');

                                if (shippingAddressRequired && notificationsTesting === false) {
                                    // Following function lives in the register-during-checkout.js script
                                    // which will be in memory at this point and thus will have been hoisted.
                                    hookUpModal($('#UserDetailsModal'));
                                }

                                // see top of this file
                                checkoutConfirm.initialize();

                                // The Bill Me button on 3rd tab
                                $('#ConfirmRegistrationBillMe').on('click', function(e) {
                                    e.preventDefault();
                                    var confirmOrderForm = $('#confirmOrder');
                                    confirmOrderForm.submit();
                                });

                                // The Cancel Registration button on 3rd tab
                                $('#Canceller').on('click', function(e) {
                                    e.preventDefault();
                                    var cancelOrderForm = $('#cancelOrder');
                                    cancelOrderForm.submit();
                                });

                                setUpEditButtons();

                                // Following 3 functions live in the register-during-checkout.js script
                                // which will be in memory at this point and thus will be hoisted
                                hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
                                hookUpChangeTypeLogic($('#RegType'));
                                hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

                                beigeFormArea.height($('#confirmation').height() + 30);
                            }

                            spinner.remove();

                        }, constants.HtmlDataType);
                    } else if (xhr.responseJSON['isSuccessful'] === false) {
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);

                        spinner.remove();
                    }
                } else {
                    spinner.remove();
                    $('#confirmation').html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                }
            }, constants.JsonDataType);

            return false;
        }
        return false;
    });
});

function isShippindAddressRequired(jQueryObject) {
    if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
        return false;
    return true;
}

function setUpEditButtons() {
    $('#revealOptions').on('click', function (e) {
        e.preventDefault();
        $('#AdjustOrder').slideToggle();
    });

    $('#revealDiscountInput').on('click', function (e) {
        e.preventDefault();
        $('#AdjustDiscount').slideToggle();
    });

    $('#revealAddLocsPanel').on('click', function (e) {
        e.preventDefault();
        $('#AdjustAddLoc').slideToggle();
    });
}
