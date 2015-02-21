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

MANAGE.addAdditionalLocation = function (e) {

    e.preventDefault();

    var newId;

    if (MANAGE.numberOfAdditionalLocations === 0) {
        newId = 0;
        var naSpan =$('#naText');
        if (naSpan.length > 0)
            naSpan.remove();
    } else {
        // first get the last previous email input
        var lastInput = MANAGE.wrapperDiv.find('input[type="email"]:last');
        // get its id
        var lastInputId = lastInput.attr('id');
        var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
        newId = id + 1;
    }

    var trashIconId = newId + '-AdditionLocationEmail-delete';
    var additionalLocationEmailId = 'AdditionalLocationEmail-' + newId;

    MANAGE.wrapperDiv.append('<span id="' + MANAGE.locationsSpanPrefix + newId + '"><input id="' + additionalLocationEmailId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" aria-describedby="AdditionalLocationEmail_'+ newId +'-error" aria-invalid="false"></input>&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + trashIconId + '"></i></span> <br id="' + newId + MANAGE.breakSuffix + '">');

    $('#' + trashIconId).on('click', MANAGE.deleteItem);
    $('#' + additionalLocationEmailId).focus();

    MANAGE.numberOfAdditionalLocations += 1;
    MANAGE.adjustAdditionalLocationsTotal(MANAGE.numberOfAdditionalLocations);
};

MANAGE.deleteItem = function (e) {

    e.preventDefault();

    MANAGE.numberOfAdditionalLocations -= 1;
    
    var trashClicked = event.currentTarget.id;
    var idx = trashClicked.substring(0, 1);
    var spanToRemove = MANAGE.locationsSpanPrefix + idx;

    $('#' + spanToRemove).hide(500, function () {
        $(this).remove();
    });

    $('#' + idx + MANAGE.breakSuffix).hide(500, function () {
        $(this).remove();
    });

    MANAGE.adjustAdditionalLocationsTotal(MANAGE.numberOfAdditionalLocations);
};

MANAGE.wireUpTrashIcons = function () {
    
    var trashCans = MANAGE.wrapperDiv.find('i');

    $.each(trashCans, function (idx, i) {
        $(i).on('click', MANAGE.deleteItem);
    });
};

MANAGE.primeDomVariables = function() {

    MANAGE.wrapperDiv = $('#collectAdditionalLocations');
    MANAGE.numberAddLocsLabel = $('#nrAddLocs');
    MANAGE.addAdditionalLocationsButton = $('#addLocationsButton');
    MANAGE.totalOptionsInput = $('DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions');

    MANAGE.locationsSpanPrefix = 'LocationSpan-';
    MANAGE.breakSuffix = '-break';
    MANAGE.numberOfAdditionalLocations = parseInt(MANAGE.numberAddLocsLabel.text());
};

MANAGE.submitForm = function(e) {
    e.preventDefault();

    var emailInputs = MANAGE.wrapperDiv.find('input[type="email"]');

    $.each(emailInputs, function (idx, i) {
        $(i).attr('name', 'AdditionalLocations[' + idx + '].Email');
    });

    var form = $('#manageOrderForm');

    $.ajax({
        type: 'POST',
        contentType: constants.FormPostContentType,
        cache: false,
        url: form.attr('action'),
        dataType: constants.JsonDataType,
        data: form.serialize(),
        beforeSend: function () {
            // this is where we append a loading image
            //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
        }
    }).done(function (data, bla, bla) {

    });
};

MANAGE.changeRegType = function(e) {

    e.preventDefault();

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
        }
    }).done(function (data) {
        //console.log('done CheckIfAddLocShouldHide');
        if (data.shouldShow === 'Yes') {
            MANAGE.addAdditionalLocationsButton.removeAttr('disabled');
        } else if (data.shouldShow === 'No') {
            //console.log('hide  CheckIfAddLocShouldHide');
            $('#collectAdditionalLocations').empty().html('<span id="naText" class="text text-info">Not applicable for this RegType</span>');
            MANAGE.numberOfAdditionalLocations = 0;
            MANAGE.numberAddLocsLabel.text(0);
            MANAGE.addAdditionalLocationsButton.attr('disabled', 'disabled');
        } else if (!data.isSuccessful) {
            formProcessor.lightUpValidationSummary('manageOrderFormValSummary', data);
        }
    }).fail(function (data) {
        $('#orderRelatedFields').html('<div class="text-error">There has been a transport-level error, please call 800-831-0678 ext 706 for immediate assistance.</div>');
    });
};

MANAGE.wireUpHandlers = function () {

    $('#addLocationsButton').on('click', MANAGE.addAdditionalLocation);
    $('#editOrderSubmitButton').on('click', MANAGE.submitForm);
    $('#RegType').on('change', MANAGE.changeRegType);
};


MANAGE.adjustAdditionalLocationsTotal = function(number) {

    MANAGE.numberAddLocsLabel.text(MANAGE.numberOfAdditionalLocations);

    var newPrice = number * parseFloat(MANAGE.addLocsUnitPrice);

    MANAGE.totalOptionsInput.val(newPrice);
};

MANAGE.searchOrder = _.debounce(function(query, process) {
    
    var searchTerm = MANAGE.orderIdInput.val();

    $.ajax({
        type: 'GET',
        contentType: constants.FormPostContentType,
        cache: false,
        url: '/Admin/GetOrdersByTypeahead',
        dataType: constants.JsonDataType,
        data: { id: searchTerm },
        beforeSend: function () {
            MANAGE.orderIdList = null; // dereference whatever is currently in 'MANAGE.orderIdList'. 
        }
    }).done(function (data) {
        MANAGE.orderIdList = data.orderIds;
        process(MANAGE.orderIdList);
    });

}, 200);