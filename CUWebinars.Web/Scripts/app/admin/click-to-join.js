/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var CLICKTOJOIN = {};

var CM = CLICKTOJOIN;

$(function() {

    CM.emailInput = $('#Email');
    CM.genClickToJoinCodeButton = $('#genClickToJoinCodeButton');
    CM.clickToJoinForm = $('#clickToJoinForm');


    CM.clickToJoinForm.on('submit', function (e) {

        e.preventDefault();

        var self = this;
        
        var url = $(self).attr('action');


        var tabInputs = formProcessor.getApplicableInputs('clickToJoinForm');
        var payload = formProcessor.processInputs(tabInputs);

        var token = $(self).find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            headers: headers,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                CM.genClickToJoinCodeButton.append('<span id="extendTimeSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                $('#accessCode').hide();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                $('#accessCode').text(data.Code).fadeIn(1000);
            } else {

            }

            $('#extendTimeSpinner').remove();
        });


    });




});
