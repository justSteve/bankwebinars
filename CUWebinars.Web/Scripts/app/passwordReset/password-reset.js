//function submitReset() {
//    $("#Email").val($("#RegisterFields_Email").val());
//    $("#Password").val($("#Password1").val());
//    $("#frmSignin").submit();
//}

// document.ready starts here
$(function () {

    $("form#ResetPasswordForm").submit(function (e) {

        e.preventDefault();

        if (!$('#ResetPassEmail').valid()) {
            return false;
        }

        var jsonUrl = "/Account/ResetPassword";
        var email = $("#ResetPassEmail").val();
        if (email.length == 0) {
            $("#ResetPassEmail").focus();
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
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Please Wait...</warning>');
                }
            }).done(function(data) {

                if (data.status === "success") {
                    $('#labelEmail').html('<span class="label label-success">&nbsp; Reset Instructions sent!</span>');
                    $('#wrapReset div.container').hide("slow");
                    $('#MailSentForm').show("slow");
                    $('#sent2Address').html(email);
                } else {
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;Error. Please retry...</span>');
                }



            }).fail(function() {
                // failed request; give feedback to user
                $('body').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
        return false;
    });

    $("form#ResetSent").submit(function (e) {

        e.preventDefault();

        var jsonUrl = "/Account/ResetPassword";
        var email = $("#RetryPassEmail").val();
        if (email.length == 0) {
            $("#RetryPassEmail").focus();
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
                    $('#labelEmail').html('<span class="label label-warning">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Please Wait...</span>');
                }
            }).done(function(data) {
                // successful request; do something with the data
                    if (data.status === "success") {

                     console.log("Hit: " + email);
                    }

                    //$('#ResetPW').hide("slow");
                    //$('#MailSent').show("slow");
                    //$('#sent2Address').html(email);
                    //$('#ResetPassLegend').html('<div class="btn-warning style="width: 200px; height: 20px;">&nbsp;&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;&nbsp;&nbsp;Reset Instructions Sent</div>');
                }).fail(function() {
                // failed request; give feedback to user
                $('body').html('<p class="error"><strong>Oops!</strong> Try that again in a few moments.</p>');
            });
        }
        return false;
    });

});