
$(function() {

    var addPasswordForm = $('#addPasswordForm');
    var submitButton = addPasswordForm.find('input[type="submit"]');
    var addPasswordContainer = $('#addPasswordContainer');
    var containerHeight = addPasswordContainer.height();

    submitButton.on('click', function(e) {

        e.preventDefault();

        var url = addPasswordForm.attr('action');
        var data = addPasswordForm.serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: data,
            beforeSend: function () {
                $(this).attr('disabled', 'disabled');
                $('#addPwdMsgLabelWrap').html('<span class="label label-info">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Adding your password...</span>');
                $('#addPwdValSummary').empty();
            }
        }).done(function(data) {
            if (data.Result === 'Success') {
                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                formParent.prepend('<h2>Password Added</h2><span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;You have successfully set your password....</span>');
            }
        });
    });


});