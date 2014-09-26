var discount,
    pageObject,
    signUpForm,
    stateManager;


discount = '';


$(function() {

    signUpForm = $("#SignUpForm");

    pageObject = new OrderRegistration.PageObject();

    pageObject.setCheckoutInProcess(checkoutInProcess);
    pageObject.setWhichStep(whichStep);
    pageObject.setOrderRowId(orderRowId);
    pageObject.setIsUserLogged(isUserLogged);

    stateManager = new OrderRegistration.StateManager(pageObject);

    stateManager.SetCartState();

    $("[id^='regTypeID_']").on("click", function (oEvent) {
        $("#stage_of_checkout").val("preReg");

        //BuildPreRegPrice(oEvent, orderRowId);
        stateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });

    $('[id^="AddToCart"]').on('click', function () {

        $(signUpForm).find('[name="RegistrationType"]').val($('input[name=RegistrationType]:checked', '#RegistrationType').val());
        //alert($('input[name=RegistrationType]:checked', '#RegistrationType').val());
        signUpForm.submit();
    });

    if (!discount == "none") {
        $('#showDiscount').css('display', 'block');
    }

    if (pageObject.getOrderRowId() > 0 && CheckoutInProcess) {
        $.get("/cart/checkoutConfirm/" + pageObject.getOrderRowId())
            .success(function (dataConfirm) {
                $('#confirmation').replaceWith(dataConfirm);
                $.get("/cart/checkoutContact/" + pageObject.getOrderRowId())
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
        if (!isUserLogged) {
            //
            //TODO: Re-use code that's already been developed for the CreateAccount use case
            //  with this difference.... the UI needs to be presented in a Modal Popup
            alert("get Contact info");
        } else {

            $("#ProgressDialogBS").modal('show');
            var data = signUpForm.serialize();
            $.post(signUpForm.attr("action"), data, function (result, status) {
                if (result.success) {
                    //jslogger.event({ signup: { from: "EndUser Checkout" } });
                    pageObject.setOrderRowId(result.orderRowId);
                    pageObject.setWhichStep(result.whichStep);

                    $.get("/cart/checkoutConfirm/" + pageObject.getOrderRowId())
                        .success(function (dataConfirm) {
                            $('#confirmation').replaceWith(dataConfirm);
                        }).done(function () {
                            $.get("/cart/checkoutOptions/" + pageObject.getOrderRowId())
                                .success(function (dataOptions) {
                                    $('#signUp').replaceWith(dataOptions);
                                    $.get("/cart/checkoutContact/" + pageObject.getOrderRowId())
                                        .success(function (dataContact) {
                                            $('#contactInfo').replaceWith(dataContact);
                                        })
                                        .done(function () {
                                            stateManager.SetCartState();
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