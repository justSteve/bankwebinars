var ActionForTheSubmit = "CheckEmail";

var wrapZip;
var theSubmitButton;
var emailInput;
var createUserForm;
var phoneBilling;
var fullNameShipping;
var streetAddressBilling;
var streetAddressBilling2;
var cityBilling;
var stateBilling;
var zipBilling;
var phoneShipping;
var shippingFirstName;
var shippingLastName;
var streetAddressShipping;
var streetAddressShipping2;
var cityShipping;
var stateShipping;
var zipShipping;
var fullName;
var firstName;
var lastName;


var pageObjects = {
    theSubmitButton: theSubmitButton || $('#TheSubmitButton'),
    emailInput: emailInput || $('#RegisterFields_Email'),
    createUserForm: createUserForm || $('#_CreateUserForm'),
    wrapEmail: wrapZip || $('#wrapEmail'),
    wrapZip: wrapZip || $('#wrapZip'),
    wrapPass: wrapZip || $('#wrapPass'),
    phoneBilling: phoneBilling || $('#RegisterFields_BillingAddress_Phone'),
    fullNameShipping: fullNameShipping || $('#FullNameShipping'),
    fullName: fullName || $("#FullName"),
    firstName: firstName || $("#FirstName"),
    lastName: lastName || $("#LastName"),
    streetAddressBilling: streetAddressBilling || $('#RegisterFields_BillingAddress_StreetAddress'),
    streetAddressBilling2: streetAddressBilling2 || $('#RegisterFields_BillingAddress_StreetAddress2'),
    cityBilling: cityBilling || $('#RegisterFields_BillingAddress_City'),
    stateBilling: stateBilling || $('#RegisterFields_BillingAddress_State'),
    zipBilling: zipBilling || $('#RegisterFields_BillingAddress_Zip'),
    phoneShipping: phoneShipping || $('#RegisterFields_ShippingAddress_Phone'),
    streetAddressShipping: streetAddressShipping || $('#RegisterFields_ShippingAddress_StreetAddress'),
    streetAddressShipping2: streetAddressShipping2 || $('#RegisterFields_ShippingAddress_StreetAddress2'),
    cityShipping: cityShipping || $('#RegisterFields_ShippingAddress_City'),
    stateShipping: stateShipping || $('#RegisterFields_ShippingAddress_State'),
    zipShipping: zipShipping || $('#RegisterFields_ShippingAddress_Zip'),
    shippingFirstName: shippingFirstName || $("#ShippingFirstName"),
    shippingLastName: shippingLastName || $("#ShippingLastName")
};

function checkAndSubmitEmail() {
    pageObjects.theSubmitButton.prop('value', 'Next...');
    if (pageObjects.emailInput.valid() == "1") {
        console.log(pageObjects.emailInput.valid());
        $('#emailAddress').val($("#checkEmail").val());
        $("#checkEmail").submit();
    }
}

function checkAndSubmitZip() {
    pageObjects.theSubmitButton.prop('value', 'Next...');
    console.log("checkAndSubmitZip");
    //$('#emailAddress').val($("#checkEmail").val());
    $("#checkZip").submit();
}


function submitCreateUserForm() {

    var valid = pageObjects.createUserForm.valid();

    console.log(valid);
    if (valid) {
        console.log("createUserForm submitted.");
        //pageObjects.createUserForm.submit();
        pageObjects.createUserForm.submit(function (e) {
            e.preventDefault();
            alert('createUserForm');
            if (ActionForTheSubmit != "SubmitLogin") {
                console.log("non-valid form");
                return false;
            }
            alert("is " + ActionForTheSubmit);

            if (pageObjects.createUserForm.valid() != "1") {
                console.log("non-valid form");
                return false;
            }
            var url = "/Account/Register";
            console.log("Submitting Register Details");

            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                data: $(this).serialize(),
                beforeSend: function () {
                    console.log("beforeSend Register Details");
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
                }
            }).done(function (data) {
                console.log("done: ");
                if (data.Status === 'Success') {
                    console.log("success: " + data.Status);
                    ActionForTheSubmit = "";
                    $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                    location.assign(path + '/Account/Login'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
                } else if (data.status === 'Fail') {
                    $('#labelEmail').html('<span class="label label-information">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
                }
            }).fail(function (data) {
                console.log("failed: " + data);
            });
        });
    }
}

function submitLogin() {
    //$("#Email").val($("#RegisterFields_Email").val());
    //$("#Password").val($("#Password1").val());
    //$("#frmSignin").submit();
    $('#Password').val($('#RegisterFields_Password').val());
    $('#Email').val($('#emailAddress').val());
    $("form#frmSignIn").submit();
}



function setShippingToBilling() {
    pageObjects.fullNameShipping.val(pageObjects.fullName.val());
    pageObjects.shippingFirstName.val(pageObjects.firstName.val());
    pageObjects.shippingLastName.val(pageObjects.lastName.val());
    pageObjects.cityShipping.val(pageObjects.cityBilling.val());
    pageObjects.stateShipping.val(pageObjects.stateBilling.val());
    pageObjects.streetAddressShipping.val(pageObjects.streetAddressBilling.val());
    pageObjects.streetAddressShipping2.val(pageObjects.streetAddressBilling2.val());
    pageObjects.zipShipping.val(pageObjects.zipBilling.val());
    $("#RegisterFields_ShippingAddress_Country").val($("#RegisterFields_BillingAddress_Country").val());
    pageObjects.phoneShipping.val(pageObjects.phoneBilling.val());
}

function getMainPath(pathToCheck) {

    if (pathToCheck.substr(pathToCheck.length - 1) === '/')
        return pathToCheck.substr(0, pathToCheck.length - 1);
    return pathToCheck;
}

// document.ready starts here
$(function () {
        //setup ajax error handling
        $.ajaxSetup({
            error: function (x, status, error) {
                if (x.status == 403) {
                    alert("Sorry, your session has expired. Please login again to continue");
                    window.location.href = "/Account/Login";
                }
                else {
                    alert("An error occurred: " + status + "nError: " + error);
                }
            }
        });
    

    var disregardIntitutionDomain = false;
    var indexOfHome = location.href.indexOf('Account');

    theSubmitButton = $('#TheSubmitButton');
    emailInput = $('#RegisterFields_Email');
    createUserForm = $('#_CreateUserForm');
    wrapZip = $('#wrapZip');

    phoneBilling = $('#RegisterFields_BillingAddress_Phone');
    fullNameShipping = $('#FullNameShipping');
    streetAddressBilling = $('#RegisterFields_BillingAddress_StreetAddress');
    streetAddressBilling2 = $('#RegisterFields_BillingAddress_StreetAddress2');
    cityBilling = $('#RegisterFields_BillingAddress_City');
    stateBilling = $('#RegisterFields_BillingAddress_State');
    zipBilling = $('#RegisterFields_BillingAddress_Zip');
    phoneShipping = $('#RegisterFields_ShippingAddress_Phone');
    fullNameShipping = $('#fullNameShipping');
    streetAddressShipping = $('#RegisterFields_ShippingAddress_StreetAddress');
    streetAddressShipping2 = $('#RegisterFields_ShippingAddress_StreetAddress2');
    cityShipping = $('#RegisterFields_ShippingAddress_City');
    stateShipping = $('#RegisterFields_ShippingAddress_State');
    zipShipping = $('#RegisterFields_ShippingAddress_Zip');
    shippingLastName = $("#ShippingLastName");
    shippingFirstName = $("#ShippingFirstName");
    fullName = $("#FullName");
    firstName = $("#FirstName");
    lastName = $("#LastName");


    var path = '';

    if (indexOfHome > -1)
        path = location.href.substr(0, location.href.indexOf('Account') - 1);
    else
        path = location.href;

    //  IE is a rubbish browser!
    if (path === '')
        path = $(location).attr('href');

    pageObjects.emailInput.bind('change keyup', function () {
        if ($(this).validate().checkForm()) {
            pageObjects.theSubmitButton.removeClass('button_disabled').attr('disabled', false);
        } else {
            pageObjects.theSubmitButton.addClass('button_disabled').attr('disabled', true);
        }
    });
    $('[name=TheSubmit]').prop('value', 'Next...');

    $("#collapseBilling").parent().hide();
    $("#collapseShipping").parent().hide();
    pageObjects.wrapZip.hide();
    $("#wrapPass").hide();
    $("#wrapReset").hide();
    $("#wrapNonUS").hide();
    $("#getFirstLast").hide();
    //$("[data-val-email]").keyup(function () { return false; });
    //$("[data-val-email]").blur(function () { return true; });

    $('input').keypress(function (event) {
        var enterOkClass = $(this).hasClass('enterSubmit');
        if (event.which == 13) {
            console.log(ActionForTheSubmit);
            event.preventDefault();
            if (ActionForTheSubmit === "CheckEmail") {
                //ActionForTheSubmit = "CheckZip";
                if (pageObjects.emailInput.valid() == "1") {
                    checkAndSubmitEmail();
                }
                return false;
            }
            if (ActionForTheSubmit === "CheckZip") {

                checkAndSubmitZip();

                return false;
            }
            if (ActionForTheSubmit === "GetPassword") {

                wrapZip.show();
                wrapPass.hide("slow");
                return false;
            }

            //if (ActionForTheSubmit === "SubmitRegister") {

            //    if (pageObjects.createUserForm.valid() == "1") {
            //        submitCreateUserForm();
            //    }
            //    return false;
            //}
            if (ActionForTheSubmit === "SubmitLogin") {

                $('#Password').val($('#Password1').val());
                $('#Email').val($('#Email1').val());
                $("form#frmSignIn").submit();

                return false;
            }
            if (!enterOkClass) {
                return false;
            }
        }
    });


    $("body").on('click', 'input:button', (function (e, data) {

        var normalResetPasswordButton = $('#NormalResetPasswordButton');
        var clickedButton = e.currentTarget.name;
        //possible values
        // TheSubmit
        // findCityState
        // nonUSAddress
        // submitLogin
        // resetPass
        // YesUseAddress
        // EnterDiffAddress
        // NotInstitution
        if (clickedButton === "TheSubmit") {
            console.log("ActionForTheSubmit = " + ActionForTheSubmit);
            if (ActionForTheSubmit == "CheckEmail") {
                console.log("CheckEmail hit");
                checkAndSubmitEmail();
            }
            if (ActionForTheSubmit == "CheckZip") {
                console.log("CheckZip hit");
                $('#ZipChecker').val($('#getZip').val());
                $("form#checkZip").submit();
            }

            if (ActionForTheSubmit == "SubmitRegister") {
                console.log("SubmitRegister hit");
                ActionForTheSubmit = "";
                if (pageObjects.createUserForm.valid() == "1") {
                    submitCreateUserForm();
                }
            }

            if (ActionForTheSubmit == "SubmitLogin") {
                console.log("submitLogin hit");
                $('#Password').val($('#Password1').val());
                $('#Email').val($('#Email1').val());
                $("form#frmSignIn").submit();
            }

            if (ActionForTheSubmit === "DisplayBillingAddressFields") {
                if (pageObjects.emailInput.valid() == "1") {
                    $("#collapseBilling").parent().show();
                    $("#collapseBilling").collapse('show');
                    $("#collapseShipping").parent().show();
                    $('#collapseEmail').collapse('toggle');


                    ActionForTheSubmit = "SubmitRegister";
                    pageObjects.theSubmitButton.prop('value', 'SubmitRegister');
                }
            }
        }


        if (clickedButton === "nonUSAddress") {
            pageObjects.wrapZip.hide("slow");
            console.log("nonUSAddress hit");
            $("#wrapNonUS").show("slow");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }


        if (clickedButton === "resetPass") {
            console.log("resetPass hit");
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $("form#ResetPasswordForm").submit();
        }

        if (clickedButton === "YesUseAddress") {
            console.log("YesUseAddress hit");
            pageObjects.wrapZip.hide("slow");
            $('#modalInstitution').modal("hide");
            $("#collapseBilling").parent().show();
            $("#collapseShipping").parent().show();
            $("#collapseBilling").collapse('show');
            $('#collapseEmail').collapse('toggle');
            $('#labelEmail').fadeOut(500, function () {
                $(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + pageObjects.emailInput.val() + ' will be used for your email address.</span>');
                $(this).fadeIn(500);
            });
            ActionForTheSubmit = "SubmitRegister";
            pageObjects.theSubmitButton.prop('value', 'SubmitRegister');
        }

        if (clickedButton === "EnterDiffAddress") {
            console.log("EnterDiffAddress hit");
            $('#modalInstitution').modal("hide");
            $('#wrapEmail').show("slow");
            pageObjects.wrapZip.hide("slow");

            //  Clear the billing and shipping addresses
            $('#collapseBilling input').val("");
            $('#collapseBilling textarea').val("Mailing address notes:");
            $('#collapseBilling select').val(0);

            $('#collapseShipping input:not("#sameAsBilling")').val("");
            $('#collapseShipping textarea').val("Mailing address notes:");
            $('#collapseShipping select').val(0);

            pageObjects.theSubmitButton.prop('value', 'Submit New Email...');
            ActionForTheSubmit = "DisplayBillingAddressFields";
        }

        if (clickedButton === "NotInstitution") {
            console.log("NotInstitution hit");
            $('#modalInstitution').modal("hide");
            $('#wrapEmail').show("slow");
            pageObjects.wrapZip.hide("slow");
            $('#labelEmail').html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            $("#RegisterFields_Institution").val("");
            pageObjects.streetAddressShipping.val("");
            pageObjects.cityShipping.val("");
            pageObjects.stateShipping.val("");
            pageObjects.zipShipping.val("");
            pageObjects.streetAddressBilling.val("");
            pageObjects.cityBilling.val("");
            pageObjects.cityBilling.val("");
            pageObjects.zipBilling.val("");
            //$('#collapseEmail').collapse('toggle');
            //$('#collapseBilling').collapse('toggle');
            pageObjects.theSubmitButton.prop('value', 'Next...');
            ActionForTheSubmit = "CheckEmail";
            //disregardIntitutionDomain = true;
        }
    }));


    $('#collapseShipping').on('shown', function () {
        if ($("#sameAsBilling:checked").val()) {
            setShippingToBilling();

        }
    });
    //var availableTags = [];
    ////http://stackoverflow.com/questions/5077409/what-does-autocomplete-request-server-response-look-like
    //$("#RegisterFields_Institution").autocomplete({
    //    source: function (request, response) {
    //        alert(request.term);
    //        $.ajax({
    //            type: 'GET',
    //            cache: false,
    //            url: '/Account/AutocompleteInstitution',
    //            data: {
    //                zip: "54636", term: request.term
    //            },
    //            //data: { zip: pageObjects.zipBilling.val()},
    //            //beforeSend: function () {
    //            //    // this is where we append a loading image
    //            //    $('#labelEmail').html('<div class="btn-warning style="width: 400px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</div>');
    //            //},
    //            success: function (data) {
    //                response($.map(data.geonames, function (item) {
    //                    return {
    //                        label: item.name + (item.adminName1 ? ", " + item.adminName1 : "") + ", " + item.countryName,
    //                        value: item.name
    //                    };
    //                }));
    //            }
    //        });
    //    },
    //    //source: '/Account/AutocompleteInstitution',
    //    //minLength: ,
    //    open: function (event, ui) {
    //        $(".ui-autocomplete").css("z-index", 1000);
    //    },
    //    select: function (event, ui) {
    //        $("#RegisterFields_Institution").val(ui.item.Work_Item);
    //        return false;
    //    }
    //});

    $("form#checkEmail").submit(function (e) {

        e.preventDefault();
        var jsonUrl = "/Account/CheckEmail";
        var email = pageObjects.emailInput.val();
        if (email.length === 0) {
            $("#RegisterFields_Email").focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: constants.JsonDataType,
                data: { email: email, disregardIntitutionDomain: disregardIntitutionDomain },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.email === "wasNotFound") {
                    pageObjects.wrapEmail.hide("fast");
                    pageObjects.wrapPass.show("fast");
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
                    ActionForTheSubmit = "GetPassword";
                }

                if (data.success === "foundInstitution") {
                    $('#wrapEmail').hide("fast");
                    $('#modalInstitution').modal("show");

                    $("#RegisterFields_Institution").val(data.Institution);
                    pageObjects.streetAddressShipping.val(data.Address);
                    pageObjects.cityShipping.val(data.City);
                    pageObjects.stateShipping.val(data.State);
                    pageObjects.zipShipping.val(data.Zip);
                    pageObjects.streetAddressBilling.val(data.Address);
                    pageObjects.cityBilling.val(data.City);
                    pageObjects.cityBilling.val(data.State);
                    pageObjects.zipBilling.val(data.Zip);
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf("@")) + '</b>&nbsp; domain has been identified.</span>');
                    $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
                } else if (data.success === "foundExisting") {
                    //TODO: Add a bit of javascript that will hijack an 'Enter' key and will fire the
                    // 'blur' method on the text input (id='Email1'). The intent is to fire the
                    // blur both on the actual blur (user clicks outside the text box or tabs off
                    // the text box) or on an 'Enter' key.
                    ActionForTheSubmit = "SubmitLogin";
                    $('#Email1').val(email);
                    $('#ResetPassEmail').val(email);
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
                    $('#wrapReset').show('slow');
                    $('#wrapEmail').hide("slow");
                }
            }).fail(function () {
                // failed request; give feedback to user
                $('#wrapEmail').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
    });

    $("form#checkZip").submit(function () {
        var jsonUrl = "/Account/CheckZip";
        var q = $("#ZipChecker").val();
        if (q.length == 0) {
            $("#RegisterFields_Zip").focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: constants.JsonDataType,
                data: { Zip: q },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.success === "true") {
                    $("#collapseBilling").parent().show();
                    $("#collapseShipping").parent().show();
                    $('#collapseEmail').collapse('toggle');
                    $('#collapseBilling').collapse('toggle');
                    //$('#collapseShipping').collapse('toggle');
                    $('#RegisterFields_BillingAddress_City').val(data.City);
                    pageObjects.stateBilling.val(data.State);
                    pageObjects.zipBilling.val(q);
                    $('#TimeZone').val(data.TimeZone);
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
                    pageObjects.theSubmitButton.prop('value', 'SubmitRegister');
                    ActionForTheSubmit = 'SubmitRegister';
                } else if (data.success === "false") {
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                } else if (data.success === "invalid format") {
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                }
            }).fail(function () {
                // failed request; give feedback to user
                pageObjects.wrapZip.html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
        return false;
    });

    //$('#RegisterFields_Institution').change(function () {
    //    alert("hit" + $('#RegisterFields_Institution').val());
    //    //  $.getJSON()
    //    $('#InstitutionChecker').val($('#RegisterFields_Institution').val());
    //    $("form#checkInstitution").submit();
    //});

    $('#FullName').blur(function () {
        var tempName = $('#FullName').val().split(' ');
        if (tempName.length == 2) {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
        } else {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
            $('#getFull').hide();
            $('#getFirstLast').show();
            $('#RegisterFields_LastName').focus();
        }
    });

    $("#sameAsBilling").change(function (e) {
        var thisCheck = $(this);

        //  Remove keyup handlers on billing input fields first time (and each thereafter - which don't matter) checkbox clicked.
        pageObjects.phoneBilling.off('keyup');
        pageObjects.streetAddressBilling.off('keyup');
        $('#RegisterFields_BillingAddress_StreetAddress2').off('keyup');
        $('#RegisterFields_BillingAddress_City').off('keyup');
        pageObjects.stateBilling.off('keyup');
        pageObjects.zipBilling.off('keyup');

        if (thisCheck.is(':checked')) {
            if ($("#sameAsBilling:checked").val()) {
                setShippingToBilling();
            } else {
                if (pageObjects.fullNameShipping.val() === null || pageObjects.fullNameShipping.val() === "") pageObjects.fullNameShipping.val(pageObjects.fullName.val());
                if (pageObjects.shippingFirstName.val() === null || pageObjects.shippingFirstName.val() === "") pageObjects.shippingFirstName.val(pageObjects.firstName.val());
                if (pageObjects.shippingLastName.val() === null || pageObjects.shippingLastName.val() === "") pageObjects.shippingLastName.val(pageObjects.lastName.val());
                if (pageObjects.cityShipping.val() === null || pageObjects.cityShipping.val() === "") pageObjects.cityShipping.val(pageObjects.cityBilling.val());
                if (pageObjects.streetAddressShipping.val() === null || pageObjects.streetAddressShipping.val() === "") pageObjects.streetAddressShipping.val(pageObjects.streetAddressBilling.val());
                if (pageObjects.streetAddressShipping2.val() === null || pageObjects.streetAddressShipping2.val() === "") pageObjects.streetAddressShipping2.val(pageObjects.streetAddressBilling2.val());
                if (pageObjects.stateShipping.val() === null || pageObjects.stateShipping.val() === "") pageObjects.stateShipping.val(pageObjects.stateBilling.val());
                if (pageObjects.zipShipping.val() === null || pageObjects.zipShipping.val() === "") pageObjects.zipShipping.val(pageObjects.zipBilling.val());
            }
        }
        $(this).validate().checkForm();
        return false;
    });


    $('#modalInstitution').on('hidden', function (e) {

        if (pageObjects.wrapZip.is(":visible")) {
            ActionForTheSubmit = 'CheckEmail';
        }

    });

    $('#TheSubmitButton').on('mouseenter', function () {
        if ($("#sameAsBilling:checked").val()) {
            setShippingToBilling();
        }
    });

});