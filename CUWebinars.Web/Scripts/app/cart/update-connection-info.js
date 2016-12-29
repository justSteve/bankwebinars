var UCI = {}; // create namespace to prevent collisions. UCI is 'Update Connection Info'

var addFilesButton,
    webinarFilesSubmitButton,
    manageFilesWrapper,
    breakSuffix,
    deleteItem,
    locationsSpanPrefix,
    numberOfWebinarFiles;


$(function () {
    UCI.primeDomVariables();
    UCI.wireUpHandlersForUpdateConnectionInfoModal();

    var modalFormOptionsOnPageLoad = {
        keyboard: true,
        backdrop: 'static',
        show: true
    };

    UCI.updateConnectionInfoModalButton.on('click', function (e) {
        e.preventDefault();
        clearValidationSummary();
        $('#result').remove();
        UCI.updateConnectionInfoModal.modal(modalFormOptionsOnPageLoad);
    });

    UCI.updateConnectionInfoModal.on('shown', function () {

    });
});

(function (ns) {

    ns.primeDomVariables = function () {
        ns.updateConnectionInfoModalButton = $('#UpdateConnectionInfoModalButton');
        ns.createCitrixWebinar = $('#CreateCitrixWebinar');
        ns.updateConnectionInfoModal = $('#UpdateConnectionInfoModal');
        ns.updateConnectionInfoForm = $('#_UpdateConnectionInfo');
    };

    ns.wireUpHandlersForUpdateConnectionInfoModal = function () {

        $('#saveDetailsButton').on('click', function (e) {

            e.preventDefault();

            var self = this;

            var inputs = formProcessor.getApplicableInputs('_UpdateConnectionInfo');
            var payload = formProcessor.processInputs(inputs);

            //Rollbar.info({ 'uci-#1': { 'payload': payload}});

            var token = UCI.updateConnectionInfoForm.find('input[name=__RequestVerificationToken]').val();
            var headers = {};
            headers['__RequestVerificationToken'] = token;

            var updateConnInfoValSummary = $('#updateConnInfoValSummary');

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: UCI.updateConnectionInfoForm.attr('action'),
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                headers: headers,
                beforeSend: function () {
                    $('#result').remove();

                    formProcessor.clearValidationSummary(updateConnInfoValSummary);

                    $(self).append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (data) {
                if (data.Result == "Success") {
                    var label = $('<div id="result" class="label label-success pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details Updated</div>');
                    label.hide().insertAfter($(self)).fadeIn(500);
                    //Rollbar.info({ 'uci-#2': { 'result': data } });
                } else if (!data.isSuccessful) {
                    formProcessor.lightUpValidationSummary('updateConnInfoValSummary', data);
                    //Rollbar.info({ 'uci-#3': { 'fail-result': data } });
                }
            }).always(function (data) {
                $('#waitSpinner').remove();
            });

        });


        $('#CreateCitrixWebinar').on('click', function (e) {

            var idWebinar = $("#idWebinar").val() + "";

            e.preventDefault();

            var self = this;

            var promptForKey = prompt("Enter WebinarKey from Citrix");

            if (promptForKey != null) {

                var token = UCI.updateConnectionInfoForm.find('input[name=__RequestVerificationToken]').val();
                var headers = {};
                headers['__RequestVerificationToken'] = token;

                var createCitrixWebinarSummary = $('#createCitrixWebinarSummary');

                $.ajax({
                    type: 'GET',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: '/Webinar/CreateCitrixWebinar/?idWebinar=' + idWebinar + "&webinarKey=" + promptForKey,
                    dataType: constants.JsonDataType,
                    data: JSON.stringify("idWebinar"),
                    headers: headers,
                    beforeSend: function () {
                        $('#result').remove();

                        formProcessor.clearValidationSummary(createCitrixWebinarSummary);

                        $(self)
                            .append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                    }
                })
                    .done(function (data) {
                        
                        console.log(data);
                        alert(data.Result);
                        if (data.Result === "Success") {

                            console.log(data);
                            $(self).html(
                                $('<div id="result" class="label label-success pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details Updated</div>'));
                            label.hide().insertAfter($(self)).fadeIn(500);

                        } else if (!data.isSuccessful) {
                            formProcessor.lightUpValidationSummary('updateConnInfoValSummary', data);

                        }
                    })
                    .always(function (data) {
                        $('#waitSpinner').remove();
                    });
            }
        });

    };

}(UCI));

// refactor opportunity - this function now lives in the formProcessor object in form-processor.js
function clearValidationSummary() {
    var valSummary = $('#updateConnInfoValSummary');
    valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

    var errorsList = valSummary.find('ul');
    errorsList.empty();
    errorsList.append('<li style="display:none"></li>');
}