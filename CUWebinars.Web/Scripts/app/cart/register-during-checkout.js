var regPageObject,
    regUserStateManager;


$(function() {

    var utilities = new Common.Utilities();

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

    regPageObject = new Registration.PageObject();
    regPageObject.initializeState();

    regUserStateManager = new Registration.StateManager(regPageObject);
    regUserStateManager.setAction(Registration.Action.CheckEmail); // starting off with CheckEmail action.

    var path = utilities.setPath();

    regPageObject.getEmailInput().bind('change keyup', function () {
        if ($(this).validate().checkForm()) {
            regPageObject.getTheSubmitButton().removeClass('button_disabled').attr('disabled', false);
        } else {
            regPageObject.getTheSubmitButton().addClass('button_disabled').attr('disabled', true);
        }
    });

    $('[name="RegisterFields.ConfirmPassword"]').on('focus', function (event) {
        $(this).next('span').removeAttr('class').attr('class', 'field-validation-valid');
        $(this).next('span span').remove();
    });

    $('body').on('click', 'input:button', (function (e, data) {

        if (e.currentTarget.value === 'Create New Account?') // called directly in the razor partial view
            return false;

        if (regUserStateManager.getInputAction() === Registration.InputAction.EnterKeyPress)
            return false;

        regUserStateManager.setInputAction(Registration.InputAction.ButtonClick);

        if (regUserStateManager.getAction() === '') {
            regPageObject.getLabelEmail().html('<span class="label label-important">&nbsp;&nbsp;There registration has encountered a problem. Please refresh the page and re-start the registration process or call Tech Support at 800-831-0678 ext. 706.</span>');

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
            case Registration.Button.SignInButton: regUserStateManager.logIn(); break;
            case Registration.Button.TheSubmit:
                //console.log('ActionForTheSubmit = ' + regUserStateManager.action);
                regUserStateManager.submit(); break;
            case Registration.Button.nonUSAddressBtn: regUserStateManager.nonUsAdddressInvoked(); break;
            case Registration.Button.ResetPass: regUserStateManager.resetPassword(normalResetPasswordButton); break;
            case Registration.Button.YesUseAddress: regUserStateManager.useRegisteredAddress(); break;
            case Registration.Button.EnterDiffAddress: regUserStateManager.enterDifferentAddress(); break;
            case Registration.Button.NotInstitution: regUserStateManager.notInstitutionAddress(); break;
            default:
        }
    }));

    $('input').keypress(function (event) {

        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (normalResetPasswordButton.filter(':visible').length > 0)
            inputElementTriggered = 'NormalResetPasswordInput';

        if (event.which == 13) {

            if (regPageObject.getModalInstitution().filter(':visible').length > 0
                && inputElementTriggered !== Registration.Button.YesUseAddress
                && inputElementTriggered !== Registration.Button.EnterDiffAddress
                && inputElementTriggered !== Registration.Button.NotInstitution) {
                return false;
            }

            regUserStateManager.setInputAction(Registration.InputAction.EnterKeyPress);

            switch (inputElementTriggered) {
                case 'Password':
                case 'Email':
                case Registration.Button.SignInButton:
                    regUserStateManager.logIn();
                    break;
                case 'RegisterFields.Password':
                case 'RegisterFields.ConfirmPassword':
                case 'RegisterFields.Email':
                case 'getZip':
                case 'Password1':
                case 'Email1':
                case Registration.Button.TheSubmit:
                    //console.log('ActionForTheSubmit = ' + regUserStateManager.action);
                    regUserStateManager.submit(); break;
                case Registration.Button.nonUSAddressBtn: regUserStateManager.nonUsAdddressInvoked(); break;
                case 'NormalResetPasswordInput':
                case 'NormalResetPasswordButton':
                    if ($('#EdgeCaseResetPasswordButton').data('clicked'))
                        $('#EdgeCaseResetPasswordButton').removeData('clicked');
                    $('#NormalResetPasswordButton').data('clicked', true);
                    $('form#ResetPasswordForm').submit();
                    break;
                case '#EdgeCaseResetPasswordButton':
                case Registration.Button.ResetPass: regUserStateManager.resetPassword(normalResetPasswordButton); break;
                case Registration.Button.YesUseAddress: regUserStateManager.useRegisteredAddress(); break;
                case Registration.Button.EnterDiffAddress: regUserStateManager.enterDifferentAddress(); break;
                case Registration.Button.NotInstitution: regUserStateManager.notInstitutionAddress(); break;
                default:
                    if (regPageObject.getTheSubmitButton().val() === regUserStateManager.getRegisterButtonText()) {
                        if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
                            regUserStateManager.setShippingToBilling();
                        }
                        regUserStateManager.setAction(Registration.Action.SubmitRegister);
                        regUserStateManager.submit();
                    }
            }

            //console.log(regUserStateManager.action);
            event.preventDefault();

        }
    });

    regPageObject.getCollapseShipping().on('shown', function () {
        if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });


    $('form#checkEmail').submit(function (e) {

        e.preventDefault();

        var jsonUrl = '/Account/CheckEmail';
        var email = regPageObject.getEmailInput().val();

        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: Registration.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: Registration.Constants.JsonDataType,
                data: { email: email, disregardIntitutionDomain: regUserStateManager.getDisregardIntitutionDomain() },
                beforeSend: function () {
                    // this is where we append a loading image
                    regPageObject.getLabelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.success === 'foundExisting') {
                    regUserStateManager.resetPasswordOrLoginView(email);

                } else if (data.success === 'foundInstitution') {
                    regUserStateManager.foundInstitutionView(data, email);

                } else if (data.email === 'wasNotFound') {
                    regUserStateManager.newPasswordView(email);
                }

            }).fail(function () {
                // failed request; give feedback to user
                regPageObject.getWrapEmail().html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                regUserStateManager.setInputAction(Registration.InputAction.None);
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
                    regPageObject.getLabelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                regUserStateManager.zipCodeVerified(data, zipCode);
            }).fail(function () {
                // failed request; give feedback to user
                regPageObject.getWrapZip().html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                regUserStateManager.setInputAction(Registration.InputAction.None);
            });
        }
        return false;
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

    regPageObject.getModalInstitution().on('hidden', function (e) {

        regUserStateManager.setInputAction(Registration.InputAction.None);

        if (regPageObject.getWrapZip().is(':visible')) {
            regUserStateManager.setAction(Registration.Action.CheckEmail);
        }

    });

    //  This handler was colliding with one by the same name in create-user.
    //  It is now invoked from the create-user.js script.
    $('#_CreateUserForm').on('submit', function (event) {
        event.preventDefault();

        var createUserForm = $(this);

        if (regPageObject.getFullNameShipping().val() === null || regPageObject.getFullNameShipping().val() === '') regPageObject.getFullNameShipping().val(regPageObject.getFullName().val());
        if (regPageObject.getShippingFirstName().val() === null || regPageObject.getShippingFirstName().val() === '') regPageObject.getShippingFirstName().val(regPageObject.getFirstName());
        if (regPageObject.getShippingLastName().val() === null || regPageObject.getShippingLastName().val() === '') regPageObject.getShippingLastName().val(regPageObject.getLastName());
        if (regPageObject.getCityShipping().val() === null || regPageObject.getCityShipping().val() === '') regPageObject.getCityShipping().val(regPageObject.getCityBilling().val());
        if (regPageObject.getStreetAddressShipping().val() === null || regPageObject.getStreetAddressShipping().val() === '') regPageObject.getStreetAddressShipping().val(regPageObject.getStreetAddressBilling().val());
        if (regPageObject.getStreetAddressShipping2().val() === null || regPageObject.getStreetAddressShipping2().val() === '') regPageObject.getStreetAddressShipping2().val(regPageObject.getStreetAddressBilling().val());
        if (regPageObject.getStateShipping().val() === null || regPageObject.getStateShipping().val() === '') regPageObject.getStateShipping().val(regPageObject.getStateBilling().val());
        if (regPageObject.getZipShipping().val() === null || regPageObject.getZipShipping().val() === '') regPageObject.getZipShipping().val(regPageObject.getZipBilling().val());

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
                regPageObject.getLabelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function (data) {
            //alert('done: ');
            if (data.Result) {
                if (data.Result === 'Success') {
                    //console.log('success: ' + data.Result);
                    regUserStateManager.setAction('');
                    regPageObject.getLabelEmail().html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                    location.assign(path + '/'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
                } else if (data.Result === 'Fail') {
                    regPageObject.getLabelEmail().html('<span class="label label-important">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
                    regUserStateManager.setAction(Registration.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                regPageObject.getLabelEmail().html('<span class="label label-important">&nbsp;&nbsp;&nbsp;&nbsp;' + data.data.Exception + '</span>');
            }

        }).fail(function (data) {
            //console.log('failed: ' + data);
        }).always(function () {
            regUserStateManager.setInputAction(Registration.InputAction.None);
        });

        return false;
    });


});
