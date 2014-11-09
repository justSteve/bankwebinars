
//  This script correlates with the Details View.

var checkoutConfirm,
    discount,
    cartStateManager,
    orderRowId,
    shippingAddressRequired,
    signUpForm,
    signUpFormContainer,
    storedHeight;

orderRowId = 0;
discount = '';
checkoutConfirm = {};


$(function () {

    signUpForm = $('#SignUpForm');
    signUpFormContainer = $('#SignUpFormContainer');

    /* This function gets invoked when the 3rd tab is loaded when an existing is using cart */
    checkoutConfirm.initialize = function (userId) {

        cartStateManager.setCancelOrderForm($('#cancelOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {

            //console.log('submitting ConfirmOrder');
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());
            //appInsights.trackEvent("submitting ConfirmOrder");

            var data = $(this).serialize();
            $('#ConfirmRegistrationBillMe').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            $.post(self.attr('action'), data, function (result, status) {
                if (result.Result === 'Success') {
                    orderRowID = result.OrderRowID;


                    //appInsights.trackEvent("Posted Order: " + orderRowID);
                    $('#orderDetails').empty();
                    $('#orderDetails').append(result.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    $('#ConfirmModal').modal('show');

                    $('#finalLoadingSpinner').remove();
                } else {
                    //TODO: Add code that will provide as much detail to the Failed message as can be obtained from result.
                    //appInsights.trackEvent("FAILED posting Order: ");
                    $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
                }
            }, constants.JsonDataType);

            $('#ConfirmModal').on('hidden', function (e) {

                location.reload(true);

            });
        });

        cartStateManager.getCancelOrderForm().on('submit', function (e) {
            //console.log('Submitting Cancel Order');

            //appInsights.trackEvent("Cancelling Order by user");
            e.preventDefault();

            $('#cancelModalOrderId').val(cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);

            $.post(self.attr('action'), data, function (response, status, xhr) {
                if (response.success) {
                    //appInsights.trackEvent("Suceeded in canceling order");
                    $('#cancelCaption').text('Order is canceled');
                    $('#cancelRegistration').off('click');
                    $('#rtn').hide();
                    $('#continueReg').show();
                    $('#cancelResult').hide();
                    $('#cancelRegistration').hide();
                    $('#continueReg').attr('href', 'http://localhost:3538/Webinar/Details/' + cartStateManager.getWebinarId());

                } else {

                    //appInsights.trackEvent("Cancel Order Failure");
                    $('.signupErrors').html('Invalid Data. Try again?');
                }
            }, 'json');
            return false;
        });

    };

    shippingAddressRequired = false; //todo: this should be done dynamically from db

    cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setWebinarId(webinarId); // set in razor view
    cartStateManager.setOrderRowId(orderRowId);
    cartStateManager.setIsUserLogged(isUserLogged);
    cartStateManager.setShippingAddressRequired(shippingAddressRequired);
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
        $.get("/cart/checkoutConfirm/" + cartStateManager.getOrderRowId())
            .success(function (dataConfirm) {
                $('#confirmation').replaceWith(dataConfirm);
                $.get("/cart/checkoutContact/" + cartStateManager.getOrderRowId())
                    .success(function (dataContact) {
                        $('#contactInfo').replaceWith(dataContact);
                    })
                    .done(SetCartState());
            }).error(function (result) {
                //jslogger.log({ exception: { name: "updateCheckout", message: "The update of checkout request failed." } });
            });
    }

    /* Submit event for the big green SignUp button */
    signUpForm.submit(function (e) {

        e.preventDefault();

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        var regTypeLabel = $.trim($('dl dt input:checked').parent().text());

        shippingAddressRequired = cartStateManager.getShippingAddressRequired();

        var data = signUpForm.serialize();

        $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');

        // If the user IS NOT LOGGED IN - move to file register-during-checkout.js
        if (!cartStateManager.getIsUserLogged()) {

            //appInsights.trackEvent("anon user hits signup");
            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {

                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);
                    $('#contactInfo').load('/Cart/CheckoutContactDetails', function (response, status, xhr) {
                        $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details2/' + result.webinarId);
                        registerDuringCheckout.initialize(result.orderId, result.webinarId, result.orderRowId, shippingAddressRequired, checkoutConfirm.initialize);
                    });
                    signUpFormContainer.height(storedHeight);

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
                    //jslogger.event({ signup: { from: 'EndUser Checkout' } });
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);
                    //appInsights.trackEvent("submitting ConfirmOrder" + result.userId);

                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function (response, status, xhr) {
                        $('#confirmationTab a').tab('show');
                        checkoutConfirm.initialize();
                        // see top of this file
                        // 
                        $('#ConfirmRegistrationBillMe').on('click', function (e) {
                            e.preventDefault();
                            var confirmOrderForm = $('#confirmOrder');
                            confirmOrderForm.submit();
                            confirmOrderForm.off('submit');
                        });

                        $('#Canceller').on('click', function (e) {
                            e.preventDefault();
                            var cancelOrderForm = cartStateManager.getCancelOrderForm();
                            cancelOrderForm.submit();
                            cancelOrderForm.off('submit');
                        });

                        hookUpApplyDiscountLogic($('#SubmitDiscountCode'));
                        hookUpChangeTypeLogic($('#RegType'));
                        hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

                    }, constants.HtmlDataType);

                    signUpFormContainer.height(storedHeight);

                } else {
                    //jslogger.log({ exception: { name: 'SignupFail', message: 'The signUpForm submission failed.' } });
                    $('.signupErrors').html('Invalid Data. Try again?');
                }
            }, constants.JsonDataType);

            return false;
        }
    });
});

function isShippindAddressRequired(regTypeLabel) {

    if (regTypeLabel.indexOf('Live Session Only') > -1 || regTypeLabel.indexOf('Live Plus OnDemand Weblinks') > -1)
        return false;
    return true;

}