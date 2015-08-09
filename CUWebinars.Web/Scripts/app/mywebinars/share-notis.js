
$(document).ready(function () {

    isValidEmailAddress = function (email) {
        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        return re.test(email);
    };

    $('button[id^="submitThis"]').click(function (e) {

        var self = $(this);
        e.preventDefault();
        e.stopImmediatePropagation();

        var payload = {
            addresses: $(this).siblings(".span9").val(),
            idOrder: this.id.split('_')[1]
        };
        if (!isValidEmailAddress(payload.addresses)) {
            alert("Invalid Email");
            return;
        } else {
            $.ajax({
                type: 'POST',

                contentType: constants.JsonContentType,
                cache: false,
                url: '/Account/ShareNotifications',
                dataType: constants.JsonDataType,
                data: JSON.stringify(payload),
                beforeSend: function () {

                    $(self).append('<span id="addSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                    $(self).attr('disabled', 'disabled');
                }
            })
                .done(function (data) {
                    if (data) {

                        alert("Submission was successful. " + payload.addresses + " will receive future notifications on this order.");
                        $(self).find('addSpinner').remove();

                        $(self["#caption"]).html('<span>Notifications will be CCed to ' + payload.addresses + '</span>');
                    } else {
                        alert("serverFail");
                        //$(self["#caption"]).html('<span>Notifications are CCed to</span>');
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    var a = 1;

                })
                .always(function (result) {

                    $(self).find('span').remove();

                   
                });
        }
    });
});
