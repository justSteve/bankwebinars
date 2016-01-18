/// <reference path="../../jquery-dateFormat.min.js" />
var UWF = {}; // create namespace to prevent collisions. UWF is 'Update Webinar Files'

// document.ready function
$(function() {
    UWF.primeDomVariables();
    UWF.wireUpHandlersForAddFilesModal();

    var updateWebinarHandoutsModalButton = $('#UpdateWebinarHandoutsModalButton');
    var updateWebinarHandoutsModal = $('#UpdateWebinarHandoutsModal');
    var modalFormOptionsOnPageLoad = {
        keyboard: true,
        backdrop: 'static',
        show: true
    };



    updateWebinarHandoutsModalButton.on('click', function (e) {
        e.preventDefault();
        clearValidationSummary();
        $('#result').remove();
        updateWebinarHandoutsModal.modal(modalFormOptionsOnPageLoad);

    });
});

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.deleteItem = function (event) {

        event.preventDefault();

        UWF.numberOfWebinarFiles -= 1;
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
                var fileDescr = $.trim(input.val());
                input.val(fileDescr + '-D'); // If an existing file mark for deletion at the server
            }
        });
    };

    ns.primeDomVariables = function() {
        UWF.addFilesButton = $('#addFilesButton');
        UWF.manageFilesWrapper = $('#manageFilesWrapper');
    };

    ns.wireUpHandlersForAddFilesModal = function() {

        var newFileId;
        UWF.numberOfWebinarFiles = UWF.manageFilesWrapper.find('div[id^="fileDetails_"]').length;
        newFileId = UWF.numberOfWebinarFiles;

        if (!UWF.manageFilesWrapper) {
            UWF.manageFilesWrapper = $('#manageFilesWrapper');
            UWF.webinarFilesSelected = $('#webinarFilesSelected');
        }

        if (UWF.numberOfWebinarFiles < 1) {
            var a = "holder";
        } else {

            var trashCans = UWF.manageFilesWrapper.find('i[id$="-Filedetails-delete"]');
            
            $.each(trashCans, function(idx, i) {
                $(i).on('click', ns.deleteItem);
            });
        }

        UWF.addFilesButton.on('click', function(e) {

            e.preventDefault();

            var newFile = UWF.getNewFileDetailsFragment(newFileId + 'N');

            // Add 'N' suffix so at server we can tell that it is a new file. The identifier is just for client-side purposes and for the form submission.
            $(newFile).hide().appendTo(UWF.manageFilesWrapper).fadeIn(500, function (e) {
                $(this).find('i.icon-trash').on('click', ns.deleteItem);
            }); 

            UWF.numberOfWebinarFiles += 1;
            newFileId += 1;

        });

        var updateWebinarFilesButton = $('#updateWebinarFilesButton');

        updateWebinarFilesButton.on('click', function (e) {
            $('#_UpdateWebinarFiles').submit();
        });

        $('#_UpdateWebinarFiles').on('submit', function (e) {

            e.preventDefault();

            var url = $(this).attr('action');
            var payload = $(this).serialize();

            $.ajax({
                type: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                cache: false,
                url: url,
                dataType: 'json',
                data: payload,
                beforeSend: function () {

                    if (UWF.webinarFilesSelected) {
                        UWF.webinarFilesSelected.empty();
                    }

                    $('#result').remove();

                    clearValidationSummary();

                    updateWebinarFilesButton.append('<span id="waitSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (data) {
                if (data['Result'] === 'Success') {
                    var label = $('<div id="result" class="label label-success pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Files Updated</div>');
                    label.hide().insertAfter(updateWebinarFilesButton).fadeIn(500);
                    
                    var hiddenInputs = UWF.manageFilesWrapper.find('input[type="text"]');

                    $.each(hiddenInputs, function (idx, i) {
                        var clone = $(i).clone();
                        clone.val($(i).val());
                        UWF.webinarFilesSelected.append(clone);
                    });

                } else if (data['Result'] === 'Fail') {
                    var label = $('<div id="result" class="label label-important pull-left block buttonAdjacentLabel">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;' + data['Message'] +'</div>');
                    label.hide().insertAfter(updateWebinarFilesButton).fadeIn(500);
                } else {
                    formProcessor.lightUpValidationSummary('updateFilesValSummary', data);
                }
            }).always(function (data) {
                $('#waitSpinner').remove();

                if (data.status && data.status === 500) {
                    data.data = {};
                    data.data.error = 'For customer service contact us by using the Online Chat button below or emailing Support@ttsTrain.com.';
                }

                if (!data['Result']){
                    formProcessor.lightUpValidationSummary('updateFilesValSummary', data);
                }
            });

        });
    };

    ns.getNewFileDetailsFragment = function(id) {
        return '<div id="fileDetails_' + id + '" class="webinarFileDetails">' +
            '<i class="icon-trash icon-white pull-right" style="cursor: pointer" id="' + id + '-Filedetails-delete"></i>' +
            '<input type="hidden" name="WebinarFiles[' + id.substring(0, 1) + '].idWebinarFile" value="' + id + '" />' +
            '<input type="hidden" name="WebinarFiles[' + id.substring(0, 1) + '].idWebinar" value="' + currentWebinarId + '" />' +
            '<div id="fileLocationDiv_' + id + '"><span class="control-label">File Name</span>' +
            '<input type="text" class="form-control" name="WebinarFiles[' + id.substring(0, 1) + '].fileLocation" />' +
            '<i class="icon-book icon-white"></i></div>' +
            '<div id="fileDescDiv_' + id + '"><span class="control-label">Label</span>' +
            '  <input type="text" class="form-control" name="WebinarFiles[' + id.substring(0, 1) + '].fileDesc" /><br /></div></div>';
        //return '<div id="fileDetails_' + id + '" class="webinarFileDetails"><i class="icon-trash icon-white pull-right" style="cursor: pointer" id="' + id + '-Filedetails-delete"></i><input type="hidden" name="WebinarFiles[' + id.substring(0, 1) + '].idWebinarFile" value="' + id + '" /><input type="hidden" name="WebinarFiles[' + id.substring(0, 1) + '].idWebinar" value="' + OCA.cartStateManager.getWebinarId() + '" /><div id="fileLocationDiv_' + id + '"><span class="control-label">File Name</span><input type="text" class="form-control" name="WebinarFiles[' + id.substring(0, 1) + '].fileLocation" /><i class="icon-book icon-white"></i></div><div id="fileDescDiv_' + id + '"><span class="control-label">Label</span><input type="text" class="form-control" name="WebinarFiles[' + id.substring(0, 1) + '].fileDesc" /><br /></div></div>';
    };
    
}(UWF));

function clearValidationSummary() {
    var valSummary = $('#updateFilesValSummary');
    valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

    var errorsList = valSummary.find('ul');
    errorsList.empty();
    errorsList.append('<li style="display:none"></li>');
}