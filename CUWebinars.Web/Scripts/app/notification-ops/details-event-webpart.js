var buttonsContainer,
    inputFormFieldsDiv,
    failedScreenMessage,
    getSendConnectionInfoEventHtmlButton,
    getSendRecordingPostedEventHtmlButton,
    noOrdersScreenMessage,
    getRecipientsButton,
    resetButton,
    selectedUpcomingWebinarId,
    selectedUpcomingWebinarIdDropDown,
    sendConnectionInfoUrl,
    sendRecordingPostedUrl,
    successScreenMessage,
    waitIndicator;

//  Create a namespace. PageObjects is getting polluted across js files.
var DEW = {
    PageObjects: {
        ButtonsContainer: function () {
            return buttonsContainer || $('#ButtonsContainer');
        },
        GetSendConnectionInfoEventHtmlButton: function () {
            return getSendConnectionInfoEventHtmlButton || $('#GetSendConnectionInfoEventHtmlButton');
        },
        GetSendRecordingPostedEventHtmlButton: function () {
            return getSendRecordingPostedEventHtmlButton || $('#GetSendRecordingPostedEventHtmlButton');
        },
        InputFormFieldsDiv: function() {
            return inputFormFieldsDiv || $('#InputFormFieldsDiv');
        },
        ResetButton: function () {
            return resetButton || $('#ResetButton');
        },
        SelectedUpcomingWebinarIdDropDown: function () {
            return selectedUpcomingWebinarIdDropDown || $('#SelectedWebinarId');
        },
        WaitIndicator: function () {
            return waitIndicator || $('#WaitIndicator');
        }
    }
};

$(function () {

    inputFormFieldsDiv = $('#InputFormFieldsDiv');
    inputFormFieldsDiv.hide();
    buttonsContainer = $('#ButtonsContainer');

    failedScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;Event Firing Has Failed</span>';
    noOrdersScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-information">&nbsp;&nbsp;There were no orders for that webinar</span>';
    successScreenMessage = '<br /><span id="ScreenMessageSpan" class="label label-success">&nbsp;The Orders have been sent.</span>';
    sendConnectionInfoUrl = '/Webinar/SendConnectionInfo';
    sendRecordingPostedUrl = '/OrderEventFiringOps/SendRecordingPosted';

    getSendConnectionInfoEventHtmlButton = $('#GetSendConnectionInfoEventHtmlButton');
    getSendRecordingPostedEventHtmlButton = $('#GetSendRecordingPostedEventHtmlButton');
    waitIndicator = $('#WaitIndicator');
    waitIndicator.hide();

    getSendConnectionInfoEventHtmlButton.on('click', function(eventArgs) {
        DEW.PageObjects.InputFormFieldsDiv().empty();
        DEW.PageObjects.ButtonsContainer().fadeOut(500);

        DEW.PageObjects.InputFormFieldsDiv().fadeIn(500);

        DEW.PageObjects.InputFormFieldsDiv().load(sendConnectionInfoUrl, function () {

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
                        DEW.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        DEW.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        DEW.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }

                }).fail(function () {

                    labelCheckRemove();

                    DEW.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    DEW.PageObjects.WaitIndicator().hide();
                    DEW.PageObjects.ResetButton().show();
                });;
            });

            DEW.PageObjects.ResetButton().on('click', function (eventArgs) {
                DEW.PageObjects.ButtonsContainer().fadeIn(500);
                DEW.PageObjects.InputFormFieldsDiv().fadeOut(500);
            });

        });
    });

    getSendRecordingPostedEventHtmlButton.on('click', function (eventArgs) {
        eventArgs.preventDefault();
        DEW.PageObjects.InputFormFieldsDiv().empty();

        
        DEW.PageObjects.InputFormFieldsDiv().load(sendRecordingPostedUrl, function () {
            
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
                        DEW.PageObjects.WaitIndicator().show();
                    }
                }).done(function (result) {

                    labelCheckRemove();

                    if (result.Result === 'Success') {
                        DEW.PageObjects.InputFormFieldsDiv().append(successScreenMessage);
                    } else if (result.Result === 'No Orders to send for that webinar') {
                        DEW.PageObjects.InputFormFieldsDiv().append(noOrdersScreenMessage);
                    }

                }).fail(function () {
                    labelCheckRemove();

                    DEW.PageObjects.InputFormFieldsDiv().append(failedScreenMessage);
                }).always(function () {
                    DEW.PageObjects.WaitIndicator().hide();
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