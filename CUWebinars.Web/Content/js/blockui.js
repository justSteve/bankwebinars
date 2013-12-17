var isAllowedHideBlockUI = false;
var isRequestEnded = true;

function showProcessingIndicator() {
    $.blockUI({
        css:
        {
            border: 'none',
            padding: '45px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
    window.setTimeout(delayedHideBlockUI, 1000);
    isAllowedHideBlockUI = false;
    isRequestEnded = false;
}

function hideProcessingIndicator() {
    if (isAllowedHideBlockUI) {
        $.unblockUI();
    }
    isRequestEnded = true;
}

// this workaround needed to make possible to show BlockUI before client-server interaction will be ended :)
// in some cases hideProcessingIndicator get called before BlockUI get rendered completely, so we need to wait for some time
function delayedHideBlockUI() {
    isAllowedHideBlockUI = true;

    if (isRequestEnded) {
        $.unblockUI();
    }
}