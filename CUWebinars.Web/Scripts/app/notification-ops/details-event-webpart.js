var buttonsContainer,
    inputFormFieldsDiv,
    failedScreenMessage,
    getSendConnectionInfoEventHtmlButton,
    sendRecordingPostedEventHtmlButton,
    noOrdersScreenMessage,
    getRecipientsButton,
    resetButton,
    selectedUpcomingWebinarId,
    selectedUpcomingWebinarIdDropDown,
    sendConnectionInfoUrl,
    sendRecordingPostedUrl,
    successScreenMessage,
    waitIndicator,
    webinarFileInput;

$(function () {

    inputFormFieldsDiv = $('#InputFormFieldsDiv');
    inputFormFieldsDiv.hide();
    buttonsContainer = $('#ButtonsContainer');

    failedScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;Event Firing Has Failed</span>';
    noOrdersScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There were no orders for that webinar</span>';
    successScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;The Orders have been sent.</span>';
    sendConnectionInfoUrl = '/Webinar/SendConnectionInfo';
    sendRecordingPostedUrl = '/Webinar/SendRecordingPosted';

    getSendConnectionInfoEventHtmlButton = $('#GetSendConnectionInfoEventHtmlButton');
    sendRecordingPostedEventHtmlButton = $('#SendRecordingPostedEventHtmlButton');
    webinarFileInput = $('#WebinarFileInput');
    waitIndicator = $('#WaitIndicator');
    waitIndicator.hide();

    getSendConnectionInfoEventHtmlButton.on('click', function (eventArgs) {
        alert("hit");
        inputFormFieldsDiv.empty();
        buttonsContainer.fadeOut(500, function() {
            inputFormFieldsDiv.fadeIn(500);
        });

        inputFormFieldsDiv.load(sendConnectionInfoUrl, function () {

            resetButton = $('#ResetButton');
            resetButton.hide();

            $('#FireSendConnInfoButton').on('click', function (eventArgs) {
                
                var payload = $('#SelectedWebinarId').val();

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: sendConnectionInfoUrl,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify({ webinarId: payload }),
                    beforeSend: function () {
                        waitIndicator.show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        inputFormFieldsDiv.append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        inputFormFieldsDiv.append(noOrdersScreenMessage);
                    }

                }).fail(function () {

                    labelCheckRemove();

                    inputFormFieldsDiv.append(failedScreenMessage);
                }).always(function () {
                    waitIndicator.hide();
                    resetButton.show();
                });;
            });

            resetButton.on('click', function (eventArgs) {
                inputFormFieldsDiv.fadeOut(500, resetEventFirePanel);
            });

        });
    });

    sendRecordingPostedEventHtmlButton.on('click', function (eventArgs) {
        eventArgs.preventDefault();

        var webinarFileName = webinarFileInput().val();

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: sendRecordingPostedUrl,
            dataType: constants.JsonDataType,
            data: JSON.stringify({ webinarId: EvtWebPart.RecordedWebinar, fileName: webinarFileName }),
            beforeSend: function () {
                waitIndicator.show();
            }
        }).done(function (result) {

            labelCheckRemove();

            if (result.Result === 'Success') {
                inputFormFieldsDiv.append(successScreenMessage);
            } else if (result.Result === 'No Orders to send for that webinar') {
                inputFormFieldsDiv.append(noOrdersScreenMessage);
            }

        }).fail(function () {
            labelCheckRemove();

            inputFormFieldsDiv.append(failedScreenMessage);
        }).always(function () {
            waitIndicator.hide();
        });        
        //inputFormFieldsDiv.load(sendRecordingPostedUrl, function () {
            
        //    $('#FireSendRecordedWebinarButton').on('click', function (eventArgs) {
                
        //        resetButton = $('#ResetButton');
        //        resetButton.hide();

        //        var payload = $('#SelectedWebinarId').val();

        //        $.ajax({
        //            type: 'POST',
        //            contentType: constants.JsonContentType,
        //            cache: false,
        //            url: sendRecordingPostedUrl,
        //            dataType: constants.JsonDataType,
        //            data: JSON.stringify({ webinarId: payload }),
        //            beforeSend: function () {
        //                waitIndicator.show();
        //            }
        //        }).done(function (result) {

        //            labelCheckRemove();

        //            if (result.Result === 'Success') {
        //                inputFormFieldsDiv.append(successScreenMessage);
        //            } else if (result.Result === 'No Orders to send for that webinar') {
        //                inputFormFieldsDiv.append(noOrdersScreenMessage);
        //            }

        //        }).fail(function () {
        //            labelCheckRemove();

        //            inputFormFieldsDiv.append(failedScreenMessage);
        //        }).always(function () {
        //            waitIndicator.hide();
        //            resetButton.show();

        //        });
        //    });

        //    resetButton.on('click', function (eventArgs) {
        //        inputFormFieldsDiv.fadeOut(500, resetEventFirePanel);
        //    });
        //});
    });

    var labelCheckRemove = function() {
        if ($('#ScreenMessageSpan').length > 0) {
            $('#ScreenMessageSpan').siblings('br').remove();
            $('#ScreenMessageSpan').remove();
        }
    };

    var resetEventFirePanel = function() {
        buttonsContainer.fadeIn(500);
    };
});