//  Rename this file 'additional-locations' if we decide to be rid of additional-locations.js

//var additionalLocationContainer,
var addLocationsButton,
    AdditionalLocationSubmitButton,
    newLocationsContainer;
    //numberOfEmailAddresses,
    //signupForm;

var additionalLocationEmailWrapper,
    breakSuffix = '-break',
    deleteItem,
    locationsSpanPrefix = 'LocationSpan-',
    numberOfAdditionalLocations;


deleteItem = function (event) {
    numberOfAdditionalLocations--;
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

$(function() {
    primeDomVariables();
    wireUpHandlers();

});

function wireUpHandlers() {

    var locationsSpanPrefix = 'LocationSpan-',
        breakSuffix = '-break';

    var modalFormOptions = {
        keyboard: true,
        backdrop: 'static',
        show: true,
        remote : '/Cart/GetAdditionalLocationByOrderId/2/1'
    };


    $('#AddLocationsButton').on('click', function (e) {

        e.preventDefault();
        
        modalFormOptions.remote = '/Cart/GetAdditionalLocationByOrderId/' + $('#Webinar_idWebinar').val() + '/' + ($('#WebUser_idUser').val() || 0).toString();
        $('#additionalLocationsModalDialog').modal(modalFormOptions);
    });

    $('#additionalLocationsModalDialog').on('shown', function() {
        wireUpHandlersForModal();
    });
    

    //signupForm.on('submit', function (e) {
    //    e.preventDefault();

    //    var mode = $('#Mode').val();
    //    var stageOfCheckout = $('#Stage_of_checkout').val();
    //    var orderId = $('[name="CheckoutOptionsViewModel.Order.idOrder"]');
    //    var webUserId = $('[name="WebUser.idUser"]');
    //    var webinarId = $('[name="Webinar.idWebinar"]');

    //    var emailAddresses = Object(); 

    //    var emailNodes = newLocationsContainer.find('input[type=text]');

    //    $.each(emailNodes, function (idx, input) {
    //        emailAddresses[idx] = $(input).val();
    //    });

    //    var payload = {
    //        'CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationViewModel.AddAdditionalLocationViewModel.Emails': emailAddresses,
    //        'CheckoutOptionsViewModel.Order.idOrder': orderId.val(),
    //        'WebUser.idUser': webUserId.val(),
    //        'Webinar.idWebinar': webinarId.val(),
    //        mode: 0,
    //        stageOfCheckout: stageOfCheckout,
    //        sixMonthPaidConnectionsCount: '',
    //        twelveMonthPaidConnectionsCount: ''
    //    };

    //    $.ajax({
    //        url: '/Cart/Signup2',
    //        type: 'POST',
    //        data: payload,
    //        dataType: 'json',
    //        contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
    //    }).done(function (msg) {
    //        var bla = msg;
    //    });

    //    return false;
    //});
};

function primeDomVariables() {
    //additionalLocationContainer = $('#AdditionalLocationContainer'); // not used
    addLocationsButton = $('#AddLocationsButton');
    newLocationsContainer = $('#NewLocationsContainer'); // commented out in razor
    //numberOfEmailAddresses = $('#NumberOfEmailAddresses'); // not used
    //signupForm = $('#RegisterAdditionalLocationsForm');
}

function wireUpHandlersForModal() {
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

    $.when(emailInputsAdded.resolve()).then(function () {
        numberOfAdditionalLocations = $('#AdditionalLocationEmailWrapper input[type="email"]').length;

        if (numberOfAdditionalLocations < 1) {
            //$('#sumbitAdditionalLocationsButton').off('click');
            
        } else {

            $('#AdditionalLocationEmailWrapper').after($('<button>',
                    {
                        id: 'sumbitAdditionalLocationsButton',
                        text: 'Submit',
                        'class': 'btn btn-primary',
                    }));

            $('#sumbitAdditionalLocationsButton').on('click', function () {

                collectAdditionalLocations.empty();
                collectAdditionalLocations.append(additionalLocationEmailWrapper.children());

                $('#additionalLocationsModalDialog').modal('hide');

                $(this).remove();
            });
            var trashCans = additionalLocationEmailWrapper.find('i');

            $.each(trashCans, function (idx, i) {
                $(i).on('click', deleteItem);
            });
        }

        $('#AddInputsButton').on('click', function () {

            if (numberOfAdditionalLocations == 0) {
                $('#AdditionalLocationEmailWrapper').after($('<button>',
                {
                    id: 'sumbitAdditionalLocationsButton',
                    text: 'Submit',
                    'class': 'btn btn-primary',
                }));

                $('#sumbitAdditionalLocationsButton').on('click', function () {

                    collectAdditionalLocations.empty();
                    collectAdditionalLocations.append(additionalLocationEmailWrapper.children());

                    $('#additionalLocationsModalDialog').modal('hide');

                    $(this).remove();
                });
            }

            additionalLocationEmailWrapper.append('<span id="' + locationsSpanPrefix + numberOfAdditionalLocations + '"><input id="AdditionalLocationEmail_' + numberOfAdditionalLocations + '" name="AdditionalLocations[' + numberOfAdditionalLocations + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + numberOfAdditionalLocations + '-AdditionLocationEmail-delete"></i></span> <br id="' + numberOfAdditionalLocations + breakSuffix + '">');
            additionalLocationEmailWrapper.find('i#' + numberOfAdditionalLocations + '-AdditionLocationEmail-delete').on('click', deleteItem);
            $('#AdditionalLocationEmail_' + numberOfAdditionalLocations).focus();
            numberOfAdditionalLocations++;

        });

        $('#closeButton').on('click', function(e) {
            if (locationsBakForCancel) {

                $.each(locationsBakForCancel.find('i'), function (idx, i) {
                    $(i).on('click', deleteItem);
                });
                collectAdditionalLocations.append(locationsBakForCancel);
            }

        });
    });
}