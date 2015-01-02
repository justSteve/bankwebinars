var MANAGE = {};


$(function () {

    var orderIdInput = $('#orderIdInput');
    orderIdInput.focus();

    $('#getOrderButton').on('click', function () {
        MANAGE.idOrder = orderIdInput.val();

        $('#orderRelatedFields').load('/Admin/GetOrderDetails/' + MANAGE.idOrder, function () {
            MANAGE.primeDomVariables();
            MANAGE.wireUpHandlers();
            MANAGE.numberAddLocsLabel = $('#nrAddLocs');
            MANAGE.addLocsUnitPrice = $('#CostPerAdditionalLocation').val();
            MANAGE.addLocsTotalPrice = $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val();
            MANAGE.retrivedAdditionalLocations = false;
        });
    });
    
});

MANAGE.primeDomVariables = function() {
    MANAGE.addLocationsButton = $('#AddLocationsButton');
    MANAGE.newLocationsContainer = $('#NewLocationsContainer'); // commented out in razor
};

MANAGE.wireUpHandlers = function () {

    locationsSpanPrefix = 'LocationSpan-',
    breakSuffix = '-break';

    var modalFormOptions = {
        keyboard: true,
        backdrop: 'static',
        show: true
    };


    $('#AddLocationsButton').on('click', function (e) {

        e.preventDefault();

        var webinarId = $('#WebinarId').val().toString();
        var userId = $('#UserId').val().toString();

        modalFormOptions.remote = '/Admin/GetAdditionalLocationByOrderId/' + MANAGE.idOrder;

        if (MANAGE.retrivedAdditionalLocations) {
            modalFormOptions.remote = '';
        }

        $('#additionalLocationsModalDialog').modal(modalFormOptions);
        MANAGE.retrivedAdditionalLocations = true;
    });

    $('#additionalLocationsModalDialog').on('shown', function() {
        MANAGE.wireUpHandlersForModal();
        $('#AddInputsButton').focus();
    });
};

MANAGE.wireUpHandlersForModal = function() {
    var locationsCloned, locationsBakForCancel;
    var collectAdditionalLocations = $('#collectAdditionalLocations');
    var locations = collectAdditionalLocations.children();

    if (locations.length > 0) {
        locationsCloned = locations.clone();
        locationsBakForCancel = locations.clone();
    }

    collectAdditionalLocations.empty();

    var emailInputsAdded = $.Deferred(function() {
        additionalLocationEmailWrapper = $('#AdditionalLocationEmailWrapper');
        if (locationsCloned)
            additionalLocationEmailWrapper.append(locationsCloned);
    });

    $.when(emailInputsAdded.resolve()).then(function() {
        numberOfAdditionalLocations = $('#AdditionalLocationEmailWrapper input[type="email"]').length;
        MANAGE.numberAddLocsLabel.text(numberOfAdditionalLocations);

        if (numberOfAdditionalLocations < 1) {
            //$('#sumbitAdditionalLocationsButton').off('click');

        } else {

            if ($('#sumbitAdditionalLocationsButton').length < 1) {

                $('#AdditionalLocationEmailWrapper').after($('<button>',
                {
                    id: 'sumbitAdditionalLocationsButton',
                    text: 'Submit',
                    'class': 'btn btn-primary',
                }));

                $('#sumbitAdditionalLocationsButton').on('click', function() {

                    collectAdditionalLocations.empty();
                    collectAdditionalLocations.append(additionalLocationEmailWrapper.children());

                    $('#additionalLocationsModalDialog').modal('hide');

                    $(this).remove();
                });
                var trashCans = additionalLocationEmailWrapper.find('i');

                $.each(trashCans, function(idx, i) {
                    $(i).on('click', MANAGE.deleteItem);
                });
            }
        }

        $('#AddInputsButton').on('click', function() {

            if (numberOfAdditionalLocations === 0) {
                $('#AdditionalLocationEmailWrapper').after($('<button>',
                {
                    id: 'sumbitAdditionalLocationsButton',
                    text: 'Submit',
                    'class': 'btn btn-primary',
                }));

                $('#sumbitAdditionalLocationsButton').on('click', function() {

                    collectAdditionalLocations.empty();
                    collectAdditionalLocations.append(additionalLocationEmailWrapper.children());

                    $('#additionalLocationsModalDialog').modal('hide');

                    $(this).remove();
                });
            }

            var newId;
            if (numberOfAdditionalLocations === 0) {
                newId = 0;
            } else {
                // first get the last previous email input
                var lastInput = additionalLocationEmailWrapper.find('input[type="email"]:last');
                // get its id
                var lastInputId = lastInput.attr('id');
                var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
                newId = id + 1;
            }
            additionalLocationEmailWrapper.append('<span id="' + locationsSpanPrefix + newId + '"><input id="AdditionalLocationEmail_' + newId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-AdditionLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
            additionalLocationEmailWrapper.find('i#' + newId + '-AdditionLocationEmail-delete').on('click', MANAGE.deleteItem);
            $('#AdditionalLocationEmail_' + newId).focus();
            numberOfAdditionalLocations++;
            MANAGE.adjustAdditionalLocationsData(numberOfAdditionalLocations);
        });

        $('#closeButton, #additionalLocationsModalDialog > div > div.modal-header > button').on('click', function(e) {
            if (locationsBakForCancel) {

                $.each(locationsBakForCancel.find('i'), function(idx, i) {
                    $(i).on('click', MANAGE.deleteItem);
                });
                collectAdditionalLocations.append(locationsBakForCancel);
            }

        });
    });
};

MANAGE.deleteItem = function (event) {
    numberOfAdditionalLocations--;
    MANAGE.adjustAdditionalLocationsData(numberOfAdditionalLocations);
    var trashClicked = event.currentTarget.id;
    var idx = trashClicked.substring(0, 1);
    var spanToRemove = locationsSpanPrefix + idx;

    $('#' + spanToRemove).hide(500, function () {
        $(this).remove();
    });

    $('#' + idx + breakSuffix).hide(500, function () {
        $(this).remove();
    });

    if (numberOfAdditionalLocations < 1) {
        $('#sumbitAdditionalLocationsButton').hide(300, function () {
            $(this).remove();
        });
    }
};

MANAGE.adjustAdditionalLocationsData = function(nr) {
    var newAddLocsPrice = nr * MANAGE.addLocsUnitPrice;
    MANAGE.numberAddLocsLabel.text(nr);
    $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val(newAddLocsPrice);
};