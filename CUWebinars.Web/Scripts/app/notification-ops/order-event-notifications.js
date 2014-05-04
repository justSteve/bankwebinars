var inputFormFieldsDiv,
    getSendConnectionInfoEventHtmlButton,
    getSendReminderEventHtmlButton,
    sendShippedOrderNotificationButton,
    waitIndicator;


//  Create a namespace. PageObjects is getting polluted across js files.
var OENS = {
    PageObjects: {

        GetSendConnectionInfoEventHtmlButton: function () {
            return getSendConnectionInfoEventHtmlButton || $('#GetSendConnectionInfoEventHtmlButton');
        },
        GetSendReminderEventHtmlButton: function () {
            return getSendReminderEventHtmlButton || $('#GetSendReminderEventHtmlButton');
        },
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

$(function() {
    getSendConnectionInfoEventHtmlButton = $('#GetSendConnectionInfoEventHtmlButton');
    getSendReminderEventHtmlButton = $('#GetSendReminderEventHtmlButton');
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
                beforeSend: function() {
                    // this is where we append a loading image
                    OENS.PageObjects.WaitIndicator().show();
                }
            }).done(function(returnData) {
                // successful request; do something with the returnData
                if (returnData.Result === 'Success') {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-success">&nbsp;&nbsp;Event Fired Successfully</span>');
                } else if (returnData.Result === 'Fail') {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-important">&nbsp;&nbsp;Event Firing Has Failed</span>');
                }
            }).fail(function() {
                OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-important">&nbsp;&nbsp;Event Firing Has Failed</span>');
            }).always(function() {
                OENS.PageObjects.WaitIndicator().hide();
            });

        });

    });

    getSendReminderEventHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFieldsDiv().empty();

        OENS.PageObjects.InputFormFieldsDiv().load('/OrderEventFiringOps/SendReminder', function() {

            $('#FireSendReminderEventButton').on('click', function (eventArgs) {
                var url = '/OrderEventFiringOps/SendReminder';
                var payload = $('#SelectedUpcomingWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function() {
                        // this is where we append a loading image
                        //pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                    }
                }).done(function(result) {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-success">&nbsp;The Orders are being sent.</span>');
                });
            });
        });
    });

    getSendConnectionInfoEventHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFieldsDiv().empty();

        
        OENS.PageObjects.InputFormFieldsDiv().load('/OrderEventFiringOps/SendConnectionInfo', function () {
            
            $('#GetSendConnectionInfoRecipientsButton').on('click', function (eventArgs) {
                var url = '/OrderEventFiringOps/SendConnectionInfo';
                var payload = $('#SelectedUpcomingWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: url,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function () {
                        // this is where we append a loading image
                        //pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                    }
                }).done(function (result) {
                    OENS.PageObjects.InputFormFieldsDiv().append('<br /><span class="label label-success">&nbsp;The Orders are being sent.</span>');
                });
            });
        });
    });
});