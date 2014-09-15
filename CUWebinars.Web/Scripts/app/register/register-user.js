/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
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

    var PageObject = (function () {
        function PageObject() {
        }
        PageObject.prototype.getTheSubmitButton = function () {
            return this.theSubmitButton || $('#TheSubmitButton');
        };
        PageObject.prototype.getCollapseEmail = function () {
            return this.collapseEmail || $('#collapseEmail');
        };
        PageObject.prototype.getEmailInput = function () {
            return this.emailInput || $('#RegisterFields_Email');
        };
        PageObject.prototype.getCityBilling = function () {
            return this.cityBilling || $('#RegisterFields_BillingAddress_City');
        };
        PageObject.prototype.getNonUSAddressBtn = function () {
            return this.nonUSAddressBtn || $('#nonUSAddressBtn');
        };
        PageObject.prototype.getnonUSAddress = function () {
            return this.nonUSAddress || $('#nonUSAddress');
        };
        PageObject.prototype.getNonUSAddressInput = function () {
            return this.nonUSAddressInput || $('#nonUSAddressInput');
        };
        PageObject.prototype.getCityShipping = function () {
            return this.cityShipping || $('#RegisterFields_ShippingAddress_City');
        };
        PageObject.prototype.getCollapseBilling = function () {
            return this.collapseBilling || $('#collapseBilling');
        };
        PageObject.prototype.getCollapseShipping = function () {
            return this.collapseShipping || $('#collapseShipping');
        };
        PageObject.prototype.getCreateUserForm = function () {
            return this.createUserForm || $('#_CreateUserForm');
        };
        PageObject.prototype.getEmailLoginInput = function () {
            return this.emailLoginInput || $('#Email');
        };
        PageObject.prototype.getFullNameShipping = function () {
            return this.fullNameShipping || $('#FullNameShipping');
        };
        PageObject.prototype.getFullName = function () {
            return this.fullName || $('#FullName');
        };
        PageObject.prototype.getFirstName = function () {
            return this.firstName || $('#FirstName');
        };
        PageObject.prototype.getGetFirstLast = function () {
            return this.getFirstLast || $('#getFirstLast');
        };
        PageObject.prototype.getInstitution = function () {
            return this.institution || $('#RegisterFields_Institution');
        };
        PageObject.prototype.getLabelEmail = function () {
            return this.labelEmail || $('#labelEmail');
        };
        PageObject.prototype.getLastName = function () {
            return this.lastName || $('#LastName');
        };
        PageObject.prototype.getLogin = function () {
            return this.login || $('#login');
        };
        PageObject.prototype.getModalInstitution = function () {
            return this.modalInstitution || $('#modalInstitution');
        };
        PageObject.prototype.getPhoneBilling = function () {
            return this.phoneBilling || $('#RegisterFields_BillingAddress_Phone');
        };
        PageObject.prototype.getPhoneShipping = function () {
            return this.phoneShipping || $('#RegisterFields_ShippingAddress_Phone');
        };
        PageObject.prototype.getRegister = function () {
            return this.register || $('#register');
        };
        PageObject.prototype.getRegisterFieldsPassword = function () {
            return this.registerFieldsPassword || $('#RegisterFields_Password');
        };
        PageObject.prototype.getReset = function () {
            return this.reset || $('#reset');
        };
        PageObject.prototype.getShippingFirstName = function () {
            return this.shippingFirstName || $('#ShippingFirstName');
        };
        PageObject.prototype.getShippingLastName = function () {
            return this.shippingLastName || $('#ShippingLastName');
        };
        PageObject.prototype.getStateBilling = function () {
            return this.stateBilling || $('#RegisterFields_BillingAddress_State');
        };
        PageObject.prototype.getStreetAddressBilling = function () {
            return this.streetAddressBilling || $('#RegisterFields_BillingAddress_StreetAddress');
        };
        PageObject.prototype.getStreetAddressBilling2 = function () {
            return this.streetAddressBilling2 || $('#RegisterFields_BillingAddress_StreetAddress2');
        };
        PageObject.prototype.getStreetAddressShipping = function () {
            return this.streetAddressShipping || $('#RegisterFields_ShippingAddress_StreetAddress');
        };
        PageObject.prototype.getStreetAddressShipping2 = function () {
            return this.streetAddressShipping2 || $('#RegisterFields_ShippingAddress_StreetAddress2');
        };
        PageObject.prototype.getStateShipping = function () {
            return this.stateShipping || $('#RegisterFields_ShippingAddress_State');
        };
        PageObject.prototype.getTypeofAddressBilling = function () {
            return this.typeofAddressBilling || $('#RegisterFields_BillingAddress_TypeOfAddress');
        };
        PageObject.prototype.getTypeofAddressShipping = function () {
            return this.typeofAddressShipping || $('#RegisterFields_ShippingAddress_TypeOfAddress');
        };
        PageObject.prototype.getWrapEmail = function () {
            return this.wrapEmail || $('#wrapEmail');
        };
        PageObject.prototype.getWrapPass = function () {
            return this.wrapPass || $('#wrapPass');
        };
        PageObject.prototype.getWrapReset = function () {
            return this.wrapReset || $('#wrapReset');
        };
        PageObject.prototype.getWrapZip = function () {
            return this.wrapZip || $('#wrapZip');
        };
        PageObject.prototype.getZipBilling = function () {
            return this.zipBilling || $('#RegisterFields_BillingAddress_Zip');
        };
        PageObject.prototype.getZipShipping = function () {
            return this.zipShipping || $('#RegisterFields_ShippingAddress_Zip');
        };

        PageObject.prototype.initializeState = function () {
            this.collapseBilling = $('#collapseBilling');
            this.collapseShipping = $('#collapseShipping');

            this.collapseBilling.parent().hide();
            this.collapseShipping.parent().hide();

            this.register = $('#register');
            this.reset = $('#reset');

            this.wrapEmail = $('#wrapEmail');
            this.wrapPass = $('#wrapPass');
            this.wrapReset = $('#wrapReset');
            this.wrapZip = $('#wrapZip');

            this.wrapZip.hide();
            this.wrapPass.hide();
            this.wrapReset.hide();

            this.nonUSAddress = $('#nonUSAddress');
            this.nonUSAddress.hide();

            this.getFirstLast = $('#getFirstLast');
            this.getFirstLast.hide();

            this.theSubmitButton = $('#TheSubmitButton');
            this.emailInput = $('#RegisterFields_Email');
            this.createUserForm = $('#_CreateUserForm');
            this.labelEmail = $('#labelEmail');

            this.phoneBilling = $('#RegisterFields_BillingAddress_Phone');
            this.streetAddressBilling = $('#RegisterFields_BillingAddress_StreetAddress');
            this.streetAddressBilling2 = $('#RegisterFields_BillingAddress_StreetAddress2');
            this.cityBilling = $('#RegisterFields_BillingAddress_City');
            this.stateBilling = $('#RegisterFields_BillingAddress_State');
            this.zipBilling = $('#RegisterFields_BillingAddress_Zip');
            this.phoneShipping = $('#RegisterFields_ShippingAddress_Phone');
            this.fullNameShipping = $('#FullNameShipping');
            this.streetAddressShipping = $('#RegisterFields_ShippingAddress_StreetAddress');
            this.streetAddressShipping2 = $('#RegisterFields_ShippingAddress_StreetAddress2');
            this.cityShipping = $('#RegisterFields_ShippingAddress_City');
            this.stateShipping = $('#RegisterFields_ShippingAddress_State');
            this.zipShipping = $('#RegisterFields_ShippingAddress_Zip');
            this.shippingLastName = $('#ShippingLastName');
            this.shippingFirstName = $('#ShippingFirstName');
            this.fullName = $('#FullName');
            this.firstName = $('#FirstName');
            this.lastName = $('#LastName');
            this.institution = $('#RegisterFields_Institution');
            this.emailLoginInput = $('#Email');

            this.typeofAddressShipping = $('#RegisterFields_ShippingAddress_TypeOfAddress').val(Constants.TypeofAddressShipping);
            this.typeofAddressBilling = $('#RegisterFields_BillingAddress_TypeOfAddress').val(Constants.TypeofAddressBilling);

            this.emailLoginInput.focus();
        };
        return PageObject;
    })();
    Registration.PageObject = PageObject;

    var StateManager = (function () {
        function StateManager(incomingPageObject) {
            this.incomingPageObject = incomingPageObject;
            this.disregardIntitutionDomain = false;
            this.zipCheckRequired = false;
            this.nextButtonText = 'Next...';
            this.registerButtonText = 'Submit Register';
            this.sameAsBillingCheckedFilter = '#sameAsBilling:checked';
            this.pageObject = incomingPageObject;
            this.initialize();
        }
        StateManager.prototype.checkAndSubmitEmail = function () {
            if (this.pageObject.getEmailInput().valid() == true) {
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
            this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);

            //console.log('checkAndSubmitZip');
            //$('#emailAddress').val($('#checkEmail').val());
            $('#checkZip').submit();
        };

        StateManager.prototype.displayBillingFields = function () {
            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            if (this.pageObject.getEmailInput().valid() == true) {
                this.pageObject.getCollapseBilling().parent().show();

                var self = this;

                var showBillingInputs = $.Deferred(function () {
                    self.pageObject.getCollapseBilling().collapse('show');
                });

                $.when(showBillingInputs.resolve()).then(function () {
                    self.pageObject.getFullName().focus();
                });

                this.pageObject.getCollapseShipping().parent().show();
                this.pageObject.getCollapseEmail().collapse('toggle');

                this.pageObject.getTheSubmitButton().prop('value', this.registerButtonText);
            }
        };

        StateManager.prototype.enterBillingPane = function (data) {
            this.pageObject.getCollapseBilling().parent().show();
            this.pageObject.getCollapseShipping().parent().show();
            this.pageObject.getCollapseEmail().collapse('toggle');

            var self = this;

            var showBillingInputs = $.Deferred(function () {
                self.pageObject.getCollapseBilling().collapse('toggle');
            });

            $.when(showBillingInputs.resolve()).then(function () {
                self.pageObject.getFullName().focus();
            });

            this.pageObject.getCityBilling().val(data.City);
            this.pageObject.getStateBilling().val(data.State);

            //this.pageObject.zipBilling().val(zipBilling);
            $('#TimeZone').val(data.TimeZone);

            this.pageObject.getLabelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
            this.pageObject.getTheSubmitButton().prop('value', this.registerButtonText);
        };

        StateManager.prototype.enterDifferentAddress = function () {
            //user choose - yes that's my institution but I'd like to enter a different address'
            ////console.log('EnterDiffAddress hit');
            this.pageObject.getModalInstitution().modal('hide');
            this.pageObject.getWrapZip().hide('slow');

            this.zipCheckRequired = true;

            this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);

            this.disregardIntitutionDomain = true;

            this.action = Action.CheckEmail;

            //    Set to none because flow ends here for that stage
            this.inputAction = InputAction.None;

            var self = this;

            var showEmailInput = $.Deferred(function () {
                self.pageObject.getWrapEmail().show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                self.pageObject.getEmailInput().focus();
            });
        };

        StateManager.prototype.foundInstitutionView = function (data, email) {
            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            ////console.log("call foundInstitutionView: " + email);
            this.pageObject.getModalInstitution().modal('show');

            this.pageObject.getInstitution().val(data.Institution);
            this.pageObject.getStreetAddressShipping().val(data.Address);
            this.pageObject.getCityShipping().val(data.City);
            this.pageObject.getStateShipping().val(data.State);
            this.pageObject.getZipShipping().val(data.Zip);
            this.pageObject.getStreetAddressBilling().val(data.Address);
            this.pageObject.getCityBilling().val(data.City);
            this.pageObject.getStateBilling().val(data.State);
            this.pageObject.getZipBilling().val(data.Zip);
            this.pageObject.getLabelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;' + email.substring(email.indexOf('@')) + '</b>&nbsp; domain has been identified.</span>');
            $('#ShowInstitution').html(data.Institution + '<br>' + data.Address + '<br>' + data.City + ', ' + data.State + ' ' + data.Zip + '<br>');
        };

        StateManager.prototype.getAction = function () {
            return this.action;
        };

        StateManager.prototype.getDisregardIntitutionDomain = function () {
            return this.disregardIntitutionDomain;
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
            this.disregardIntitutionDomain = false;
            this.zipCheckRequired = true;
            this.startView();

            this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);
        };

        StateManager.prototype.logIn = function () {
            $('form#frmSignIn').submit();
        };

        StateManager.prototype.newPasswordView = function (email) {
            ////console.log("call newPasswordView: " + email);
            this.pageObject.getWrapEmail().hide('fast');

            var self = this;

            var showPwdInput = $.Deferred(function () {
                //console.log("Deferred newPasswordView: " + email);
                self.pageObject.getWrapPass().show('fast');
            });

            $.when(showPwdInput.resolve()).then(function () {
                self.pageObject.getRegisterFieldsPassword().focus();
            });

            if (email) {
                this.pageObject.getLabelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
            } else {
                this.pageObject.getModalInstitution().modal('hide');
                this.pageObject.getLabelEmail().fadeOut(500, function () {
                    $(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + self.pageObject.getEmailInput().val() + ' will be used for your email address.</span>');
                    $(this).fadeIn(500);
                });
            }

            this.action = Action.GetPassword;

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;

            this.pageObject.getTheSubmitButton().on('mouseenter', function () {
                if ($(self.sameAsBillingCheckedFilter).val()) {
                    self.setShippingToBilling();
                }
            });
        };

        StateManager.prototype.newPassWordToNextStep = function () {
            var confirmPasswordInput = $('[name="RegisterFields.ConfirmPassword"]');
            var pwd = $.trim(this.pageObject.getRegisterFieldsPassword().val());
            var confirmedPwd = $.trim(confirmPasswordInput.val());

            if (!confirmedPwd || pwd !== confirmedPwd) {
                confirmPasswordInput.next('span').removeAttr('class').attr('class', 'field-validation-error').append('<span>Passwords don\'t match.</span>');
                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
                return false;
            }

            if (!pwd || pwd.length < 2 || this.pageObject.getRegisterFieldsPassword().nextAll('span:last').hasClass('field-validation-error')) {
                //console.log('call RegisterFields_Password');
                if (this.inputAction === InputAction.EnterKeyPress)
                    this.inputAction = InputAction.None;
                return false;
            }

            if (this.zipCheckRequired) {
                this.pageObject.getWrapPass().hide('slow');
                this.action = Action.CheckZip;

                var self = this;

                var showGetZipInput = $.Deferred(function () {
                    self.pageObject.getWrapZip().show('slow');
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
            this.pageObject.getLabelEmail().html('<span class="label label-info"><b>&nbsp;&nbsp;Non-US address? Please enter Special Handling Instructions</span>');
            this.pageObject.getZipBilling().val('na');
            this.pageObject.getNonUSAddressInput().show();
            this.displayBillingFields();
            this.action = Action.SubmitRegister;
        };

        StateManager.prototype.notInstitutionAddress = function () {
            //console.log('NotInstitution hit');
            this.pageObject.getModalInstitution().modal('hide');

            var self = this;

            var showEmailInput = $.Deferred(function () {
                self.pageObject.getWrapEmail().show('slow');
            });

            $.when(showEmailInput.resolve()).then(function () {
                self.pageObject.getEmailInput().focus();
            });

            this.pageObject.getWrapZip().hide('slow');
            this.pageObject.getLabelEmail().html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;Proceed or enter a different email address.</span>');

            //  Clear the billing and shipping addresses
            $('#collapseBilling input').val('');
            $('#collapseBilling textarea').val('Mailing address notes:');
            $('#collapseBilling select').val(0);

            $('#collapseShipping input:not("#sameAsBilling")').val('');
            $('#collapseShipping textarea').val('Mailing address notes:');
            $('#collapseShipping select').val(0);

            this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);

            this.disregardIntitutionDomain = true;

            this.action = Action.CheckEmail;

            this.pageObject.getTypeofAddressBilling().val(Constants.TypeofAddressBilling);
            this.pageObject.getTypeofAddressShipping().val(Constants.TypeofAddressShipping);

            if (this.inputAction === InputAction.EnterKeyPress)
                this.inputAction = InputAction.None;
        };

        StateManager.prototype.passResetView = function () {
            //console.log("call passResetView");
            this.pageObject.getLogin().hide('slow');

            var self = this;

            var showResetInput = $.Deferred(function () {
                //TODO: inprocess indicator here?
                self.pageObject.getReset().show('slow');
            });

            $.when(showResetInput.resolve()).then(function () {
                $('#ResetPassEmail').focus();
            });
        };

        StateManager.prototype.registerView = function () {
            this.pageObject.getLogin().hide('slow');

            //this.pageObject.getRegister().show('slow');
            this.action = Action.CheckEmail;

            var self = this;

            var showRegisterInput = $.Deferred(function (pageObject) {
                self.pageObject.getRegister().show('slow');
            });

            $.when(showRegisterInput.resolve()).then(function () {
                self.pageObject.getEmailInput().focus();
            });
        };

        StateManager.prototype.resetPassword = function (normalResetPasswordButton) {
            //console.log('resetPass hit');
            if (normalResetPasswordButton.data('clicked'))
                normalResetPasswordButton.removeData('clicked');

            $('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('form#ResetPasswordForm').submit();
        };

        StateManager.prototype.resetPasswordOrLoginView = function (email) {
            //console.log("call resetPasswordOrLoginView: " + email);
            $('#Email1').val(email);
            $('#ResetPassEmail').val(email);
            this.pageObject.getLabelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; is already on file.</span>');
            this.pageObject.getWrapEmail().hide('slow');

            var self = this;

            var showLoginInput = $.Deferred(function () {
                self.pageObject.getWrapReset().show('slow');
            });

            $.when(showLoginInput.resolve()).then(function () {
                $('#Email1').focus();
            });

            this.action = Action.SubmitLogin;
            this.pageObject.getTheSubmitButton().prop('value', 'Log In');
        };

        StateManager.prototype.setAction = function (incomingAction) {
            this.action = incomingAction;
        };

        StateManager.prototype.setInputAction = function (incomingInputAction) {
            this.inputAction = incomingInputAction;
        };

        StateManager.prototype.setShippingToBilling = function () {
            this.pageObject.getFullNameShipping().val(this.pageObject.getFullName().val());
            this.pageObject.getShippingFirstName().val(this.pageObject.getFirstName().val());
            this.pageObject.getShippingLastName().val(this.pageObject.getLastName().val());
            this.pageObject.getCityShipping().val(this.pageObject.getCityBilling().val());
            this.pageObject.getStateShipping().val(this.pageObject.getStateBilling().val());
            this.pageObject.getStreetAddressShipping().val(this.pageObject.getStreetAddressBilling().val());
            this.pageObject.getStreetAddressShipping2().val(this.pageObject.getStreetAddressBilling2().val());
            this.pageObject.getZipShipping().val(this.pageObject.getZipBilling().val());
            $('#RegisterFields_ShippingAddress_Country').val($('#RegisterFields_BillingAddress_Country').val());
            this.pageObject.getPhoneShipping().val(this.pageObject.getPhoneBilling().val());
        };

        StateManager.prototype.startView = function () {
            this.pageObject.getRegister().hide();
            this.pageObject.getReset().hide();
            this.pageObject.getNonUSAddressInput().hide();
            this.pageObject.getNonUSAddressBtn().hide();
        };

        StateManager.prototype.submit = function () {
            switch (this.action) {
                case Action.CheckEmail:
                    //console.log('CheckEmail hit');
                    if (this.checkAndSubmitEmail()) {
                        if (!this.disregardIntitutionDomain)
                            this.disregardIntitutionDomain = true;
                        this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);
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
                    if (this.pageObject.getCreateUserForm().valid() == true) {
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
                    this.pageObject.getEmailLoginInput().val($('#Email1').val());
                    $('form#frmSignIn').submit();
                    break;
                case Action.DisplayBillingAddressFields:
                    this.displayBillingFields();
                    break;
            }
        };

        StateManager.prototype.submitCreateUserForm = function () {
            var valid = this.pageObject.getCreateUserForm().valid();

            //on account creation default the shipping phone to be same as billing
            $("#RegisterFields.ShippingAddress.phone").val(this.pageObject.getPhoneBilling());

            //console.log("validating createUserForm: " + valid);
            if (valid) {
                //console.log('createUserForm submitted.');
                this.pageObject.getCreateUserForm().submit();

                this.pageObject.getTheSubmitButton().off('mouseenter');
            }
        };

        StateManager.prototype.submitLogin = function () {
            //REG.PageObjects.emailLoginInput().val($('#RegisterFields_Email').val());
            //$('#Password').val($('#Password1').val());
            //$('#frmSignin').submit();
            $('#Password').val(this.pageObject.getRegisterFieldsPassword().val());
            this.pageObject.getEmailLoginInput().val($('#emailAddress').val());
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
                    this.pageObject.getZipBilling().val(zipCode);
                    this.action = Action.SubmitRegister;
                    this.enterBillingPane(data);
                    break;
                case 'false':
                    //console.log('zipCodeVerified-false hit');
                    this.pageObject.getLabelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode was not found!</span>');
                    $('#nonUSAddressBtn').show('slow');
                    break;
                case 'invalid format':
                    //console.log('zipCodeVerified-invalidformat hit');
                    this.pageObject.getLabelEmail().html('<span class="label label-important"><b>&nbsp;&nbsp;Zipcode entered was not in the correct format!</span>');
                    break;
            }
        };
        return StateManager;
    })();
    Registration.StateManager = StateManager;
    ;
})(Registration || (Registration = {}));

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
//# sourceMappingURL=register-user.js.map
