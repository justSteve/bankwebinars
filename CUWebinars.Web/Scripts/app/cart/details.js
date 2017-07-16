//  This script correlates with the Details View.


var desCheckout, userHasDiscount, additionalLocationsList, cCLocationsList, checkoutConfirm, discount, cartStateManager, okToLeave, shippingAddressRequired, signUpForm, signUpFormContainer, storedHeight, numberOfCcLocationsTab3, numberOfAdditionalLocationsTab3;

discount = '';
checkoutConfirm = {};
okToLeave = true;
pagetitle = $("h1:first").text();
userHasDiscount = false;

function getInternetExplorerVersion()
// Returns the version of Internet Explorer or a -1
// (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.
    if (navigator.appName === 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) !== null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}
var ieVer = "";
function checkVersion() {

    var ver = getInternetExplorerVersion();

    if (ver > -1) {
        if (ver >= 9.0)
            ieVer = "";
        else
            ieVer = "preIE10";
    }
}


$(function () {
    checkVersion();

    if (ieVer === "preIE10")
        $("#iePre10").show();

    signUpForm = $('#SignUpForm');

    signUpFormContainer = $('#SignUpFormContainer'); // The big beige box

    // This function gets invoked when the 3rd tab is loaded and an existing user is using the cart
    checkoutConfirm.initialize = function (userId) {

        cartStateManager.setCancelOrderForm($('#cancelOrder'));
        cartStateManager.setConfirmOrderForm($('#confirmOrder'));

        cartStateManager.getConfirmOrderForm().on('submit', function (e) {

            if (desCheckout) {
                //$('#ConfirmRegistrationBillMe').hide();
                $('#ContinueShoppingButton').hide();
                $('#linkIsOrderForCoworker').hide();
                $('#revealAddLocsPanel').hide();
                $('#revealDiscountInput').hide();
                $('#revealOptions').hide();
                $('#AttendRegTypesCaption').text("Subscription Overview");
            }
            e.preventDefault();
            var self = $(this);
            self.find('input[name="id"]').val(cartStateManager.getOrderRowId());

            var data = $(this).serialize();
            var confirmRegistrationBillMe = $('#ConfirmRegistrationBillMe');
            var utilities = new Common.Utilities();


            $.ajax({
                type: 'POST',
                contentType: RegistrationInCart.Constants.FormPostContentType,
                cache: false,
                // form is submitted to Cart/ConfirmOrder
                url: self.attr('action'),
                dataType: RegistrationInCart.Constants.JsonDataType,
                data: data,
                beforeSend: function () {

                    confirmRegistrationBillMe.prepend('<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                    confirmRegistrationBillMe.attr('disabled', 'disabled');
                }
            }).done(function (data) {
                if (data.Result === 'Success') {

                    $('#orderDetails').empty();
                    $('#orderDetails').append(data.Msg);

                    $('#orderStatusLabel').text("Submitted").removeClass('label-warning').addClass('label-success');

                    confirmRegistrationBillMe
                        .after('<span>&nbsp;<span class="label label-success">&nbsp;<i id="finalLoadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;Order Confirmed!</span></span>');

                    okToLeave = true;
                    utilities.goToUrl('/Account/OrderComplete/' + data.OrderRowID);

                } else if (data.Result === 'UserHasMulti') {

                    utilities.goToUrl('/Cart/Checkout');
                } else {
                    //console.error('Failed to post order');

                    confirmRegistrationBillMe.after('<span class="field-validation-error">Invalid Data #554. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                }

                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                $('#signUpSpinner').remove();
            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#finalLoadingSpinner').remove();
                confirmRegistrationBillMe.removeAttr('disabled');
                $('#signUpSpinner').remove();
                confirmRegistrationBillMe.after('<span class="field-validation-error">Transport error #555. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');

            });

        });
        var cancelOrderForm = cartStateManager.getCancelOrderForm();

        $('#CancelModal').on('show', function (e) {

            $('#cancelModalOrderId').val(cartStateManager.getOrderId());

            var data = cancelOrderForm.serialize();

            $('#cancelRegistrationbtn').on('click', function (e) {

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
                //this.off('click');
                $('#rtn').off('click');
            });
        });

    };

    cartStateManager = new OrderRegistration.StateManager();

    // values set in razor view assigned to member of cartStateManager
    cartStateManager.setWebinarId(webinarId); // webinarId is set in a script tag in razor view Details.cshtml
    cartStateManager.setOrderRowId(orderRowId); // orderRowId is set in the razor view Details.cshtml
    cartStateManager.setIsUserLoggedIn(isUserLoggedIn); // isUserLogged is set in a script tag in razor view Details.cshtml
    cartStateManager.setUserHasDiscount(userHasDiscount); // isUserLogged is set in a script tag in razor view Details.cshtml
    cartStateManager.setCheckoutInProcess(checkoutInProcess); // checkoutInProcess is set in a script tag in razor view Details.cshtml
    cartStateManager.setAddressVerified(addressVerified); // addressVerified is set in a script tag in razor view Details.cshtml
    cartStateManager.setNotificationsTesting(notificationsTesting); // notificationsTesting is set in a script tag in razor view Details.cshtml


    cartStateManager.SetCartState();
    $('#SendHardcopy').on('click', applySendHardcopy);

    $("[id^='regTypeID_']").on("click", function (oEvent) {
        cartStateManager.CheckIfAddLocShouldHide(oEvent.currentTarget.value);
    });

    $('#addGmail').on('click', function (e) {

        e.preventDefault();
        var frmAddGmail = $('#frmAddGmail');
        $("#glWebinar").val(webinarId);
        $("#glOrder").val(orderRowId);
        frmAddGmail.submit();
    });

    $('#addICS').on('click', function (e) {

        e.preventDefault();
        var frmAddIcs = $('#frmAddIcs');
        $("#icsWebinar").val(webinarId);
        $("#icsOrder").val(orderId);
        frmAddIcs.submit();
    });


    // Click event for the BIG GREEN SignUp button
    $('#AddToCart').on('click', function () {
        $(this).prepend('<i id="signUpSpinner" class="icon-spinner icon-spin"></i>').attr('disabled', 'disabled');
        signUpForm.submit();
    });


    // Flow goes inside this block where the order exists and is in process e.g. previously abandoned before finializing
    if (cartStateManager.getOrderRowId() > 0 && cartStateManager.getCheckoutInProcess()) {
        if (shippingAddressRequired && !cartStateManager.getNotificationsTesting() && !cartStateManager.getAddressVerified()) {
            // Following function lives in the register-during-checkout.js script
            // which will be in memory at this point and thus will have been hoisted.
            hookUpModal($('#UserDetailsModal'));
        }

        cartStateManager.setOrderId(orderId);
        // see top of this file
        checkoutConfirm.initialize();

        //The BIG GREEN 'Bill Me' button on 3rd tab
        $('#ConfirmRegistrationBillMe').on('click', function (e) {

            e.preventDefault();
            var confirmOrderForm = $('#confirmOrder');
            confirmOrderForm.submit();
        });


        // The grey CANCEL Registration button on 3rd tab       
        $('#Canceller').on('click', function (e) {
            e.preventDefault();

            $('#CancelModal').modal('show');
        });

        populateAdditionalLocationsOn3rdTab();
        populateCcLocationsOn3rdTab();
        setUpEditButtons();

        // Following 3 functions live in the register-during-checkout.js script
        // which will be in memory at this point. So these functions will be hoisted.
        hookUpApplyDiscountLogic($('#SubmitDiscountCode'), cartStateManager.getOrderRowId());
        hookUpChangeTypeLogic($('#RegType'));
        hookUpAddLocsLogic($('#AddLocs'));
        hookUpEditUserLogic(null, shippingAddressRequired);
    }

    /* Submit event for the big green SIGNUP button */
    signUpForm.on('submit', function (e) {

        e.preventDefault();
        $('#EventDescription').slideToggle();
        window.scrollTo(0, 0);
        var beigeFormArea = signUpFormContainer.find('div.well');

        $('#loginEmail').val($('#Email1').val());
        $('#loginPassword').val($('#Password1').val());

        //  value converted to a Boolean in isShippingAddressRequired function
        //  value comes from a hidden input in the radio btn list next to the relevant radio button (previous-sibling)
        shippingAddressRequired = isShippingAddressRequired($('#RegistrationType > dl dt input:checked').prev());

        var data = signUpForm.serialize();


        var spinner = $('#signUpSpinner');
        $('#SignUpFormContainer > div').prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>');
        var loadingSpinner = $('#loadingSpinner');

        // If the user IS NOT LOGGED IN - control moves to the register-during-checkout.js script
        if (!cartStateManager.getIsUserLoggedIn()) {

            //Account/Signup2
            $.post(signUpForm.attr('action'), data, function (response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {
                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);
                        $('#contactInfo').load('/Cart/CheckoutContactDetails', function (response, status, xhr) {
                            if (status !== 'error') {
                                $('#_CreateUserForm input[name="returnUrl"]').val('/Webinar/Details/' + cartStateManager.getWebinarId());

                                var addressOptions = {
                                    'shippingAddressRequired': shippingAddressRequired,
                                    'notificationsTesting': cartStateManager.getNotificationsTesting(),
                                    'addressVerified': cartStateManager.getAddressVerified()
                                };

                                registerDuringCheckout.initialize(cartStateManager.getOrderId(), cartStateManager.getWebinarId(), cartStateManager.getOrderRowId(), addressOptions, checkoutConfirm.initialize);
                            } else {
                                $('#labelEmail').html('<span class="label label-important">Server error #21. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance!</span>');

                            }
                            loadingSpinner.remove();
                        });

                        $('#contactInfoTab a').tab('show');
                    } else if (!xhr.responseJSON['isSuccessful']) {
                        $('#labelEmail').html('<span class="label label-important">&nbsp;There were some problems with the form. Please refer to the items in red.</span>');
                        formProcessor.lightUpValidationSummary('valSummarySignUpForm', xhr.responseJSON);
                        loadingSpinner.remove();

                    }
                } else {
                    $('#labelEmail').html('<span class="label label-important">&nbsp;Server error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance!</span>');
                    loadingSpinner.remove();

                }
            }, constants.JsonDataType);
        } else {
            // If the user IS LOGGED IN
            $.post(signUpForm.attr('action'), data, function (response, status, xhr) {
                if (status !== 'error') {
                    if (xhr.responseJSON['success']) {

                        cartStateManager.setOrderRowId(xhr.responseJSON['orderRowId']);
                        cartStateManager.setOrderId(xhr.responseJSON['orderId']);
                        cartStateManager.setWebinarId(xhr.responseJSON['webinarId']);


                        $('#confirmation').load('/cart/checkoutConfirm/' + cartStateManager.getOrderId(), function (response, status, xhr) {

                            if (status === 'error') {
                                $(this).html('<div class="text-error">There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.</div>');
                                $('#loadingSpinner').remove();
                                $('#confirmationTab a').tab('show');


                            } else {
                                if (DiscountSurcharge === "A $50 surcharge is added for shipping & handling") {
                                    alert("Note that a $50 surcharge is added for shipping & handling");
                                }
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
                                $('#ConfirmRegistrationBillMe').on('click', function (e) {
                                    e.preventDefault();
                                    var confirmOrderForm = $('#confirmOrder');
                                    confirmOrderForm.submit();
                                });
                                
                                // The Cancel Registration button on 3rd tab
                                $('#Canceller').on('click', function (e) {
                                    e.preventDefault();

                                    $('#CancelModal').modal('show');
                                });

                                populateAdditionalLocationsOn3rdTab();
                                populateCcLocationsOn3rdTab();
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

                    }
                } else {
                    spinner.remove();
                    $('#confirmation').html('<div class="text-error">There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.</div>');
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

        var confirmationMessage = 'It looks like you have been creating an order.\r\n';
        cartStateManager.getOrderId() && (confirmationMessage += 'OrderId: ' + cartStateManager.getOrderId() + '.\r\n');
        confirmationMessage += 'If you leave before completing the order, your changes will be lost.\r\n';
        confirmationMessage += 'Are you sure you want to abandon this order?';

        return confirmationMessage;
    });
});

function isShippingAddressRequired(jQueryObject) {
    //todo: re-enable
    //if ($.trim(jQueryObject.val()).toLowerCase() === 'false')
    return false;
    //return true;
}

function setUpEditButtons() {

    $('#addCcLoc').on('click', function (e) {

        e.preventDefault();

        var newId;
        console.log(numberOfCcLocationsTab3);
        if (numberOfCcLocationsTab3 === 0) {

            if ($("#cCLocationsList").length === 1) {
                $("#cCLocationsList").after($('<button>',
                    {
                        id: 'applycCLocationsButton',
                        text: 'apply',
                        'class': 'btn btn-mini btn-primary'
                    }));

                $('#applycCLocationsButton').on('click', applyCcLocations);
            }
            newId = 0;
        } else {

            $('#applycCLocationsButton').on('click', applyCcLocations);
            // first get the last previous email input
            var lastInput = $("#cCLocationsList").find('input[type="email"]:last');
            // get its id

            console.log(lastInput);
            var lastInputId = lastInput.attr('id');
            var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
            newId = id + 1;
        }


        $("#cCLocationsList").append('<span id=cCLocationSpan-"' + newId + '"><input id="CcLocationEmail_' + newId + '" name="cCLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-CcLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
        $("#cCLocationsList").find('i#' + newId + '-CcLocationEmail-delete').on('click', deleteAddLocInputTabb3);
        $('#CcLocationEmail_' + newId).focus();
        numberOfCcLocationsTab3++;
    });

    $('#addAnotherAddLoc').on('click', function (e) {
        e.preventDefault();

        var newId;

        if (numberOfAdditionalLocationsTab3 === 0) {
            if ($("#additionalLocationsList").length === 1) {
                $("#additionalLocationsList").after($('<button>',
                    {
                        id: 'applyAdditionalLocationsButton',
                        text: 'apply',
                        'class': 'btn btn-mini btn-primary'
                    }));

                $('#applyAdditionalLocationsButton').on('click', applyAdditionalLocations);
            }
            newId = 0;
        } else {

            // first get the last previous email input
            var lastInput = $("#additionalLocationsList").find('input[type="email"]:last');
            // get its id

            var lastInputId = lastInput.attr('id');
            var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
            newId = id + 1;
        }
        $("#additionalLocationsList").append('<span id="' + locationsSpanPrefix + newId + '"><input id="AdditionalLocationEmail_' + newId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-AdditionLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
        $("#additionalLocationsList").find('i#' + newId + '-AdditionLocationEmail-delete').on('click', deleteAddLocInputTabb3);
        $('#AdditionalLocationEmail_' + newId).focus();
        numberOfAdditionalLocationsTab3++;
    });

    if (desCheckout) {

        //('#ConfirmRegistrationBillMe').hide();
        $('#ContinueShoppingButton').hide();
        $('#linkIsOrderForCoworker').hide();
        $('#revealAddLocsPanel').hide();
        $('#revealDiscountInput').hide();
        $('#discountedText').hide();
        $('#revealOptions').hide();
        $('#AttendRegTypesCaption').text("Terms & Agreement");
    }
    $('#revealOptions').on('click', function (e) {
        e.preventDefault();

        $('#AdjustOrder').slideToggle();
    });

    $('#revealDiscountInput').on('click', function (e) {
        e.preventDefault();
        $('#AdjustDiscount').slideToggle();
    });
    $('#closeApplyCode').on('click', function (e) {
        e.preventDefault();
        $('#AdjustDiscount').slideToggle();
    });

    $('#revealAddLocsPanel').on('click', function (e) {
        e.preventDefault();

        $('#AdjustAddLoc').slideToggle(400, function () { wireUpHandlers(); });



    });
    $('#editUserDetails').on('click', function (e) {
        e.preventDefault();
        $('#AdjustUserDetails').slideToggle(400, function () { $('#editUserResult').remove(); });
    });


}

function populateAdditionalLocationsOn3rdTab() {

    // There is re-use involved with additional locations as they can be manipulated on either the 1st or 3rd tab. 
    // Hence, the locationsSpanPrefix may already exist in some scenarios.
    if (!locationsSpanPrefix) {
        locationsSpanPrefix = 'LocationSpan-',
            breakSuffix = '-break';
    }
    console.log($('#collectAdditionalLocations'));
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
                text: 'done adding?',
                'class': 'btn btn-mini btn-primary'
            }));

        $('#applyAdditionalLocationsButton').on('click', applyAdditionalLocations);

        var trashCans = additionalLocationsList.find('i');

        $.each(trashCans, function (idx, i) {
            $(i).on('click', deleteAddLocInputTabb3);
        });
        console.log("locations: " + locations);
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

    //if (numberOfAdditionalLocationsTab3 < 1) {
    //    $('#applyAdditionalLocationsButton').hide(300, function () {
    //        $(this).remove();
    //    });
    //}
};

var applySendHardcopy = function (e) {
    
    e.preventDefault();
    var self = $(this);

    console.log(self);

    var url = "/cart/SendHardcopy";
    
    var formData = {
        idOrder: cartStateManager.getOrderId(),
    };

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
        if (data) {
            console.log(data);
            $('#flyUpdateSuccessFlag').html(data.UpdateCaption).show();
            alert(data.UpdateCaption);
            
        } else {
            alert("Failed to update");
        }
        $('#waitSpinner').remove();
    });
};

var applyCcLocations = function (e) {

    e.preventDefault();

    var self = $(this);

    var adjustCcLocsForm = $('#AdjustCcLocsForm');

    var url = adjustCcLocsForm.attr('action');
    var addresses = "";
    $.each(adjustCcLocsForm.find('input[type="email"]'), function () {
        addresses += $(this).val() + ",";
    });

    var formData = {
        idOrder: cartStateManager.getOrderId(),
        addresses: addresses
    };

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

        if (data) {
            console.log(data);

            var alertCaption = data.UpdateCaption;
            $('#flyUpdateSuccessFlag').html(data.UpdateCaption).show();

            alert(alertCaption);

            $("#AdjustAddLoc").slideToggle();
        } else {
            alert("failed to add");
            var a = 'holder';
        }
        $('#waitSpinner').remove();
    });
};

function populateCcLocationsOn3rdTab() {

    // There is re-use involved with additional locations as they can be manipulated on either the 1st or 3rd tab. 
    // Hence, the locationsSpanPrefix may already exist in some scenarios.

    // This variable gets declared elsewhere. Hnce, no 'var' keyword
    cCLocsOn1stTabContainer = $('#collectCcLocations');
    var cClocations = cCLocsOn1stTabContainer.children();
    console.log(cClocations);
    numberOfCcLocationsTab3 = cClocations.filter('span').length;

    var copyOfLocations = cClocations.clone();
    cCLocsOn1stTabContainer.remove();

    cCLocationsList = $('#cCLocationsList');

    cCLocationsList.append('<input id="newOrderRowId" name="newOrderRowId"  type="hidden" value=' + cartStateManager.getOrderRowId() + ' data-val="true" data-val-number="The field newOrderRowId must be a number." data-val-required="The newOrderRowId field is required."/>');
    cCLocationsList.append(copyOfLocations);

    if (numberOfCcLocationsTab3 > 0) {

        cCLocationsList.after($('<button>',
            {
                id: 'applyCcLocationsButton',
                text: 'done adding?',
                'class': 'btn btn-mini btn-primary'
            }));

        $('#applyCcLocationsButton').on('click', applyCcLocations);

        var cCtrashCans = cCLocationsList.find('i');

        $.each(cCtrashCans, function (idx, i) {
            $(i).on('click', deleteCcLocInputTab3);
        });

    }
}

var deleteCcLocInputTab3 = function (event) {

    numberOfCcLocationsTab3--;

    var trashClicked = event.currentTarget.id;
    var idx = trashClicked.substring(0, 1);
    var spanToRemove = "cCLocationSpan"  + idx;

    $('#' + spanToRemove).hide(500, function () {
        $(this).remove();
    });

    $('#' + idx + breakSuffix).hide(500, function () {
        $(this).remove();
    });

};
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
        if (data) {

            var infoLabel = $('#addLocsText');

            var priceLabel = $('#totalAdLocsPrice');

            var addLocPrice = 0;

            if (typeof ADDLOC != "undefined") {
                addLocPrice = ADDLOC.price;
            } else if (typeof webinarAdditionalLocationPrice != "undefined") {
                addLocPrice = webinarAdditionalLocationPrice;
            }

            if (data.Tax > 0) {
                $('#showTax').removeClass("hidden");
            } else {
                $('#showTax').addClass("hidden");
            }
            //am working here
            var alertCaption = data.UpdateSuccessCaption;
            $('#flyUpdateSuccessFlag').html(data.UpdateSuccessCaption).show();
            $('#discountCaption').html(data.DiscountCaption);
            $('#optionLabel').html(data.regTypeShort);

            $('#baseCost').html('$' + data.BasePrice + '');
            $('#totalDiscount').html('<span id="showDiscount">$' + data.Discount + '');
            $('#taxAmt').html(data.Tax + '');
            $('#addLocsText').html('$' + data.OptionsPrice + '');
            if (data.OrderStatusCaption) {
                if (data.OrderStatusCaption.includes("outstanding")) {
                    $('#orderStatusLabel').html(data.OrderStatusCaption);
                    alertCaption += " This previously paid order now has a balance due: $" + data.OutstandingBalance;
                }
            }
            $('#totalPrice').html('<span id="totalPrice">$' + data.Total + '</span>');
            if (data.TotalPaid !== 0) {
                if (data.OutstandingBalance > 0) {
                    $('#showOutstandingBalance').html('<span style=\"color: red;\"  id="outstandingBalance">Due: $' + data.OutstandingBalance + '</span>');

                } else {
                    $("#ShowPayByCCModal").hide();
                    $('#showOutstandingBalance').html('<br><span style=\"color: green;\"  id="outstandingBalance">Due: $(' + data.OutstandingBalance + ')</span>');
                }
            }
            alert(alertCaption);

            $("#AdjustAddLoc").slideToggle();
        } else {
            alert("failed to add");
            var a = 'holder';
        }
        $('#waitSpinner').remove();
    });
};
