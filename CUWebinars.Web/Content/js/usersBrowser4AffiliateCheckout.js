var usersBrowserTable;

function initUsersBrowser(tableId, options) {

    // default settings, will be overridden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bSort": true,
        "bStateSave": true,
        "fnSetFilteringDelay": 750,
        "aaSorting": [[1, "asc"]],
        "aoColumns": [
            { "sWidth": "5%", "bSortable": false },
            { "bSortable": true,
                "sWidth": "60%",
                "fnRender": function (obj) {
                    return obj.aData[1].LastName + ", " + obj.aData[1].FirstName +
                           "<a href='mailto:" + obj.aData[1].Email + "'> [Email]</a> " + obj.aData[1].Email + " <br />" +
                           obj.aData[1].Institution + " <a href='/account/edit/" + obj.aData[0] + "' target='_new' >[Edit User]</a> - <a href='/account/takecontrol/" + obj.aData[0] + "' target='_new' >[Function As This User]</a>";
                }
            },
            { "sWidth": "15%", "bSortable": false },
            { "sWidth": "15%" },
            {
                "sWidth": "5%",
                "fnRender": function (obj) {
                    var html = obj.aData[obj.IdataColumn];
                    ////console.dir(obj);
                    if (obj.aData[obj.IdataColumn] != "0")
                        html += "&nbsp;<img src='/content/images/add.png' class='toggleRegistrations' id='toggleRegistrations" + obj.aData[0] + "' />";

                    return html;
                },
                "bSortable": false
            }
        ],

        "fnServerData": function (sSource, aoData, fnCallback) {
            //alert("sSource" + sSource);
            $.ajax({
                "dataType": "json",
                "type": "POST",
                "url": sSource,
                "data": aoData,
                "success": fnCallback
            });
        },
        "fnDrawCallback": function () {
            initUsersBrowserRegistrations();
        }
    };


    jQuery.extend(settings, options);
    usersBrowserTable = $("#" + tableId).dataTable(settings);

    return usersBrowserTable;
}

function initUsersBrowserRegistrations() {
    $("img.toggleRegistrations").click(function () {
        ////console.dir(this.parentNode.parentNode);

        var nTr = this.parentNode.parentNode;
        if (this.src.match("delete")) {
            this.src = "/content/images/add.png";
            usersBrowserTable.fnClose(nTr);
        }
        else {
            this.src = "/content/images/delete.png";

            // getting the user ID from parent row
            var iIndex = usersBrowserTable.fnGetPosition(nTr);
            var aData = usersBrowserTable.fnSettings().aoData[iIndex]._aData;
            var userID = aData[0];

            usersBrowserTable.fnOpen(nTr, getRegistrationsTableCode(userID), "registrationsTableContainer");
            usersBrowserTable.fnSettings().aoOpenRows.push({
                "nTr": $(nTr).next()[0],
                "nParent": nTr
            });
            initRegistrationsTable(userID);
        }
    });

}

function getRegistrationsTableCode(userID) {
    //alert(userID);
    var template = $("#registrationsTableTemplate")
        .clone()
        .attr("id", "registrationsTable" + userID)
        .show()
        .outerHTML();

    return template;
}

function initRegistrationsTable(userID) {

    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bSort": true,
        "aaSorting": [[1, "asc"]],
        "sAjaxSource": "/admin/registrations/registrationssubtabledatabyuser/" + userID,
        "aoColumns": [
        //Column1: order data
            {
            "bSortable": true, "sWidth": "15%",
            "fnRender": function (obj) {
                var orderData = obj.aData[obj.IdataColumn];
                return orderData.orderId + " <br>Placed by: " + orderData.initiatedBy;
            }
        },

        //Column2: event link
            {
            "bSortable": true, "sWidth": "45%",
            "fnRender": function (obj) {
                var eventData = obj.aData[obj.IdataColumn];
                return eventData.eventdate + " <BR> " + eventData.title;
            }
        },

        //Column3: registration type 
            {
            "bSortable": true, "sWidth": "15%",
            "fnRender": function (obj) {
                var eventData = obj.aData[obj.IdataColumn];
                return eventData.regType + " | " + eventData.paymentAmt;
            }
        },

        //Column4: order date
            {"bSortable": false, "sWidth": "10%" },

        //Column5:  edit and resend links
            {"bSortable": true,
            "sWidth": "10%"
        },
        //Column6: status
        {
        "bSortable": true
            , "sWidth": "10%"

    }

        ]
};

$("#registrationsTable" + userID).dataTable(settings);
}
function resendConnectionInfo(link) {
    $.post($(link).attr("href"), function (data, status) {
        alert("Connection info was resent successfully");
    });

    return false;
}

// one-line, but useful jQuery plugin :)
jQuery.fn.outerHTML = function () {
    return $('<div>').append(this.eq(0).clone()).html();
};