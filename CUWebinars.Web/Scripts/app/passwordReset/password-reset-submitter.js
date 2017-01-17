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

                    alert("The reset was successful. You will now be logged in.");
                    $('#emailFR').val(data.email);
                    $('#passwordFR').val(data.password);
                    $('#frmSignInFR').submit();

                });
            } else if (data.Result === 'Not Found') {
                form.fadeOut(500, function () {
                    formParent.append('<div class="legendImitator">That Email was not found.</div><div style="margin-bottom: 25px"><span class="label label-danger">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp; We were unable to find that email. Please try again. In case of continued problems, please contact us with our online chat (lower right corner of this page).</span></div>');
                });
                //Rollbar.info("#699. reset succeeded");
            } else if (data.Result === 'Invalid Key') {
                form.fadeOut(500, function () {
                    formParent.append('<div class="legendImitator">Expired Reset Key.</div><div style="margin-bottom: 25px"><span class="label label-danger">&nbsp;&nbsp;Please ensure you are clicking the most recent email with subject line: Your New Password Reset Request". In case you are not receiving the notifications from us it can be helpful to send an email to @Model.TenentSupportEmail with a subject line of "Test". In case immediate need please contact us with our online chat (lower right corner of this page).</div>');
                });
                //Rollbar.info("#699. reset succeeded");
            } else if (data.isSuccessful === false) {
                
                //formProcessor.lightUpValidationSummary('valSummaryResetPwdForm', data);
                console.log(data);
                crunchingLabel.html('<div class="label large label-danger">&nbsp;&nbsp;&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Reset Error #547.</div><div > Please ensure you are using a new password.</div>');

                //crunchingLabel.remove();
            } else {

                crunchingLabel.html('<div class="label label-warning">&nbsp;&nbsp;&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Reset Error #545.</div><div > Please refresh the page and try again. In case of continued problems, please contact us with our online chat (lower right corner of this page).</span>');
                //Rollbar.error("#698 fail ");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown)
            crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Connection Error #547. Please refresh the page and try again. In case of continued problems, please contact us with our online chat (lower right corner of this page).</span>');
        });

        return false;
    });
});