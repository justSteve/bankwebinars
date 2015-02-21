
$(function() {

    var addPasswordForm = $('#addPasswordForm');
    var submitButton = addPasswordForm.find('input[type="submit"]');
    var addPasswordContainer = $('#addPasswordContainer');
    var containerHeight = addPasswordContainer.height();
    $('#NewPassword').focus();

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
                formParent.prepend('<div class="legendImitator">Account Created</div><span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Your account is confirmed.</span><div><a href="/Account/Login">Log In</a><div>');

                $('#main_menu ul.primary_menu').append('<li><a href="/Account/MyWebinars">My Webinars</a></li>');
            } else if (data.Result === 'TimedOut') {
                //TODO: This branch should post back to a method that will display the order summary

                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                formParent.prepend('<div class="legendImitator">Password Confirmation Stalled</div><div><p>Your order is recorded as summarized below. You will receive email with a temparary password so that your account can be fully confirmed within our system.</p></div>');
                
                //$('#main_menu ul.primary_menu').append('<li><a href="/Account/MyWebinars">My Webinars</a></li>');
            }
        });
    });
});