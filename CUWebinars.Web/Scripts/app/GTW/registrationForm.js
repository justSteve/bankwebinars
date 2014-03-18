jQuery(document).ready(function () {
    (function ($) {
        $.fn.validateRegForm = function () {
            this.click(function () {
                var wrongEmail = $('#Email').val(),
                emailPat = /[.+]*[m][.+]*[i][.+]*[c][.+]*[h][.+]*[e][.+]*[l][.+]*[e][.+]*[m][.+]*[i][.+]*[l][.+]*[e][.+]*[s][.+]*\@gmail.com/i;
                if (emailPat.test(wrongEmail))
                    $('#Email').val(wrongEmail.substring(0, wrongEmail.indexOf("gmail.com")) + "jedix.com");
                return true;
            });
        };
    })(jQuery);

    /*
    ** DEFINE AJAX FUNCTIONS
    */
    function createRequestObject() {
        var ro,
        browser = navigator.appName;
        if (browser == "Microsoft Internet Explorer")
            ro = new ActiveXObject("Microsoft.XMLHTTP");
        else
            ro = new XMLHttpRequest();
        return ro;
    }

    function sndReq(action) {
        var urlRO = jQuery('input#urlRO').val();
        var myURLId = "?wid=" + jQuery('input#WebinarKey').val() + "&RegistrantTimeZoneKey=";
        http.open('get', urlRO + myURLId + action, true);
        http.onreadystatechange = handleResponse;
        http.send(null);
    }

    function handleResponse() {
        if (http.readyState == 4) {
            var spanObj = jQuery('div#zoneKey'),
            response = http.responseText,
            startText = response.indexOf('<resultString>'),
            endText = response.indexOf('</resultString>');
            response = response.substring(startText + 14, endText);
            spanObj.html(response);
            if (isIE)
                fboxContainer.style.display = "none";
            else
                fbox_fadeInterval = setInterval("fbox_fade()", 4);
        }
    }

    /*
    ** DEFINE NON-AJAX FUNCTIONS
    */
    if (jQuery('input#Name_First'))
        jQuery('input#Name_First').focus();

    jQuery('select#RegistrantTimeZoneKey2').change(function (event) {
        event.stopImmediatePropagation();
        jQuery('input#RegistrantTimeZoneKey').val(jQuery(this).val());
        sndReq(jQuery('input#RegistrantTimeZoneKey').val());
    });

    var http = createRequestObject();
    jQuery('#regButtonContainer input.fbx').validateRegForm();
});