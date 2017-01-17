var registerDuringCheckout = {};
registerDuringCheckout.institutionNames = {};


registerDuringCheckout.initialize = function (orderId, webinarId, orderRowId, addressOptions, callback) {

    OCA.cartStateManager.setCancelOrderForm($('#cancelOrder'));
    OCA.cartStateManager.setConfirmOrderForm($('#confirmOrder'));
    registerDuringCheckout.addressOptions = addressOptions;

    var regUserStateManager, userId;

    var utilities = new Common.Utilities();

    regUserStateManager = new RegistrationInCartByAffiliate.StateManager();
    regUserStateManager.initializeState();
    regUserStateManager.setAction(RegistrationInCartByAffiliate.Action.CheckEmail); // starting off with CheckEmail action.
    //regUserStateManager.setisShippingAddressRequired(addressOptions['shippingAddressRequired']);

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

        if (e.currentTarget.value === 'Create New Account?') // called directly in the razor partial view
            return false;

        if (regUserStateManager.getInputAction() === RegistrationInCartByAffiliate.InputAction.EnterKeyPress)
            return false;

        regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.ButtonClick);

        if (regUserStateManager.getAction() === '') {

            $('#labelEmail').html('<span class="label label-important">&nbsp;Connection Error #893. Please refresh the page and re-try or contact @tenantTechEmail or, for immediate assistant, call @tenant.TechPhone.</span>');
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
            case RegistrationInCartByAffiliate.Button.SignInButton:
                regUserStateManager.logIn();
                break;
            case RegistrationInCartByAffiliate.Button.TheSubmit:
                regUserStateManager.submit();
                break;
            case RegistrationInCartByAffiliate.Button.nonUSAddressBtn:
                regUserStateManager.nonUsAdddressInvoked();
                break;
            case RegistrationInCartByAffiliate.Button.ResetPass:
                regUserStateManager.resetPassword(normalResetPasswordButton);
                break;
            case RegistrationInCartByAffiliate.Button.YesUseAddress:
                regUserStateManager.useRegisteredAddress();
                break;
            case RegistrationInCartByAffiliate.Button.EnterDiffAddress:
                regUserStateManager.enterDifferentAddress();
                break;
            case RegistrationInCartByAffiliate.Button.NotInstitution:

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
                && inputElementTriggered !== RegistrationInCartByAffiliate.Button.YesUseAddress
                && inputElementTriggered !== RegistrationInCartByAffiliate.Button.EnterDiffAddress
                && inputElementTriggered !== RegistrationInCartByAffiliate.Button.NotInstitution) {
                return false;
            }

            regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.EnterKeyPress);

            switch (inputElementTriggered) {
                case 'Password':
                case 'Email':
                case RegistrationInCartByAffiliate.Button.SignInButton:
                    regUserStateManager.logIn();
                    break;
                case 'RegisterFields.Password':
                case 'RegisterFields.ConfirmPassword':
                case 'RegisterFields.Email':
                case 'getZip':
                case 'Password1':
                case 'Email1':
                case RegistrationInCartByAffiliate.Button.TheSubmit:
                    regUserStateManager.submit();
                    break;
                case RegistrationInCartByAffiliate.Button.nonUSAddressBtn:
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
                case RegistrationInCartByAffiliate.Button.ResetPass:
                    regUserStateManager.resetPassword(normalResetPasswordButton);
                    break;
                case RegistrationInCartByAffiliate.Button.YesUseAddress:
                    regUserStateManager.useRegisteredAddress();
                    break;
                case RegistrationInCartByAffiliate.Button.EnterDiffAddress:
                    regUserStateManager.enterDifferentAddress();
                    break;
                case RegistrationInCartByAffiliate.Button.NotInstitution:
                    regUserStateManager.notInstitutionAddress();
                    break;
                default:
                    if ($('#TheSubmitButton').val() === regUserStateManager.getRegisterButtonText()) {
                        if ($(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
                            regUserStateManager.setShippingToBilling();
                        }
                        regUserStateManager.setAction(RegistrationInCartByAffiliate.Action.SubmitRegister);
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
        if ($('#TheSubmitButton').val() === regUserStateManager.getRegisterButtonText()
            && $(regUserStateManager.getSameAsBillingCheckedFilter()).val()) {
            regUserStateManager.setShippingToBilling();
        }
    });

    $('form#checkEmail').submit(function (e) {

        e.preventDefault();

        var jsonUrl = '/Account/CheckEmail';
        var email = $('#RegisterFields_Email').val();
        var token = $(this).find('input[name="__RequestVerificationToken"]').val();
        var payload = { email: email, disregardInstitutionDomain: regUserStateManager.getDisregardIntitutionDomain(), orderId: orderId, __RequestVerificationToken: token };

        if (email.length === 0) {
            $('#RegisterFields_Email').focus();
        } else {
            $.ajax({
                type: 'POST',
                contentType: RegistrationInCartByAffiliate.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCartByAffiliate.Constants.JsonDataType,
                data: payload,
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {

                regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.None);

                // successful request; do something with the data
                if (data.success === 'foundExisting') {

                    regUserStateManager.goToAddressFields(email);
                    if (data.createUser === "true") {
                        createUserAccount(email);
                    }
                } else if (data.success === 'foundInstitution') {
                    //regUserStateManager.foundInstitutionView(data, email);
                    regUserStateManager.goToAddressFields(email);
                    if (data.createUser === "true") {
                        createUserAccount(email);
                    }
                } else if (data.email === 'wasNotFound') {
                    regUserStateManager.goToAddressFields(email);
                    if (data.createUser === "true") {
                        createUserAccount(email);
                    }
                } else if (data.error === 'Fail') {

                    L.clientLogger.info("goToAddressFields #319", { data: data });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Connection Error #319. Email @tenantTechEmail or, for immediate assistance, call @tenant.TechPhone.</span>');
                } else if (data.error === 'Uncaught Ajax Error') {
                    L.clientLogger.error("Uncaught Ajax Error 343", { result: data || "data was falsey", payload: payload });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Uncaught Ajax Error 343</span>');
                }

            }).fail(commonFuncs.failCallBack)
              .always(function (data, status, message) {
                  regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.None);

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
                contentType: RegistrationInCartByAffiliate.Constants.FormPostContentType,
                cache: false,
                url: jsonUrl,
                dataType: RegistrationInCartByAffiliate.Constants.JsonDataType,
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
                regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.None);
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

        regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.None);

        if ($('#wrapZip').is(':visible')) {
            regUserStateManager.setAction(RegistrationInCartByAffiliate.Action.CheckEmail);
        }

    });


    $('#_CreateUserFromCartForm').on('submit', function (event) {
        event.preventDefault();

        var createUserForm = $(this);
        var beigeFormArea = OCA.signUpFormContainer.find('div.well');

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

        var url = createUserForm.attr('action');
        //        alert(url); /// account\registerfromcart
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

                //beigeFormArea.height(500);
                //
            }
        }).done(function (data) {
            if (data.Result) {
                if (data.Result === 'Success') {

                    regUserStateManager.setAction('');
                    registerDuringCheckout.emailOfNewUser = $.trim($('#RegisterFields_Email').val());
                    $('#idUser').val(data.UserId);

                    OCA.signUpForm.submit();

                } else if (data.Result === 'Fail') {
                    L.clientLogger.error("Connection Error #332", { data: data });
                    $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Connection Error #332. Please try again. In case of persistent problem please contact us with our online chat (lower right corner of this page).</span>');
                    regUserStateManager.setAction(RegistrationInCartByAffiliate.Action.SubmitRegister);
                }
            } else if (!data.isSuccessful) {
                L.clientLogger.error("Connection Error #332", { data: data });

                $('#labelEmail').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                formProcessor.lightUpValidationSummary('valSummarySignUpInCart', data);
            }

        }).fail(commonFuncs.failCallBack).always(function () {
            regUserStateManager.setInputAction(RegistrationInCartByAffiliate.InputAction.None);
        });
    });

    $('#loadingSpinner').remove();

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

    var cartStateManager = new OrderRegistration.StateManager();

    cartStateManager.setConfirmOrderForm($('#confirmOrder'));

    var confirmOrderForm = cartStateManager.getConfirmOrderForm();

    confirmOrderForm.on('submit', function (e) {

        e.preventDefault();

        var self = $(this);
        self.find('input[name="id"]').val(orderRowId);

        var data = $(this).serialize();

        var confirmRegistrationBillMe = $('#ConfirmRegistrationBillMe');

        confirmRegistrationBillMe.prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
        confirmRegistrationBillMe.attr('disabled', 'disabled');

        $.ajax({
            type: 'POST',
            contentType: RegistrationInCartByAffiliate.Constants.FormPostContentType,
            cache: false,
            url: self.attr('action'),
            dataType: RegistrationInCartByAffiliate.Constants.JsonDataType,
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

            // enable button again upon ending operation.
            //$('#cancelRegistration').removeAttr('disabled');  // [dar] NO. On staging, redirect is slow and button enabled again. User could have clicked it again.

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

function hookUpEditUserLogic(button) {
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
            //alert(url);
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

                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Error #416. For customer service contact us by using the Online Chat button below or emailing Support@ttsTrain.com.</span>');

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
                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Error #417. For customer service contact us by using the Online Chat button below or emailing Support@ttsTrain.com.</span>');
            });
        });


    });

    modalForm.on('hidden', function (e) {
        $('#saveChangesButton').off('click');
        $('#userDetailsForm').off('submit');
    });
}

// This function's purpose is to update pricing details where the RegType DropDown has its selected value changed.
// It also displays the Shipping Details modal form where the RegType chosen has a shipping address requirement.
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

