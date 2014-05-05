var inputFormFieldsDiv,
    failedScreenMessage,
    getSendConnectionInfoEventHtmlButton,
    getSendRecordingPostedEventHtmlButton,
    getSendReminderEventHtmlButton,
    noOrdersScreenMessage,
    sendConnectionInfoUrl,
    sendOrderShippedUrl,
    sendRecordingPostedUrl,
    sendReminderUrl,
    sendShippedOrderNotificationButton,
    successScreenMessage,
    waitIndicator;


//  Create a namespace. PageObjects is getting polluted across js files.
var OENS = {
    PageObjects: {
        GetSendConnectionInfoEventHtmlButton: function () {
            return getSendConnectionInfoEventHtmlButton || $('#GetSendConnectionInfoEventHtmlButton');
        },
        GetSendRecordingPostedEventHtmlButton: function () {
            return getSendRecordingPostedEventHtmlButton || $('#GetSendRecordingPostedEventHtmlButton');
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

$(function () {
    failedScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;Event Firing Has Failed</span>';
    noOrdersScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There were no orders for that webinar</span>';
    successScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;The Orders have been sent.</span>';
    sendConnectionInfoUrl = '/OrderEventFiringOps/SendConnectionInfo';
    sendReminderUrl = '/OrderEventFiringOps/SendReminder';
    sendRecordingPostedUrl = '/OrderEventFiringOps/SendRecordingPosted';
    sendOrderShippedUrl = '/OrderEventFiringOps/SendShippedOrder';

    getSendConnectionInfoEventHtmlButton = $('#GetSendConnectionInfoEventHtmlButton');
    getSendRecordingPostedEventHtmlButton = $('#GetSendRecordingPostedEventHtmlButton');
    getSendReminderEventHtmlButton = $('#GetSendReminderEventHtmlButton');
    sendShippedOrderNotificationButton = $('#SendShippedOrderNotificationButton');
    inputFormFieldsDiv = $('#InputFormFieldsDiv');
    waitIndicator = $('#WaitIndicator');
    waitIndicator.hide();

    sendShippedOrderNotificationButton.on('click', function(evtArgs) {

        OENS.PageObjects.InputFormFieldsDiv().empty();
        OENS.PageObjects.InputFormFieldsDiv().load(sendOrderShippedUrl, function () {

            $('#GetSendShippedOrderNotificationButton').on('click', function (eventArgs) {

                var payload = $('#SelectedOrderId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendOrderShippedUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ orderId: payload }),
                    beforeSend: function () {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    OENS.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });

    });

    getSendReminderEventHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFieldsDiv().empty();

        OENS.PageObjects.InputFormFieldsDiv().load(sendReminderUrl, function() {

            $('#FireSendReminderEventButton').on('click', function (eventArgs) {
                
                var payload = $('#SelectedWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendReminderUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function() {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    OENS.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });
    });

    getSendConnectionInfoEventHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFieldsDiv().empty();

        
        OENS.PageObjects.InputFormFieldsDiv().load(sendConnectionInfoUrl, function () {
            
            $('#GetSendConnectionInfoRecipientsButton').on('click', function (eventArgs) {
                
                var payload = $('#SelectedWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function () {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }

                }).fail(function () {

                    labelCheckRemove();

                    OENS.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });
    });

    getSendRecordingPostedEventHtmlButton.on('click', function (eventArgs) {
        eventArgs.preventDefault();
        OENS.PageObjects.InputFormFieldsDiv().empty();

        
        OENS.PageObjects.InputFormFieldsDiv().load(sendRecordingPostedUrl, function () {
            
            $('#GetSendRecordingPostedRecipientsButton').on('click', function (eventArgs) {
                
                var payload = $('#SelectedWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendRecordingPostedUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function () {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }

                }).fail(function () {
                    labelCheckRemove();

                    OENS.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });
            });
        });
    });

    var labelCheckRemove = function() {
        if ($('#ScreenMessageSpan').length > 0) {
            $('#ScreenMessageSpan').siblings('br').remove();
            $('#ScreenMessageSpan').remove();
        }
    };
});