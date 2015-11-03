var WR = {}; // create namespace to prevent collisions. WR is 'Update Webinar Files'

$(function () {
    WR.primeDomVariables();
    WR.wireUpHandlersForAddFilesModal();
    
    var updateWebinarRecordingModalButton = $('#UpdateWebinarRecordingModalButton');
    var updateWebinarRecordingModal = $('#UpdateWebinarRecordingModal');
    var modalFormOptionsOnPageLoad = {
        keyboard: true,
        backdrop: 'static',
        show: true
    };

    updateWebinarRecordingModalButton.on('click', function (e) {

        e.preventDefault();

        clearValidationSummary();

        $('#result').remove();

        updateWebinarRecordingModal.modal(modalFormOptionsOnPageLoad);

    });
});

(function (ns) {

    ns.deleteItem = function (event) {
        WR.numberOfWebinarFiles--;
        var trashClicked = event.currentTarget;
        //var trashClickedId = trashClicked.id;
        //var idx = trashClickedId.substring(0, trashClickedId.indexOf('-'));

        var divToRemove = $(trashClicked).parent();

        divToRemove.hide(500, function () {
            var id = divToRemove.attr('id');
            var input = $(this).find('div[id^="fileDescDiv_"] input');

            if (id.charAt(id.length - 1) === 'N') {
                input.val(input.val() + '-ND'); // If new file mark to be ignored. Not removed from dom because indexing of collection must be preserved for the Model Binders to deserialize ViewModel.

            } else {
                input.val(input.val() + '-D'); // If an existing file mark for deletion at the server
            }
        });
    };

    ns.primeDomVariables = function () {
        WR.addFilesButton = $('#addFilesButton');
        WR.manageRecordingsWrapper = $('#manageRecordingsWrapper');
    };

    ns.wireUpHandlersForAddFilesModal = function () {

        var newFileId;
        WR.numberOfWebinarFiles = WR.manageRecordingsWrapper.find('div[id^="fileDetails_"]').length;
        newFileId = WR.numberOfWebinarFiles++;

        if (WR.numberOfWebinarFiles < 1) {
            //$('#sumbitAdditionalLocationsButton').off('click');

        } else {

            var trashCans = WR.manageRecordingsWrapper.find('i[id$="-Filedetails-delete"]');

            $.each(trashCans, function (idx, i) {
                $(i).on('click', ns.deleteItem);
            });
        }

        //WR.addFilesButton.on('click', function (e) {

        //    e.preventDefault();
        //    // Add 'N' suffix so at server we can tell that it is a new file. The identifier is just for client-side purposes and for the form submission.
        //    $(getNewFileDetailsFragment(newFileId + 'N')).hide().appendTo(manageRecordingsWrapper).fadeIn(500, function (e) {
        //        $(this).find('i').on('click', ns.deleteItem);
        //    });

        //    WR.numberOfWebinarFiles++;
        //    newFileId++;

        //});

        var updateWebinarRecordingButton = $('#updateWebinarRecordingButton');

        updateWebinarRecordingButton.on('click', function (e) {

            e.preventDefault();

            $('#_UpdateWebinarRecording').submit();
        });

        $('#_UpdateWebinarRecording').on('submit', function (e) {

            e.preventDefault();

            var updateFilesValSummary = $('#updateFilesValSummary');

            var url = $(this).attr('action');
            var payload = $(this).serialize();

            //Rollbar.info({ 'ur-#1': { 'payload': payload } });

            $.ajax({
                type: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                cache: false,
                url: url,
                dataType: 'json',
                data: payload,
                beforeSend: function () {

                    $('#result').remove();
                    formProcessor.clearValidationSummary(updateFilesValSummary);

                    updateWebinarRecordingButton.append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (data) {
                if (data.result === 'Success') {
                    var label = $('<div id="result" class="label label-success pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Files Updated</div>');
                    label.hide().insertAfter(updateWebinarRecordingButton).fadeIn(500);
                    //Rollbar.info({ 'ur-#2': { 'result': data } });
                } else if (data['Result'] === 'Fail') {
                    var label = $('<div id="result" class="label label-important pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;' + data['Message'] + '</div>');
                    label.hide().insertAfter(updateWebinarRecordingButton).fadeIn(500);
                    //Rollbar.info({ 'ur-#3': { 'fail-result': data } });
                } else {
                    formProcessor.lightUpValidationSummary('updateFilesValSummary', data);
                    //Rollbar.info({ 'ur-#4': { 'fail-result': data } });
                }
            }).always(function (data) {
                $('#waitSpinner').remove();
            });

        });
    };

}(WR));

function getNewFileDetailsFragment(id) {
//placeholder 
    return '<div id="fileDetails_' + id + '" class="recordingFile"></div>';
}
