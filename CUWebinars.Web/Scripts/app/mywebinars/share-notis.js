
$(document).ready(function () {

    isValidEmailAddress = function (email) {
        console.log(email);
        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        return re.test(email);
    };

    $('button[id^="submitThis"]').click(function (e) {

        var self = $(this);
        e.preventDefault();
        e.stopImmediatePropagation();
        var payload = {
            addresses: $(this).siblings(".span9").val(),
            idOrder: this.id.split('_')[1],
            source: $(this).siblings(".source").val()
        };
        console.log("this source " + payload.source)
        if (!isValidEmailAddress(payload.addresses)) {

            var hasMulti = payload.addresses.split(',');

            if (hasMulti.length > 0) {
                for (var i = 0; i < hasMulti.length; i++) {
                    if (!isValidEmailAddress(hasMulti[i])) {
                        alert("One of the emails in your list is not valid.");
                        return;
                    }
                }
                postAddress(payload, e);
                return;
            }
            alert("Invalid Email");
            return;
        } else {
            postAddress(payload, e);
        }
    });
});

function postAddress(payload, e) {
    var self = $(this);
    e.preventDefault();
    e.stopImmediatePropagation();
    console.log(payload.source);
    $.ajax({
        type: 'POST',

        contentType: constants.JsonContentType,
        cache: false,
        url: payload.source,
        dataType: constants.JsonDataType,
        data: JSON.stringify(payload),
        beforeSend: function () {

            //$(self).append('<span id="addSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            //$(self).attr('disabled', 'disabled');
        }
    })
        .done(function (data) {
            if (data.Result === "Success") {

                alert(data.UpdateCaption);

            } else {
                alert("Action Failed");

                //$(self["#caption"]).html('<span>Notifications are CCed to</span>');
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {

            alert("serverFail");

        })
        .always(function (result) {
            //$(self).find('span').remove();
        });
}