//  This script correlates with the DetailsAffiliate View.

var ORDERCREATIONAFFILIATE = {}; // object which lives in Global namespace
var OCA = ORDERCREATIONAFFILIATE; // shortcut alias to ORDERCREATIONAFFILIATE object (to reduce filesize)

OCA.shippingAddressRequired = {};
OCA.checkoutConfirm = {};
OCA.searchBy = '';
OCA.showSetAssignedAffiliate = $('#showSetAssignedAffiliate');
OCA.okToLeave = false;

OCA.initializeFunctions = function () {

    OCA.hookUpChangeTypeLogic = function (dropDown) {

        dropDown.on('change', function (e) {

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
                beforeSend: function () {
                    dropDown.attr('disabled', 'disabled').after('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                }
            }).done(function (data) {

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

            if (OCA.shippingAddressRequired && !OCA.cartStateManager.getNotificationsTesting(notificationsTesting) && !OCA.cartStateManager.getAddressVerified(addressVerified)) {
                OCA.displayModal($('#UserDetailsModal'));
            }
        });
    };

    OCA.setUpEditButtons = function () {


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
                }

                $('#userDetailsSpinner').remove();

            }).always(function (data) {
            });
        });

        $('#addAnotherAddLoc').on('click', function (e) {

            e.preventDefault();

            var newId;

            if (OCA.numberOfAdditionalLocationsTab3 == 0) {

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
            additionalLocationsList.find('i#' + newId + '-AdditionLocationEmail-delete').on('click', OCA.deleteAddLocInputTabb3);
            $('#AdditionalLocationEmail_' + newId).focus();
            OCA.numberOfAdditionalLocationsTab3++;
        });
    };

    OCA.populateAdditionalLocationsOn3rdTab = function () {

        if (!locationsSpanPrefix) {
            locationsSpanPrefix = 'LocationSpan-',
            breakSuffix = '-break';
        }

        var addLocsOn1stTabContainer = $('#collectAdditionalLocations');
        var locations = addLocsOn1stTabContainer.children();
        OCA.numberOfAdditionalLocationsTab3 = locations.filter('span').length;

        var copyOfLocations = locations.clone();

        addLocsOn1stTabContainer.remove();

        var additionalLocationsList = $('#additionalLocationsList');

        additionalLocationsList.append('<input id="newOrderRowId" name="newOrderRowId"  type="hidden" value=' + cartStateManager.getOrderRowId() + ' data-val="true" data-val-number="The field newOrderRowId must be a number." data-val-required="The newOrderRowId field is required."/>');
        additionalLocationsList.append(copyOfLocations);

        if (OCA.numberOfAdditionalLocationsTab3 > 0) {
            additionalLocationsList.after($('<button>',
            {
                id: 'applyAdditionalLocationsButton',
                text: 'apply',
                'class': 'btn btn-mini btn-primary',
            }));

            $('#applyAdditionalLocationsButton').on('click', applyAdditionalLocations);

            var trashCans = additionalLocationsList.find('i');

            $.each(trashCans, function (idx, i) {
                $(i).on('click', OCA.deleteAddLocInputTabb3);
            });
        }
    };

    OCA.deleteAddLocInputTabb3 = function (event) {

        OCA.numberOfAdditionalLocationsTab3--;

        var trashClicked = event.currentTarget.id;
        var idx = trashClicked.substring(0, 1);
        var spanToRemove = locationsSpanPrefix + idx;

        $('#' + spanToRemove).hide(500, function () {
            $(this).remove();
        });

        $('#' + idx + breakSuffix).hide(500, function () {
            $(this).remove();
        });

        if (OCA.numberOfAdditionalLocationsTab3 < 1) {
            $('#applyAdditionalLocationsButton').hide(300, function () {
                $(this).remove();
            });
        }
    };

    OCA.applyAdditionalLocations = function (e) {

        e.preventDefault();

        var self = $(this);

        var adjustAddLocsForm = $('#AdjustAddLocsForm');

        var url = adjustAddLocsForm.attr('action');

        // Ensure array that is sent starts with index 0.
        $.each(adjustAddLocsForm.find('input[type="email"]'), function (idx, value) {
            $(value).attr('name', 'AdditionalLocations[' + idx + '].Email');
        });

        var formData = adjustAddLocsForm.serialize();

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
                var newText = numberOfAdditionalLocationsTab3 + $.trim(infoLabel.html()).slice(1);

                infoLabel.fadeOut(200, function () {
                    infoLabel.html(newText);
                    infoLabel.fadeIn(200);
                });

            } else {
                console.error('Failed to post order');
            }
            $('#waitSpinner').remove();
        });

    };

    OCA.isShippindAddressRequired = function (jQueryObject) {
        if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
            return false;
        return true;
    };

    OCA.wireUpMainButtonsOn3rdTab = function () {
        // The Bill Me button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function (e) {
            e.preventDefault();
            var confirmOrderForm = $('#confirmOrderForAffiliateForm');
            confirmOrderForm.submit();
        });

        // The Cancel Registration button on 3rd tab
        $('#Canceller').on('click', function (e) {
            e.preventDefault();
            var cancelOrderForm = $('#cancelOrder');
            cancelOrderForm.submit();
        });


        // The 'TO PAY BY CREDIT CARD' button on 3rd tab
        $('#ConfirmRegistrationPayByCC').on('click', function (e) {
            e.preventDefault();

            var orderId = OCA.cartStateManager.getOrderId();
            var url = '/Cart/PayCC/' + orderId;

            OCA.utilities.goToUrl(url);

        });
    };

    OCA.displayModal = function (modalForm) {

        modalForm.modal('show');

        modalForm.on('shown', modalShown);

        modalForm.on('hidden', function (e) {
            $('#saveChangesButton').off('click');
            $('#userDetailsForm').off('submit');
        });
    };

    OCA.hookUpEditUserLogic = function (button) {

        var modalForm = $('#UserDetailsModal');

        button.on('click', function (e) {

            e.preventDefault();

            OCA.displayModal(modalForm);
        });
    };

    /* This function gets invoked when the 3rd tab is loaded and an existing user is using the cart */
    OCA.checkoutConfirm.initialize = function (userId) {

        OCA.cartStateManager.setCancelOrderForm($('#cancelOrder'));
        OCA.cartStateManager.setConfirmOrderForm($('#confirmOrderForAffiliateForm'));
        var confirmRegistrationBillMe = $('#ConfirmRegistrationBillMe');

        OCA.cartStateManager.getConfirmOrderForm().on('submit', function (e) {

            e.preventDefault();

            if (OCA.selectedWebUserInput.val() == OCA.adminCreatedUserInput.val()) {
                $('#userCreatedByAdmin').val(true);
            }

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
                beforeSend: function () {
                    confirmRegistrationBillMe.attr('disabled', 'disabled');
                    //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
                }
            }).done(function (data) {
                if (data.Result === 'Success') {
                    orderRowId = data.OrderRowId;

                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    OCA.okToLeave = true;

                    OCA.utilities.goToUrl('/Account/OrderCompleteAffiliate/' + OCA.cartStateManager.getOrderId());

                } else {
                    confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                confirmRegistrationBillMe.after('<span class="field-validation-error">Transport error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
            });
        });

        var cancelOrderForm = OCA.cartStateManager.getCancelOrderForm();

        cancelOrderForm.on('submit', function (e) {

            console.log('cancelOrderForm submit hit');
            //Rollbar.info('Submitting cancelOrderForm');
            e.preventDefault();
            e.stopImmediatePropagation();

            $('#cancelModalOrderId').val(OCA.cartStateManager.getOrderId());
            var data = $(this).serialize();

            var self = $(this);

            $('#cancelRegistration').on('click', function (e) {
                e.preventDefault();

                // disable button while operation in progress
                $(this).attr('disabled', 'disabled');
                $('#CancelModal').find('div[class="modal-footer"]').prepend('<span id="cancelSpinner"><span>&nbsp;<i class="icon icon-spinner icon-spin"></i></span></span>');

                $.post(self.attr('action'), data, function (response, status, xhr) {

                    if (status !== 'error') {
                        if (xhr.responseJSON['success']) {
                            Rollbar.info('Succeeded in cancelling order: ', { data: xhr && xhr.data });

                            OCA.okToLeave = true;

                            var utilities = new Common.Utilities();
                            console.log('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                            utilities.goToUrl('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                        } else {
                            
                            Rollbar.error( 'Cancel Order Failure: ', { data: xhr && xhr.data });

                            confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                        // enable button again upon ending operation.
                        //$('#cancelRegistration').removeAttr('disabled');  // [dar] NO. On staging, redirect is slow and button enabled again. User could have clicked it again.
                    } else {
                        Rollbar.error({ 'Cancel Order Failure: ': { data: xhr.data } });

                        confirmRegistrationBillMe.after('<span class="field-validation-error">Server Error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                        $('#CancelModal').modal('hide');
                    }

                    $('#cancelSpinner').remove();
                }, 'json');

                // unbind event so we don't get them building up each time the user clicks the Cancel Registration button.
                $(this).off('click');
                $('#rtn').off('click');
            });

            $('#rtn').on('click', function (e) {
                e.preventDefault();
                $('#CancelModal').modal('hide');
                $(this).off('click');
                $('#cancelRegistration').off('click');
            });

            $('#CancelModal').modal('show');

        });

        $('#previewEmailButton').attr('href', '/Cart/PreviewEmail/' + OCA.cartStateManager.getOrderRowId());

    };
};

OCA.initializeState = function () {

    OCA.signUpForm = $('#affiliateSignUpForm');
    OCA.signUpFormContainer = $('#SignUpFormContainer'); // The big beige box
    OCA.lastNameInput = $('#lastName');
    OCA.searchWebUsersButton = $('#searchWebUsersButton');
    OCA.foundUsersList = $('#userResults');
    OCA.selectedWebUserInput = $('#SelectedWebUser');
    OCA.chosenUserNameSpan = $('#chosenUserName');
    OCA.webUserIdInput = $('#idUser');
    OCA.createNewUserButton = $('#createNewUserButton');
    OCA.adminCreatedUserInput = $('#adminCreatedUser');
    OCA.users = {};

    OCA.cartStateManager = new OrderRegistration.StateManager();

    OCA.cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tab in razor view DetailsAffiliate.cshtml
    OCA.cartStateManager.setAddressVerified(addressVerified); // addressVerified is set in a script tag in razor view Details.cshtml
    OCA.cartStateManager.setNotificationsTesting(notificationsTesting); // notificationsTesting is set in a script tag in razor view Details.cshtml


    OCA.cartStateManager.SetCartState('affiliate');
    OCA.numberOfAdditionalLocationsTab3 = 0;

    OCA.utilities = new Common.Utilities();
};

OCA.wireUpHandlers = function () {

    /* Click event for the big GREEN SignUp button */
    $('#AddToCart').on('click', function () {

        var chosenUserName = $('#chosenUserName');

        if (chosenUserName.length < 1 || !chosenUserName.is(':visible')) {
            alert('You must select a user using the textbox to the left of the Sign Up button.');
            return;
        }

        $(this).append('<i id="signUpSpinnerInButton" class="icon-spinner icon-spin"></i>');

        formProcessor.clearValidationSummary($('#valSummarySignUpForm'));

        OCA.signUpForm.submit();
    });

    /* Submit event for the big GREEN SignUp button */
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

        $('#SignUpFormContainer').before('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        $('#errorAtServer').remove(); // remove error text, if present from a previous fail

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

                                if (OCA.shippingAddressRequired && !OCA.cartStateManager.getNotificationsTesting(notificationsTesting) && !OCA.cartStateManager.getAddressVerified(addressVerified)) {

                                    OCA.displayModal($('#UserDetailsModal'));
                                }

                                OCA.hookUpChangeTypeLogic($('#RegType'));

                                OCA.checkoutConfirm.initialize();

                                OCA.wireUpMainButtonsOn3rdTab();

                                OCA.setUpEditButtons();

                                $('#EmailOrderButton').on('click', OCA.emailOrderButtonHandler);

                                beigeFormArea.height(confirmationForAffiliateDiv.height() + 25);

                                $('#showSetAssignedAffiliate').removeAttr('disabled');
                            }

                            spinner.remove();

                        }, constants.HtmlDataType);
                    } else if (xhr.responseJSON['Result'] === 'Fail') {
                        $('#AttendRegTypes').before('<div id="errorAtServer" class="text-error">' + xhr.responseJSON['Msg'] + '.</div>');
                        $('#signUpSpinnerInButton').remove();
                    } else if (xhr.responseJSON['isSuccessful'] === false) {
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);
                        $('#AddToCart').removeAttr('disabled');
                    }
                    spinner.remove();
                } else {
                    confirmationForAffiliateDiv.html('<div class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance.</div>');
                    spinner.remove();
                }
                spinner.remove();

            }, constants.JsonDataType);

            return false;
        }
        return false;
    });

    //var searchPeople = _.debounce(function( query, process ){

    //    OCA.foundUsersList.hide();

    //    var searchTerm = OCA.lastNameInput.val();

    //    $.ajax({
    //        type: 'GET',
    //        contentType: constants.FormPostContentType,
    //        cache: false,
    //        url: '/Cart/SearchWebUsers',
    //        dataType: constants.JsonDataType,
    //        data: { lastName: searchTerm },
    //        beforeSend: function () {
    //            OCA.users = null; // dereference whatever is currently in 'users'. 

    //            // this is where we append a loading image
    //            //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
    //        }
    //    }).done(function (data) {
    //        OCA.users = data.people;

    //        var results = _.map(OCA.users, function (user) {
    //            return user.id;
    //        });
    //        process(results);
    //    });

    //    }, 500);

    var searchPeople = _.debounce(function (query, process) {

        OCA.foundUsersList.hide();

        var searchTerm = OCA.lastNameInput.val();

        if (searchTerm === '')
            return;
        if (searchTerm.indexOf('@') == 0) {
            // in here if searching for an email
            searchBy = 'emailDomain';
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByEmailDomain',
                dataType: constants.JsonDataType,
                data: { email: searchTerm },
                beforeSend: function () {
                    OCA.users = null; // dereference whatever is currently in 'OCA.users'. 
                }
            }).done(function (data) {

                OCA.users = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(OCA.users);

            });

        }
        else if (searchTerm.indexOf('@') > 1) {
            // in here if searching for an email
            searchBy = 'email';
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByEmailTypeahead',
                dataType: constants.JsonDataType,
                data: { email: searchTerm },
                beforeSend: function () {
                    OCA.users = null; // dereference whatever is currently in 'OCA.users'. 
                }
            }).done(function (data) {

                OCA.users = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(OCA.users);

            });

        } else if (_.isFinite(searchTerm)) {
            // in here if searching on an order number
            searchBy = 'orderID';
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByTypeahead',
                dataType: constants.JsonDataType,
                data: { id: searchTerm },
                beforeSend: function () {
                    OCA.users = null; // dereference whatever is currently in 'OCA.users'. 
                }
            }).done(function (data) {

                OCA.users = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(OCA.users);
            });
        } else {
            // if not a number and not an email address, search is by lastname
            searchBy = 'lastName';
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByLastName',
                dataType: constants.JsonDataType,
                data: { lastName: searchTerm },
                beforeSend: function () {
                    OCA.users = null; // dereference whatever is currently in 'OCA.users'. 
                }
            }).done(function (data) {

                OCA.users = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(OCA.users);
            });
        }

    }, 200);
    //}).done(function (data) {
    //    OCA.users = data.people;

    //    var results = _.map(OCA.users, function (user) {
    //        return user.id;
    //    });
    //    process(results);
    //});

    //}, 500);


    OCA.lastNameInput.typeahead({
        source: function (query, process) {
            searchPeople(query, process);
        },

        matcher: function (item) {
            return true;
        },

        highlighter: function (listedUser) {
            var item = JSON.parse(listedUser);
            var user = _.find(OCA.users, function (webUser) {
                return JSON.parse(webUser)['id'] === item.id;
            });
            if (user !== null && typeof user !== 'undefined') {
                var userParsed = JSON.parse(user);
                if (searchBy == "email") {
                    return userParsed.email + ', ' + userParsed.lastName;
                }
                if (searchBy == "orderID") {
                    return userParsed.email + ', ' + userParsed.lastName;
                }
                if (searchBy == "emailDomain") {
                    return userParsed.institution + ', ' + userParsed.email;
                }
                if (searchBy == "lastName") {
                    return userParsed.lastName + ', ' + userParsed.firstName;
                }

            }
        },

        sorter: function (items) {
            return items;
        },

        updater: function (userJson) {
            var userParsed = JSON.parse(userJson);
            var user = _.find(OCA.users, function (p) {
                return JSON.parse(p)['id'] === userParsed['id'];
            });

            if (typeof user !== 'undefined') {
                var parsedUser = JSON.parse(user);
                OCA.setSelectedProduct(parsedUser);
                return parsedUser['lastName'] + ', ' + parsedUser['firstName'];
            }
            return '';
        }

    });

    OCA.emailOrderButtonHandler = function (e) {

        e.preventDefault();

        $('#emailPanel').fadeIn();

        $('#dispatchButton').on('click', function (e) {

            e.preventDefault();

            var self = this;
            var payload = { emails: $('#EmailAddressesInput').val() };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Cart/EmailOrder/' + OCA.cartStateManager.getOrderRowId(),
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $(self).prepend('<i id="emailSendingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                    $(self).attr('disabled', 'disabled');
                    if ($('#resultLabel').length > 0)
                        $('#resultLabel').remove();
                }
            }).done(function (data) {
                if (data.Result === 'Success') {
                    orderRowId = data.OrderRowId;
                    $(self).after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');

                } else {

                    $(self).after('<span class="field-validation-error">Invalid Data. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
                }

                $('#emailSendingSpinner').remove();
                $(self).removeAttr('disabled');

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#emailSendingSpinner').remove();
                $(self).removeAttr('disabled');

                $(self).after('<span class="field-validation-error">Transport error. Try again or call 800-831-0678 ext 706 for immediate assistance! </span>');
            });
        });
    };

    OCA.foundUsersList.hide();

    OCA.setSelectedProduct = function (webUser) {

        OCA.chosenUserNameSpan.text(webUser.firstName + ', ' + webUser.lastName);
        OCA.selectedWebUserInput.val(webUser.id);
        OCA.webUserIdInput.val(webUser.id);
    };

    OCA.displayNewUserModal = function(e) {

        e.preventDefault();

        $(this).attr('disabled', 'disabled').after('<i id="loadSpinner" class="icon-spinner icon-spin"></i>');

        var modalFormOptions = {
            keyboard: true,
            backdrop: 'static',
            show: true
        };

        $('#addNewUserModal > div.modal-body').load('/Account/GetAddUserFieldsForModal', function(data) {

            $('#addNewUserModal').modal(modalFormOptions);

            $('#confirmCreateUserButton').on('click', function(e) {

                e.preventDefault();

                var self = this;

                var form = $('#adminAddUserForm');
                var url = form.attr('action');

                $('#ShippingAddress_Name').val($('#BillingAddress_Name').val());
                $('#ShippingAddress_StreetAddress').val($('#BillingAddress_StreetAddress').val());
                $('#ShippingAddress_StreetAddress2').val($('#BillingAddress_StreetAddress2').val());
                $('#ShippingAddress_City').val($('#BillingAddress_City').val());
                $('#ShippingAddress_State').val($('#BillingAddress_State').val());
                $('#ShippingAddress_Zip').val($('#BillingAddress_Zip').val());
                $('#ShippingAddress_Country').val($('#BillingAddress_Country').val());
                $('#ShippingAddress_Phone').val($('#BillingAddress_Phone').val());
                $('#ShippingAddress_TypeOfAddress').val('Shipping');
                $('#BillingAddress_TypeOfAddress').val('Billing');

                var data = form.serialize();

                $.ajax({
                    type: 'POST',
                    contentType: constants.FormPostContentType,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    data: data,
                    beforeSend: function() {
                        $(self).attr('disabled', 'disabled').after('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
                        //Rollbar.info("addPasswordForm Sent");

                    }
                }).done(function(data, textStatus, jqXHR) {
                    $('#loadingSpinner').remove();
                    OCA.createNewUserButton.removeAttr('disabled');

                    if (data.Result === "Success") {
                        OCA.adminCreatedUserInput.val(data.UserId);
                        OCA.selectedWebUserInput.val(data.UserId);
                        $('#userCreatedByAdmin').val(true);
                        OCA.chosenUserNameSpan.text($('#NewUserFirstName').val() + ' ' + $('#NewUserLastName').val());
                        OCA.lastNameInput.attr('disabled', 'disabled');
                        $('#confirmCreateUserButton').removeAttr('disabled');
                        $('#cancelCreateUserButton').text('close');
                        $(self).before('<span id="newUserResultLabel">&nbsp;<span class="label label-success">User created successully!</span>&nbsp;</span>');
                    }
                });
            });

        });

        $('#addNewUserModal ').on('hidden', function (e) {
            //$('#submitChangeAffilate').off('click');
            modalFormOptions = null;
            $('#cancelCreateUserButton').text('Cancel');
            $('#newUserResultLabel').remove();
        });

        $('#addNewUserModal ').on('shown', function (e) {
            $('#loadSpinner').remove();
        });
    }

    OCA.createNewUserButton.on('click', OCA.displayNewUserModal);

    OCA.displaySetAffiliateModal = function (link) {
        
        // permits Admin to choose which affiliate will be credited with order

        var modalFormOptions = {
            keyboard: true,
            backdrop: 'static',
            show: true
        };

        $(link).after('<span id="loadAffModal">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        $('#frmChangeAffiliate > div.modal-body').load('/Admin/GetAffiliates', function () {

            $('#SelectedAffiliate').val($('#idAffiliate').text());

            $('#setAssignedAffiliate').modal(modalFormOptions);

            $('#submitChangeAffilate').on('click', function (e) {

                e.preventDefault();

                var self = this;

                var payload = {
                    idAffiliate: $('#SelectedAffiliate').val(),
                    idOrder: OCA.cartStateManager.getOrderId()
                };

                var url = $('#frmChangeAffiliate').attr('action');

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
                    beforeSend: function () {
                        $(self).append('<span id="changeAffSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                        $(self).attr('disabled', 'disabled');
                        $('#resultLabel').remove();
                    }
                }).done(function (data) {

                    if(data.Result === 'Success') {
                        $('#setAssignedAffiliate').find('div[class="modal-footer"]').prepend('<span id="resultLabel" class="label label-success"><span>&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Affiliate updated successfully</span>&nbsp;</span>');
                        $(self).removeAttr('disabled');
                        $('#changeAffSpinner').remove();
                        
                        $('#showSetAssignedAffiliate').text('Selected Affiliate: ' + $('#SelectedAffiliate :selected').text());
                    }
                });
            });
        });

        $('#setAssignedAffiliate').on('hidden', function (e) {
            $('#submitChangeAffilate').off('click');
            modalFormOptions = null;
            $('#resultLabel').remove();
        });

        $('#setAssignedAffiliate').on('shown', function (e) {
            $('#loadAffModal').remove();
            $('#resultLabel').remove();
        });

    };

    OCA.showSetAssignedAffiliate.on('click', function (e) {
        e.preventDefault();
        OCA.displaySetAffiliateModal(this);
    });
};

// $(document).ready function
$(function () {

    OCA.initializeFunctions();

    OCA.initializeState();

    OCA.wireUpHandlers();

    $(window).on('beforeunload', function (e) {
        
        //  taken from this SO answer http://stackoverflow.com/a/7317311/540156
        if (OCA.okToLeave) {
            return undefined;
        }

        L.clientLogger.info('da-#1', { 'User error': 'User attempted to abandoned order', OrderId: OCA.cartStateManager.getOrderId() || 'No order id available yet'});

        var confirmationMessage = 'It looks like you have been creating an order.\r\n';
        OCA.cartStateManager.getOrderId() && (confirmationMessage += 'OrderId: ' + OCA.cartStateManager.getOrderId() + '.\r\n');
        confirmationMessage += 'If you leave before completing the order, your changes will be lost.\r\n';
        confirmationMessage += 'Are you sure you want to abandon this order?';
        
        return confirmationMessage; 
    });
});


function modalShown(e) {

    $('#updateShippingMsgLabelWrap').empty();

    $('#saveChangesButton').on('click', function () {
        e.preventDefault();

        $('#userDetailsForm').submit();
    });

    $('#userDetailsForm').on('submit', function (e) {
        e.preventDefault();

        var url = $(this).attr('action');

        var payload = $(this).serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            data: payload,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            beforeSend: function (xhr) {
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

                $('#updateShippingMsgLabelWrap').html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details updated successfully.</span>');

            } else if (!data.isSuccessful) {
                Rollbar.error('#348 userDetailsFormUrl: ' + userDetailsFormUrl, { data: data });
                $('#updateShippingMsgLabelWrap').empty();
                formProcessor.lightUpValidationSummary('userDetailsValSummary', data);
            } else {
                Rollbar.error('#348 userDetailsFormUrl Post to ' + userDetailsFormUrl + ' !data.isSuccessful');
                
                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>[Connection Error #348] Please call us at 800-831-0678 ext. 3 to resolve.</span>');
            }
        }).fail(function (data) {

            Rollbar.error('FAIL: Post to userDetailsFormUrlData ' + userDetailsFormUrlData, { data: data });
            
            $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Connection Error #048] Please call us at 800-831-0678 ext. 3 to resolve.</span>');
        });
    });

    $('#passwordWrapper').remove();

    if ($('#RegisterFields_Password').length < 1) {
        $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields_Password" name="RegisterFields.Password" value="456rty^Y" />');
        $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields.ConfirmPassword" name="RegisterFields.ConfirmPassword" value="456rty^Y" />');
    }
};
