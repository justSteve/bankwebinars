var discount,
    cartStateManager,
    signUpForm,
    cartStateManager;


discount = '';


$(function() {

    signUpForm = $("#SignUpForm");

    cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setCheckoutInProcess(checkoutInProcess);
    cartStateManager.setWhichStep(whichStep);
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

    if (cartStateManager.getOrderRowId() > 0 && CheckoutInProcess) {
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
        CheckoutInProcess = true;

        if (!cartStateManager.getIsUserLogged()) {

            cartStateManager.setWhichStep('Step1');
            cartStateManager.SetCartState();
            $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
            $('#contactInfo').load('/Cart/CheckoutContactDetails', function () {
                
                registerDuringCheckout.initialize();
            });

        } else {

            $("#ProgressDialogBS").modal('show');
            var data = signUpForm.serialize();
            $.post(signUpForm.attr("action"), data, function (result, status) {
                if (result.success) {
                    //jslogger.event({ signup: { from: "EndUser Checkout" } });
                    cartStateManager.setOrderRowId(result.orderRowId);
                    cartStateManager.setWhichStep(result.whichStep);

                    $.get("/cart/checkoutConfirm/" + cartStateManager.getOrderRowId())
                        .success(function (dataConfirm) {
                            $('#confirmation').replaceWith(dataConfirm);
                        }).done(function () {
                            $.get("/cart/checkoutOptions/" + cartStateManager.getOrderRowId())
                                .success(function (dataOptions) {
                                    $('#signUp').replaceWith(dataOptions);
                                    $.get("/cart/checkoutContact/" + cartStateManager.getOrderRowId())
                                        .success(function (dataContact) {
                                            $('#contactInfo').replaceWith(dataContact);
                                        })
                                        .done(function () {
                                            cartStateManager.SetCartState();
                                            //alert(whichStep);
                                        });
                                });
                        });
                } else {
                    //jslogger.log({ exception: { name: "SignupFail", message: "The signUpForm submission failed." } });
                    $('.signupErrors').html("Invalid Data. Try again?");
                }
            }, "json");


            $("#confirmationTab").tab('show');

            $("#ProgressDialogBS").modal('hide');

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