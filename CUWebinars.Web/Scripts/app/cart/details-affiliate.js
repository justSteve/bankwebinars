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

        $('#revealAddLocsPanel').on('click', function(e) {
            e.preventDefault();
            $('#AdjustAddLoc').slideToggle();
        });
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

    OCA.signUpForm = $('#SignUpForm');
    OCA.signUpFormContainer = $('#SignUpFormContainer'); // The big beige box
    OCA.lastNameInput = $('#lastName');
    OCA.searchWebUsersButton = $('#searchWebUsersButton');
    OCA.foundUsersList = $('#userResults');
    OCA.users = {};

    OCA.cartStateManager = new OrderRegistration.StateManager();

    OCA.cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tab in razor view DetailsAffiliate.cshtml

    OCA.cartStateManager.SetCartState('affiliate');

};

OCA.wireUpHandlers = function() {

    /*  not reqd as adlocs off table */
    // click event for the RadioButton group
    //$("[id^='regTypeID_']").on("click", function (oEvent) {

    //    OCA.cartStateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);

    //});

    /* Click event for the big green SignUp button */
    $('#AddToCart').on('click', function () {
        $(this).attr('disabled', 'disabled');
        OCA.signUpForm.submit();
    });

    /* Submit event for the big green SignUp button */
    OCA.signUpForm.on('submit', function (e) {
        e.preventDefault();

        var beigeFormArea = OCA.signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippindAddressRequired function
        //  value comes from a hidden input in the radio btn list next to the relevant radio button (previous-sibling)
        //OCA.shippingAddressRequired = OCA.isShippindAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = OCA.signUpForm.serialize();

        $('#SignUpForm > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
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

                        $('#confirmation').load('/cart/checkoutConfirm/' + OCA.cartStateManager.getOrderRowId(), function (response, status, xhr) {

                            if (status === 'error') {
                                $(this).html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                                $('#loadingSpinner').remove();
                                $('#confirmationTab a').tab('show');
                            } else {

                                $('#confirmationTab a').tab('show');

                                if (OCA.shippingAddressRequired && notificationsTesting === false) {
                                    // Following function lives in the register-during-checkout.js script
                                    // which will be in memory at this point and thus will have been hoisted.
                                    hookUpModal($('#UserDetailsModal'));
                                }

                                // see top of this file
                                OCA.checkoutConfirm.initialize();

                                OCA.wireUpMainButtonsOn3rdTab();

                                OCA.setUpEditButtons();

                                beigeFormArea.height($('#confirmation').height() + 25);
                            }

                            spinner.remove();

                        }, constants.HtmlDataType);
                    } else if (xhr.responseJSON['isSuccessful'] === false) {
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);

                        spinner.remove();
                    }
                } else {
                    spinner.remove();
                    $('#confirmation').html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                }
            }, constants.JsonDataType);

            return false;
        }
        return false;
    });

    OCA.lastNameInput.typeahead({
        source: function (query, process) {
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
                    // this is where we append a loading image
                    //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
                }
            }).done(function (data) {
                OCA.users = data.people;
            });

            var results = _.map(OCA.users, function (user) {
                return { id: user.id, lastname: user.lastname, firstname: user.firstname };
            });
            process(results);
        },

        matcher: function (item) {
            return true;
        },

        highlighter: function (obj) {
            var user = _.find(OCA.users, function (webUser) {
                return webUser.id == obj['id'];
            });
            return user.lastname + ', ' + user.firstname;
        },

        sorter: function (items) {
            return items;
        },

        updater: function (id) {
            var user = _.find(OCA.users, function (p) {
                return p.id == id['id'];
            });
            OCA.setSelectedProduct(user);
            return user.name;
        }

    });

    OCA.foundUsersList.hide();
    OCA.setSelectedProduct = function(webUser) {
        OCA.foundUsersList.html(webUser.firstname + ', ' + webUser.lastname).show();
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

