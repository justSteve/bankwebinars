// refactoring to be a little more basic as I had trouble with the reliability of the bootstrap tab events (1 of 3)
var currentAffId = 0; // initial tab is the master (0)
var formerAffId = 0;

$(document).ready(function () {

    $("#sendAll").on("click", function (e) {
        
        e.preventDefault();
        var $btn = $(this);
        $btn.blur();

        // check to see if we are on the master, we should be, since this button should only be on the master
        if (currentAffId == 0) {
            $("#editor_0").val(GetCurrentEditorCopy()); // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate
        }

        SendAll($btn);
    });

    // refactoring to be a little more basic as I had trouble with the reliability of the bootstrap tab events (2 of 3)
    // event documentation from http://getbootstrap.com/javascript/#tabs
    //$('a[data-toggle="pill"]').on('shown.bs.tab', function (e) { // was "shown.bs.tab" but that stopped firing?? switch to show.bs.tab, it worked, switch back to shown and *that* worked??
    $('a[data-toggle="pill"]').on('click', function (e) { // handle click event instead of built-in "tab shown" event
        //e.target // newly activated tab/pill
        //e.relatedTarget // previous active tab/pill

        // refactoring to be a little more basic as I had trouble with the reliability of the bootstrap tab events (3 of 3)
        //var currentAffId = $(e.target).data("link-affid");
        //var formerAffId = $(e.relatedTarget).data("link-affid");
        formerAffId = currentAffId;
        currentAffId = $(this).data("link-affid");
        
        var $editorTA = $("#editorTA");

        // start by retrieving what is in the editor now
        var currCopy = $.trim($editorTA.wijeditor("getText"));

        // if it starts with our special comment (<!--WIJ-NULL-->) remove that
        currCopy = stripWijNull(currCopy);

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
            // check to see if we are on the master, we should be, since this button should only be on the master
            if (currentAffId == 0) {
                $("#editor_0").val(GetCurrentEditorCopy()); // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate
            }

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

    $("#writeAllAffMarkupToStorage").on("click", function (e) {
        e.preventDefault();
        var $btn = $(this);
        $btn.blur();

        // check to see if we are on the master, we should be, since this button should only be on the master
        if (currentAffId == 0) {
            $("#editor_0").val(GetCurrentEditorCopy()); // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate
        }

        WriteAllAffMarkupToStorageAJAX($btn);
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

    currCopy = stripWijNull(currCopy);
    return currCopy;

    return currCopy;
}

function GetAffiliateCopy(affiliateId, isActive) {

    var currCopy = "";

    // is this affiliate currently showing?  if so, grab Editor value rather than hidden text area
    if (isActive)
        currCopy = $.trim($("#editorTA").wijeditor("getText"));
    else
        currCopy = $.trim($("#editor_" + affiliateId).val());
    
    currCopy = stripWijNull(currCopy);
    return currCopy;
}

function stripWijNull(copy)
{
    // if it contains our special empty comment (staticComment / <!--WIJ-NULL-->) remove that
    return copy.replace(/<!--WIJ-NULL-->/g, "");
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
        url: '/Admin/GeneratePromo',
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

function SendAll($btn) {

    // save time...
    var tStart = (new Date()).getTime();

    // setup UI for user feedback
    var origBtnText = $btn.text();
    $btn.text("Saving...");
    $btn.append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');

    var errorAffs = [];
    var callsNeeded = $(".tab-pane").length - 1; // don't count Master tab, we're skipping that one
    var callsComplete = 0;
    var messages = [];

    // loop through all tabs...
    $(".tab-pane").each(function (idx) {
        var $tab = $(this);
        var affiliateId = $tab.data("pane-affid");

        if (affiliateId == 0) // skip the Master tab, although, conceivably, we could store that in a special file and use it for something...
            return;

        // get affiliate specific copy
        var isActive = $tab.hasClass("active"); // should ALWAYS be false in this method, as the Save All button is only on the master tab
        var currCopy = GetAffiliateCopy(affiliateId, isActive);

        //localCopy = replaceMasterTokensForAffiliate(affObj);

        if (currCopy == "")     // if they don't have customized copy we'll need to create it
        {
            // generate from master...
            var affObj = arrayLookup(affs, "idUserAff", affiliateId);
            if (affObj != null) {
                currCopy = replaceMasterTokensForAffiliate(affObj); // always pulls from editor_0 (master)
                // could save it to affiliate text area, since we have it?

                // deal with the SendToList_XYZ textbox
                var $sendTo = $("#SendToList_" + affiliateId);
                if ($.trim($sendTo.val()) == "") {
                    $sendTo.val(affObj.ContactEmail);
                }
            }
        }


        $.ajax({
            url: '/Admin/SendSinglePromo',
            type: 'POST',
            data: { "affiliateId": affiliateId, "messageBodyHtml": currCopy },
            dataType: "json",
            //contentType: "json",
            success: function (result) {
                //$('#send' + affID).text("Success");
                console.log(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // $('#send' + affID).text(textStatus + " " + errorThrown);
                alert("error");
            }
        });
        // remove spinner
        $('#submitSpinWrapper').remove();
        $btn.text("Send!");

        setTimeout(function () { $btn.text(origBtnText); }, 2000);

    });
}



function WriteAllAffMarkupToStorageAJAX($btn) {

    // save time...
    var tStart = (new Date()).getTime();

    // setup UI for user feedback
    var origBtnText = $btn.text();
    $btn.text("Saving...");
    $btn.append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');

    var errorAffs = [];
    var callsNeeded = $(".tab-pane").length - 1; // don't count Master tab, we're skipping that one
    var callsComplete = 0;
    var messages = [];

    // loop through all tabs...
    $(".tab-pane").each(function (idx) {
        var $tab = $(this);
        var affiliateId = $tab.data("pane-affid");

        if (affiliateId == 0) // skip the Master tab, although, conceivably, we could store that in a special file and use it for something...
            return;

        // get affiliate specific copy
        var isActive = $tab.hasClass("active"); // should ALWAYS be false in this method, as the Save All button is only on the master tab
        var currCopy = GetAffiliateCopy(affiliateId, isActive);
        if (currCopy == "")     // if they don't have customized copy we'll need to create it
        {
            // generate from master...
            var affObj = arrayLookup(affs, "idUserAff", affiliateId);
            if (affObj != null) {
                currCopy = replaceMasterTokensForAffiliate(affObj); // always pulls from editor_0 (master)
                // could save it to affiliate text area, since we have it?

                // deal with the SendToList_XYZ textbox
                var $sendTo = $("#SendToList_" + affiliateId);
                if ($.trim($sendTo.val()) == "") {
                    $sendTo.val(affObj.ContactEmail);
                }
            }
        }

        // send to server for additional processing and eventual storage
        var currCopyEnc = encodeURIComponent(currCopy);

        $.ajax({
            url: '/Admin/WritePromoToStorage',
            type: 'POST',
            // async: false, // ?? blast the target w/ requests or do one at a time? seems to work fine asynchronously, make sure to deal with errors!
            data: {
                "Affiliate.idUserAff": affiliateId,
                "Affiliate.ContactEmail": $("#SendToList_" + affiliateId).val(),
                "Affiliate.ttsDomain": $("#ttsDomain_" + affiliateId).val(),
                "Webinar.idWebinar": $("#Webinar_idWebinar").val(),
                "Webinar.Title": $("#Webinar_Title").val(),
                "SendDate": $("#SendDate").val(),
                "EventBody": currCopyEnc
            },
            dataType: "json",
            //contentType: "application/json",
            beforeSend: function () {
            },
            success: function (result) {
                // console.log(idx + ": affiliate " + affiliateId + " saved");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Error occurred during the Save of Affiliate (" + affiliateId + "): " + errorThrown + ".");
                errorAffs.push(affiliateId);
            }
        }).done(function (result) {
            callsComplete++;
            // console.log("done with: " + callsComplete);

            // record the returned messages, just record ones that don't totally succeed (non-blank) for now
            if (result.returnMessage != null &&
                result.returnMessage != "") {
                messages.push(result.returnMessage);
            }

            // figure out if this was the "last one" so we can clean up the UI, report errors, etc.
            if (callsComplete >= callsNeeded)
            {
                // print elapsed time...
                var tEnd = (new Date()).getTime();
                console.log("Time taken for all (" + callsComplete + ") AJAX calls: " + (tEnd - tStart) + " ms");

                // technical issues, errors, ajax, etc.
                if (errorAffs.length) {
                    var errorMsg = "Errors encountered during the save of the following affiliates, please review: " + errorAffs.join(",");
                    console.log(errorMsg)
                    alert(errorMsg);
                }

                // no technical issues, but some business logic messaging...
                if (messages.length) {
                    alert(messages.join("")); // modal? div on page? has a good chance of being a long message
                }

                // remove spinner
                $('#submitSpinWrapper').remove();
                $btn.text("Saved!");

                setTimeout(function () { $btn.text(origBtnText); }, 2000);

            }

        });
        
    });
}


// should be pretty (very!) similar to the above "WriteAllAffMarkupToStorageAJAX" function
function WriteMarkupToStorageAJAX($btn, affiliateId) {

    // send to server for additional processing and eventual storage
    var currCopy = GetCurrentEditorCopy();
    var currCopyEnc = encodeURIComponent(currCopy);
    var origBtnText = $btn.text();

    $.ajax({
        url: '/Admin/WritePromoToStorage',
        type: 'POST',
        data: {
            "Affiliate.idUserAff": affiliateId,
            "Affiliate.ContactEmail": $("#SendToList_" + affiliateId).val(),
            "Affiliate.ttsDomain": $("#ttsDomain_" + affiliateId).val(),
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

        if (result.returnMessage != null &&
            result.returnMessage != "") {
            alert(result.returnMessage);
        } else {
            $btn.text("Saved!");
        }

        $('#submitSpinWrapper').remove();
        setTimeout(function () { $btn.text(origBtnText); }, 2000);
        $btn.blur();
    });
}

function SendToAff(affiliateId) {

    //alert($("#SendToList_" + affiliateId).val());
    var currCopy = GetCurrentEditorCopy();


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