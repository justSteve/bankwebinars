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
        var trashClickedId = trashClicked.id;
        //var idx = trashClickedId.substring(0, trashClickedId.indexOf('-'));

        var divToRemove = $(trashClicked).parent();

        divToRemove.hide(500, function () {
            var id = divToRemove.attr('id');
            if (id.charAt(id.length - 1) === 'N') {
                $(this).remove();
            } else {
                var input = $(this).find('input[type="hidden"]');
                input.val(input.val() + 'D'); // If an existing file mark for deletion at the server
            }

            console.log($(this).data('delete'));
        });
    };

    ns.primeDomVariables = function() {
        addFilesButton = $('#addFilesButton');
        manageFilesWrapper = $('#manageFilesWrapper');
    };

    ns.wireUpHandlersForAddFilesModal = function() {

        var newFileId = 1;
        numberOfWebinarFiles = $('#manageFilesWrapper div[id^="fileDetails_"]').length;

        if (numberOfWebinarFiles < 1) {
            //$('#sumbitAdditionalLocationsButton').off('click');

        } else {

            var trashCans = manageFilesWrapper.find('i');

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
    return '<div id="fileDetails_' + id + '" class="webinarFileDetails"><i class="icon-trash icon-white pull-right" style="cursor: pointer" id="' + id + '-Filedetails-delete"></i><input type="hidden" name="idWebinarFile_' + id + '" value="' + id + '" /><div id="fileLocationDiv_' + id + '"><span class="control-label">File Name</span><input type="text" class="form-control" name="fileLocation_' + id + '" /></div><div id="fileDescDiv_' + id + '"><span class="control-label">Label</span><input type="text" class="form-control" name="fileDesc_' + id + '" /><br /></div></div>';
}