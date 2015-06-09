/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

if (SESSIONCHECK === null || typeof SESSIONCHECK === 'undefined')
    var SESSIONCHECK = {}; // object to holds all references and methods.

var SC = SESSIONCHECK; // create shortcut alias

// document.ready function
$(function () {

    //SC.checkSessionForm = $('#sessionCheckForm');
    //SC.checkSessionForm.on('submit', SC.checkSession);
    SC.loginBtn = $('#btnLogin');

    //window.setInterval(SC.checkSession, 10000);
});

// self-invoking function for creating methods using Module pattern.
(function (ns) {

    ns.checkSession = function() {

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/Home/SessionIsActive',
            dataType: constants.JsonDataType,
        }).done(function (data, textStatus, jqXHR) {

            // if Success, session is still alive. If Fail, it is expired.
            if(data.Result === 'Success') {
                
            } else if (data.Result === 'Fail') {

                SC.loginBtn.fadeOut(200, function() {
                    $(this).next().remove();
                    $(this).replaceWith('<a id="btnLogin" href="/Account/Login"><i class="icon-user"></i> Login</a>').fadeIn(200);
                }); 
            }

        });
    }

})(SC);