//  Rename this file 'additional-locations' if we decide to be rid of additional-locations.js

var additionalLocationsContainer,
    addLocationsButton,
    additionalLocationsSubmitButton,
    newLocationsContainer,
    numberOfEmailAddresses,
    signupForm;

$(function() {
    primeDomVariables();
    wireUpHandlers();

});

function wireUpHandlers() {

    var locationsSpanPrefix = 'LocationSpan-',
        breakSuffix = '-break';

    var modalFormOptions = {
        keyboard: true,
        show: true,
        //remote: '/Webinar/GetAdditionalLocationsByOrderId'
    };

    $('#LoadAddLocationsModalButton').on('click', function () {
        modalFormOptions.remote = '/Webinar/GetAdditionalLocationsByOrderId/' + $('#CheckoutOptionsViewModel_Order_idOrder').val();
        $('#SignupModal').modal(modalFormOptions);
    });

    
    $('#myModal').on('show', function() {
        // show load div
        var oi = 'l';
    });

    //$('#LoadAddLocationsModalButton').on('click', function(e) {
        
    //});

    addLocationsButton.on('click', function(e) {
        var numberOfInputsToAdd = numberOfEmailAddresses.val();
        newLocationsContainer.empty();

        for (var i = 0; i < numberOfInputsToAdd; i++) {
            newLocationsContainer.append('<span id="' + locationsSpanPrefix + i + '"><input id="AdditionLocationEmail-' + i + '" name="AddAdditionalLocationsViewModel.Emails[' + i + ']" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + i + '-AdditionLocationEmail-delete"></i></span> <br id="' + i + breakSuffix + '">');
        }

        if (typeof additionalLocationsSubmitButton === 'undefined' && numberOfInputsToAdd > 0) {
            additionalLocationsSubmitButton = $('<input>',
            {
                id: 'AdditionalLocationsSubmitButton',
                value: 'Submit Locations',
                'class': 'btn btn-success btn-small',
                type: 'submit'
            });
            //additionalLocationsSubmitButton.on('click', function () {
            //    signupForm.submit();
            //});
        }

        newLocationsContainer.find('i').on('click', function (event) {
            var trashClicked = event.currentTarget.id;
            var idx = trashClicked.substring(0, 1);
            var spanToRemove = locationsSpanPrefix + idx;
            
            $('#' + spanToRemove).hide(500, function() {
                $(this).remove();
                $('#' + idx + breakSuffix).remove();

                var inputsRemaining = newLocationsContainer.find('i');

                if (inputsRemaining.length < 1) {
                    additionalLocationsSubmitButton.hide(500, function() {
                        $(this).remove();
                    });
                }

            });
        });

        newLocationsContainer.append(additionalLocationsSubmitButton);

        if (!additionalLocationsSubmitButton.is(':visible'))
            additionalLocationsSubmitButton.show(500);

        if (numberOfInputsToAdd < 1)
            additionalLocationsSubmitButton.remove();
    });

    signupForm.on('submit', function (e) {
        e.preventDefault();

        var mode = $('#Mode').val();
        var stageOfCheckout = $('#Stage_of_checkout').val();
        var orderId = $('[name="CheckoutOptionsViewModel.Order.idOrder"]');
        var webUserId = $('[name="WebUser.idUser"]');
        var webinarId = $('[name="Webinar.idWebinar"]');

        var emailAddresses = Object(); 

        var emailNodes = newLocationsContainer.find('input[type=email]');

        $.each(emailNodes, function (idx, input) {
            emailAddresses[idx] = $(input).val();
        });

        var payload = {
            'CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationsViewModel.AddAdditionalLocationsViewModel.Emails': emailAddresses,
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
    });
};

function primeDomVariables() {
    additionalLocationsContainer = $('#AdditionalLocationsContainer');
    addLocationsButton = $('#AddLocationsButton');
    newLocationsContainer = $('#NewLocationsContainer');
    numberOfEmailAddresses = $('#NumberOfEmailAddresses');
    signupForm = $('#Signup2');
}