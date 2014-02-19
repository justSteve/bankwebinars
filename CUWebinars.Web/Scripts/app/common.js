$(function () {
    $(".control-label:not(span)").append(":&nbsp;");
});

var shsGlobal = new Globals();

function Globals() {

}

Globals.prototype.ajaxLoaderStart = function () {
    
    $("#ProgressDialogBS").show();
};

Globals.prototype.ajaxLoaderStop = function() {
    $("#ProgressDialogBS").hide();
};


$(document).ajaxStart(function () {
    shsGlobal.ajaxLoaderStart();
});

$(document).ajaxStop(function () {
    shsGlobal.ajaxLoaderStop();
});