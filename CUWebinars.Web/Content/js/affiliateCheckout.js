//



function OptionsChanged(fromInput) {
    //handles changed order properties
    //alert("OptionsChanged has been passed: " + fromInput);
    readyToSubmit = true;
    var DivNum = fromInput.split(",");
    
    $('#changedOrderID').val(DivNum[1]);
    //console.log(fromInput);
    //showProcessingIndicator();
    $('#frmchooseOptions').ajaxSubmit({
        beforeSubmit: checkReady,
        cache: false,
        url: "/affiliatecheckout/updateoptions",
        target: "#result" + DivNum[1]
        //        , success: success
    });
};

function checkReady() {
    if (!readyToSubmit) {
        return false;
    }
}

function success() {

    readyToSubmit = false;
}




// Begins original Nav-oriented manager code


var stepsCount = 4;
var readyToSubmit = false;
var processingEmails = false;

function initCheckoutWizard(workflowState) {
    //runs at each form submission and i think also at returns?
    $.ajaxSetup({
        // Disable caching of AJAX responses */
        cache: false,
        beforeSubmit: checkReady()
    });


    $("#Step" + workflowState.CurrentStep + "_NextButton").click(function () {
        $("#checkoutWizardHandle" + workflowState.CurrentStep).click();
    });

    $("#Step" + workflowState.CurrentStep + "_BackButton").click(function () {
        $("#checkoutWizardHandle" + (workflowState.CurrentStep - 2)).click();
    });

    $("#Step" + workflowState.CurrentStep + "_Container form").not(".nonAjax").each(function () {
        $(this).validate({
            submitHandler: function (form) {
                showProcessingIndicator();
                $(form).ajaxSubmit({
                    cache: false,
                    target: "#Step" + workflowState.CurrentStep + "_Container",
                    success: hideProcessingIndicator,
                    beforeSubmit: checkReady()
                })
            }
        });
    });

    //special case
    if (workflowState.CurrentStep != 4) {
        setWizardStepsLockingStatus(workflowState.EnabledStepsCount);
        for (var i4 = 0; i < workflowState.CurrentStep; i++) {
            $(".checkoutWizard").hrzAccordionActivate(i);
        }
    } else {
        for (var i = 0; i < 4; i++) {
            $(".checkoutWizard").hrzAccordionInactivate(i);
        }
    }

    if (workflowState.ProceedToNextStep) {
        $("#Step" + (workflowState.CurrentStep + 1) + "_Container").html('');
        $("#Step" + (workflowState.CurrentStep + 1) + "_Container").load("/affiliatecheckout/step" + (workflowState.CurrentStep + 1));
        $(".checkoutWizard").hrzAccordionEnable(workflowState.CurrentStep);
        $("#checkoutWizardHandle" + workflowState.CurrentStep).click();
    }
}

function setWizardStepsLockingStatus(count) {
    for (var i = 0; i < count - 1; i++) {
        $(".checkoutWizard").hrzAccordionEnable(i);
    }

    for (var i = count - 1; i < stepsCount; i++) {
        $(".checkoutWizard").hrzAccordionDisable(i);
    }
}

$(window).load(function () {
    toggleChooseWebinarSubmit();
    $("#ChooseWebinar_UpcomingWebinarId").change(function () {
        if ($(this).val()) {
            $("#ChooseWebinar_RecordedWebinarId").val("");
        }
        toggleChooseWebinarSubmit();
    });
    $("#ChooseWebinar_RecordedWebinarId").change(function () {
        if ($(this).val()) {
            $("#ChooseWebinar_UpcomingWebinarId").val("");
        }
        toggleChooseWebinarSubmit();
    });
});

function toggleChooseWebinarSubmit() {
    if ($("#ChooseWebinar_RecordedWebinarId").val() || $("#ChooseWebinar_UpcomingWebinarId").val()) {
        $("#ChooseWebinar_Submit").show();
    } else {
        $("#ChooseWebinar_Submit").hide();
    }
}
