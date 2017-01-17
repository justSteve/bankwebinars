// document.ready starts here
$(function () {

    var normalResetPasswordButton = $('#NormalResetPasswordButton');
    var resetPassEmail = $('#ResetPassEmail');

    normalResetPasswordButton.on('click', function (e) {

        if ($('#EdgeCaseResetPasswordButton').data('clicked'))
            $('#EdgeCaseResetPasswordButton').removeData('clicked');
        $(this).data('clicked', true);
    });

    resetPassEmail.on('keyup', function() {
        return false;
    });

    $('form#ResetPasswordForm').submit(function (e) {

        e.preventDefault();
        
        var normalResetPasswordButtonClicked = normalResetPasswordButton.data('clicked');

        //  Check if the label is already in existence (user may hit the button twice - for some odd reason). If not, create it.
        var crunchingLabel = $('#crunchingLabel');
        if (crunchingLabel.length < 1)
            crunchingLabel = normalResetPasswordButtonClicked ? resetPassEmail.after('<div id="crunchingLabel" style="display:inline-block; margin-left:5px"></div>').next() : $('#labelEmail');
            
        if (!resetPassEmail.valid()) {
            return false;
        }

        var token = $('#ResetPasswordForm').find('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var jsonUrl = $(this).attr('action');
        var email = resetPassEmail.val();
        var payload = { email: email };

        L.clientLogger.info('pw-#1', payload);

        if (email.length === 0) {
            resetPassEmail.focus();
        } else {
            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                dataType: constants.JsonDataType,
                cache: false,
                url: jsonUrl,
                data: JSON.stringify(payload),
                headers: headers,
                beforeSend: function() {
                    // this is where we append a loading image
                    crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Please Wait...</span>');
                }
            }).done(function (data) {

                if (data.Result === 'Success') {
                    crunchingLabel.html('<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp; Reset Instructions sent!</span>');
                    $('#wrapReset div.container').hide("slow");
                    $('#NormalResetPasswordButton').hide();
                } else {

                    //Rollbar.error({ 'Else branch of form#ResetPasswordForm: ': { 'data': data} });

                    if (data['Invalid'] === 'UserNotVerified') {
                        crunchingLabel.html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;There was a problem resetting your password. Contact us at @globalConfig.TenantEmail</span>');
                    } else if (data['Invalid'] === 'UnkownEmail') {
                        crunchingLabel.html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;We do not have a record of that email address. Contact us at with our online chat (lower right corner of this page).</span>');
                    } else {
                        crunchingLabel.html('<span class="label label-important">&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;Error. Please retry...</span>');
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // failed request; give feedback to user
                crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;<i class="icon icon-exclamation-sign"></i>&nbsp;<strong>Oops!</strong> Try that again in a few moments.In case of continued problems, please contact us with our online chat (lower right corner of this page).</span>');
                

                //Rollbar.error({ 'ResetPassword AJAX error: ': { 'errorThrown': errorThrown } });

            });
        }
        return false;
    });
});