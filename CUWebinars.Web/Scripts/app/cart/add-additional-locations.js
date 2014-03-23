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

    addLocationsButton.on('click', function(e) {
        var numberOfInputsToAdd = numberOfEmailAddresses.val();
        newLocationsContainer.empty();

        for (var i = 0; i < numberOfInputsToAdd; i++) {
            newLocationsContainer.append('<span><input id="AdditionLocationEmail-' + i + '" type="email" placeholder="Enter email address" /></span> <br>');
        }

        if (typeof additionalLocationsSubmitButton === 'undefined') {
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

        newLocationsContainer.append(additionalLocationsSubmitButton);

    });

    signupForm.on('submit', function (e) {
        e.preventDefault();

        var mode = $('#Mode').val();
        var stageOfCheckout = $('#Stage_of_checkout').val();
        var orderId = $('[name="order.idOrder"]');
        var webUserId = $('[name="webuser.idUser"]');
        var webinarId = $('[name="webinar.idWebinar"]');

        var emailAddresses = Object(); 

        var emailNodes = newLocationsContainer.find('input[type=email]');

        $.each(emailNodes, function (idx, input) {
            emailAddresses[idx] = $(input).val();
        });

        var payload = {
            emailAddresses: emailAddresses,
            'Order.dOrder': orderId.val(),
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