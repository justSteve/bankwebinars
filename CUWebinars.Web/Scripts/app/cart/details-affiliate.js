//  This script correlates with the DetailsAffiliate View.

var ORDERCREATIONAFFILIATE = {}; // object which lives in Global namespace
var OCA = ORDERCREATIONAFFILIATE; // shortcut alias to ORDERCREATIONAFFILIATE object (to reduce filesize)

OCA.shippingAddressRequired = {};
OCA.checkoutConfirm = {};
OCA.searchBy = '';
OCA.showSetAssignedAffiliate = $('#showSetAssignedAffiliate');
OCA.okToLeave = true;


function timeSince(date) {

    var seconds = Math.floor((new Date() - date) / 1000);

    var interval = Math.floor(seconds / 31536000);

    if (interval > 1) {
        return interval + " years";
    }
    interval = Math.floor(seconds / 2592000);
    if (interval > 1) {
        return interval + " months";
    }
    interval = Math.floor(seconds / 86400);
    if (interval > 1) {
        return interval + " days";
    }
    interval = Math.floor(seconds / 3600);
    if (interval > 1) {
        return interval + " hours";
    }
    interval = Math.floor(seconds / 60);
    if (interval > 1) {
        return interval + " minutes";
    }
    return Math.floor(seconds) + " seconds";
}



OCA.initializeFunctions = function () {

    $('#fireConnInfoSender').on('click', function (eventArgs) {

        var payload = { webinarId: currentWebinarId };
        eventArgs.preventDefault();

        //var logStartOperation = toastLogger.getLogFn('SendConnectionInfo');
        //logStartOperation("Sending ConnectionInfo", null, true);

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Webinar/SendConnectionInfo',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {

                $('#fireConnInfoSender').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            }
        }).done(function (result) {

            labelCheckRemove();

            if (result.Result === 'Success') {
                $('#InputFormFields').append('<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;Messsage Sent</span>');
            } else if (result.Result === 'No Orders to send for that webinar') {
                $('#InputFormFields').append(noOrdersScreenMessage);
            }

        }).fail(function (jqXHR, textStatus, errorThrown) {

            labelCheckRemove();

            $('#InputFormFields').append(failedScreenMessage);
            //Rollbar.error({ 'oen-#23': { 'statusCode': jqXHR && jqXHR.statusCode().status } });
            //Rollbar.error({ 'oen-#24': { 'errorThrown': errorThrown } });

        }).always(function () {
            $('#loadingSpinner').remove();
        });
    });
    $('#submitSynchOrders').on("click", function () {

        var self = this;

        var payload = { webinarId: currentWebinarId, affiliateId: currentAffiliateId };

        var d = new Date();

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/SynchOrders',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).html('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Synching...</span></span>');

            }
        }).done(function (result) {

            if (result.Success === 'Success') {
                $('#spinnerLabel').remove();
                $(self).html('<span>&nbsp;&nbsp;Synch Succeeded</span>');

            } else if (result.Result === 'Fail') {
                $('#spinnerLabel').remove();
                $(self).html('<span>&nbsp;&nbsp;Synch Failed</span>');

            }

        }).fail(function () {
            var a = 1;
        }).always(function () {
            $('#loadingSpinner').remove();
        });
    });


    OCA.hookUpApplyDiscountLogic = function (btn, orderRowId) {

        btn.on('click', function (e) {
            alert("Hit hook");
            e.preventDefault();

            registerDuringCheckout.gatherPricingData();

            if (registerDuringCheckout.totalPrice < 1) {
                return;
            }
            var token = $(this).find('input[name=__RequestVerificationToken]').val();
            var headers = {};
            headers['__RequestVerificationToken'] = token;

            var url = '/cart/ApplyDiscountCode';
            var payload = { code: $('#CheckoutDiscountCode').val(), orderRowId: orderRowId };
            var self = this;

            $.ajax({
                type: 'POST',

                contentType: constants.JsonContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                headers: headers,
                beforeSend: function () {
                    $(self).prepend('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                    $(self).attr('disabled', 'disabled');
                }
            }).done(function (data) {

                if (data.Result == 0) {
                    alert("The discount code " + $('#CheckoutDiscountCode').val() + " was not found or had an error that prevented usage. Try again or use our Help & Feedback button (lower right corner)  for assistance.");
                }
                //                regTypeShort = row.RegistrationType.OptionLabelShort,
                //BasePrice = pricesAndDiscounts.UnitPrice,
                //Discount = pricesAndDiscounts.TotalDiscount,
                //OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                //Tax = pricesAndDiscounts.TaxAmount,
                //Total = pricesAndDiscounts.TotalOrderPrice,
                //FlatOff = pricesAndDiscounts.Discount.FlatOff,
                //PercentOff = pricesAndDiscounts.Discount.PercentOff
                if (data.PercentOff > 0) {

                    registerDuringCheckout.totalDiscount = data.Discount;
                } else {
                    registerDuringCheckout.totalDiscount = data.Discount;
                }

                console.log(data);
                var newTotalPrice = data.Total;

                $("#amount").val(newTotalPrice);

                if (newTotalPrice < 0)
                    newTotalPrice = 0;

                $('#addlocSpiel').text('To add additional locations for this order please email us at  @globalConfig.TenantEmail.').addClass('text-info');

                $('#discountedText').html('Discounted: <span id="totalDiscount">$' + registerDuringCheckout.totalDiscount + '</span>').removeClass('muted');
                $('#showTotalPrice').html('Total Cost: <span id="totalPrice">$' + newTotalPrice.toString() + '.00</span>');
                //$('#totalPriceText').html('Total Cost: <span id="totalPrice">$' + newTotalPrice.toString() + '.00</span>');

                $('#discountSpinner').remove();

            }).fail(commonFuncs.failCallBack).always(function (e) {
                $('#discountSpinner').remove();
                $(self).removeAttr('disabled');
            });
        });
    };

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
            data: JSON.stringify(payLoad)
        }).done(function (data) {

            if (data) {

                $('#baseCost').html('$' + data.BasePrice + '.00');
                $('#totalPriceText').html('Total Cost: <span id="totalPrice">$' + data.Total + '.00</span>');
            }

            dropDown.removeAttr('disabled');
            $('#discountSpinner').remove();

            //if (OCA.shippingAddressrequired
            //    && !OCA.cartStateManager.getAddressVerified(addressVerified)) {
            //    OCA.displayModal($('#UserDetailsModal'));
            //}
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
            //var headers = {};
            //headers['__RequestVerificationToken'] = token;


            var payload = {
                email: $('#AdjustUserDetailsPanel_Email').val(),
                idUser: $('#AdjustUserDetailsPanel_idUser').val(),
                firstname: $('#AdjustUserDetailsPanel_FirstName').val(),
                lastname: $('#AdjustUserDetailsPanel_LastName').val(),
                Institution: $('#AdjustUserDetailsPanel_Institution').val(),
                BillingAddress: $('#AdjustUserDetailsPanel_BillingAddress').val(),
                ShippingAddress: $('#AdjustUserDetailsPanel_Shipping').val()
            };

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                //headers: headers,
                beforeSend: function () {
                    $('#editUserResult').remove();
                    self.after('<span id="userDetailsSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (data) {

                if (data.Result === 'Success') {
                    self.after('<span id="editUserResult">&nbsp;<span class="label label-success"><span> Details updated successfully! </span></span></span>').hide().fadeIn(500);
                } else {
                    //Rollbar.error({ 'editUserResult Failure: ': { data: xhr.data } });

                    self.after('<span id="editUserResult">&nbsp;<span class="label label-warning"><span> There was a problem with that edit. Try again or </span></span></span>').hide().fadeIn(500);
                }

                $('#userDetailsSpinner').remove();

            });
            //    .always(function (data) {
            //    var unused = data;
            //});
        });

        $('#addAnotherAddLoc').on('click', function (e) {

            e.preventDefault();

            var newId;

            if (OCA.numberOfAdditionalLocationsTab3 == 0) {

                $("#additionalLocationsList").after($('<button>',
                {
                    id: 'applyAdditionalLocationsButton',
                    text: 'apply',
                    'class': 'btn btn-mini btn-primary'
                }));

                $('#applyAdditionalLocationsButton').on('click', OCA.applyAdditionalLocations);

                newId = 0;
            } else {
                // first get the last previous email input
                var lastInput = additionalLocationsList.find('input[type="email"]:last');
                // get its id
                var lastInputId = lastInput.attr('id');
                var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
                newId = id + 1;
            }
            $("#additionalLocationsList").append('<span id="' + locationsSpanPrefix + newId + '"><input id="AdditionalLocationEmail_' + newId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-AdditionLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
            $("#additionalLocationsList").find('i#' + newId + '-AdditionLocationEmail-delete').on('click', OCA.deleteAddLocInputTabb3);
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

        //var additionalLocationsList = $('#additionalLocationsList');

        $('#additionalLocationsList').append('<input id="newOrderRowId" name="newOrderRowId"  type="hidden" value=' + OCA.cartStateManager.getOrderRowId() + ' data-val="true" data-val-number="The field newOrderRowId must be a number." data-val-required="The newOrderRowId field is required."/>');
        $('#additionalLocationsList').append(copyOfLocations);

        if (OCA.numberOfAdditionalLocationsTab3 > 0) {
            $("#additionalLocationsList").after($('<button>',
            {
                id: 'applyAdditionalLocationsButton',
                text: 'apply',
                'class': 'btn btn-mini btn-primary'
            }));

            $('#applyAdditionalLocationsButton').on('click', OCA.applyAdditionalLocations);

            var trashCans = $('#additionalLocationsList').find('i');

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
        //this handler only applies to shopping cart AddLoc control - for grid editor see edit-forms-in-child-row
        e.preventDefault();

        var self = $(this);

        var adjustAddLocsForm = $('#AdjustAddLocsForm');

        var url = adjustAddLocsForm.attr('action');

        // Ensure array that is sent starts with index 0.
        $.each(adjustAddLocsForm.find('input[type="email"]'), function (idx, value) {
            $(value).attr('name', 'AdditionalLocations[' + idx + '].Email');
        });

        $('#additionalLocationsList').append('<input id="newOrderRowId" name="newOrderRowId"  type="hidden" value=' + OCA.cartStateManager.getOrderRowId() + ' data-val="true" data-val-number="The field newOrderRowId must be a number." data-val-required="The newOrderRowId field is required."/>');

        var formData = adjustAddLocsForm.serialize();

        console.log(formData);
        $.ajax({
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            cache: false,
            url: url,
            dataType: 'json',
            data: formData,
            beforeSend: function () {
                self.append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (data) {
            console.log(data);

            if (data.Result === 'Success') {

                var infoLabel = $('#addLocsText');
                var newText = $("#numberOfAdditionalLocationsTab3") + $.trim(infoLabel.html()).slice(1);

                infoLabel.fadeOut(200, function () {
                    infoLabel.html(newText);
                    infoLabel.fadeIn(200);
                });


                if (data.Tax > 0) {
                    $('#showTax').removeClass("hidden");
                } else {
                    $('#showTax').addClass("hidden");
                }

                $('#flyUpdateSuccessFlag').html(data.UpdateSuccessCaption).show();
                $('#discountCaption').html(data.DiscountCaption);
                //$('#optionLabel').html(data.regTypeShort);
                $('#baseCost').html('$' + data.BasePrice + '');
                $('#totalDiscount').html('<span id="showDiscount">$' + data.Discount + '');
                $('#taxAmt').html(data.Tax + '');
                $('#totalAdLocsPrice').html('$' + data.OptionsPrice + '');
                $('#totalPrice').html('<span id="totalPrice">$' + data.Total + '</span>');
                $('#additionalLocationsCaption').addClass("hidden");

            } else {
                console.error('Failed to post order');
            }
            $('#waitSpinner').remove();
        });

    };

    OCA.isShippingAddressRequired = function (jQueryObject) {
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
            alert(self.attr('action'));
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
                    //seems unused -- delete?
                    //var orderRowId = data.OrderRowId;

                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    OCA.okToLeave = true;

                    OCA.utilities.goToUrl('/Account/OrderCompleteAffiliate/' + OCA.cartStateManager.getOrderId());

                } else {
                    confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Please try again. For customer service contact us by using the Online Chat button below or emailing @globalConfig.TenantEmail .</span>');
                }

                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                confirmRegistrationBillMe.after('<span class="field-validation-error">Transport error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
            });
        });

        var cancelOrderForm = OCA.cartStateManager.getCancelOrderForm();

        cancelOrderForm.on('submit', function (e) {

            //console.log('cancelOrderForm submit hit');
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
                            //Rollbar.info('Succeeded in cancelling order: ', { data: xhr && xhr.data });

                            OCA.okToLeave = true;

                            var utilities = new Common.Utilities();
                            //console.log('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                            utilities.goToUrl('/webinar/details/' + OCA.cartStateManager.getWebinarId());
                        } else {

                            //Rollbar.error('Cancel Order Failure: ', { data: xhr && xhr.data });

                            confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                            $('#CancelModal').modal('hide');
                        }

                        // enable button again upon ending operation.
                        //$('#cancelRegistration').removeAttr('disabled');  
                        // [dar] NO. On staging, redirect is slow and button enabled again. User could have clicked it again.
                    } else {
                        //Rollbar.error({ 'Cancel Order Failure: ': { data: xhr.data } });

                        confirmRegistrationBillMe.after('<span class="field-validation-error">Server Error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
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
    shippingAddressRequired = "";
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

    //OCA.cartStateManager.setWebinarId(idWebinar); // webinarId is set in a script tab in razor view DetailsAffiliate.cshtml
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

    OCA.wireUpMainButtonsOn3rdTab();

    /* Click event for the big GREEN SignUp button */
    $('#AddToCart').on('click', function () {

        $('#users').collapse('show');

    });

    OCA.setUpEditButtons();

    OCA.populateAdditionalLocationsOn3rdTab();

    OCA.AddOrder = function (e, idUser) {

        $('#idUser').val(idUser);

        formProcessor.clearValidationSummary($('#valSummarySignUpForm'));

        OCA.signUpForm.submit();
    }



    /* Submit event for the big GREEN SignUp button */
    OCA.signUpForm.on('submit', function (e) {

        e.preventDefault();
        $('#users').collapse('hide');
        $('#createNewUserButton').html("<b>processing...</b>");

        $('#confirmationTabForAffiliate a').tab('show');
        var confirmationForAffiliateDiv = $('#confirmationForAffiliate');

        var beigeFormArea = OCA.signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippingAddressRequired function
        //  value comes from a hidden input in the radio btn list next to the relevant radio button (previous-sibling)
        OCA.shippingAddressRequired = OCA.isShippingAddressRequired($('#RegistrationType > dl dt input:checked').prev());


        var data = OCA.signUpForm.serialize();
        console.log(data);
        $('#SignUpFormContainer').before('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        $('#errorAtServer').remove(); // remove error text, if present from a previous fail

        var spinner = $('#loadingSpinner');

        // If the user IS NOT LOGGED IN - direct them to. But they should not be able to make it to this page if they are anonymous.
        // The controller would direct them to the vanilla Details page if anonymous.
        if (!OCA.cartStateManager.getIsUserLoggedIn()) {
            alert('log in');
        } else {
            console.log(OCA.signUpForm.attr('action'));
            // So the user IS LOGGED IN
            $.post(OCA.signUpForm.attr('action'), data, function (response, status, xhr) {

                if (status !== 'error') {
                    switch (xhr.responseJSON['success']) {
                        case "AlreadySubmitted":


                            //if (xhr.responseJSON['success'] == "AlreadySubmitted") {
                            OCA.cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                            OCA.cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                            OCA.cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                            $('#createNewUserButton').html("New User");
                            $('#createNewUserButton').hide();
                            $('#regTabs').html('<div class="text-error">A registration already exists for that email.</div>');
                            $('#regTabs').show();
                            break;
                        case "WasCanceled":
                        case "InProcess":

                            $('#SignUpFormContainer').before('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');

                            OCA.cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                            OCA.cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                            OCA.cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                            $('#createNewUserButton').html("New User");
                            $('#createNewUserButton').hide();

                            confirmationForAffiliateDiv.load('/cart/CheckoutConfirmForAffiliate/' + OCA.cartStateManager.getOrderId(), function (response, status, xhr) {

                                if (status === 'error') {
                                    $(this).html('<div class="text-error">There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.</div>');

                                    $('#confirmationTabForAffiliate a').tab('show');
                                } else {

                                    $('#confirmationTabForAffiliate a').tab('show');
                                    $('#loadingSpinner').remove();
                                    $('#AdjustOrder').hide();

                                    if (OCA.shippingAddressRequired
                                        && !OCA.cartStateManager.getAddressVerified(addressVerified)) {
                                        OCA.displayModal($('#UserDetailsModal'));
                                    }


                                    OCA.checkoutConfirm.initialize();

                                    OCA.wireUpMainButtonsOn3rdTab();

                                    OCA.setUpEditButtons();
                                    OCA.hookUpChangeTypeLogic($('#RegType'));
                                    OCA.hookUpApplyDiscountLogic($('#SubmitDiscountCode'), OCA.cartStateManager.getOrderRowId());

                                    $('#EmailOrderButton').on('click', OCA.emailOrderButtonHandler);

                                    beigeFormArea.height(confirmationForAffiliateDiv.height() + 25);

                                    $('#showSetAssignedAffiliate').removeAttr('disabled');
                                }

                                $('#Canceller').hide();

                            }, constants.HtmlDataType);
                            break;
                        case "success":

                            OCA.cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                            OCA.cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                            OCA.cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                            $('#createNewUserButton').html("New User");
                            $('#createNewUserButton').hide();
                            $('#SignUpFormContainer').before('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');


                            confirmationForAffiliateDiv.load('/cart/CheckoutConfirmForAffiliate/'
                                + OCA.cartStateManager.getOrderId(), function (response, status, xhr) {

                                    if (status === 'error') {
                                        $(this).html('<div class="text-error">There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.</div>');
                                        $('#loadingSpinner').remove();
                                        $('#confirmationTabForAffiliate a').tab('show');
                                    } else {

                                        $('#confirmationTabForAffiliate a').tab('show');

                                        $('#AdjustOrder').hide();

                                        if (OCA.shippingAddressRequired
                                            && !OCA.cartStateManager.getAddressVerified(addressVerified)) {
                                            OCA.displayModal($('#UserDetailsModal'));
                                        }

                                        OCA.checkoutConfirm.initialize();

                                        OCA.wireUpMainButtonsOn3rdTab();

                                        OCA.setUpEditButtons();

                                        OCA.hookUpChangeTypeLogic($('#RegType'));
                                        OCA.hookUpApplyDiscountLogic($('#SubmitDiscountCode'), OCA.cartStateManager.getOrderRowId());

                                        $('#EmailOrderButton').on('click', OCA.emailOrderButtonHandler);

                                        beigeFormArea.height(confirmationForAffiliateDiv.height() + 25);

                                        $('#showSetAssignedAffiliate').removeAttr('disabled');
                                    }

                                    spinner.remove();

                                    $('#Canceller').hide();
                                }, constants.HtmlDataType);
                            break;

                        default:
                            if (xhr.responseJSON['Result'] === 'Fail') {
                                $('#AttendRegTypes').before('<div id="errorAtServer" class="text-error">' + xhr.responseJSON['Msg'] + '.</div>');
                                $('#signUpSpinnerInButton').remove();
                            } else if (xhr.responseJSON['isSuccessful'] === false) {
                                formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);
                                $('#AddToCart').removeAttr('disabled');
                            }
                            //    default code block
                    }

                } else {
                    confirmationForAffiliateDiv.html('<div class="text-error">There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.</div>');
                    spinner.remove();
                }
                spinner.remove();

            }, constants.JsonDataType);

            return false;
        }
        return false;
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
                    //seems unused
                    //var orderRowId = data.OrderRowId;
                    $(self).after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');

                } else {

                    $(self).after('<span class="field-validation-error">Invalid Data. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                }

                $('#emailSendingSpinner').remove();
                $(self).removeAttr('disabled');

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#emailSendingSpinner').remove();
                $(self).removeAttr('disabled');

                $(self).after('<span class="field-validation-error">Transport error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
            });
        });
    };

    //OCA.foundUsersList.hide();

    OCA.setSelectedProduct = function (webUser) {

        OCA.chosenUserNameSpan.text(webUser.firstName + ', ' + webUser.lastName);
        OCA.selectedWebUserInput.val(webUser.id);
        OCA.webUserIdInput.val(webUser.id);
    };

    OCA.displayNewUserModal = function (e) {

        e.preventDefault();

        $(this).attr('disabled', 'disabled').after('<i id="loadSpinner" class="icon-spinner icon-spin"></i>');

        var modalFormOptions = {
            keyboard: true,
            backdrop: 'static',
            show: true
        };
        $('#contactInfoForAffiliate').load('/Cart/CheckoutContactDetails?fromAffCheckout=1', function (response, status, xhr) {

            if (status !== 'error') {

                var addressOptions = {
                    'shippingAddressRequired': OCA.shippingAddressRequired,
                    'notificationsTesting': OCA.cartStateManager.getNotificationsTesting(),
                    'addressVerified': OCA.cartStateManager.getAddressVerified()
                };

                registerDuringCheckout.initialize(OCA.cartStateManager.getOrderId(), OCA.cartStateManager.getWebinarId(), OCA.cartStateManager.getOrderRowId(), OCA.addressOptions, OCA.checkoutConfirm.initialize);
            } else {
                $('#labelEmail').html('<span class="label label-important">Server error #21. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance!</span>');
                L.clientLogger.error('d-#28', { 'responseObject': xhr.responseJSON, 'anonymousUserSubmit': 'Fail condition.' });
            }
            $('#loadSpinner').remove();
        });

        $('#contactInfoTab a').tab('show');
        $('#addNewUserModal ').on('hidden', function (e) {
            //$('#submitChangeAffilate').off('click');
            modalFormOptions = null;
            $('#cancelCreateUserButton').text('Cancel');
            $('#newUserResultLabel').remove();
            if ($('#idUser').val() > 0) {
                OCA.signUpForm.submit();
            }
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

                    if (data.Result === 'Success') {
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

        L.clientLogger.info('da-#1', { 'User error': 'User attempted to abandoned order', OrderId: OCA.cartStateManager.getOrderId() || 'No order id available yet' });

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
                //Rollbar.error('#348 userDetailsFormUrl: ' + userDetailsFormUrl, { data: data });
                $('#updateShippingMsgLabelWrap').empty();
                formProcessor.lightUpValidationSummary('userDetailsValSummary', data);
            } else {
                //Rollbar.error('#348 userDetailsFormUrl Post to ' + userDetailsFormUrl + ' !data.isSuccessful');

                $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>[Connection Error #348] For customer service contact us by using the Online Chat button below or emailing @globalConfig.TenantEmail .</span>');
            }
        }).fail(function (data) {

            //Rollbar.error('FAIL: Post to userDetailsFormUrlData ' + userDetailsFormUrlData, { data: data });

            $('#updateShippingMsgLabelWrap').html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>Connection Error #048] For customer service contact us by using the Online Chat button below or emailing @globalConfig.TenantEmail .</span>');
        });
    });

    //$('#passwordWrapper').remove();

    //if ($('#RegisterFields_Password').length < 1) {
    //    $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields_Password" name="RegisterFields.Password" value="456rty^Y" />');
    //    $('#userDetailsForm').prepend('<input type="hidden" id="RegisterFields.ConfirmPassword" name="RegisterFields.ConfirmPassword" value="456rty^Y" />');
    //}
};
