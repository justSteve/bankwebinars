var adhocNotificationForm,
    InputFormFields,
    failedScreenMessage,
    getAdhocEventsHtmlButton,
    getResendConnectionInfoHtmlButton,
    getResendOrderConfirmationHtmlButton,
    getSendConnectionInfoEventHtmlButton,
    getSendRecordingPostedEventHtmlButton,
    getSendReminderEventHtmlButton,
    noOrderScreenMessage,
    noOrdersScreenMessage,
    getRecipientsButton,
    regTypesCheckBoxesDiv,
    resendConnectionInfoUrl,
    resendOrderConfirmationUrl,
    selectedUpcomingWebinarId,
    selectedUpcomingWebinarIdDropDown,
    sendAdhocEventUrl,
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
        getResendConnectionInfoHtmlButton: function () {
            return getResendConnectionInfoHtmlButton || $('#GetResendConnectionInfoHtmlButton');
        },
        GetResendOrderConfirmationHtmlButton: function () {
            return getResendOrderConfirmationHtmlButton|| $('#GetResendOrderConfirmationHtmlButton');
        },
        GetSendConnectionInfoEventHtmlButton: function () {
            return getSendConnectionInfoEventHtmlButton || $('#GetSendConnectionInfoEventHtmlButton');
        },
        GetSendRecordingPostedEventHtmlButton: function () {
            return getSendRecordingPostedEventHtmlButton || $('#GetSendRecordingPostedEventHtmlButton');
        },
        GetSendReminderEventHtmlButton: function () {
            return getSendReminderEventHtmlButton || $('#GetSendReminderEventHtmlButton');
        },
        InputFormFields: function() {
            return InputFormFields || $('#InputFormFields');
        },
        RegTypesCheckBoxesDiv: function () {
            return regTypesCheckBoxesDiv || $('#RegTypesCheckBoxes').find('.controls');
        },
        SelectedUpcomingWebinarIdDropDown: function () {
            return selectedUpcomingWebinarIdDropDown || $('#SelectedWebinarId');
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

    getAdhocEventsHtmlButton = $('#GetAdhocEventsHtmlButton');

    failedScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;Event Firing Has Failed</span>';
    noOrdersScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There were no orders for that webinar</span>';
    noOrderScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There is no order which matches that Order Id</span>';
    successScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;The Orders have been sent.</span>';
    sendAdhocEventUrl = '/Admin/SendAdhocEvent';
    sendConnectionInfoUrl = '/Admin/SendConnectionInfo';
    sendReminderUrl = '/Admin/SendReminder';
    sendRecordingPostedUrl = '/Admin/SendRecordingPosted';
    resendConnectionInfoUrl = '/Admin/ResendConnectionInfo';
    resendOrderConfirmationUrl = '/Admin/ResendOrderConfirmation';
    sendOrderShippedUrl = '/Admin/SendShippedOrder';

    getResendConnectionInfoHtmlButton = $('#GetResendConnectionInfoHtmlButton');
    getResendOrderConfirmationHtmlButton = $('#GetResendOrderConfirmationHtmlButton');
    getSendConnectionInfoEventHtmlButton = $('#GetSendConnectionInfoEventHtmlButton');
    getSendRecordingPostedEventHtmlButton = $('#GetSendRecordingPostedEventHtmlButton');
    getSendReminderEventHtmlButton = $('#GetSendReminderEventHtmlButton');
    sendShippedOrderNotificationButton = $('#SendShippedOrderNotificationButton');
    InputFormFields = $('#InputFormFields');
    waitIndicator = $('#WaitIndicator');
    waitIndicator.hide();


    getResendConnectionInfoHtmlButton.on('click', function(eventArgs) {
        eventArgs.preventDefault();
        OENS.PageObjects.InputFormFields().empty();

        OENS.PageObjects.InputFormFields().load(resendConnectionInfoUrl, function () {

            $('#ResendConnectionInfoButton').on('click', function () {

                labelCheckRemove();

                var orderId = $.trim($('#OrderId').val());

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: resendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ orderId: orderId }),
                    beforeSend: function () {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        OENS.PageObjects.InputFormFields().append(noOrderScreenMessage);
                    }

                }).fail(function () {

                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });
            });

        });
    });
            
    getResendOrderConfirmationHtmlButton.on('click', function(eventArgs) {
        eventArgs.preventDefault();
        OENS.PageObjects.InputFormFields().empty();

        OENS.PageObjects.InputFormFields().load(resendOrderConfirmationUrl, function () {

            $('#ResendOrderConfirmationButton').on('click', function () {

                labelCheckRemove();

                var orderId = $.trim($('#OrderId').val());

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: resendOrderConfirmationUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ orderId: orderId }),
                    beforeSend: function() {
                        OENS.PageObjects.WaitIndicator().show();
                    }
                }).done(function(result) {

                    if (result.Result === 'Success') {
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        OENS.PageObjects.InputFormFields().append(noOrderScreenMessage);
                    }


                }).fail(function() {
                    
                }).always(function() {
                    OENS.PageObjects.WaitIndicator().hide();
                });
            });
        });
    });

    getAdhocEventsHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFields().empty();

        OENS.PageObjects.InputFormFields().load(sendAdhocEventUrl, function () {
            $('#RegTypesCheckBoxes').hide();
            adhocNotificationForm = $('#AdhocNotificationForm');
            selectedUpcomingWebinarIdDropDown = $('#SelectedWebinarId');

            OENS.PageObjects.SelectedUpcomingWebinarIdDropDown().on('change', function (args) {
                selectedUpcomingWebinarId = $(this).val();
                $('#RegTypesCheckBoxes').show(100);
                
                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendAdhocEventUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: selectedUpcomingWebinarId }),
                    beforeSend: function () {
                        // this is where we append a loading image
                        //pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                    }
                }).done(function (data) {
                    // successful request; do something with the data
                    //pageObjects.RegTypesCheckBoxesDiv().empty();
                    regTypesCheckBoxesDiv = $('#RegTypesCheckBoxes').find('.controls');


                    $.each(data, function (idx, value) {
                        OENS.PageObjects.RegTypesCheckBoxesDiv().append('<label class="checkbox-inline"><input type="checkbox" id="inlineCheckbox_' + idx + '" value="' + value["Value"] + '">' + value["Text"] + '</label>');
                    });

                    OENS.PageObjects.RegTypesCheckBoxesDiv().append('<button id="GetRecipientsButton" class="btn" style="margin-top:10px;">Get Recipients</button>');
                    getRecipientsButton = $('#GetRecipientsButton');

                    getRecipientsButton.on('click', function (evtArgs) {
                        evtArgs.preventDefault();

                        var checkboxes = $("[id^=inlineCheckbox_]");
                        var regTypes = checkboxes.map(function () {
                            if ($(this).is(':checked'))
                                return $(this).val();
                        });

                        var payload = {
                            webinarId: selectedUpcomingWebinarId,
                            regTypeIds: $.makeArray(regTypes)
                        };

                        var url = '/AdhocNotification/WebUsersOfWebinars';

                        $.ajax({
                            type: 'POST',
                            contentType: constants.JsonContentType,
                            cache: false,
                            url: url,
                            dataType: constants.JsonDataType,
                            data: JSON.stringify(payload),
                            beforeSend: function () {
                                // this is where we append a loading image
                                //OENS.PageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                            }
                        }).done(function (emails) {

                            var recipientsEmailAddresses = '';
                            $.each(emails, function (idx, value) {
                                return recipientsEmailAddresses += value + ';';
                            });

                            recipientsEmailAddresses = recipientsEmailAddresses.substring(0, recipientsEmailAddresses.length - 1);

                            OENS.PageObjects.RegTypesCheckBoxesDiv().append('<br /><input type="Text" id="SubjectInput" class="input-xxlarge" style="margin-top:10px;" placeholder="Enter Subject" />');
                            OENS.PageObjects.RegTypesCheckBoxesDiv().append('<input type="Text" id="RecipientsInput" style="width:100%;margin-top:10px;clear:left" value="' + recipientsEmailAddresses + '" />');
                            OENS.PageObjects.RegTypesCheckBoxesDiv().append('<button id="SendNotificationButton" class="btn btn-primary" style="margin-top:10px;">Send Notification</button>');
                        });

                    });

                }).fail(function () {
                    // failed request; give feedback to user

                }).always(function () {

                });
            });
        });
    });

    
    sendShippedOrderNotificationButton.on('click', function(evtArgs) {

        OENS.PageObjects.InputFormFields().empty();
        OENS.PageObjects.InputFormFields().load(sendOrderShippedUrl, function () {

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
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFields().append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    OENS.PageObjects.InputFormFields().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });

    });

    getSendReminderEventHtmlButton.on('click', function(eventArgs) {
        OENS.PageObjects.InputFormFields().empty();

        OENS.PageObjects.InputFormFields().load(sendReminderUrl, function() {

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
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFields().append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    OENS.PageObjects.InputFormFields().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });
    });

    getSendConnectionInfoEventHtmlButton.on('click', function (eventArgs) {

        OENS.PageObjects.InputFormFields().empty();

        OENS.PageObjects.InputFormFields().load(sendConnectionInfoUrl, function () {
        
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
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFields().append(noOrdersScreenMessage);
                    }

                }).fail(function () {

                    labelCheckRemove();

                    OENS.PageObjects.InputFormFields().append(failedScreenMessage);
                }).always(function () {
                    OENS.PageObjects.WaitIndicator().hide();
                });;
            });
        });
    });

    getSendRecordingPostedEventHtmlButton.on('click', function (eventArgs) {
        eventArgs.preventDefault();
        OENS.PageObjects.InputFormFields().empty();

        
        OENS.PageObjects.InputFormFields().load(sendRecordingPostedUrl, function () {
            
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
                        OENS.PageObjects.InputFormFields().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        OENS.PageObjects.InputFormFields().append(noOrdersScreenMessage);
                    }

                }).fail(function () {
                    labelCheckRemove();

                    OENS.PageObjects.InputFormFields().append(failedScreenMessage);
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

    var removeInnerHandlers = function() {
        $('#ResendConnectionInfoButton').off('click', OENS.PageObjects.InputFormFields());
    };
});