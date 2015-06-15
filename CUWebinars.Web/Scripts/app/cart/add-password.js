
$(function() {

    var addPasswordForm = $('#addPasswordForm');
    var submitButton = addPasswordForm.find('input[type="submit"]');
    var addPasswordContainer = $('#addPasswordContainer');
    var feedbackContainer = $('#FeedbackContainer');
    var containerHeight = addPasswordContainer.height();
    var addPwdMsgLabelWrap = $('#addPwdMsgLabelWrap');
    var validationSummary = $('#addPwdValSummary');
    $('#NewPassword').focus();

    submitButton.on('click', function(e) {
        e.preventDefault();

        addPasswordForm.submit();
    });

    addPasswordForm.on('submit', function(e) {

        e.preventDefault();

        $.validator.unobtrusive.parse(addPasswordForm);

        if (!addPasswordForm.valid())
            return false;

        var url = addPasswordForm.attr('action');
        var data = addPasswordForm.serialize();

        Rollbar.info({ "ap-#1": { payload: data } });

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: data,
            beforeSend: function () {
                $(this).attr('disabled', 'disabled');
                addPwdMsgLabelWrap.html('<span class="label label-info">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Adding your password...</span>');
                formProcessor.clearValidationSummary(validationSummary);

                //Rollbar.info("addPasswordForm Sent");
            }
        }).done(function (data) {
            if (data.Result === 'Success') {

                //Rollbar.info("addPasswordForm Success");
                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                formParent.prepend('<div class="legendImitator">Account Created</div><span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Your account is confirmed.</span><div><a href="/Account/Login">Log In</a><div>');
                feedbackContainer.show();

                $('#main_menu ul.primary_menu').append('<li><a href="/Account/MyWebinars">My Webinars</a></li>');

                Rollbar.info({ 'ap-#2': { result: 'add password succeeded' } });
            } else if (data.Result === 'TimedOut') {
                feedbackContainer.show();

                addPasswordContainer.height(containerHeight);
                var formParent = addPasswordForm.parent();
                addPasswordForm.fadeOut();
                Rollbar.error({ 'ap-#3': { result: data, msg: 'add password timed out' } });
                formParent.prepend('<div class="legendImitator">Order Entry Completed</div><div>Your order is recorded. Watch your email for links to your event\'s materials and other important information.</p></div>');
            } else if (!data.isSuccessful) {
                addPwdMsgLabelWrap.html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There has been an error</span>');
                Rollbar.error({ 'ap-#4': { result: data } });
                formProcessor.lightUpValidationSummary('addPwdValSummary', data);
            }
        });
    });
});