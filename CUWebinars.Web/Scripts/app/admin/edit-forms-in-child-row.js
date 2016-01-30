
function AttachDataTableEditEvents() {

    // user wants to edit a cell
    $('.dataTable').on('click', 'td.details-control', function () {

        // check to see if there is already an edit in progress.  we are going to close it so they 
        //  can browse around, when they re-open it though, it will re-populate the form fields from the server,
        //  any unsaved changes will be "lost".
        if ($("tr.shown").length > 0 &&
            !$(this).hasClass("child-showing")) {

            $(this).parents(".dataTable").find("td.child-showing").click();

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

    $('.dataTable').on("click", "#cancel-changes", function (e) {
        e.preventDefault();
        $(this).parents(".dataTable").find("td.child-showing").click();
    });





    // EditUser_Compact form events
    $('.dataTable').on("click", ".show-billing-address", function (e) {
        e.preventDefault();

        $(".shipping-field").addClass("hidden"); // .hide();
        $(".show-shipping-address").removeClass("btn-success");

        $(".billing-field").removeClass("hidden"); //.show();
        $(".show-billing-address").addClass("btn-success");
    });


    $('.dataTable').on("click", ".show-shipping-address", function (e) {
        e.preventDefault();

        $(".billing-field").addClass("hidden"); //.hide();
        $(".show-billing-address").removeClass("btn-success");

        $(".shipping-field").removeClass("hidden"); //.show();
        $(".show-shipping-address").addClass("btn-success");
    });



    $('.dataTable').on("click", "#save-changes-user", function (e) {
        e.preventDefault();

        var form = $(this).parents("form");
        var $form = $(form);

        // configure the current validator to validate hidden form elements
        $.validator.unobtrusive.parse($form);
        $form.validate().settings.ignore = []; // so it doens't "ignore" .hidden fields

        // check to see if the form is invalid
        if (!$form.valid()) {
            // are the invalid fields on the hidden panel? flash the button (or something!)
            if ($(".hidden .input-validation-error").length) {
                if ($(".show-billing-address.btn-success").length) {
                    $(".show-shipping-address").addClass("btn-danger");
                    setTimeout(function () {
                        $(".show-shipping-address").removeClass("btn-danger");
                    }, 2000);
                }

                if ($(".show-shipping-address.btn-success").length) {
                    $(".show-billing-address").addClass("btn-danger");
                    setTimeout(function () {
                        $(".show-billing-address").removeClass("btn-danger");
                    }, 2000);
                }
            } else {
                return false; // do not allow form to submit if it is invalid
            }
        }


        // form validation passed, save edited User fields to the database...
        // could definitely use a "busy" cursor.

        // the EditUser_Compact screen uses EditUserInfoModel directly, but we post
        //  it to the Account/EditUser Action as an EditUserViewModel.  both models contain an EditFields object,
        //  which is the actual representation of the form, and MVC lines the sub-objects up as it deserializes it

        var data = $form.serialize();
        $.ajax({
            async: false,
            url: "/account/updateuser",
            data: data,
            dataType: "json",
            type: "POST",
            success: function (data) {
                console.log(data);

                // need to update the currently displaying name (in case it changed)

                var $cell = $("td.child-showing");
                $cell.click(); // hide the child row
                fireSuccessIndicator($cell);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });


    $('.dataTable').on("click", "#save-changes-discount", function (e) {
        e.preventDefault();

        var form = $(this).parents("form");
        var $form = $(form);

        // configure the current validator to validate hidden form elements
        $.validator.unobtrusive.parse($form);
        $form.validate().settings.ignore = []; // so it doens't "ignore" .hidden fields

        // check to see if the form is invalid
        if (!$form.valid()) {
            // are the invalid fields on the hidden panel? flash the button (or something!)
            if ($(".hidden .input-validation-error").length) {
                var a = "holder";
            } else {
                return false; // do not allow form to submit if it is invalid
            }
        }


        // form validation passed, save edited Discount fields to the database...
        // could definitely use a "busy" cursor.

        var data = $form.serialize();
        $.ajax({
            async: false,
            url: "/account/updatediscount",
            data: data,
            dataType: "json",
            type: "POST",
            success: function (data) {
                console.log(data);

                // need to update the currently displaying name (in case it changed)

                var $cell = $("td.child-showing");
                $cell.click(); // hide the child row
                fireSuccessIndicator($cell);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });
    

    $('.dataTable').on("click", "#save-changes-institution", function (e) {
        e.preventDefault();

        var form = $(this).parents("form");
        var $form = $(form);

        // configure the current validator to validate hidden form elements
        $.validator.unobtrusive.parse($form);
        $form.validate().settings.ignore = []; // so it doens't "ignore" .hidden fields

        // check to see if the form is invalid
        if (!$form.valid()) {
            // are the invalid fields on the hidden panel? flash the button (or something!)
            if ($(".hidden .input-validation-error").length) {
                // add check to ensure zip code matches city/state
            } else {
                return false; // do not allow form to submit if it is invalid
            }
        }

        // could definitely use a "busy" cursor.

        var data = $form.serialize();
        $.ajax({
            async: false,
            url: "/account/updateinstitution",
            data: data,
            dataType: "json",
            type: "POST",
            success: function (data) {
                console.log(data);

                // need to update the currently displaying name (in case it changed)

                var $cell = $("td.child-showing");
                $cell.click(); // hide the child row
                fireSuccessIndicator($cell);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });

    // EditBilling_Compact form events
    $('.dataTable').on("click", "#listOfRegTypes li", function (e) { // EDIT.listOfRegTypes (not defined yet)
        e.preventDefault();

        // could refactor into the NS module pattern
        // EDIT.changeRegType(e, $(this), $(this).parent());
        updateRegType(this, $('input[name="OrderRowId"]').val(), $(this).val());

    });

}

function fireSuccessIndicator($cell) {
    $cell.addClass("success"); // has a background color specified
    setTimeout(function () {
        $cell.addClass("save-bg-transition"); // specifies an ease effect so the next line "fades" back to normal
        $cell.removeClass("success"); // remove the customized background color
        setTimeout(function () {
            $cell.removeClass("save-bg-transition"); // reset this so next color change doesn't fade in
        }, 3000); // this should be at least as long as the duration of the animation specified in the css
    }, 1500); // wait this long prior to removing the background-color from the cell
}

// set removeAfter (milliseconds) to -1 to skip 'removing' logic (allowing you to do it manually as needed)
function addIsLoadingIndicator($cell, removeAfter) {
    $cell.addClass("loading"); // has a background color, etc specified
    if (removeAfter != -1) {
        setTimeout(function () {
            removeIsLoadingIndicator($cell);
        }, removeAfter);
    }
}

function removeIsLoadingIndicator($cell) {
    $cell.addClass("loading-bg-transition"); // specifies an ease effect so the next line "fades" back to normal
    $cell.removeClass("loading"); // remove the customized background color
    setTimeout(function () {
        $cell.removeClass("loading-bg-transition"); // reset this so next color change doesn't fade in
    }, 1500); // this should be at least as long as the duration of the animation specified in the css
}


function createChildRow(cell, $td, rowData) {

    if ($td.hasClass("edit-user-name-email")) {
        return editUserCell(cell, $td, rowData);
    }  //

    if ($td.hasClass("edit-institution")) {
        return editInstitutionCell(cell, $td, rowData);
    }

    if ($td.hasClass("edit-billing")) {
        return editBillingCell(cell, $td, rowData);
    }

    if ($td.hasClass("edit-resends")) {
        return editResendsCell(cell, $td, rowData);
    }

    // doing the order status dropdown "live" on the parent row
    //if ($td.hasClass("edit-order-status")) {
    //    return editOrderStatusCell(cell, $td, rowData);
    //}

    return "Row " + cell.index().row + ", Col " + cell.index().column;

}


function editUserCell(cell, $td, rowData) {

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
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}


function editBillingCell(cell, $td, rowData) {

    var html = "";

    $.ajax({
        async: false,
        url: "/account/GetEditBillingForm",
        data: ({ orderId: rowData.idOrder }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}


function editInstitutionCell(cell, $td, rowData) {

    var html = "";

    $.ajax({
        async: false,
        url: "/account/geteditinstitutionform",
        data: ({ id: rowData.idUser }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}


function editResendsCell(cell, $td, rowData) {

    var html = "";

    $.ajax({
        async: false,
        url: "/account/GetResendInfoForm",
        data: ({ orderId: rowData.idOrder }),
        dataType: "json",
        type: "POST",
        success: function (data) {
            html = data.html;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        },
        beforeSend: function () {
            addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
        },
        complete: function () {
            removeIsLoadingIndicator($td);
        }
    });

    return html;
}

// EditOrderStatus_Compact form event
function updateOrderStatus(item, orderId, newOrderStatus) {
    var $item = $(item);
    var form = $item.parents("form");
    var $form = $(form);

    $("input[name='Id']", $form).val(orderId);
    $("input[name='DisplayRowPriceViewModel.OrderStatus']", $form).val(newOrderStatus);

    var data = $form.serialize();

    $.ajax({
        async: false,
        url: "/admin/updateorderstatus",
        data: data,
        dataType: "json",
        type: "POST",
        success: function (data) {
            console.log(data);

            if (data.Result == "Success") {
                // need to update the currently displaying status (presuming it changed)
                $(".dropdown-toggle", $form).html(data.orderStatus + "&nbsp;<b class=\"caret\"></b>");

                var $cell = $item.parents("td");
                fireSuccessIndicator($cell);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        }
    });
}

// EditDiscount_Compact form event
//  derived from // EditOrderStatus_Compact form event
function updateDiscount(item, orderId, newDiscount) {
    var $item = $(item);
    var form = $item.parents("form");
    var $form = $(form);

    $("input[name='Id']", $form).val(orderId);
    $("input[name='Discount']", $form).val(newDiscount);

    var data = $form.serialize();

    $.ajax({
        async: false,
        url: "/admin/updateDiscount",
        data: data,
        dataType: "json",
        type: "POST",
        success: function (data) {
            console.log(data);

            if (data.Result == "Success") {
                // need to update the currently displaying status (presuming it changed)
                $(".dropdown-toggle", $form).html(data.Discount + "&nbsp;<b class=\"caret\"></b>");

                var $cell = $item.parents("td");
                fireSuccessIndicator($cell);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        }
    });
}

function updateEmail(e, thatThis) {
    
    e.preventDefault();

    var form = $(thatThis).parents("form");
    var $form = $(form);

    // configure the current validator to validate hidden form elements
    $.validator.unobtrusive.parse($form);
    $form.validate().settings.ignore = []; // so it doens't "ignore" .hidden fields

    // check to see if the form is invalid
    //if (!$form.valid()) {

    //}
    // how can we di

    var data = $form.serialize();
    $.ajax({
        async: false,
        url: "/account/editemail",
        data: data,
        dataType: "json",
        type: "POST",
        success: function (data) {
            console.log(data);

            var $cell = $("td.child-showing");
            $("#edit-email").text("Edit is complete").addClass("btn btn-success");
            $("#EditEmailModal").modal("hide");

            fireSuccessIndicator($cell);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            
            console.log(data);
            alert(textStatus);
        }
    });

}

// EditRegType_DropDown form event
function updateRegType(item, orderId, newRegTypeId) {
    var $item = $(item);
    var form = $item.parents("form");
    var $form = $(form);


    var payLoad = {
        idOrderRow: orderId,
        idRegType: newRegTypeId
    };

    $.ajax({
        //async: false,
        type: "POST",
        contentType: constants.JsonContentType,
        cache: false,
        url: "/cart/updateorderdetails",
        dataType: constants.JsonDataType,
        data: JSON.stringify(payLoad),
        success: function (data) {
            if (data) {

                // need to update the currently displaying regType and associated costs
                $(".dropdown-toggle", $form).html(data.regTypeShort + "&nbsp;<b class=\"caret\"></b>");
                $("#DisplayRowPriceViewModel_PricesAndDiscounts_UnitPrice").html("$" + data.BasePrice);
                $("#DisplayRowPriceViewModel_PricesAndDiscounts_TotalCostOfOptions").html("$" + data.OptionsPrice);
                $("#DisplayRowPriceViewModel_PricesAndDiscounts_TotalDiscount").html("$" + data.Discount);

                $("#DisplayRowPriceViewModel_PricesAndDiscounts_Tax").html("$" + data.Tax);
                if (data.Tax > 0)
                    $("#DisplayRowPriceViewModel_PricesAndDiscounts_Tax").parents("tr").removeClass("hidden");

                $("#DisplayRowPriceViewModel_PricesAndDiscounts_TotalOrderPrice").html("$" + data.Total);

                // also need to update the originating cell in parent row
                var $parentCell = $("td.child-showing");

                //  Reuse logic already in display-orders.js for when the datatables.net gets created...
                var parentHtml = DO.getBillingCellHtml(data.FlatOff, data.PercentOff, data.regTypeShort, data.Total);
                $parentCell.html(parentHtml);

                var $childRow = $item.closest("td.child-row");
                fireSuccessIndicator($childRow.add($parentCell)); // not auto-hiding the child row yet...  color both the child row and the originating parent
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
        }
    });
}


//function editOrderStatusCell(cell, $td, rowData) {

//    // could definitely use a "busy" cursor.

//    var html = "";
//    var orderToEdit = (rowData.idOrderLegacy != 0) ? rowData.idOrderLegacy : rowData.idOrder;

//    $.ajax({
//        async: false,
//        url: "/admin/geteditorderstatuscompactform",
//        data: ({ id: orderToEdit }),
//        dataType: "json",
//        type: "POST",
//        success: function (data) {
//            html = data.html;
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            alert(textStatus);
//        },
//beforeSend: function () {
//    addIsLoadingIndicator($td, -1); // let ajax "complete" call remove
//},
//complete: function () {
//    removeIsLoadingIndicator($td);
//}
//    });

//    return html;
//}



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