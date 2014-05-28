$('input[name=mode]:eq(0)').attr('checked', 'checked');
var discount = '';
var wait4Emails = '';
var orderRowID = 0;
var CheckoutInProcess = false;
var isUserLogged = false;
var whichStep = "Step0";

function setPath() {
    var indexOfHome = location.href.indexOf('Account');
    var path = '';

    if (indexOfHome > -1)
        path = location.href.substr(0, location.href.indexOf('Account') - 1);
    else
        path = location.href;

    //  IE is a rubbish browser!
    if (path === '')
        path = $(location).attr('href');
    return path;
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
};
function showHtmlPopup(url) {
    window.open(url, null, "width=800, height=600,toolbar=1,scrollbars=1,location=1,statusbar=1,menubar=1,resizable=1");
    //'toolbar=1,scrollbars=0,location=1,statusbar=1,menubar=1,resizable=1,width=100,height=100');"
}


var exprsOrder = function (e) {
    alert(e);
};

$(document).ready(function () {
    SetCartState();
    var path = setPath();
    $("[id^='mode_']").on("click", function (oEvent) {
        $("#stage_of_checkout").val("preReg");
        //BuildPreRegPrice(oEvent);
        CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });


    $('[id^="AddToCart"]').on('click', function () {
        $("#frmSignup2 [name='mode']").val($('input[name=mode]:checked', '#ModeOptions').val());
        var emails2Add = "";
        $('[name^="Email"]').each(function () {
            emails2Add = emails2Add + ',' + $(this).val();
        });
        $("#frmSignup2 [name='addEmails']").val(emails2Add);   //alert($("#frmSignup2 [name='mode']").val());
        //alert($("#frmSignup2 [name='addEmails']").val());
        signUpForm.submit();
    });
    if (!discount == "none") {
        $('#showDiscount').css('display', 'block');
    }

    if (orderRowID > 0 && CheckoutInProcess == "true") {
        $.get("/cart/checkoutConfirm/" + orderRowID)
            .success(function (dataConfirm) {
                $('#confirmation').replaceWith(dataConfirm);
                $.get("/cart/checkoutContact/" + orderRowID)
                    .success(function (dataContact) {
                        $('#contactInfo').replaceWith(dataContact);
                    })
                    .done(SetCartState());
            }).error(function (result) {
                //jslogger.log({ exception: { name: "updateCheckout", message: "The update of checkout request failed." } });
            });
        $("#eDetails").collapse('hide');
    }

    var signUpForm = $("#frmSignup2");
    signUpForm.submit(function (e) {

        e.preventDefault();
        CheckoutInProcess = true;
        if (!isUserLogged) {
            window.location.href = "/Account/Login?returnURL=" + window.location;
            //TODO: How can javascript re-direct the execution to the '_CreateUserFrom' partial of the Login.cshtml?
            //  idea is that we should start off with the prompt for the email - if an account already exists
            // for that email the user is prompted to enter password.
            // --
            //  the flow must include enough 'returnURL' info to resume checkout after a new account (or login to existing)
            // is completed.
        } else {

            $("#ProgressDialogBS").modal('show');
            var data = signUpForm.serialize();
            $.post(signUpForm.attr("action"), data, function (result, status) {
                if (result.success) {
                    //jslogger.event({ signup: { from: "EndUser Checkout" } });
                    orderRowID = result.orderRowID;
                    whichStep = result.whichStep;

                    $("#eDetails").collapse('hide');
                    $.get("/cart/checkoutConfirm/" + orderRowID)
                        .success(function (dataConfirm) {
                            $('#confirmation').replaceWith(dataConfirm);
                        }).done(function () {
                            $.get("/cart/checkoutOptions/" + orderRowID)
                                .success(function (dataOptions) {
                                    $('#signUp').replaceWith(dataOptions);
                                    $.get("/cart/checkoutContact/" + orderRowID)
                                        .success(function (dataContact) {
                                            $('#contactInfo').replaceWith(dataContact);
                                        })
                                        .done(function () {
                                            SetCartState();
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



    //$('[id^="ConfirmRegistrationConfirmRegistration"]').on('click', function (e) {
    //    $('#stage_of_checkout').val(e.name);
    //    $('#LastName').val(lastName);

    //    $(signUpForm).submit();
    //});

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


    //  This handler was colliding with one by the same name in create-user.
    //  It is now invoked from the create-user.js script.
    $("#_CreateUserForm").on('submit', function (event) {
        event.preventDefault();
        var createUserForm = $(this);
        $("#CreateUserHeader").text("Submitting Entry");
        if ($("#FullNameShipping").val() === null || $("#FullNameShipping").val() === "") $("#FullNameShipping").val($("#FullName").val());
        if ($("#ShippingFirstName").val() === null || $("#ShippingFirstName").val() === "") $("#ShippingFirstName").val($("#FirstName").val());
        if ($("#ShippingLastName").val() === null || $("#ShippingLastName").val() === "") $("#ShippingLastName").val($("#LastName").val());
        if ($("#ShippingCity").val() === null || $("#ShippingCity").val() === "") $("#ShippingCity").val($("#City").val());
        if ($("#ShippingAddress").val() === null || $("#ShippingAddress").val() === "") $("#").val($("#Address").val());
        if ($("#ShippingAddress2").val() === null || $("#ShippingAddress2").val() === "") $("#").val($("#Address2").val());
        if ($("#ShippingState").val() === null || $("#ShippingState").val() === "") $("#ShippingState").val($("#State").val());
        if ($("#ShippingZip").val() === null || $("#ShippingZip").val() === "") $("#ShippingZip").val($("#Zip").val());

        //$("#ProgressDialogBS").modal('show');

        var data = createUserForm.serialize();
        var url = createUserForm.attr("action");

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: data,
            beforeSend: function () {
                console.log('beforeSend Register Details');
                // this is where we append a loading image
                REG.PageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function (data) {
            //alert('done: ');
            if (data.Result === 'Success') {
                console.log('success: ' + data.Result);
                stateManager.action = '';
                REG.PageObjects.labelEmail().html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                location.assign(path + '/'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
            } else if (data.Result === 'Fail') {
                REG.PageObjects.labelEmail().html('<span class="label label-important">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
                stateManager.action = actions.SubmitRegister;
            }
        }).fail(function (data) {
            console.log('failed: ' + data);
        }).always(function () {
            stateManager.inputAction = inputActions.None;
        });

        return false;
    });


});


function CheckIfAddLocShouldHide(optionID) {
    //don't show AdditionalEmails when RegType
    // can't support them. (ex: recorded only)

    $.ajax({
        url: "/cart/CheckIfAddLocShouldHide?optionID=" + optionID,
        type: "GET",
        cache: false,
        dataType: constants.JsonDataType,

        beforeSend: function () {
            console.log('beforeSend CheckIfAddLocShouldHide');
            // no loading image needed
        }
    }).done(function (data) {
        console.log('done CheckIfAddLocShouldHide');
        if (data.shouldShow === 'Yes') {
            console.log('show CheckIfAddLocShouldHide');
            $("#displayAddLoc").show('slow');
        } else if (data.shouldShow === 'No') {
            console.log('hide  CheckIfAddLocShouldHide');
            $("#displayAddLoc").hide(1000);
        }
    }).fail(function (data) {
        console.log('CheckIfAddLocShouldHide failed!!! ');
    });
}

function BuildPreRegPrice(oEvent) {
    //permits a 'preReg' pricing scheme to handle
    //computation of discounts and addl locations prior
    //to stepping to confirmation.

    var $form = $("#BuildPrice");

    oEvent.preventDefault();
    $("#ProgressDialogBS").modal('show');
    $.ajax({
        url: '/cart/buildPrice',
        type: "POST",
        data: $form.serialize(),
        success: function (data) {
            $("#mode_" + data.orderRowID).prop('checked', true);

        },
        error: function () {

        },
        complete: function () {
            $("#ProgressDialogBS").modal('hide');
        }
    });
}


function SetCartState() {
    switch (whichStep) {
        case "Step0":
            console.log("Step0");
            $("#connectionsCount").val(0);
            $('#collectAdditionalLocation').html('');
            $("#confirmationTab").hide();
            $("#signUpTab").hide();
            $("#contactInfoTab").hide();
            $('#AddToCart').attr({ disabled: false, value: 'SignUp' });
            $('#AddToCart1').attr({ disabled: false, value: 'SignUp' });

            break;

        case "Step1":
            console.log("Step1");

            $("#confirmationTab").hide();
            $("#signUpTab").hide();
            $("#contactInfoTab").show();
            break;

        case "Step2":
            $("#signUpTab").hide();
            $("#signUp").hide();

            $("#confirmationTab").tab('show');
            $("#confirmationTab").addClass('active');
            $("#confirmation").addClass('active');
            $("#confirmation").show();
            //$.get("/cart/checkoutConfirm/" + orderRowID)
            //    .success(function (dataConfirm) {
            //        $('#confirmation').html(dataConfirm);
            //    })
            //.done($("#confirmation").show())
            $("#confirmationTab").trigger('click');

            console.log("Step2");

            break;

        case "Registered":
            console.log("state is registered");
            $("#confirmationTab a").text('Order Summary');
            $("#signUpTab a").text('Connection Info');

            $.get("/cart/checkoutConfirm/" + orderRowID)
                .success(function (dataConfirm) {
                    $('#confirmation').html(dataConfirm);
                });
            $('#AddToCart').hide();
            $('#AddToCart1').hide();
            break;
    }
}
