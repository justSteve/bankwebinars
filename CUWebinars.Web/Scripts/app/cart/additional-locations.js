
$(document).ready(function () {

    //var modalFormOptions = {
    //    keyboard: true,
    //    show: true,
    //    remote: '/Webinar/GetAdditionalLocationByOrderId'
    //};

    //$('#LoadAddLocationsModalButton').on('click', function () {
    //    modalFormOptions.remote = modalFormOptions.remote + '/' + $('#CheckoutOptionsViewModel_Order_idOrder').val();
    //    $('#SignupModal').modal(modalFormOptions);
    //});

function CheckEmails() {
    $('[name^="Email"]').each(function (nr) {
        if ($(this).val() === "") { } else {
            //console.log("checking if isValid:" + $(this).val());
            if (isValidEmailAddress($(this).val())) {
                //console.log("found it valid");
                //$("[id^=SubmitAddLocations_]").removeAttr("disabled").val("Submit");
                //$("[id^=SubmitResendConfirmation]").removeAttr("disabled").val("Submit");

                wait4Emails = false;
                $('#instruct').hide(500);
                $('#AddToCart').attr({ disabled: false, value: 'Sign Up' });
                $('#AddToCart1').attr({ disabled: false, value: 'Sign Up' });
            }
        }
    });
}

function AddLocation(countIterator, cost) {
    ConnCount = (countIterator);
    //alert(ConnCount);
    $("#AddButton_" + formID).hide();
    $("#" + parentForm + " [name=submitAddLocations]").attr("disabled", "disabled").val("Waiting for valid email").show();

    var inputNode = $(' <div id="line_' + (countIterator) + '">' + countIterator + '. <input type="text" onblur=CheckEmails(); id="Email_' + (countIterator) + '" name="Email_' + (countIterator) + '" class="emailInput"  placeholder="email" size="30"/>&nbsp;<a class="btn btn-mini btn-danger" onclick=RemoveLocation("line_' + (countIterator) + '"); href="#"><i class="icon-trash icon-large"></i> Remove?</a> ' + cost + '</div>');
    $("#" + parentForm + " .action").val("add");
    $("#CollectAdditionalLocation_" + formID).append(inputNode);
    $("#" + parentForm + " [name=connectionsCount]").val((ConnCount) * 1);
    $("#CollectAdditionalLocation_" + formID + " a").hide();
    $("#CollectAdditionalLocation_" + formID + " input").focus();
    $("#" + parentForm).animate({ scrollTop: $("#" + parentForm).scrollHeight }, 1000);
}

function RemoveLocation(theLine) {
    //ConnCount = (ConnCount - 1);
    var theInput = theLine.split("_")[1];
    console.log(theLine);
    alert(parentForm);
    $("#" + parentForm + " [name=submitAddLocations]").removeAttr("disabled").val("Submit");
    $("#" + parentForm + " [name=action]").val($("#" + parentForm + " [name=Email_" + theInput + "]").val());
    $("#" + parentForm + " .line_" + theInput).remove();
    $("#" + parentForm + " [name=connectionsCount]").val((ConnCount) * 1);
    $("#" + parentForm + " [name=submitAddLocations]").trigger('click');
}
var timer; // external so it's value is held over all instances of the timer function

    $('#connectionsCount').on('focus', function () {
        $("#connectionsCount").val('');
    });

    $('body').on('keyup', 'input:text.emailInput', function () {
        if (timer) {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            console.log("checking:");
            // perform your check
            CheckEmails();
        }, 500);
    });

    $('#connectionsCount').on('blur', function () {
       
        wait4Emails = true;
        var collectEmails = '<div id="instruct">Please enter your desired emails</div>';
        var numInputs2Render = $("#connectionsCount").val();

        $('#AddToCart').attr({ disabled: 'disabled', value: 'Waiting for Emails' });
        $('#AddToCart1').attr({ disabled: 'disabled', value: 'Waiting for Emails' });

        if (numInputs2Render == 0) {
            wait4Emails = false;
            collectEmails = "";
            $('#AddToCart').attr({ disabled: false, value: 'Sign Up' });
            $('#AddToCart1').attr({ disabled: false, value: 'Sign Up' });
        }

        for (var i = 0; i < numInputs2Render; i++) {
            collectEmails = collectEmails + '<input type="text" id="Email_' + i + '" name="Email_' + i + '" class="emailInput"  size="30"   ><br>';
        }

        $('#collectAdditionalLocation').html(collectEmails);
        $('#Email_0').focus();
    });

    $("[id^=AdditionalLocationContainer]").on('click', "[id^=SubmitAddLocations]", function (nEvent) {
        alert("Hit");
        var $form = $(this).closest("form");
        nEvent.preventDefault();
        $("#AddLocModal_" + formID).html("<span class=\"label label-warning\">Processing...please wait.</span>");

        $.ajax({
            url: $form.attr('action'),
            type: "POST",
            data: $form.serialize(),
            success: function (result) {
                $("#AddLocModal_" + formID).html("<span class=\"label label-success\">" + result.msg + "</span>");
            },
            error: function () {
                $("#" + parentForm + " .modalHeaderText").html("<span class=\"label label-error\">Error condition detected</span>");
            },
            complete: function () {
                $("#AddButton_" + formID).show();
                $("#NumLocationsMsg_" + formID).hide();
                // $("#" + parentForm + " [name=connectionsCount]").val((result.numLocations) * 1);
                //$("#[id^=SubmitAddLocations]").hide();
                $("[id^=CollectAdditionalLocation] a").show();
                //$("[id^=ShowsAddLocTotalCost]").html(result.optionsCost);
                $("#ProgressDialogBS").modal('hide');
            }
        });
    });
});
