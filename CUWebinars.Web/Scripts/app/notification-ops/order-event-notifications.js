var adhocNotificationForm,
    failedScreenMessage,
    getAdhocEventsHtmlButton,
    noOrderScreenMessage,
    noOrdersScreenMessage,
    getRecipientsButton,
    resendConnectionInfoUrl,
    resendOrderConfirmationUrl,
    selectedUpcomingWebinarId,
    sendAdhocEventUrl,
    sendConnectionInfoUrl,
    sendOrderShippedUrl,
    sendRecordingPostedUrl,
    sendReminderUrl,
    successScreenMessage;

//  Create a namespace.
var OENS = {};

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

    $('#WaitIndicator').hide();


    $('#GetResendConnectionInfoHtmlButton').on('click', function (eventArgs) {
        eventArgs.preventDefault();
        $('#InputFormFields').empty();

        $('#InputFormFields').load(resendConnectionInfoUrl, function () {

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
                        $('#WaitIndicator').show();
                    }
                }).done(function (result) {

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        $('#InputFormFields').append(noOrderScreenMessage);
                    }

                }).fail(function () {

                }).always(function () {
                    $('#WaitIndicator').hide();
                });
            });

        });
    });
            
    $('#GetResendOrderConfirmationHtmlButton').on('click', function (eventArgs) {
        eventArgs.preventDefault();
        $('#InputFormFields').empty();

        $('#InputFormFields').load(resendOrderConfirmationUrl, function () {

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
                        $('#WaitIndicator').show();
                    }
                }).done(function(result) {

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        $('#InputFormFields').append(noOrderScreenMessage);
                    }


                }).fail(function() {
                    
                }).always(function() {
                    $('#WaitIndicator').hide();
                });
            });
        });
    });

    getAdhocEventsHtmlButton.on('click', function(eventArgs) {
        $('#InputFormFields').empty();

        $('#InputFormFields').load(sendAdhocEventUrl, function () {
            $('#RegTypesCheckBoxes').hide();
            adhocNotificationForm = $('#AdhocNotificationForm');
            selectedUpcomingWebinarIdDropDown = $('#SelectedWebinarId');

            selectedUpcomingWebinarIdDropDown.on('change', function (args) {
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
                    regTypesCheckBoxesDiv = $('#RegTypesCheckBoxes').find('.controls');


                    $.each(data, function (idx, value) {
                        regTypesCheckBoxesDiv.append('<label class="checkbox-inline"><input type="checkbox" id="inlineCheckbox_' + idx + '" value="' + value["Value"] + '">' + value["Text"] + '</label>');
                    });

                    regTypesCheckBoxesDiv.append('<button id="GetRecipientsButton" class="btn" style="margin-top:10px;">Get Recipients</button>');
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

                            regTypesCheckBoxesDiv.append('<br /><input type="Text" id="SubjectInput" class="input-xxlarge" style="margin-top:10px;" placeholder="Enter Subject" />');
                            regTypesCheckBoxesDiv.append('<input type="Text" id="RecipientsInput" style="width:100%;margin-top:10px;clear:left" value="' + recipientsEmailAddresses + '" />');
                            regTypesCheckBoxesDiv.append('<button id="SendNotificationButton" class="btn btn-primary" style="margin-top:10px;">Send Notification</button>');
                        });

                    });

                }).fail(function () {
                    // failed request; give feedback to user

                }).always(function () {

                });
            });
        });
    });

    
    $('#SendShippedOrderNotificationButton').on('click', function (evtArgs) {
        
        $('#InputFormFields').empty().load(sendOrderShippedUrl, function () {

            var selectedOrderId = $('#SelectedOrderId'); 

            $('#GetSendShippedOrderNotificationButton').on('click', function (eventArgs) {

                var payload = selectedOrderId.val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendOrderShippedUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ orderId: payload }),
                    beforeSend: function () {
                        $('#WaitIndicator').show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        $('#InputFormFields').append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    $('#InputFormFields').append(failedScreenMessage);
                }).always(function () {
                    $('#WaitIndicator').hide();
                });;
            });

            $('#PreviewShippedOrderEmailButton').on('click', function (e) {
                e.preventDefault();

                var url = '/Admin/PreviewShippedOrder/' + selectedOrderId.val();

                var modalPreview = $('#previewModal'),
                    modalFormOptionsOnPageLoad = {
                        keyboard: true,
                        backdrop: 'static',
                        show: true,
                    };
                
                $.get(url, function (data) {
                    $('#emailContent').html(data);

                    modalPreview.modal(modalFormOptionsOnPageLoad);
                    modalPreview.modal();

                });

            });

        });

    });

    $('#GetSendReminderEventHtmlButton').on('click', function (eventArgs) {
        $('#InputFormFields').empty();

        $('#InputFormFields').load(sendReminderUrl, function() {

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
                        $('#WaitIndicator').show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        $('#InputFormFields').append(noOrdersScreenMessage);
                    }
                }).fail(function () {
                    labelCheckRemove();
                    $('#InputFormFields').append(failedScreenMessage);
                }).always(function () {
                    $('#WaitIndicator').hide();
                });;
            });
        });
    });

    $('#GetSendConnectionInfoEventHtmlButton').on('click', function (eventArgs) {
        
        $('#InputFormFields').empty().load(sendConnectionInfoUrl, function () {
        
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
                        $('#WaitIndicator').show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        $('#InputFormFields').append(noOrdersScreenMessage);
                    }

                }).fail(function () {

                    labelCheckRemove();

                    $('#InputFormFields').append(failedScreenMessage);
                }).always(function () {
                    $('#WaitIndicator').hide();
                });;
            });
        });
    });

    $('#GetSendRecordingPostedEventHtmlButton').on('click', function (eventArgs) {
        eventArgs.preventDefault();
        $('#InputFormFields').empty();

        
        $('#InputFormFields').load(sendRecordingPostedUrl, function () {
            
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
                        $('#WaitIndicator').show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        $('#InputFormFields').append(noOrdersScreenMessage);
                    }

                }).fail(function () {
                    labelCheckRemove();

                    $('#InputFormFields').append(failedScreenMessage);
                }).always(function () {
                    $('#WaitIndicator').hide();
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
        $('#ResendConnectionInfoButton').off('click', $('#InputFormFields'));
    };
});