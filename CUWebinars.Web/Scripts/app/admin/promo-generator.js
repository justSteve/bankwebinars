$(document).ready(function () {

    // event documentation from http://getbootstrap.com/javascript/#tabs
    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) { // was "shown.bs.tab" but that stopped firing?? switch to show.bs.tab, it worked, switch back to shown and *that* worked??
        //e.target // newly activated tab/pill
        //e.relatedTarget // previous active tab/pill

        var currentAffId = $(e.target).data("link-affid");
        var formerAffId = $(e.relatedTarget).data("link-affid");
        
        var $editorTA = $("#editorTA");

        // start by retrieving current
        var currCopy = $.trim($editorTA.wijeditor("getText"));

        // if it starts with our special comment (<!--WIJ-NULL-->) remove that
        if (currCopy.indexOf("<!--WIJ-NULL-->") == 0)
            currCopy = currCopy.substr(15);

        // store it in the proper textarea
        $("#editor_" + formerAffId).val(currCopy);

        // grab the target's textarea and load up the editor and its associated textarea
        affCopy = $("#editor_" + currentAffId).val();
        
        SetEditorTabForAffiliate(currentAffId, affCopy);
    });

    $("#submitGenPromoTexts").on('click', function (e) {
        e.preventDefault();
        GetMasterMarkupAJAX($(this));
    });

    $("#resetAllAffiliates").on("click", function (e) {
        if (confirm("Are you sure?"))
        {
            $("#editor_0").val(GetCurrentEditorCopy()); // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate

            $(".tab-pane").each(function (index) {
                var $tab = $(this);
                var affId = $tab.data("pane-affid");
                
                var localCopy = "";
                if (affId != 0)
                {
                    var affObj = arrayLookup(affs, "idUserAff", affId);
                    if (affObj != null) {
                        localCopy = replaceMasterTokensForAffiliate(affObj);
                    }

                    $("#editor_" + affId, $tab).val(localCopy);
                }

            });
        }
    });

    $(".send-to-affiliate").on("click", function (e) {
        var affiliateId = $(this).closest(".tab-pane").data("pane-affid");
        SendToAff(affiliateId);
    });

    $(".write-markup-to-storage").on("click", function (e) {
        e.preventDefault();
        var $btn = $(this);
        var affiliateId = $btn.closest(".tab-pane").data("pane-affid");
        WriteMarkupToStorageAJAX($btn, affiliateId);
    });

    $(".reset-to-master").on("click", function (e) {
        var affiliateId = $(this).closest(".tab-pane").data("pane-affid");
        SetEditorTabForAffiliate(affiliateId, ""); // blank for copy will cause it to revert to current master
    });

    $(".copy-markup").on("click", function (e) {

        // need a field to hold the content, don't really need it visible, but if it's hidden then the copy doesn't work...
        SetCopyToClipboardTextArea(true);

        setTimeout(function () {
            try {
                document.execCommand('copy');
            }
            catch (err) {
                alert('Please use Ctrl/Cmd+C to copy');
            }
        }, 0);
        
    });

    $("#copyToClipboard").on("focus", function (e) {
        SetCopyToClipboardTextArea(true);
    });

});

function GetCurrentEditorCopy()
{
    var $editorTA = $("#editorTA");
    var currCopy = $.trim($editorTA.wijeditor("getText"));

    var staticComment = "<!--WIJ-NULL-->";
    // if it starts with an empty comment (staticComment / <!--WIJ-NULL-->) remove that
    if (currCopy.indexOf(staticComment) == 0)
        currCopy = currCopy.substr(staticComment.length);

    return currCopy;
}

function SetCopyToClipboardTextArea(selectText)
{
    var currCopy = GetCurrentEditorCopy();
    var $fld = $("#copyToClipboard");
    $fld.val(currCopy);
    if (selectText)
        $fld.select();
}

// not the cleanest function ever... pending a refactor?
function SetEditorTabForAffiliate(affiliateId, affiliateCopy)
{
    var localCopy = affiliateCopy;

    // if it's blank, run master conversion
    if (affiliateId != 0 &&
        $.trim(localCopy) == "") {

        var affObj = arrayLookup(affs, "idUserAff", affiliateId);
        if (affObj != null)
        {
            localCopy = replaceMasterTokensForAffiliate(affObj);

            // this probably needs to be moved out of this function, but only needs to run on initial tab load, just like the replacement code
            // deal with the SendToList_XYZ box
            var sendTo = $("#SendToList_" + affiliateId);
            if ($.trim(sendTo.val()) == "") {
                sendTo.val(affObj.ContactEmail); // TODO: zzz ALS expand this or switch to expanded field?
            }
        }
    }

    $("#editor_" + affiliateId).val(localCopy); // do this right away even though it's not really needed, makes it easier to think about when debugging

    if (localCopy == "")
        localCopy = "<!--WIJ-NULL-->"; // WIJEDITOR seems to choke on erasing / going to blank ("") and leaves the prior text, use a special comment that we have to program around to overcome that

    var $editorTA = $("#editorTA");
    $editorTA.wijeditor("setText", localCopy);
    SetCopyToClipboardTextArea(false);
    $editorTA.focus();

}

function GetMasterMarkupAJAX($btn) {

    $.ajax({
        url: '/Admin/PromoGenerate',
        type: 'POST',
        data: $('#mailer').serialize(),
        dataType: "json",
        //contentType: "application/json",
        beforeSend: function () {
            $btn.append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');
        },
        success: function (result) {

            var $editorTA = $("#editorTA");

            $editorTA.wijeditor("setText", result.masterText);
            $editorTA.focus();
            $("#editor_0").val(result.masterText); // 0 is master, store it right away
            SetCopyToClipboardTextArea(false);

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + " " + errorThrown);
        }
    }).done(function (result) {
        $('#submitSpinWrapper').remove();
        $btn.blur();
    });
}

function WriteMarkupToStorageAJAX($btn, affiliateId) {
    
    var currCopy = GetCurrentEditorCopy();
    currCopyEnc = encodeURIComponent(currCopy);
    var origBtnText = $btn.text();
    
    $.ajax({
        url: '/Admin/WritePromoToStorage',
        type: 'POST',
        data: {
            "Affiliate.idUserAff": affiliateId,
            "Webinar.idWebinar": $("#Webinar_idWebinar").val(),
            "Webinar.Title": $("#Webinar_Title").val(),
            "SendDate": $("#SendDate").val(),
            "EventBody": currCopyEnc
        },
        dataType: "json",
        //contentType: "application/json",
        beforeSend: function () {
            $btn.text("Saving...");
            $btn.append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');
        },
        success: function (result) {
            //alert('test: ' + result.result);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + " " + errorThrown);
        }
    }).done(function (result) {
        $btn.text("Saved!");
        $('#submitSpinWrapper').remove();
        setTimeout(function () { $btn.text(origBtnText); }, 2000);
        $btn.blur();
    });
}

function SendToAff(affiliateId) {

    //alert($("#SendToList_" + affiliateId).val());
    var currCopy = GetCurrentEditorCopy();
    //alert(currCopy);
    alert('Not quite implemented yet...');
    return;

    $.ajax({
        url: '/Admin/SendSinglePromo',
        type: 'POST',
        data: { "affiliateId": affiliateId, "messageBodyHtml": currCopy },
        dataType: "json",
        //contentType: "json",
        success: function (result) {
            //$('#send' + affID).text("Success");
            alert('success');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // $('#send' + affID).text(textStatus + " " + errorThrown);
            alert("error");
        }
    });
}

// unforatunately there is a server-side version of this function in the AdminController file as well...
function replaceMasterTokensForAffiliate(aff)
{
    var copy = $("#editor_0").val(); // 0 is master

    copy = copy.replace(/\{aff_EmailBanner\}/gi, aff.EmailBanner);
    copy = copy.replace(/\{aff_ttsdomain\}/gi, aff.ttsDomain);
    copy = copy.replace(/\{aff_idUserAff\}/gi, aff.idUserAff);
    copy = copy.replace(/\{aff_ContactPerson\}/gi, aff.ContactPerson);
    copy = copy.replace(/\{aff_ContactEmail\}/gi, aff.ContactEmail);
    copy = copy.replace(/\{aff_ContactPhone\}/gi, aff.ContactPhone);
    copy = copy.replace(/\{aff_EmailFooter\}/gi, aff.EmailFooter);

    return copy;
}

// "Best, Fastest way [to find an object in an array by a property value] is" http://stackoverflow.com/questions/5579678/jquery-how-to-find-an-object-by-attribute-in-an-array
function arrayLookup(array, prop, val) {
    for (var i = 0, len = array.length; i < len; i++) {
        if (array[i].hasOwnProperty(prop) && array[i][prop] === val) {
            return array[i];
        }
    }
    return null;
}