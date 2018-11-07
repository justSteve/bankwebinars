var registerDuringCheckout = {};
registerDuringCheckout.institutionNames = {};

function getCookie(name) {
    var cookie = document.cookie;
    var prefix = name + "=";
    var begin = cookie.indexOf("; " + prefix);
    if (begin == -1) {
        begin = cookie.indexOf(prefix);
        if (begin != 0) return null;
    } else {
        begin += 2;
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
            end = cookie.length;
        }
    }
    return unescape(cookie.substring(begin + prefix.length, end));
}
registerDuringCheckout.initialize = function (orderId, webinarId, orderRowId, addressOptions, callback) {

    L.clientLogger.info('registerDuringCheckout.initialize', { orderId: orderId, webinarId: webinarId, orderRowId: orderRowId, shippingAddressRequired: shippingAddressRequired });

    cartStateManager.setCancelOrderForm($('#cancelOrder'));
    cartStateManager.setConfirmOrderForm($('#confirmOrder'));
    registerDuringCheckout.addressOptions = addressOptions;

    var regUserStateManager, userId;

    var utilities = new Common.Utilities();

    regUserStateManager = new RegistrationInCart.StateManager();
    regUserStateManager.initializeState();
    regUserStateManager.setAction(RegistrationInCart.Action.CheckEmail); // starting off with CheckEmail action.
    regUserStateManager.setisShippingAddressRequired(addressOptions['shippingAddressRequired']);

    $('#RegisterFields_Email').bind('change keyup', function () {
        regUserStateManager.ensureFormValidatorParsed();
        if ($(this).valid() == true) {
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

        //

        if (e.currentTarget.value === 'Create New Account?') // called directly in the razor partial view
            return false;

        if (regUserStateManager.getInputAction() === RegistrationInCart.InputAction.EnterKeyPress)
            return false;

        regUserStateManager.setInputAction(RegistrationInCart.InputAction.ButtonClick);

        if (regUserStateManager.getAction() === '') {

            $('#labelEmail').html('<span class="label label-important">&nbsp;Connection Error #893. Please refresh the page and re-try or contact info@ttstrain.com  or, for immediate assistant, call @tenant.TechPhone.</span>');
            L.clientLogger.error("Connection Error #893. Item clicked: ", { value: e.currentTarget.value });
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

    $('input').keypress(function (event) {

        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (normalResetPasswordButton.filter(':visible').length > 0)
            inputElementTriggered = 'NormalResetPasswordInput';

        // 13 is enter key
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

            event.preventDefault();

        }
    });

    $('#collapseShipping').on('shown', function () {
        if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });

    $('#TheSubmitButton').on('mouseenter', function () {
        if ($('#TheSubmitButton').val() === regUserStateManager.getRegisterButtonText() && $(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });

    $('#ResetPasswordForm').on('submit', function (e) {
        //handles submit and postback of /Account/ResetPasword
        var hiddenInput = $('#ResetPassEmail');

        e.preventDefault();

        var jsonUrl = $(this).attr('action');
        var jsonPayload = { email: hiddenInput.val() };

        var token = $(this).find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        $.ajax({
            type: 'POST',
            contentType: RegistrationInCart.Constants.JsonContentType,
            cache: false,
            url: jsonUrl,
            dataType: RegistrationInCart.Constants.JsonDataType,
            data: JSON.stringify(jsonPayload),
            headers: headers,
            beforeSend: function () {
                $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
                $('#EdgeCaseResetPasswordButton').hide();
            }
        }).done(function (data) {
            //console.log(data);
            if (data.Result === 'Success') {
                $('#labelEmail').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Reset instructions are on the way.</span>');
                $('#modalEnterPasswordResetLink').modal('show');

            } else {
                if (data['Invalid'] === 'UserNotVerified') {
                    L.clientLogger.error("#388 UserNotVerified ", { result: data && data.Result });

                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Connection Error #388. Email:  info@ttstrain.com  or, for immediate assistance, use our Help & Feedback button (lower right corner).</span>');
                } else if (data['Invalid'] === 'UnkownEmail') {
                    L.clientLogger.error("UnknownEmail", { result: data && data.Result });

                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;We do not have a record of that email address. Email:  info@ttstrain.com  or, for immediate assistance, use our Help & Feedback button (lower right corner).</span>');
                } else {
                    L.clientLogger.error("Unknown error #454", { result: data && data.Result });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Connection Error #454. Email:  info@ttstrain.com  or, for immediate assistance, use our Help & Feedback button (lower right corner).</span>');
                }
            }
        }).fail(commonFuncs.failCallBack);

    });

    $('form#checkEmail').submit(function (e) {

        e.preventDefault();

        var jsonUrl = '/Account/CheckEmail';
        var email = $('#RegisterFields_Email').val();
        var token = $(this).find('input[name="__RequestVerificationToken"]').val();
        var payload = {
            email: email, disregardInstitutionDomain: regUserStateManager.getDisregardIntitutionDomain(),
            orderId: orderId, __RequestVerificationToken: token
        };

        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'POST',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: payload,
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {

                console.log(data);

                regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
                if (data.orderRowId) {
                    cartStateManager.setOrderRowId(data.orderRowId);
                }
                if (data.orderId) {
                    cartStateManager.setOrderId(data.orderId);
                }
                var cookieVals = getCookie("OrderStart");

                if (cookieVals == null) {
                    var a = "holder";
                }
                else {
                    cookieVals = cookieVals.replace("OrderId=", "Email=" + email + "&OrderId=")
                    window.Cookies.set('OrderStart', cookieVals);
                }

                // successful request; do something with the data
                if (data.success === 'foundExisting') {
                    if (data.IsEntered) {
                        regUserStateManager.modalShowPaidRegistrationExists(email, webinarId);

                    } else {
                        if (data.isConfirmed !== 'true') {
                            regUserStateManager.resetPasswordOrLoginViewUnconfirmed(email, webinarId);
                        } else {
                            regUserStateManager.resetPasswordOrLoginView(email, webinarId);
                        }
                    }
                } else if (data.success === 'foundInstitution') {
                    regUserStateManager.foundInstitutionView(data, email);
                    L.clientLogger.info('foundInstitution for user: ', { email: email });
                    if (data.createUser === "true") {
                        createUserAccount(email);
                    }
                } else if (data.email === 'wasNotFound') {
                    regUserStateManager.goToAddressFields(email);
                    if (data.createUser === "true") {
                        createUserAccount(email);
                    }
                } else if (data.error === 'Fail') {

                    L.clientLogger.info("goToAddressFields 319", { data: data });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Connection Error #319. Email:  info@ttstrain.com  or, for immediate assistance, use our Help & Feedback button (lower right corner).</span>');
                } else if (data.error === 'Uncaught Ajax Error') {
                    L.clientLogger.error("Uncaught Ajax Error 343", { result: data || "data was falsey", payload: payload });


                    $('#labelEmail').html('<span class="label label-important">&nbsp;Uncaught Ajax Error 343</span>');
                }

            }).fail(commonFuncs.failCallBack)
                .always(function (data, status, message) {
                    regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);

                    if (data && data.responseText) {
                        if (status === 'error' && JSON.parse(data.responseText)['Message'] === 'Uncaught Ajax Error') {
                            L.clientLogger.error("HandleAjaxExceptionAttribute #458 ", { result: data });
                        }

                    }
                });
        }
    });

    $('form#checkZip').submit(function () {

        var jsonUrl = '/Account/CheckZip';
        var zipCode = $('#ZipChecker').val();

        if (zipCode.length === 0) {
            $('#RegisterFields_Zip').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: { Zip: zipCode },
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                L.clientLogger.info('Zipcode check result', { 'data': data || 'data was falsey' });
                regUserStateManager.zipCodeVerified(data, zipCode);
            }).fail(function () {
                alert('Error condition detected. Please visit us in our online chat application (lower right corner of this window) and reference error #777 for immediate assistance.');
                L.clientLogger.info('Zipcode check failed', { 'zipCode': zipCode });
            }).always(function () {
                regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
                L.clientLogger.info('Zipcode checked', { 'zipCode': zipCode });
            });
        }
        return false;
    });

    $('#FullName').blur(function () {
        var tempName = $('#FullName').val().split(' ');
        if (tempName.length == 2) {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
            $('#RegisterFields_Title').focus();
        } else {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#getFullNameBilling').hide();
            $('#getFirstLast').show();
            $('#RegisterFields_LastName').focus();
        }
    });

    $('#getFirstLast').on('blur', '#RegisterFields_FirstName, #RegisterFields_LastName', function () {
        var fullNameInput = $('#FullName');
        fullNameInput.val($('#RegisterFields_FirstName').val() + ' ' + $('#RegisterFields_LastName').val());
    });

    $('#modalInstitution').on('hidden', function (e) {

        regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);

        if ($('#wrapZip').is(':visible')) {
            regUserStateManager.setAction(RegistrationInCart.Action.CheckEmail);
        }

    });


    $('#_CreateUserFromCartForm').on('submit', function (event) {
        event.preventDefault();

        var createUserForm = $(this);
        var beigeFormArea = signUpFormContainer.find('div.well');

        var fullNameShipping = $('#FullNameShipping');
        var nameBilling = $('#RegisterFields_BillingAddress_Name');
        var nameShipping = $('#RegisterFields_ShippingAddress_Name');

        if (!fullNameShipping.val()) fullNameShipping.val($('#FullName').val());
        if (!nameBilling.val()) nameBilling.val($('#FullName').val());
        if (!nameShipping.val()) nameShipping.val($('#FullName').val());

        if ($('#RegisterFields_ShippingAddress_City').val() === null || $('#RegisterFields_ShippingAddress_City').val() === '') $('#RegisterFields_ShippingAddress_City').val($('#RegisterFields_BillingAddress_City').val());
        if ($('#RegisterFields_ShippingAddress_Phone').val() === null || $('#RegisterFields_ShippingAddress_Phone').val() === '') $('#RegisterFields_ShippingAddress_Phone').val($('#RegisterFields_BillingAddress_Phone').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress').val($('#RegisterFields_BillingAddress_StreetAddress').val());
        if ($('#RegisterFields_ShippingAddress_StreetAddress2').val() === null || $('#RegisterFields_ShippingAddress_StreetAddress2').val() === '') $('#RegisterFields_ShippingAddress_StreetAddress2').val($('#RegisterFields_BillingAddress_StreetAddress2').val());
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
        //Account/RegisterFromCart
        var url = createUserForm.attr('action');
        console.log(payload);
        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers,
            beforeSend: function () {
                // this is where we append a loading image
                $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Registering new user...</span>');

                var valSummary = $('#valSummarySignUpForm');
                valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                var errorsList = valSummary.find('ul');
                errorsList.empty();
                errorsList.append('<li style="display:none"></li>');

            }
        }).done(function (data) {

            console.log('#ConfirmRegistrationBillMe3 ini');
            if (data.Result) {
                if (data.Result === 'Success') {

                    regUserStateManager.setAction('');
                    registerDuringCheckout.emailOfNewUser = $.trim($('#RegisterFields_Email').val());

                    // This 'if' guard may not be required
                    if (utilities.relativePathStartsWith(payload['returnUrl'])) {
                        $('#labelEmail').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;You have successfully registered! On to check-out...</span>');

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
                            beforeSend: function () {
                                L.clientLogger.info("submitting " + url + " with: ", {
                                    orderId: orderId,
                                    userId: data.UserId
                                });
                                $('#SignUpFormContainer > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
                            }
                        }).done(function () {

                            console.log('#ConfirmRegistrationBillMe ini');

                            // Upon return, load the 3rd tab. And once loaded, 
                            //create the MR UserAccount (but don't log the user in). 

                            $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderId(), function (response, status, xhr) {

                                if (status == 'error') {
                                    L.clientLogger.error("Error at /cart/checkoutConfirm/", { rowid: cartStateManager.getOrderId() });
                                    $(this).html('<div class="text-error">There has been error at the server, please use our Help & Feedback button (lower right corner)  for immediate assistance.</div>');
                                    $('#loadingSpinner').remove();
                                    $('#confirmationTab a').tab('show');
                                } else {

                                    $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                        // This location is hit when new user created at order entry
                                        completeOrder(userId, orderRowId, webinarId, orderId);
                                        L.clientLogger.info("BigGreenBillMe from register-user-in-cart", { orderid: orderId });
                                    });

                                    // The 'TO PAY BY CREDIT CARD' button on 3rd tab
                                    $('#ConfirmRegistrationPayByCC').on('click', function (e) {
                                        L.clientLogger.info("ConfirmRegistrationPayByCC is clicked", { orderid: orderId });

                                        e.preventDefault();

                                        var url = '/Cart/PayCC/' + orderId;

                                        utilities.goToUrl(url);
                                    });

                                    $('#Canceller').on('click', function (e) {
                                        e.preventDefault();


                                        $('#CancelModal').modal('show');
                                        //cancelOrder(orderId, webinarId);
                                    });

                                    populateAdditionalLocationsOn3rdTab();
                                    setUpEditButtons();

                                    hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
                                    hookUpChangeTypeLogic($('#RegType'));
                                    hookUpAddLocsLogic($('#AddLocs'));
                                    hookUpEditUserLogic(null);

                                    if (registerDuringCheckout.addressOptions['shippingAddressRequired'] && !registerDuringCheckout.addressOptions['notificationsTesting'] && !registerDuringCheckout.addressOptions['addressVerified']) {
                                        hookUpModal($('#UserDetailsModal'));
                                    }

                                    //  Now that we are on the 3rd tab, remove the 2nd tab else we'll have some fields with identical id's on both tabs (edit user fields)
                                    $('#_CreateUserFromCartForm').remove();

                                    $('#loadingSpinner').remove();

                                    beigeFormArea.height($('#confirmation').height() + 30);
                                }
                            });

                        }).fail(commonFuncs.failCallBack);

                        $('#confirmationTab a').tab('show');

                    } else {
                        $('#labelEmail').html('<span class="label label-success">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;You have successfully registered!</span>');
                    }

                } else if (data.Result === 'Fail') {
                    L.clientLogger.error("Connection Error #332", { data: data });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Connection Error #332. Please try again. In case of persistent problem please contact us with our online chat (lower right corner of this page).</span>');
                    regUserStateManager.setAction(RegistrationInCart.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                L.clientLogger.error("Connection Error #332", { data: data });

                $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                formProcessor.lightUpValidationSummary('valSummarySignUpInCart', data);
            }

        }).fail(commonFuncs.failCallBack).always(function () {
            regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
        });
    });

    $('#frmSignIn').on('submit', function (event) {

        event.preventDefault();
        var valSummary = $('#loginErrorSummary');
        var labelEmail = $('#labelEmail');

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
                labelEmail.html('<span class="label label-info">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Logging you in...</span>');
                formProcessor.clearValidationSummary(valSummary);


            }
        }).done(function (data) {
            if (data.result) {
                //console.log(data);
                if (data.result === 'LoggedIn') {

                    var userId = data.UserId;
                    regUserStateManager.setAction('');

                    $('#labelEmail').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;You have successfully logged in!</span>').fadeOut(500, function () {
                        $(this).html('<span class="label label-info">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Processing order...</span>').fadeIn(100);
                    });

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
                                headers: headers
                            }).done(function (data) {

                                if (data.Result === 'Success') {

                                    $('#SignUpFormContainer > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');

                                    $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderId(), function (response, status, xhr) {
                                        console.log("hit checkoutConfirm")
                                        if (status == 'error') {
                                            L.clientLogger.error("Connection Error #106.", { 'data': data || 'data was falsey' });

                                            $(this).html('<div class="text-error">Connection Error #106. Email ' + globalConfig.TenantEmail + ' or use our Help & Feedback button (lower right corner)  for immediate assistance.</div>');
                                            $('#confirmationTab a').tab('show');
                                        } else {
                                            $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                                console.log("hit ConfirmRegistrationBillMe");
                                                e.preventDefault();
                                                callback();
                                                //TODO: centralize confirmOrderSubmission by OrderRegistration.stateManager.PlaceOrder();
                                                $('#confirmOrder').submit();
                                            });

                                            $('#Canceller').on('click', function (e) {
                                                e.preventDefault();
                                                callback();
                                                var cancelOrderForm = cartStateManager.getCancelOrderForm();
                                                cancelOrderForm.submit();
                                            });

                                            populateAdditionalLocationsOn3rdTab();
                                            setUpEditButtons();

                                            hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
                                            hookUpChangeTypeLogic($('#RegType'));
                                            hookUpAddLocsLogic($('#AddLocs'));
                                            hookUpEditUserLogic(null);

                                            if (registerDuringCheckout.addressOptions['shippingAddressRequired'] && !registerDuringCheckout.addressOptions['notificationsTesting'] && !registerDuringCheckout.addressOptions['addressVerified']) {
                                                hookUpModal($('#UserDetailsModal'));
                                            }

                                            //  Now that we are on the 3rd tab, remove the 2nd tab else we'll have some fields with identical id's on both tabs (edit user fields)
                                            $('#_CreateUserFromCartForm').remove();
                                        }
                                        $('#loadingSpinner').remove();
                                    });

                                    $('#confirmationTab a').tab('show');
                                }
                            }).fail(commonFuncs.failCallBack);
                        });
                    });
                }
            } else if (!data.isSuccessful) {
                //invalid Logins return here
                L.clientLogger.error("Error #641: ", { data: data && data });
                //console.log("here");
                valSummary.removeClass('validation-summary-valid').addClass('validation-summary-errors');
                var errorsList = valSummary.find('ul');
                errorsList.empty();

                for (var error in data.data) {
                    if (data.data.hasOwnProperty(error)) {
                        errorsList.append('<li>' + data.data[error] + '</li>');
                        if (data.data[error].toString().indexOf("password") < 1) {
                            L.clientLogger.error("Login error with something besides Invalid Password. ", { data: data.data[error] });
                        };
                    } else {
                        L.clientLogger.info("Invalid Password. ", { data: data.data[error] });
                    }
                }
            };
            labelEmail.remove();

        }).fail(function (jqXHR, textStatus, errorThrown) {

            if (jqXHR.statusCode().status == 403) {
                alert('Sorry, your session has expired. Please login again to continue');
                window.location.href = '/Account/Login';
            } else if (jqXHR.statusCode().status === 0 && errorThrown === '' && textStatus === 'error') {
                ;// do nothing
            } else {
                alert('An error occurred: ' + jqXHR.statusCode().status + ' nError: ' + jqXHR.statusCode().statusText);
            };

            L.clientLogger.error("Error #902: ", { data: xhr && xhr.data });

            labelEmail.remove();

        }).always(function () {
            regUserStateManager.setInputAction(RegistrationInCart.InputAction.None);
        });

        return false;
    });

};

registerDuringCheckout.gatherPricingData = function () {
    registerDuringCheckout.addLocsPrice = parseInt($('#totalAdLocsPrice').text().slice(1));
    registerDuringCheckout.totalPrice = parseInt($('#totalPrice').text().slice(1));
    registerDuringCheckout.totalDiscount = parseInt($('#totalDiscount').text().slice(1));
};

registerDuringCheckout.searchInstitution = _.debounce(function (query, process) {

    var searchTerm = $('#RegisterFields_Institution').val();

    $.ajax({
        type: 'POST',
        contentType: constants.FormPostContentType,
        cache: false,
        url: '/Account/GetInstitutionsByName',
        dataType: constants.JsonDataType,
        data: { institutionName: searchTerm },
        beforeSend: function () {
            registerDuringCheckout.institutionNames = null; // dereference whatever is currently in 'institutionNames'. 
        }
    }).done(function (data) {
        registerDuringCheckout.institutionNames = data.institutions;

        process(registerDuringCheckout.institutionNames);
    }).fail(commonFuncs.failCallBack);

}, 200);

function completeOrder(userId, orderRowId, webinarId, orderId) {
    // currently only hit by new user creations
    // might be good to generalize things thru this point
    var cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setConfirmOrderForm($('#confirmOrder'));

    var confirmOrderForm = cartStateManager.getConfirmOrderForm();

    confirmOrderForm.on('submit', function (e) {
        console.log("confirmOrderForm");
        e.preventDefault();

        var self = $(this);
        self.find('input[name="id"]').val(orderRowId);

        var data = $(this).serialize();

        var confirmRegistrationBillMe = $('#ConfirmRegistrationBillMe');

        confirmRegistrationBillMe.prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
        confirmRegistrationBillMe.attr('disabled', 'disabled');

        $.ajax({
            type: 'POST',
            contentType: RegistrationInCart.Constants.FormPostContentType,
            cache: false,
            url: self.attr('action'),
            dataType: RegistrationInCart.Constants.JsonDataType,
            data: data,
            beforeSend: function () {
                confirmRegistrationBillMe.attr('disabled', 'disabled');

            }
        }).done(function (result) {
            if (result.Result === 'Success') {
                orderRowId = result.OrderRowId;

                $('#orderDetails').empty();
                $('#orderDetails').append(result.Msg);

                $('#orderStatusLabel').text('Submitted').removeClass('label-warning').addClass('label-success');
                confirmRegistrationBillMe.after('<span>&nbsp;<span class="label label-success">&nbsp;<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;Transferring you now...</span></span>');

                okToLeave = true;

                var utilities = new Common.Utilities();
                utilities.goToUrl('/Account/OrderComplete/' + orderId);

            } else {

                L.clientLogger.error("Error #935: ", { result: result && result.Result });
                confirmRegistrationBillMe.after('<span class="text-error">Error #935. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
            }

            $('#finalLoadingSpinner').remove();
            confirmRegistrationBillMe.removeAttr('disabled');
        }).fail(commonFuncs.failCallBack);
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
        //        self.find('input[name="id"]').val(cartStateManager.getOrderRowId());
        console.log("hit CancelReg");
        console.log(data);

        $.post(cancelOrderForm.attr('action'), data, function (response, status, xhr) {
            if (response.success) {
                okToLeave = true;
                var utilities = new Common.Utilities();
                utilities.goToUrl('/webinar/details/' + webinarId);

            } else {

                L.clientLogger.error("Cancel Order Failure: ", { data: xhr && xhr.data });

                $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Error #216. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                $('#CancelModal').modal('hide');
            }

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

    modalForm.modal('show');
}

function hookUpChangeTypeLogic(dropDown) {
    //alert("Hit");
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

    dropDown.on('change', function (e) {

        e.preventDefault();

        valOfTypeChosenCurrent = $(this).val();
        var totalPrice = 0;

        var url = '/Cart/CheckIfAddLocShouldHide?optionID=' + valOfTypeChosenCurrent;

        $.ajax({
            type: 'GET',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            beforeSend: function () {
                dropDown.attr('disabled', 'disabled').after('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
            }
        }).done(function (data) {

            // see CartController's CheckIfAddLocShouldHide method for commented explanation regarding the 'shouldShow' property.
            if (data.shouldShow === 'No') {

                registerDuringCheckout.gatherPricingData();

                //  First, check if there are currently any Additional Locations added to the order.
                if (anyAddLocs === true) {

                    position = $('#confirmation').offset();

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
                        data: JSON.stringify(payLoad)
                    }).done(function (data) {

                        if (data.Result === 'Success') {
                            // This next variable is initially set in the CheckoutConfirm.cshtml razor view
                            anyAddLocs = false;

                            $('#additionalLocationsCaption').html('None');

                            $('#addlocSpiel').text('To add additional locations for this order, please use our Help & Feedback button (lower right corner)  for immediate assistance').addClass('text-info');

                            $('#addLocsText').html('Additional Locations: <span id="totalAdLocsPrice">$0.00</span>').addClass('muted');
                            totalPrice = registerDuringCheckout.totalPrice - registerDuringCheckout.addLocsPrice;

                            updatePriceOnNewSelection(valOfTypeChosenCurrent, totalPrice, dropDown);
                        }
                    }).fail(commonFuncs.failCallBack);

                    valOfTypeChosenCurrent = valOfTypeChosenPrevious = dropDown.val();
                    typeChosenCurrent = typeChosenPrevious = $.trim($('#RegType option:selected').text());
                    chosenRegTypeLabel.empty().text(typeChosenCurrent);
                } else {
                    typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
                    chosenRegTypeLabel.empty().text(typeChosenCurrent);
                    valOfTypeChosenPrevious = valOfTypeChosenCurrent;

                    updatePriceOnNewSelection(valOfTypeChosenCurrent, registerDuringCheckout.totalPrice, dropDown);
                }
            } else {
                typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
                chosenRegTypeLabel.empty().text(typeChosenCurrent);
                valOfTypeChosenPrevious = valOfTypeChosenCurrent;
                updatePriceOnNewSelection(valOfTypeChosenCurrent, registerDuringCheckout.totalPrice, dropDown);
            }

            //if (data.shippingDetailsRqrd === 'Yes') {
            //    registerDuringCheckout.addressOptions['shippingAddressRequired'] = true;
            //} else {
            //    registerDuringCheckout.addressOptions['shippingAddressRequired'] = false;
            //}
        }).fail(commonFuncs.failCallBack);
    });
}

function hookUpEditUserLogic(button, shippingAddressRequired) {

    var modalForm = $('#UserDetailsModal');

    // There may be times where a button does not trigger the modal.
    if (button) {
        button.on('click', function (e) {

            e.preventDefault();

            hookUpModal(modalForm);
        });
    }

    modalForm.on('shown', function (e) {

        $('#updateShippingMsgLabelWrap').empty();
        var userDetailsForm = $('#userDetailsForm');

        $('#saveChangesButton').on('click', function () {
            e.preventDefault();

            userDetailsForm.submit();
        });

        userDetailsForm.on('submit', function (e) {

            e.preventDefault();

            var url = $(this).attr('action'); // -> /Account/UpdateShippingDetails

            var payload = $(this).serialize();
            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                data: payload,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                beforeSend: function (xhr) {
                    $('#updateShippingMsgLabelWrap').html('<span class="label label-info">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Updating details...</span>');
                }
            }).done(function (data) {

                if (data.Result === 'Success') {
                    var fullname = $('#ShippingAddress_Name').val();

                    $('#userFullnameLabel').text(fullname);

                    $('#updateShippingMsgLabelWrap').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details updated successfully.</span>');

                } else if (!data.isSuccessful) {

                    L.clientLogger.error("userDetailsForm Submission Fails: ", { data: data && data.Result });
                    $('#updateShippingMsgLabelWrap').empty();
                    formProcessor.lightUpValidationSummary('userDetailsValSummary', data);
                } else {
                    L.clientLogger.error("Error #416: ", { data: data && data.Result });
                };

                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Error #416. For customer service contact us by using the Online Chat button below or emailing @globalConfig.TenantEmail .</span>');

            }).fail(function (data) {

                if (jqXHR.statusCode().status == 403) {
                    alert('Sorry, your session has expired. Please login again to continue');
                    window.location.href = '/Account/Login';
                } else if (jqXHR.statusCode().status === 0 && errorThrown === '' && textStatus === 'error') {
                    ; // do nothing
                } else {
                    alert('An error occurred: ' + jqXHR.statusCode().status + ' nError: ' + jqXHR.statusCode().statusText);
                };

                L.clientLogger.error("Error #417", { jqXHR: jqXHR && jqXHR.statusCode().statusText });
                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Error #417. For customer service contact us by using the Online Chat button below or emailing @globalConfig.TenantEmail .</span>');
            });
        });


    });

    modalForm.on('hidden', function (e) {
        $('#saveChangesButton').off('click');
        $('#userDetailsForm').off('submit');
    });
}

function change(_addLocs) {

    change.on('change', function (e) {

        e.preventDefault();

        addLocs = $(this).val();
        var totalPrice = 0;

        var url = '/Cart/CheckIfAddLocShouldHide?optionID=' + valOfTypeChosenCurrent;

        $.ajax({
            type: 'GET',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            beforeSend: function () {
                dropDown.attr('disabled', 'disabled').after('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
            }
        }).done(function (data) {

            // see CartController's CheckIfAddLocShouldHide method for commented explanation regarding the 'shouldShow' property.
            if (data.shouldShow === 'No') {

                registerDuringCheckout.gatherPricingData();

                //  First, check if there are currently any Additional Locations
                //added to the order.
                if (anyAddLocs === true) {

                    position = $('#confirmation').offset();

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
                        data: JSON.stringify(payLoad)
                    }).done(function (data) {

                        if (data.Result === 'Success') {
                            // This next variable is initially set in the CheckoutConfirm.cshtml razor view
                            anyAddLocs = false;

                            $('#additionalLocationsCaption').html('None');

                            $('#addlocSpiel').text('To add additional locations for this order, please use our Help & Feedback button (lower right corner)  for immediate assistance').addClass('text-info');

                            $('#addLocsText').html('Additional Locations: <span id="totalAdLocsPrice">$0.00</span>').addClass('muted');
                            totalPrice = registerDuringCheckout.totalPrice - registerDuringCheckout.addLocsPrice;

                            updatePriceOnNewSelection(valOfTypeChosenCurrent, totalPrice, dropDown);
                        }
                    }).fail(commonFuncs.failCallBack);
                    alert("of failed");
                } else {
                    updatePriceOnAddLocChange(addLocs, registerDuringCheckout.totalPrice, "");
                }
            } else {
                updatePriceOnNewSelection(valOfTypeChosenCurrent, registerDuringCheckout.totalPrice, dropDown);
            }

            //if (data.shippingDetailsRqrd === 'Yes') {
            //    registerDuringCheckout.addressOptions['shippingAddressRequired'] = true;
            //} else {
            //    registerDuringCheckout.addressOptions['shippingAddressRequired'] = false;
            //}
        }).fail(commonFuncs.failCallBack);
    });

}

function hookUpAddLocsLogic(_addLocs) {

    var changeTypeConfirmModal = $('#changeAddLocsModal');
    var addLocCaption = $('#chosenAddLoc');
    var addLocs = "";

}

// This function's purpose is to update pricing details where the RegType DropDown has its selected value changed.
// TODO: Display 'Confirm Shipping Address' via the Shipping Details modal form where the RegType chosen has a shipping address requirement.
function updatePriceOnNewSelection(registrationTypeId, totalPrice, dropDown) {

    registerDuringCheckout.gatherPricingData();

    var url = '/Cart/UpdateOrderDetails';

    var payLoad = {
        idOrderRow: cartStateManager.getOrderRowId(),
        idRegType: registrationTypeId
    };

    $.ajax({
        type: 'POST',
        contentType: constants.JsonContentType,
        cache: false,
        url: url,
        dataType: constants.JsonDataType,
        data: JSON.stringify(payLoad)
    }).done(function (data) {
        console.log(data);
        if (data) {
            if (data.Tax > 0) {
                $('#showTax').removeClass("hidden");
            } else {
                $('#showTax').addClass("hidden");
            }
            console.log(data);
            var alertCaption = data.UpdateSuccessCaption;
            $('#flyUpdateSuccessFlag').html(data.UpdateSuccessCaption).show();
            $('#discountCaption').html(data.DiscountCaption);
            $('#optionLabel').html(data.regTypeShort);

            $('#baseCost').html('$' + data.BasePrice + '');
            $('#totalDiscount').html('<span id="showDiscount">$' + data.Discount + '');
            $('#taxAmt').html(data.Tax + '');
            $('#totalAdLocsPrice').html('$' + data.OptionsPrice + '');
            //if (data.OrderStatusCaption !== "") {
            $('#orderStatusLabel').html(data.OrderStatusCaption);



            $('#OrderSumRegType').html("<span id=\"OrderSumRegType\"><i>Type: </i>" + data.regTypeShort + "</span>");
            $('#OrderSumUserStatus').html("<span id=\"OrderSumUserStatus\"><i>Status: </i>" + data.OrderStatusCaption + "</span>");
            $('#OrderSumUserCost').html("<span id=\"OrderSumUserCost\"><i>Total Cost:</i>" + data.Total + "</span>");
            //    alertCaption += " This previously paid order now has a balance due: $" + data.OutstandingBalance;
            //}
            $('#totalPrice').html('<span id="totalPrice">$' + data.Total + '</span>');
            if (data.TotalPaid !== 0) {
                console.log(data.TotalPaid);
                if (data.OutstandingBalance > 0) {
                    $('#showOutstandingBalance').html('<span style=\"color: red;\"  id="outstandingBalance">Due: $' + data.OutstandingBalance + '</span>');

                } else {

                    $("#ShowPayByCCModal").hide();
                }
            }
        }
        alert(alertCaption);
        location.reload();
        dropDown.removeAttr('disabled');
        $('#discountSpinner').remove();

        ShowModalForShippingDetails();
    }).fail(commonFuncs.failCallBack);
}
// TODO: Display 'Confirm Shipping Address' via the Shipping Details modal form where the RegType chosen has a shipping address requirement.
function updatePriceOnAddLocChange(addLocs, totalPrice) {

    registerDuringCheckout.gatherPricingData();

    var url = '/Cart/UpdateAdditionalLocations';

    var payLoad = {
        idOrderRow: cartStateManager.getOrderRowId(),
        addLocs: addLocs
    };

    $.ajax({
        type: 'POST',
        contentType: constants.JsonContentType,
        cache: false,
        url: url,
        dataType: constants.JsonDataType,
        data: JSON.stringify(payLoad)
    }).done(function (data) {
        console.log(data);
        if (data) {
            if (data.Tax > 0) {
                $('#showTax').removeClass("hidden");
            } else {
                $('#showTax').addClass("hidden");
            }

            $('#flyUpdateSuccessFlag').html(data.UpdateSuccessCaption).show();
            $('#discountCaption').html(data.DiscountCaption);
            $('#optionLabel').html(data.regTypeShort);
            $('#baseCost').html('$' + data.BasePrice + '');
            $('#totalDiscount').html('<span id="showDiscount">$' + data.Discount + '');
            $('#taxAmt').html(data.Tax + '');
            $('#totalAdLocsPrice').html('$' + data.OptionsPrice + '');
            $('#totalPrice').html('<span id="totalPrice">$' + data.Total + '</span>');
        }

        dropDown.removeAttr('disabled');
        $('#discountSpinner').remove();

        ShowModalForShippingDetails();
    }).fail(commonFuncs.failCallBack);
}

function ShowModalForShippingDetails(shippingDetailsRqrd) {

    if (shippingDetailsRqrd === true) {
        hookUpModal($('#UserDetailsModal'));
    }
}

function hookUpApplyDiscountLogic(btn, orderRowId) {

    btn.on('click', function (e) {
        var $item = $(e);
        var form = $item.parents("form");
        var $form = $(form);

        e.preventDefault();

        registerDuringCheckout.gatherPricingData();

        if (registerDuringCheckout.totalPrice < 1) {
            return;
        }
        var token = $(this).find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var url = '/cart/ApplyDiscountCode';
        var payload = { code: $('#CheckoutDiscountCode').val(), orderRowId: orderRowId };
        var self = this;

        $.ajax({
            type: 'POST',

            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers,
            beforeSend: function () {
                $(self).prepend('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                $(self).attr('disabled', 'disabled');
            }
        }).done(function (data) {

            if (data.Result == 0) {
                alert("The discount code " +
                    $('#CheckoutDiscountCode').val() +
                    " was not found or had an error that prevented usage. Try again or use our Help & Feedback button (lower right corner)  for assistance.");
            }
            if (data.Tax > 0) {
                $('#showTax').removeClass("hidden");
            } else {
                $('#showTax').addClass("hidden");
            }
            console.log(data);
            var alertCaption = data.UpdateSuccessCaption;
            $('#flyUpdateSuccessFlag').html(data.UpdateSuccessCaption).show();
            $('#discountCaption').html(data.DiscountCaption);
            $('#optionLabel').html(data.regTypeShort);

            $('#baseCost').html('$' + data.BasePrice + '');
            $('#totalDiscount').html('<span id="showDiscount">$' + data.Discount + '');
            $('#taxAmt').html(data.Tax + '');
            $('#totalAdLocsPrice').html('$' + data.OptionsPrice + '');

            $('#totalPrice').html('<span id="totalPrice">$' + data.Total + '</span>');
            if (data.TotalPaid !== 0) {
                if (data.OutstandingBalance > 0) {
                    $('#showOutstandingBalance').html('<span style=\"color: red;\"  id="outstandingBalance">Due: $' +
                        data.OutstandingBalance +
                        '</span>');

                } else {
                    if (data.OrderStatusCaption !== "") {
                        $('#orderStatusLabel').html(data.OrderStatusCaption);
                        alertCaption += " This previously paid order now has a balance due: $" +
                            data
                                .OutstandingBalance;
                    }
                    $("#ShowPayByCCModal").hide();
                    $('#showOutstandingBalance')
                        .html('<br><span style=\"color: green;\"  id="outstandingBalance">Due: $(' +
                        data.OutstandingBalance +
                        ')</span>');
                }
            }

            if (data.Total === 0 || data.Total < 0) {
                alertCaption += " The order is fully discounted.";

                $("#ConfirmRegistrationBillMe").text("Submit Order");
                $("#ShowPayByCCModal").hide();
            }

            alert(alertCaption);
            $('#discountSpinner').remove();

        }).fail(commonFuncs.failCallBack).always(function (e) {
            $('#discountSpinner').remove();
            $(self).removeAttr('disabled');
        });
    });
}

function showModalForShippingAddressDetails() {
    var modalShippingDetails = $('#UserDetailsModal'),
        modalFormOptionsOnPageLoad = {
            keyboard: true,
            backdrop: 'static',
            show: true
        };

    modalShippingDetails.on('shown', function (e) {
        $('#updateShippingMsgLabelWrap').empty();
    });

    modalShippingDetails.modal(modalFormOptionsOnPageLoad);
}

function createUserAccount(email) {

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
        data: JSON.stringify({ email: email }),
        headers: headersMr,
        beforeSend: function () {

        }
    }).done(function (data) {
        return; // do nothing. This is a fire and forget operation.
    }).fail(commonFuncs.failCallBack);
}