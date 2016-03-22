
function AttachDataTableWebinarEvents() {

    // user wants to edit a cell
    $('.dataTable').on('click', 'td.details-control', function () {
        
        // check to see if there is already an edit in progress.  we are going to close it so they 
        //  can browse around, when they re-open it though, it will re-populate the form fields from the server,
        //  any unsaved changes will be "lost".
        if ($("tr.shown").length > 0 &&
            !$(this).hasClass("child-showing")) {

            $(this).parents(".dataTable").find("td.child-showing").click();

        }

        var table = $('.dataTable').DataTable();
        var tr = $(this).parents('tr');
        var row = table.row(tr);

        var cell = table.cell($(this));
        var $td = $(cell.node());

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $("td", tr).removeClass("child-showing");
        } else {
            row.child(createChildRow(cell, $td, row.data()), "child-row").show();
            $td.addClass("child-showing");
            tr.addClass('shown');
        }
        //if (row.child.isShown()) {
        //    row.child(createChildRow(cell, $td, row.data()), "child-row").show();
        //    $td.addClass("child-showing");
        //    tr.addClass('shown'); 
        //} else {
        //    row.child.hide();
        //    tr.removeClass('shown');
        //    $("td", tr).removeClass("child-showing");
        //}
    });

    $('.dataTable').on("click", "#cancel-changes", function (e) {
        e.preventDefault();
        $(this).parents(".dataTable").find("td.child-showing").click();
    });


    // grid events handers


    // sample from Edit_compact form events
    $('.dataTable').on("click", "#listOfRegTypes li", function (e) { // EDIT.listOfRegTypes (not defined yet)
        e.preventDefault();

        // could refactor into the NS module pattern
        // EDIT.changeRegType(e, $(this), $(this).parent());
        updateRegType(this, $('input[name="OrderRowId"]').val(), $(this).val());

    });

}

function fireSuccessIndicator($cell) {
    $cell.addClass("success"); // has a background color specified
    setTimeout(function () {
        $cell.addClass("save-bg-transition"); // specifies an ease effect so the next line "fades" back to normal
        $cell.removeClass("success"); // remove the customized background color
        setTimeout(function () {
            $cell.removeClass("save-bg-transition"); // reset this so next color change doesn't fade in
        }, 3000); // this should be at least as long as the duration of the animation specified in the css
    }, 1500); // wait this long prior to removing the background-color from the cell
}

// set removeAfter (milliseconds) to -1 to skip 'removing' logic (allowing you to do it manually as needed)
function addIsLoadingIndicator($cell, removeAfter) {
    $cell.addClass("loading"); // has a background color, etc specified
    if (removeAfter != -1) {
        setTimeout(function () {
            removeIsLoadingIndicator($cell);
        }, removeAfter);
    }
}

function removeIsLoadingIndicator($cell) {
    $cell.addClass("loading-bg-transition"); // specifies an ease effect so the next line "fades" back to normal
    $cell.removeClass("loading"); // remove the customized background color
    setTimeout(function () {
        $cell.removeClass("loading-bg-transition"); // reset this so next color change doesn't fade in
    }, 1500); // this should be at least as long as the duration of the animation specified in the css
}


function createChildRow(cell, $td, rowData) {

    if ($td.hasClass("wDate")) {
        return wDateCell(cell, $td, rowData);
    }  //

    if ($td.hasClass("wTitle")) {
        return wTitleCell(cell, $td, rowData);
    }

    if ($td.hasClass("presenter")) {
        return presenterCell(cell, $td, rowData);
    }

    if ($td.hasClass("topicsTitles")) {
        return topicsTitlesCell(cell, $td, rowData);
    }

    if ($td.hasClass("orders")) {
        return ordersCell(cell, $td, rowData);
    }

    // doing the order status dropdown "live" on the parent row
    //if ($td.hasClass("edit-order-status")) {
    //    return editOrderStatusCell(cell, $td, rowData);
    //}

    return "Row " + cell.index().row + ", Col " + cell.index().column;

}


function ordersCell(cell, $td, rowData) {
    var html = rowData;
    console.log(rowData);
    $.ajax({
        async: false,
        url: "/admin/getorderscompact",
        data: ({ idWebinar: rowData.idWebinar }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/getorderscompact: " + textStatus);
            $zopim.livechat.addTags(errorThrown);


        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
            $zopim.livechat.addTags("editing: " + rowData.idUser);

            L.clientLogger.appendNotes('editing', { 'responseObject': XMLHttpRequest.responseJSON, 'geteditusercompactform': rowData.idUser });
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}

function topicsTitlesCell(cell, $td, rowData) {

    var html = "";

    //$.ajax({
    //    async: false,
    //    url: "/account/GetEditBillingForm",
    //    data: ({ orderId: rowData.idOrder }),
    //    dataType: "json",
    //    type: "POST",
    //    success: function (data) {
    //        html = data.html;
    //    },
    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
    //        alert("/account/GetEditBillingForm idOrder=" + rowData.idOrder + textStatus);
    //    },
    //    beforeSend: function () {
    //        addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
    //    },
    //    complete: function () {
    //        removeIsLoadingIndicator($td);
    //    }
    //});

    return html;
}

function presenterCell(cell, $td, rowData) {

    var html = "";

    $.ajax({
        async: false,
        url: "/webinar/GetPresenterCompact",
        data: ({ id: rowData.idWebinar }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/GetPresenterCompact: " + errorThrown);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}

function wTitleCell(cell, $td, rowData) {

    var html = "";
    
    $.ajax({
        async: false,
        url: "/webinar/getwebinardetailscompact",
        data: ({ id: rowData.idWebinar}),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/GetResendInfoForm: " + textStatus);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}

function wDateCell(cell, $td, rowData) {

    var html = "";

    $.ajax({
        async: false,
        url: "/account/GetResendInfoForm",
        data: ({ orderId: rowData.idOrder }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("/account/GetResendInfoForm: " + textStatus);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}
