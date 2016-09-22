$(function () {
    $("button.edit-order").on("click", function() {
        var $container = $(this).closest("div[id^='odrSum-']");
        ShowEditOrderForm($container.data("order-id"), $container.data("webinar-id"));
    });

    $("button.remove-order").on("click", function () {
        alert('bye');
    });

});

function ShowEditOrderForm(orderId, webinarId) {
    // deal with any open ones.  may have edits in progress... leave that for later
    $("div[id^='edit-order-form-'").hide().html("");

    // very similar to what is in show-webinars-child-row.js

    var html = "";
    
    $.ajax({
        async: true,
        url: "/webinar/geteditincartjson",
        data: ({ id: webinarId }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
            var $div = $("#edit-order-form-" + orderId);
            $div.html(html);
            $div.show("slower");

            $("button.cancel-edit-order").on("click", function () {
                $div.hide();
                $div.html("");
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/getaddtocartjson: " + textStatus);
        },
        beforeSend: function () {
            console.log("beforesend");
            // IE and Chrome don't time this stuff very well due to the async=false... :/
            //$td.append('<div id="loadingSpinnerContainer"><i id="loadingSpinner" class="icon-spinner icon-spin"></i>&nbsp;</div>');
            //addIsLoadingIndicator($td, -1); // let the ajax "complete" call (below) remove loading indicator
        },
        complete: function () {
            console.log("complete");
            // IE and Chrome don't time this stuff very well due to the async=false... :/
            //removeIsLoadingIndicator($td);
            //$('#loadingSpinnerContainer', $td).fadeOut();
            //setTimeout(function () { $('#loadingSpinnerContainer', $td).remove(); }, 1000);
        }
    });
}
