var webinarsCalendarTable;

function initWebinarsCalendar(tableId, options) {
    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": false,
        "aaSorting": [[2, "asc"]],
        "aoColumns": [
            { "sWidth": "5%" },
            { "sWidth": "50%", "bSortable": true },
            { "sWidth": "10%" },
            { "sWidth": "25%" },
            { "bVisible": false }
        ],
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.ajax({
                "dataType": "json",
                "type": "POST",
                "url": sSource,
                "data": aoData,
                "success": fnCallback
            });
        },
        "fnDrawCallback": function () {
            //initWebinarsBrowserRegistrations();
        }
    };

    jQuery.extend(settings, options);
    webinarsCalendarTable = $("#" + tableId).dataTable(settings);

    return webinarsCalendarTable;
}

// one-line, but useful jQuery plugin :)
jQuery.fn.outerHTML = function () {
    return $('<div>').append(this.eq(0).clone()).html();
};
