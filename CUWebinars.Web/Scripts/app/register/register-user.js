/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
var Registration;
(function (Registration) {
    var Button = (function () {
        function Button() {
        }
        Button.EnterDiffAddress = 'EnterDiffAddress';
        Button.nonUSAddressBtn = 'nonUSAddressBtn';
        Button.NotInstitution = 'NotInstitution';
        Button.ResetPass = 'resetPass';
        Button.SignInButton = 'SignInButton';
        Button.TheSubmit = 'TheSubmit';
        Button.YesUseAddress = 'YesUseAddress';
        return Button;
    })();
    Registration.Button = Button;
    ;

    var InputAction = (function () {
        function InputAction() {
        }
        InputAction.EnterKeyPress = 'EnterKeyPress';
        InputAction.ButtonClick = 'ButtonClick';
        InputAction.None = 'None';
        return InputAction;
    })();
    Registration.InputAction = InputAction;
    ;

    var Action = (function () {
        function Action() {
        }
        Action.CheckEmail = 'CheckEmail';
        Action.CheckZip = 'CheckZip';
        Action.GetPassword = 'GetPassword';
        Action.LogIn = 'LogIn';
        Action.PostCreateAccount = 'PostCreateAccount';
        Action.SubmitLogin = 'SubmitLogin';
        Action.SubmitRegister = 'SubmitRegister';
        Action.DisplayBillingAddressFields = 'DisplayBillingAddressFields';
        return Action;
    })();
    Registration.Action = Action;
    ;

    var Constants = (function () {
        function Constants() {
        }
        Constants.BillingAddressFields = '#RegisterFields_BillingAddress';
        Constants.City = '_City';
        Constants.Country = '_Country';
        Constants.ConfirmDeleteShippingAddressdialog = '#ConfirmDeleteShippingAddressdialog';
        Constants.FormPostContentType = 'application/x-www-form-urlencoded';
        Constants.JsonContentType = 'application/json; charset=utf-8';
        Constants.JsonDataType = 'json';
        Constants.HtmlDataType = 'html';
        Constants.Phone = '_Phone';
        Constants.ShippingAddressFields = '#RegisterFields_ShippingAddress';
        Constants.ShippingAddressContainer = '#ShippingAddressContainer';
        Constants.State = '_State';
        Constants.AddShippingAddressLink = '#AddShippingAddressLink';
        Constants.HideAddShippingAddressLink = '#HideAddShippingAddressLink';
        Constants.StreetAddress = '_StreetAddress';
        Constants.StreetAddress2 = '_StreetAddress2';
        Constants.TypeofAddressBilling = 'Billing';
        Constants.TypeofAddressShipping = 'Shipping';
        Constants.Zip = '_Zip';
        return Constants;
    })();
    Registration.Constants = Constants;
    ;

    var StateManager = (function () {
        function StateManager() {
            this.disregardInstitutionDomain = false;
            this.zipCheckRequired = false;
            this.nextButtonText = 'Next...';
            this.registerButtonText = 'Submit';
            this.sameAsBillingCheckedFilter = '#sameAsBilling:checked';
            this.initialize();
        }
        StateManager.prototype.initializeState = function () {
            $('#collapseBilling').parent().hide();
            $('#collapseShipping').parent().hide();

            $('#wrapZip').hide();
            $('#wrapPass').hide();
            $('#wrapReset').hide();

            //$('#nonUSAddress').hide();

            $('#getFirstLast').hide();

            this.typeofAddressShipping = $('#RegisterFields_ShippingAddress_TypeOfAddress').val(Constants.TypeofAddressShipping);
            this.typeofAddressBilling = $('#RegisterFields_BillingAddress_TypeOfAddress').val(Constants.TypeofAddressBilling);
        };

        StateManager.prototype.checkAndSubmitEmail = function () {
            if ($('#RegisterFields_Email').valid() == true) {
                

                $('#emailAddress').val($('#checkEmail').val());
                $('form#checkEmail').submit();
                return true;
            } else {
                this.inputAction = InputAction.None;
                return false;
            }
        };

        StateManager.prototype.checkAndSubmitZip = function () {
            $('#TheSubmitButton').prop('value', this.nextButtonText);

            //console.log('checkAndSubmitZip');
            //$('#emailAddress').val($('#checkEmail').val());
            $('#checkZip').submit();
        };

        StateManager.prototype.displayBillingFields = function () {
            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            if ($('#RegisterFields_Email').valid() == true) {
                $('#collapseBilling').parent().show();

                var showBillingInputs = $.Deferred(function () {
                    $('#collapseBilling').collapse('show');
                });

                $.when(showBillingInputs.resolve()).then(function () {
                    $('#FullName').focus();
                });

                $('#collapseShipping').parent().show();
                $('#collapseEmail').collapse('toggle');

                $('#TheSubmitButton').prop('value', this.registerButtonText);
            }
        };

        StateManager.prototype.enterBillingPane = function (data) {
            $('#collapseBilling').parent().show();
            $('#collapseShipping').parent().show();
            $('#collapseEmail').collapse('toggle');

            var showBillingInputs = $.Deferred(function () {
                $('#collapseBilling').collapse('toggle');
            });

            $.when(showBillingInputs.resolve()).then(function () {
                $('#FullName').focus();
            });

            $('#RegisterFields_BillingAddress_City').val(data.City);
            $('#RegisterFields_BillingAddress_State').val(data.State);

            $('#TimeZone').val(data.TimeZone);

            $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
            $('#TheSubmitButton').prop('value', this.registerButtonText);
        };

        StateManager.prototype.enterDifferentAddress = function () {
            //user choose - yes that's my institution but I'd like to enter a different address'
            ////console.log('EnterDiffAddress hit');
            $('#modalInstitution').modal('hide');
            $('#wrapZip').hide('slow');

            this.zipCheckRequired = true;

            $('#TheSubmitButton').prop('value', this.nextButtonText);

            this.disregardInstitutionDomain = true;

            this.action = Action.CheckEmail;

            //    Set to none because flow ends here for that stage
            this.inputAction = InputAction.None;

            var showEmailInput = $.Deferred(function () {
                $('#wrapEmail').show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                $('#RegisterFields_Email').focus();
            });
        };

        StateManager.prototype.foundInstitutionView = function (data, email) {
            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            ////console.log("call foundInstitutionView: " + email);
            $('#modalInstitution').modal('show');

            $('#RegisterFields_Institution').val(data.Institution);
            $('#RegisterFields_ShippingAddress_StreetAddress').val(data.Address);
            $('#RegisterFields_ShippingAddress_City').val(data.City);
            $('#RegisterFields_ShippingAddress_State').val(data.State);
            $('#RegisterFields_ShippingAddress_Zip').val(data.Zip);
            $('#RegisterFields_BillingAddress_StreetAddress').val(data.Address);
            $('#RegisterFields_BillingAddress_City').val(data.City);
            $('#RegisterFields_BillingAddress_State').val(data.State);
            $('#RegisterFields_BillingAddress_Zip').val(data.Zip);
            $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;Recorded: ' + email + '</b>&nbsp;</span>');
            $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
            $('input[name=YesUseAddress]').focus();
        };

        StateManager.prototype.getAction = function () {
            return this.action;
        };

        StateManager.prototype.getDisregardInstitutionDomain = function () {
            return this.disregardInstitutionDomain;
        };

        StateManager.prototype.getInputAction = function () {
            return this.inputAction;
        };

        StateManager.prototype.getRegisterButtonText = function () {
            return this.registerButtonText;
        };

        StateManager.prototype.getSameAsBillingCheckedFilter = function () {
            return this.sameAsBillingCheckedFilter;
        };

        StateManager.prototype.initialize = function () {
            this.action = Action.LogIn;
            this.inputAction = InputAction.None;
            this.disregardInstitutionDomain = false;
            this.zipCheckRequired = true;
            this.startView();

            $('#TheSubmitButton').prop('value', this.nextButtonText);
        };

        StateManager.prototype.logIn = function () {
            $('form#frmSignIn').submit();
        };

        StateManager.prototype.newPasswordView = function (email) {
            ////console.log("call newPasswordView: " + email);
            $('#wrapEmail').hide('fast');

            var self = this;

            if (email) {
                $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
            } else {
                $('#modalInstitution').modal('hide');
                $('#labelEmail').fadeOut(500, function () {
                    $(this).html('<span class="label label-success">&nbsp;&nbsp;Recorded: ' + $('#RegisterFields_Email').val() + '</span>');
                    $(this).fadeIn(500);
                });
            }

            this.action = Action.GetPassword;

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            var showPwdInput = $.Deferred(function () {
                //console.log("Deferred newPasswordView: " + email);
                $('#wrapPass').show('fast');
            });

            $.when(showPwdInput.resolve()).then(function () {
                $('#RegisterFields_Password').focus();
            });
        };

        StateManager.prototype.newPassWordToNextStep = function () {
            var confirmPasswordInput = $('[name="RegisterFields.ConfirmPassword"]');
            var pwd = $.trim($('#RegisterFields_Password').val());
            var confirmedPwd = $.trim(confirmPasswordInput.val());

            if (!confirmedPwd || pwd !== confirmedPwd) {
                confirmPasswordInput.next('span').removeAttr('class').attr('class', 'field-validation-error').append('<span>Passwords don\'t match.</span>');
                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
                return false;
            }

            if (!pwd || pwd.length < 2 || $('#RegisterFields_Password').nextAll('span:last').hasClass('field-validation-error')) {
                //console.log('call RegisterFields_Password');
                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
                return false;
            }

            if (this.zipCheckRequired) {
                $('#wrapPass').hide('slow');
                this.action = Action.CheckZip;

                var showGetZipInput = $.Deferred(function () {
                    $('#wrapZip').show('slow');
                });

                $.when(showGetZipInput.resolve()).then(function () {
                    $('#getZip').focus();
                });

                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
            } else {
                this.displayBillingFields();

                //TODO: does it make sense to fire a formfield validation at this point?
                this.action = Action.SubmitRegister;
                this.zipCheckRequired = true;
            }

            return true;
        };

        StateManager.prototype.nonUsAdddressInvoked = function () {
            //console.log("hit nonUsAdddressInvoked");
            $('#labelEmail').html('<span class="label label-info"><b>&nbsp;&nbsp;Non-US address? Please enter Special Handling Instructions</span>');
            $('#RegisterFields_BillingAddress_Zip').val('na');
            $('#nonUSAddressInput').show();
            this.displayBillingFields();
            this.action = Action.SubmitRegister;
        };

        StateManager.prototype.notInstitutionAddress = function () {
            //console.log('NotInstitution hit');
            $('#modalInstitution').modal('hide');

            var showEmailInput = $.Deferred(function () {
                $('#wrapEmail').show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                $('#RegisterFields_Email').focus();
            });

            $('#wrapZip').hide('slow');
            $('#labelEmail').html('<span class="label label-info">&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            //  Clear the billing and shipping addresses
            $('#collapseBilling input').val('');
            $('#collapseBilling textarea').val('Mailing address notes:');
            $('#collapseBilling select').val(0);

            $('#collapseShipping input:not("#sameAsBilling")').val('');
            $('#collapseShipping textarea').val('Mailing address notes:');
            $('#collapseShipping select').val(0);

            $('#TheSubmitButton').prop('value', this.nextButtonText);

            this.disregardInstitutionDomain = true;

            this.action = Action.CheckEmail;

            $('#RegisterFields_BillingAddress_TypeOfAddress').val(Constants.TypeofAddressBilling);
            $('#RegisterFields_ShippingAddress_TypeOfAddress').val(Constants.TypeofAddressShipping);

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;
        };

        StateManager.prototype.passResetView = function () {
            //console.log("call passResetView");
            this.inputAction = InputAction.None;

            $('#login').hide('slow');

            var showResetInput = $.Deferred(function () {
                //TODO: inprocess indicator here?
                $('#reset').show('slow');
            });

            $.when(showResetInput.resolve()).then(function () {
                $('#ResetPassEmail').val($('#Email').val());
                $('#ResetPassEmail').focus();
            });
        };

        StateManager.prototype.findLink = function () {
            //console.log("call passResetView");
            
            this.inputAction = InputAction.None;

            $('#login').hide('slow');

            var showFindLinkInput = $.Deferred(function () {
                $('#findLinkPartial').show('slow');
            });

            $.when(showFindLinkInput.resolve()).then(function () {
                $('#FindLinkByEmail').val($('#Email').val());
                $('#FindLinkByOrderId').val($('#Email').val());
                $('#FindLinkByEmail').focus();
            });
        };

        StateManager.prototype.registerView = function () {
            $('#login').hide('slow');

            this.action = Action.CheckEmail;
            this.inputAction = InputAction.None;

            var showRegisterInput = $.Deferred(function () {
                $('#register').show('slow');
            });

            $.when(showRegisterInput.resolve()).then(function () {
                $('#RegisterFields_Email').focus();
            });
        };

        StateManager.prototype.resetPassword = function (normalResetPasswordButton) {
            //console.log('resetPass hit');
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('form#ResetPasswordForm').submit();
            $('#TheSubmitButton').attr('disabled', 'disabled');
        };

        StateManager.prototype.resetPasswordOrLoginView = function (email) {
            //console.log("call resetPasswordOrLoginView: " + email);
            $('#Email1').val(email);
            $('#ResetPassEmail').val(email);
            $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is on file.</span>');
            $('#wrapEmail').hide('slow');

            var showLoginInput = $.Deferred(function () {
                $('#wrapReset').show('slow');
            });

            $.when(showLoginInput.resolve()).then(function () {
                $('#Email1').focus();
            });

            this.action = Action.SubmitLogin;
            $('#TheSubmitButton').prop('value', 'Log In');
        };

        StateManager.prototype.setAction = function (incomingAction) {
            this.action = incomingAction;
        };

        StateManager.prototype.setInputAction = function (incomingInputAction) {
            this.inputAction = incomingInputAction;
        };

        StateManager.prototype.setShippingToBilling = function () {
            var firstName = $('#RegisterFields_FirstName').val();
            var lastName = $('#RegisterFields_LastName').val();

            if (!firstName) {
                firstName = 'nonValidFirst';
            }
            if (!lastName) {
                lastName = 'nonValidLast';
            }

            if ($('#RegisterFields_LastName').val()) {
                $('#FullNameShipping').val(firstName + ' ' + lastName);
                $('#RegisterFields_ShippingAddress_Name').val(firstName + ' ' + lastName);
            } else {
                $('#FullNameShipping').val($('#FullName').val());
                $('#RegisterFields_ShippingAddress_Name').val($('#FullName').val());
            }

            $('#RegisterFields_ShippingAddress_City').val($('#RegisterFields_BillingAddress_City').val());
            $('#RegisterFields_ShippingAddress_State').val($('#RegisterFields_BillingAddress_State').val());
            $('#RegisterFields_ShippingAddress_StreetAddress').val($('#RegisterFields_BillingAddress_StreetAddress').val());
            $('#RegisterFields_ShippingAddress_StreetAddress2').val($('#RegisterFields_BillingAddress_StreetAddress2').val());
            $('#RegisterFields_ShippingAddress_Zip').val($('#RegisterFields_BillingAddress_Zip').val());
            $('#RegisterFields_ShippingAddress_Country').val($('#RegisterFields_BillingAddress_Country').val());
            $('#RegisterFields_ShippingAddress_Phone').val($('#RegisterFields_BillingAddress_Phone').val());
        };

        StateManager.prototype.startView = function () {
            $('#loginMsgLabel').hide();

            //$('#register').hide();
            //$('#reset').hide();
            $('#nonUSAddressInput').hide();
            $('#nonUSAddressBtn').show();
        };

        StateManager.prototype.submit = function () {
            switch (this.action) {
                case Action.CheckEmail:
                    //console.log('CheckEmail hit');
                    if (this.checkAndSubmitEmail()) {
                        if (!this.disregardInstitutionDomain)
                            this.disregardInstitutionDomain = true;
                        $('#TheSubmitButton').prop('value', this.nextButtonText);
                    }
                    break;
                case Action.GetPassword:
                    this.newPassWordToNextStep();
                    break;
                case Action.CheckZip:
                    //console.log('CheckZip hit');
                    $('#ZipChecker').val($('#getZip').val());
                    $('form#checkZip').submit();
                    break;
                case Action.SubmitRegister:
                    //console.log('SubmitRegister hit');
                    if ($('#_CreateUserForm').valid() == true) {
                        this.action = Action.PostCreateAccount;
                        this.submitCreateUserForm();
                    } else {
                        if (this.inputAction === InputAction.EnterKeyPress)
                            this.inputAction = InputAction.None;
                    }
                    break;
                case Action.SubmitLogin:
                    //console.log('submitLogin hit');
                    $('#Password').val($('#Password1').val());
                    $('#Email').val($('#Email1').val());
                    $('#ReturnUrl').val($('#returnUrl').val() || '/');
                    $('form#frmSignIn').submit();

                    if (this.inputAction === InputAction.EnterKeyPress)
                        this.inputAction = InputAction.None;

                    break;
                case Action.DisplayBillingAddressFields:
                    this.displayBillingFields();
                    break;
            }
        };

        StateManager.prototype.submitCreateUserForm = function () {
            var valid = $('#_CreateUserForm').valid();

            //on account creation default the shipping phone to be same as billing
            $('#RegisterFields.ShippingAddress.phone').val($('#RegisterFields_BillingAddress_Phone').val());

            //console.log("validating createUserForm: " + valid);
            if (valid) {
                //console.log('createUserForm submitted.');
                $('#_CreateUserForm').submit();

                $('#TheSubmitButton').off('mouseenter');
            }
        };

        StateManager.prototype.submitLogin = function () {
            $('#Password').val($('#RegisterFields_Password').val());
            $('#Email').val($('#emailAddress').val());
            $('form#frmSignIn').submit();
        };

        StateManager.prototype.submitResetPassword = function () {
            $('#Password').val($('#RegisterFields_Password').val());
            $('#Email').val($('#emailAddress').val());
            $('form#frmSignIn').submit();
        };

        StateManager.prototype.useRegisteredAddress = function () {
            //console.log('YesUseAddress hit');
            this.zipCheckRequired = false;

            this.newPasswordView(null);
        };

        StateManager.prototype.zipCodeVerified = function (data, zipCode) {
            switch (data.success) {
                case 'true':
                    //console.log('zipCodeVerified-true hit');
                    $('#RegisterFields_BillingAddress_Zip').val(zipCode);
                    this.action = Action.SubmitRegister;
                    this.enterBillingPane(data);
                    break;
                case 'false':
                    //console.log('zipCodeVerified-false hit');
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                    $('#nonUSAddressBtn').show('slow');
                    break;
                case 'invalid format':
                    //console.log('zipCodeVerified-invalidformat hit');
                    $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                    break;
            }
        };
        return StateManager;
    })();
    Registration.StateManager = StateManager;
    ;
})(Registration || (Registration = {}));
//# sourceMappingURL=register-user.js.map
