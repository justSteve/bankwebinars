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

	export class PageObject {

		private collapseEmail: JQuery;
		private emailLoginInput: JQuery;
		private nonUSAddress: JQuery;
		private nonUSAddressBtn: JQuery;
		private nonUSAddressInput: JQuery;
		private wrapEmail: JQuery;
		private wrapPass: JQuery;
		private wrapReset: JQuery;
		private wrapZip: JQuery;
		private theSubmitButton: JQuery;
		private emailInput: JQuery;
		private createUserForm: JQuery;
		private phoneBilling: JQuery;
		private fullNameShipping: JQuery;
		private institution: JQuery;
		private streetAddressBilling: JQuery;
		private streetAddressBilling2: JQuery;
		private cityBilling: JQuery;
		private stateBilling: JQuery;
		private zipBilling: JQuery;
		private phoneShipping: JQuery;
		private shippingFirstName: JQuery;
		private shippingLastName: JQuery;
        private shippingCity: JQuery;
        private shippingAddress: JQuery;
        private shippingAddress2: JQuery;
        private shippingZip: JQuery;
        private shippingState: JQuery;
		private streetAddressShipping: JQuery;
		private streetAddressShipping2: JQuery;
		private cityShipping: JQuery;
		private stateShipping: JQuery;
		private zipShipping: JQuery;
		private fullName: JQuery;
		private firstName: JQuery;
		private lastName: JQuery;
		private login: JQuery;
		private reset: JQuery;
		private register: JQuery;
		private labelEmail: JQuery;
		private modalInstitution: JQuery;
		private collapseBilling: JQuery;
		private collapseShipping: JQuery;
		private registerFieldsPassword: JQuery;
		private getFirstLast: JQuery;
		private typeofAddressBilling: JQuery;
		private typeofAddressShipping;

		constructor() {
			
		}

		getTheSubmitButton(): JQuery {
			return this.theSubmitButton || $('#TheSubmitButton');
		}
		getCollapseEmail(): JQuery {
			return this.collapseEmail || $('#collapseEmail');
		}
		getEmailInput(): JQuery {
			return this.emailInput || $('#RegisterFields_Email');
		}
		getCityBilling(): JQuery {
			return this.cityBilling || $('#RegisterFields_BillingAddress_City');
		}
		getNonUSAddressBtn(): JQuery {
			return this.nonUSAddressBtn || $('#nonUSAddressBtn');
		}
		getnonUSAddress(): JQuery {
			return this.nonUSAddress || $('#nonUSAddress');
		}
		getNonUSAddressInput(): JQuery {
			return this.nonUSAddressInput || $('#nonUSAddressInput');
		}
		getCityShipping(): JQuery {
			return this.cityShipping || $('#RegisterFields_ShippingAddress_City');
		}
		getCollapseBilling(): JQuery {
			return this.collapseBilling || $('#collapseBilling');
		}
		getCollapseShipping(): JQuery {
			return this.collapseShipping || $('#collapseShipping');
		}
		getCreateUserForm(): JQuery {
			return this.createUserForm || $('#_CreateUserForm');
		}
		getEmailLoginInput(): JQuery {
			return this.emailLoginInput || $('#Email');
		}
		getFullNameShipping(): JQuery {
			return this.fullNameShipping || $('#FullNameShipping');
		}
		getFullName(): JQuery {
			return this.fullName || $('#FullName');
		}
		getFirstName(): JQuery {
			return this.firstName || $('#FirstName');
		}
		getGetFirstLast(): JQuery {
			return this.getFirstLast || $('#getFirstLast');
		}
		getInstitution(): JQuery {
			return this.institution || $('#RegisterFields_Institution');
		}
		getLabelEmail(): JQuery {
			return this.labelEmail || $('#labelEmail');
		}
		getLastName(): JQuery {
			return this.lastName || $('#LastName');
		}
		getLogin(): JQuery {
			return this.login || $('#login');
		}
		getModalInstitution(): JQuery {
			return this.modalInstitution || $('#modalInstitution');
		}
		getPhoneBilling(): JQuery {
			return this.phoneBilling || $('#RegisterFields_BillingAddress_Phone');
		}
		getPhoneShipping(): JQuery {
			return this.phoneShipping || $('#RegisterFields_ShippingAddress_Phone');
		}
		getRegister(): JQuery {
			return this.register || $('#register');
		}
		getRegisterFieldsPassword(): JQuery {
			return this.registerFieldsPassword || $('#RegisterFields_Password');
		}
		getReset(): JQuery {
			return this.reset || $('#reset');
		}
		getShippingFirstName(): JQuery {
			return this.shippingFirstName || $('#ShippingFirstName');
		}
		getShippingLastName(): JQuery {
			return this.shippingLastName || $('#ShippingLastName');
		}
		getStateBilling(): JQuery {
			return this.stateBilling || $('#RegisterFields_BillingAddress_State');
		}
		getStreetAddressBilling(): JQuery {
			return this.streetAddressBilling || $('#RegisterFields_BillingAddress_StreetAddress');
		}
		getStreetAddressBilling2(): JQuery {
			return this.streetAddressBilling2 || $('#RegisterFields_BillingAddress_StreetAddress2');
		}
		getStreetAddressShipping(): JQuery {
			return this.streetAddressShipping || $('#RegisterFields_ShippingAddress_StreetAddress');
		}
		getStreetAddressShipping2(): JQuery {
			return this.streetAddressShipping2 || $('#RegisterFields_ShippingAddress_StreetAddress2');
		}
		getStateShipping(): JQuery {
			return this.stateShipping || $('#RegisterFields_ShippingAddress_State');
		}
		getTypeofAddressBilling(): JQuery {
			return this.typeofAddressBilling || $('#RegisterFields_BillingAddress_TypeOfAddress');
		}
		getTypeofAddressShipping(): JQuery {
			return this.typeofAddressShipping || $('#RegisterFields_ShippingAddress_TypeOfAddress');
		}
		getWrapEmail(): JQuery {
			return this.wrapEmail || $('#wrapEmail');
		}
		getWrapPass(): JQuery {
			return this.wrapPass || $('#wrapPass');
		}
		getWrapReset(): JQuery {
			return this.wrapReset || $('#wrapReset');
		}
		getWrapZip(): JQuery {
			return this.wrapZip || $('#wrapZip');
		}
		getZipBilling(): JQuery {
			return this.zipBilling || $('#RegisterFields_BillingAddress_Zip');
		}
		getZipShipping(): JQuery {
			return this.zipShipping || $('#RegisterFields_ShippingAddress_Zip');
		}

		initializeState(): void {

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
		}
	}

	export class StateManager {
		private action: Action;
		private inputAction: InputAction;
		private disregardIntitutionDomain: boolean = false;
		private zipCheckRequired: boolean = false;
		private pageObject: Registration.PageObject;
		private nextButtonText: string = 'Next...';
		private registerButtonText: string = 'Submit Register';
		private sameAsBillingCheckedFilter: string = '#sameAsBilling:checked';

        constructor(public incomingPageObject: Registration.PageObject) {
			this.pageObject = incomingPageObject;
			this.initialize();
		}

		checkAndSubmitEmail(): boolean {
			if (this.pageObject.getEmailInput().valid() == true) {
				//console.log(REG.PageObjects.emailInput().valid());
				$('#emailAddress').val($('#checkEmail').val());
				$('form#checkEmail').submit();
				return true;
			} else {
				this.inputAction = InputAction.None;
				return false;
			}
		}

		checkAndSubmitZip(): void {
			this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);
			//console.log('checkAndSubmitZip');
			//$('#emailAddress').val($('#checkEmail').val());
			$('#checkZip').submit();
		}

		displayBillingFields(): void {

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
		}

		enterBillingPane(data: any): void {
			this.pageObject.getCollapseBilling().parent().show();
			this.pageObject.getCollapseShipping().parent().show();
			this.pageObject.getCollapseEmail().collapse('toggle');

			var self = this;

			var showBillingInputs = $.Deferred(function() {
				self.pageObject.getCollapseBilling().collapse('toggle');
			});

			$.when(showBillingInputs.resolve()).then(function() {
				self.pageObject.getFullName().focus();
			});

			this.pageObject.getCityBilling().val(data.City);
			this.pageObject.getStateBilling().val(data.State);
			//this.pageObject.zipBilling().val(zipBilling);
			$('#TimeZone').val(data.TimeZone);

			this.pageObject.getLabelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;Email and Zipcode are recorded.</span>');
			this.pageObject.getTheSubmitButton().prop('value', this.registerButtonText);
		}

		enterDifferentAddress() : void {
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
		}

		foundInstitutionView(data, email) : void {

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
		}

		getAction(): Action {
			return this.action;
		}

		getDisregardIntitutionDomain(): boolean {
			return this.disregardIntitutionDomain;
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
			this.disregardIntitutionDomain = false;
			this.zipCheckRequired = true;
			this.startView();

			this.pageObject.getTheSubmitButton().prop('value', this.nextButtonText);
		}

		logIn(): void {

			$('form#frmSignIn').submit();
		}

		newPasswordView(email: string): void {
			////console.log("call newPasswordView: " + email);
			this.pageObject.getWrapEmail().hide('fast');

			var self = this;

			var showPwdInput = $.Deferred(function () {
				//console.log("Deferred newPasswordView: " + email);
				self.pageObject.getWrapPass().show('fast');
			});

			$.when(showPwdInput.resolve()).then(function() {
				self.pageObject.getRegisterFieldsPassword().focus();
			});


			if (email) {
				this.pageObject.getLabelEmail().html('<span class="label label-success"><b>&nbsp;&nbsp;' + email + '</b>&nbsp; has been recorded.</span>');
			} else {
				this.pageObject.getModalInstitution().modal('hide');
				this.pageObject.getLabelEmail().fadeOut(500, function() {
					$(this).html('<span class="label label-success">&nbsp;&nbsp;&nbsp;&nbsp;' + self.pageObject.getEmailInput().val() + ' will be used for your email address.</span>');
					$(this).fadeIn(500);
				});
			}

			this.action = Action.GetPassword;

			if (this.inputAction === InputAction.EnterKeyPress)
				this.inputAction = InputAction.None;

			this.pageObject.getTheSubmitButton().on('mouseenter', function() {
				if ($(self.sameAsBillingCheckedFilter).val()) {
					self.setShippingToBilling();
				}
			});

		}

		newPassWordToNextStep(): boolean {

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
		}

		nonUsAdddressInvoked(): void {
			//console.log("hit nonUsAdddressInvoked");
			this.pageObject.getLabelEmail().html('<span class="label label-info"><b>&nbsp;&nbsp;Non-US address? Please enter Special Handling Instructions</span>');
			this.pageObject.getZipBilling().val('na');
			this.pageObject.getNonUSAddressInput().show();
			this.displayBillingFields();
			this.action = Action.SubmitRegister;
		}

		notInstitutionAddress() : void {
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
		}

		passResetView(): void {
			//console.log("call passResetView");
			this.pageObject.getLogin().hide('slow');

			var self = this;

			var showResetInput = $.Deferred(function() {
				//TODO: inprocess indicator here?
				self.pageObject.getReset().show('slow');
			});

			$.when(showResetInput.resolve()).then(function() {
				$('#ResetPassEmail').focus();
			});
		}

		registerView() : void {
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
		}

		resetPassword(normalResetPasswordButton: JQuery) : void {
			//console.log('resetPass hit');
			if (normalResetPasswordButton.data('clicked'))
				normalResetPasswordButton.removeData('clicked');

			$('#EdgeCaseResetPasswordButton').data('clicked', true);
			$('form#ResetPasswordForm').submit();
		}

		resetPasswordOrLoginView (email:JQuery) : void {
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
		}

		setAction(incomingAction: Action): void {
			this.action = incomingAction;
		}

		setInputAction(incomingInputAction: InputAction): void {
			this.inputAction = incomingInputAction;
		}

		setShippingToBilling(): void {
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
		}

		startView(): void {
			this.pageObject.getRegister().hide();
			this.pageObject.getReset().hide();
			this.pageObject.getNonUSAddressInput().hide();
			this.pageObject.getNonUSAddressBtn().hide();
		}

		submit () : void {


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
		}

		submitCreateUserForm(): void {

			var valid = this.pageObject.getCreateUserForm().valid();

			//on account creation default the shipping phone to be same as billing
			$("#RegisterFields.ShippingAddress.phone").val(this.pageObject.getPhoneBilling());
			//console.log("validating createUserForm: " + valid);

			if (valid) {
				//console.log('createUserForm submitted.');
				this.pageObject.getCreateUserForm().submit();

				this.pageObject.getTheSubmitButton().off('mouseenter');
			}
		}

		submitLogin(): void {
			//REG.PageObjects.emailLoginInput().val($('#RegisterFields_Email').val());
			//$('#Password').val($('#Password1').val());
			//$('#frmSignin').submit();
			$('#Password').val(this.pageObject.getRegisterFieldsPassword().val());
			this.pageObject.getEmailLoginInput().val($('#emailAddress').val());
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
		}

	};

}