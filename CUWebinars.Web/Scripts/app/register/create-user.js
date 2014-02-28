var ActionForTheSubmit = "CheckEmail";
var wrapZip = $('#wrapZip');

$('#modalInstitution').on('hidden', function (e) {

    var d = e.target.id;

    if (wrapZip.is(":visible")) {
        ActionForTheSubmit = 'CheckEmail';
    }

    // do something…
    console.log(65);
});

function checkAndSubmitEmail() {
    $('[name=TheSubmit]').prop('value', 'Next...');
    if ($('#RegisterFields_Email').valid() === 1) {
        console.log($('#RegisterFields_Email').valid());
        $('#emailAddress').val($("#checkEmail").val());
        $("#checkEmail").submit();
    }
}

function submitCreateUserForm() {
    
    if ($('#_CreateUserForm').valid() === 1) {
        console.log($('#_CreateUserForm').valid());
        $("#_CreateUserForm").submit();
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

function setModalTop() {

    //$('#modalInstitution').modal().css('margin-top:145px;');
}

function setShippingToBilling() {
    $("#FullNameShipping").val($("#FullName").val());
    $("#ShippingFirstName").val($("#FirstName").val());
    $("#ShippingLastName").val($("#LastName").val());
    $("#RegisterFields_ShippingAddress_City").val($("#RegisterFields_BillingAddress_City").val());
    $("#RegisterFields_ShippingAddress_StreetAddress").val($("#RegisterFields_BillingAddress_StreetAddress").val());
    $("#RegisterFields_ShippingAddress_StreetAddress2").val($("#RegisterFields_BillingAddress_StreetAddress2").val());
    $("#RegisterFields_ShippingAddress_State").val($("#RegisterFields_BillingAddress_State").val());
    $("#RegisterFields_ShippingAddress_Zip").val($("#RegisterFields_BillingAddress_Zip").val());
    $("#RegisterFields_ShippingAddress_Country").val($("#RegisterFields_BillingAddress_Country").val());
    $("#RegisterFields_ShippingAddress_Phone").val($("#RegisterFields_BillingAddress_Phone").val());
}

function getMainPath(pathToCheck) {

    if (pathToCheck.substr(pathToCheck.length - 1) === '/')
        return pathToCheck.substr(0, pathToCheck.length - 1);
    return pathToCheck;
}

// document.ready starts here
$(function() {

    var disregardIntitutionDomain = false;
    var indexOfHome = location.href.indexOf('Account');
    var path = '';

    if (indexOfHome > -1)
        path = location.href.substr(0, location.href.indexOf('Account') - 1);
    else
        path = location.href;

    //  IE is a rubbish browser!
    if (path === '')
        path = $(location).attr('href');

    $('#RegisterFields_Email').bind('change keyup', function() {
        if ($(this).validate().checkForm()) {
            $('[name=TheSubmit]').removeClass('button_disabled').attr('disabled', false);
        } else {
            $('[name=TheSubmit]').addClass('button_disabled').attr('disabled', true);
        }
    });

    // Turns out that I've contracted a cold/flu during the travel - got a little but not enough
    // progress with this form. If you'd like to see if you can advance the ball on it - feel free 
    // But I'm going back to bed - given how much sleep i've already had, i may log in another few hours...
    // not really sure.
    // 
    // Have I mentioned we have to support browsers back to IE6? Lots of banks still running that. 
    // Rather than beat our heads making the jquery version work (barring an asounding finding) I think
    // we can browser detect and for anything at IE7 and less, re-direct to a seperate page and
    // just give the flat list of fields that our first draft had. 


    //Because there are so many sub-tasks going on before the full form is ready to submit - 
    // and because any 'Enter' key will trigger the validation much too early (that's what was going 
    // on yesterday when the Billing pane opened with all fields showing red)....
    //
    //I've replaced the 'submit' input with a 'button' with name = TheSubmit -- I'll assign different 
    // strings to it to indicate what function it should trigger. Of course, now that it's a button and not
    // a submit the Enter key doesn't auto-fire. So I'm trapping keypress as per a few lines below this one.

    // 
    $('[name=TheSubmit]').prop('value', 'Next...');

    $("#collapseBilling").parent().hide();
    $("#collapseShipping").parent().hide();
    $("#wrapZip").hide();
    $("#wrapReset").hide();
    $("#wrapNonUS").hide();
    $("#getFirstLast").hide();
    //$("[data-val-email]").keyup(function () { return false; });
    //$("[data-val-email]").blur(function () { return true; });

    $('input').keypress(function(event) {
        var enterOkClass = $(this).attr('class');

        if (event.which == 13) {
            if (ActionForTheSubmit === "CheckEmail") {
                ActionForTheSubmit = "CheckZip";
                if ($('#RegisterFields_Email').valid() === 1) {
                    checkAndSubmitEmail();
                }
            }
            if (ActionForTheSubmit === "CheckZip") {
                ActionForTheSubmit = "SubmitRegister";
                
                if ($('#_CreateUserForm').valid() === 1) {
                    checkAndSubmitEmail();
                }
            }

            if (ActionForTheSubmit === "SubmitRegister") {

                if ($('#_CreateUserForm').valid() === 1) {
                    submitCreateUserForm();
                }
            }
            if (enterOkClass !== 'enterSubmit') {
                event.preventDefault();
                return false;
            }
        }
    });

    $("#RegisterFields_Email").validate({
        //TODO: this is an attempt to turn off real-time validation of the email address.
        onfocusout: true,
        onkeyup: false,
        onkeypress: false,
        onkeydown: false
    });

    $("body").on('click', 'input:button', (function(e, data) {
        var clickedButton = e.currentTarget.name;
        //possible values
        // findCityState
        // nonUSAddress
        // submitLogin
        // resetPass
        // YesUseAddress
        // EnterDiffAddress
        // NotInstitution
        if (clickedButton === "TheSubmit") {

            if (ActionForTheSubmit === "CheckEmail") {
                checkAndSubmitEmail();
            }

            if (ActionForTheSubmit === "SubmitRegister") {
                if ($('#_CreateUserForm').valid() === 1) {
                    submitCreateUserForm();
                }
            }

            if (ActionForTheSubmit === "DisplayBillingAddressFields") {
                if ($('#RegisterFields_Email').valid() === "1") {
                    $("#collapseBilling").parent().show();
                    $("#collapseBilling").collapse('show');
                    $("#collapseShipping").parent().show();
                    $('#collapseEmail').collapse('toggle');

                    ActionForTheSubmit = "SubmitRegister";
                    $('[name=TheSubmit]').prop('value', 'SubmitRegister');
                }
            }
        }
        if (clickedButton === "findCityState") {
            $('#ZipChecker').val($('#getZip').val());

            $("form#checkZip").submit();
        }
        if (clickedButton === "nonUSAddress") {

            $("#wrapZip").hide("slow");
            $("#wrapNonUS").show("slow");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }
        if (clickedButton === "submitLogin") {
            $('#Password').val($('#Password1').val());
            $('#Email').val($('#Email1').val());
            $("form#frmSignIn").submit();
        }
        if (clickedButton === "resetPass") {
            if ($('#NormalResetPasswordButton').data('clicked'))
                $('#NormalResetPasswordButton').removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $("form#ResetPasswordForm").submit();
        }
        if (clickedButton === "YesUseAddress") {
            $("#wrapZip").hide("slow");
            $('#modalInstitution').modal("hide");
            $("#collapseBilling").parent().show();
            $("#collapseShipping").parent().show();
            $("#collapseBilling").collapse('show');
            $('#collapseEmail').collapse('toggle');
            $('#labelEmail').fadeOut(500, function() {
                $(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + $('#RegisterFields_Email').val() + ' will be used for your email address.</span>');
                $(this).fadeIn(500);
            });
            ActionForTheSubmit = "SubmitRegister";

        }
        if (clickedButton === "EnterDiffAddress") {
            $('#modalInstitution').modal("hide");
            $('#wrapEmail').show("slow");
            $("#wrapZip").hide("slow");

            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $('[name=TheSubmit]').prop('value', 'Submit New Email...');
            ActionForTheSubmit = "DisplayBillingAddressFields";
        }
        if (clickedButton === "NotInstitution") {

            $('#modalInstitution').modal("hide");
            $('#wrapEmail').show("slow");
            $("#wrapZip").hide("slow");
            $('#labelEmail').html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            $("#RegisterFields_Institution").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_City").val("");
            $("#RegisterFields_ShippingAddress_State").val("");
            $("#RegisterFields_ShippingAddress_Zip").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_City").val("");
            $("#RegisterFields_BillingAddress_State").val("");
            $("#RegisterFields_BillingAddress_Zip").val("");
            //$('#collapseEmail').collapse('toggle');
            //$('#collapseBilling').collapse('toggle');
            $('[name=TheSubmit]').prop('value', 'Next...');
            ActionForTheSubmit = "CheckEmail";
            disregardIntitutionDomain = true;
        }
    }));

    $('#modalInstitution').on('shown', function() {
        setModalTop();
    });
    $('#collapseShipping').on('shown', function() {
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
    //            //data: { zip: $("#RegisterFields_BillingAddress_Zip").val()},
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

        console.log("Hit");
        var jsonUrl = "/Account/CheckEmail";
        var email = $('#RegisterFields_Email').val();
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
                beforeSend: function() {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function(data) {
                // successful request; do something with the data
                if (data.email === "wasNotFound") {
                    $('#wrapEmail').hide("fast");
                    $('#wrapZip').show("fast");
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
                }

                if (data.success === "foundInstitution") {
                    $('#wrapEmail').hide("fast");
                    $('#modalInstitution').modal("show");

                    $("#RegisterFields_Institution").val(data.Institution);
                    $("#RegisterFields_ShippingAddress_StreetAddress").val(data.Address);
                    $("#RegisterFields_ShippingAddress_City").val(data.City);
                    $("#RegisterFields_ShippingAddress_State").val(data.State);
                    $("#RegisterFields_ShippingAddress_Zip").val(data.Zip);
                    $("#RegisterFields_BillingAddress_StreetAddress").val(data.Address);
                    $("#RegisterFields_BillingAddress_City").val(data.City);
                    $("#RegisterFields_BillingAddress_State").val(data.State);
                    $("#RegisterFields_BillingAddress_Zip").val(data.Zip);
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf("@")) + '</b>&nbsp; domain has been identified.</span>');
                    $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
                } else if (data.success === "foundExisting") {
                    //TODO: Add a bit of javascript that will hijack an 'Enter' key and will fire the
                    // 'blur' method on the text input (id='Email1'). The intent is to fire the
                    // blur both on the actual blur (user clicks outside the text box or tabs off
                    // the text box) or on an 'Enter' key.
                    $('#Email1').val(email);
                    $('#ResetPassEmail').val(email);
                    $('#MailSentForm').hide();
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
                    $('#wrapReset').show('slow');
                    $('#wrapEmail').hide("slow");
                }
            }).fail(function() {
                // failed request; give feedback to user
                $('#wrapEmail').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
    });

    $("form#checkZip").submit(function() {
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
                beforeSend: function() {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function(data) {
                // successful request; do something with the data
                if (data.success === "true") {
                    $("#collapseBilling").parent().show();
                    $("#collapseShipping").parent().show();
                    $('#collapseEmail').collapse('toggle');
                    $('#collapseBilling').collapse('toggle');
                    $('#RegisterFields_BillingAddress_City').val(data.City);
                    $('#RegisterFields_BillingAddress_State').val(data.State);
                    $('#RegisterFields_BillingAddress_Zip').val(q);
                    $('#TimeZone').val(data.TimeZone);
                    $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
                    $('[name=TheSubmit]').prop('value', 'SubmitRegister');
                    ActionForTheSubmit = 'SubmitRegister';
                } else if (data.success === "false") {
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                } else if (data.success === "invalid format") {
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                }
            }).fail(function() {
                // failed request; give feedback to user
                $('#wrapZip').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
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

    $('#FullName').blur(function() {
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

    $("#sameAsBilling").change(function(e) {
        var thisCheck = $(this);
        if (thisCheck.is(':checked')) {
            if ($("#sameAsBilling:checked").val()) {
                setShippingToBilling();
            } else {
                if ($("#FullNameShipping").val() === null || $("#FullNameShipping").val() === "") $("#FullNameShipping").val($("#FullName").val());
                if ($("#ShippingFirstName").val() === null || $("#ShippingFirstName").val() === "") $("#ShippingFirstName").val($("#FirstName").val());
                if ($("#ShippingLastName").val() === null || $("#ShippingLastName").val() === "") $("#ShippingLastName").val($("#LastName").val());
                if ($("#RegisterFields_ShippingAddress_City").val() === null || $("#RegisterFields_ShippingAddress_City").val() === "") $("#RegisterFields_ShippingAddress_City").val($("#RegisterFields_BillingAddress_City").val());
                if ($("#RegisterFields_ShippingAddress_StreetAddress").val() === null || $("#RegisterFields_ShippingAddress_StreetAddress").val() === "") $("#RegisterFields_ShippingAddress_StreetAddress").val($("#RegisterFields_BillingAddress_StreetAddress").val());
                if ($("#RegisterFields_ShippingAddress_StreetAddress2").val() === null || $("#RegisterFields_ShippingAddress_StreetAddress2").val() === "") $("#RegisterFields_ShippingAddress_StreetAddress2").val($("#RegisterFields_BillingAddress_StreetAddress2").val());
                if ($("#RegisterFields_ShippingAddress_State").val() === null || $("#RegisterFields_ShippingAddress_State").val() === "") $("#RegisterFields_ShippingAddress_State").val($("#RegisterFields_BillingAddress_State").val());
                if ($("#RegisterFields_ShippingAddress_Zip").val() === null || $("#RegisterFields_ShippingAddress_Zip").val() === "") $("#RegisterFields_ShippingAddress_Zip").val($("#RegisterFields_BillingAddress_Zip").val());
            }
        }
        return false;
    });

    $("#_CreateUserForm").on('submit', function (e) {

        e.preventDefault();

        console.log("Submitting Register Details");
        var url = "/Account/Register";
        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: $(this).serialize(),
            beforeSend: function() {
                // this is where we append a loading image
                $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function(data) {
            console.log(data);
            if (data.Status === 'Success') {
                $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                location.assign(path + '/Account/Login'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
            } else if (data.status === 'Fail') {
                $('#labelEmail').html('<span class="label label-information">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
            }
        }).fail(function(data) {
            console.log(data);
        });
    });
});