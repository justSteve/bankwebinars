//var affiliateReportTable;
//var listOfAffiliates = new Array();
//var idWebinar;

var shippingReportTable;
var listOfOrders = new Array();
var idWebinar;



function sendAll() {

    for (var row in listOfOrders) {
        
        setShipped(listOfOrders[row]);
    }
    //alert("Be sure to navigate away from this page before selecting a different event.");
    this.location.href = "/Admin";
}


function initShippingReport(tableId, options) {
    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": true,
        "sDom": '<"top">rt<"bottom"i><"clear">',
        "aaSorting": [[3, "asc"]],
        "bFilter": false,
        "aoColumns": [
            {
                "sWidth": "30%",
                "bSortable": false,
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
                    
                    listOfOrders.push(orderData.Id);

                    return orderData.webinarTitle + ' - ' + orderData.webinarDate +
                        '<br /><a href="/Admin/Registrations/Edit/' + orderData.Id + '"> OrderID: ' + orderData.orderID + '</a>';
                }

            },
            {
                "sWidth": "30%",
                "bSortable": false,
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
                    return orderData.FullName + '<br />' +
                        orderData.Address + '<br />' +
                        orderData.City + ', ' + orderData.State + ' ' + orderData.Zip + '<br />';
                }
            },
            {
                "sWidth": "30%",
                "bSortable": false,
            },
            {
                "sWidth": "5%", "bVisible": false
            },
            {
                "sWidth": "15%",
                "bSortable": false,
                "fnRender": function (obj) {
                    
                    var orderData = obj.aData[obj.IdataColumn];
                    console.log(orderData);
                    if (orderData.shippedDate == '') {
                        return '<form action="/Admin/Reports/SetShipmentDate/" method="post"><input  class=\"pull-right\" id="RecordingShipped" name="RecordingShipped" onclick=setShipped(' + orderData.Id + '); type="button" value="&nbsp;Set to Shipped&nbsp; " />';
                    } else {
                        return "<div class=\"pull-right\">" + orderData.shippedDate + "</div>";
                    }
                }
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
    };

    jQuery.extend(settings, options);
    shippingReportTable = $("#" + tableId).dataTable(settings);

    return shippingReportTable;
}

function getSelectedWebinarID() {
    recordedWebinarControl = $("#ChooseWebinar_RecordedWebinarId");

    webinarID = null;
    if (recordedWebinarControl.attr("value") != "") {
        webinarID = recordedWebinarControl.attr("value");
    }

    return webinarID;
}

function refreshWebinarTitle() {

    webinarID = getSelectedWebinarID();
    if (webinarID != null) {
        
        $.post('/admin/webinars/webinartitle/' + webinarID, function (data) {
            $("#selectedWebinarName").html('<h2>' + data + '</h2>');
        });
        browserTable.fnDraw();
    }
}

function setShipped(row) {

    $.ajax({
            "type": "POST",
            "url": "/Admin/Reports/SetShipmentDate/" + row,
            //data: "webinarID=" + idWebinar + "&affiliateID=" + affiliateID,

            "dataType": "json",
            "success": function (result) {
                
            },
            "error": function(req, status, error) {
                alert("Unable to set Shipped Date. Please contact the administrator.");
            }
        });
}

//function initWebinarRegistrations() {
//    $("img.toggleRegistrations").click(function() {
//        var nTr = this.parentNode.parentNode;
//        if (this.src.match("delete")) {
//            this.src = "/content/images/add.png";
//            shippingReportTable.fnClose(nTr);
//        }
//        else {
//            this.src = "/content/images/delete.png";

//            // getting the affiliate ID from parent row
//            var iIndex = shippingReportTable.fnGetPosition(nTr);
//            var aData = shippingReportTable.fnSettings().aoData[iIndex]._aData;
//            var affiliateID = aData[0];

//            shippingReportTable.fnOpen(nTr, getRegistrationsTableCode(affiliateID), "registrationsTableContainer");
//            shippingReportTable.fnSettings().aoOpenRows.push({
//                "nTr": $(nTr).next()[0],
//                "nParent": nTr
//            });
//            webinarID = getSelectedWebinarID();
//            initShippingTable(affiliateID, webinarID);
//        }
//    });
//}

//function getRegistrationsTableCode(affiliateID) {

//    var template = $("#registrationsTableTemplate")
//        .clone()
//        .attr("id", "registrationsTable" + affiliateID)
//        .show()
//        .outerHTML();

//    return template;
//}

////the subtable is initialized
//function initRegistrationsTable(affiliateID, webinarID) {
//    var settings = {
//        "bProcessing": true,
//        "bSort": true,
//        "bPaginate": false,
//        "bFilter": false,
//        "bLengthChange": false,
//        "sAjaxSource": "/admin/reports/affiliatereportsubtabledata?webinarID=" + webinarID + "&affiliateID=" + affiliateID,
//        "aoColumns": [
//        //No
//            {"bSortable": false,"sWidth": "5%" },
//        //Name
//            {
//            "sWidth": "20%",
//            "fnRender": function (obj) {
//                var orderData = obj.aData[obj.IdataColumn];
//                columnData = orderData.customerName + "<br/>" + orderData.customerEmail;
//                if (orderData.customerInstitution) {
//                    columnData += "<br/>" + orderData.customerInstitution;
//                }
//                return columnData;
//            }
//        },
//        //Order
//            {
//            "sWidth": "20%",
//            "fnRender": function (obj) {
//                var orderData = obj.aData[obj.IdataColumn];
//                return orderData.orderDate + "<br/>" + orderData.registrationType + "<br/>" + orderData.paymentType;
//            }
//        },
//        //Paid
//            {"bSortable": false,"sWidth": "20%" },
//        //Commision
//            {
//            "sWidth": "20%",
//            "bSortable": false,
//            "fnRender": function (obj) {
//                var orderData = obj.aData[obj.IdataColumn];
//                return orderData.commission; // + "<br/>" + orderData.commissionModel;
//            }
//        },
//        //Status
//            {"bSortable": false,"sWidth": "20%" }
//        ]
//    };
//    $("#registrationsTable" + affiliateID).dataTable(settings);
//}

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
