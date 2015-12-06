// MANAGE namespace
if (MANAGE === null || typeof MANAGE === 'undefined')
    var MANAGE = {};

var M = MANAGE; // alias for code brevity

// jQuery doc.ready function
$(function () {

    M.orderIdInput = $('#orderIdInput');
    M.orderIdInput.focus();
    M.idOrder = M.orderIdInput.val();

    M.orderIdList = {}; // javascript object to be used in the Bootstrap typahead as in-memory list

    M.orderIdInput.typeahead({
        source: function (query, process) {
            M.searchOrder(query, process);
        },

        matcher: function (item) {
            return true;
        },

        highlighter: function (listedUser) {

            // if integer, return it as we are dealing with an orderID
            if (_.isFinite(listedUser)) {
                return listedUser;
            }

            var item = JSON.parse(listedUser);
            var user = _.find(M.orderIdList, function (webUser) {
                return JSON.parse(webUser)['id'] === item.id;
            });
            if (user !== null && typeof user !== 'undefined') {
                var userParsed = JSON.parse(user);
                if (M.searchBy == "email") {
                    return userParsed.email + ', ' + userParsed.lastName;
                }
                if (M.searchBy == "orderID") {
                    return userParsed.email + ', ' + userParsed.lastName;
                }
                if (M.searchBy == "lastName") {
                    return userParsed.lastName + ', ' + userParsed.firstName;
                }

            }
        },

        sorter: function (items) {
            return items;
        },

        updater: function (selection) {

            if (_.isFinite(selection)) {
                return selection;
            }

            var userParsed = JSON.parse(selection);
            var user = _.find(M.orderIdList, function (p) {
                return JSON.parse(p)['id'] === userParsed['id'];
            });

            if (typeof user !== 'undefined') {
                var parsedUser = JSON.parse(user);
                //OCA.setSelectedProduct(parsedUser);
                return parsedUser['lastName'] + ', ' + parsedUser['firstName'];
            }
            return '';
        }
    });


    M.primeDomVariables();
    M.wireUpHandlers();

    M.addLocsUnitPrice = $('#CostPerAdditionalLocation').val();
    M.addLocsTotalPrice = $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val();

    M.wireUpTrashIcons();

    M.getOrderButton.on('click', M.getOrder);
});


// self-invoking function adds methods to MANAGE namespace
// replace MANAGE with parameter 'ns' as MANAGE is passed in at bottom in the self-invoking parentheses.
(function (ns) {

    ns.addAdditionalLocation = function (e) {

        e.preventDefault();

        var noLocationsSpane = $('#noLocationsText');
        if (noLocationsSpane.length > 0)
            noLocationsSpane.remove();

        var newId;

        if (ns.numberOfAdditionalLocations === 0) {
            newId = 0;
            var naSpan = $('#naText');
            if (naSpan.length > 0)
                naSpan.remove();
        } else {
            // first get the last previous email input
            var lastInput = ns.wrapperDiv.find('input[type="email"]:last');
            // get its id
            var lastInputId = lastInput.attr('id');
            var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
            newId = id + 1;
        }

        var trashIconId = newId + '-AdditionLocationEmail-delete';
        var additionalLocationEmailId = 'AdditionalLocationEmail-' + newId;

        ns.wrapperDiv.append('<span id="' + ns.locationsSpanPrefix + newId + '"><input id="' + additionalLocationEmailId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" aria-describedby="AdditionalLocationEmail_' + newId + '-error" aria-invalid="false"></input>&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + trashIconId + '"></i></span> <br id="' + newId + ns.breakSuffix + '">');

        $('#' + trashIconId).on('click', ns.deleteItem);
        $('#' + additionalLocationEmailId).focus();

        ns.numberOfAdditionalLocations += 1;
        ns.adjustAdditionalLocationsTotal(ns.numberOfAdditionalLocations);
        ns.adjustTotalPrice();
    };

    ns.deleteItem = function (e) {

        e.preventDefault();

        ns.numberOfAdditionalLocations -= 1;

        var trashClicked = e.currentTarget.id;
        var idx = trashClicked.substring(0, 1);
        var spanToRemove = ns.locationsSpanPrefix + idx;

        $('#' + spanToRemove).hide(500, function () {
            $(this).remove();
        });

        $('#' + idx + ns.breakSuffix).hide(500, function () {
            $(this).remove();
        });

        ns.adjustAdditionalLocationsTotal(ns.numberOfAdditionalLocations);
        ns.adjustTotalPrice();
    };

    ns.wireUpTrashIcons = function () {

        var trashCans = ns.wrapperDiv.find('i');

        $.each(trashCans, function (idx, i) {
            $(i).on('click', ns.deleteItem);
        });
    };

    ns.primeDomVariables = function () {

        ns.getOrderButton = $('#getOrderButton');
        ns.wrapperDiv = $('#collectAdditionalLocations');
        ns.numberAddLocsLabel = $('#nrAddLocs');
        ns.addAdditionalLocationsButton = $('#addLocationsButton');
        ns.updateAddLocsButton = $('#UpdateAddLocsButton');
        ns.totalOptionsInput = $('DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions');
        ns.updateAdditionalLocationsForm = $('#updateAdditionalLocationsForm');
        ns.showChangeAssignedAffiliate = $('#showChangeAssignedAffiliate');
        ns.showChangeUser = $('#showChangeUser');
        ns.changeAssignedUser = $('#changeAssignedUser');
        ns.changeUserOrderButton = $('#changeUserOrderButton');
        ns.changeUserOrdersButton = $('#changeUserOrdersButton');
        ns.extendPostEventAccessModal = $('#ExtendPostEventAccessModal');
        ns.extendEventAccessButton = $('#extendEventAccessButton');
        ns.fireResendConfirmationButton = $('#fireResendConfirmationButton');
        ns.add6MonthsButton = $('#Add6MonthsButton');
        ns.add5DaysButton = $('#Add5DaysButton');
        ns.adInfinitumButton = $('#AdInfinitumButton');

        ns.regTypesList = $('#RegType');
        ns.orderRowId = $('#manageOrderForm input[name="ID"]').val();

        ns.locationsSpanPrefix = 'LocationSpan-';
        ns.breakSuffix = '-break';
        ns.numberOfAdditionalLocations = parseInt(ns.numberAddLocsLabel.text());
        //ns.selectedAdditionalLocationsPrice = ns.regTypesList.find(":selected").data('price');

        ns.additionalLocationsTotal = $('#TotalCostOfOptionsText');
        ns.totalDiscountInput = $('#TotalDiscountText');
        ns.totalPriceInput = $('#TotalOrderPriceText');
        ns.basePrice = parseFloat($('#UnitPriceText').val());

        ns.orderIdHiddenInputInDropdownPartial = $('#regTypeSelectWrapper input[type="hidden"]');
        ns.orderIdHiddenInputInDropdownPartial.attr('name', 'DisplayOptionsInDropDownViewModel.OrderRowId');
        ns.orderRowIdHidden = $('input[name="DisplayOptionsInDropDownViewModel.OrderRowId"]');

        ns.titleHeading = null;
        ns.titleHeading = $('#webinarHeading');
        ns.titleInput = $('#titleInput');

        ns.gatherPricingData();

        ns.toastLogger = new Common.Logger(); // for toast notifications
        ns.logInvalidOperation = ns.toastLogger.getLogFn('ManageOrderFormSubmit', 'error');
    };

    ns.submitForm = function (e) {

        e.preventDefault();

        var emailInputs = ns.wrapperDiv.find('input[type="email"]');

        var invalidEmailInput = [];

        $.each(emailInputs, function (idx, i) {
            if ($(i).val().indexOf('@') < 0) {
                invalidEmailInput.push($(i).attr('id'));
                $(i).css('border-color', '#b94a48').css('background-color', '#ec8d8d');
            }
            $(i).attr('name', 'AdditionalLocations[' + idx + '].Email');
        });

        if (invalidEmailInput.length > 0) {
            ns.logInvalidOperation("At least 1 of the email address textboxes is empty or has an invalid address. Please add a valid address or delete the textbox by clicking the adjacent trashcan.", null, true);
            return; // if even 1 email input has no email address, stop processing. Remove it or enter an email address.
        }

        // next 4 lines not really required, as those parts of the ViewModel aren't necessary for the POST. 
        //But may as well set them, as easy enough to do.
        if (ns.numberOfAdditionalLocations > -1) {
            $('#NumberOfAdditionalLocations').val(ns.numberOfAdditionalLocations);
            $('#DisplayRowPriceViewModel_NumberOfAdditionalLocations').val(ns.numberOfAdditionalLocations);
        }

        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOrderPrice').val($('#TotalOrderPriceText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalDiscount').val($('#TotalDiscountText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_UnitPrice').val($('#UnitPriceText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalCostOfOptions').val($('#TotalCostOfOptionsText').val());

        var self = $(this);

        var form = $('#manageOrderForm');

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: form.attr('action'),
            dataType: constants.JsonDataType,
            data: form.serialize(),
            beforeSend: function () {
                $('#postFeedbackLabel').remove();
                self.append('<span id="submitSpinWrapper">&nbsp;<span class="label label-info"><i id="spinner" class="icon-spinner icon-spin"></i>&nbsp;loading...</span></span>');
            }
        }).done(function (data, bla, bla) {
            $('#submitSpinWrapper').remove();
            self.after('<span id="postFeedbackLabel">&nbsp;<span class="label label-success">&nbsp;Operation succeeded</span></span>');
        });
    };

    ns.changeRegType = function (e) {

        e.preventDefault();

        var self = this;

        var optionId = $(this).val();

        $.ajax({
            url: "/cart/CheckIfAddLocShouldHide?optionID=" + optionId,
            type: "GET",
            cache: false,
            dataType: constants.JsonDataType,

            beforeSend: function () {
                var valSummary = $('#manageOrderFormValSummary');
                valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                var errorsList = valSummary.find('ul');
                errorsList.empty();
                errorsList.append('<li style="display:none"></li>');

                ns.regTypesList.after('<span id="regTypeSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (data) {
            //console.log('done CheckIfAddLocShouldHide');
            if (data.shouldShow === 'Yes') {
                ns.addAdditionalLocationsButton.removeAttr('disabled');
                $('#collectAdditionalLocations').empty().html('<span id="naText" class="text text-info">No additionalLocations yet</span>');
                //var typeChosenCurrent = $.trim($('#RegType option:selected').text());
                ns.updatePriceOnNewSelection(ns.regTypesList.val(), ns.totalPrice, ns.regTypesList);

            } else if (data.shouldShow === 'No') {
                //console.log('hide  CheckIfAddLocShouldHide');
                $('#collectAdditionalLocations').empty().html('<span id="naText" class="text text-info">Not applicable for this RegType</span>');
                ns.numberOfAdditionalLocations = 0;
                ns.numberAddLocsLabel.text(0);
                ns.addAdditionalLocationsButton.attr('disabled', 'disabled');

                var url = '/Cart/RemoveAdditionalLocationsFromOrder';
                var payLoad = {
                    idOrderRow: ns.orderRowIdHidden.val()
                };

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payLoad)
                }).done(function (data) {

                    if (data.Result === 'Success') {

                        var totalPrice = ns.totalPrice - ns.allAddLocsPrice;

                        ns.updatePriceOnNewSelection(ns.regTypesList.val(), totalPrice, ns.regTypesList);
                    }
                });

            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('manageOrderFormValSummary', data);
            }
        }).fail(function (data) {
            $('#orderRelatedFields').html('<div class="text-error">There has been a transport-level error, please call 800-831-0678 ext 706 for immediate assistance.</div>');
        });
    };

    ns.updatePriceOnNewSelection = function (registrationTypeId, totalPrice, dropDown) {

        ns.gatherPricingData();

        var url = '/Cart/UpdateOrderDetails';

        var payLoad = {
            idOrderRow: ns.orderRowIdHidden.val(),
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

                $('#UnitPriceText').val(data.BasePrice);
                $('#DisplayRowPriceViewModel_PricesAndDiscounts_UnitPriceUnitPriceText').val(data.BasePrice);
                $('#TotalDiscountText').val(data.Discount);
                $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalDiscount').val(data.Discount);
                $('#TotalCostOfOptionsText').val(data.OptionsPrice);
                $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalCostOfOptions').val(data.OptionsPrice);
                $('#TotalOrderPriceText').val(data.Total);
                $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOrderPrice').val(data.Total);

                $('#regTypeSpinner').remove();
            }

            dropDown.removeAttr('disabled');
            //$('#discountSpinner').remove();
        });
    };

    ns.wireUpHandlers = function () {

        ns.addAdditionalLocationsButton.on('click', ns.addAdditionalLocation);
        $('#editOrderSubmitButton').on('click', ns.submitForm);
        $('#applyDiscountButton').on('click', ns.hookUpApplyDiscountLogic);
        ns.regTypesList.on('change', ns.changeRegType);
        ns.updateAddLocsButton.on('click', ns.updateAdditionalLocations);
        ns.updateAdditionalLocationsForm.on('submit', ns.submitUpdateAddLocsForm);

        ns.extendEventAccessButton.on('click', ns.extendEventAccess);
        ns.add6MonthsButton.on('click', ns.add6MonthsButtonClick);
        ns.add5DaysButton.on('click', ns.add5DaysButtonClick);
        ns.adInfinitumButton.on('click', ns.adInfinitumButtonClick);

        ns.extendPostEventAccessModal.on('shown', function () {

            $('#newExpiryDate').focus();

            $('#resultLabel').remove();

            var valSummary = $('#extendPostEventAccessValSummary');
            valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

            var errorsList = valSummary.find('ul');
            errorsList.empty();
            errorsList.append('<li style="display:none"></li>');
        });

        ns.extendPostEventAccessModal.on('hidden', function () {
            $('#resultLabel').remove();

            var valSummary = $('#extendPostEventAccessValSummary');
            valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

            var errorsList = valSummary.find('ul');
            errorsList.empty();
            errorsList.append('<li style="display:none"></li>');
        });

        ns.showChangeUser.on('click', function (e) {

            e.preventDefault();

            var modalFormOptions = {
                keyboard: true,
                backdrop: 'static',
                show: true
            };

            ns.changeAssignedUser.modal(modalFormOptions);

            ns.changeAssignedUser.on('hidden', function () {
                modalFormOptions = null;
            });
        });

        ns.changeUserOrderButton.on('click', ns.changeUserOrder);

        ns.changeUserOrdersButton.on('click', ns.changeUserOrders);

        ns.showChangeAssignedAffiliate.on('click', ns.displayChangeAffiliateModal);
    };

    ns.adjustAdditionalLocationsTotal = function (number) {

        ns.numberAddLocsLabel.text(ns.numberOfAdditionalLocations);

        var newAddLocsPrice = number * parseFloat(ns.addLocsUnitPrice);

        ns.additionalLocationsTotal.val(newAddLocsPrice);
        ns.allAddLocsPrice = parseInt(ns.additionalLocationsTotal.val());
        ns.totalPriceSansDiscount = ns.allAddLocsPrice + ns.basePrice;
    };

    ns.adjustTotalPrice = function () {
        var newTotalPrice = ns.totalPriceSansDiscount - (ns.totalDiscount || 0);

        ns.totalPrice = newTotalPrice;
        ns.totalPriceInput.val(newTotalPrice);
    };

    ns.searchOrder = _.debounce(function (query, process) {

        var searchTerm = $.trim(ns.orderIdInput.val());

        if (searchTerm === '')
            return;

        if (searchTerm.indexOf('@') > 0) {
            // in here if searching for an email

            M.searchBy = 'email';

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByEmailTypeahead',
                dataType: constants.JsonDataType,
                data: { email: searchTerm },
                beforeSend: function () {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function (data) {
                M.orderIdList = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(M.orderIdList);
            });

        } else if (_.isFinite(searchTerm)) {
            // in here if searching on an order number

            M.searchBy = 'orderID';

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByTypeahead',
                dataType: constants.JsonDataType,
                data: { id: searchTerm },
                beforeSend: function () {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function (data) {
                ns.orderIdList = data.results;
                process(ns.orderIdList);
            });
        } else {
            // if not a number and not an email address, search is by lastname

            M.searchBy = 'lastName';

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByLastName',
                dataType: constants.JsonDataType,
                data: { lastName: searchTerm },
                beforeSend: function () {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function (data) {

                M.orderIdList = _.map(data.results, function (item) {
                    var aItem = { id: item.id, firstName: item.firstName, lastName: item.lastName, email: item.billingEmail, institution: item.institution };
                    return JSON.stringify(aItem);
                });

                process(M.orderIdList);
            });
        }

    }, 200);

    ns.hookUpApplyDiscountLogic = function (e) {

        e.preventDefault();

        ns.gatherPricingData();

        if (ns.totalPriceSansDiscount < 1) {
            return;
        }

        var url = '/cart/ApplyDiscountCode';
        var payload = { code: $('#DisplayRowPriceViewModel_Discount_DiscountCode').val(), orderRowId: ns.orderRowId };
        var self = this;

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).prepend('<span id="discountSpinner"><i class="icon-spinner icon-spin"></i>&nbsp;</span>');
                $(self).attr('disabled', 'disabled');
            }
        }).done(function (data) {
            if (data.Result == -1) {
                ns.totalDiscount = 'no credits';
            } else {
                if (data.Result.indexOf('%') !== -1) {
                    var amount2Discount = data.Result.replace('.00%', '') / 100;
                    ns.totalDiscount = ns.totalPriceSansDiscount * amount2Discount;
                } else {
                    ns.totalDiscount = data.Result;
                }
                ns.adjustTotalPrice();
            }
            ns.totalDiscountInput.val(ns.totalDiscount);

            $('#discountSpinner').remove();

        }).always(function (e) {
            $('#discountSpinner').remove();
            $(self).removeAttr('disabled');
        });
    };

    ns.updateAdditionalLocations = function (e) {

        e.preventDefault();

        ns.updateAdditionalLocationsForm.submit();
    };

    ns.submitUpdateAddLocsForm = function (e) {

        e.preventDefault();

        var emailInputs = ns.wrapperDiv.find('input[type="email"]');
        var addLocs = [];

        _.each(emailInputs, function (element, index) {
            addLocs.push({
                Email: $(element).val()
            });
        });

        var payload = {
            orderRowId: ns.orderRowIdHidden.val(),
            additionalLocations: addLocs
        };

        var form = $(this);

        var url = form.attr('action');
        var token = form.find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;


        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers,
            beforeSend: function () {
                ns.updateAddLocsButton.append('<span id="addLocUpSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (data) {
            if (data.Result === 'Success') {
                $('#addLocUpSpinner').remove();
            }
        });
    };


    ns.fireResendConfirmation = function (e) {

        e.preventDefault();

        var fireResendConfirmationDiv = $('#fireResendConfirmation');

        $('#fireResendConfirmation').on('click', function () {
            labelCheckRemove();

            var orderId = fireResendConfirmationDiv.data("orderid");

            var self = this;
            //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            //var logStartOperation = toastLogger.getLogFn('ResendOrderConfirmation');
            //logStartOperation("Re-sending Order Confirmation", null, true);

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: resendOrderConfirmationUrl,
                dataType: constants.JsonDataType,
                data: JSON.stringify({ orderId: orderId }),
                beforeSend: function () {
                    $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                }
            }).done(function (result) {

                if (result.Result === 'Success') {
                    $('#fireResendConfirmationDiv').append(successScreenMessage);
                } else if (result.Result === 'Fail') {
                    $('#fireResendConfirmationDiv').append("fail");
                }
                $('#spinnerLabel').remove();
            }).fail(function () {
                $('#spinnerLabel').remove();
            }).always(function () {
                //$('#loadingSpinner').remove();
            });
        });

        $('#getHtmlSpinner').remove();

    };

    ns.getOrder = function (e) {

        e.preventDefault();
        //TODO: update controller action to permit email submission
        ns.idOrder = ns.orderIdInput.val();

        // loading spinner
        ns.orderIdInput.after('<span id="spinWrapper">&nbsp;<span class="label label-info"><i id="spinner" class="icon-spinner icon-spin"></i>&nbsp;loading...</span></span>');
        if ($('#errorDiv').length > 0)
            $('#errorDiv').remove();

        $('#orderRelatedFields').load('/Admin/GetOrderDetails/' + ns.idOrder, function (response, status, xhr) {

            if (status === 'error') {
                $(this).html('<div id="errorDiv" class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance. <br />' + (xhr.statusText === 'Internal Server Error' ? '' : xhr.statusText) + '</div>');
            } else {
                ns.primeDomVariables();
                ns.wireUpHandlers();
                ns.titleHeading.text(ns.titleInput.val());

                ns.addLocsUnitPrice = $('#CostPerAdditionalLocation').val();
                ns.addLocsTotalPrice = $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val();

                ns.wireUpTrashIcons();
            }

            // remove loading spinner
            $('#spinWrapper').remove();
        });
    };

    ns.displayChangeAffiliateModal = function (e) {

        e.preventDefault();

        var modalFormOptions = {
            keyboard: true,
            backdrop: 'static',
            show: true
        };

        $(this).after('<span id="loadAffModal">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        $('#frmChangeAffiliate > div.modal-body').load('/Admin/GetAffiliates', function () {

            $('#SelectedAffiliate').val($('#idAffiliate').text());

            $('#changeAssignedAffiliate').modal(modalFormOptions);

            $('#submitChangeAffilate').on('click', function (e) {

                e.preventDefault();

                var self = this;


                var payload = {
                    idAffiliate: $('#SelectedAffiliate').val(),
                    idOrder: $('#Id').val()
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
                    }
                }).done(function (data) {
                    if (data.Result === "Success") {

                        $('#cancelChangeAffiliate').addClass("btn-success").text("Update Successful");
                        $('#submitChangeAffilate').hide();

                        var oi = data.Result;
                    }

                    $(self).removeAttr('disabled');
                    $('#changeAffSpinner').remove();
                    $('#idAffiliate').text(payload['idAffiliate']);
                    $('#affiliateNameText').text($('#SelectedAffiliate :selected').text());
                });
            });
        });

        $('#changeAssignedAffiliate').on('hidden', function (e) {
            $('#submitChangeAffilate').off('click');
            modalFormOptions = null;
        });

        $('#changeAssignedAffiliate').on('shown', function (e) {
            $('#loadAffModal').remove();
        });

    };

    ns.changeUserOrder = function (e) {
        e.preventDefault();

        var self = this;

        var url = $('#frmSetUserAssignedToOrder').attr('action');

        var tabInputs = formProcessor.getApplicableInputs('frmSetUserAssignedToOrder');
        var payload = formProcessor.processInputs(tabInputs);

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).append('<span id="changeUserSpinner"><i class="icon-spinner icon-spin"></i>&nbsp;</span>');
                $(self).attr('disabled', 'disabled');
            }
        }).done(function (data) {

            if (data.Result === 'Success') {
                $('#editUserLink').attr('href', data.NewHref);
                $('#UserEmail').val(data.Email);
                $('#FullName').val(data.FullName);
                $('#Phone ').val(data.Phone);
            }
            $('#changeUserSpinner').remove();
            $(self).removeAttr('disabled');

        });
    };

    ns.changeUserOrders = function (e) {
        e.preventDefault();

        var self = this;

        var url = $('#frmSetUserAssignedToOrder').attr('action');

        var tabInputs = formProcessor.getApplicableInputs('frmSetUserAssignedToOrder');
        var payload = formProcessor.processInputs(tabInputs);
        payload['migrateOrder'] = 'moveAll';

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).append('<span id="changeUserSpinner"><i class="icon-spinner icon-spin"></i>&nbsp;</span>');
                $(self).attr('disabled', 'disabled');
            }
        }).done(function (data) {

            if (data.Result === 'Success') {
                $('#editUserLink').attr('href', data.NewHref);
                $('#UserEmail').val(data.Email);
                $('#FullName').val(data.FullName);
                $('#Phone ').val(data.Phone);
            }
            $('#changeUserSpinner').remove();
            $(self).removeAttr('disabled');

        });
    };

    ns.extendEventAccess = function (e) {

        e.preventDefault();

        var self = this;

        var form = $('#frmExtendPostEventAccess');

        var url = form.attr('action');

        var token = form.find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var tabInputs = formProcessor.getApplicableInputs('frmExtendPostEventAccess');
        var payload = formProcessor.processInputs(tabInputs);
        payload['email'] = $('#UserEmail').val();

        // delete the following properties from the payload which are from inputs that are not
        // part of the form. Not sure why they are being picked up by the formProcessor.
        delete (payload['DisplayRowPriceViewModel.OrderStatus']);
        delete (payload['RegType']);

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            headers: headers,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).append('<span id="extendTimeSpinner"><i class="icon-spinner icon-spin"></i>&nbsp;</span>');
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                $(self).after('<span id="resultLabel">&nbsp;<span class="label label-success">Access Extended!</span></span>');
                $('#postEventAccessExpirey').text($('#newExpiryDate').val());
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('extendPostEventAccessValSummary', data);
            }

            $('#extendTimeSpinner').remove();
        });

    };

    ns.editDiscount = function (e) {

        e.preventDefault();

        var self = this;

        var form = $('#frmEditDiscount');

        var url = form.attr('action');

        var token = form.find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var tabInputs = formProcessor.getApplicableInputs('frmEditDiscount');
        var payload = formProcessor.processInputs(tabInputs);
        payload['email'] = $('#UserEmail').val();

        // delete the following properties from the payload which are from inputs that are not
        // part of the form. Not sure why they are being picked up by the formProcessor.
        delete (payload['DisplayRowPriceViewModel.OrderStatus']);
        delete (payload['RegType']);

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            headers: headers,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).append('<span id="EditDiscountSpinner"><i class="icon-spinner icon-spin"></i>&nbsp;</span>');
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                $(self).after('<span id="resultLabel">&nbsp;<span class="label label-success">Edit is commited</span></span>');
                $('#postEventAccessExpirey').text($('#newExpiryDate').val());
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('editDiscountSummary', data);
            }

            $('#EditDiscountSpinner').remove();
        });

    };

    ns.add6MonthsButtonClick = function (e) {

        e.preventDefault();

        var dateVal = new Date();
        dateVal.setDate(dateVal.getDate() + 183);

        $('#frmExtendPostEventAccess').find('input[name="newExpiryDate"]').val(ns.convertDateToStringUSFormat(dateVal));
    };

    ns.add5DaysButtonClick = function (e) {

        e.preventDefault();

        var dateVal = new Date();
        dateVal.setDate(dateVal.getDate() + 5);

        $('#frmExtendPostEventAccess').find('input[name="newExpiryDate"]').val(ns.convertDateToStringUSFormat(dateVal));
    };

    ns.adInfinitumButtonClick = function (e) {

        e.preventDefault();

        var dateVal = new Date();
        dateVal.setDate(dateVal.getDate() + 10000);

        $('#frmExtendPostEventAccess').find('input[name="newExpiryDate"]').val(ns.convertDateToStringUSFormat(dateVal));
    };

    ns.convertDateToStringUSFormat = function (dateVal) {
        var yyyy = dateVal.getFullYear().toString();
        var mm = (dateVal.getMonth() + 1).toString();
        var dd = dateVal.getDate().toString();

        return (mm[1] ? mm : "0" + mm[0]) + '/' + (dd[1] ? dd : "0" + dd[0]) + '/' + yyyy;
    };

    ns.gatherPricingData = function () {
        ns.allAddLocsPrice = parseInt(ns.additionalLocationsTotal.val());
        ns.totalDiscount = parseInt(ns.totalDiscountInput.val());
        ns.totalPrice = parseInt(ns.totalPriceInput.val());
        ns.totalPriceSansDiscount = (ns.allAddLocsPrice || 0) + ns.basePrice;
    };

})(MANAGE);