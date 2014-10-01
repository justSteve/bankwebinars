var stateManager,
    utilities;

$(function () {

    //setup ajax error handling
    $.ajaxSetup({
        error: function (x, status, error) {
            if (x.status == 403) {
                alert('Sorry, your session has expired. Please login again to continue');
                window.location.href = '/Account/Login';
            }
            else {
                alert('An error occurred: ' + status + 'nError: ' + error);
            }
        }
    });

    utilities = new Common.Utilities();

    stateManager = new Registration.StateManager();
    stateManager.initializeState();

    var path = utilities.setPath();

    $('#RegisterFields_Email').bind('change keyup', function () {
        if ($(this).validate().checkForm()) {
            $('#TheSubmitButton').removeClass('button_disabled').attr('disabled', false);
        } else {
            $('#TheSubmitButton').addClass('button_disabled').attr('disabled', true);
        }
    });

    $('[name="RegisterFields.ConfirmPassword"]').on('focus', function (event) {
        $(this).next('span').removeAttr('class').attr('class', 'field-validation-valid');
        $(this).next('span span').remove();
    });

    $('body').on('click', 'input:button', (function (e, data) {

        if (e.currentTarget.value === 'Create New Account?') // called directly in the razor partial view
            return false;

        if (stateManager.getInputAction() === Registration.InputAction.EnterKeyPress)
            return false;

        stateManager.setInputAction(Registration.InputAction.ButtonClick);

        if (stateManager.getAction() === '') {
            $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There registration has encountered a problem. Please refresh the page and re-start the registration process or call Tech Support at 800-831-0678 ext. 706.</span>');

            return false;
        }

        var normalResetPasswordButton = $('#NormalResetPasswordButton');
        var clickedButton = e.currentTarget.name;

        //  Possible values: //
        // TheSubmit
        // findCityState
        // nonUSAddress
        // submitLogin
        // resetPass
        // YesUseAddress
        // EnterDiffAddress
        // NotInstitution

        switch (clickedButton) {
            case Registration.Button.SignInButton: stateManager.logIn(); break;
            case Registration.Button.TheSubmit:
                //console.log('ActionForTheSubmit = ' + stateManager.action);
                stateManager.submit(); break;
            case Registration.Button.nonUSAddressBtn: stateManager.nonUsAdddressInvoked(); break;
            case Registration.Button.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
            case Registration.Button.YesUseAddress: stateManager.useRegisteredAddress(); break;
            case Registration.Button.EnterDiffAddress: stateManager.enterDifferentAddress(); break;
            case Registration.Button.NotInstitution: stateManager.notInstitutionAddress(); break;
            default:
        }
    }));

    $('input').keypress(function (event) {

        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (normalResetPasswordButton.filter(':visible').length > 0)
            inputElementTriggered = 'NormalResetPasswordInput';

        if (event.which == 13) {

            if ($('#modalInstitution').filter(':visible').length > 0
                && inputElementTriggered !== Registration.Button.YesUseAddress
                && inputElementTriggered !== Registration.Button.EnterDiffAddress
                && inputElementTriggered !== Registration.Button.NotInstitution) {
                return false;
            }

            stateManager.setInputAction(Registration.InputAction.EnterKeyPress);

            switch (inputElementTriggered) {
                case 'Password':
                case 'Email':
                case Registration.Button.SignInButton:
                    stateManager.logIn();
                    break;
                case 'RegisterFields.Password':
                case 'RegisterFields.ConfirmPassword':
                case 'RegisterFields.Email':
                case 'getZip':
                case 'Password1':
                case 'Email1':
                case Registration.Button.TheSubmit:
                    //console.log('ActionForTheSubmit = ' + stateManager.action);
                    stateManager.submit(); break;
                case Registration.Button.nonUSAddressBtn: stateManager.nonUsAdddressInvoked(); break;
                case 'NormalResetPasswordInput':
                case 'NormalResetPasswordButton':
                    if ($('#EdgeCaseResetPasswordButton').data('clicked'))
                        $('#EdgeCaseResetPasswordButton').removeData('clicked');
                    $('#NormalResetPasswordButton').data('clicked', true);
                    $('form#ResetPasswordForm').submit();
                    break;
                case '#EdgeCaseResetPasswordButton':
                case Registration.Button.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
                case Registration.Button.YesUseAddress: stateManager.useRegisteredAddress(); break;
                case Registration.Button.EnterDiffAddress: stateManager.enterDifferentAddress(); break;
                case Registration.Button.NotInstitution: stateManager.notInstitutionAddress(); break;
                default:
                    if ($('#TheSubmitButton').val() === stateManager.getRegisterButtonText()) {
                        if ($(stateManager.getSameAsBillingCheckedFilter()).val()) {
                            stateManager.setShippingToBilling();
                        }
                        stateManager.setAction(Registration.Action.SubmitRegister);
                        stateManager.submit();
                    }
            }

            //console.log(stateManager.action);
            event.preventDefault();

        }
    });

    $('#CollapseShipping').on('shown', function () {
        if ($(stateManager.getSameAsBillingCheckedFilter()).val()) {
            stateManager.setShippingToBilling();
        }
    });


    $('form#checkEmail').submit(function (e) {

        e.preventDefault();

        var jsonUrl = '/Account/CheckEmail';
        var email = $('#RegisterFields_Email').val();

        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: Registration.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: Registration.Constants.JsonDataType,
                data: { email: email, disregardInstitutionDomain: stateManager.getDisregardInstitutionDomain() },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.success === 'foundExisting') {
                    stateManager.resetPasswordOrLoginView(email);

                } else if (data.success === 'foundInstitution') {
                    stateManager.foundInstitutionView(data, email);

                } else if (data.email === 'wasNotFound') {
                    stateManager.newPasswordView(email);
                }

            }).fail(function () {
                // failed request; give feedback to user
                $('#wrapEmail').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                stateManager.setInputAction(Registration.InputAction.None);
            });
        }
    });

    $('form#checkZip').submit(function () {

        var jsonUrl = '/Account/CheckZip';
        var zipCode = $('#ZipChecker').val();

        if (zipCode.length == 0) {
            $('#RegisterFields_Zip').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: Registration.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: Registration.Constants.JsonDataType,
                data: { Zip: zipCode },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                stateManager.zipCodeVerified(data, zipCode);
            }).fail(function () {
                // failed request; give feedback to user
                $('#wrapZip').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                stateManager.setInputAction(Registration.InputAction.None);
            });
        }
        return false;
    });

    $('form#frmSignIn').submit(function (e) {

        e.preventDefault();

        if (!$('#ReturnUrl').val())
            $('#ReturnUrl').val('/');

        var data = $(this).serialize();
        var url = $(this).attr('action');

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: Registration.Constants.JsonDataType,
            contentType: Registration.Constants.FormPostContentType,
            //headers: headers,
            beforeSend: function (xhr) {
                if ($('#labelEmail').is(':visible')) {
                    $('#labelEmail').html('<span class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Signing in...</span>');
                    $('#loginErrorSummary').empty();
                } else {
                    $('#loginMsgLabelWrap').show();
                    $('#loginMsgLabelWrap').html('<span class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Signing in...</span>');

                    var valSummary = $('#LoginValSummary');
                    valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                    var errorsList = valSummary.find('ul');
                    errorsList.empty();
                    errorsList.append('<li style="display:none"></li>');
                }
            }
        }).done(function (data) {
            if (data.result === 'LoggedIn') {
                window.location.href = path + '/Account/MyWebinars';
            } else if (data.data) {

                if ($('#labelEmail').is(':visible')) {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;Login error...</span>');
                    var valSummary = $('#loginErrorSummary');
                    valSummary.addClass('validation-summary-errors');
                    valSummary.append('Please address the following login errors: <ul></ul>');

                    var errorsList = valSummary.find('ul');
                    errorsList.empty();

                    for (var error in data.data) {
                        if (data.data.hasOwnProperty(error)) {
                            errorsList.append('<li>' + data.data[error] + '</li>');
                            console.log(data.data[error]);
                        }
                    }

                } else {
                    var valSummary = $('#LoginValSummary');
                    valSummary.removeClass('validation-summary-valid').addClass('validation-summary-errors');

                    var errorsList = valSummary.find('ul');
                    errorsList.empty();

                    for (var error in data.data) {
                        if (data.data.hasOwnProperty(error)) {
                            errorsList.append('<li>' + data.data[error] + '</li>');
                            console.log(data.data[error]);
                        }
                    }
                }
            }
        }).fail(function (data) {

        }).always(function (data) {
            $('#loginMsgLabelWrap').hide();
        });

    });

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

    $('#modalInstitution').on('hidden', function (e) {

        stateManager.setInputAction(Registration.InputAction.None);

        if ($('#wrapZip').is(':visible')) {
            stateManager.setAction(Registration.Action.CheckEmail);
        }

    });

    //  This handler was colliding with one by the same name in create-user.
    //  It is now invoked from the create-user.js script.
    $('#_CreateUserForm').on('submit', function (event) {
        event.preventDefault();

        var createUserForm = $(this);
        
        if ($('#FullNameShipping').val() === null || $('#FullNameShipping').val() === '') $('#FullNameShipping').val($('#FullName').val());
        if ($('#ShippingFirstName').val() === null || $('#ShippingFirstName').val() === '') $('#ShippingFirstName').val($('#FirstName'));
        if ($('#ShippingLastName').val() === null || $('#ShippingLastName').val() === '') $('#ShippingLastName').val($('#LastName'));
        if ($('#RegisterFields_ShippingAddress_City').val() === null || $('#RegisterFields_ShippingAddress_City').val() === '') $('#RegisterFields_ShippingAddress_City').val($('#RegisterFields_BillingAddress_City').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress').val($('#RegisterFields_BillingAddress_StreetAddress').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress2').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress2').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress2').val($('#RegisterFields_BillingAddress_StreetAddress').val());
        if ($('#RegisterFields_ShippingAddress_State').val() === null || $('#RegisterFields_ShippingAddress_State').val() === '') $('#RegisterFields_ShippingAddress_State').val($('#RegisterFields_BillingAddress_State').val());
        if ($('#RegisterFields_ShippingAddress_Zip').val() === null || $('#RegisterFields_ShippingAddress_Zip').val() === '') $('#RegisterFields_ShippingAddress_Zip').val($('#RegisterFields_BillingAddress_Zip').val());

        //$("#ProgressDialogBS").modal('show');

        var data = createUserForm.serialize();
        var url = createUserForm.attr('action');

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: data,
            beforeSend: function () {
                //console.log('beforeSend Register Details');
                // this is where we append a loading image
                $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function (data) {
            //alert('done: ');
            if (data.Result) {
                if (data.Result === 'Success') {
                    //console.log('success: ' + data.Result);
                    stateManager.setAction('');
                    $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                    location.assign(path + '/'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
                } else if (data.Result === 'Fail') {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
                    stateManager.setAction(Registration.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;&nbsp;&nbsp;' + data.data.Exception + '</span>');
            }

        }).fail(function (data) {
            //console.log('failed: ' + data);
        }).always(function () {
            stateManager.setInputAction(Registration.InputAction.None);
        });

        return false;
    });


});
