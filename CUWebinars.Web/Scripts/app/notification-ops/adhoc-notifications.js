var adhocNotificationForm,
    getRecipientsButton,
    regTypesCheckBoxesDiv,
    selectedUpcomingWebinarId,
    selectedUpcomingWebinarIdDropDown;

var pageObjects = {
    AdhocNotificationForm: function () {
        return adhocNotificationForm || $('#AdhocNotificationForm');
    },
    GetRecipientsButton: function () {
        return getRecipientsButton || $('#GetRecipientsButton');
    },
    RegTypesCheckBoxesDiv: function () {
        return regTypesCheckBoxesDiv || $('#RegTypesCheckBoxes').find('.controls');
    },
    SelectedUpcomingWebinarIdDropDown: function () {
        return selectedUpcomingWebinarIdDropDown || $('#SelectedUpcomingWebinarId');
    }
};

$(function () {

    adhocNotificationForm = $('#AdhocNotificationForm');
    regTypesCheckBoxesDiv = $('#RegTypesCheckBoxes').find('.controls');
    selectedUpcomingWebinarIdDropDown = $('#SelectedUpcomingWebinarId');

    $('#RegTypesCheckBoxes').hide();

    pageObjects.SelectedUpcomingWebinarIdDropDown().on('change', function (args) {
        selectedUpcomingWebinarId = $(this).val();
        $('#RegTypesCheckBoxes').show(100);

        var url = '/AdhocNotification/Index';

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify({ webinarId: selectedUpcomingWebinarId }),
            beforeSend: function () {
                // this is where we append a loading image
                //pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
            }
        }).done(function (data) {
            // successful request; do something with the data
            pageObjects.RegTypesCheckBoxesDiv().empty();

            $.each(data, function(idx, value) {
                pageObjects.RegTypesCheckBoxesDiv().append('<label class="checkbox-inline"><input type="checkbox" id="inlineCheckbox_' + idx + '" value="' + value["Value"] + '">' + value["Text"] + '</label>');
            });

            pageObjects.RegTypesCheckBoxesDiv().append('<button id="GetRecipientsButton" class="btn" style="margin-top:10px;">Get Recipients</button>');
            getRecipientsButton = $('#GetRecipientsButton');

            getRecipientsButton.on('click', function (evtArgs) {
                evtArgs.preventDefault();

                var checkboxes = $("[id^=inlineCheckbox_]");
                var regTypes = checkboxes.map(function () {
                    if($(this).is(':checked'))
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
                    beforeSend: function() {
                        // this is where we append a loading image
                        //pageObjects.labelEmail().html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Checking that Email...</span>');
                    }
                }).done(function (emails) {

                    var recipientsEmailAddresses = '';
                        $.each(emails, function (idx, value) {
                            return recipientsEmailAddresses += value + ';';
                    });

                    recipientsEmailAddresses = recipientsEmailAddresses.substring(0, recipientsEmailAddresses.length - 1);

                    pageObjects.RegTypesCheckBoxesDiv().append('<br /><input type="Text" id="SubjectInput" class="input-xxlarge" style="margin-top:10px;" placeholder="Enter Subject" />');
                    pageObjects.RegTypesCheckBoxesDiv().append('<input type="Text" id="RecipientsInput" style="width:100%;margin-top:10px;clear:left" value="' + recipientsEmailAddresses + '" />');
                    pageObjects.RegTypesCheckBoxesDiv().append('<button id="SendNotificationButton" class="btn btn-primary" style="margin-top:10px;">Send Notification</button>');
                });

            });

        }).fail(function () {
            // failed request; give feedback to user
            
        }).always(function () {
            
        });
    });

    
});

