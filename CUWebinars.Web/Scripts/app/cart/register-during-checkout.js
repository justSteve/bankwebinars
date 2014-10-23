var registerDuringCheckout = {};

registerDuringCheckout.initialize = function (orderId, webinarId, orderRowId, callback) {
    
    var regUserStateManager, userId;

    var utilities = new Common.Utilities();

    //setup ajax error handling
    $.ajaxSetup({
        error: function(x, status, error) {
            if (x.status == 403) {
                alert('Sorry, your session has expired. Please login again to continue');
                window.location.href = '/Account/Login';
            } else {
                alert('An error occurred: ' + status + 'nError: ' + error);
            }
        }
    });

    regUserStateManager = new RegistrationInCart.StateManager();
    regUserStateManager.initializeState();
    regUserStateManager.setAction(RegistrationInCart.Action.CheckEmail); // starting off with CheckEmail action.

    var path = utilities.setPathToBaseUrl();

    $('#RegisterFields_Email').bind('change keyup', function() {
        regUserStateManager.ensureFormValidatorParsed();
        if ($(this).valid() == true) {
            $('#TheSubmitButton').removeClass('button_disabled').attr('disabled', false);
        } else {
            $('#TheSubmitButton').addClass('button_disabled').attr('disabled', true);
        }
    });

    $('[name="RegisterFields.ConfirmPassword"]').on('focus', function(event) {
        $(this).next('span').removeAttr('class').attr('class', 'field-validation-valid');
        $(this).next('span span').remove();
    });

    $('body').on('click', 'input:button', (function(e, data) {

        if (e.currentTarget.value === 'Create New Account?') // called directly in the razor partial view
            return false;

        if (regUserStateManager.getInputAction() === RegistrationInCart.InputAction.EnterKeyPress)
            return false;

        regUserStateManager.setInputAction(RegistrationInCart.InputAction.ButtonClick);

        if (regUserStateManager.getAction() === '') {
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
        case RegistrationInCart.Button.SignInButton:
            regUserStateManager.logIn();
            break;
        case RegistrationInCart.Button.TheSubmit:
            //console.log('ActionForTheSubmit = ' + regUserStateManager.action);
            regUserStateManager.submit();
            break;
        case RegistrationInCart.Button.nonUSAddressBtn:
            regUserStateManager.nonUsAdddressInvoked();
            break;
        case RegistrationInCart.Button.ResetPass:
            regUserStateManager.resetPassword(normalResetPasswordButton);
            break;
        case RegistrationInCart.Button.YesUseAddress:
            regUserStateManager.useRegisteredAddress();
            break;
        case RegistrationInCart.Button.EnterDiffAddress:
            regUserStateManager.enterDifferentAddress();
            break;
        case RegistrationInCart.Button.NotInstitution:
            regUserStateManager.notInstitutionAddress();
            break;
        default:
        }
    }));

    $('input').keypress(function(event) {

        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (normalResetPasswordButton.filter(':visible').length > 0)
            inputElementTriggered = 'NormalResetPasswordInput';

        if (event.which == 13) {

            if ($('#modalInstitution').filter(':visible').length > 0
                && inputElementTriggered !== RegistrationInCart.Button.YesUseAddress
                && inputElementTriggered !== RegistrationInCart.Button.EnterDiffAddress
                && inputElementTriggered !== RegistrationInCart.Button.NotInstitution) {
                return false;
            }

            regUserStateManager.setInputAction(RegistrationInCart.InputAction.EnterKeyPress);

            switch (inputElementTriggered) {
            case 'Password':
            case 'Email':
            case RegistrationInCart.Button.SignInButton:
                regUserStateManager.logIn();
                break;
            case 'RegisterFields.Password':
            case 'RegisterFields.ConfirmPassword':
            case 'RegisterFields.Email':
            case 'getZip':
            case 'Password1':
            case 'Email1':
            case RegistrationInCart.Button.TheSubmit:
                //console.log('ActionForTheSubmit = ' + regUserStateManager.action);
                regUserStateManager.submit();
                break;
            case RegistrationInCart.Button.nonUSAddressBtn:
                regUserStateManager.nonUsAdddressInvoked();
                break;
            case 'NormalResetPasswordInput':
            case 'NormalResetPasswordButton':
                if ($('#EdgeCaseResetPasswordButton').data('clicked'))
                    $('#EdgeCaseResetPasswordButton').removeData('clicked');
                $('#NormalResetPasswordButton').data('clicked', true);
                $('form#ResetPasswordForm').submit();
                break;
            case '#EdgeCaseResetPasswordButton':
            case RegistrationInCart.Button.ResetPass:
                regUserStateManager.resetPassword(normalResetPasswordButton);
                break;
            case RegistrationInCart.Button.YesUseAddress:
                regUserStateManager.useRegisteredAddress();
                break;
            case RegistrationInCart.Button.EnterDiffAddress:
                regUserStateManager.enterDifferentAddress();
                break;
            case RegistrationInCart.Button.NotInstitution:
                regUserStateManager.notInstitutionAddress();
                break;
            default:
                if ($('#TheSubmitButton').val() === regUserStateManager.getRegisterButtonText()) {
                    if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
                        regUserStateManager.setShippingToBilling();
                    }
                    regUserStateManager.setAction(RegistrationInCart.Action.SubmitRegister);
                    regUserStateManager.submit();
                }
            }

            //console.log(regUserStateManager.action);
            event.preventDefault();

        }
    });

    $('#collapseShipping').on('shown', function() {
        if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });

    $('#TheSubmitButton').on('mouseenter', function () {
        if ($('#TheSubmitButton').val() === regUserStateManager.getRegisterButtonText() && $(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });


    $('form#checkEmail').submit(function(e) {

        e.preventDefault();

        var jsonUrl = '/Account/CheckEmail';
        var email = $('#RegisterFields_Email').val();

        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: { email: email, disregardInstitutionDomain: regUserStateManager.getDisregardIntitutionDomain() },
                beforeSend: function() {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {

                regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);

                // successful request; do something with the data
                if (data.success === 'foundExisting') {
                    regUserStateManager.resetPasswordOrLoginView(email, webinarId);

                } else if (data.success === 'foundInstitution') {
                    regUserStateManager.foundInstitutionView(data, email);

                } else if (data.email === 'wasNotFound') {
                    regUserStateManager.goToAddressFields(email);
                }

            }).fail(function() {
                // failed request; give feedback to user
                $('#wrapEmail').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function() {
                regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
            });
        }
    });

    $('form#checkZip').submit(function() {

        var jsonUrl = '/Account/CheckZip';
        var zipCode = $('#ZipChecker').val();

        if (zipCode.length == 0) {
            $('#RegisterFields_Zip').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: { Zip: zipCode },
                beforeSend: function() {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function(data) {
                // successful request; do something with the data
                regUserStateManager.zipCodeVerified(data, zipCode);
            }).fail(function() {
                // failed request; give feedback to user
                $('#wrapZip').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function() {
                regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
            });
        }
        return false;
    });

    $('#FullName').blur(function() {
        var tempName = $('#FullName').val().split(' ');
        if (tempName.length == 2) {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
        } else {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
            $('#getFullNameBilling').hide();
            $('#getFirstLast').show();
            $('#RegisterFields_LastName').focus();
        }
    });

    $('#modalInstitution').on('hidden', function(e) {

        regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);

        if ($('#wrapZip').is(':visible')) {
            regUserStateManager.setAction(RegistrationInCart.Action.CheckEmail);
        }

    });

    //  This handler was colliding with one by the same name in create-user.
    //  It is now invoked from the register-user-in-cart.js script.
    $('#_CreateUserFromCartForm').on('submit', function(event) {
        event.preventDefault();

        var createUserForm = $(this);

        if ($('#FullNameShipping').val() === null || $('#FullNameShipping').val() === '') $('#FullNameShipping').val($('#FullName').val());
        if ($('#ShippingFirstName').val() === null || $('#ShippingFirstName').val() === '') $('#ShippingFirstName').val($('#FirstName').val());
        if ($('#ShippingLastName').val() === null || $('#ShippingLastName').val() === '') $('#ShippingLastName').val($('#LastName').val());
        if ($('#RegisterFields_ShippingAddress_City').val() === null || $('#RegisterFields_ShippingAddress_City').val() === '') $('#RegisterFields_ShippingAddress_City').val($('#RegisterFields_BillingAddress_City').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress').val($('#RegisterFields_BillingAddress_StreetAddress').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress2').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress2').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress2').val($('#RegisterFields_BillingAddress_StreetAddress').val());
        if ($('#RegisterFields_ShippingAddress_State').val() === null || $('#RegisterFields_ShippingAddress_State').val() === '') $('#RegisterFields_ShippingAddress_State').val($('#RegisterFields_BillingAddress_State').val());
        if ($('#RegisterFields_ShippingAddress_Zip').val() === null || $('#RegisterFields_ShippingAddress_Zip').val() === '') $('#RegisterFields_ShippingAddress_Zip').val($('#RegisterFields_BillingAddress_Zip').val());

        //  First, sort out the Antiforgery token for json POST
        var token = $('[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var tabInputs = formProcessor.getApplicableInputs('contactInfo');
        var formInputs = formProcessor.getApplicableInputs('_CreateUserFromCartForm');
        var payloadFromTab = formProcessor.processInputs(tabInputs);
        var payloadFromForm = formProcessor.processInputs(formInputs);
        var payload = _.extend(payloadFromTab, payloadFromForm);
        delete (payload['undefined']); // this was the __RequestVerificationToken which we chucked in the headers. See immediately above.

        var url = createUserForm.attr('action');

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers,
            beforeSend: function() {
                //console.log('beforeSend Register Details');
                // this is where we append a loading image
                $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Registering new user...</span>');

                var valSummary = $('#valSummarySignUpForm');
                valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                var errorsList = valSummary.find('ul');
                errorsList.empty();
                errorsList.append('<li style="display:none"></li>');
            }
        }).done(function(data) {
            //alert('done: ');
            if (data.Result) {
                if (data.Result === 'Success') {
                    //console.log('success: ' + data.Result);
                    regUserStateManager.setAction('');

                    // This if guard may not be required
                    if (utilities.relativePathStartsWith(payload['returnUrl'])) { 
                        $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;&nbsp;You have successfully registered! On to check-out...</span>');

                        // The next POST updates the Order number with the newly create id of the WebUser
                        var updateOrderWithUserForm = $('#_UpdateOrderWithUserId');
                        var url = updateOrderWithUserForm.attr('action');
                        var token = updateOrderWithUserForm.find('input[name=__RequestVerificationToken]').val();
                        var headers = {};
                        headers['__RequestVerificationToken'] = token;

                        var payloadForUpdate = {
                            orderId: orderId,
                            userId: data.UserId
                        };

                        userId = data.UserId;

                        $.ajax({
                            type: 'POST',
                            contentType: constants.JsonContentType,
                            cache: false,
                            url: url,
                            dataType: constants.JsonDataType,
                            data: JSON.stringify(payloadForUpdate),
                            headers: headers
                        }).done(function(data) {

                            // Upon return, load the 3rd tab. And once loaded, create the MR UserAccount (but don't log the user in). 
                            $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function (response, status, xhr) {

                                //  MembershipReboot create user post. Needs its own headers/__RequestVerificationToken
                                var createUserAccountForm = $('#_CreateUserAccountForm');
                                var tokenMr = createUserAccountForm.find('input[name=__RequestVerificationToken]').val();
                                var headersMr = {};
                                headersMr['__RequestVerificationToken'] = tokenMr;
                                var urlMr = createUserAccountForm.attr('action');

                                $.ajax({
                                    type: 'POST',
                                    contentType: constants.JsonContentType,
                                    cache: false,
                                    url: urlMr,
                                    dataType: constants.JsonDataType,
                                    data: JSON.stringify(payload),
                                    headers: headersMr,
                                    beforeSend: function() {

                                    }
                                }).done(function(data) {
                                    //  do nothing.              
                                });

                                // 
                                $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                    $('#confirmation').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
                                    completeOrder(userId, orderRowId);
                                });
                            });
                        });

                        $('#confirmationTab a').tab('show');
                    } else {
                        $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;You have successfully registered!</span>');
                    }

                } else if (data.Result === 'Fail') {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
                    regUserStateManager.setAction(RegistrationInCart.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There were some problems with the form. Please refer to the items in red.</span>');

                formProcessor.lightUpValidationSummary('valSummarySignUpInCart', data);
            }

        }).fail(function(data) {
            //console.log('failed: ' + data);
        }).always(function() {
            regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
        });

        return false;
    });

    $('#frmSignIn').on('submit', function (event) {

        event.preventDefault();

        var signInForm = $(this);

        if ($('#loginEmail').val() === null || $('#loginEmail').val() === '') $('#loginEmail').val($('#Email1').val());
        if ($('#loginPassword').val() === null || $('#loginPassword').val() === '') $('#loginPassword').val($('#Password1').val());
        //if ($('#loginRememberMe').val() === null || $('#ShippingLastName').val() === '') $('#loginRememberMe').val($('#LastName'));



        var url = signInForm.attr('action');
        var data = signInForm.serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: data,
            beforeSend: function () {
                $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Logging you in...</span>');
                $('#loginErrorSummary').empty();
            }
        }).done(function (data) {
            if (data.result) {
                if (data.result === 'LoggedIn') {

                    var userId = data.UserId;
                    regUserStateManager.setAction('');
                    

                    $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;&nbsp;You have successfully logged in!</span>');

                    $('#loginContainer').empty().load('/Account/GetLoginPartial', function (e) {

                        $('#updateOrderWithUserIdWrapper').empty();
                        $('#updateOrderWithUserIdWrapper').load('/Cart/UpdateOrderWithUserIdForm', function (response, status, xhr) {

                            var updateOrderWithUserForm = $('#_UpdateOrderWithUserId');
                            var url = updateOrderWithUserForm.attr('action');

                            var token = updateOrderWithUserForm.find('input[name=__RequestVerificationToken]').val();
                            var headers = {};
                            headers['__RequestVerificationToken'] = token;

                            var payloadForUpdate = {
                                orderId: orderId,
                                userId: userId
                            };

                            $.ajax({
                                type: 'POST',
                                contentType: constants.JsonContentType,
                                cache: false,
                                url: url,
                                dataType: constants.JsonDataType,
                                data: JSON.stringify(payloadForUpdate),
                                headers: headers,
                            }).done(function (data) {

                                if (data.Result === 'Success') {
                                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function(response, status, xhr) {
                                        $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                            callback();
                                            $('#confirmOrder').submit();
                                        });
                                    });

                                    $('#confirmationTab a').tab('show');
                                }
                            });
                        });
                    });
                } 
            } else if (!data.isSuccessful) {
                $('#labelEmail').html('<span class="label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;There has been a problem with your log in attempt.</span>');

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
            }

        }).fail(function (data) {
            //console.log('failed: ' + data);
        }).always(function () {
            regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
        });

        return false;
    });

    $('#loadingSpinner').remove();
};

function completeOrder(userId, orderRowId) {

    var cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setCancelOrderForm($('#cancelOrder'));
    cartStateManager.setConfirmOrderForm($('#confirmOrder'));

    cartStateManager.getConfirmOrderForm().on('submit', function (e) {

        console.log('submitting ConfirmOrder');

        e.preventDefault();

        var self = $(this);
        self.find('input[name="id"]').val(orderRowId);

        var data = $(this).serialize();

        $.post(self.attr('action'), data, function (result, status) {
            if (result.success) {
                cartStateManager.setOrderRowId(result.orderRowID);

                $('#confirmResult').html(result.msg);
                $('#confirmRegistration').attr('href', 'javascript:location.reload();');

                cartStateManager.setCheckoutInProcess(false);


                logUserIn(userId);
                

                $('#ConfirmModal').modal('show');
            } else {
                $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
            }
        }, 'json');
        return false;

    });
    cartStateManager.getConfirmOrderForm().submit();
    cartStateManager.getConfirmOrderForm().off('submit');
}

function logUserIn(userId) {

    $('#_SignInAfterCheckout').on('submit', function (e) {

        e.preventDefault();

        var payload = { userId: userId };

        var token = $(this).find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var url = $(this).attr('action');

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers
        }).done(function (data) {

            $('#loginContainer').empty().load('/Account/GetLoginPartial', function() {
                $('#loadingSpinner').remove();
            });
        });
    });

    $('#_SignInAfterCheckout').submit();

    $('#_SignInAfterCheckout').off('submit');
}