var DiscountReportTable;

function initDiscountReport(tableId, options) {

    // default settings, will be overriden by values from options
    var settings = {
        "oLanguage": { "sInfoFiltered": " - filtering from _MAX_ records" },
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": true,
        "aaSorting": [[3, "asc"]],
        "bFilter": true,
        "fnDrawCallback": function (oSettings) {
            SetColumnHeaders();
        },
        "aoColumns": [
            {
                "sWidth": "10%", "bSortable": false
            },
            {
                "sWidth": "30%", "bSortable": false
            },
            {
                "sWidth": "10%"
            },
            {
                "sWidth": "30%", "bSortable": false
            }
        ],

    };

    jQuery.extend(settings, options);
    //    *      $('#example').dataTable( {

    //*      } );
    DiscountReportTable = $("#" + tableId).dataTable(settings).fnSetFilteringDelay(2000);;
    return DiscountReportTable;
}

function SetColumnHeaders() {

    if (typeOfSubscription == 1 || typeOfSubscription == 2 || typeOfSubscription == 3 || typeOfSubscription == 4) {


        $("#col1").text("Action");
        $("#col2").text("Found In Orders");
        $("#col3").text("Found By Users");
        $("#col4").text("Notes");
    };

    if (typeOfSubscription > 4) {
        $("#col1").text("Action");
        $("#col2").text("Assigned to Order");
        $("#col3").text("Expires");
        $("#col4").text("Notes");
    }

}