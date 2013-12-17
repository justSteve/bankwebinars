var affiliateReportTable;
var listOfAffiliates = new Array();
var idWebinar;

function sendAll() {

    for (var Aff in listOfAffiliates) {
        sendReport(idWebinar, listOfAffiliates[Aff]);
        
    }
    //alert("Be sure to navigate away from this page before selecting a different event.");
    this.location.href = "/Admin";
}


function initAffiliateReport(tableId, options) {
    //a bug exists in the list of affiliates.
    //
    //The array will not re-initalize without a full, complete page restore.
    //


    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        "bFilter": false,
        "bPaginate": false,
        "bServerSide": true,
        "bStateSave": true,
        "aoColumns": [
            { "sWidth": "5%", "bSortable": false },
            {
                "bVisible": false,
                "bSortable": false, "sWidth": "5%"
            },
            { "sWidth": "45%", "bSortable": false },
            { "sWidth": "10%", "bSortable": false },
            { "sWidth": "25%", "bSortable": false },
            {
                "sWidth": "5%",
                "fnRender": function (obj) {
                    var html = obj.aData[obj.iDataColumn];
                    if (obj.aData[obj.iDataColumn] != "0")
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
            initAffiliateRegistrations();
        }
    };

    jQuery.extend(settings, options);
    affiliateReportTable = $("#" + tableId).dataTable(settings);

    return affiliateReportTable;
}

function getSelectedWebinarID() {
    upcomingWebinarControl = $("#ChooseWebinar_UpcomingWebinarId");
    recordedWebinarControl = $("#ChooseWebinar_RecordedWebinarId");

    webinarID = null;
    if (upcomingWebinarControl.attr("value") != "") {
        webinarID = upcomingWebinarControl.attr("value");
    } else if (recordedWebinarControl.attr("value") != "") {
        webinarID = recordedWebinarControl.attr("value");
    }

    return webinarID;
}

function refreshWebinarTitle() {
    webinarID = getSelectedWebinarID();
    if (webinarID != null) {
        $.post('/admin/webinars/webinartitle/' + webinarID, function(data) {
            $("#selectedWebinarName").html('<h2>' + data + '</h2>');
        });
    }
}

function sendReport(webinarID, affiliateID) {
//    var answer = confirm("Do you want to send the report to the affiliate?")
//    if (!answer) {
//        return;
//    }
    
    $.ajax(
        {
            type: "POST",
            url: "/Admin/Reports/SendAffiliateReport",
            data: "webinarID=" + webinarID + "&affiliateID=" + affiliateID,
            cache: false,
            async: false,
            success: function(result) {
                alert("Report sent to the affiliate #: "+affiliateID);
            },
            error: function(req, status, error) {
                alert("Unable to send the report to the affiliate. Please contact the administrator.");
            }
        });
}

function initAffiliateRegistrations() {
    $("img.toggleRegistrations").click(function() {
        var nTr = this.parentNode.parentNode;
        if (this.src.match("delete")) {
            this.src = "/content/images/add.png";
            affiliateReportTable.fnClose(nTr);
        }
        else {
            this.src = "/content/images/delete.png";

            // getting the affiliate ID from parent row
            var iIndex = affiliateReportTable.fnGetPosition(nTr);
            var aData = affiliateReportTable.fnSettings().aoData[iIndex]._aData;
            var affiliateID = aData[0];

            affiliateReportTable.fnOpen(nTr, getRegistrationsTableCode(affiliateID), "registrationsTableContainer");
            affiliateReportTable.fnSettings().aoOpenRows.push({
                "nTr": $(nTr).next()[0],
                "nParent": nTr
            });
            webinarID = getSelectedWebinarID();
            initRegistrationsTable(affiliateID, webinarID);
        }
    });
}

function getRegistrationsTableCode(affiliateID) {

    var template = $("#registrationsTableTemplate")
        .clone()
        .attr("id", "registrationsTable" + affiliateID)
        .show()
        .outerHTML();

    return template;
}

//the subtable is initialized
function initRegistrationsTable(affiliateID, webinarID) {
    var settings = {
        "bProcessing": true,
        "bSort": true,
        "bPaginate": false,
        "bFilter": false,
        "bLengthChange": false,
        "sAjaxSource": "/admin/reports/affiliatereportsubtabledata?webinarID=" + webinarID + "&affiliateID=" + affiliateID,
        "aoColumns": [
        //No
            {"bSortable": false,"sWidth": "5%" },
        //Name
            {
            "sWidth": "20%",
            "fnRender": function (obj) {
                var orderData = obj.aData[obj.iDataColumn];
                columnData = orderData.customerName + "<br/>" + orderData.customerEmail;
                if (orderData.customerInstitution) {
                    columnData += "<br/>" + orderData.customerInstitution;
                }
                return columnData;
            }
        },
        //Order
            {
            "sWidth": "20%",
            "fnRender": function (obj) {
                var orderData = obj.aData[obj.iDataColumn];
                return orderData.orderDate + "<br/>" + orderData.registrationType + "<br/>" + orderData.paymentType;
            }
        },
        //Paid
            {"bSortable": false,"sWidth": "20%" },
        //Commision
            {
            "sWidth": "20%",
            "bSortable": false,
            "fnRender": function (obj) {
                var orderData = obj.aData[obj.iDataColumn];
                return orderData.commission; // + "<br/>" + orderData.commissionModel;
            }
        },
        //Status
            {"bSortable": false,"sWidth": "20%" }
        ]
    };

    $("#registrationsTable" + affiliateID).dataTable(settings);
}

function resendConnectionInfo(link) {
    $.post($(link).attr("href"), function(data, status) {
        alert("Connection info was resent successfully");
    });

    return false;
}

// one-line, but useful jQuery plugin :)
jQuery.fn.outerHTML = function() {
    return $('<div>').append(this.eq(0).clone()).html();
};
