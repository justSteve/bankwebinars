var webinarsBrowserTable;

function initWebinarsBrowser(tableId, options) {
    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": false,
        "aaSorting": [[2, "asc"]],
        "aoColumns": [
            { "sWidth": "5%" },
            { "sWidth": "50%", "bSortable": false },
            { "sWidth": "10%" },
            { "sWidth": "25%" },
            {
                "sWidth": "5%",
                "fnRender": function (obj) {
                    var html = obj.aData[obj.IdataColumn];
                    //console.dir(obj);
                    if (obj.aData[obj.IdataColumn] != "0")
                        html += "&nbsp;<img src='/content/images/add.png' class='toggleRegistrations' id='toggleRegistrations" + obj.aData[0] + "' />";

                    return html;
                },
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
        "fnDrawCallback": function () {
            initWebinarsBrowserRegistrations();
        }
    };

    jQuery.extend(settings, options);
    webinarsBrowserTable = $("#" + tableId).dataTable(settings);

    return webinarsBrowserTable;
}

function initWebinarsBrowserRegistrations() {
    $("img.toggleRegistrations").click(function () {
        //console.dir(this.parentNode.parentNode);
        var nTr = this.parentNode.parentNode;
        if (this.src.match("delete")) {
            this.src = "/content/images/add.png";
            webinarsBrowserTable.fnClose(nTr);
        }
        else {
            this.src = "/content/images/delete.png";

            // getting the webinar ID from parent row
            var iIndex = webinarsBrowserTable.fnGetPosition(nTr);
            var aData = webinarsBrowserTable.fnSettings().aoData[iIndex]._aData;
            var webinarID = aData[0];

            webinarsBrowserTable.fnOpen(nTr, getRegistrationsTableCode(webinarID), "registrationsTableContainer");
            webinarsBrowserTable.fnSettings().aoOpenRows.push({
                "nTr": $(nTr).next()[0],
                "nParent": nTr
            });
            initRegistrationsTable(webinarID);
        }
    });
}

function getRegistrationsTableCode(webinarID) {

    var template = $("#registrationsTableTemplate")
        .clone()
        .attr("id", "registrationsTable" + webinarID)
        .show()
        .outerHTML();

    return template;
}

//the subtable is initialized
function initRegistrationsTable(webinarID) {

    var settings = {
        "bProcessing": true,
        "bSort": true,
        "bFilter": false,
        "bLengthChange": false,
        "bStateSave": false,
        "aaSorting": [[3, "asc"]],
        "sAjaxSource": "/admin/registrations/registrationsbrowserSubtabledata/" + webinarID,
        "aoColumns": [
        //order data
            {
                "sWidth": "10%",
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
                    return "<small>" + orderData.orderId + " <br>via: " + orderData.initiatedBy + "</small>";
                }
            },

        //user link
            {
                "sWidth": "25%",
                "aaSorting": [[2, "asc"]],
                "fnRender": function (obj) {
                    var userData = obj.aData[obj.IdataColumn];
                    return "<div style='display:none;'>" + userData.userName + "</div><a href='/account/edit/" + userData.userId + "' target='_blank'>"
                                + userData.userName
                                + "</a>";
                }
            },

        //registration type
            { "sWidth": "25%" },

        //order date
            { "sWidth": "10%" },
        //Affiliate
            { "sWidth": "10%", "bVisible": showAffiliateColumn },

        // edit and resend links
            { "sWidth": "10%" },

        //Column6: status
            {
                "bSortable": false
                , "sWidth": "10%"
            }

        ]
    };
    $("#registrationsTable" + webinarID).dataTable(settings);
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

function confirmWebinarDelete(webinarTitle, webinarID) {
    var answer = confirm('Do you really want to delete the webinar titled ' + webinarTitle);
    if (answer) {
        document.forms['delete' + webinarID].submit();
    }
}

function cloneWebinar() {
    var webinarID = $("#cloneWebinarID").val();

    showProcessingIndicator();

    $.ajax(
        {
            type: "POST",
            url: "/Admin/Webinars/Clone",
            data: "ID=" + webinarID,
            cache: false,
            async: false,
            success: function (result) {
                $("#webinarForm").html(result);
                hideProcessingIndicator();
            },
            error: function (req, status, error) {
                hideProcessingIndicator();
                alert("Unable to clone the webinar with ID=" + webinarID);
            }
        });
}