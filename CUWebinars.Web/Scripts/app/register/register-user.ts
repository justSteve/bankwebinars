/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />

module Registration {

	declare var $;

	export class Button {
		static EnterDiffAddress : string = 'EnterDiffAddress';
		static nonUSAddressBtn : string = 'nonUSAddressBtn';
		static NotInstitution : string = 'NotInstitution';
		static ResetPass : string = 'resetPass';
		static SignInButton : string = 'SignInButton';
		static TheSubmit : string = 'TheSubmit';
		static YesUseAddress : string = 'YesUseAddress';
	};

	export class InputAction {
		static EnterKeyPress : string = 'EnterKeyPress';
		static ButtonClick : string = 'ButtonClick';
		static None : string = 'None';
	};

	export class Action {
		static CheckEmail : string = 'CheckEmail';
		static CheckZip : string = 'CheckZip';
		static GetPassword : string = 'GetPassword';
		static LogIn : string = 'LogIn';
		static PostCreateAccount : string = 'PostCreateAccount';
		static SubmitLogin : string = 'SubmitLogin';
		static SubmitRegister : string = 'SubmitRegister';
		static DisplayBillingAddressFields : string = 'DisplayBillingAddressFields';
	};

	export class Constants {
		static BillingAddressFields: string = '#RegisterFields_BillingAddress';
		static City: string = '_City';
		static Country: string = '_Country';
		static ConfirmDeleteShippingAddressdialog: string = '#ConfirmDeleteShippingAddressdialog';
		static FormPostContentType: string = 'application/x-www-form-urlencoded';
		static JsonContentType: string = 'application/json; charset=utf-8';
		static JsonDataType: string = 'json';
		static HtmlDataType: string = 'html';
		static Phone: string = '_Phone';
		static ShippingAddressFields: string = '#RegisterFields_ShippingAddress';
		static ShippingAddressContainer: string = '#ShippingAddressContainer';
		static State: string = '_State';
		static AddShippingAddressLink: string = '#AddShippingAddressLink';
		static HideAddShippingAddressLink: string = '#HideAddShippingAddressLink';
		static StreetAddress: string = '_StreetAddress';
		static StreetAddress2: string = '_StreetAddress2';
		static TypeofAddressBilling: string = 'Billing';
		static TypeofAddressShipping: string = 'Shipping';
		static Zip: string = '_Zip';        
	};

	export class StateManager {
		private action: Action;
		private inputAction: InputAction;
        private disregardInstitutionDomain: boolean = false;
		private zipCheckRequired: boolean = false;
		private nextButtonText: string = 'Next...';
		private registerButtonText: string = 'Submit Register';
        private sameAsBillingCheckedFilter: string = '#sameAsBilling:checked';
        private typeofAddressBilling: JQuery;
        private typeofAddressShipping;

		constructor() {
			this.initialize();
        }

        initializeState(): void {

            
            $('#collapseBilling').parent().hide();
            $('#collapseShipping').parent().hide();

            $('#wrapZip').hide();
            $('#wrapPass').hide();
            $('#wrapReset').hide();

            $('#nonUSAddress').hide();

            $('#getFirstLast').hide();

            this.typeofAddressShipping = $('#RegisterFields_ShippingAddress_TypeOfAddress').val(Constants.TypeofAddressShipping);
            this.typeofAddressBilling = $('#RegisterFields_BillingAddress_TypeOfAddress').val(Constants.TypeofAddressBilling);

            $('#RegisterFields_Email').focus();
        }

		checkAndSubmitEmail(): boolean {
			if ($('#RegisterFields_Email').valid() == true) {
				$('#emailAddress').val($('#checkEmail').val());
				$('form#checkEmail').submit();
				return true;
			} else {
				this.inputAction = InputAction.None;
				return false;
			}
		}

		checkAndSubmitZip(): void {
			$('#TheSubmitButton').prop('value', this.nextButtonText);
			//console.log('checkAndSubmitZip');
			//$('#emailAddress').val($('#checkEmail').val());
			$('#checkZip').submit();
		}

		displayBillingFields(): void {

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
		}

		enterBillingPane(data: any): void {
			$('#collapseBilling').parent().show();
			$('#collapseShipping').parent().show();
			$('#collapseEmail').collapse('toggle');

			var showBillingInputs = $.Deferred(function() {
                $('#collapseBilling').collapse('toggle');
			});

			$.when(showBillingInputs.resolve()).then(function() {
                $('#FullName').focus();
			});

			$('#RegisterFields_BillingAddress_City').val(data.City);
			$('#RegisterFields_BillingAddress_State').val(data.State);
			
			$('#TimeZone').val(data.TimeZone);

			$('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
			$('#TheSubmitButton').prop('value', this.registerButtonText);
		}

		enterDifferentAddress() : void {
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
		}

		foundInstitutionView(data, email) : void {

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
		}

		getAction(): Action {
			return this.action;
		}

		getDisregardInstitutionDomain(): boolean {
            return this.disregardInstitutionDomain;
		}

		getInputAction(): InputAction {
			return this.inputAction;
		}

		getRegisterButtonText(): string {
			return this.registerButtonText;
		}

		getSameAsBillingCheckedFilter(): string {
			return this.sameAsBillingCheckedFilter;
		}

		initialize(): void {
			this.action = Action.LogIn;
			this.inputAction = InputAction.None;
            this.disregardInstitutionDomain = false;
			this.zipCheckRequired = true;
			this.startView();

			$('#TheSubmitButton').prop('value', this.nextButtonText);
		}

		logIn(): void {

			$('form#frmSignIn').submit();
		}

		newPasswordView(email: string): void {
			////console.log("call newPasswordView: " + email);
			$('#wrapEmail').hide('fast');

			var self = this;


			if (email) {
				$('#labelEmail').html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
			} else {
				$('#modalInstitution').modal('hide');
				$('#labelEmail').fadeOut(500, function() {
					$(this).html('<span class="label label-success">&nbsp;&nbsp;' + $('#RegisterFields_Email').val() + ' will be used for your email address.</span>');
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
		}

		newPassWordToNextStep(): boolean {

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
		}

		nonUsAdddressInvoked(): void {
			//console.log("hit nonUsAdddressInvoked");
			$('#labelEmail').html('<span class="label label-info"><b>&nbsp;&nbsp;Non-US address? Please enter Special Handling Instructions</span>');
			$('#RegisterFields_BillingAddress_Zip').val('na');
			$('#nonUSAddressInput').show();
			this.displayBillingFields();
			this.action = Action.SubmitRegister;
		}

		notInstitutionAddress() : void {
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
		}

		passResetView(): void {
			//console.log("call passResetView");

            this.inputAction = InputAction.None;

			$('#login').hide('slow');

			var showResetInput = $.Deferred(function() {
				//TODO: inprocess indicator here?
				$('#reset').show('slow');
			});

			$.when(showResetInput.resolve()).then(function() {
				$('#ResetPassEmail').focus();
			});
		}

		registerView() : void {
			$('#login').hide('slow');

            this.action = Action.CheckEmail;
		    this.inputAction = InputAction.None;

			var showRegisterInput = $.Deferred(function () {
                $('#register').show('slow');
			});

			$.when(showRegisterInput.resolve()).then(function () {
                $('#RegisterFields_Email').focus();
			});
		}

		resetPassword(normalResetPasswordButton: JQuery) : void {
			//console.log('resetPass hit');
			if (normalResetPasswordButton.data('clicked'))
				normalResetPasswordButton.removeData('clicked');

			$('#EdgeCaseResetPasswordButton').data('clicked', true);
            $('form#ResetPasswordForm').submit();
            $('#TheSubmitButton').attr('disabled', 'disabled');
		}

		resetPasswordOrLoginView (email:JQuery) : void {
			//console.log("call resetPasswordOrLoginView: " + email);
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
		}

		setAction(incomingAction: Action): void {
			this.action = incomingAction;
		}

		setInputAction(incomingInputAction: InputAction): void {
			this.inputAction = incomingInputAction;
		}

        setShippingToBilling(): void {

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
		}

        startView(): void {

            $('#loginMsgLabel').hide();
            $('#register').hide();
			$('#reset').hide();
			$('#nonUSAddressInput').hide();
			$('#nonUSAddressBtn').hide();
		}

		submit () : void {


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
		}

		submitCreateUserForm(): void {

			var valid = $('#_CreateUserForm').valid();

			//on account creation default the shipping phone to be same as billing
			$('#RegisterFields.ShippingAddress.phone').val($('#RegisterFields_BillingAddress_Phone').val());
			//console.log("validating createUserForm: " + valid);

			if (valid) {
				//console.log('createUserForm submitted.');
				$('#_CreateUserForm').submit();

				$('#TheSubmitButton').off('mouseenter');
			}
		}

		submitLogin(): void {
            $('#Password').val($('#RegisterFields_Password').val());
			$('#Email').val($('#emailAddress').val());
			$('form#frmSignIn').submit();
		}

		useRegisteredAddress() : void {
			//console.log('YesUseAddress hit');

			this.zipCheckRequired = false;

			this.newPasswordView(null);
		}

		zipCodeVerified(data:any, zipCode:string) {

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
		}

	};

}