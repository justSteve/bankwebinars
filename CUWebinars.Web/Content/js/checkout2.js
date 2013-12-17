var stepsCount = 3;
var currentWorkflowState;

function returnHandle(value) {
    //alert("Hit return");
    hideProcessingIndicator();
    readyToSubmit = false;
}

function loadStep2() {
    $("#checkoutWizardHandle1").click();

    $("#Step2_Container").load("/cart/step2?" + new Date().getTime(), function () {
        hideProcessingIndicator();
    });
}

function setWizardStepsLockingStatus(count) {
    for (var i = 0; i < count; i++) {
        //$(".checkoutWizard").hrzAccordionEnable(i);
        //alert(".checkoutWizard");
    }

    for (var i = count; i < stepsCount; i++) {
        //$(".checkoutWizard").hrzAccordionDisable(i);
    }
}


function loadStep3() {
    $(".checkoutWizard").hrzAccordionEnable(2);
    $("#checkoutWizardHandle2").click();

    $(".checkoutWizard").hrzAccordionInactivate(0);
    $(".checkoutWizard").hrzAccordionInactivate(1);
    $(".checkoutWizard").hrzAccordionInactivate(2);

    $("#Step3_Container").load("/cart/step3?" + new Date().getTime(), function (responseText, textStatus, XMLHttpRequest) {
        hideProcessingIndicator();
    });
}
function initCheckoutWizard(workflowState) {
    // Refresh login button
    $.ajax({
        url: "/Account/ShowLoginStatus",
        cache: false,
        success: function (html) {
            $(".showLoggedUser").html(html);
        }
    });



    $("#Step" + workflowState.CurrentStep + "_NextButton").click(function () {
        var i = parseFloat(workflowState.CurrentStep) + 1;
        var loadStepper = ("loadStep" + i);
        
        eval(loadStepper);
    });

    $("#Step" + workflowState.CurrentStep + "_BackButton").click(function () {
        $("#checkoutWizardHandle" + (workflowState.CurrentStep - 2)).click();
    });

    $("#Step" + workflowState.CurrentStep + "_Container form").each(function () {
        if (this.id != 'Step2_ContinueShoppingForm') {
            $(this).validate({
                submitHandler: function (form) {
                    showProcessingIndicator();
                    $(form).ajaxSubmit({
                        target: "#Step" + workflowState.CurrentStep + "_Container",
                        success: function () {
                            if (!currentWorkflowState.ProceedToNextStep)
                                hideProcessingIndicator();
                        }
                    })
                }
            });
        }
    });

    // special case
    if (workflowState.CurrentStep != 3) {
        setWizardStepsLockingStatus(workflowState.EnabledStepsCount);
    }

    if (workflowState.ProceedToNextStep) {
        // special case
        if (workflowState.CurrentStep == 1) {
            loadStep2();

        } else {
            $("#checkoutWizardHandle" + (workflowState.CurrentStep)).click();
        }
    }
}


