// MANAGE namespace
if (MANAGE === null || typeof MANAGE === 'undefined')
    var MANAGE = {}; 


$(function () {
    
    MANAGE.orderIdInput = $('#orderIdInput');
    MANAGE.orderIdInput.focus();
    MANAGE.orderIdList = {};

    $('#getOrderButton').on('click', function () {
        MANAGE.idOrder = MANAGE.orderIdInput.val();

        // loading spinner
        MANAGE.orderIdInput.after('<span id="spinWrapper" class="label label-info"><i id="spinner" class="icon-spinner icon-spin"></i>&nbsp;loading...</span>');
        if ($('#errorDiv').length > 0)
            $('#errorDiv').remove();

        $('#orderRelatedFields').load('/Admin/GetOrderDetails/' + MANAGE.idOrder, function (response, status, xhr) {

            if (status === 'error') {
                $(this).html('<div id="errorDiv" class="text-error">There has been an error at the server, please call 800-831-0678 ext 706 for immediate assistance. <br />' + (xhr.statusText === 'Internal Server Error' ? '' : xhr.statusText) + '</div>');
            } else {
                MANAGE.primeDomVariables();
                MANAGE.wireUpHandlers();

                MANAGE.addLocsUnitPrice = $('#CostPerAdditionalLocation').val();
                MANAGE.addLocsTotalPrice = $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val();

                MANAGE.wireUpTrashIcons();
            }

            // remove loading spinner
            $('#spinWrapper').remove();
        });
    });

    MANAGE.orderIdInput.typeahead({
        source: function (query, process) {
            MANAGE.searchOrder(query, process);
        },

        matcher: function (item) {
            return true;
        },

        highlighter: function (name) {
            return name;
        },

        sorter: function (items) {
            return items;
        },

        updater: function (name) {
            return name;
        }

    });

});


// self-invoking function adds methods to MANAGE namespace
// replace MANAGE with parameter 'ns' as MANAGE is passed in at bottom.
(function (ns) {

    ns.addAdditionalLocation = function(e) {

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

    ns.deleteItem = function(e) {

        e.preventDefault();

        ns.numberOfAdditionalLocations -= 1;

        var trashClicked = event.currentTarget.id;
        var idx = trashClicked.substring(0, 1);
        var spanToRemove = ns.locationsSpanPrefix + idx;

        $('#' + spanToRemove).hide(500, function() {
            $(this).remove();
        });

        $('#' + idx + ns.breakSuffix).hide(500, function() {
            $(this).remove();
        });

        ns.adjustAdditionalLocationsTotal(ns.numberOfAdditionalLocations);
        ns.adjustTotalPrice();
    };

    ns.wireUpTrashIcons = function() {

        var trashCans = ns.wrapperDiv.find('i');

        $.each(trashCans, function(idx, i) {
            $(i).on('click', ns.deleteItem);
        });
    };

    ns.primeDomVariables = function() {

        ns.wrapperDiv = $('#collectAdditionalLocations');
        ns.numberAddLocsLabel = $('#nrAddLocs');
        ns.addAdditionalLocationsButton = $('#addLocationsButton');
        ns.totalOptionsInput = $('DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions');

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

        ns.orderIdHiddenInputInDropdownPartial = $('#regTypeSelectWapper input[type="hidden"]');
        //ns.orderIdHiddenInputInDropdownPartial.removeAttr('name');
        ns.orderIdHiddenInputInDropdownPartial.attr('name', 'DisplayOptionsInDropDownViewModel.OrderRowId');


        ns.gatherPricingData();
        ns.adjustTotalPrice();

        ns.toastLogger = new Common.Logger(); // for toast notifications
        ns.logInvalidOperation = ns.toastLogger.getLogFn('ManageOrderFormSubmit', 'error');
    };

    ns.submitForm = function(e) {
        e.preventDefault();

        var emailInputs = ns.wrapperDiv.find('input[type="email"]');

        var invalidEmailInput = [];

        $.each(emailInputs, function(idx, i) {
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

        // next 4 lines not really required, as those parts of the ViewModel aren't necessary for the POST. But may as well set them, as easy enough to do.
        if (ns.numberOfAdditionalLocations > 0) {
            $('#NumberOfAdditionalLocations').val(ns.numberOfAdditionalLocations);
            $('#DisplayRowPriceViewModel_NumberOfAdditionalLocations').val(ns.numberOfAdditionalLocations);
        }

        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOrderPrice').val($('#TotalOrderPriceText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalDiscount').val($('#TotalDiscountText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_UnitPrice').val($('#UnitPriceText').val());
        $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalCostOfOptions').val($('#TotalCostOfOptionsText').val());


        var form = $('#manageOrderForm');

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: form.attr('action'),
            dataType: constants.JsonDataType,
            data: form.serialize(),
            beforeSend: function() {
                // this is where we append a loading image
                //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
            }
        }).done(function(data, bla, bla) {

        });
    };

    ns.changeRegType = function(e) {

        e.preventDefault();

        var self = this;

        var optionId = $(this).val();

        $.ajax({
            url: "/cart/CheckIfAddLocShouldHide?optionID=" + optionId,
            type: "GET",
            cache: false,
            dataType: constants.JsonDataType,

            beforeSend: function() {
                var valSummary = $('#manageOrderFormValSummary');
                valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

                var errorsList = valSummary.find('ul');
                errorsList.empty();
                errorsList.append('<li style="display:none"></li>');
            }
        }).done(function(data) {
            //console.log('done CheckIfAddLocShouldHide');
            if (data.shouldShow === 'Yes') {
                ns.addAdditionalLocationsButton.removeAttr('disabled');
                //ns.selectedAdditionalLocationsPrice = ns.changeRegType.find(":selected").data('price');
            } else if (data.shouldShow === 'No') {
                //console.log('hide  CheckIfAddLocShouldHide');
                $('#collectAdditionalLocations').empty().html('<span id="naText" class="text text-info">Not applicable for this RegType</span>');
                ns.numberOfAdditionalLocations = 0;
                ns.numberAddLocsLabel.text(0);
                ns.addAdditionalLocationsButton.attr('disabled', 'disabled');
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('manageOrderFormValSummary', data);
            }
        }).fail(function(data) {
            $('#orderRelatedFields').html('<div class="text-error">There has been a transport-level error, please call 800-831-0678 ext 706 for immediate assistance.</div>');
        });
    };

    ns.wireUpHandlers = function() {

        $('#addLocationsButton').on('click', ns.addAdditionalLocation);
        $('#editOrderSubmitButton').on('click', ns.submitForm);
        $('#applyDiscountButton').on('click', ns.hookUpApplyDiscountLogic);
        $('#RegType').on('change', ns.changeRegType);

    };

    ns.adjustAdditionalLocationsTotal = function(number) {

        ns.numberAddLocsLabel.text(ns.numberOfAdditionalLocations);

        var newPrice = number * parseFloat(ns.addLocsUnitPrice);

        ns.additionalLocationsTotal.val(newPrice);
        ns.allAddLocsPrice = parseInt(ns.additionalLocationsTotal.val());
    };

    ns.adjustTotalPrice = function() {

        var newPrice = ns.basePrice + (ns.allAddLocsPrice || 0) - (ns.totalDiscount || 0);

        ns.totalPriceInput.val(newPrice);
    };

    ns.searchOrder = _.debounce(function(query, process) {

        var searchTerm = $.trim(ns.orderIdInput.val());

        if (searchTerm === '')
            return;

        if (searchTerm.indexOf('@') > 0) {
            // in here if searching for an email
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByEmailTypeahead',
                dataType: constants.JsonDataType,
                data: { email: searchTerm },
                beforeSend: function() {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function(data) {
                ns.orderIdList = data.orderIds;
                process(ns.orderIdList);
            });

        } else if (_.isFinite(searchTerm)) {
            // in here if searching on an order number

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByTypeahead',
                dataType: constants.JsonDataType,
                data: { id: searchTerm },
                beforeSend: function() {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function(data) {
                ns.orderIdList = data.orderIds;
                process(ns.orderIdList);
            });
        } else {
            // if not a number and not an email address, search is by lastname
            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/GetOrdersByLastName',
                dataType: constants.JsonDataType,
                data: { lastName: searchTerm },
                beforeSend: function() {
                    ns.orderIdList = null; // dereference whatever is currently in 'ns.orderIdList'. 
                }
            }).done(function(data) {
                ns.orderIdList = data.orderIds;
                process(ns.orderIdList);
            });
        }

    }, 200);

    ns.hookUpApplyDiscountLogic = function(e) {

        e.preventDefault();

        ns.gatherPricingData();

        if (ns.totalPrice < 1) {
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
            beforeSend: function() {
                $(self).prepend('<i id="discountSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                $(self).attr('disabled', 'disabled');
            }
        }).done(function(data) {

            if (data.Result.indexOf('%') !== -1) {
                var amount2Discount = data.Result.replace(".00%", "") / 100;
                ns.totalDiscount = ns.totalPrice * amount2Discount;
            } else {
                ns.totalDiscount = data.Result;
            }

            ns.adjustTotalPrice();

            //if (newTotalPrice < 0)
            //    newTotalPrice = 0;

            //$('#addlocSpiel').text('To add additional locations for this order, please call 800-831-0678 ext 3.').addClass('text-info');

            //$('#discountedText').html('Discounted: <span id="totalDiscount">$' + registerDuringCheckout.totalDiscount + '</span>').removeClass('muted');
            //$('#totalPriceText').html('Total Cost: <span id="totalPrice">$' + newTotalPrice.toString() + '.00</span>');

            $('#discountSpinner').remove();

        }).always(function(e) {
            $('#discountSpinner').remove();
            $(self).removeAttr('disabled');
        });
    };

    ns.gatherPricingData = function() {
        ns.allAddLocsPrice = parseInt(ns.additionalLocationsTotal.val());
        ns.totalDiscount = parseInt(ns.totalDiscountInput.val());
        ns.totalPrice = parseInt(ns.totalPriceInput.val());
    };
})(MANAGE);