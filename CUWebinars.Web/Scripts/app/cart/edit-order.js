
var parentForm;
var ConnCount = 0;
var formID = 0;
var formName = "myForm";

function payByCC(id) {
    //window.showModalDialog("@(System.Configuration.ConfigurationManager.AppSettings[AppConst.CC_PAYMENT_PROCESSING_URL_KEY])/@Model.OrderRow.Order.ID", null, "dialogwidth: 900px; dialogheight: 370px; center: yes; resizable: yes");
    window.showModalDialog("/home/PayCC/" + id, null, "dialogwidth: 900px; dialogheight: 370px; center: yes; resizable: yes");
    $('#step2_Submit2Sage' + id).submit();
    window.location.reload();
}
function OrderDetails(orderID) {
    window.showModalDialog("/Admin/Registrations/OrderDetailsToUser/" + orderID, null, "dialogwidth: 900px; dialogheight: 370px; center: yes; resizable: yes");
    //$('#step2_Submit2Sage' + id).submit();
    window.location.reload();
}


function OptionsChanged(fromInput) {
    //alert("hit optionschanged");
    //handles changed order properties
    //readyToSubmit = true;
    $('#regTypeForm').submit();
}


function CloseAndOpenAdditonalLocations(ID) {
    $("#ResendConnectionInfo_" + ID).modal('hide');
    $("#AddLocation_" + ID).modal('show');
}

function CheckEmails() {
    $('input:text.emailInput').each(function (nr) {
        if ($(this).val() === "") { } else {
            //console.log("checking isValid");
            if (isValidEmailAddress($(this).val())) {
                //console.log("found it valid");
                $("[id^=SubmitAddLocations_]").removeAttr("disabled").val("Submit");
                $("[id^=SubmitResendConfirmation]").removeAttr("disabled").val("Submit");
            }
        }
    });
}


function isValidEmailAddress(emailAddress) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(emailAddress);
};


$(document).ready(function () {

    $("[id^=ResendConfirmation]").on('click', "[id^='SubmitResendConfirmation']", function (nEvent) {
        var tForm = $(this).closest("form");

        //var formElements = new Array();
        //$('form input:text.resendInputs').each(function (nr) {
        //    formElements.push($(this).val());
        //    alert("this = "+ $(this).val() ); 
        //});

        $("[id^='ModalResendHeaderText_']").html("<span class=\"label label-warning\">Processing...please wait.</span>");
        $("[id^='CollectResendLocations_']").text(" ");
        $("[id^='SubmitResendConfirmation_']").hide();

        //$('form [name=listOfEmails]').val(formElements.toString());
        nEvent.preventDefault();

        //$("#ProgressDialogBS").modal('show');
        $.ajax({
            url: tForm.attr('action'),
            type: "POST",
            data: tForm.serialize(),
            success: function () {
                $("[id^='ModalResendHeaderText_']").html("<span class=\"label label-success\">Confirmation was sent.</span>");
            },
            error: function () {
                $("#" + parentForm + " .modalHeaderText").html("<span class=\"label label-warning\">Error condition detected</span>");
            },
            complete: function () {
                //$("#ProgressDialogBS").modal('hide');
            }
        });
    });

    //$(".dropdown-menu").click(function (event) {

    //    formName = $(event.target).closest('a').attr('href').split("_")[0].replace("#", "");
    //    parentForm = $(event.target).closest('a').attr('href').replace("#", "");
    //    var formIDasnum = $(event.target).closest('a').attr('href').split("_")[1];
    //    $("#" + parentForm + " [id^=SubmitAddLocations]").attr("disabled", "disabled").val("Waiting for valid email");


    //    //AddressesCount = 1;
    //    ConnCount = ($("#ConnectionsCount_" + formIDasnum).val() * 1);

    //    //forces recalc
    //    $("[id^=AddLocation_]").on('hidden', function () {
    //        //console.log('Modal is hidden');
    //        document.location.reload(true);
    //    });

    //    $("[id^=EditRegistrationType_]").on('hidden', function () {
    //        //console.log('Modal is hidden');
    //        document.location.reload(true);
    //    });
    //    //$("#myModal").modal('show');
    //});

    var timer; // external so it's value is held over all instances of the timer function
    $('body').on('keyup', 'input:text.emailInput', function () {
        if (timer) {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            // perform your check
            CheckEmails();
        }, 500);
    });



    //$("#ChangeAssignedUser").on('click', "input:button", function (event) {
    //    //alert("this inputButton");
    //    if (this.name != "addLocNumberButton") {
    //        return false;
    //    }
    //    //alert("fell thru");
    //    event.preventDefault();
    //    var myForm = $("#changeAssignedUser form");

    //    $("[name=migrateOrder]").val(this.name);

    //    $("#ProgressDialogBS").modal('show');
    //    $.post({
    //        url: myForm.action,
    //        type: "POST",
    //        //data: myForm.serialize(),
    //        success: function (data) {
    //            $(this).closest("div").html(data);
    //            return true;
    //        },
    //        error: function (jqXhr, textStatus, errorThrown) {
    //            alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
    //        },
    //        complete: function () {

    //            $("#ProgressDialogBS").modal('hide');
    //        }
    //    });

    //    myForm.submit();
    //});


    $(document).on('click', "[name^='change']", function (event) {
        var changeAllOrders = this.name;
        var $form = $("#changeAssignedUser form");
        var $target = $($form.attr('data-target'));

        $.ajax({
            type: $form.attr('method'),
            url: $form.attr('action'),
            data: $form.serialize(),

            success: function (data, status) {
                $target.html(data);
            }
        });

        event.preventDefault();
    });
});


//function isValidEmailAddress(emailAddress) {
//    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
//    return re.test(emailAddress);
//};


function getRegistrationsTableCode(userID) {
    //userID = $("#targetUserID").val();

    var template = $("#registrationsTableTemplate")
        .clone()
        .attr("id", "registrationsTable")
        .show()
    //    .outerHTML()
    ;
    initRegistrationsTable(userID);
    return template;
}

function OpenSummary(userID) {
    window.open('/Admin/Registrations/UserSummary/' + userID, 'UserSummary');
}

function initRegistrationsTable(userID) {
    //
    var settings = {
        "bPaginate": false,
        "bInfo": false,
        "bLengthChange": false,
        "bFilter": false,
        "bProcessing": true,
        "bServerSide": true,
        "bStateSave": false,
        "bSort": true,
        "aaSorting": [
            [1, "asc"]
        ],
        "sAjaxSource": "/admin/registrations/registrationssubtabledatabyuser/" + userID,
        "aoColumns": [
        //Column1: order data
        {
            "bSortable": false,
            "sWidth": "15%",
            "fnRender": function (obj) {
                var orderData = obj.aData[obj.iDataColumn];
                return "<span class='alignright'>" + orderData.orderId + "<br>Source: " + orderData.Origin+ "</span> ";
            }
        },

        //Column2: event link
        {
            "bSortable": false,
            "sWidth": "30%",
            "fnRender": function (obj) {
                var eventData = obj.aData[obj.iDataColumn];
                return eventData.eventdate + " <BR> " + eventData.title;
            }
        },

        //Column3: registration type 
        {
            "bSortable": true,
            "sWidth": "20%",
            "fnRender": function (obj) {
                var eventData = obj.aData[obj.iDataColumn];
                return eventData.regType + " | " + eventData.paymentAmt;
            }
        },

        //Column3.5: Order Type
        {
            "bSortable": false,
            "bVisible": false,
            "sWidth": "25%"
        },

        //Column4: order date
        {
            "bSortable": false,
            "sWidth": "10%"
        },

        //Column5:  edit and resend links
        {
            "bSortable": false,
            "sWidth": "5%"
        },
        //Column6: status
        {
            "bSortable": false,
            "sWidth": "10%",
            "bVisible": false
        }

        ]
    };

    $("#registrationsTable").dataTable(settings);
}

function changeAssignedUser(theUser) {
    $("#targetUserID").val(theUser.value);

    $("[name='moveAll']").show().attr('value', "Change all orders to... ").removeAttr("disabled");
    $("[name='moveOne']").show().attr('value', "Change just this order to... ").removeAttr("disabled");
}

function resendConnectionInfo(link) {

    $.post($(link).attr("href"), function (data, status) {
        alert("Connection info was resent successfully");
    });

    return false;
}
