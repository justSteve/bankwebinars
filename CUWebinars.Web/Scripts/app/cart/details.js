//  This script correlates with the Details View.
var additionalLocationsList, checkoutConfirm, discount, cartStateManager, okToLeave, shippingAddressRequired, signUpForm, signUpFormContainer, storedHeight, numberOfAdditionalLocationsTab3;

discount = '';
checkoutConfirm = {};
okToLeave = true;
pagetitle = $("h1:first").text();


$(function () {

    signUpForm = $('#SignUpForm');
    signUpFormContainer = $('#SignUpFormContainer'); // The big beige box

    // This function gets invoked when the 3rd tab is loaded and an existing user is using the cart
    checkoutConfirm.initialize = function(userId) {
        cartStateManager.setCancelOrderForm($('#cancelOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        //console.log('initialize hit');

        cartStateManager.getConfirmOrderForm().on('submit', function(e) {
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());

            L.clientLogger.info('d-#1', { 'submitting getConfirmOrderForm': cartStateManager.getOrderRowId() });

            var data = $(this).serialize();

            L.clientLogger.info('d-#2', { 'Serialized Form: ': data });

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
                beforeSend: function() {
                    confirmRegistrationBillMe.attr('disabled', 'disabled');
                }
            }).done(function(data) {
                if (data.Result === 'Success') {
                    L.clientLogger.info('d-#3', { 'OrderRowId': data.OrderRowId });
                    var orderRowId = data.OrderRowId;

                    console.info('Posted orderRow:' + orderRowId);

                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    confirmRegistrationBillMe.after('<span>&nbsp;<span class="label label-success">&nbsp;<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;Transferring you now...</span></span>');

                    okToLeave = false;

                    var utilities = new Common.Utilities();
                    utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());

                } else {
                    console.error('Failed to post order');
                    L.clientLogger.error('d-#4 Failed to post order', { 'jsonResponse': data });

                    confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data #554. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                $('#signUpSpinner').remove();
            }).fail(function(jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                $('#signUpSpinner').remove();
                confirmRegistrationBillMe.after('<span class="field-validation-error">Transport error #555. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                L.clientLogger.error('d-#5', { 'Error': jqXHR.responseText });
            });

        });


        var cancelOrderForm = cartStateManager.getCancelOrderForm();
        // CANCEL REGISTRATION BUTTON CLICKED
        cancelOrderForm.on('submit', function(e) {

            console.log('cancelOrderForm submit hit');

            e.preventDefault();
            e.stopImmediatePropagation();

            $('#cancelModalOrderId').val(cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);
            // RED BUTTON - DELETE REGISTRATION
            $('#cancelRegistration').on('click', function(e) {
                e.preventDefault();

                // disable button while operation in progress
                $('#cancelRegistration').attr('disabled', 'disabled').after('<span id="cancelSpinner"><span>&nbsp;<i class="icon icon-spinner icon-spin"></i></span></span>');

                L.clientLogger.info('d-#6', { 'orderCancellation': 'deleting order at 3rd tab', 'orderId': cartStateManager.getOrderId() });

                $.post(self.attr('action'), data, function(response, status, xhr) {

                    if (status !== 'error') {
                        if (xhr.responseJSON['success']) {
                            L.clientLogger.info('d-#7', { 'orderCancellationConfirmed': 'deletion succeeded' });

                            okToLeave = true;

                            var utilities = new Common.Utilities();
                            //console.log('/webinar/details/' + cartStateManager.getWebinarId());
                            $('#cancelSpinner').remove();
                            utilities.goToUrl('/webinar/details/' + cartStateManager.getWebinarId());
                        } else {
                            L.clientLogger.error('d-#8', { 'orderCancellationFailed': 'Deletion failed. System potentially in error state.' });
                            $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Invalid Data #88. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                    } else {
                        L.clientLogger.error('d-#9', { 'orderCancellationFailed': 'Deletion failed. System potentially in error state.' });
                        $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Server Error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                        $('#CancelModal').modal('hide');
                    }
                }, 'json');

                // unbind event so we don't get them building up each time the user clicks the Cancel Registration button.
                $(this).off('click');
                $('#rtn').off('click');
            });
            // BLACK BUTTON - RETURN TO ORDER
            $('#rtn').on('click', function(e) {
                e.preventDefault();
                $('#CancelModal').modal('hide');
                $(this).off('click');
                $('#cancelRegistration').off('click');
            });

            $('#CancelModal').modal('show');
        });
    };

    cartStateManager = new OrderRegistration.StateManager();

    // values set in razor view assigned to member of cartStateManager
    cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tag in razor view Details.cshtml
    cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view Details.cshtml
    cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tag in razor view Details.cshtml
    cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tag in razor view Details.cshtml
    cartStateManager.setAddressVerified(addressVerified); // addressVerified is set in a script tag in razor view Details.cshtml
    cartStateManager.setNotificationsTesting(notificationsTesting); // notificationsTesting is set in a script tag in razor view Details.cshtml

    L.clientLogger.info(
        'd-#10', {
            'serverVariables': {
                'webinarId': webinarId,
                'orderRowId': orderRowId,
                'isUserLoggedIn': isUserLoggedIn,
                'checkoutInProcess': checkoutInProcess,
                'addressVerified': addressVerified,
                'notificationsTesting': notificationsTesting
            }
        });

    cartStateManager.SetCartState();

    $("[id^='regTypeID_']").on("click", function(oEvent) {
        cartStateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });

    // Click event for the BIG GREEN SignUp button
    $('#AddToCart').on('click', function() {
        $(this).prepend('<i id="signUpSpinner" class="icon-spinner icon-spin"></i>').attr('disabled', 'disabled');
        signUpForm.submit();
    });

    if (discount !== 'none') {
        $('#showDiscount').css('display', 'block');
    }

    // Flow goes inside this block where the order exists and is in process e.g. previously abandoned before finializing
    if (cartStateManager.getOrderRowId() > 0 && cartStateManager.getCheckoutInProcess()) {

        L.clientLogger.info('d-#11', { 'returnUnfinishedOrder': 'User finishing order row: ' + cartStateManager.getOrderRowId() });

        if (shippingAddressRequired && !cartStateManager.getNotificationsTesting() && !cartStateManager.getAddressVerified()) {
            // Following function lives in the register-during-checkout.js script
            // which will be in memory at this point and thus will have been hoisted.
            hookUpModal($('#UserDetailsModal'));
        }

        cartStateManager.setOrderId(orderId);
        L.clientLogger.info('d-#12', { 'returnUnfinishedOrder': 'User finishing order: ' + orderId });

// see top of this file
        checkoutConfirm.initialize();

//The BIG GREEN 'Bill Me' button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function(e) {
            e.preventDefault();
            var confirmOrderForm = $('#confirmOrder');
            confirmOrderForm.submit();
        });


        // The 'TO PAY BY CREDIT CARD' button on 3rd tab
        $('#ConfirmRegistrationPayByCC').on('click', function(e) {
            e.preventDefault();

            var orderId = cartStateManager.getOrderId();
            var url = '/Cart/PayCC/' + orderId;

            var utilities = new Common.Utilities();

            utilities.goToUrl(url);

        });

        // The grey CANCEL Registration button on 3rd tab       
        $('#Canceller').on('click', function(e) {
            e.preventDefault();
            var cancelOrderForm = $('#cancelOrder');
            cancelOrderForm.submit();
        });

        populateAdditionalLocationsOn3rdTab();
        setUpEditButtons();

// Following 3 functions live in the register-during-checkout.js script
// which will be in memory at this point. So these functions will be hoisted.
        hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
        hookUpChangeTypeLogic($('#RegType'));
        hookUpEditUserLogic(null, shippingAddressRequired);
    }

/* Submit event for the big green SIGNUP button */
    signUpForm.on('submit', function(e) {
        e.preventDefault();

        var beigeFormArea = signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippindAddressRequired function
        //  value comes from a hidden input in the radio btn list next to the relevant radio button (previous-sibling)
        shippingAddressRequired = isShippindAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = signUpForm.serialize();

        L.clientLogger.info('d-#19', { 'signupData': data });

        var spinner = $('#signUpSpinner');
        $('#SignUpFormContainer > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        var loadingSpinner = $('#loadingSpinner');

// If the user IS NOT LOGGED IN - control moves to the register-during-checkout.js script
        if (!cartStateManager.getIsUserLoggedIn()) {

            L.clientLogger.info('d-#20', { 'anonymousUser': 'Order created for anonymous user. Not yet finalized.' });

            $.post(signUpForm.attr('action'), data, function(response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {
                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                        $('#contactInfo').load('/Cart/CheckoutContactDetails', function(response, status, xhr) {
                            if (status !== 'error') {
                                $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details/' + cartStateManager.getWebinarId());

                                var addressOptions = {
                                    'shippingAddressRequired': shippingAddressRequired,
                                    'notificationsTesting': cartStateManager.getNotificationsTesting(),
                                    'addressVerified': cartStateManager.getAddressVerified()
                                };

                                registerDuringCheckout.initialize(cartStateManager.getOrderId(), cartStateManager.getWebinarId(), cartStateManager.getOrderRowId(), addressOptions, checkoutConfirm.initialize);
                            } else {
                                $('#labelEmail').html('<span class="label label-important">Server error #21. Try again or call 800-831-0678 ext 706 for immediate assistance!</span>');
                                L.clientLogger.error('d-#28', { 'responseObject': xhr.responseJSON, 'anonymousUserSubmit': 'Fail condition.' });
                            }
                            loadingSpinner.remove();
                        });

                        $('#contactInfoTab a').tab('show');
                    } else if (!xhr.responseJSON['isSuccessful']) {
                        $('#labelEmail').html('<span class="label label-important">&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);
                        loadingSpinner.remove();
                        L.clientLogger.error('d-#22', { 'anonymousUserSubmit': 'Fail condition.' });
                    }
                } else {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Server error. Try again or call 800-831-0678 ext 706 for immediate assistance!</span>');
                    loadingSpinner.remove();
                    L.clientLogger.error('d-#23', { 'anonymousUserSubmit': 'Fail condition.' });
                }
            }, constants.JsonDataType);
        } else {
            // If the user IS LOGGED IN
            $.post(signUpForm.attr('action'), data, function(response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {

                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);

                        L.clientLogger.info('d-#30', { 'loggedInUser': 'submitting ConfirmOrder' + xhr.responseJSON['orderId'] });

                        $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderRowId(), function(response, status, xhr) {

                            if (status === 'error') {
                                $(this).html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                                $('#loadingSpinner').remove();
                                $('#confirmationTab a').tab('show');

                                L.clientLogger.error('d-#31', { 'loggedInUser': 'Fail condition.', 'responseObject': xhr.responseJSON });
                            } else {

                                $('#confirmationTab a').tab('show');
                                var utilities = new Common.Utilities();

                                if (shippingAddressRequired && !cartStateManager.getNotificationsTesting() && !cartStateManager.getAddressVerified()) {
                                    // Following function lives in the register-during-checkout.js script
                                    // which will be in memory at this point and thus will have been hoisted.
                                    hookUpModal($('#UserDetailsModal'));
                                }

                                // see top of this file
                                checkoutConfirm.initialize();

                                // The Bill Me button on 3rd tab
                                $('#ConfirmRegistrationBillMe').on('click', function(e) {
                                    e.preventDefault();
                                    var confirmOrderForm = $('#confirmOrder');
                                    confirmOrderForm.submit();
                                });

                                // The 'To pay by credit card' button on 3rd tab
                                $('#ConfirmRegistrationPayByCC').on('click', function(e) {
                                    e.preventDefault();

                                    var orderId = cartStateManager.getOrderId();
                                    var url = '/Cart/PayCC/' + orderId;

                                    utilities.goToUrl(url);

                                });

                                // The Cancel Registration button on 3rd tab
                                $('#Canceller').on('click', function(e) {
                                    e.preventDefault();
                                    var cancelOrderForm = $('#cancelOrder');
                                    cancelOrderForm.submit();
                                });

                                populateAdditionalLocationsOn3rdTab();
                                setUpEditButtons();

                                // Following 3 functions live in the register-during-checkout.js script
                                // which will be in memory at this point and thus will be hoisted
                                hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
                                hookUpChangeTypeLogic($('#RegType'));
                                hookUpEditUserLogic(null, shippingAddressRequired);

                                beigeFormArea.height($('#confirmation').height() + 30);
                            }

                            spinner.remove();
                            $('#loadingSpinner').remove();

                        }, constants.HtmlDataType);
                    } else if (xhr.responseJSON['isSuccessful'] === false) {
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);

                        spinner.remove();

                        L.clientLogger.error('d-#27', { 'loggedInUser': 'Fail condition.', 'responseObject': xhr.responseJSON });
                    }
                } else {
                    spinner.remove();
                    $('#confirmation').html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                    L.clientLogger.error('d-#26', { 'loggedInUser': 'Fail condition.' });
                }
            }, constants.JsonDataType);

            return false;
        }
        return false;
    });

    $(window).on('beforeunload', function (e) {
        //  taken from this SO answer http://stackoverflow.com/a/7317311/540156
        if (okToLeave) {
            return undefined;
        }

        L.clientLogger.info('d-#32', { 'User error': 'User attempted to abandoned order', OrderId: cartStateManager.getOrderId() || 'No order id available yet' });

        var confirmationMessage = 'It looks like you have been creating an order.\r\n';
        cartStateManager.getOrderId() && (confirmationMessage += 'OrderId: ' + cartStateManager.getOrderId() + '.\r\n');
        confirmationMessage += 'If you leave before completing the order, your changes will be lost.\r\n';
        confirmationMessage += 'Are you sure you want to abandon this order?';

        return confirmationMessage;
    });
});

function isShippindAddressRequired(jQueryObject) {
    //todo: re-enable
    //if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
        return false;
    //return true;
}

function setUpEditButtons() {
    $('#revealOptions').on('click', function (e) {
        e.preventDefault();
        $('#AdjustOrder').slideToggle();
    });

    $('#revealDiscountInput').on('click', function (e) {
        e.preventDefault();
        $('#AdjustDiscount').slideToggle();
    });

    $('#revealAddLocsPanel').on('click', function (e) {
        e.preventDefault();
        $('#AdjustAddLoc').slideToggle();
    });
    $('#editUserDetails').on('click', function (e) {
        e.preventDefault();
        $('#AdjustUserDetails').slideToggle(400, function () { $('#editUserResult').remove(); });
    });

    $('#SubmitUserDetailEdits').on('click', function (e) {

        e.preventDefault();

        var self = $(this);

        var form = $('#UserDetailsAdjustForm');

        var url = form.attr('action');

        var token = form.find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;


        var payload = {
            email: $('#AdjustUserDetailsPanel_Email').val(),
            idUser: $('#AdjustUserDetailsPanel_idUser').val(),
            firstname: $('#AdjustUserDetailsPanel_FirstName').val(),
            lastname: $('#AdjustUserDetailsPanel_LastName').val(),
            Institution: $('#AdjustUserDetailsPanel_Institution').val()
        };

        L.clientLogger.info( 'd-#17', { 'userDetailEdits': payload } );

    $.ajax({
        type: 'POST',
        contentType: constants.JsonContentType,
        cache: false,
        url: url,
        dataType: constants.JsonDataType,
        data: JSON.stringify(payload),
        headers: headers,
        beforeSend: function () {
            $('#editUserResult').remove();
            self.after('<span id="userDetailsSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
        }
    }).done(function (data) {

        if (data.Result === 'Success') {
            self.after('<span id="editUserResult">&nbsp;<span class="label label-success"><span> Details updated successfully! </span></span></span>').hide().fadeIn(500);
            L.clientLogger.info( 'd-#18', { 'userDetailEditsResult': 'edits succeeded' } );
    }

        $('#userDetailsSpinner').remove();
});
});

$('#addAnotherAddLoc').on('click', function (e) {

    e.preventDefault();

    var newId;

    if (numberOfAdditionalLocationsTab3 == 0) {

        additionalLocationsList.after($('<button>',
        {
            id: 'applyAdditionalLocationsButton',
            text: 'apply',
            'class': 'btn btn-mini btn-primary',
        }));

        $('#applyAdditionalLocationsButton').on('click', applyAdditionalLocations);

        newId = 0;
    } else {
        // first get the last previous email input
        var lastInput = additionalLocationsList.find('input[type="email"]:last');
        // get its id
        var lastInputId = lastInput.attr('id');
        var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
        newId = id + 1;
    }
    additionalLocationsList.append('<span id="' + locationsSpanPrefix + newId + '"><input id="AdditionalLocationEmail_' + newId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-AdditionLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
    additionalLocationsList.find('i#' + newId + '-AdditionLocationEmail-delete').on('click', deleteAddLocInputTabb3);
    $('#AdditionalLocationEmail_' + newId).focus();
    numberOfAdditionalLocationsTab3++;
});
}

function populateAdditionalLocationsOn3rdTab() {

    // There is re-use involved with additional locations as they can be manipulated on either the 1st or 3rd tab. 
    // Hence, the locationsSpanPrefix may already exist in some scenarios.
    if (!locationsSpanPrefix) {
        locationsSpanPrefix = 'LocationSpan-',
        breakSuffix = '-break';
    }

    // This variable gets declared elsewhere. Hnce, no 'var' keyword
    addLocsOn1stTabContainer = $('#collectAdditionalLocations');
    var locations = addLocsOn1stTabContainer.children();
    numberOfAdditionalLocationsTab3 = locations.filter('span').length;


var copyOfLocations = locations.clone();

addLocsOn1stTabContainer.remove();

additionalLocationsList = $('#additionalLocationsList');

additionalLocationsList.append('<input id="newOrderRowId" name="newOrderRowId"  type="hidden" value=' + cartStateManager.getOrderRowId() + ' data-val="true" data-val-number="The field newOrderRowId must be a number." data-val-required="The newOrderRowId field is required."/>');
additionalLocationsList.append(copyOfLocations);

if (numberOfAdditionalLocationsTab3 > 0) {
    additionalLocationsList.after($('<button>',
    {
        id: 'applyAdditionalLocationsButton',
        text: 'apply',
        'class': 'btn btn-mini btn-primary',
    }));

    $('#applyAdditionalLocationsButton').on('click', applyAdditionalLocations);

    var trashCans = additionalLocationsList.find('i');

    $.each(trashCans, function (idx, i) {
        $(i).on('click', deleteAddLocInputTabb3);
    });
}
}

var deleteAddLocInputTabb3 = function (event) {

    numberOfAdditionalLocationsTab3--;

    var trashClicked = event.currentTarget.id;
    var idx = trashClicked.substring(0, 1);
    var spanToRemove = locationsSpanPrefix + idx;

    $('#' + spanToRemove).hide(500, function () {
        $(this).remove();
    });

    $('#' + idx + breakSuffix).hide(500, function () {
        $(this).remove();
    });

    if (numberOfAdditionalLocationsTab3 < 1) {
        $('#applyAdditionalLocationsButton').hide(300, function () {
            $(this).remove();
        });
    }
};

var applyAdditionalLocations = function (e) {

    e.preventDefault();

    var self = $(this);

    var adjustAddLocsForm = $('#AdjustAddLocsForm');

    var url = adjustAddLocsForm.attr('action');

    // Ensure array that is sent starts with index 0.
    $.each(adjustAddLocsForm.find('input[type="email"]'), function (idx, value) {
        $(value).attr('name', 'AdditionalLocations[' + idx + '].Email');
    });

    var formData = adjustAddLocsForm.serialize();

    L.clientLogger.info( 'd-#14', { 'applyEditsToAdditionalLocationsTab3': formData } );

$.ajax({
    type: 'POST',
    contentType: RegistrationInCart.Constants.FormPostContentType,
    cache: false,
    url: url,
    dataType: RegistrationInCart.Constants.JsonDataType,
    data: formData,
    beforeSend: function () {
        self.append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
    }
}).done(function (data) {
    if (data.Result === 'Success') {
        var infoLabel = $('#addLocsText');

        var priceLabel = $('#totalAdLocsPrice');
        priceLabel.text('$' + (numberOfAdditionalLocationsTab3 * ADDLOC.price));

        var newText = numberOfAdditionalLocationsTab3 + $.trim(infoLabel.html()).slice(1);

        infoLabel.fadeOut(200, function () {
            infoLabel.html(newText);
            infoLabel.fadeIn(200);
        });

        L.clientLogger.info( 'd-#15', { 'applyEditsToAdditionalLocationsResult': 'edits succeeded' } );

} else {
        L.clientLogger.error( 'd-#16', { 'applyEditsToAdditionalLocationsResult': 'edits failed', 'returnObject': data } );
}
$('#waitSpinner').remove();
});
};
