$('form#frmSignIn').submit(function (e) {

    e.preventDefault();

    var data = $(this).serialize();
    var url = $(this).attr('action');

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        dataType: constants.JsonDataType,
        contentType:constants.FormPostContentType,
        //headers: headers,
        beforeSend: function(xhr) {

            var valSummary = $('#LoginValSummary');
            valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

            var errorsList = valSummary.find('ul');
            errorsList.empty();
            errorsList.append('<li style="display:none"></li>');
            $('#signingInMsg').remove();
            $('#loginMsgLabelWrap').append('<span id="signingInMsg" class="label label-info"><i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Submitting ...</span>');
        }
    }).done(function (data) {
        if (data.result === 'LoggedIn') {
            $('#signingInMsg').html('<i class="icon-spinner icon-spin "></i>&nbsp;&nbsp;Redirecting you now ...');
            //var utilities = new Common.Utilities();
            window.location.href = 'http://localhost:3538/' + data.returnUrl;
        }
    }).fail(function (data) {
        var oi = data;
    }).always(function (data) {

    });

});
