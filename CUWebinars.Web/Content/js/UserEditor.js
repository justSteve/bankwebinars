
function getZip() {
    $("#getZip").show();
    $(".loginContainer").hide();
}

$(document).ready(function () {
    $('#myTabs a[href="#billing"]').tab('show');

    $("#getZip").hide();
    $("#getFirstLast").hide();
    //$("#main").css("height", "400px");
    $("#sameAsBilling").change(function (e) {
        var thisCheck = $(this);
        if (thisCheck.is(':checked')) {
            if ($("#sameAsBilling:checked").val()) {
                $("#ShippingFirstName").val($("#FirstName").val());
                $("#ShippingLastName").val($("#LastName").val());
                $("#ShippingCity").val($("#City").val());
                $("#ShippingAddress").val($("#Address").val());
                $("#ShippingState").val($("#State").val());
                $("#ShippingZip").val($("#Zip").val());
                $("#ShippingCountry").val($("#Country").val());
            }
        }
        return false;
    });

    $(".toggleShipping").click(function (e) {
        $("fieldset.userInfo").toggle("fast");
        return false;
    });
    $("#passwordChangeLink").click(function (e) {
        $("div.changePass").toggle("fast");
    });
    $("#TimeZone").val("@TZ");


    $('<div class="tl"></div><div class="tr"></div><div class="bl"></div><div class="br"></div>')
        .appendTo("div.field");
    $('#FullName').blur(function () {

        var tempName = $('#FullName').val().split(' ');

        if (tempName.length == 2) {
            $('input:#FirstName').val(tempName[0]);
            $('input:#LastName').val(tempName[1]);

        } else {
            $('input:#FirstName').val = tempName[0];
            $('input:#LastName').val = tempName[1];
            $('#getFull').hide();
            $('#getFirstLast').show();
            $('input:#FirstName').focus();
        }
    });

    $("[id^=ResendConfirmation]").on('click', "[id^=SubmitResendConfirmation]", function (nEvent) {
        var myForm = $(this).closest("form");
        nEvent.preventDefault();

        $("#ProgressDialogBS").modal('show');
        $.ajax({
            url: myForm.attr('action'),
            type: "POST",
            data: myForm.serialize(),
            success: function (data) {
                $("#FormContainer").html(data);
            },
            error: function () {
                $("#" + parentForm + " .modalHeaderText").html("<span class=\"label label-warning\">Error condition detected</span>");

            },
            complete: function () {

                $("#ProgressDialogBS").modal('hide');
            }
        });
    });

    $(document).on('click', "[id=resetPassword]", function (nEvent) {
        var myForm = $(this).closest("form");

        nEvent.preventDefault();

        $("#ProgressDialogBS").modal('show');
        $.ajax({
            url: '/Account/resetpassword/?UserID=@(Model.Id)',
            type: "POST",
            data: myForm.serialize(),
            success: function (data) {
                $("#ProgressDialogBS").modal('hide');
                $("#resetPassword").hide();
                $("#PassResetResult").html("<span>A new password has been sent to @Model.Email.</span>");
                $("#cancelContinue").addClass("btn-success btn-large").text("Continue");
            },
            error: function () {
                //alert("Error  (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
                $("#ResultContainer").html("<span class=\"label label-warning\">Error condition detected</span>");
            },
            complete: function () {
                $("#ResultContainer").html("<span class=\"label label-info\">Process is completed.</span>");
            }
        });
    });
});