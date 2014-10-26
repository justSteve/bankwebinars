//  Rename this file 'additional-locations' if we decide to be rid of additional-locations.js

var additionalLocationContainer,
    addLocationsButton,
    AdditionalLocationSubmitButton,
    newLocationsContainer,
    numberOfEmailAddresses,
    signupForm;

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
        $('#' + idx + breakSuffix).remove();
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

    
    //$('#myModal').on('show', function() {
    //    // show load div
    //    var oi = 'l';
    //});

    //$('#LoadAddLocationsModalButton').on('click', function(e) {
        
    //});

    //addLocationsButton.on('click', function(e) {
    //    var numberOfInputsToAdd = numberOfEmailAddresses.val();
    //    newLocationsContainer.empty();

    //    for (var i = 0; i < numberOfInputsToAdd; i++) {
    //        newLocationsContainer.append('<span id="' + locationsSpanPrefix + i + '"><input id="AdditionLocationEmail-' + i + '" name="AddAdditionalLocationViewModel.Emails[' + i + ']" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + i + '-AdditionLocationEmail-delete"></i></span> <br id="' + i + breakSuffix + '">');
    //    }

    //    if (typeof AdditionalLocationSubmitButton === 'undefined' && numberOfInputsToAdd > 0) {
    //        AdditionalLocationSubmitButton = $('<input>',
    //        {
    //            id: 'AdditionalLocationSubmitButton',
    //            value: 'Submit Locations',
    //            'class': 'btn btn-success btn-small',
    //            type: 'submit'
    //        });
    //        //AdditionalLocationSubmitButton.on('click', function () {
    //        //    signupForm.submit();
    //        //});
    //    }

    //    newLocationsContainer.find('i').on('click', function (event) {
    //        var trashClicked = event.currentTarget.id;
    //        var idx = trashClicked.substring(0, 1);
    //        var spanToRemove = locationsSpanPrefix + idx;
            
    //        $('#' + spanToRemove).hide(500, function() {
    //            $(this).remove();
    //            $('#' + idx + breakSuffix).remove();

    //            var inputsRemaining = newLocationsContainer.find('i');

    //            if (inputsRemaining.length < 1) {
    //                AdditionalLocationSubmitButton.hide(500, function() {
    //                    $(this).remove();
    //                });
    //            }

    //        });
    //    });

    //    newLocationsContainer.append(AdditionalLocationSubmitButton);

    //    if (!AdditionalLocationSubmitButton.is(':visible'))
    //        AdditionalLocationSubmitButton.show(500);

    //    if (numberOfInputsToAdd < 1)
    //        AdditionalLocationSubmitButton.remove();
    //});

    signupForm.on('submit', function (e) {
        e.preventDefault();

        var mode = $('#Mode').val();
        var stageOfCheckout = $('#Stage_of_checkout').val();
        var orderId = $('[name="CheckoutOptionsViewModel.Order.idOrder"]');
        var webUserId = $('[name="WebUser.idUser"]');
        var webinarId = $('[name="Webinar.idWebinar"]');

        var emailAddresses = Object(); 

        var emailNodes = newLocationsContainer.find('input[type=text]');

        $.each(emailNodes, function (idx, input) {
            emailAddresses[idx] = $(input).val();
        });

        var payload = {
            'CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationViewModel.AddAdditionalLocationViewModel.Emails': emailAddresses,
            'CheckoutOptionsViewModel.Order.idOrder': orderId.val(),
            'WebUser.idUser': webUserId.val(),
            'Webinar.idWebinar': webinarId.val(),
            mode: 0,
            stageOfCheckout: stageOfCheckout,
            sixMonthPaidConnectionsCount: '',
            twelveMonthPaidConnectionsCount: ''
        };

        $.ajax({
            url: '/Cart/Signup2',
            type: 'POST',
            data: payload,
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
        }).done(function (msg) {
            var bla = msg;
        });

        return false;
    });
};

function primeDomVariables() {
    additionalLocationContainer = $('#AdditionalLocationContainer'); // not used
    addLocationsButton = $('#AddLocationsButton');
    newLocationsContainer = $('#NewLocationsContainer'); // commented out in razor
    numberOfEmailAddresses = $('#NumberOfEmailAddresses'); // not used
    signupForm = $('#RegisterAdditionalLocationsForm');
}

function wireUpHandlersForModal() {

    numberOfAdditionalLocations = $('#AdditionalLocationEmailWrapper input[type="text"]').length;
    if (numberOfAdditionalLocations < 1) {
        $('#sumbitAdditionalLocationsButton').off('click');
        $('#sumbitAdditionalLocationsButton').remove();

        additionalLocationEmailWrapper = $('#AdditionalLocationEmailWrapper');

        $('#AddInputsButton').on('click', function () {

            if (numberOfAdditionalLocations == 0) {
                $('#AdditionalLocationEmailWrapper').after($('<button>',
                {
                    id: 'sumbitAdditionalLocationsButton',
                    text: 'Submit',
                    'class': 'btn btn-primary',
                }));

                $('#sumbitAdditionalLocationsButton').on('click', function () {

                    var emailNodes = additionalLocationEmailWrapper.find('input[type=email]');
                    var trashCanNodes = additionalLocationEmailWrapper.find('i');

                    //$.each(emailNodes, function (idx, input) {
                    //    $(input).off('click');
                    //});

                    //$.each(trashCanNodes, function (idx, i) {
                    //    $(i).off('click');
                    //});

                    // copy inputs from modal to formInputs, which we'll copy to the main page.
                    //var formInputs = additionalLocationEmailWrapper.clone(); 

                    // Once copied, blow away the inputs in the modal.
                    //$.each(emailNodes, function (idx, input) {
                    //    $('#' + idx + breakSuffix).remove();
                    //    $(input).parent().remove();
                    //});

                    //var newTrashCans = formInputs.find('i');

                    //$.each(newTrashCans, function (idx, i) {
                    //    $(i).on('click', deleteItem);
                    //});

                    $('#collectAdditionalLocations').append(additionalLocationEmailWrapper.children());

                    //formInputs.remove();

                    $('#additionalLocationsModalDialog').modal('hide');

                    $(this).remove();
                });
            }

            additionalLocationEmailWrapper.append('<span id="' + locationsSpanPrefix + numberOfAdditionalLocations + '"><input id="AdditionalLocationEmail_' + numberOfAdditionalLocations + '" name="AdditionalLocations[' + numberOfAdditionalLocations + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + numberOfAdditionalLocations + '-AdditionLocationEmail-delete"></i></span> <br id="' + numberOfAdditionalLocations + breakSuffix + '">');
            additionalLocationEmailWrapper.find('i#' + numberOfAdditionalLocations + '-AdditionLocationEmail-delete').on('click', deleteItem);
            $('#AdditionalLocationEmail_' + numberOfAdditionalLocations).focus();
            numberOfAdditionalLocations++;

        });

        additionalLocationEmailWrapper.find('i').on('click', deleteItem);
    }
}