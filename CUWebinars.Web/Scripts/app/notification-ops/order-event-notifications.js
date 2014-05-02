var inputFormFieldsDiv,
    sendShippedOrderNotificationButton,
    waitIndicator;



var OENS = {
    PageObjects: {

        InputFormFieldsDiv: function() {
            return inputFormFieldsDiv || $('#InputFormFieldsDiv');
        },
        SendShippedOrderNotificationButton: function() {
            return sendShippedOrderNotificationButton || $('#SendShippedOrderNotificationButton');
        },
        WaitIndicator: function () {
            return waitIndicator || $('#WaitIndicator');
        }
    }
};

$(function () {
    sendShippedOrderNotificationButton = $('#SendShippedOrderNotificationButton');
    inputFormFieldsDiv = $('#InputFormFieldsDiv');
    waitIndicator = $('#WaitIndicator');
    waitIndicator.hide();

    sendShippedOrderNotificationButton.on('click', function(evtArgs) {

        OENS.PageObjects.InputFormFieldsDiv().empty();

        OENS.PageObjects.InputFormFieldsDiv().append('<input id="OrderIdInput" class="input input-large" type="Text" />');
        OENS.PageObjects.InputFormFieldsDiv().append('<br /><button id="OrderIdSubmitButton" class="btn" style="margin-top:10px;">Fire Event</button>');

        $('#OrderIdSubmitButton').on('click', function(evtArgs) {
            var orderId = $.trim($('#OrderIdInput').val());
            var url = '/OrderEventFiringOps/FireSendOrderShippedEvent';

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                data: JSON.stringify({ orderId: orderId }),
                beforeSend: function () {
                    // this is where we append a loading image
                    OENS.PageObjects.WaitIndicator().show();
                }
            }).done(function (returnData) {
                // successful request; do something with the returnData
                if (returnData.Result === 'Success') {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-success">&nbsp;&nbsp;Event Fired Successfully</span>');
                }
                else if (returnData.Result === 'Fail') {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-important">&nbsp;&nbsp;Event Firing Has Failed</span>');
                }
            }).fail(function() {
                OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-important">&nbsp;&nbsp;Event Firing Has Failed</span>');
            }).always(function() {
                OENS.PageObjects.WaitIndicator().hide();
            });

        });

    });
});