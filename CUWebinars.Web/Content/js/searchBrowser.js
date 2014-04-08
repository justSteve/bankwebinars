var searchBrowserTable;
var presenters = {};

function initSearchBrowser(tableId, options) {
    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        
        "bServerSide": true,
        "bStateSave": false,
        "aaSorting": [[2, "asc"]],
        "aoColumns": [
            { "sWidth": "5%" },
            { "sWidth": "65%", "bSortable": false },
            {
                "sWidth": "5%", "bVisible": false,
                //"fnRender": function (obj) {
                //    var html = obj.aData[obj.IdataColumn];
                //    //presenters.push[obj.aData[obj.IdataColumn]]
                //},
            },
            { "sWidth": "15%" },
            {
                "sWidth": "5%",
                //"fnRender": function (obj) {
                //    var html = obj.aData[obj.IdataColumn];
                //    //console.dir(obj);

                //},
                "bSortable": false
            }
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
        "fnSetFilteringDelay": 1000,
        "fnDrawCallback": function () {
            //fillPresenters();
        }
    };

    jQuery.extend(settings, options);
    searchBrowserTable = $("#" + tableId).dataTable(settings).fnSetFilteringDelay();

    return searchBrowserTable;
}
$(document).ready(function () {

});

var isStaticWebinarsGrid = false;

function initWebinarsGrid(isStatic) {
    isStaticWebinarsGrid = isStatic;

    fillPresenters();

    $("#filterPresenterName").change(function () {
        applyFilters();
    });

    $("#filterTitle").keyup(function () {
        applyFilters();
    });

    $("#filterUpcoming, #filterRecorded").click(function () {
        applyFilters();
    });

    //if (!isStaticWebinarsGrid) {
    //    fillWebinarsTable();
    //}
}

//function fillWebinarsTable() {
//    $("tr[id^=webinarRow]").remove();

//    var html = "";
//    for (var i = 0; i < allWebinars.length; i++) {
//        html +=
//            "<tr id=\"webinarRow" + allWebinars[i].Id + "\"> \
//					<td>" + allWebinars[i].Date + "</td> \
//					<td>" + allWebinars[i].Title + "</td> \
//					<td>" + allWebinars[i].PresenterName + "</td> \
//					<td>" + allWebinars[i].ShortDescription + "</td> \
//					<td>" + allWebinars[i].TopicTags.join(", ") + "</td> \
//				</tr>";
//    }
//    $("#webinarsTable").append(html);
//}

function filterPresenterName(webinar) {
    if (!$("#filterPresenterName").val()) {
        return true;
    }
    return webinar.PresenterName == $("#filterPresenterName").val();
}

function filterTitle(webinar) {
    if ($("#filterTitle").val().length < 2) {
        return true;
    }
    return webinar.Title.toLowerCase().indexOf($("#filterTitle").val().toLowerCase()) != -1;
}

function filterUpcoming(webinar) {
    if ($("#filterUpcoming").attr("checked") === $("#filterRecorded").attr("checked")) {
        return true;
    }

    return (webinar.IsUpcoming && $("#filterUpcoming").attr("checked")) || (!webinar.IsUpcoming && $("#filterRecorded").attr("checked"));
}

function applyFilters() {
    //for (var i = 0; i < allWebinars.length; i++) {
    //    var allowed = filterPresenterName(allWebinars[i]) && filterTitle(allWebinars[i]) && filterUpcoming(allWebinars[i]);

    //    if (allowed) {
    //        $("#webinarRow" + allWebinars[i].Id).show();
    //    } else {
    //        $("#webinarRow" + allWebinars[i].Id).hide();
    //    }
    //}
}

function fillPresenters() {

    for (var presenterName in presenters) {
        $("#filterPresenterName")
            .append("<RegType value='" + presenterName + "'>" + presenterName + "</RegType>");
    }
}


// one-line, but useful jQuery plugin :)
jQuery.fn.outerHTML = function () {
    return $('<div>').append(this.eq(0).clone()).html();
};

