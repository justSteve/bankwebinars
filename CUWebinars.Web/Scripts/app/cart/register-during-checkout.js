var registerDuringCheckout = {};


registerDuringCheckout.initialize = function (orderId, webinarId, orderRowId, shippingAddressRequired, callback) {

    Rollbar.info({ 'reg-during-check': { orderId: orderId, webinarId: webinarId, orderRowId: orderRowId, shippingAddressRequired: shippingAddressRequired } });

    cartStateManager.setCancelOrderForm($('#cancelOrder'));
    cartStateManager.setConfirmOrderForm($('#confirmOrder'));

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
    regUserStateManager.setIsShippindAddressRequired(shippingAddressRequired);

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

    $('#ResetPasswordForm').on('submit', function(e) {

        var hiddenInput = $('#ResetPassEmail');

        e.preventDefault();

        var jsonUrl = $(this).attr('action');
        var jsonPayload = { email: hiddenInput.val() };

        $.ajax({
            type: 'POST',
            contentType: RegistrationInCart.Constants.JsonContentType,
            cache: false,
            url: jsonUrl,
            dataType: RegistrationInCart.Constants.JsonDataType,
            data: JSON.stringify(jsonPayload),
            beforeSend: function() {
                $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;&nbsp;Working...</span>');
            }
        }).done(function(data) {
            if (data.Result === 'Success') {
                $('#labelEmail').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Reset instructions have been sent. Follow instructions and come back here to log in...</span>');
            } else {
                if (data['Invalid']) {
                    Rollbar.error("Invalid email address.");
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;We do not have a record of that email address. Are you sure you typed it corretly?</span>');
                } else {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Error. Please retry...</span>');
                }
            }
        }).always(function(data) {
            
        });

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
                data: { email: email, disregardInstitutionDomain: regUserStateManager.getDisregardIntitutionDomain(), orderId: orderId },
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
                } else if (data.error === 'Fail') {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;An error has occurred at the server. Please contact the administrator.</span>');
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
        var beigeFormArea = signUpFormContainer.find('div.well');

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
                
                beigeFormArea.height(500);
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
                            headers: headers,
                            beforeSend: function() {
                                $('#SignUpFormContainer > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
                            }
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

                                
                                $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                    //$('#confirmation').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>');
                                    completeOrder(userId, orderRowId, webinarId);
                                });

                                $('#Canceller').on('click', function(e) {
                                    cancelOrder(orderId, webinarId);
                                });

                                hookUpApplyDiscountLogic($('#SubmitDiscountCode'));
                                hookUpChangeTypeLogic($('#RegType'));
                                hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

                                if (shippingAddressRequired && notificationsTesting === 'false') {
                                    hookUpModal($('#UserDetailsModal'));
                                }

                                //  Now that we are on the 3rd tab, remove the 2nd tab else we'll have some fields with identical id's on both tabs (edit user fields)
                                $('#_CreateUserFromCartForm').remove();

                                $('#loadingSpinner').remove();
                                
                                beigeFormArea.height($('#confirmation').height() + 30);
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
    });

    $('#frmSignIn').on('submit', function (event) {

        event.preventDefault();

        var signInForm = $(this);

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());
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
                                            e.preventDefault();
                                            callback();
                                            $('#confirmOrder').submit();
                                        });

                                        $('#Canceller').on('click', function (e) {
                                            e.preventDefault();
                                            callback();
                                            var cancelOrderForm = cartStateManager.getCancelOrderForm();
                                            cancelOrderForm.submit();
                                        });

                                        hookUpApplyDiscountLogic($('#SubmitDiscountCode'));
                                        hookUpChangeTypeLogic($('#RegType'));
                                        hookUpEditUserLogic($('#editUserDetails'), shippingAddressRequired);

                                        if (shippingAddressRequired && notificationsTesting === 'false') {
                                            hookUpModal($('#UserDetailsModal'));
                                        }

                                        //  Now that we are on the 3rd tab, remove the 2nd tab else we'll have some fields with identical id's on both tabs (edit user fields)
                                        $('#_CreateUserFromCartForm').remove();

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

function completeOrder(userId, orderRowId, webinarId) {
    
    var cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setConfirmOrderForm($('#confirmOrder'));

    $('#ConfirmRegistrationBillMe').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

    var confirmOrderForm = cartStateManager.getConfirmOrderForm();

    confirmOrderForm.on('submit', function (e) {

        console.log('submitting ConfirmOrder');

        e.preventDefault();

        var self = $(this);
        self.find('input[name="id"]').val(orderRowId);

        var data = $(this).serialize();

        $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');

        $.post(self.attr('action'), data, function (result, status) {
            if (result.Result === 'Success') {
                cartStateManager.setOrderRowId(result.OrderRowID);

                $('#orderDetails').empty();
                $('#orderDetails').append(result.Msg);

                $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');
                
                $('#ConfirmModal').modal('show');

                $('#finalLoadingSpinner').remove();

            } else {
                $('.signupErrors').html('Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! ');
            }

            $('#ConfirmRegistrationBillMe').removeAttr('disabled');

        }, 'json');
        
        $('#ConfirmModal').on('hidden', function (e) {

            var utilities = new Common.Utilities();
            console.log('/webinar/details/' + webinarId);
            utilities.goToUrl('/webinar/details/' + webinarId);

        });
    });

    confirmOrderForm.submit();
    confirmOrderForm.off('submit');
}

function cancelOrder(orderId, webinarId) {

    var cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setCancelOrderForm($('#cancelOrder'));

    var cancelOrderForm = cartStateManager.getCancelOrderForm();

    $('#cancelModalOrderId').val(orderId);
    var data = cancelOrderForm.serialize();
    
    $('#cancelRegistration').on('click', function (e) {
        e.preventDefault();

        // disable button while operation in progress
        $('#cancelRegistration').attr('disabled', 'disabled');

        $.post(cancelOrderForm.attr('action'), data, function (response, status, xhr) {
            if (response.success) {
                //appInsights.trackEvent("Suceeded in canceling order");

                var utilities = new Common.Utilities();
                console.log('/webinar/details/' + webinarId);
                utilities.goToUrl('/webinar/details/' + webinarId);

            } else {

                //appInsights.trackEvent("Cancel Order Failure");
                $('.signupErrors').html('Invalid Data. Try again?');
                $('#ConfirmModal').modal('hide');
            }

            // enable button again upon ending operation.
            $('#cancelRegistration').removeAttr('disabled');

        }, 'json');

        // unbind event so we don't get them building up each time the user clicks the Cancel Registration button.
        this.off('click');
        $('#rtn').off('click');
    });

    $('#rtn').on('click', function (e) {
        e.preventDefault();
        $('#CancelModal').modal('hide');

        this.off('click');
        $('#cancelRegistration').off('click');
    });

    $('#CancelModal').modal('show');
}

function hookUpModal(modalForm) {

    var modalFormOptions = {
        keyboard: true,
        backdrop: 'static',
        show: true,
    };

    modalForm.modal(modalFormOptions);
}

function hookUpEditUserLogic(button, isShippindAddressRequired) {

    registerDuringCheckout.shippingAddressRequired = isShippindAddressRequired;

    var modalForm = $('#UserDetailsModal');

    button.on('click', function(e) {

        e.preventDefault();

        hookUpModal(modalForm);
    });

    modalForm.on('shown', function (e) {

        setUiLayout(registerDuringCheckout.shippingAddressRequired);

        $('#saveChangesButton').on('click', function() {
            e.preventDefault();

            $('#userDetailsForm').submit();
        });

        $('#userDetailsForm').on('submit', function(e) {
            e.preventDefault();

            var url = $(this).attr('action');

            var payload = $(this).serialize();

            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                data: payload,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
            }).done(function (data) {
                if (data.Result === 'Success') {
                    modalForm.modal('hide');
                    var fullname = $('#RegisterFields_FirstName').val() + ' ' + $('#RegisterFields_LastName').val();
                    $('#userFullname').text(fullname);

                    var userNameInsuranceAndButton = $('#userDetailsSummed > p:nth-child(1)');
                    userNameInsuranceAndButton.empty();
                    userNameInsuranceAndButton.html(fullname + ' - ' + $('#RegisterFields_Institution').val() + '<br> ' + $('#RegisterFields_Email').val() + ' - <a id="editUserDetails" role="button" class="btn btn-mini" target="new"> Edit?</a>');

                    $('#editUserDetails').on('click', function (e) {

                        e.preventDefault();

                        hookUpModal(modalForm);
                    });

                } else if (!data.isSuccessful) {
                    formProcessor.lightUpValidationSummary('userDetailsValSummary', data);
                }
            });
        });

        $('#HideAddShippingAddressLink').on('click', function (e) {
            e.preventDefault();

            $(this).fadeOut('500', function() {
                $('#ShippingAddressContainer').fadeOut('500', function () {
                    $('#AddShippingAddressLink').fadeIn('500');
                });
            });

        });

        $('#AddShippingAddressLink').on('click', function (e) {
            e.preventDefault();

            $(this).fadeOut('500', function() {
                $('#ShippingAddressContainer').fadeIn('500', function() {
                    $('#HideAddShippingAddressLink').fadeIn('500');
                });
            } );
        });
        
        $('#passwordWrapper').remove();

        if ($('#RegisterFields_Password').length < 1) {
            $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields_Password" name="RegisterFields.Password" value="456rty^Y" />');
            $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields.ConfirmPassword" name="RegisterFields.ConfirmPassword" value="456rty^Y" />');
        }
    });

    modalForm.on('hidden', function (e) {
        $('#saveChangesButton').off('click');
        $('#userDetailsForm').off('submit');
    });
}

function setUiLayout(isShippindAddressRequired) {

    if (isShippindAddressRequired) {
        $('#ShippingAddressContainer').hide();
        $('#HideAddShippingAddressLink').hide();
        $('#linksToAddShippingFields').show();
        $('#AddShippingAddressLink').show();
    } else {
        $('#ShippingAddressContainer, #linksToAddShippingFields, #AddShippingAddressLink').hide();
    }
}

function hookUpChangeTypeLogic(dropDown, shippingAddressRequired) {

    var changeTypeConfirmModal = $('#changeTypeConfirmModal');
    var chosenRegTypeLabel = $('#chosenRegType');
    var position,
        typeChosenCurrent,
        typeChosenPrevious,
        valOfTypeChosenPrevious,
        valOfTypeChosenCurrent;

        //  need to save state in the event that a Modal is displayed and Cancel is clicked on it.
        typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
        valOfTypeChosenPrevious = valOfTypeChosenCurrent = dropDown.val();

    $('#confirmTypeChange').on('click', function(e) {

        var url = '/Cart/RemoveAdditionalLocationsFromOrder';
        var payLoad = {
            idOrderRow: cartStateManager.getOrderRowId()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payLoad),
        }).done(function(data) {
            if (data.Result === 'Success') {
                $('#confirmation > div:nth-child(4) > div:nth-child(6)').empty();
                // TODO: Also adjust the price accordingly
                changeTypeConfirmModal.modal('hide');

                $('html, body').animate({ scrollTop: position.top }, 500);

                $('#addlocSpiel').text('To add additional locations for this order, please call 800-831-0678 ext 706 for immediate assistance').addClass('text-info');
            }
        });

        valOfTypeChosenCurrent = valOfTypeChosenPrevious = dropDown.val();
        typeChosenCurrent = typeChosenPrevious = $.trim($('#RegType option:selected').text());
        chosenRegTypeLabel.empty().text(typeChosenCurrent);
    });

    $('#cancelTypeChange').on('click', function(e) {
        changeTypeConfirmModal.modal('hide');
        chosenRegTypeLabel.text(typeChosenPrevious);
        dropDown.val(valOfTypeChosenPrevious);
    });
    
    dropDown.on('change', function (e) {

        e.preventDefault();

        valOfTypeChosenCurrent = $(this).val();

        var url = '/Cart/CheckIfAddLocShouldHide?optionID=' + valOfTypeChosenCurrent;

        $.ajax({
            type: 'GET',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
        }).done(function(data) {
            if (data.shouldShow === 'No') {

                //  First, check if there are currently any Additional Locations added to the order.
                if ($('#addlocSpiel').length > 0) {
                    var modalFormOptions = {
                        keyboard: true,
                        backdrop: 'static',
                        show: true,
                    };

                    position = $('#confirmation').offset();

                    changeTypeConfirmModal.modal(modalFormOptions);
                } else {
                    typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
                    chosenRegTypeLabel.empty().text(typeChosenCurrent);
                    valOfTypeChosenPrevious = valOfTypeChosenCurrent;
                }
            } else {
                typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
                chosenRegTypeLabel.empty().text(typeChosenCurrent);
                valOfTypeChosenPrevious = valOfTypeChosenCurrent;
            }

            if (data.shippingDetailsRqrd === 'Yes') {
                registerDuringCheckout.shippingAddressRequired = true;
                //hookUpEditUserLogic(null, true);
                hookUpModal($('#UserDetailsModal'));
            } else {
                registerDuringCheckout.shippingAddressRequired = false;
            }
        });
    });
}

function hookUpApplyDiscountLogic(btn) {

    btn.on('click', function (e) {

        e.preventDefault();

        var url = '/cart/ApplyDiscountCode';
        var payload = { code: $('#DiscountCode').val() };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $('#confirmation').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
            }
        }).done(function (data) {
            //  do stuff here with discount.              
            var num = data.result;
        }).always(function (e) {
            $('#loadingSpinner').remove();
        });
    });
}

function showModalForShippingAddressDetails() {
    var modalShippingDetails = $('#UserDetailsModal'),
        modalFormOptionsOnPageLoad = {
            keyboard: true,
            backdrop: 'static',
            show: true,
        };

    modalShippingDetails.on('shown', function (e) {
        setUiLayout(true);
    });

    modalShippingDetails.modal(modalFormOptionsOnPageLoad);
}