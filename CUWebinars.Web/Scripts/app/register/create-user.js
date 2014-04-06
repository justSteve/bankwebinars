var registerButtonText = 'Submit Register';
var nextButtonText = 'Next...';

// TODO: Inprogress indicator missing on ResetPW via:
//  EdgeCaseResetPasswordButton

//ResetPW ajax/text feedback missing
//Post ajax/return from 'Yes But enter diff address') needs UI something
// phone/address missing on inst copy. or is it.

// the reply  button lable is: '  Reset Instructions sent!' 
// but is missing the bit of text narrative about checking email

// was a <P> wrapped in a <div> i think.

//  simple C-like enums implementation
var actions = { CheckEmail: 'CheckEmail', CheckZip: 'CheckZip', GetPassword: 'GetPassword', LogIn: 'LogIn', PostCreateAccount: 'PostCreateAccount', SubmitLogin: 'SubmitLogin', SubmitRegister: 'SubmitRegister', DisplayBillingAddressFields: 'DisplayBillingAddressFields' };
var inputActions = { EnterKeyPress: 'EnterKeyPress', ButtonClick: 'ButtonClick', None: 'None' };
var buttons = { EnterDiffAddress: 'EnterDiffAddress', nonUSAddressBtn: 'nonUSAddressBtn', NotInstitution: 'NotInstitution', ResetPass: 'resetPass', SignInButton: 'SignInButton', TheSubmit: 'TheSubmit', YesUseAddress: 'YesUseAddress' };

var emailLoginInput;
var nonUSAddressBtn;
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
var collapseBilling;
var collapseShipping;
var registerFieldsPassword;
var getFirstLast;


var pageObjects = {
    //  This is the pattern which I want to implement. I just need to apply it to each of the members of the pageObjects object.
    theSubmitButton: function() {
        return theSubmitButton || $('#TheSubmitButton');
    },
    emailInput: function() {
        return emailInput || $('#RegisterFields_Email');
    },
    cityBilling: function() {
        return cityBilling || $('#RegisterFields_BillingAddress_City');
    },
    nonUSAddressBtn: function() {
        return nonUSAddressBtn || $('#nonUSAddressBtn');
    },
    cityShipping: function() {
        return cityShipping || $('#RegisterFields_ShippingAddress_City');
    },
    collapseBilling: function() {
        return collapseBilling || $('#collapseBilling');
    },
    collapseShipping: function() {
        return collapseShipping || $('#collapseShipping');
    },
    createUserForm: function() {
        return createUserForm || $('#_CreateUserForm');
    },
    emailLoginInput: function() {
        return emailLoginInput || $('#Email');
    },
    fullNameShipping: function() {
        return fullNameShipping || $('#FullNameShipping');
    },
    fullName: function() {
        return fullName || $('#FullName');
    },
    firstName: function() {
        return firstName || $('#FirstName');
    },
    getFirstLast: function () {
        return getFirstLast || $('#getFirstLast');
    },
    institution: function() {
        return institution || $('#RegisterFields_Institution');
    },
    labelEmail: function() {
        return labelEmail || $('#labelEmail');
    },
    lastName: function() {
        return lastName || $('#LastName');
    },
    login: function() {
        return login || $('#login');
    },
    modalInstitution: function() {
        return modalInstitution || $('#modalInstitution');
    },
    phoneBilling: function() {
        return phoneBilling || $('#RegisterFields_BillingAddress_Phone');
    },
    phoneShipping: function() {
        return phoneShipping || $('#RegisterFields_ShippingAddress_Phone');
    },
    register: function() {
        return register || $('#register');
    },
    registerFieldsPassword: function() {
        return registerFieldsPassword || $('#RegisterFields_Password');
    },
    reset: function() {
        return reset || $('#reset');
    },
    shippingFirstName: function() {
        return shippingFirstName || $('#ShippingFirstName');
    },
    shippingLastName: function() {
        return shippingLastName || $('#ShippingLastName');
    },
    stateBilling: function() {
        return stateBilling || $('#RegisterFields_BillingAddress_State');
    },
    streetAddressBilling: function() {
        return streetAddressBilling || $('#RegisterFields_BillingAddress_StreetAddress');
    },
    streetAddressBilling2: function() {
        return streetAddressBilling2 || $('#RegisterFields_BillingAddress_StreetAddress2');
    },
    streetAddressShipping: function() {
        return streetAddressShipping || $('#RegisterFields_ShippingAddress_StreetAddress');
    },
    streetAddressShipping2: function() {
        return streetAddressShipping2 || $('#RegisterFields_ShippingAddress_StreetAddress2');
    },
    stateShipping: function() {
        return stateShipping || $('#RegisterFields_ShippingAddress_State');
    },
    wrapEmail: function() {
        return wrapEmail || $('#wrapEmail');
    },
    wrapPass: function() {
        return wrapPass || $('#wrapPass');
    },
    wrapReset: function() {
        return wrapReset || $('#wrapReset');
    },
    wrapZip: function() {
        return wrapZip || $('#wrapZip');
    },
    zipBilling: function() {
        return zipBilling || $('#RegisterFields_BillingAddress_Zip');
    },
    zipShipping: function() {
        return zipShipping || $('#RegisterFields_ShippingAddress_Zip');
    },
};

function enterBillingPane(data) {
    pageObjects.collapseBilling().parent().show();
    pageObjects.collapseShipping().parent().show();
    $('#collapseEmail').collapse('toggle');

    var showBillingInputs = $.Deferred(function () {
        pageObjects.collapseBilling().collapse('toggle');

    });

    $.when(showBillingInputs.resolve()).then(function () {
        pageObjects.fullName().focus();
    });

    pageObjects.cityBilling().val(data.City);
    pageObjects.stateBilling().val(data.State);
    //pageObjects.zipBilling().val(zipBilling);
    $('#TimeZone').val(data.TimeZone);

    pageObjects.labelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
    pageObjects.theSubmitButton().prop('value', registerButtonText);
}

var stateManager = function () {

    var action = null,
        inputAction = null,
        disregardIntitutionDomain = null,
        zipCheckRequired = null,

        displayBillingFields = function () {

            if (stateManager.inputAction === inputActions.EnterKeyPress)
                stateManager.inputAction = inputActions.None;

            if (pageObjects.emailInput().valid() == '1') {
                pageObjects.collapseBilling().parent().show();

                var showBillingInputs = $.Deferred(function () {
                    pageObjects.collapseBilling().collapse('show');
                });

                $.when(showBillingInputs.resolve()).then(function () {
                    pageObjects.fullName().focus();
                });

                pageObjects.collapseShipping().parent().show();
                $('#collapseEmail').collapse('toggle');

                pageObjects.theSubmitButton().prop('value', registerButtonText);
            }
        },

        enterDifferentAddress = function () {
            //user choose - yes that's my institution but I'd like to enter a different address'
            console.log('EnterDiffAddress hit');
            pageObjects.modalInstitution().modal('hide');
            pageObjects.wrapZip().hide('slow');

            zipCheckRequired = true;

            pageObjects.theSubmitButton().prop('value', nextButtonText);

            disregardIntitutionDomain = true;

            stateManager.action = actions.CheckEmail;

            //    Set to none because flow ends here for that stage
            stateManager.inputAction = inputActions.None;

            var showEmailInput = $.Deferred(function () {
                pageObjects.wrapEmail().show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                pageObjects.emailInput().focus();
            });
        },

        resetPasswordOrLoginView = function (email) {
            console.log("call resetPasswordOrLoginView: " + email);
            $('#Email1').val(email);
            $('#ResetPassEmail').val(email);
            pageObjects.labelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
            pageObjects.wrapEmail().hide('slow');

            var showLoginInput = $.Deferred(function () {
                pageObjects.wrapReset().show('slow');
            });

            $.when(showLoginInput.resolve()).then(function () {
                $('#Email1').focus();
            });

            stateManager.action = actions.SubmitLogin;
            pageObjects.theSubmitButton().prop('value', 'Log In');
        },

        foundInstitutionView = function (data, email) {

            if (stateManager.inputAction.inputAction === inputActions.EnterKeyPress)
                stateManager.inputAction = inputActions.None;
            console.log("call foundInstitutionView: " + email);
            pageObjects.modalInstitution().modal('show');

            pageObjects.institution().val(data.Institution);
            pageObjects.streetAddressShipping().val(data.Address);
            pageObjects.cityShipping().val(data.City);
            pageObjects.stateShipping().val(data.State);
            pageObjects.zipShipping().val(data.Zip);
            pageObjects.streetAddressBilling().val(data.Address);
            pageObjects.cityBilling().val(data.City);
            pageObjects.stateBilling().val(data.State);
            pageObjects.zipBilling().val(data.Zip);
            pageObjects.labelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf('@')) + '</b>&nbsp; domain has been identified.</span>');
            $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
        },

        logIn = function () {

            $('form#frmSignIn').submit();
        },

        newPasswordView = function (email) {
            console.log("call newPasswordView: " + email);
            pageObjects.wrapEmail().hide('fast');

            var showPwdInput = $.Deferred(function () {
                console.log("Deferred newPasswordView: " + email);
                pageObjects.wrapPass().show('fast');
            });

            $.when(showPwdInput.resolve()).then(function () {
                pageObjects.registerFieldsPassword().focus();
            });



            if (email) {
                pageObjects.labelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
            } else {
                pageObjects.modalInstitution().modal('hide');
                pageObjects.labelEmail().fadeOut(500, function () {
                    $(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + pageObjects.emailInput().val() + ' will be used for your email address.</span>');
                    $(this).fadeIn(500);
                });
            }

            stateManager.action = actions.GetPassword;

            if (stateManager.inputAction === inputActions.EnterKeyPress)
                stateManager.inputAction = inputActions.None;

            pageObjects.theSubmitButton().on('mouseenter', function () {
                if ($('#sameAsBilling:checked').val()) {
                    setShippingToBilling();
                }
            });
        },

        newPassWordToNextStep = function () {

            var confirmPasswordInput = $('[name="RegisterFields.ConfirmPassword"]');
            var pwd = $.trim(pageObjects.registerFieldsPassword().val());
            var confirmedPwd = $.trim(confirmPasswordInput.val());

            if (!confirmedPwd) {
                confirmPasswordInput.next('span').removeAttr('class').attr('class','field-validation-error').append('<span>Passwors don\'t match.</span>');
            }

            if (!pwd || !confirmedPwd || pwd.length < 2 || pageObjects.registerFieldsPassword().nextAll('span:last').hasClass('field-validation-error')) {
                console.log('call RegisterFields_Password');
                if (stateManager.inputAction === inputActions.EnterKeyPress)
                    stateManager.inputAction = inputActions.None;
                return false;
            }

            if (zipCheckRequired) {
                pageObjects.wrapPass().hide('slow');
                stateManager.action = actions.CheckZip;

                var showGetZipInput = $.Deferred(function () {
                    pageObjects.wrapZip().show('slow');
                });

                $.when(showGetZipInput.resolve()).then(function () {
                    $('#getZip').focus();
                });

                if (stateManager.inputAction === inputActions.EnterKeyPress)
                    stateManager.inputAction = inputActions.None;

            } else {
                displayBillingFields();

                //TODO: does it make sense to fire a formfield validation at this point?
                stateManager.action = actions.SubmitRegister;
                zipCheckRequired = true;
            }
        },

        notInstitutionAddress = function () {
            console.log('NotInstitution hit');
            pageObjects.modalInstitution().modal('hide');

            var showEmailInput = $.Deferred(function () {
                pageObjects.wrapEmail().show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                pageObjects.emailInput().focus();
            });


            pageObjects.wrapZip().hide('slow');
            pageObjects.labelEmail().html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            //  Clear the billing and shipping addresses
            $('#collapseBilling input').val('');
            $('#collapseBilling textarea').val('Mailing address notes:');
            $('#collapseBilling select').val(0);

            $('#collapseShipping input:not("#sameAsBilling")').val('');
            $('#collapseShipping textarea').val('Mailing address notes:');
            $('#collapseShipping select').val(0);

            pageObjects.theSubmitButton().prop('value', nextButtonText);

            disregardIntitutionDomain = true;

            stateManager.action = actions.CheckEmail;

            if (stateManager.inputAction === inputActions.EnterKeyPress)
                stateManager.inputAction = inputActions.None;
        },

        passResetView = function () {
            console.log("call passResetView");
            pageObjects.login().hide('slow');


            var showResetInput = $.Deferred(function () {
                //TODO: inprocess indicator here?
                pageObjects.reset().show('slow');
            });

            $.when(showResetInput.resolve()).then(function () {
                $('#ResetPassEmail').focus();
            });

        },
        registerView = function () {
            pageObjects.login().hide('slow');
            pageObjects.register().show('slow');
            stateManager.action = actions.CheckEmail;

            var showRegisterInput = $.Deferred(function () {
                pageObjects.register().show('slow');
            });

            $.when(showRegisterInput.resolve()).then(function () {
                pageObjects.emailInput().focus();
            });
        },

        resetPassword = function (normalResetPasswordButton) {
            console.log('resetPass hit');
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('form#ResetPasswordForm').submit();
        },

        startView = function () {
            pageObjects.register().hide();
            pageObjects.reset().hide();
            $('#nonUSAddressInput').hide();
        },

        useRegisteredAddress = function () {
            console.log('YesUseAddress hit');

            zipCheckRequired = false;

            stateManager.showNewPasswordInputs();
        },
        nonUsAdddressInvoked = function () {
            console.log("hit nonUsAdddressInvoked");
            pageObjects.labelEmail().html('<span class="label label-info"><b>&nbsp;&nbsp;Non-US address? Please enter Special Handling Instructions</span>');
            pageObjects.zipBilling().val('na');
            $('#nonUSAddressInput').show();
            displayBillingFields();
            stateManager.action = actions.SubmitRegister;
        },

        zipCodeVerified = function (data, zipCode) {

            switch (data.success) {
                case 'true':
                    console.log('zipCodeVerified-true hit');
                    pageObjects.zipBilling().val(zipCode);
                    stateManager.action = actions.SubmitRegister;
                    enterBillingPane(data);
                    break;
                case 'false':
                    console.log('zipCodeVerified-false hit');
                    pageObjects.labelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                    $('#nonUSAddressBtn').show('slow');
                    break;
                case 'invalid format':
                    console.log('zipCodeVerified-invalidformat hit');
                    pageObjects.labelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                    break;
            }

        },

        submit = function () {

            switch (stateManager.action) {
                case actions.CheckEmail:
                    console.log('CheckEmail hit');
                    checkAndSubmitEmail();
                    if (!stateManager.disregardIntitutionDomain)
                        stateManager.disregardIntitutionDomain = true;
                    pageObjects.theSubmitButton().prop('value', nextButtonText);
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
                    stateManager.action = actions.PostCreateAccount;
                    if (pageObjects.createUserForm().valid() == '1') {
                        submitCreateUserForm();
                    }
                    break;
                case actions.SubmitLogin:
                    console.log('submitLogin hit');
                    $('#Password').val($('#Password1').val());
                    pageObjects.emailLoginInput().val($('#Email1').val());
                    $('form#frmSignIn').submit();
                    break;
                case actions.DisplayBillingAddressFields:
                    displayBillingFields();
                    break;
            }
        },

        init = function (args) {
            stateManager.action = actions.LogIn;
            stateManager.inputAction = inputActions.None;
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
        logIn: logIn,
        newPassWordToNextStep: newPassWordToNextStep,
        nonUsAdddressInvoked: nonUsAdddressInvoked,
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
    if (pageObjects.emailInput().valid() == '1') {
        console.log(pageObjects.emailInput().valid());
        $('#emailAddress').val($('#checkEmail').val());
        $('form#checkEmail').submit();
    }
}

function checkAndSubmitZip() {
    pageObjects.theSubmitButton().prop('value', nextButtonText);
    console.log('checkAndSubmitZip');
    //$('#emailAddress').val($('#checkEmail').val());
    $('#checkZip').submit();
}


function submitCreateUserForm() {
    
    var valid = pageObjects.createUserForm().valid();

    //on account creation default the shipping phone to be same as billing
    $("#RegisterFields.ShippingAddress.phone").val(phoneBilling);
    console.log("validating createUserForm: " + valid);

    if (valid) {
        console.log('createUserForm submitted.');
        pageObjects.createUserForm().submit();

        pageObjects.theSubmitButton().off('mouseenter', function () {
            if ($('#sameAsBilling:checked').val()) {
                setShippingToBilling();
            }
        });
    }
}

function submitLogin() {
    //pageObjects.emailLoginInput().val($('#RegisterFields_Email').val());
    //$('#Password').val($('#Password1').val());
    //$('#frmSignin').submit();
    $('#Password').val(pageObjects.registerFieldsPassword().val());
    pageObjects.emailLoginInput().val($('#emailAddress').val());
    $('form#frmSignIn').submit();
}



function setShippingToBilling() {
    pageObjects.fullNameShipping().val(pageObjects.fullName().val());
    pageObjects.shippingFirstName().val(pageObjects.firstName().val());
    pageObjects.shippingLastName().val(pageObjects.lastName().val());
    pageObjects.cityShipping().val(pageObjects.cityBilling().val());
    pageObjects.stateShipping().val(pageObjects.stateBilling().val());
    pageObjects.streetAddressShipping().val(pageObjects.streetAddressBilling().val());
    pageObjects.streetAddressShipping2().val(pageObjects.streetAddressBilling2().val());
    pageObjects.zipShipping().val(pageObjects.zipBilling().val());
    $('#RegisterFields_ShippingAddress_Country').val($('#RegisterFields_BillingAddress_Country').val());
    pageObjects.phoneShipping().val(pageObjects.phoneBilling().val());
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

function initializeState() {

    collapseBilling = $('#collapseBilling');
    collapseShipping = $('#collapseShipping');

    collapseBilling.parent().hide();
    collapseShipping.parent().hide();

    stateManager.initialize();

    register = $('#register');
    reset = $('#reset');

    wrapEmail = $('#wrapEmail');
    wrapPass = $('#wrapPass');
    wrapReset = $('#wrapReset');
    wrapZip = $('#wrapZip');

    pageObjects.wrapZip().hide();
    pageObjects.wrapPass().hide();
    pageObjects.wrapReset().hide();
    $('#nonUSAddress').hide();

    getFirstLast = $('#getFirstLast');
    pageObjects.getFirstLast().hide();

    theSubmitButton = $('#TheSubmitButton');
    emailInput = $('#RegisterFields_Email');
    createUserForm = $('#_CreateUserForm');
    labelEmail = $('#labelEmail');

    phoneBilling = $('#RegisterFields_BillingAddress_Phone');
    streetAddressBilling = $('#RegisterFields_BillingAddress_StreetAddress');
    streetAddressBilling2 = $('#RegisterFields_BillingAddress_StreetAddress2');
    cityBilling = $('#RegisterFields_BillingAddress_City');
    stateBilling = $('#RegisterFields_BillingAddress_State');
    zipBilling = $('#RegisterFields_BillingAddress_Zip');
    phoneShipping = $('#RegisterFields_ShippingAddress_Phone');
    fullNameShipping = $('#FullNameShipping');
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
    emailLoginInput = $('#Email');

    pageObjects.emailLoginInput().focus();
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

    initializeState();

    var path = setPath();

    pageObjects.emailInput().bind('change keyup', function () {
        if ($(this).validate().checkForm()) {
            pageObjects.theSubmitButton().removeClass('button_disabled').attr('disabled', false);
        } else {
            pageObjects.theSubmitButton().addClass('button_disabled').attr('disabled', true);
        }
    });

    pageObjects.theSubmitButton().prop('value', nextButtonText);

    $('[name="RegisterFields.ConfirmPassword"]').on('focus', function(event) {
        $(this).next('span').removeAttr('class').attr('class', 'field-validation-valid');
        $(this).next('span span').remove();
    });

    $('input').keypress(function (event) {

        var inputElementTriggered = event.currentTarget.name;
        var normalResetPasswordButton = $('#NormalResetPasswordButton');

        if (normalResetPasswordButton.filter(':visible').length > 0)
            inputElementTriggered = 'NormalResetPasswordInput';

        if (event.which == 13) {

            if (pageObjects.modalInstitution().filter(':visible').length > 0
                && inputElementTriggered !== buttons.YesUseAddress
                && inputElementTriggered !== buttons.EnterDiffAddress
                && inputElementTriggered !== buttons.NotInstitution) {
                return false;
            }

            stateManager.inputAction = inputActions.EnterKeyPress;

            switch (inputElementTriggered) {
                case 'Password':
                case 'Email':
                case buttons.SignInButton:
                    stateManager.logIn();
                    break;
                case 'RegisterFields.Password':
                case 'RegisterFields.ConfirmPassword':
                case 'RegisterFields.Email':
                case 'getZip':
                case 'Password1':
                case 'Email1':
                case buttons.TheSubmit:
                    console.log('ActionForTheSubmit = ' + stateManager.action);
                    stateManager.submit(); break;
                case buttons.nonUSAddressBtn: stateManager.nonUsAdddressInvoked(); break;
                case 'NormalResetPasswordInput':
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
                    if (pageObjects.theSubmitButton().val() === registerButtonText) {
                        if ($('#sameAsBilling:checked').val()) {
                            setShippingToBilling();
                        }
                        stateManager.action = actions.SubmitRegister;
                        stateManager.submit();
                    }
            }

            console.log(stateManager.action);
            event.preventDefault();

        }
    });


    $('body').on('click', 'input:button', (function (e, data) {

        if (stateManager.inputAction === inputActions.EnterKeyPress)
            return false;

        stateManager.inputAction = inputActions.ButtonClick;

        if (stateManager.action === '') {
            pageObjects.labelEmail().html('<span class="label label-important">&nbsp;&nbsp;There registration has encountered a problem. Please refresh the page and re-start the registration process or call Tech Support at 800-831-0678 ext. 706.</span>');

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
            case buttons.SignInButton: stateManager.logIn(); break;
            case buttons.TheSubmit:
                console.log('ActionForTheSubmit = ' + stateManager.action);
                stateManager.submit(); break;
            case buttons.nonUSAddressBtn: stateManager.nonUsAdddressInvoked(); break;
            case buttons.ResetPass: stateManager.resetPassword(normalResetPasswordButton); break;
            case buttons.YesUseAddress: stateManager.useRegisteredAddress(); break;
            case buttons.EnterDiffAddress: stateManager.enterDifferentAddress(); break;
            case buttons.NotInstitution: stateManager.notInstitutionAddress(); break;
            default:
        }
    }));


    pageObjects.collapseShipping().on('shown', function () {
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
    //            //data: { zip: pageObjects.zipBilling().val()},
    //            //beforeSend: function () {
    //            //    // this is where we append a loading image
    //            //    pageObjects.labelEmail().html('<div class="btn-warning style='width: 400px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</div>');
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
    //        pageObjects.institution().val(ui.item.Work_Item);
    //        return false;
    //    }
    //});

    //pageObjects.createUserForm().on('submit', function (e) {
    //    e.preventDefault();

    //    if (stateManager.action !== actions.SubmitLogin) {
    //        console.log('non-valid form');
    //        return false;
    //    }

    //    if (pageObjects.createUserForm().valid() != '1') {
    //        console.log('non-valid form');
    //        return false;
    //    }

    //    var url = '/Account/Register';
    //    console.log('Submitting Register Details');
    //    alert("startSubmitting");
    //    $.ajax({
    //        type: 'POST',
    //        contentType: constants.FormPostContentType,
    //        cache: false,
    //        url: url,
    //        dataType: constants.JsonDataType,
    //        data: $(this).serialize(),
    //        beforeSend: function () {
    //            console.log('beforeSend Register Details!!!');
    //            // this is where we append a loading image
    //            pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Registering new user...</span>');
    //        }
    //    }).done(function (data) {
    //        alert("datareturned");// never fires!
    //        console.log('done Register Details');
    //        if (data.Status == 'Success') {
    //            console.log('success  Register Details');
    //            stateManager.action = '';
    //            pageObjects.labelEmail().html('<span class="label label-success">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;You have successfully registered! Please wait while we log you in...</span>');
    //            //location.assign(path + '/'); //TODO: we need to implement a pattern where if 'returnURL' (server-side origin) is populated, it's used. Otherwise return to home page.
    //            location.href = '/Home';
    //        } else if (data.Status === 'Fail') {
    //            console.log('statusFail  Register Details');
    //            pageObjects.labelEmail().html('<span class="label label-information">&nbsp;&nbsp;There has been an error in the request. Please try again or call tech support at 800-831-0678 ext 706.</span>');
    //        }
    //    }).fail(function (data) {
    //        console.log('failed: ' + data);
    //    });
    //});

    $('form#checkEmail').submit(function (e) {

        e.preventDefault();
        var jsonUrl = '/Account/CheckEmail';
        var email = pageObjects.emailInput().val();
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
                    pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
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
                pageObjects.wrapEmail().html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                stateManager.inputAction = inputActions.None;
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
                contentType: constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: constants.JsonDataType,
                data: { Zip: zipCode },
                beforeSend: function () {
                    // this is where we append a loading image
                    pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Zip...</span>');
                }
            }).done(function (data) {
                // successful request; do something with the data
                stateManager.zipCodeVerified(data, zipCode);
            }).fail(function () {
                // failed request; give feedback to user
                pageObjects.wrapZip().html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            }).always(function () {
                stateManager.inputAction = inputActions.None;
            });
        }
        return false;
    });

    //pageObjects.institution().change(function () {
    //    alert('hit' + pageObjects.institution().val());
    //    //  $.getJSON()
    //    $('#InstitutionChecker').val(pageObjects.institution().val());
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




    pageObjects.modalInstitution().on('hidden', function (e) {
        stateManager.inputAction = inputActions.None;
        if (pageObjects.wrapZip().is(':visible')) {
            stateManager.action = actions.CheckEmail;
        }

    });



});