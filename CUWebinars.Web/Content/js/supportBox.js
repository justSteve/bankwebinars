var supportBox = null;

$(function () {

    supportBox = $("#supportBox").overlay({ api: true });

    // define function that opens the overlay
    window.openSupportBox = function () {
        supportBox.load();
    }
    var form = $("#supportForm");
    form.submit(function () {
        $.ajax(
            {
                type: "POST",
                url: form.attr("action"),
                data: form.serialize(),
                cache: false,
                async: false,
                success: function (result) {
                    if (result == 'OK') {
                        alert('Your comment has been sent');
                        supportBox.close();
                    } else {
                        alert(result);
                    }
                },
                error: function (req, status, error) {
                    alert('Error. Please try again.');
                }
            });
        return false;
    });
});
