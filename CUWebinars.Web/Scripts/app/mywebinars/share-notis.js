
$(document).ready(function () {

    isValidEmailAddress = function (emailAddress) {
        var pattern = new RegExp('\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b');
        return pattern.test(emailAddress);
    };

    $('button[id^="submitThis"]').click( function (e) {

        var self = $(this);
        
            e.stopPropagation();
    
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

                    contentType: constants.FormPostContentType,
                    cache: false,
                    url: '/Account/ShareNotifications',
                    dataType: JSON,
                    data: payload,
                    beforeSend: function() {

                        $(self).append('<span id="addSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                        $(self).attr('disabled', 'disabled');
                    }
                })
                .done(function(result) {
                    if (result.Result === 'Success') {
                        var a = 2;
                        //$(self).find('span').remove();

                        //$(self["#caption"]).html('<span>Notifications will be CCed to ' + payload.addresses + '</span>');
                    } else {
                        alert("serverFail");
                        //$(self["#caption"]).html('<span>Notifications are CCed to</span>');
                    }
                })
                .fail(function(jqXHR, textStatus, errorThrown) {
                    var a = 1;

                });
        }
    });
});
