var shippingReportTable;

function initShippingReport(tableId, options) {
    // default settings, will be overriden by values from options
    var settings = {
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": true,
        "aaSorting": [[3, "asc"]],
        "bFilter": false,
        "aoColumns": [
            {
                "sWidth": "40%",
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
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
                "sWidth": "5%"
            },
            {
                "sWidth": "5%",
                "bSortable": false,
                "fnRender": function (obj) {
                    var orderData = obj.aData[obj.IdataColumn];
                    return '<form action="/Admin/Reports/SetShipmentDate/' + orderData.Id + '" method="post"><input id="RecordingShipped" name="RecordingShipped" type="submit" value="Recording Shipped" />';
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
