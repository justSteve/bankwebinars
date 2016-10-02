$(function () {
    $("button.edit-order").on("click", function() {
        var $this = $(this);
        var $container = $this.closest("div[id^='odrSum-']");
        ShowEditOrderForm($this, $container.data("order-id"), $container.data("webinar-id"));
    });

    $("button.remove-order").on("click", function () {
        var $this = $(this);
        var $container = $this.closest("div[id^='odrSum-']");
        if ($this.text() === "Remove") {
            RemoveOrder($this, $container.data("order-id"), $container.data("order-row-id"));
        } else if ($this.text() === "Undo") {
            UndoRemoveOrder($this, $container.data("order-id"), $container.data("order-row-id"));
        }
    });

});

function ShowEditOrderForm($button, orderId, webinarId) {
    // deal with any open ones.  may have edits in progress... leave that for later, for now they will be lost
    $("div[id^='edit-order-form-'").hide().html("");

    var html = "";
    
    $.ajax({
        async: true,
        url: "/webinar/geteditincartjson",
        data: ({ id: webinarId }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;

            var $editPanel = $("#edit-order-form-" + orderId);
            $editPanel.html(html);
            $editPanel.show("slower");

            // bind button clicks
            $("button.cancel-edit-order").on("click", function () {
                $editPanel.hide("slower");
                $editPanel.html("");
            });

            // somehow need to trap the actual edit that they make (if any), 
            //  in order to refresh the primary summary

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/getaddtocartjson: " + textStatus);
        },
        beforeSend: function () {
            //console.log("beforesend");
            // add spinner to button
            $button.append('<span id="loadingSpinnerContainer">&nbsp;<i id="loadingSpinner" class="icon-spinner icon-spin"></i></span>');

        },
        complete: function () {
            //console.log("complete");
            // remove spinner
            $('#loadingSpinnerContainer', $button).fadeOut();
            setTimeout(function () { $('#loadingSpinnerContainer', $button).remove(); }, 1500);
        }
    });
}

function RemoveOrder($button, orderId, orderRowId) {
    var message = "";

    var $editPanel = $("#edit-order-form-" + orderId);
    $editPanel.hide("slower");
    $editPanel.html("");

    $.ajax({
        async: true,
        url: "/cart/removeorderjson",
        data: (
        {
            idOrder: orderId,
            idOrderRow: orderRowId
        }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            console.log(data);
            // do something w/ the UI for this order...
            $button.text("Undo");
            $button.toggleClass("btn-success", "btn-danger");

            var $container = $button.closest(".well");
            $container.addClass("removed-order-in-cart");
            $(".edit-order", $container).hide("slower");

            $("#discountCaptionMulti").html(data.discountCaptionMultiMsg);
            $("#grandTotalCaptionMulti").html(data.grandTotalCaptionMultiMsg);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("/account/removeorderjson: " + textStatus + ", " + errorThrown);
        },
        beforeSend: function () {
            //console.log("beforesend");
            // add spinner to button
            //$button.append('<span id="loadingSpinnerContainer">&nbsp;<i id="loadingSpinner" class="icon-spinner icon-spin"></i></span>');
        },
        complete: function () {
            //console.log("complete");
            // remove spinner
            //$('#loadingSpinnerContainer', $button).fadeOut();
            //setTimeout(function () { $('#loadingSpinnerContainer', $button).remove(); }, 1500);
        }
    });
}

function UndoRemoveOrder($button, orderId, orderRowId) {
    var message = "";

    $.ajax({
        async: true,
        url: "/cart/undoremoveorderjson",
        data: (
        {
            idOrder: orderId,
            idOrderRow: orderRowId
        }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            console.log(data);

            // do something w/ the UI for this order...
            $button.text("Remove");
            $button.toggleClass("btn-success", "btn-danger");

            var $container = $button.closest(".well");
            $container.removeClass("removed-order-in-cart");
            $(".edit-order", $container).show("slower");

            $("#discountCaptionMulti").html(data.discountCaptionMultiMsg);
            $("#grandTotalCaptionMulti").html(data.grandTotalCaptionMultiMsg);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("/account/removeorderjson: " + textStatus + ", " + errorThrown);
        },
        beforeSend: function () {
            //console.log("beforesend");
            // add spinner to button
            //$button.append('<span id="loadingSpinnerContainer">&nbsp;<i id="loadingSpinner" class="icon-spinner icon-spin"></i></span>');
        },
        complete: function () {
            //console.log("complete");
            // remove spinner
            //$('#loadingSpinnerContainer', $button).fadeOut();
            //setTimeout(function () { $('#loadingSpinnerContainer', $button).remove(); }, 1500);
        }
    });
}