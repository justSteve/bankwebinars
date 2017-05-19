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
    generateWeeklyInvoicesEventUrl,
    sendConnectionInfoUrl,
    sendOrderShippedUrl,
    sendRecordingPostedUrl,
    sendReminderUrl,
    successScreenMessage;

//  Create a namespace.
var OENS = {};

$(function () {

    //var toastLogger = new Common.Logger(); // for toast notifications

    getAdhocEventsHtmlButton = $('#GetAdhocEventsHtmlButton');

    failedScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;Event Firing Has Failed</span>';
    noOrdersScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There were no orders for that webinar</span>';
    noOrderScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There is no order which matches that Order Id</span>';
    successScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;The Orders have been sent.</span>';
    sendAdhocEventUrl = '/Admin/SendAdhocEvent';
    generateWeeklyInvoicesEventUrl = '/Admin/GenerateWeeklyInvoicesEvent';
    generateWeeklyInvoices2Url = '/Admin/GenerateWeeklyInvoices2';
    sendConnectionInfoUrl = '/Admin/SendConnectionInfo';
    sendReminderUrl = '/Admin/SendReminder';
    sendRecordingPostedUrl = '/Admin/SendRecordingPosted';
    resendConnectionInfoUrl = '/Admin/ResendConnectionInfo';
    resendOrderConfirmationUrl = '/Admin/ResendOrderConfirmation';
    sendOrderShippedUrl = '/Admin/SendShippedOrder';

    $('#WaitIndicator').hide();
    var labelCheckRemove = function () {
        if ($('#ScreenMessageSpan').length > 0) {
            $('#ScreenMessageSpan').siblings('br').remove();
            $('#ScreenMessageSpan').remove();
        }
    };

    $('#GetResendConnectionInfoHtmlButton').on('click', function (eventArgs) {
        eventArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(resendConnectionInfoUrl, function () {

            $('#ResendConnectionInfoButton').on('click', function () {

                labelCheckRemove();

                var self = this;

                var orderId = $.trim($('#OrderId').val());

                //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                //var logStartOperation = toastLogger.getLogFn('ResendConnectionInfo');
                //logStartOperation("Re-sending ConnectionInfo", null, true);

                var payload = { orderId: orderId };
                //Rollbar.info({ 'oen-#1': { 'payload': payload } });

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: resendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
                    beforeSend: function () {
                        $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                    }
                }).done(function (result) {

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        $('#InputFormFields').append(noOrderScreenMessage);
                    }
                    $('#spinnerLabel').remove();

                    //Rollbar.info({ 'oen-#2': { 'result': result } });

                }).fail(function () {

                }).always(function () {
                    //$('#loadingSpinner').remove();
                });
            });

            $('#getHtmlSpinner').remove();

        });

    });


    $('#GetResendPostEventMaterialHtmlButton').on('click', function (eventArgs) {
        eventArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(sendRecordingPostedUrl, function () {
            //$('#InputFormFields').empty().load(resendConnectionInfoUrl, function () {

            $('#ResendPostEventMaterialButton').on('click', function () {

                labelCheckRemove();

                var self = this;

                var orderId = $.trim($('#OrderId').val());

                var payload = { orderId: orderId };
                //Rollbar.info({ 'oen-#1': { 'payload': payload } });

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: resendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
                    beforeSend: function () {
                        $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                    }
                }).done(function (result) {

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        $('#InputFormFields').append(noOrderScreenMessage);
                    }
                    $('#spinnerLabel').remove();

                    //Rollbar.info({ 'oen-#2': { 'result': result } });

                }).fail(function () {

                }).always(function () {
                    //$('#loadingSpinner').remove();
                });
            });

            $('#getHtmlSpinner').remove();

        });

    });

    $('#GetResendOrderConfirmationHtmlButton').on('click', function (eventArgs) {

        eventArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(resendOrderConfirmationUrl, function () {

            $('#ResendOrderConfirmationButton').on('click', function () {

                labelCheckRemove();

                var orderId = $.trim($('#OrderId').val());

                var self = this;

                //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                //var logStartOperation = toastLogger.getLogFn('ResendOrderConfirmation');
                //logStartOperation("Re-sending Order Confirmation", null, true);

                var payload = { orderId: orderId };
                //Rollbar.info({ 'oen-#3': { 'payload': payload } });

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: resendOrderConfirmationUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
                    beforeSend: function () {
                        $(self).after('<span id="spinnerLabel" class="label label-info" style="margin-left:5px"><span>&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Sending...</span></span>');
                    }
                }).done(function (result) {

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'Fail') {
                        $('#InputFormFields').append(noOrderScreenMessage);
                    }
                    $('#spinnerLabel').remove();
                    //Rollbar.info({ 'oen-#4': { 'result': result }});
                }).fail(function () {
                    //Rollbar.error({ 'oen-#5': { 'fail-result': 'no data' } });
                }).always(function () {
                    //$('#loadingSpinner').remove();
                });
            });

            $('#getHtmlSpinner').remove();
        });


    });

    getAdhocEventsHtmlButton.on('click', function (eventArgs) {

        eventArgs.preventDefault();

        $('#InputFormFields').empty().load(sendAdhocEventUrl, function () {
            $('#RegTypesCheckBoxes').hide();
            adhocNotificationForm = $('#AdhocNotificationForm');
            selectedUpcomingWebinarIdDropDown = $('#SelectedWebinarId');

            selectedUpcomingWebinarIdDropDown.on('change', function (args) {
                selectedUpcomingWebinarId = $(this).val();
                $('#RegTypesCheckBoxes').show(100);

                var payload = { webinarId: selectedUpcomingWebinarId };
                //Rollbar.info({ 'oen-#6': { 'payload': payload } });

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendAdhocEventUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: selectedUpcomingWebinarId }),
                    beforeSend: function () {
                        // this is where we append a loading image
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
                                $('#SubjectInput').remove();
                                $('#RecipientsInput').remove();
                                $('#NotificationBody').remove();
                                $('#SendNotificationButton').remove();
                                regTypesCheckBoxesDiv.find('br').remove();
                            }
                        }).done(function (emails) {

                            var recipientsEmailAddresses = '';
                            $.each(emails, function (idx, value) {
                                return recipientsEmailAddresses += value + ';';
                            });

                            recipientsEmailAddresses = recipientsEmailAddresses.substring(0, recipientsEmailAddresses.length - 1);

                            regTypesCheckBoxesDiv.append('<br /><input type="Text" id="SubjectInput" class="input-xxlarge" style="margin-top:10px;" placeholder="Enter Subject" />');
                            regTypesCheckBoxesDiv.append('<input type="Text" id="RecipientsInput" style="width:100%;margin-top:10px;clear:left" placeholder="Recipient emails, semi-colon separated" value="' + recipientsEmailAddresses + '" />');
                            regTypesCheckBoxesDiv.append('<textarea cols="40" data-val="true" data-val-required="The NotificationBody field is required." id="NotificationBody" name="NotificationBody" rows="2" placeholder="Enter the body of the notification" style="width:100%;margin-top:10px;clear:left"></textarea>');
                            regTypesCheckBoxesDiv.append('<button id="SendNotificationButton" class="btn btn-primary" style="margin-top:10px;">Send Notification</button>');

                            $('#SendNotificationButton').on('click', function (e) {

                                e.preventDefault();

                                var self = this;

                                var form = $('#AdhocNotificationForm');
                                var url = form.attr('action');
                                var payload = {
                                    emails: $('#RecipientsInput').val(),
                                    body: $('#NotificationBody').val(),
                                    subject: $('#SubjectInput').val(),
                                    __RequestVerificationToken: form.find('input[name="__RequestVerificationToken"]').val()
                                }

                                $.ajax({
                                    type: 'POST',
                                    contentType: constants.FormPostContentType,
                                    cache: false,
                                    url: url,
                                    dataType: constants.JsonDataType,
                                    data: payload,
                                    beforeSend: function () {
                                        $(self).append('<span id="sendNotifnSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                                        $('#postFeedbackLabel').remove();
                                    }
                                }).done(function (data, textStatus, jqXHR) {
                                    if (data.Result === 'Success') {
                                        $(self).after('<span id="postFeedbackLabel">&nbsp;<span class="label label-success">&nbsp;Operation succeeded</span></span>');
                                    } else {

                                    }
                                    $('#sendNotifnSpinner').remove();
                                }).fail(function (jqXHR, textStatus, errorThrown) {
                                    $('#sendNotifnSpinner').remove();
                                }).always(function (/* arguments vary if fail or not */) {

                                });


                            });

                            //Rollbar.info({ 'oen-#9': { 'result': emails } });
                        });

                        //Rollbar.info({ 'oen-#8': { 'payload': payload } });// added after the AJAX call so as not to hold it up.
                    });

                    //Rollbar.info({ 'oen-#7': { 'result': data } });
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    // failed request; give feedback to user
                    //Rollbar.error({ 'oen-#14': { 'statusCode': jqXHR && jqXHR.statusCode().status } });
                    //Rollbar.error({ 'oen-#15': { 'errorThrown': errorThrown } });

                }).always(function () {

                });
            });
        });
    });



    $('#SendShippedOrderNotificationButton').on('click', function (evtArgs) {

        evtArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(sendOrderShippedUrl, function () {

            var selectedOrderId = $('#SelectedOrderId');

            $('#GetSendShippedOrderNotificationButton').on('click', function (eventArgs) {

                var payload = selectedOrderId.val();

                if (!payload)
                    $('#EmailOrderButton').after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');

                //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                //var logStartOperation = toastLogger.getLogFn('SendShippedOrderNotification');
                //logStartOperation("Sending shipped order notification", null, true);


                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendOrderShippedUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ orderId: payload }),
                    beforeSend: function () {

                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        $('#InputFormFields').append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        $('#InputFormFields').append(noOrdersScreenMessage);
                    }
                    //Rollbar.info({ 'oen-#11': { 'result': result } });
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    labelCheckRemove();
                    $('#InputFormFields').append(failedScreenMessage);

                    //Rollbar.error({ 'oen-#12': { 'statusCode': jqXHR && jqXHR.statusCode().status } });
                    //Rollbar.error({ 'oen-#13': { 'errorThrown': errorThrown } });
                }).always(function () {
                    //$('#loadingSpinner').remove();

                });;

                //Rollbar.info({ 'oen-#10': { 'payload': payload } });// added after the AJAX call so as not to hold it up.
            });

            $('#PreviewShippedOrderEmailButton').on('click', function (e) {

                e.preventDefault();

                var resultLabel = $('#resultLabel');
                if (resultLabel.length > 0)
                    resultLabel.remove();

                var selectedOrderIdVal = selectedOrderIdVal.val();

                if (!selectedOrderIdVal) {
                    $('#EmailOrderButton').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
                    return;
                }


                $(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                var that = this;

                var url = '/Admin/PreviewShippedOrder/' + selectedOrderIdVal;

                var modalPreview = $('#previewModal'),
                    modalFormOptionsOnPageLoad = {
                        keyboard: true,
                        backdrop: 'static',
                        show: true
                    };

                $.get(url, function (data) {
                    $('#emailContent').html(data);
                    $('#loadingSpinner').remove();

                    modalPreview.modal(modalFormOptionsOnPageLoad);
                });

                //Rollbar.info({ 'oen-#16': { 'selectedOrderId': selectedOrderIdVal } });// added after the AJAX call so as not to hold it up.
            });

            $('#EmailOrderButton').on('click', function (e) {

                e.preventDefault();

                var resultLabel = $('#resultLabel');
                if (resultLabel.length > 0)
                    resultLabel.remove();

                var selectedOrderIdVal = selectedOrderId.val();

                if (!selectedOrderIdVal) {
                    $('#EmailOrderButton').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
                    return;
                }

                $('#emailPanel').fadeIn();

                $('#dispatchButton').on('click', function (e) {

                    e.preventDefault();

                    var self = this;
                    var payload = { emails: $('#EmailAddressesInput').val() };

                    $.ajax({
                        type: 'POST',
                        contentType: constants.JsonContentType,
                        cache: false,
                        url: '/Admin/EmailOrder/' + selectedOrderIdVal,
                        dataType: constants.JsonDataType,
                        data: JSON.stringify(payload),
                        beforeSend: function () {
                            $(self).prepend('<i id="emailSendingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                            $(self).attr('disabled', 'disabled');
                            var resultLabel = $('#resultLabel');
                            var errorText = $('#errorText');
                            if (resultLabel.length > 0)
                                resultLabel.remove();
                            if (errorText.length > 0)
                                errorText.remove();
                        }
                    }).done(function (data) {
                        if (data.Result === 'Success') {
                            $(self).after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');
                        } else if (data.Result === 'Fail') {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;' + data.Msg + ' </span>');
                        } else {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Invalid Data. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                        }

                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');

                        $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Transport error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                    });
                    //Rollbar.info({ 'oen-#17': { 'selectedOrderId': selectedOrderIdVal } });// added after the AJAX call so as not to hold it up.
                    //Rollbar.info({ 'oen-#18': { 'payload': payload } });// added after the AJAX call so as not to hold it up.
                });
            });

            $('#getHtmlSpinner').remove();
        });

    });


    $('#GetSendConnectionInfoEventHtmlButton').on('click', function (eventArgs) {

        eventArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(sendConnectionInfoUrl, function () {
            var webinarsDropdownList = $('#SelectedWebinarId');

            $('#SendConnectionInfoRecipientsButton').on('click', function (eventArgs) {
                //console.log(sendConnectionInfoUrl);
                var webinarsDropdownListVal = webinarsDropdownList.val();
                var payload = { webinarId: webinarsDropdownListVal };

                //$(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                //var logStartOperation = toastLogger.getLogFn('SendConnectionInfo');
                //logStartOperation("Sending ConnectionInfo", null, true);

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
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

                }).fail(function (jqXHR, textStatus, errorThrown) {

                    labelCheckRemove();

                    $('#InputFormFields').append(failedScreenMessage);
                    //Rollbar.error({ 'oen-#23': { 'statusCode': jqXHR && jqXHR.statusCode().status } });
                    //Rollbar.error({ 'oen-#24': { 'errorThrown': errorThrown } });

                }).always(function () {
                    //$('#loadingSpinner').remove();
                });;

                //Rollbar.info({ 'oen-#20': { 'payload': payload } });// added after the AJAX call so as not to hold it up.

            });

            //$('#PreviewSendConnectionInfoEmailButton').on('click', function (e) {

            //    e.preventDefault();

            //    var resultLabel = $('#resultLabel');
            //    if (resultLabel.length > 0)
            //        resultLabel.remove();

            //    var webinar = webinarsDropdownList.val();

            //    if (!webinar) {
            //        $('#EmailConnectionInfo').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
            //        return;
            //    }


            //    $(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

            //    var url = '/Admin/PreviewConnectionInfo/' + webinar;

            //    var modalPreview = $('#previewModal'),
            //        modalFormOptionsOnPageLoad = {
            //            keyboard: true,
            //            backdrop: 'static',
            //            show: true
            //        };

            //    $.get(url, function (data) {
            //        $('#emailContent').html(data);
            //        $('#loadingSpinner').remove();

            //        modalPreview.modal(modalFormOptionsOnPageLoad);
            //    });

            //    //Rollbar.info({ 'oen-#24': { 'webinar': webinar } });// added after the AJAX call so as not to hold it up.

            //});

            $('#EmailConnectionInfo').on('click', function (e) {

                e.preventDefault();

                var resultLabel = $('#resultLabel');
                if (resultLabel.length > 0)
                    resultLabel.remove();

                var webinar = webinarsDropdownList.val();

                if (!webinar) {
                    $('#EmailConnectionInfo').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
                    return;
                }

                $('#emailPanel').fadeIn();

                $('#dispatchButton').on('click', function (e) {

                    e.preventDefault();

                    var self = this;
                    var payload = { emails: $('#EmailAddressesInput').val() };

                    $.ajax({
                        type: 'POST',
                        contentType: constants.JsonContentType,
                        cache: false,
                        url: '/Admin/EmailOrderConnectionInfo/' + webinar,
                        dataType: constants.JsonDataType,
                        data: JSON.stringify(payload),
                        beforeSend: function () {
                            $(self).prepend('<i id="emailSendingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                            $(self).attr('disabled', 'disabled');
                            var resultLabel = $('#resultLabel');
                            var errorText = $('#errorText');
                            if (resultLabel.length > 0)
                                resultLabel.remove();
                            if (errorText.length > 0)
                                errorText.remove();
                        }
                    }).done(function (data) {
                        if (data.Result === 'Success') {
                            $(self).after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');
                        } else if (data.Result === 'Fail') {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;' + data.Msg + ' </span>');
                        } else {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Invalid Data. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                        }

                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');

                        $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Transport error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                    });
                    //Rollbar.info({ 'oen-#26': { 'webinar': webinar } });// added after the AJAX call so as not to hold it up.
                    //Rollbar.info({ 'oen-#27': { 'payload': payload } });
                });
            });

            $('#getHtmlSpinner').remove();

        });
    });

    $('#GetSendRecordingPostedEventHtmlButton').on('click', function (eventArgs) {

        eventArgs.preventDefault();

        $('#OrdersMenuHeader').after('<i id="getHtmlSpinner" class="icon-spinner icon-spin"></i>');

        $('#InputFormFields').empty().load(sendRecordingPostedUrl, function () {

            var webinarsDropdownList = $('#SelectedWebinarId');

            $('#SendRecordingPostedButton').on('click', function (eventArgs) {

                var webinarsDropdownListVal = webinarsDropdownList.val();
                var payload = { webinarId: webinarsDropdownListVal };

                $(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendRecordingPostedUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(payload),
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


                }).fail(function (jqXHR, textStatus, errorThrown) {
                    labelCheckRemove();

                    $('#InputFormFields').append(failedScreenMessage);
                    //Rollbar.error({ 'oen-#30': { 'statusCode': jqXHR && jqXHR.statusCode().status } });// added after the AJAX call so as not to hold it up.
                    //Rollbar.error({ 'oen-#31': { 'errorThrown': errorThrown } });

                }).always(function () {
                    $('#loadingSpinner').hide();
                });

            });

            $('#PreviewSendRecordingPostedButton').on('click', function (e) {

                e.preventDefault();

                var resultLabel = $('#resultLabel');
                if (resultLabel.length > 0)
                    resultLabel.remove();

                var webinar = webinarsDropdownList.val();

                if (!webinar) {
                    $('#EmailPostedRecording').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
                    return;
                }

                $(this).prepend('<i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');

                var url = '/Admin/PreviewRecordingPosted/' + webinar;

                var modalPreview = $('#previewModal'),
                    modalFormOptionsOnPageLoad = {
                        keyboard: true,
                        backdrop: 'static',
                        show: true
                    };

                $.get(url, function (data) {
                    $('#emailContent').html(data);
                    $('#loadingSpinner').remove();

                    modalPreview.modal(modalFormOptionsOnPageLoad);
                });
                //Rollbar.info({ 'oen-#32': { 'webinar': webinar } });// added after the AJAX call so as not to hold it up.
            });

            $('#EmailPostedRecording').on('click', function (e) {

                e.preventDefault();

                var resultLabel = $('#resultLabel');
                if (resultLabel.length > 0)
                    resultLabel.remove();

                var webinar = webinarsDropdownList.val();

                if (!webinar) {
                    $('#EmailPostedRecording').after('<span id="resultLabel" class="label label-important" style="margin-left:5px"><i class="icon icon-exclamation-sign"></i>&nbsp;You need to select an Order from the Dropdown List.</span>');
                    return;
                }

                $('#emailPanel').fadeIn();

                $('#dispatchButton').on('click', function (e) {

                    e.preventDefault();

                    var self = this;
                    var payload = { emails: $('#EmailAddressesInput').val() };

                    $.ajax({
                        type: 'POST',
                        contentType: constants.JsonContentType,
                        cache: false,
                        url: '/Admin/EmailRecordingPosted/' + webinar,
                        dataType: constants.JsonDataType,
                        data: JSON.stringify(payload),
                        beforeSend: function () {
                            $(self).prepend('<i id="emailSendingSpinner" class="icon-spinner icon-spin"></i>&nbsp;');
                            $(self).attr('disabled', 'disabled');
                            var resultLabel = $('#resultLabel');
                            var errorText = $('#errorText');
                            if (resultLabel.length > 0)
                                resultLabel.remove();
                            if (errorText.length > 0)
                                errorText.remove();
                        }
                    }).done(function (data) {
                        if (data.Result === 'Success') {
                            $(self).after('<span id="resultLabel" class="label label-success" style="margin-left:5px">&nbsp;Email sent</span>');
                        } else if (data.Result === 'Fail') {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;' + data.Msg + ' </span>');
                        } else {
                            $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Invalid Data. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                        }

                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');
                        //Rollbar.info({ 'oen-#35': { 'result': data } });
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        $('#emailSendingSpinner').remove();
                        $(self).removeAttr('disabled');

                        $(self).after('<span id="errorText" class="field-validation-error"><i class="icon icon-exclamation-sign"></i>&nbsp;Transport error. Try again or use our Help & Feedback button (lower right corner)  for immediate assistance! </span>');
                        //Rollbar.error({ 'oen-#36': { 'statusCode': jqXHR && jqXHR.statusCode().status } });
                        //Rollbar.error({ 'oen-#37': { 'errorThrown': errorThrown } });
                    });

                    //Rollbar.info({ 'oen-#33': { 'webinar': webinar } });// added after the AJAX call so as not to hold it up.
                    //Rollbar.info({ 'oen-#34': { 'payload': payload } });
                });
            });

            $('#getHtmlSpinner').remove();
        });

    });


});


function FireGenerator() { // used recursively!!!

    console.log("Starting: " + callsComplete);

    // can't use beforeSend as async: false blocks UI
    crunchingLabel.html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Processing ' + (callsComplete + 1) + ' of ' + callsNeeded + '</span>');

    var payload = { startDate: $("#startDate").val(), _idAffiliate: arryAff[callsComplete] };

    window.setTimeout(function () { // "window" seems to be required...
        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            async: false,
            // timeout: 3000, // I don't think this is a good idea, maybe though...
            url: "/Admin/GenerateWeeklyInvoices2?startDate=" + payload.startDate + "&_idAffiliate=" + payload._idAffiliate,
            //url: "/Admin/GenerateWeeklyInvoicesEvent?startDate=" + payload.startDate + "&_idAffiliate=" + payload._idAffiliate,
            dataType: constants.JsonDataType,
            beforeSend: function () {
                //crunchingLabel.html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Processing ' + (callsComplete + 1) + ' of ' + arryAff.length + '</span>');
            }
        }).done(function (data) {
            if (data.InvoicesFound) {

                var parsedInvoice = 0;
                if (data.InvoiceId) {
                    parsedInvoice = data.InvoiceId;
                }
                // dynamically create / add checkboxes with the information returned
                sendRptCheckBoxesDiv.append('<p class="checkbox-inline">' +
                    '<label style="display:inline"><input type="checkbox" checked="Checked" ' +
                    'id="inlineCheckbox_' +
                    parsedInvoice +
                    '" ' +
                    'data-affid="' +
                    data.AffiliateId +
                    '" ' +
                    'data-invoiceid="' +
                    data.InvoiceId +
                    '" ' +
                    'data-invoiceurl="' +
                    data.Link.PrimaryUri +
                    '" ' +
                    'data-affiliatecontactemail="' +
                    data.AffiliateContactEmail +
                    '" ' +
                    'data-daterange="' +
                    data.DateRange +
                    '" ' +
                    'value="' +
                    data.AffiliateLabel +
                    '">' +
                    data.AffiliateLabel +
                    '</label> ' +
                    '<a href="' +
                    data.Link.PrimaryUri +
                    '" _target=_new>View Report</a></p>');
            } else {
                if (data
                    .OrdersFound) {

                    // successful request; do something with the data
                    // console.log(data);

                    var parsedInvoice = 0;
                    if (data.InvoiceId) {
                        parsedInvoice = data.InvoiceId;
                    }

                    // dynamically create / add checkboxes with the information returned
                    sendRptCheckBoxesDiv.append('<p class="checkbox-inline">' +
                        '<label style="display:inline"><input type="checkbox" checked="Checked" ' +
                        'id="inlineCheckbox_' + parsedInvoice + '" ' +
                        'data-affid="' + data.AffiliateId + '" ' +
                        'data-invoiceid="' + data.InvoiceId + '" ' +
                        'data-invoiceurl="' + data.Link.PrimaryUri + '" ' +
                        'data-affiliatecontactemail="' + data.AffiliateContactEmail + '" ' +
                        'data-daterange="' + data.DateRange + '" ' +
                        'value="' + data.AffiliateLabel + '">' +
                        data.AffiliateLabel + '</label> ' + '<a href="' + data.Link.PrimaryUri + '" _target=_new>View Report</a></p>');

                } else {

                    console.log("NO INVOICE DATA: " + payload._idAffiliate);
                    sendRptCheckBoxesDiv.append('<p>No orders found for ' + data.AffiliateLabel + '</p>');
                }
            }
            console.log("Done: " + callsComplete);


        }).fail(function (jqXHR, textStatus, errorThrown) {

            // failed request; give feedback to user
            alert("Problem with " + payload._idAffiliate + " (iteration " + callsComplete + ") -- " + errorThrown);

            console.log("Error on iteration " + callsComplete);

        }).always(function () {
            callsComplete++;
            // figure out if this was the "last one" so we can clean up the UI, report errors, etc.
            if (callsComplete >= callsNeeded) {
                crunchingLabel.html('Processing complete')
                setTimeout(function () { crunchingLabel.html('&nbsp;') }, 2000);
                $('#sendRpt').show();
            } else {
                FireGenerator(); // Call ourself
            }
        });
    }, 1);
}

function SendInvoices() {

    var payload = [];

    // add each of the checked invoices to the payload for posting to controller
    $("#SendRptCheckBoxes input[id^='inlineCheckbox_']:checked").each(function (idx, item) {
        var $item = $(item);
        payload[idx] = { "Affiliate.idUserAff": $item.data("affid"), "Affiliate.ContactEmail": $item.data("affiliatecontactemail"), "InvoiceId": $item.data("invoiceid"), "InvoiceStorageUri": $item.data("invoiceurl"), "DateRange": $item.data("daterange") };
    });

    console.log(payload);

    if (payload.length == 0)
        return;

    // post the list to the action / controller
    $.ajax({
        type: 'POST',
        contentType: constants.JsonContentType,
        cache: false,
        url: "/Admin/SendWeeklyInvoicesEvent",
        dataType: constants.JsonDataType,
        data: JSON.stringify(payload),
        beforeSend: function () {
            crunchingLabel.html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Processing ' + payload.length + ' notifications...</span>');
        }
    }).done(function (data) {

        console.log(data);
        crunchingLabel.html('Processing complete.  ' + data.EmailsQueued + ' emails queued.');
        setTimeout(function () { crunchingLabel.html('&nbsp;') }, 2000);

    }).fail(function (jqXHR, textStatus, errorThrown) {
        // failed request; give feedback to user
        alert("problem encountered -- " + errorThrown);
        crunchingLabel.html('');
    });
}