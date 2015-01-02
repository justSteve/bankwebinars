var MANAGE = {};


$(function () {

    var orderIdInput = $('#orderIdInput');
    orderIdInput.focus();

    $('#getOrderButton').on('click', function () {
        MANAGE.idOrder = orderIdInput.val();

        $('#orderRelatedFields').load('/Admin/GetOrderDetails/' + MANAGE.idOrder, function () {

            MANAGE.locationsSpanPrefix = 'LocationSpan-';
            MANAGE.breakSuffix = '-break';
            
            MANAGE.primeDomVariables();
            MANAGE.wireUpHandlers();
            MANAGE.numberAddLocsLabel = $('#nrAddLocs');
            MANAGE.addLocsUnitPrice = $('#CostPerAdditionalLocation').val();
            MANAGE.addLocsTotalPrice = $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val();
            MANAGE.retrivedAdditionalLocations = false;

            MANAGE.wireUpTrashIcons();
        });
    });
});

MANAGE.addAdditionalLocation = function (e) {

    e.preventDefault();

    var newId;

    if (MANAGE.numberOfAdditionalLocations === 0) {
        newId = 0;
    } else {
        // first get the last previous email input
        var lastInput = MANAGE.wrapperDiv.find('input[type="email"]:last');
        // get its id
        var lastInputId = lastInput.attr('id');
        var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
        newId = id + 1;
    }

    var trashIconId = newId + '-AdditionLocationEmail-delete';
    var additionalLocationEmailId = 'AdditionalLocationEmail_' + newId;

    MANAGE.wrapperDiv.append('<span id="' + MANAGE.locationsSpanPrefix + newId + '"><input id="' + additionalLocationEmailId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" aria-describedby="AdditionalLocationEmail_'+ newId +'-error" aria-invalid="false"></input>&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + trashIconId + '"></i></span> <br id="' + newId + MANAGE.breakSuffix + '">');

    $('#' + trashIconId).on('click', MANAGE.deleteItem);
    $('#' + additionalLocationEmailId).focus();

    MANAGE.numberOfAdditionalLocations += 1;
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
};

MANAGE.wireUpTrashIcons = function () {
    
    var trashCans = MANAGE.wrapperDiv.find('i');
    MANAGE.numberOfAdditionalLocations = MANAGE.numberAddLocsLabel.text();

    $.each(trashCans, function (idx, i) {
        $(i).on('click', MANAGE.deleteItem);
    });
};

MANAGE.primeDomVariables = function() {
    MANAGE.wrapperDiv = $('#collectAdditionalLocations');
};

MANAGE.wireUpHandlers = function () {

    $('#addLocationsButton').on('click', MANAGE.addAdditionalLocation);
    $('#editOrderSubmitButton').on('click', function (e) {

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
            beforeSend: function() {
                // this is where we append a loading image
                //$('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Checking that Email...</span>');
            }
        }).done(function(data, bla, bla) {

        }).fail(function(data, bla, bla) {
            
        }).always(function(data, bla, bla) {
            
        });

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

MANAGE.adjustAdditionalLocationsData = function(nr) {
    var newAddLocsPrice = nr * MANAGE.addLocsUnitPrice;
    MANAGE.numberAddLocsLabel.text(nr);
    $('#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOptions').val(newAddLocsPrice);
};