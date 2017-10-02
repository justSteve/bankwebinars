var formProcessor,
    institutionInput,
    institutionNames,
    stateManager,
    utilities;

$(function () {

    institutionInput = $('#RegisterFields_Institution');
    institutionNames = {};

    utilities = new Common.Utilities();

    stateManager = new Registration.StateManager();
    stateManager.initializeState();

    var path = utilities.setPathToBaseUrl();

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
            $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;There registration has encountered a problem. Please refresh the page and try again. In case of persistent problems please contact us with our online chat (lower right corner of this page).</span>');

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
        if (inputElementTriggered === '' && event.currentTarget.id === 'registerLinkButton')
            inputElementTriggered = event.currentTarget.id;

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
                    var edgeCaseResetPasswordButton = $('#EdgeCaseResetPasswordButton');
                    if (edgeCaseResetPasswordButton.data('clicked'))
                        edgeCaseResetPasswordButton.removeData('clicked');
                    $('#NormalResetPasswordButton').data('clicked', true);
                    $('form#ResetPasswordForm').submit();
                    break;
                case 'EdgeCaseResetPasswordButton':
                case Registration.Button.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
                case Registration.Button.YesUseAddress: stateManager.useRegisteredAddress(); break;
                case Registration.Button.EnterDiffAddress: stateManager.enterDifferentAddress(); break;
                case Registration.Button.NotInstitution: stateManager.notInstitutionAddress(); break;
                case 'registerLinkButton': stateManager.registerView(); break;
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

    $('#collapseShipping').on('shown', function () {
        if ($(stateManager.getSameAsBillingCheckedFilter()).val()) {
            stateManager.setShippingToBilling();
        }
    });


    $('form#checkEmail').submit(function(e) {

        e.preventDefault();

        var emailInput = $('#RegisterFields_Email');

        var jsonUrl = '/Account/CheckEmail';
        var email = emailInput.val();

        if (email.length === 0) {
            emailInput.focus();
        } else {

            var token = $(this).find('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                    type: 'POST',
                    contentType: Registration.Constants.FormPostContentType,
                    cache: false,
                    url: jsonUrl,
                    dataType: Registration.Constants.JsonDataType,
                    data: { email: email, disregardInstitutionDomain: stateManager.getDisregardInstitutionDomain(), __RequestVerificationToken: token },
                    beforeSend: function() {
                        // this is where we append a loading image
                        $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
                    }
                }).done(function(data) {
                    // successful request; do something with the data
                    if (data.success === 'foundExisting') {
                        stateManager.resetPasswordOrLoginView(email);

                    } else if (data.success === 'foundInstitution') {
                        stateManager.foundInstitutionView(data, email);

                    } else if (data.email === 'wasNotFound') {
                        stateManager.newPasswordView(email);
                    }

                }).fail(commonFuncs.failCallBack).always(function() {
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
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                stateManager.zipCodeVerified(data, zipCode);
                //Rollbar.info({ 'rdc-#1 Zipcode check result': { 'data': data || 'data was falsey' } });
            }).fail(commonFuncs.failCallBack).always(function () {
                stateManager.setInputAction(Registration.InputAction.None);
                //Rollbar.info({ 'rdc-#2 Zipcode checked': { 'zipCode': zipCode } });
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

                //  If true, then we are in the "register during checkout" flow. O/w, we are in the regular log-in flow.
                if ($('#labelEmail').is(':visible')) {
                    $('#labelEmail').html('<span class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Signing in...</span>');
                    $('#loginErrorSummary').empty();
                } else {
                    $('#loginMsgLabelWrap').show();
                    $('#loginMsgLabelWrap').html('<span id="signingInMsg" class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Signing in ...</span>');

                    var valSummary = $('#LoginValSummary');
                    valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                    var errorsList = valSummary.find('ul');
                    errorsList.empty();
                    errorsList.append('<li style="display:none"></li>');
                }
            }
        }).done(function (data) {
            console.log(data);
            if (data.msgForUser && data.msgForUser != '') {
                alert(data.msgForUser);
            }
            if (data.result === 'LoggedIn') {
                $('#signingInMsg').html('<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Redirecting you now ...');

                if (data.returnUrl !== '/' && !(data.returnUrl.indexOf('OrderComplete') > 0)) {
                    utilities.goToUrl(data.returnUrl);
                } else {
                    utilities.goToUrl('/Cart/MyWebinars');
                }

            } else if (data.result === 'Confirmed') {
                //  if here, user has to verify before they can log in
                //Rollbar.info('rdc-#3 Unverified', { 'msg': 'user has to verify before they can log in' });
                utilities.goToUrl('/Account/Confirmed?email=' + data.email + '&password=' + data.password);
            } else if (data.data) {

                if ($('#signingInMsg').is(':visible'))
                    $('#signingInMsg').remove();

                if ($('#labelEmail').is(':visible')) {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;&nbsp;Login error...</span>');
                }
                //Rollbar.error('rdc-#4 Validation Fail', { 'data': data && data.data });
                formProcessor.lightUpValidationSummary('LoginValSummary', data);
            }
        }).fail(commonFuncs.failCallBack);

    });

    $('#FullName').blur(function () {
        var tempName = $('#FullName').val().split(' ');
        if (tempName.length == 2) {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#RegisterFields_LastName').val(tempName[1]);
            $('#RegisterFields_Title').focus();
        } else {
            $('#RegisterFields_FirstName').val(tempName[0]);
            $('#getFull').hide();
            $('#getFirstLast').show();
            $('#RegisterFields_LastName').focus();
        }
    });

    $('#getFirstLast').on('blur', '#RegisterFields_FirstName, #RegisterFields_LastName', function () {
        var fullNameInput = $('#FullName');
        fullNameInput.val($('#RegisterFields_FirstName').val() + ' ' + $('#RegisterFields_LastName').val());
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

        var fullNameShipping = $('#FullNameShipping');
        var nameBilling = $('#RegisterFields_BillingAddress_Name');
        var nameShipping = $('#RegisterFields_ShippingAddress_Name');

        if (!fullNameShipping.val()) fullNameShipping.val($('#FullName').val());
        if (!nameBilling.val()) nameBilling.val($('#FullName').val());
        if (!nameShipping.val()) nameShipping.val($('#FullName').val());

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
                $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function (data) {
            //alert('done: ');
            if (data.Result) {
                if (data.Result === 'Success') {
                    //console.log('success: ' + data.Result);
                    stateManager.setAction('');
                    $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                    utilities.goToUrl('/');
                } else if (data.Result === 'Fail') {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There has been an error in the request. Please try again. In case of persistent problem please contact us with our online chat (lower right corner of this page).</span>');
                    stateManager.setAction(Registration.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                stateManager.setAction(Registration.Action.SubmitRegister);
                $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                formProcessor.lightUpValidationSummary('registerValSummary', data);
            }

        }).fail(commonFuncs.failCallBack).always(function () {
            stateManager.setInputAction(Registration.InputAction.None);
        });

        return false;
    });

    $('#TheSubmitButton').on('mouseenter', function () {
        if ($('#TheSubmitButton').val() === stateManager.getRegisterButtonText() && $(stateManager.getSameAsBillingCheckedFilter()).val()) {
            stateManager.setShippingToBilling();
        }
    });

    $('#collapseShipping').on('shown', function () {
        if ($(stateManager.getSameAsBillingCheckedFilter()).val()) {
            stateManager.setShippingToBilling();
        }
    });

    $('#RegisterFields_Institution').typeahead({
        source: function (query, process) {
            searchInstitution(query, process);
        },

        matcher: function (item) {
            return true;
        },

        highlighter: function (name) {
            return name;
        },

        sorter: function (items) {
            return items;
        },

        updater: function (name) {
            return name;
        }

    });

    $('#Email').focus();

    $('#RegisterFields_Institution').attr('autocorrect', 'off');
});

var searchInstitution = _.debounce(function (query, process) {

    var searchTerm = institutionInput.val();

    $.ajax({
        type: 'POST',
        contentType: constants.FormPostContentType,
        cache: false,
        url: '/Account/GetInstitutionsByName',
        dataType: constants.JsonDataType,
        data: { institutionName: searchTerm },
        beforeSend: function() {
            institutionNames = null; // dereference whatever is currently in 'institutionNames'. 
        }
    }).done(function(data) {
        institutionNames = data.institutions;

        process(institutionNames);
    }).fail(commonFuncs.failCallBack);

}, 200);