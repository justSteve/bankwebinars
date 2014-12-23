/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
var RegistrationInCart;
(function (RegistrationInCart) {
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
    RegistrationInCart.Button = Button;
    ;

    var InputAction = (function () {
        function InputAction() {
        }
        InputAction.EnterKeyPress = 'EnterKeyPress';
        InputAction.ButtonClick = 'ButtonClick';
        InputAction.None = 'None';
        return InputAction;
    })();
    RegistrationInCart.InputAction = InputAction;
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
    RegistrationInCart.Action = Action;
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
    RegistrationInCart.Constants = Constants;
    ;

    var StateManager = (function () {
        function StateManager() {
            this.disregardInstitutionDomain = false;
            this.formParsedByValidator = false;
            this.zipCheckRequired = false;
            this.nextButtonText = 'Next...';
            this.registerButtonText = 'Submit Register';
            this.sameAsBillingCheckedFilter = '#sameAsBilling:checked';
            this.initialize();
        }
        StateManager.prototype.initializeState = function () {
            $('#sameAsBilling').attr('checked', 'checked'); // So shipping address fields same as billing address fields by default.
            $('#collapseBilling').parent().hide();
            $('#collapseShipping').parent().hide();

            $('#wrapZip').hide();

            $('#wrapReset').hide();

            $('#nonUSAddress').hide();

            $('#getFirstLast').hide();

            this.typeofAddressShipping = $('#RegisterFields_ShippingAddress_TypeOfAddress').val(Constants.TypeofAddressShipping);
            this.typeofAddressBilling = $('#RegisterFields_BillingAddress_TypeOfAddress').val(Constants.TypeofAddressBilling);

            $('#RegisterFields_Email').focus();
        };

        StateManager.prototype.checkAndSubmitEmail = function () {
            this.ensureFormValidatorParsed();
            if ($('#RegisterFields_Email').valid() == true) {
                //console.log(REG.PageObjects.emailInput().valid());
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

                if (this.isShippindAddressRequired)
                    $('#collapseShipping').parent().show();

                $('#collapseEmail').collapse('toggle');

                $('#TheSubmitButton').prop('value', this.registerButtonText);
            }

            //  set TimeZone to Central time if there is none.
            var timeZoneInput = $('#TimeZone');

            if (!timeZoneInput.val())
                timeZoneInput.val('3'); // Central = 3
        };

        StateManager.prototype.ensureFormValidatorParsed = function () {
            if (!this.formParsedByValidator) {
                $.validator.unobtrusive.parse($('#_CreateUserFromCartForm'));
                this.formParsedByValidator = true;
            }
        };

        StateManager.prototype.enterBillingPane = function (data) {
            $('#collapseBilling').parent().show();

            if (this.isShippindAddressRequired)
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

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;
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
            $('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf('@')) + '</b>&nbsp; domain has been identified.</span>');
            $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
            $('input[name=YesUseAddress]').focus();
        };

        StateManager.prototype.getAction = function () {
            return this.action;
        };

        StateManager.prototype.getDisregardIntitutionDomain = function () {
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
            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            $('form#frmSignIn').submit();
        };

        StateManager.prototype.goToAddressFields = function (email) {
            ////console.log("call newPasswordView: " + email);
            if (email)
                $('#labelEmail').html('<span class="label label-success">&nbsp;&nbsp;' + $('#RegisterFields_Email').val() + ' will be used for your email address.</span>');

            if (this.zipCheckRequired) {
                this.action = Action.CheckZip;

                $('#wrapEmail').hide('fast');

                var showGetZipInput = $.Deferred(function () {
                    $('#wrapZip').show('slow');
                });

                $.when(showGetZipInput.resolve()).then(function () {
                    $('#getZip').focus();
                });

                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
            } else {
                $('#wrapEmail').hide('fast');

                this.action = Action.SubmitRegister;

                this.displayBillingFields();

                this.zipCheckRequired = true;

                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;

                $('#TheSubmitButton').on('mouseenter', function () {
                    if ($(this.sameAsBillingCheckedFilter).val()) {
                        $('#SetShippingToBilling');
                    }
                });
            }
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
            console.log("call passResetView");
            $('#login').hide('slow');

            var showResetInput = $.Deferred(function () {
                //TODO: inprocess indicator here?
                $('#reset').show('slow');
            });

            $.when(showResetInput.resolve()).then(function () {
                $('#ResetPassEmail').focus();
            });
        };

        StateManager.prototype.registerView = function () {
            $('#login').hide('slow');

            this.action = Action.CheckEmail;

            var showRegisterInput = $.Deferred(function (pageObject) {
                $('#register').show('slow');
            });

            $.when(showRegisterInput.resolve()).then(function () {
                $('#RegisterFields_Email').focus();
            });
        };

        StateManager.prototype.resetPassword = function (normalResetPasswordButton) {
            console.log('resetPass hit');
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('#ResetPassEmail').val($('#Email1').val());
            $('form#ResetPasswordForm').submit();
        };

        StateManager.prototype.resetPasswordOrLoginView = function (email, webinarId) {
            console.log("call resetPasswordOrLoginView: " + email);
            $('#Email1').val(email);
            $('#ResetPassEmail').val(email);
            $('#labelEmail').html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
            $('#wrapEmail').hide('slow');

            var showLoginInput = $.Deferred(function () {
                $('#wrapReset').show('slow');
            });

            $.when(showLoginInput.resolve()).then(function () {
                $('#Email1').focus();
            });

            this.action = Action.SubmitLogin;
            $('#TheSubmitButton').prop('value', 'Log In');

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;
        };

        StateManager.prototype.setAction = function (incomingAction) {
            this.action = incomingAction;
        };

        StateManager.prototype.setInputAction = function (incomingInputAction) {
            this.inputAction = incomingInputAction;
        };

        StateManager.prototype.setIsShippindAddressRequired = function (isShippindAddressRequired) {
            this.isShippindAddressRequired = isShippindAddressRequired;
        };

        StateManager.prototype.setShippingToBilling = function () {
            var firstName = $('#RegisterFields_FirstName').val();
            var lastName = $('#RegisterFields_LastName').val();

            if ($('#RegisterFields_LastName').val()) {
                $('#ShippingFirstName').val(firstName);
                $('#ShippingLastName').val(lastName);
                $('#FirstName').val(firstName);
                $('#LastName').val(lastName);
                $('#FullNameShipping').val(firstName + ' ' + lastName);
            } else {
                $('#FullNameShipping').val($('#FullName').val());
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
            $('#register').hide();
            $('#reset').hide();
            $('#nonUSAddressInput').hide();
            $('#nonUSAddressBtn').hide();
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
                case Action.CheckZip:
                    //console.log('CheckZip hit');
                    $('#ZipChecker').val($('#getZip').val());
                    $('form#checkZip').submit();
                    break;
                case Action.SubmitRegister:
                    //console.log('SubmitRegister hit');
                    if ($('#_CreateUserFromCartForm').valid() == true) {
                        this.action = Action.PostCreateAccount;
                        this.submitCreateUserForm();
                    } else {
                        if (this.inputAction === InputAction.EnterKeyPress)
                            this.inputAction = InputAction.None;
                    }
                    break;
                case Action.SubmitLogin:
                    //console.log('submitLogin hit');
                    if (this.inputAction === InputAction.EnterKeyPress)
                        this.inputAction = InputAction.None;
                    $('#Password').val($('#Password1').val());
                    $('#Email').val($('#Email1').val());
                    $('form#frmSignIn').submit();
                    break;
                case Action.DisplayBillingAddressFields:
                    this.displayBillingFields();
                    break;
            }
        };

        StateManager.prototype.submitCreateUserForm = function () {
            var valid = $('#_CreateUserFromCartForm').valid();

            // on account creation default the shipping phone to be same as billing
            $("#RegisterFields.ShippingAddress.phone").val($('#RegisterFields_BillingAddress_Phone'));

            //console.log("validating createUserForm: " + valid);
            if (valid) {
                //console.log('createUserForm submitted.');
                $('#_CreateUserFromCartForm').submit();

                $('#TheSubmitButton').off('mouseenter');
            }
        };

        StateManager.prototype.submitLogin = function () {
            //REG.PageObjects.emailLoginInput().val($('#RegisterFields_Email').val());
            //$('#Password').val($('#Password1').val());
            //$('#frmSignin').submit();
            $('#Password').val($('#RegisterFields_Password').val());
            $('#Email').val($('#emailAddress').val());
            $('form#frmSignIn').submit();
        };

        StateManager.prototype.useRegisteredAddress = function () {
            //console.log('YesUseAddress hit');
            $('#modalInstitution').modal('hide');

            this.zipCheckRequired = false;

            this.goToAddressFields(null);
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
    RegistrationInCart.StateManager = StateManager;
    ;
})(RegistrationInCart || (RegistrationInCart = {}));
//# sourceMappingURL=register-user-in-cart.js.map
