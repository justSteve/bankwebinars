var UWF = {}; // create namespace to prevent collisions. UWF is 'Update Webinar Files'

var addFilesButton,
    webinarFilesSubmitButton,
    manageFilesWrapper,
    breakSuffix,
    deleteItem,
    locationsSpanPrefix,
    numberOfWebinarFiles;


$(function() {
    UWF.primeDomVariables();
    UWF.wireUpHandlersForAddFilesModal();
});

(function (ns) {

    ns.deleteItem = function(event) {
        numberOfWebinarFiles--;
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

    ns.primeDomVariables = function() {
        addFilesButton = $('#addFilesButton');
        manageFilesWrapper = $('#manageFilesWrapper');
    };

    ns.wireUpHandlersForAddFilesModal = function() {

        var newFileId;
        numberOfWebinarFiles = $('#manageFilesWrapper div[id^="fileDetails_"]').length;
        newFileId = numberOfWebinarFiles++;

        if (numberOfWebinarFiles < 1) {
            //$('#sumbitAdditionalLocationsButton').off('click');

        } else {

            var trashCans = manageFilesWrapper.find('i[id$="-Filedetails-delete"]');
            
            $.each(trashCans, function(idx, i) {
                $(i).on('click', ns.deleteItem);
            });
        }

        $('#addFilesButton').on('click', function(e) {

            e.preventDefault();
            // Add 'N' suffix so at server we can tell that it is a new file. The identifier is just for client-side purposes and for the form submission.
            $(getNewFileDetailsFragment(newFileId + 'N')).hide().appendTo(manageFilesWrapper).fadeIn(500, function(e) {
                $(this).find('i').on('click', ns.deleteItem);
            }); 

            numberOfWebinarFiles++;
            newFileId++;

        });

        $('#updateWebinarFilesButton').on('click', function (e) {
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
                    $('#updateWebinarFilesButton').after().html('<span class="label label-info">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Updating...</span>');
                }
            }).done(function (data) {
                if (data.result === 'Success') {
                    
                } else {
                    
                }
            }).always(function (data) {
                $('#updateWebinarFilesButton').after().html('<span class="label label-info">&nbsp;&nbsp;&nbsp;&nbsp;done...</span>');
            });

        });
    };

}(UWF));

function getNewFileDetailsFragment(id) {
    return '<div id="fileDetails_' + id + '" class="webinarFileDetails"><i class="icon-trash icon-white pull-right" style="cursor: pointer" id="' + id + '-Filedetails-delete"></i><input type="hidden" name="connectionInfoModel.WebinarFiles[' + id.substring(0, 1) + '].idWebinarFile" value="' + id + '" /><input type="hidden" name="connectionInfoModel.WebinarFiles[' + id.substring(0, 1) + '].idWebinar" value="' + cartStateManager.getWebinarId() + '" /><div id="fileLocationDiv_' + id + '"><span class="control-label">File Name</span><input type="text" class="form-control" name="connectionInfoModel.WebinarFiles[' + id.substring(0, 1) + '].fileLocation" /><i class="icon-book icon-white"></i></div><div id="fileDescDiv_' + id + '"><span class="control-label">Label</span><input type="text" class="form-control" name="connectionInfoModel.WebinarFiles[' + id.substring(0, 1) + '].fileDesc" /><br /></div></div>';
}