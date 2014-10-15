//  This script correlates with the Details2 View.

var checkoutConfirm,
    discount,
    cartStateManager,
    signUpForm,
    cartStateManager;


discount = '';
checkoutConfirm = {};


$(function() {

    signUpForm = $('#SignUpForm');

    checkoutConfirm.initialize = function () {

        cartStateManager.setCancelOrderForm($('#confirmOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        $('#ConfirmRegistrationBillMe').on('click', function(e) {
            cartStateManager.PlaceOrder();
        });

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {

            console.log('submitting ConfirmOrder');

            e.preventDefault();

            var self = $(this);
            //$('#ProgressDialogBS').modal('show');
            self.find('input[name="id"]').val(cartStateManager.getOrderId());

            var data = $(this).serialize();

            $.post(self.attr('action'), data, function (result, status) {
                if (result.success) {
                    orderRowID = result.orderRowID;

                    $('#confirmResult').html(result.msg);
                    $('#confirmRegistration').attr('href', 'javascript:location.reload();');
                    //$.get('/cart/checkoutConfirm/' + orderRowID, function (dataConfirm) {
                    //    $('#confirmation').replaceWith(dataConfirm);
                    //});
                    //$('#signUpTab').show();
                    //$('#contactInfoTab').show();

                    CheckoutInProcess = false;

                    //$('#ProgressDialogBS').modal('hide');
                    $('#ConfirmModal').modal('show');
                } else {
                    $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
                }
            }, 'json');
            return false;

        });

        cancelOrderForm.submit(function(e) {
            console.log("Submitting Cancel Order");
            e.preventDefault();
            var data = cancelOrderForm.serialize();
            $.post(cancelOrderForm.attr("action"), data, function(result, status) {
                if (result.success) {

                    $('#cancelCaption').text("Order is canceled");
                    $("#cancelRegistration").unbind("click");
                    $("#rtn").hide();
                    $("#continueReg").show();
                    $("#cancelResult").hide();
                    $("#cancelRegistration").hide();
                    //$("#continueReg").attr("href", "/");

                } else {
                    alert("else");
                    $('.signupErrors').html("Invalid Data. Try again?");
                }
            }, "json");
            return false;
        });

    };

    cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setCheckoutInProcess(checkoutInProcess);
    cartStateManager.setOrderRowId(orderRowId);
    cartStateManager.setIsUserLogged(isUserLogged);


    cartStateManager.SetCartState();

    $("[id^='regTypeID_']").on("click", function (oEvent) {
        $("#stage_of_checkout").val("preReg");

        //BuildPreRegPrice(oEvent, orderRowId);
        cartStateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });

    $('[id^="AddToCart"]').on('click', function () {

        $(signUpForm).find('[name="RegistrationType"]').val($('input[name=RegistrationType]:checked', '#RegistrationType').val());
        $(signUpForm).find('[name="stageOfCheckout"]').val("preRegistration");
        
        signUpForm.submit();
    });

    if (!discount == "none") {
        $('#showDiscount').css('display', 'block');
    }

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
        $("#eDetails").collapse('hide');
    }

    signUpForm.submit(function (e) {

        e.preventDefault();
        cartStateManager.setCheckoutInProcess(true);
        $('#ProgressDialogBS').modal('show');
        var data = signUpForm.serialize();

        if (!cartStateManager.getIsUserLogged()) {

            $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');

            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {
                    
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);
                    $('#contactInfo').load('/Cart/CheckoutContactDetails', function(response, status, xhr) {
                        $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details2/' + result.webinarId);
                        registerDuringCheckout.initialize(result.orderId, result.webinarId);
                    });

                    $('#contactInfoTab a').tab('show');
                } else {
                    //jslogger.log({ exception: { name: 'SignupFail', message: 'The signUpForm submission failed.' } });
                    $('.signupErrors').html('Invalid Data. Try again?');
                }
            }, constants.JsonDataType);
            

        } else {

            $.post(signUpForm.attr('action'), data, function (result) {
                if (result.success) {
                    //jslogger.event({ signup: { from: 'EndUser Checkout' } });
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setOrderId(result.orderId);
                    cartStateManager.setWebinarId(result.webinarId);

                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function (response, status, xhr) {

                        $('#confirmationTab a').tab('show');
                        checkoutConfirm.initialize();
                    });

                } else {
                    //jslogger.log({ exception: { name: 'SignupFail', message: 'The signUpForm submission failed.' } });
                    $('.signupErrors').html('Invalid Data. Try again?');
                }
            }, constants.JsonDataType);


            //$('#confirmationTab').tab('show');

            //$('#ProgressDialogBS').modal('hide');

            return false;
        }
    });

    $('#CreateUserSubmitter').on('click', function () {
        var nameVal = $('#FullName').val();
        var nameLength = nameVal.length;
        var nameSplit = nameVal.split(" ");
        var lastLength = nameLength - nameSplit[0].length;
        var lastNameLength = nameSplit[0].length + 1;
        var lastName = nameVal.slice(lastNameLength);
        $('#FirstName').val(nameSplit[0]);
        $('#LastName').val(lastName);

        nameVal = $('#FullNameShipping').val();
        nameLength = nameVal.length;
        nameSplit = nameVal.split(" ");
        lastLength = nameLength - nameSplit[0].length;
        lastNameLength = nameSplit[0].length + 1;
        lastName = nameVal.slice(lastNameLength);
        $('#ShippingFirstName').val(nameSplit[0]);
        $('#ShippingLastName').val(lastName);

        $("#_CreateUserForm").submit();
    });

});