//  This script correlates with the DetailsAffiliate View.

var ORDERCREATIONAFFILIATE = {}; // object which lives in Global namespace
var OCA = ORDERCREATIONAFFILIATE; // shortcut alias to ORDERCREATIONAFFILIATE object (to reduce filesize)

OCA.shippingAddressRequired = {};
OCA.checkoutConfirm = {};

OCA.initializeFunctions = function () {

    //OCA.hookUpChangeTypeLogic = function(dropDown, shippingAddressRequired) {

    //    //var changeTypeConfirmModal = $('#changeTypeConfirmModal');
    //    var chosenRegTypeLabel = $('#chosenRegType');
    //    var position,
    //        typeChosenCurrent,
    //        typeChosenPrevious,
    //        valOfTypeChosenPrevious,
    //        valOfTypeChosenCurrent;

    //    dropDown.on('change', function(e) {

    //        e.preventDefault();

    //        valOfTypeChosenCurrent = $(this).val();
    //        var totalPrice = 0;

    //        var url = '/Cart/CheckIfAddLocShouldHide?optionID=' + valOfTypeChosenCurrent;

    //        $.ajax({
    //            type: 'GET',
    //            contentType: constants.FormPostContentType,
    //            cache: false,
    //            url: url,
    //            dataType: constants.JsonDataType,
    //            beforeSend: function() {
    //                dropDown.attr('disabled', 'disabled').after('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
    //            }
    //        }).done(function(data) {

    //            // see CartController's CheckIfAddLocShouldHide method for commented explanation regarding the 'shouldShow' property.
    //            if (data.shouldShow === 'No') {

    //                registerDuringCheckout.gatherPricingData(); /* BUTTONS DIFFERENT HERE*/

    //                //  First, check if there are currently any Additional Locations added to the order.
    //                if (anyAddLocs === true) {

    //                    position = $('#confirmation').offset();

    //                    var url = '/Cart/RemoveAdditionalLocationsFromOrder';
    //                    var payLoad = {
    //                        idOrderRow: OCA.cartStateManager.getOrderRowId()
    //                    };

    //                    $.ajax({
    //                        type: 'POST',
    //                        contentType: constants.JsonContentType,
    //                        cache: false,
    //                        url: url,
    //                        dataType: constants.JsonDataType,
    //                        data: JSON.stringify(payLoad),
    //                    }).done(function(data) {

    //                        if (data.Result === 'Success') {
    //                            // This next variable is initially set in the CheckoutConfirm.cshtml razor view
    //                            anyAddLocs = false;
    //                            $('#additionalLocationsCaption').html('None');

    //                            $('#addlocSpiel').text('To add additional locations for this order, please call 800-831-0678 ext 706 for immediate assistance').addClass('text-info');

    //                            $('#addLocsText').html('Additional Locations: <span id="totalAdLocsPrice">$0.00</span>').addClass('muted');
    //                            totalPrice = registerDuringCheckout.totalPrice - registerDuringCheckout.addLocsPrice;

    //                            updatePriceOnNewSelection(valOfTypeChosenCurrent, totalPrice, dropDown);
    //                        }
    //                    });

    //                    valOfTypeChosenCurrent = valOfTypeChosenPrevious = dropDown.val();
    //                    typeChosenCurrent = typeChosenPrevious = $.trim($('#RegType option:selected').text());
    //                    chosenRegTypeLabel.empty().text(typeChosenCurrent);
    //                } else {
    //                    typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
    //                    chosenRegTypeLabel.empty().text(typeChosenCurrent);
    //                    valOfTypeChosenPrevious = valOfTypeChosenCurrent;

    //                    updatePriceOnNewSelection(valOfTypeChosenCurrent, registerDuringCheckout.totalPrice, dropDown);
    //                }
    //            } else {
    //                typeChosenPrevious = typeChosenCurrent = $.trim($('#RegType option:selected').text());
    //                chosenRegTypeLabel.empty().text(typeChosenCurrent);
    //                valOfTypeChosenPrevious = valOfTypeChosenCurrent;
    //                updatePriceOnNewSelection(valOfTypeChosenCurrent, registerDuringCheckout.totalPrice, dropDown);
    //            }

    //            if (data.shippingDetailsRqrd === 'Yes') {
    //                registerDuringCheckout.shippingAddressRequired = true;
    //            } else {
    //                registerDuringCheckout.shippingAddressRequired = false;
    //            }
    //        });
    //    });
    //};

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
            var confirmOrderForm = $('#confirmOrder');
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
                        //var fullname = $('#ShippingAddress_Name').val();

                        //var userNameInsuranceAndButton = $('#userDetailsSummed > p:nth-child(1)');
                        //userNameInsuranceAndButton.empty();
                        //userNameInsuranceAndButton.html(fullname + ' - ' + $('#RegisterFields_Institution').val() + '<br> ' + $('#RegisterFields_Email').val() + ' - <a id="editUserDetails" role="button" class="btn btn-mini" target="new"> Edit?</a>');

                        $('#editUserDetails').on('click', function (e) {

                            e.preventDefault();

                            OCA.displayModal(modalForm);
                        });

                        $('#updateShippingMsgLabelWrap').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details updated successfully.</span>');
                        //modalForm.modal('hide');
                        // [sjh] - i might surmise that this is an attempt to fix the above bug 

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
    OCA.checkoutConfirm.initialize = function(userId) {
        OCA.cartStateManager.setCancelOrderForm($('#cancelOrder'));
        OCA.cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        console.log('initialize hit');

        OCA.cartStateManager.getConfirmOrderForm().on('submit', function(e) {
            //Rollbar.info('submitting confirmOrder form');
            e.preventDefault();

            var self = $(this);
            self.find('input[name="id"]').val(OCA.cartStateManager.getOrderRowId());
            var err = new Error('submitting ConfirmOrder' + OCA.cartStateManager.getOrderRowId());
            //NREUM.noticeError(err);

            var data = $(this).serialize();
            $('#ConfirmRegistrationBillMe').prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');

            $.ajax({
                type: 'POST',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                url: self.attr('action'),
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: data,
                beforeSend: function() {
                    $('#ConfirmRegistrationBillMe').attr('disabled', 'disabled');
                    //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
                }
            }).done(function(data) {
                if (data.Result === 'Success') {
                    orderRowID = data.OrderRowID;

                    var err = new Error('Posted Order: ' + orderRowID);
                    //NREUM.noticeError(err);
                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    $('#ConfirmModal').modal('show');

                } else {

                    var err = new Error('FAILED posting Order: ');
                    //NREUM.noticeError(err);

                    $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                $('#ConfirmRegistrationBillMe').removeAttr('disabled');
            }).fail(function(jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                $('#ConfirmRegistrationBillMe').removeAttr('disabled');
                $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Transport error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
            });

            $('#ConfirmModal').on('hidden', function(e) {
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

                            $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                        // enable button again upon ending operation.
                        $('#cancelRegistration').removeAttr('disabled');
                    } else {
                        $('#ConfirmRegistrationBillMe').after('<span class="field-validation-error">Server Error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
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

                                // see top of this file
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

