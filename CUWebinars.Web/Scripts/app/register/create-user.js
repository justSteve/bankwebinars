function CheckAndSubmitEmail() {
    $('[name=TheSubmit]').prop('value', 'Next...');
    if ($('#RegisterFields_Email').valid() == "1") {
        console.log($('#RegisterFields_Email').valid());
        $("#checkEmail").submit();
    }
}

function submitLogin() {
    $("#Email").val($("#RegisterFields_Email").val());
    $("#Password").val($("#Password1").val());
    $("#frmSignin").submit();
}

function SetModalTop() {

    $('#modalInstitution').modal().css('position:absolute;top:145px;');
}

$(function () {

    $('#RegisterFields_Email').bind('change keyup', function () {
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
    var ActionForTheSubmit = "CheckEmail";
    $('[name=TheSubmit]').prop('value', 'Next...');

    $("#collapseBilling").parent().hide();
    $("#collapseShipping").parent().hide();
    $("#wrapZip").hide();
    $("#wrapReset").hide();
    $("#wrapNonUS").hide();
    $("#getFirstLast").hide();
    //$("[data-val-email]").keyup(function () { return false; });
    //$("[data-val-email]").blur(function () { return true; });

    $('input').keypress(function (event) {
        var enterOkClass = $(this).attr('class');

        if (event.which == 13) {
            if (ActionForTheSubmit == "CheckEmail") {
                ActionForTheSubmit = "CheckZip";
                if ($('#RegisterFields_Email').valid() == 1) {
                    CheckAndSubmitEmail();
                }
            }
            if (ActionForTheSubmit == "CheckZip") {
                ActionForTheSubmit = "SubmitRegister";
                if ($('#_CreateUserForm').valid() == 1) {
                    CheckAndSubmitEmail();
                }
            }

            if (ActionForTheSubmit == "SubmitRegister") {
                if ($('#_CreateUserForm').valid() == 1) {
                    CheckAndSubmitEmail();
                }
            }
            if (enterOkClass != 'enterSubmit') {
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

    $("body").on('click', 'input:button', (function (e, data) {
        var clickedButton = e.currentTarget.name;
        //possible values
        // findCityState
        // nonUSAddress
        // submitLogin
        // resetPass
        // YesUseAddress
        // EnterDiffAddress
        // NotInstitution
        if (clickedButton == "TheSubmit") {

            if (ActionForTheSubmit == "CheckEmail") {
                CheckAndSubmitEmail();
            }
        }
        if (clickedButton == "findCityState") {
            $('#ZipChecker').val($('#getZip').val());

            $("form#checkZip").submit();
        }
        if (clickedButton == "nonUSAddress") {

            $("#wrapZip").hide("slow");
            $("#wrapNonUS").show("slow");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }
        if (clickedButton == "submitLogin") {
            $('#Password').val($('#Password1').val());
            $('#Email').val($('#Email1').val());
            $("form#frmSignIn").submit();
        }
        if (clickedButton == "resetPass") {
            //
            $("form#ResetPasswordForm").submit();
        }
        if (clickedButton == "YesUseAddress") {
            $("#wrapZip").hide("slow");
            $('#modalInstitution').modal("hide");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }
        if (clickedButton == "EnterDiffAddress") {
            $("#wrapZip").hide("slow");
            $('#modalInstitution').modal("hide");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }
        if (clickedButton == "NotInstitution") {

            $('#modalInstitution').modal("hide");

            $("#wrapZip").hide("slow");
            $("#RegisterFields_Institution").val("");
            $("#RegisterFields_ShippingAddress_StreetAddress").val("");
            $("#RegisterFields_ShippingAddress_City").val("");
            $("#RegisterFields_ShippingAddress_State").val("");
            $("#RegisterFields_ShippingAddress_Zip").val("");
            $("#RegisterFields_BillingAddress_StreetAddress").val("");
            $("#RegisterFields_BillingAddress_City").val("");
            $("#RegisterFields_BillingAddress_State").val("");
            $("#RegisterFields_BillingAddress_Zip").val("");
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');
        }
    }));

    $('#modalInstitution').on('shown', function () {
        SetModalTop();
    });
    $('#collapseShipping').on('shown', function () {
        if ($("#sameAsBilling:checked").val()) {
            SetShippingToBilling();
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

    $("form#checkEmail").submit(function () {
        console.log("Hit");
        var jsonUrl = "/Account/CheckEmail";
        var email = $('#RegisterFields_Email').val();
        if (email.length === 0) {
            $("#RegisterFields_Email").focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: 'application/x-www-form-urlencoded',
                cache: false,
                url: jsonUrl,
                dataType: "json",
                data: { email: email },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<div class="btn-warning style="width: 400px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</div>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.email == "wasNotFound") {
                    $('#wrapEmail').hide("fast");
                    $('#wrapZip').show("fast");
                    $('#labelEmail').html('<div class="btn-success" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is recorded.</div>');
                }
                if (data.success == "foundInstitution") {
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
                    $('#labelEmail').html('<div class="btn-success" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is recorded.</div>');
                    $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
                }

                if (data.success == "foundExisting") {
                    //TODO: Add a bit of javascript that will hijack an 'Enter' key and will fire the
                    // 'blur' method on the text input (id='Email1'). The intent is to fire the
                    // blur both on the actual blur (user clicks outside the text box or tabs off
                    // the text box) or on an 'Enter' key.
                    $('#Email1').val(email);
                    $('#ResetPassEmail').val(email);
                    $('#labelEmail').html('<div class="btn-danger" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</div>');
                    $('#wrapReset').show('fast');
                    $('#wrapEmail').hide("fast");
                }
            }).fail(function () {
                // failed request; give feedback to user
                $('#wrapEmail').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
        return false;
    });

    $("form#checkZip").submit(function () {
        var jsonUrl = "/Account/CheckZip";
        var q = $("#ZipChecker").val();
        if (q.length == 0) {
            $("#RegisterFields_Zip").focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: 'application/x-www-form-urlencoded',
                cache: false,
                url: jsonUrl,
                dataType: "json",
                data: { Zip: q },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<div class="btn-warning style="width: 400px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</div>');
                }
            }).done(function (data) {
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
                        $('#labelEmail').html('<div class="btn-success" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</div>');
                    } else if (data.success === "false") { 
                        $('#labelEmail').html('<div class="btn-danger" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;Zipcode was not found!</div>');
                    } else if (data.success === "invalid format") {
                        $('#labelEmail').html('<div class="btn-danger" style="width: 400px; height: 20px;"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</div>');
                    }
                }).fail(function () {
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
        if (thisCheck.is(':checked')) {
            if ($("#sameAsBilling:checked").val()) {
                SetShippingToBilling();
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


    function SetShippingToBilling() {
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
});