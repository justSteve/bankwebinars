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

        //  Check if the label is already in existance (user may hit the button twice - for some odd reason). If not, create it.
        var crunchingLabel = $('#crunchingLabel');
        if (crunchingLabel.length < 1)
            crunchingLabel = normalResetPasswordButtonClicked ? resetPassEmail.after('<div id="crunchingLabel" style="display:inline-block; margin-left:5px"></div>').next() : $('#labelEmail');
            
        if (!resetPassEmail.valid()) {
            return false;
        }

        var jsonUrl = '/Account/ResetPassword';
        var email = resetPassEmail.val();

        if (email.length === 0) {
            resetPassEmail.focus();
        } else {
            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                dataType: constants.JsonDataType,
                cache: false,
                url: jsonUrl,
                data: JSON.stringify({ email: email }),
                beforeSend: function() {
                    // this is where we append a loading image
                    crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Please Wait...</span>');
                }
            }).done(function(data) {

                if (data.Status === 'Success') {
                    crunchingLabel.html('<span class="label label-success">&nbsp; Reset Instructions sent!</span>');
                    $('#wrapReset div.container').hide("slow");
                    $('#sent2Address').html(email);
                } else {
                    crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;Error. Please retry...</span>');
                }

            }).fail(function() {
                // failed request; give feedback to user
                crunchingLabel.html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;<strong>Oops!</strong> Try that again in a few moments.</span>');
            });
        }
        return false;
    });
});