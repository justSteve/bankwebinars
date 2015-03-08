
$(function() {

    var addPasswordForm = $('#addPasswordForm');
    var submitButton = addPasswordForm.find('input[type="submit"]');
    var addPasswordContainer = $('#addPasswordContainer');
    var feedbackContainer = $('#FeedbackContainer');
    var containerHeight = addPasswordContainer.height();
    $('#NewPassword').focus();

    submitButton.on('click', function(e) {
        alert("hit");
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
        }).done(function (data) {
            feedbackContainer.show();
            if (data.Result === 'Success') {
                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                formParent.prepend('<div class="legendImitator">Account Created</div><span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Your account is confirmed.</span><div><a href="/Account/Login">Log In</a><div>');

                $('#main_menu ul.primary_menu').append('<li><a href="/Account/MyWebinars">My Webinars</a></li>');
            } else if (data.Result === 'TimedOut') {
                
                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                Rollbar.error("AddPasswordFromCartCheckoutTimedOut", data.Result);
                formParent.prepend('<div class="legendImitator">Order Entry Completed</div><div>Your order is recorded. Watch your email for links to your event\'s materials and other important information.</p></div>');
            }
        });
    });
});