var registerButtonText = 'Submit Register';
var nextButtonText = 'Next...';

//  simple C-like enum implementation
var actions = { CheckEmail: 'CheckEmail', CheckZip: 'CheckZip', GetPassword: 'GetPassword', SubmitLogin: 'SubmitLogin', SubmitRegister: 'SubmitRegister', DisplayBillingAddressFields: 'DisplayBillingAddressFields' };
var inputActions = { EnterKeyPress: 'EnterKeyPress', ButtonClick: 'ButtonClick', None: 'None' };
var buttons = { EnterDiffAddress:'EnterDiffAddress', nonUSAddress: 'nonUSAddress', NotInstitution: 'NotInstitution', ResetPass: 'resetPass', TheSubmit: 'TheSubmit', YesUseAddress: 'YesUseAddress' };


var wrapEmail;
var wrapPass;
var wrapReset;
var wrapZip;
var theSubmitButton;
var emailInput;
var createUserForm;
var phoneBilling;
var fullNameShipping;
var institution;
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
var login;
var reset;
var register;
var labelEmail;
var modalInstitution;


var pageObjects = {
    theSubmitButton: theSubmitButton || $('#TheSubmitButton'),
    emailInput: emailInput || $('#RegisterFields_Email'),
    cityBilling: cityBilling || $('#RegisterFields_BillingAddress_City'),
    cityShipping: cityShipping || $('#RegisterFields_ShippingAddress_City'),
    createUserForm: createUserForm || $('#_CreateUserForm'),
    phoneBilling: phoneBilling || $('#RegisterFields_BillingAddress_Phone'),
    fullNameShipping: fullNameShipping || $('#FullNameShipping'),
    fullName: fullName || $('#FullName'),
    firstName: firstName || $('#FirstName'),
    institution: institution || $('#RegisterFields_Institution'),
    labelEmail: labelEmail || $('#labelEmail'),
    lastName: lastName || $('#LastName'),
    login: login || $('#login'),
    modalInstitution: modalInstitution || $('#modalInstitution'),
    phoneShipping: phoneShipping || $('#RegisterFields_ShippingAddress_Phone'),
    register: register || $('#register'),
    reset: reset || $('#reset'),
    shippingFirstName: shippingFirstName || $('#ShippingFirstName'),
    shippingLastName: shippingLastName || $('#ShippingLastName'),
    stateBilling: stateBilling || $('#RegisterFields_BillingAddress_State'),
    streetAddressBilling: streetAddressBilling || $('#RegisterFields_BillingAddress_StreetAddress'),
    streetAddressBilling2: streetAddressBilling2 || $('#RegisterFields_BillingAddress_StreetAddress2'),
    streetAddressShipping: streetAddressShipping || $('#RegisterFields_ShippingAddress_StreetAddress'),
    streetAddressShipping2: streetAddressShipping2 || $('#RegisterFields_ShippingAddress_StreetAddress2'),
    stateShipping: stateShipping || $('#RegisterFields_ShippingAddress_State'),
    wrapEmail: wrapEmail || $('#wrapEmail'),
    wrapReset: wrapReset || $('#wrapReset'),
    wrapZip: wrapZip || $('#wrapZip'),
    wrapPass: wrapPass || $('#wrapPass'),
    zipBilling: zipBilling || $('#RegisterFields_BillingAddress_Zip'),
    zipShipping: zipShipping || $('#RegisterFields_ShippingAddress_Zip')
};

var stateManager = function () {

    var action = null,
        inputAction = null,
        disregardIntitutionDomain = null,
        zipCheckRequired = null,

        displayBillingFields = function() {
            if (pageObjects.emailInput.valid() == '1') {
                $('#collapseBilling').parent().show();

                var showBillingInputs = $.Deferred(function() {
                    return $('#collapseBilling').collapse('show');
                });

                $.when(showBillingInputs.resolve()).then(function() {
                    pageObjects.fullName.focus();
                });
                
                $('#collapseShipping').parent().show();
                $('#collapseEmail').collapse('toggle');



                pageObjects.theSubmitButton.prop('value', registerButtonText);
            }
        },

        enterDifferentAddress = function() {
            console.log('EnterDiffAddress hit');
            pageObjects.modalInstitution.modal('hide');
            pageObjects.wrapEmail.show('slow');
            pageObjects.wrapZip.hide('slow');

            zipCheckRequired = true;

            pageObjects.theSubmitButton.prop('value', nextButtonText);

            disregardIntitutionDomain = true;

            stateManager.action = actions.CheckEmail;
        },

        resetPasswordOrLoginView = function(email) {

            $('#Email1').val(email);
            $('#ResetPassEmail').val(email);
            pageObjects.labelEmail.html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
            pageObjects.wrapEmail.hide('slow');

            var showLoginInput = $.Deferred(function() {
                return pageObjects.wrapReset.show('slow');
            });

            $.when(showLoginInput.resolve()).then(function() {
                $('#Email1').focus();
            });

            stateManager.action = actions.SubmitLogin;
            pageObjects.theSubmitButton.prop('value', 'Log In');
        },

        foundInstitutionView = function(data, email) {
            stateManager.inputAction = inputActions.None;
            pageObjects.modalInstitution.modal('show');

            pageObjects.institution.val(data.Institution);
            pageObjects.streetAddressShipping.val(data.Address);
            pageObjects.cityShipping.val(data.City);
            pageObjects.stateShipping.val(data.State);
            pageObjects.zipShipping.val(data.Zip);
            pageObjects.streetAddressBilling.val(data.Address);
            pageObjects.cityBilling.val(data.City);
            pageObjects.stateBilling.val(data.State);
            pageObjects.zipBilling.val(data.Zip);
            pageObjects.labelEmail.html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf('@')) + '</b>&nbsp; domain has been identified.</span>');
            $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
        },

        newPasswordView = function(email) {
            pageObjects.wrapEmail.hide('fast');

            var showPwdInput = $.Deferred(function() {
                pageObjects.wrapPass.show('fast');
            });

            $.when(showPwdInput.resolve()).then(function() {
                 return $('#RegisterFields_Password').focus();
            });

            

            if (email) {
                pageObjects.labelEmail.html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
            } else {
                pageObjects.modalInstitution.modal('hide');
                pageObjects.labelEmail.fadeOut(500, function() {
                    $(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + pageObjects.emailInput.val() + ' will be used for your email address.</span>');
                    $(this).fadeIn(500);
                });
            }

            stateManager.action = actions.GetPassword;

            pageObjects.theSubmitButton.on('mouseenter', function() {
                if ($('#sameAsBilling:checked').val()) {
                    setShippingToBilling();
                }
            });

        },

        newPassWordToNextStep = function() {

            if (zipCheckRequired) {
                wrapPass.hide('slow');
                stateManager.action = actions.CheckZip;

                var showGetZipInput = $.Deferred(function() {
                    wrapZip.show('slow');
                });

                $.when(showGetZipInput.resolve()).then(function() {
                    return $('#getZip').focus();
                });

            } else {
                displayBillingFields();
                stateManager.action = actions.SubmitRegister;
                zipCheckRequired = true;
            }
        },

        nonUsAdddress = function() {
            pageObjects.wrapZip.hide('slow');
            console.log('nonUSAddress hit');
            $('#nonUSAddress').show('slow');
            $('#collapseEmail').collapse('toggle');
            $('#collapseBilling').collapse('toggle');

            //TODO:  not sure what to set the state to here. Need to discuss.
        },

        notInstitutionAddress = function() {
            console.log('NotInstitution hit');
            pageObjects.modalInstitution.modal('hide');
            pageObjects.wrapEmail.show('slow');
            pageObjects.wrapZip.hide('slow');
            pageObjects.labelEmail.html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            //  Clear the billing and shipping addresses
            $('#collapseBilling input').val('');
            $('#collapseBilling textarea').val('Mailing address notes:');
            $('#collapseBilling select').val(0);

            $('#collapseShipping input:not("#sameAsBilling")').val('');
            $('#collapseShipping textarea').val('Mailing address notes:');
            $('#collapseShipping select').val(0);

            pageObjects.theSubmitButton.prop('value', nextButtonText);

            disregardIntitutionDomain = true;

            stateManager.action = actions.CheckEmail;
        },

        passResetView = function() {
            pageObjects.login.hide('slow');
            

            var showResetInput = $.Deferred(function () {
                pageObjects.reset.show('slow');
            });

            $.when(showResetInput.resolve()).then(function () {
                return $('#ResetPassEmail').focus();
            });

        },
        registerView = function() {
            pageObjects.login.hide('slow');
            pageObjects.register.show('slow');
            stateManager.action = actions.CheckEmail;

            var showRegisterInput = $.Deferred(function () {
                pageObjects.register.show('slow');
            });

            $.when(showRegisterInput.resolve()).then(function () {
                return pageObjects.emailInput.focus();
            });
        },

        resetPassword = function (normalResetPasswordButton) {
            console.log('resetPass hit');
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('form#ResetPasswordForm').submit();
        },

        startView = function() {
            pageObjects.register.hide();
            pageObjects.reset.hide();
        },

        useRegisteredAddress = function() {
            console.log('YesUseAddress hit');

            zipCheckRequired = false;

            stateManager.showNewPasswordInputs();
        },

        zipCodeVerified = function(data, zipCode) {

            switch (data.success) {
            case 'true':
                console.log('zipCodeVerified-true hit');
                $('#collapseBilling').parent().show();
                $('#collapseShipping').parent().show();
                $('#collapseEmail').collapse('toggle');
                $('#collapseBilling').collapse('toggle');

                pageObjects.cityBilling.val(data.City);
                pageObjects.stateBilling.val(data.State);
                pageObjects.zipBilling.val(zipCode);
                $('#TimeZone').val(data.TimeZone);

                pageObjects.labelEmail.html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
                pageObjects.theSubmitButton.prop('value', registerButtonText);

                stateManager.action = actions.SubmitRegister;
                break;
            case 'false':
                console.log('zipCodeVerified-false hit');
                pageObjects.labelEmail.html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                break;
            case 'invalid format':
                console.log('zipCodeVerified-invalidformat hit');
                pageObjects.labelEmail.html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                break;
            }

        },
        submit = function() {

            switch (stateManager.action) {
            case actions.CheckEmail:
                console.log('CheckEmail hit');
                checkAndSubmitEmail();
                if (!stateManager.disregardIntitutionDomain)
                    stateManager.disregardIntitutionDomain = true;
                pageObjects.theSubmitButton.prop('value', nextButtonText);
                break;
            case actions.GetPassword:
                stateManager.newPassWordToNextStep();
                break;
            case actions.CheckZip:
                console.log('CheckZip hit');
                $('#ZipChecker').val($('#getZip').val());
                $('form#checkZip').submit();
                break;
            case actions.SubmitRegister:
                console.log('SubmitRegister hit');
                stateManager.action = '';
                if (pageObjects.createUserForm.valid() == '1') {
                    submitCreateUserForm();
                }
                break;
            case actions.SubmitLogin:
                console.log('submitLogin hit');
                $('#Password').val($('#Password1').val());
                $('#Email').val($('#Email1').val());
                $('form#frmSignIn').submit();
                break;
            case actions.DisplayBillingAddressFields:
                displayBillingFields();
                break;
            }
        },

        init = function(args) {
            stateManager.action = actions.CheckEmail;
            stateManager.inputAction = inputActions.ButtonClick;
            stateManager.disregardIntitutionDomain = false;
            zipCheckRequired = true;
            startView();
        };


    return {
        action: action,
        enterDifferentAddress: enterDifferentAddress,
        foundInstitution: foundInstitutionView,
        disregardIntitutionDomain: disregardIntitutionDomain,
        initialize: init,
        inputAction: inputAction,
        newPassWordToNextStep: newPassWordToNextStep,
        nonUsAdddressInvoked: nonUsAdddress,
        notInstitutionAddress: notInstitutionAddress,
        resetPassword: resetPassword,
        showNewPasswordInputs: newPasswordView,
        showPassReset: passResetView,
        showRegister: registerView,
        showResetPasswordOrLoginView: resetPasswordOrLoginView,
        submit: submit,
        useRegisteredAddress: useRegisteredAddress,
        zipCodeVerified: zipCodeVerified
    };
}();


function checkAndSubmitEmail() {
    if (pageObjects.emailInput.valid() == '1') {
        console.log(pageObjects.emailInput.valid());
        $('#emailAddress').val($('#checkEmail').val());
        $('form#checkEmail').submit();
    }
}

function checkAndSubmitZip() {
    pageObjects.theSubmitButton.prop('value', nextButtonText);
    console.log('checkAndSubmitZip');
    //$('#emailAddress').val($('#checkEmail').val());
    $('#checkZip').submit();
}


function submitCreateUserForm() {

    var valid = pageObjects.createUserForm.valid();

    console.log(valid);

    if (valid) {
        console.log('createUserForm submitted.');
        pageObjects.createUserForm.submit();

        pageObjects.theSubmitButton.off('mouseenter', function () {
            if ($('#sameAsBilling:checked').val()) {
                setShippingToBilling();
            }
        });
    }
}

function submitLogin() {
    //$('#Email').val($('#RegisterFields_Email').val());
    //$('#Password').val($('#Password1').val());
    //$('#frmSignin').submit();
    $('#Password').val($('#RegisterFields_Password').val());
    $('#Email').val($('#emailAddress').val());
    $('form#frmSignIn').submit();
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
    $('#RegisterFields_ShippingAddress_Country').val($('#RegisterFields_BillingAddress_Country').val());
    pageObjects.phoneShipping.val(pageObjects.phoneBilling.val());
}

function getMainPath(pathToCheck) {

    if (pathToCheck.substr(pathToCheck.length - 1) === '/')
        return pathToCheck.substr(0, pathToCheck.length - 1);
    return pathToCheck;
}

function setPath() {
    var indexOfHome = location.href.indexOf('Account');
    var path = '';

    if (indexOfHome > -1)
        path = location.href.substr(0, location.href.indexOf('Account') - 1);
    else
        path = location.href;

    //  IE is a rubbish browser!
    if (path === '')
        path = $(location).attr('href');
    return path;
}

// document.ready starts here
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

    $('#collapseBilling').parent().hide();
    $('#collapseShipping').parent().hide();

    register = $('#register');
    reset = $('#reset');

    stateManager.initialize();

    wrapEmail = $('#wrapEmail');
    wrapPass = $('#wrapPass');
    wrapReset = $('#wrapReset');
    wrapZip = $('#wrapZip');


    pageObjects.wrapZip.hide();
    pageObjects.wrapPass.hide();
    pageObjects.wrapReset.hide();
    $('#nonUSAddress').hide();
    $('#getFirstLast').hide();

    theSubmitButton = $('#TheSubmitButton');
    emailInput = $('#RegisterFields_Email');
    createUserForm = $('#_CreateUserForm');
    labelEmail = $('#labelEmail');

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
    shippingLastName = $('#ShippingLastName');
    shippingFirstName = $('#ShippingFirstName');
    fullName = $('#FullName');
    firstName = $('#FirstName');
    lastName = $('#LastName');
    institution = $('#RegisterFields_Institution');


    var path = setPath();

    pageObjects.emailInput.bind('change keyup', function () {
        if ($(this).validate().checkForm()) {
            pageObjects.theSubmitButton.removeClass('button_disabled').attr('disabled', false);
        } else {
            pageObjects.theSubmitButton.addClass('button_disabled').attr('disabled', true);
        }
    });
    $('[name=TheSubmit]').prop('value', nextButtonText);


    $('input').keypress(function (event) {
        var enterOkClass = $(this).hasClass('enterSubmit');
        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (event.which == 13) {

            if (pageObjects.modalInstitution.filter(':visible').length > 0
                && inputElementTriggered !== buttons.YesUseAddress
                && inputElementTriggered !== buttons.EnterDiffAddress
                && inputElementTriggered !== buttons.NotInstitution) {
                return false;
            }

            stateManager.inputAction = inputActions.EnterKeyPress;

            switch (inputElementTriggered) {
                case 'RegisterFields_Password':
                case 'RegisterFields.ConfirmPassword':
                case 'RegisterFields.Email':
                case 'getZip':
                case 'Password1':
                case 'Email1':
                case buttons.TheSubmit:
                    console.log('ActionForTheSubmit = ' + stateManager.action);
                    stateManager.submit(); break;
                case buttons.nonUSAddress: stateManager.nonUsAdddressInvoked(); break;
                case 'Email':
                case 'NormalResetPasswordButton':
                    if ($('#EdgeCaseResetPasswordButton').data('clicked'))
                        $('#EdgeCaseResetPasswordButton').removeData('clicked');
                    $('#NormalResetPasswordButton').data('clicked', true);
                    $('form#ResetPasswordForm').submit();
                    break;
                case '#EdgeCaseResetPasswordButton':
                case buttons.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
                case buttons.YesUseAddress: stateManager.useRegisteredAddress(); break;
                case buttons.EnterDiffAddress: stateManager.enterDifferentAddress(); break;
                case buttons.NotInstitution: stateManager.notInstitutionAddress(); break;
                default:
                    if (pageObjects.theSubmitButton.val() === registerButtonText) {
                        if ($('#sameAsBilling:checked').val()) {
                            setShippingToBilling();
                        }
                        stateManager.action = actions.SubmitRegister;
                        stateManager.submit();
                    }
            }

            console.log(stateManager.action);
            event.preventDefault();

            //if (ActionForTheSubmit === 'CheckEmail') {
            //    //ActionForTheSubmit = 'CheckZip';
            //    if (pageObjects.emailInput.valid() == '1') {
            //        checkAndSubmitEmail();
            //    }
            //    return false;
            //}
            //if (ActionForTheSubmit === 'CheckZip') {

            //    checkAndSubmitZip();

            //    return false;
            //}
            //if (stateManager.action === actions.GetPassword) {

            //    stateManager.inputAction = inputActions.EnterKeyPress;
            //    stateManager.newPassWordToCheckZip();
            //    return false;
            //}

            ////if (ActionForTheSubmit === 'SubmitRegister') {

            ////    if (pageObjects.createUserForm.valid() == '1') {
            ////        submitCreateUserForm();
            ////    }
            ////    return false;
            ////}
            //if (stateManager.action === actions.SubmitLogin) {

            //    $('#Password').val($('#Password1').val());
            //    $('#Email').val($('#Email1').val());
            //    $('form#frmSignIn').submit();

            //    return false;
            //}
            //if (!enterOkClass) {
            //    return false;
            //}
        }
    });


    $('body').on('click', 'input:button', (function (e, data) {

        if (stateManager.inputAction === inputActions.EnterKeyPress)
            return false;

        stateManager.inputAction = inputActions.ButtonClick;

        if (stateManager.action === '') {
            pageObjects.labelEmail.html('<span class="label label-important">&nbsp;&nbsp;There registration has encountered a problem. You\'ll need to start the registration process again.</span>');
            
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
            case buttons.TheSubmit:
                console.log('ActionForTheSubmit = ' + stateManager.action);
                stateManager.submit(); break;
            case buttons.nonUSAddress: stateManager.nonUsAdddressInvoked(); break;
            case buttons.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
            case buttons.YesUseAddress: stateManager.useRegisteredAddress(); break;
            case buttons.EnterDiffAddress: stateManager.enterDifferentAddress();break;
            case buttons.NotInstitution: stateManager.notInstitutionAddress();break;
        default:
        }
    }));


    $('#collapseShipping').on('shown', function () {
        if ($('#sameAsBilling:checked').val()) {
            setShippingToBilling();
        }
    });
    //var availableTags = [];
    ////http://stackoverflow.com/questions/5077409/what-does-autocomplete-request-server-response-look-like
    //$('#RegisterFields_Institution').autocomplete({
    //    source: function (request, response) {
    //        alert(request.term);
    //        $.ajax({
    //            type: 'GET',
    //            cache: false,
    //            url: '/Account/AutocompleteInstitution',
    //            data: {
    //                zip: '54636', term: request.term
    //            },
    //            //data: { zip: pageObjects.zipBilling.val()},
    //            //beforeSend: function () {
    //            //    // this is where we append a loading image
    //            //    pageObjects.labelEmail.html('<div class="btn-warning style='width: 400px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</div>');
    //            //},
    //            success: function (data) {
    //                response($.map(data.geonames, function (item) {
    //                    return {
    //                        label: item.name + (item.adminName1 ? ', ' + item.adminName1 : '') + ', ' + item.countryName,
    //                        value: item.name
    //                    };
    //                }));
    //            }
    //        });
    //    },
    //    //source: '/Account/AutocompleteInstitution',
    //    //minLength: ,
    //    open: function (event, ui) {
    //        $('.ui-autocomplete').css('z-index', 1000);
    //    },
    //    select: function (event, ui) {
    //        pageObjects.institution.val(ui.item.Work_Item);
    //        return false;
    //    }
    //});

    pageObjects.createUserForm.on('submit', function (e) {
        e.preventDefault();

        if (stateManager.action !== actions.SubmitLogin) {
            console.log('non-valid form');
            return false;
        }

        if (pageObjects.createUserForm.valid() != '1') {
            console.log('non-valid form');
            return false;
        }

        var url = '/Account/Register';
        console.log('Submitting Register Details');

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: $(this).serialize(),
            beforeSend: function () {
                console.log('beforeSend Register Details');
                // this is where we append a loading image
                pageObjects.labelEmail.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
            }
        }).done(function (data) {
            console.log('done: ');
            if (data.Status === 'Success') {
                console.log('success: ' + data.Status);
                stateManager.action = '';
                pageObjects.labelEmail.html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
                location.assign(path + '/Account/Login'); //recommend using url lib whose name I've forgotten to build this url. Remind me if this comment is till here
            } else if (data.Status === 'Fail') {
                pageObjects.labelEmail.html('<span class="label label-information">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
            }
        }).fail(function (data) {
            console.log('failed: ' + data);
        });
    });

    $('form#checkEmail').submit(function (e) {

        e.preventDefault();
        var jsonUrl = '/Account/CheckEmail';
        var email = pageObjects.emailInput.val();
        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: constants.JsonDataType,
                data: { email: email, disregardIntitutionDomain: stateManager.disregardIntitutionDomain },
                beforeSend: function () {
                    // this is where we append a loading image
                    pageObjects.labelEmail.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                if (data.success === 'foundExisting') {
                    stateManager.showResetPasswordOrLoginView(email);

                } else if (data.success === 'foundInstitution') {
                    stateManager.foundInstitution(data, email);

                } else if (data.email === 'wasNotFound') {
                    stateManager.showNewPasswordInputs(email);
                }

            }).fail(function () {
                // failed request; give feedback to user
                pageObjects.wrapEmail.html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function() {
                 stateManager.inputAction = inputActions.None;
            });
        }
    });

    $('form#checkZip').submit(function () {
        var jsonUrl = '/Account/CheckZip';
        var q = $('#ZipChecker').val();
        if (q.length == 0) {
            $('#RegisterFields_Zip').focus();
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
                    pageObjects.labelEmail.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                stateManager.zipCodeVerified(data, q);
            }).fail(function () {
                // failed request; give feedback to user
                pageObjects.wrapZip.html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                stateManager.inputAction = inputActions.None;
            });
        }
        return false;
    });

    //pageObjects.institution.change(function () {
    //    alert('hit' + pageObjects.institution.val());
    //    //  $.getJSON()
    //    $('#InstitutionChecker').val(pageObjects.institution.val());
    //    $('form#checkInstitution').submit();
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

    //$('#sameAsBilling').change(function (e) {
    //    var thisCheck = $(this);

    //    //  Remove keyup handlers on billing input fields first time (and each thereafter - which don't matter) checkbox clicked.
    //    pageObjects.phoneBilling.off('keyup');
    //    pageObjects.streetAddressBilling.off('keyup');
    //    $('#RegisterFields_BillingAddress_StreetAddress2').off('keyup');
    //    pageObjects.cityBilling.off('keyup');
    //    pageObjects.stateBilling.off('keyup');
    //    pageObjects.zipBilling.off('keyup');

    //    if (thisCheck.is(':checked')) {
    //        if ($('#sameAsBilling:checked').val()) {
    //            setShippingToBilling();
    //        } else {
    //            if (pageObjects.fullNameShipping.val() === null || pageObjects.fullNameShipping.val() === '') pageObjects.fullNameShipping.val(pageObjects.fullName.val());
    //            if (pageObjects.shippingFirstName.val() === null || pageObjects.shippingFirstName.val() === '') pageObjects.shippingFirstName.val(pageObjects.firstName.val());
    //            if (pageObjects.shippingLastName.val() === null || pageObjects.shippingLastName.val() === '') pageObjects.shippingLastName.val(pageObjects.lastName.val());
    //            if (pageObjects.cityShipping.val() === null || pageObjects.cityShipping.val() === '') pageObjects.cityShipping.val(pageObjects.cityBilling.val());
    //            if (pageObjects.streetAddressShipping.val() === null || pageObjects.streetAddressShipping.val() === '') pageObjects.streetAddressShipping.val(pageObjects.streetAddressBilling.val());
    //            if (pageObjects.streetAddressShipping2.val() === null || pageObjects.streetAddressShipping2.val() === '') pageObjects.streetAddressShipping2.val(pageObjects.streetAddressBilling2.val());
    //            if (pageObjects.stateShipping.val() === null || pageObjects.stateShipping.val() === '') pageObjects.stateShipping.val(pageObjects.stateBilling.val());
    //            if (pageObjects.zipShipping.val() === null || pageObjects.zipShipping.val() === '') pageObjects.zipShipping.val(pageObjects.zipBilling.val());
    //        }
    //    }
    //    $(this).validate().checkForm();
    //    return false;
    //});


    pageObjects.modalInstitution.on('hidden', function (e) {
        stateManager.inputAction = inputActions.None;
        if (pageObjects.wrapZip.is(':visible')) {
            stateManager.action = actions.CheckEmail;
        }

    });



});