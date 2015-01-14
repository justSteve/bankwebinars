//  This script correlates with the DetailsAffiliate View.

var ORDERCREATIONAFFILIATE = {}; // object which lives in Global namespace
var OCA = ORDERCREATIONAFFILIATE; // shortcut alias to ORDERCREATIONAFFILIATE object (to reduce filesize)

OCA.shippingAddressRequired = {};
OCA.checkoutConfirm = {};

OCA.initializeFunctions = function () {

    OCA.hookUpChangeTypeLogic = function(dropDown) {
        
        dropDown.on('change', function(e) {

            e.preventDefault();

            var valOfTypeChosenCurrent = $(this).val();
            var totalPrice = 0;

            var url = '/Cart/CheckIfAddLocShouldHide?optionID=' + valOfTypeChosenCurrent;

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                beforeSend: function() {
                    dropDown.attr('disabled', 'disabled').after('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                }
            }).done(function(data) {

                // see CartController's CheckIfAddLocShouldHide method for commented explanation regarding the 'shouldShow' property.
                if (data) {

                    if (data.shippingDetailsRqrd === 'Yes') {
                        OCA.shippingAddressRequired = true;
                    } else {
                        OCA.shippingAddressRequired = false;
                    }

                    OCA.updatePriceOnNewSelection(valOfTypeChosenCurrent, totalPrice, dropDown);
                }
            });
        });
    };

    // This function's purpose is to update pricing details where the RegType DropDown has its selected value changed.
    // It also displays the Shipping Details modal form where the RegType chosen has a shipping address requirement.
    OCA.updatePriceOnNewSelection = function (registrationTypeId, totalPrice, dropDown) {

        var url = '/Cart/UpdateOrderDetails';

        var payLoad = {
            idOrderRow: OCA.cartStateManager.getOrderRowId(),
            idRegType: registrationTypeId
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payLoad),
        }).done(function (data) {

            if (data) {

                $('#baseCost').html('$' + data.BasePrice + '.00');
                $('#totalPriceText').html('Total Cost: <span id="totalPrice">$' + data.Total + '.00</span>');
            }

            dropDown.removeAttr('disabled');
            $('#discountSpinner').remove();

            if (OCA.shippingAddressRequired) {
                OCA.displayModal($('#UserDetailsModal'));
            }
        });
    };

    OCA.setUpEditButtons = function() {
        $('#revealOptions').on('click', function(e) {
            e.preventDefault();
            $('#AdjustOrder').slideToggle();
        });

        OCA.hookUpEditUserLogic($('#editUserDetails'));
    };

    OCA.isShippindAddressRequired = function(jQueryObject) {
        if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
            return false;
        return true;
    };

    OCA.wireUpMainButtonsOn3rdTab = function() {
        // The Bill Me button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function(e) {
            e.preventDefault();
            var confirmOrderForm = $('#confirmOrderForAffiliateForm');
            confirmOrderForm.submit();
        });

        // The Cancel Registration button on 3rd tab
        $('#Canceller').on('click', function(e) {
            e.preventDefault();
            var cancelOrderForm = $('#cancelOrder');
            cancelOrderForm.submit();
        });
    };

    OCA.displayModal = function(modalForm) {

        modalForm.modal('show');
    };

    OCA.setUiLayout = function(isShippindAddressRequired) {
        console.log('isShippindAddressRequired fires');
        if (isShippindAddressRequired) {
            $('#ShippingAddressContainer').hide();
            $('#HideAddShippingAddressLink').hide();
            $('#linksToAddShippingFields').show();
            $('#AddShippingAddressLink').show();
        } else {
            $('#ShippingAddressContainer, #linksToAddShippingFields, #AddShippingAddressLink').hide();
        }

        $('#updateShippingMsgLabelWrap').empty();
    };

    OCA.hookUpEditUserLogic = function (button) {

        var modalForm = $('#UserDetailsModal');

        button.on('click', function (e) {

            e.preventDefault();

            OCA.displayModal(modalForm);
        });

        modalForm.on('shown', function (e) {

            OCA.setUiLayout(OCA.shippingAddressRequired);

            $('#saveChangesButton').on('click', function () {
                e.preventDefault();

                $('#userDetailsForm').submit();
            });

            $('#userDetailsForm').on('submit', function (e) {
                e.preventDefault();

                var url = $(this).attr('action');

                var payload = $(this).serialize();
                //TODO: The payload here is highly valuable. The fact that they will be POSTed
                // instead of GET means they don't show in the server logs, which is often nice to have.
                // can we fire a GET to an internal address where nothing happens except that
                // a line dumps to the server with all form values.
                $.ajax({
                    type: 'POST',
                    contentType: constants.FormPostContentType,
                    data: payload,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    beforeSend: function(xhr){
                        $('#updateShippingMsgLabelWrap').html('<span class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Updating details...</span>');

                        var valSummary = $('#userDetailsValSummary');
                        valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                        var errorsList = valSummary.find('ul');
                        errorsList.empty();
                        errorsList.append('<li style="display:none"></li>');

                    }
                }).done(function (data) {
                    //BUG:  A submitted order prematurely turns off the 'in-process' spinner.
                    // Intermittent - Can't reproduce. Is same as was earlier reported where the lag between
                    //  new user submission and the point where panel 3 displays is quite long
                    //  and does not display any 'in-process' spinner
                    // I think the cause would have to be rooted here over very near here.

                    if (data.Result === 'Success') {

                        $('#editUserDetails').on('click', function (e) {

                            e.preventDefault();

                            OCA.displayModal(modalForm);
                        });

                        $('#updateShippingMsgLabelWrap').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details updated successfully.</span>');

                    } else if (!data.isSuccessful) {
                        //var err = new Error('Post to ' + userDetailsFormUrl + ' !data.isSuccessful');
                        //NREUM.noticeError(err);

                        $('#updateShippingMsgLabelWrap').empty();
                        formProcessor.lightUpValidationSummary('userDetailsValSummary', data);
                    } else {
                        var err = new Error('Post to ' + userDetailsFormUrl + ' !data.isSuccessful');
                        //NREUM.noticeError(err);
                        $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>There has been an error in the operation.Please call us at 800-831-0678 ext. 3 to resolve.</span>');
                    }
                }).fail(function (data) {
                    var err = new Error('FAIL: Post to userDetailsFormUrlData ' + userDetailsFormUrlData + ' !data.isSuccessful');
                    //NREUM.noticeError(err);
                    $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Error in the server response. Please call us at 800-831-0678 ext. 3 to resolve.</span>');
                });
            });

            $('#HideAddShippingAddressLink').on('click', function (e) {
                e.preventDefault();

                $(this).fadeOut('500', function () {
                    $('#ShippingAddressContainer').fadeOut('500', function () {
                        $('#AddShippingAddressLink').fadeIn('500');
                    });
                });

            });

            $('#AddShippingAddressLink').on('click', function (e) {
                e.preventDefault();

                $(this).fadeOut('500', function () {
                    $('#ShippingAddressContainer').fadeIn('500', function () {
                        $('#HideAddShippingAddressLink').fadeIn('500');
                    });
                });
            });

            $('#passwordWrapper').remove();

            if ($('#RegisterFields_Password').length < 1) {
                $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields_Password" name="RegisterFields.Password" value="456rty^Y" />');
                $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields.ConfirmPassword" name="RegisterFields.ConfirmPassword" value="456rty^Y" />');
            }
        });

        modalForm.on('hidden', function (e) {
            $('#saveChangesButton').off('click');
            $('#userDetailsForm').off('submit');
        });
    };

    /* This function gets invoked when the 3rd tab is loaded and an existing user is using the cart */
    OCA.checkoutConfirm.initialize = function (userId) {

        OCA.cartStateManager.setCancelOrderForm($('#cancelOrder'));
        OCA.cartStateManager.setConfirmOrderForm($('#confirmOrderForAffiliateForm'));
        var confirmRegistrationBillMe = $('#ConfirmRegistrationBillMe');
        var confirmModal = $('#ConfirmModal');

        OCA.cartStateManager.getConfirmOrderForm().on('submit', function(e) {
            
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(OCA.cartStateManager.getOrderRowId());

            var data = $(this).serialize();
            confirmRegistrationBillMe.prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                cache: false,
                url: self.attr('action'),
                dataType: constants.JsonDataType,
                data: data,
                beforeSend: function() {
                    confirmRegistrationBillMe.attr('disabled', 'disabled');
                    //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
                }
            }).done(function(data) {
                if (data.Result === 'Success') {
                    orderRowID = data.OrderRowID;

                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    confirmModal.modal('show');

                } else {
                    confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');

            }).fail(function(jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                confirmRegistrationBillMe.after('<span class="field-validation-error">Transport error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
            });

            confirmModal.on('hidden', function (e) {
                var utilities = new Common.Utilities();
                console.log('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                utilities.goToUrl('/webinar/details/' + OCA.cartStateManager.getWebinarId());
            });
        });

        var cancelOrderForm = OCA.cartStateManager.getCancelOrderForm();

        cancelOrderForm.on('submit', function(e) {

            console.log('cancelOrderForm submit hit');
            //Rollbar.info('Submitting cancelOrderForm');
            e.preventDefault();
            e.stopImmediatePropagation();

            $('#cancelModalOrderId').val(OCA.cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);

            $('#cancelRegistration').on('click', function(e) {
                e.preventDefault();

                // disable button while operation in progress
                $('#cancelRegistration').attr('disabled', 'disabled');

                $.post(self.attr('action'), data, function(response, status, xhr) {

                    if (status !== 'error') {
                        if (xhr.responseJSON['success']) {
                            var err = new Error('Suceeded in cancelling order');
                            //NREUM.noticeError(err);

                            var utilities = new Common.Utilities();
                            console.log('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                            utilities.goToUrl('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                        } else {
                            var err = new Error('Cancel Order Failure');
                            //NREUM.noticeError(err);

                            confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                        // enable button again upon ending operation.
                        $('#cancelRegistration').removeAttr('disabled');
                    } else {
                        confirmRegistrationBillMe.after('<span class="field-validation-error">Server Error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                        $('#CancelModal').modal('hide');
                    }
                }, 'json');

                // unbind event so we don't get them building up each time the user clicks the Cancel Registration button.
                $(this).off('click');
                $('#rtn').off('click');
            });

            $('#rtn').on('click', function(e) {
                e.preventDefault();
                $('#CancelModal').modal('hide');
                $(this).off('click');
                $('#cancelRegistration').off('click');
            });

            $('#CancelModal').modal('show');

        });
    };
};

OCA.initializeState = function() {

    OCA.signUpForm = $('#affiliateSignUpForm');
    OCA.signUpFormContainer = $('#SignUpFormContainer'); // The big beige box
    OCA.lastNameInput = $('#lastName');
    OCA.searchWebUsersButton = $('#searchWebUsersButton');
    OCA.foundUsersList = $('#userResults');
    OCA.selectedWebUserInput = $('#SelectedWebUser');
    OCA.chosenUserNameSpan = $('#chosenUserName');
    OCA.webUserIdInput= $('#idUser');
    OCA.users = {};

    OCA.cartStateManager = new OrderRegistration.StateManager();

    OCA.cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tab in razor view DetailsAffiliate.cshtml

    OCA.cartStateManager.SetCartState('affiliate');

};

OCA.wireUpHandlers = function() {

    /* Click event for the big green SignUp button */
    $('#AddToCart').on('click', function () {
        $(this).attr('disabled', 'disabled');
        OCA.signUpForm.submit();
    });

    /* Submit event for the big green SignUp button */
    OCA.signUpForm.on('submit', function (e) {
        e.preventDefault();

        var confirmationForAffiliateDiv = $('#confirmationForAffiliate');

        var beigeFormArea = OCA.signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippindAddressRequired function
        //  value comes from a hidden input in the radio btn list next to the relevant radio button (previous-sibling)
        OCA.shippingAddressRequired = OCA.isShippindAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = OCA.signUpForm.serialize();

        $('#affiliateSignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        var spinner = $('#loadingSpinner');

        // If the user IS NOT LOGGED IN - direct them to. But they should not be able to make it to this page if they are anonymous.
        // The controller would direct them to the vanilla Details page if anonymous.
        if (!OCA.cartStateManager.getIsUserLoggedIn()) {
            alert('log in');
        } else {
            // So the user IS LOGGED IN
            $.post(OCA.signUpForm.attr('action'), data, function (response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {
                        OCA.cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        OCA.cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        OCA.cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);

                        confirmationForAffiliateDiv.load('/cart/CheckoutConfirmForAffiliate/' + OCA.cartStateManager.getOrderRowId(), function (response, status, xhr) {

                            if (status === 'error') {
                                $(this).html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                                $('#loadingSpinner').remove();
                                $('#confirmationTabForAffiliate a').tab('show');
                            } else {

                                $('#confirmationTabForAffiliate a').tab('show');

                                $('#AdjustOrder').hide();

                                if (OCA.shippingAddressRequired && notificationsTesting === false) {
                                    OCA.displayModal($('#UserDetailsModal'));
                                }

                                OCA.hookUpChangeTypeLogic($('#RegType'));
                                
                                OCA.checkoutConfirm.initialize();

                                OCA.wireUpMainButtonsOn3rdTab();

                                OCA.setUpEditButtons();

                                beigeFormArea.height(confirmationForAffiliateDiv.height() + 25);
                            }

                            spinner.remove();

                        }, constants.HtmlDataType);
                    } else if (xhr.responseJSON['isSuccessful'] === false) {
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);

                        spinner.remove();
                    }
                } else {
                    spinner.remove();
                    confirmationForAffiliateDiv.html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                }
            }, constants.JsonDataType);

            return false;
        }
        return false;
    });
    
    var searchPeople = _.debounce(function( query, process ){

        OCA.foundUsersList.hide();

        var searchTerm = OCA.lastNameInput.val();

        $.ajax({
            type: 'GET',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/Cart/SearchWebUsers',
            dataType: constants.JsonDataType,
            data: { lastName: searchTerm },
            beforeSend: function () {
                OCA.users = null; // dereference whatever is currently in 'users'. 

                // this is where we append a loading image
                //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
            }
        }).done(function (data) {
            OCA.users = data.people;

            var results = _.map(OCA.users, function (user) {
                return user.id;
            });
            process(results);
        });

        }, 200);

    OCA.lastNameInput.typeahead({
        source: function (query, process) {
            searchPeople(query, process);
        },

        matcher: function (item) {

            return true;
        },

        highlighter: function (id) {
            var user = _.find(OCA.users, function (webUser) {
                return webUser.id === id;
            });

            if (user !== null && typeof user !== 'undefined') {
                return user.lastname + ', ' + user.firstname;
            }
        },

        sorter: function (items) {
            return items;
        },

        updater: function (id) {
            var user = _.find(OCA.users, function (p) {
                return p['id'] == id;
            });

            if (typeof user !== 'undefined') {
                OCA.setSelectedProduct(user);
            }
        }

    });

    OCA.foundUsersList.hide();

    OCA.setSelectedProduct = function (webUser) {

        OCA.chosenUserNameSpan.text(webUser.firstname + ', ' + webUser.lastname);
        OCA.selectedWebUserInput.val(webUser.id);
        OCA.webUserIdInput.val(webUser.id);
    };

};

// $(document).ready function
$(function () {

    OCA.initializeFunctions();

    OCA.initializeState();

    OCA.wireUpHandlers();

    //  flow goes inside this block where the order exists and is in process e.g. previously abandoned before finializing
    if (OCA.cartStateManager.getOrderRowId() > 0 && OCA.cartStateManager.getCheckoutInProcess()) {

        OCA.cartStateManager.setOrderId(orderId);

        // see top of this file
        OCA.checkoutConfirm.initialize();

        OCA.wireUpMainButtonsOn3rdTab();

        OCA.setUpEditButtons();
    }
});

