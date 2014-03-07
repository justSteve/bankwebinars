$('input[name=mode]:eq(0)').attr('checked', 'checked');
var discount = '';
var wait4Emails = '';
var orderRowID = 0;
var CheckoutInProcess = false;
var isUserLogged = false;
var whichStep = "Step0";


function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
};
function showHtmlPopup(url) {
    window.open(url, null, "width=800, height=600,toolbar=1,scrollbars=1,location=1,statusbar=1,menubar=1,resizable=1");
    //'toolbar=1,scrollbars=0,location=1,statusbar=1,menubar=1,resizable=1,width=100,height=100');"
}

$(document).ready(function () {
    SetCartState();

    $('[id^="AddToCart"]').on('click', function () {
        $("#frmSignup2 [name='mode']").val($('input[name=mode]:checked', '#ModeOptions').val());
        var emails2Add = "";
        $('[name^="Email"]').each(function() {
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
                    jslogger.event({ signup: { from: "EndUser Checkout" } });
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
                    jslogger.log({ exception: { name: "SignupFail", message: "The signUpForm submission failed." } });
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


    // Handle form submit ...
    $("#_CreateUserForm").on('submit', function (event) {
        event.preventDefault();
        var cForm = $(this);
        $("#CreateUserHeader").text("Submitting Entry");
        if ($("#FullNameShipping").val() === null || $("#FullNameShipping").val() === "") $("#FullNameShipping").val($("#FullName").val());
        if ($("#ShippingFirstName").val() === null || $("#ShippingFirstName").val() === "") $("#ShippingFirstName").val($("#FirstName").val());
        if ($("#ShippingLastName").val() === null || $("#ShippingLastName").val() === "") $("#ShippingLastName").val($("#LastName").val());
        if ($("#ShippingCity").val() === null || $("#ShippingCity").val() === "") $("#ShippingCity").val($("#City").val());
        if ($("#ShippingAddress").val() === null || $("#ShippingAddress").val() === "") $("#").val($("#Address").val());
        if ($("#ShippingAddress2").val() === null || $("#ShippingAddress2").val() === "") $("#").val($("#Address2").val());
        if ($("#ShippingState").val() === null || $("#ShippingState").val() === "") $("#ShippingState").val($("#State").val());
        if ($("#ShippingZip").val() === null || $("#ShippingZip").val() === "") $("#ShippingZip").val($("#Zip").val());

        $("#ProgressDialogBS").modal('show');

        var data = cForm.serialize();
        $.post(
            cForm.attr("action"), data, function (result, status) {
                if (result.Success) {
                    $("#displayLogin").modal("hide");
                    //Show user name
                    $.ajax({
                        url: "/Account/ShowLoginStatus",
                        cache: false,
                        success: function (html) {
                            $("#showLoggedUser").html(html);
                        }
                    });
                } else {
                    $('.loginErrors').html("Invalid Submission. Try again?");
                }

                $("#ProgressDialogBS").modal('hide');
            }, "json");

        return false;
    });

});


function SetCartState() {
    switch (whichStep) {
        case "Step0":
            console.log("Step0");
            $("#connectionsCount").val(0);
            $('#collectAdditionalLocations').html('');
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
