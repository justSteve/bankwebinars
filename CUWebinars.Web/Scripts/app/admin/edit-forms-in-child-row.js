
function AttachDataTableEditEvents() {

    // user wants to edit a cell
    $('.dataTable').on('click', 'td.details-control', function () {

        // is there already an edit happening?
        if ($("tr.shown").length > 0 &&
            !$(this).hasClass("child-showing")) {
            // could communicate to the user that they need to close the open one...
            return;
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
    });


    $('.dataTable').on("click", "#cancel-changes", function () {
        $(this).parents(".dataTable").find("td.child-showing").click();
    });

    $('.dataTable').on("click", "#save-changes", function () {
        alert("save here...");
    });



    // EditUser_Compact form events
    $('.dataTable').on("click", ".show-billing-address", function () {
        $(".shipping-field").addClass("hidden"); // .hide();
        $(".show-shipping-address").removeClass("btn-success");

        $(".billing-field").removeClass("hidden"); //.show();
        $(".show-billing-address").addClass("btn-success");
    });

    $('.dataTable').on("click", ".show-shipping-address", function () {
        $(".billing-field").addClass("hidden"); //.hide();
        $(".show-billing-address").removeClass("btn-success");

        $(".shipping-field").removeClass("hidden"); //.show();
        $(".show-shipping-address").addClass("btn-success");
    });

}



function createChildRow(cell, $td, rowData) {

    if ($td.hasClass("edit-user-name-email")) {
        return editUserCell(cell, $td, rowData);
    }

    if ($td.hasClass("edit-date")) {
        return editDateCell(cell, $td, rowData);
    }

    return "Row " + cell.index().row + ", Col " + cell.index().column;

}


function editUserCell(cell, $td, rowData) {

    // could definitely use a "busy" cursor.

    var html = "";

    $.ajax({
        async: false,
        url: "/account/geteditusercompactform",
        data: ({ id: rowData.idUser }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        }
    });

    return html;
}


function editDateCell(cell, $td, rowData) {
    // need this in some sort of editable format...  pull from server via ajax?
    //  apply editor plugin?

    return "" +
"<form class='form-inline'>" +
  "<div class='form-group'>" +
    "<label for='orderDate'>Order Date</label>" +
    "<input type='date' class='form-control' id='orderDate' value='" + rowData.OrderDateString + "'>" +
  "</div>" +
  "<button type='submit' class='btn btn-default'>Save</button> <button type='button' class='btn btn-link'>Cancel</button>" +
"</form>"

}





//function createChildRow(rowData)
//{
//// if you return an HTML string that isn't a TR, datatables.net gives you a TD colspan=x, so the child content can have the whole row.
//    return "" +
//"<form class='form-inline'>" +
//  "<div class='form-group'>" +
//    "<label for='exampleInputName2'>First Name</label>" +
//    "<input type='text' class='form-control' id='exampleInputName2' placeholder='Jane' value='" + rowData.FirstName + "'>" +
//  "</div>" +
//  "<div class='form-group'>" +
//    "<label for='exampleInputName2'>Last Name</label>" +
//    "<input type='text' class='form-control' id='exampleInputName2' placeholder='Doe' value='" + rowData.LastName + "'>" +
//  "</div>" +
//  "<div class='form-group'>" +
//    "<label for='exampleInputEmail2'>Email</label>" +
//    "<input type='email' class='form-control' id='exampleInputEmail2' disabled placeholder='jane.doe@example.com' value='" + rowData.BillingEmail + "'>" +
//  "</div>" +
//  "<button type='submit' class='btn btn-default'>Save</button> <button type='button' class='btn btn-link'>Cancel</button>" +
//"</form>"

//// if you return a Row then it nests naturally under the parent.
//    return $(
//        '<tr>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//            '<td>asdf</td>' +
//        '</tr>'
//    );
//}