var UCI = {}; // create namespace to prevent collisions. UCI is 'Update Connection Info'

var addFilesButton,
    webinarFilesSubmitButton,
    manageFilesWrapper,
    breakSuffix,
    deleteItem,
    locationsSpanPrefix,
    numberOfWebinarFiles;


$(function() {
    UCI.primeDomVariables();
    UCI.wireUpHandlersForUpdateConnectionInfoModal();

    var modalFormOptionsOnPageLoad = {
        keyboard: true,
        backdrop: 'static',
        show: true,
    };

    UCI.updateConnectionInfoModalButton.on('click', function (e) {
        e.preventDefault();
        clearValidationSummary();
        $('#result').remove();
        UCI.updateConnectionInfoModal.modal(modalFormOptionsOnPageLoad);
    });

    UCI.updateConnectionInfoModal.on('shown', function() {
        
    });
});

(function (ns) {

    ns.primeDomVariables = function () {
        ns.updateConnectionInfoModalButton = $('#UpdateConnectionInfoModalButton');
        ns.updateConnectionInfoModal = $('#UpdateConnectionInfoModal');
        ns.updateConnectionInfoForm = $('#_UpdateConnectionInfo');
    };

    ns.wireUpHandlersForUpdateConnectionInfoModal = function () {

        $('#saveDetailsButton').on('click', function(e) {

            e.preventDefault();

            var self = this;

            var inputs = formProcessor.getApplicableInputs('_UpdateConnectionInfo');
            var payload = formProcessor.processInputs(inputs);

            var token = UCI.updateConnectionInfoForm.find('input[name=__RequestVerificationToken]').val();
            var headers = {};
            headers['__RequestVerificationToken'] = token;

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: UCI.updateConnectionInfoForm.attr('action'),
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                headers: headers,
                beforeSend: function() {
                    $('#result').remove();

                    clearValidationSummary();

                    $(self).append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (data) {
                if (data.Result == "Success") {
                    var label = $('<div id="result" class="label label-success pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Details Updated</div>');
                    label.hide().insertAfter($(self)).fadeIn(500);
                } else if (!data.isSuccessful) {
                    formProcessor.lightUpValidationSummary('updateConnInfoValSummary', data);
                }
            }).always(function (data) {
                $('#waitSpinner').remove();
            });

        });

    };

}(UCI));


function clearValidationSummary() {
    var valSummary = $('#updateConnInfoValSummary');
    valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

    var errorsList = valSummary.find('ul');
    errorsList.empty();
    errorsList.append('<li style="display:none"></li>');
}