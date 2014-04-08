$(function () {
    //primeDomVariables();
    wireUpHandlers();

});


function wireUpHandlers() {

    var locationsSpanPrefix = 'LocationSpan-',
        breakSuffix = '-break';
    $('#SumbitAdditionalLocationsForm').on('click', function() {
        alert('hi');
    });

    $('#RegisterAdditionalLocationsForm').on('submit', function (e) {
        alert('hi dave');
        debugger;
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
