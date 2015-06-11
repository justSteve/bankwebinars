// document.ready starts here
$(function () {

    var submitNewPasswordButton = $('#submitNewPasswordButton');
    var form = $('#LogInContainer form');
    var formParent = form.parent();
    $('#Password').focus();

    submitNewPasswordButton.on('click', function (e) {

        e.preventDefault();

        //  Check if the label is already in existance (user may hit the button twice - for some odd reason). If not, create it.
        var crunchingLabel = $('#crunchingLabel');
        if (crunchingLabel.length < 1)
            crunchingLabel = submitNewPasswordButton.after('<div id="crunchingLabel" style="display:inline-block; margin-left:5px"></div>').next();


        var url = form.attr('action');
        var data = form.serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            dataType: constants.JsonDataType,
            cache: false,
            url: url,
            data: data,
            beforeSend: function () {
                // this is where we append a loading image
                crunchingLabel.html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Submitting password...</span>');
            }
        }).done(function (data) {

            if (data.Result === 'Success') {
                form.fadeOut(500, function () {
                    formParent.append('<div class="legendImitator">Password Reset</div><div style="margin-bottom: 25px"><span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp; Password reset succeeded!</span></div>');
                });
                Rollbar.info("#699. reset succeeded");
            } else if (data.isSuccessful === false) {
                formProcessor.lightUpValidationSummary('valSummaryResetPwdForm', data);

                crunchingLabel.remove();
            } else {
                crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;<strong>Oops!</strong>Connection Error #545. Please refresh the page and try again. In case of continued problems, please contact us at 800-831-0678 ext 707.</span>');
                Rollbar.error("#698 fail ");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Rollbar.error("#697 fail ");
            crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;<strong>Oops!</strong>Connection Error #547. Please refresh the page and try again. In case of continued problems, please contact us at 800-831-0678 ext 707.</span>');
        });

        return false;
    });
});