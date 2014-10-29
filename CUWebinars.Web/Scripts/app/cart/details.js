
//  This script correlates with the Details2 View.

var checkoutConfirm,
    discount,
    cartStateManager,
    signUpForm,
    signUpFormContainer,
    storedHeight;


discount = '';
checkoutConfirm = {};


$(function () {

    signUpForm = $('#SignUpForm');
    signUpFormContainer = $('#SignUpFormContainer > div > div');

    /* This function gets invoked when the 3rd tab is loaded when an existing is using cart */
    checkoutConfirm.initialize = function (userId) {

        cartStateManager.setCancelOrderForm($('#cancelOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {

            //console.log('submitting ConfirmOrder');
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());

            JL("myLogger").info("submitting ConfirmOrder");
            var data = $(this).serialize();

            $.post(self.attr('action'), data, function (result, status) {
                if (result.success) {
                    orderRowID = result.orderRowID;

                    JL("myLogger").info("Posted Order: " + orderRowID);
                    $('#confirmResult').html(result.msg);
                    $('#confirmRegistration').attr('href', 'javascript:location.reload();');
                    
                    $('#ConfirmModal').modal('show');
                } else {
                    //TODO: Add code that will provide as much detail to the Failed message as can be obtained from result.
                    JL("myLogger").fatal("FAILED posting Order: ");
                    $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
                }
            }, constants.JsonDataType);
            return false;

        });

        cartStateManager.getCancelOrderForm().submit(function (e) {
            //console.log("Submitting Cancel Order");
            JL("myLogger").info("Canceling Order");
            e.preventDefault();
            var data = cancelOrderForm.serialize();
            $.post(cancelOrderForm.attr("action"), data, function (result, status) {
                if (result.success) {

                    JL("myLogger").info("Suceeded in canceling order");
                    $('#cancelCaption').text("Order is canceled");
                    $("#cancelRegistration").unbind("click");
                    $("#rtn").hide();
                    $("#continueReg").show();
                    $("#cancelResult").hide();
                    $("#cancelRegistration").hide();
                    //$("#continueReg").attr("href", "/");

                } else {

                    JL("myLogger").fatal("Cancel Order Failure");
                    $('.signupErrors').html("Invalid Data. Try again?");
                }
            }, "json");
            return false;
        });

    };

    cartStateManager = new OrderRegistration.StateManager();
    
    cartStateManager.setOrderRowId(orderRowId);
    cartStateManager.setIsUserLogged(isUserLogged);
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

        var data = signUpForm.serialize();

        $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');

        // If the user IS NOT LOGGED IN - move to file register-during-checkout.js
        if (!cartStateManager.getIsUserLogged()) {

            JL("myLogger").info("the user IS NOT LOGGED IN");

            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {

                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);
                    $('#contactInfo').load('/Cart/CheckoutContactDetails', function (response, status, xhr) {
                        $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details2/' + result.webinarId);
                        registerDuringCheckout.initialize(result.orderId, result.webinarId, result.orderRowId, checkoutConfirm.initialize);
                    });
                    //signUpFormContainer.height(storedHeight);

                    $('#contactInfoTab a').tab('show');
                } else {
                    //jslogger.log({ exception: { name: 'SignupFail', message: 'The signUpForm submission failed.' } });
                    $('.signupErrors').html('Invalid Data. Try again?');
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

                    JL("myLogger").info("submitting ConfirmOrder" + result.userId);

                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function (response, status, xhr) {
                        $('#confirmationTab a').tab('show');
                        checkoutConfirm.initialize();
                        // see top of this file
                        // 
                        $('#ConfirmRegistrationBillMe').on('click', function (e) {
                            $('#confirmOrder').submit();
                        });

                        hookUpApplyDiscountLogic($('#SubmitDiscountCode'));

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

