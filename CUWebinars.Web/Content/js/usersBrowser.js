var usersBrowserTable;

function initUsersBrowser(tableId, options) {

    // default settings, will be overridden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bSort": true,
        "bStateSave": false,
        "fnSetFilteringDelay": 1750,
        "aaSorting": [[1, "asc"]],
        "aoColumns": [
        //ID
            {
                "sWidth": "10%", "bSortable": true
                ,
                "fnRender": function (obj) {
                    return obj.aData[0] + "<br><small> <a href=\"/account/edit/" + obj.aData[0] + "\" target=\"_new\" >[Edit]</a></small>";
                }
            },
        //Name
            {
                "bSortable": true,
                "sWidth": "55%",
                "fnRender": function (obj) {
                    return obj.aData[1].LastName + ", " + obj.aData[1].FirstName +
                               "<a href='mailto:" + obj.aData[1].Email + "'>  " + obj.aData[1].Email + "</a> <br />" +
                               obj.aData[1].Institution + "<br>" + obj.aData[1].NumDiscounts;
                }
                // - <a href='/account/takecontrol/" + obj.aData[0] + "' target='_new' >[Function As This User]</a>
            },
        //phone
            { "sWidth": "15%", "bSortable": false },
        //Affiliate
            { "sWidth": "15%", "bSortable": true, "bVisible": showIfAdmin },
        //DateCreated
            { "sWidth": "15%", "bSortable": false },
        //Registrations
            {
                "sWidth": "5%",
                "fnRender": function (obj) {
                    var html = "<small>"+ obj.aData[obj.IdataColumn] +"</small>";
                    //console.dir(obj);
                    //if (obj.aData[obj.IdataColumn] != "0")
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
        //console.dir(this.parentNode.parentNode);
        
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
            var userID = aData[0].split('<')[0];

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
//    "bProcessing": true,
//"bSort": true,
//"bFilter": false,
//"bLengthChange": false,
//"bStateSave": false,
    var settings = {
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bProcessing": true,
        "bStateSave": false,
        "bSort": true,
        "aaSorting": [[1, "asc"]],
        "sAjaxSource": "/admin/registrations/registrationssubtabledatabyuser/" + userID,
        "aoColumns": [
        //Column1: order data
            {
                "bSortable": true, "sWidth": "10%",
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
                    return "<small>"+ orderData.orderId + " <br>via: " + orderData.Origin+ "</small>";
                }
            },

        //Column2: event link
            {
                "bSortable": true, "sWidth": "45%"
            },

        //Column3: registration type 
            {
                "bSortable": true, "sWidth": "15%"
            },

        //Column3.5: Order Affiliate
            { "bSortable": true, "bVisible": showIfAdmin, "sWidth": "10%" },

        //Column4: order date
            { "bSortable": true, "sWidth": "5%" },

        //Column5:  edit and resend links
            {
                "bSortable": false,
                "sWidth": "10%"
            },
        //Column6: status
        {
            "bSortable": false
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
