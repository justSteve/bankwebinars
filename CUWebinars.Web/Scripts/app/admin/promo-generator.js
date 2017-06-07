// refactoring to be a little more basic as I had trouble with the reliability of the bootstrap tab events (1 of 3)
var currentAffId = 0; // initial tab is the master (0)
var formerAffId = 0;
var timeString = "";
$(document).ready(function () {
    $("#showCampaignsBtn").button();

    $("#sendAll").hide();
    $(".setStatusBtn").off();
    $(".setStatusBtn").on("click", SetwStatus);

    $("#showCampaignsBtn").on("click", showHideCampaign);


    if (currentUserIsAffiliate) {
        $(".setStatusBtn").hide();
        $("#leftSideBar").hide();
        $("#showCampaignsBtn").hide();
    }

    if (valOfStatus === "Pending") {
        $(".setStatusBtn").text("Set event to 'Scheduled'.");

    } else {
        $(".setStatusBtn").text("Set event to 'Pending'.");

        $("#sendAll").show();
    }


    $("#sendAll").on("click", function (e) {

        //var r = confirm("Do you want to continue", "yes");
        if (!confirm("Do you want to continue")) {
            alert("Submission has been canceled.");
            return false;
        } else {

            e.preventDefault();
            var $btn = $(this);
            $btn.blur();

            // check to see if we are on the master, we should be, since this button should only be on the master
            if (currentAffId == 0) {
                $("#editor_0").val(GetCurrentEditorCopy());
                // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate
            }

            SendAll($btn);
        }
    });


    $('a[data-toggle="pill"]').on('click', function (e) {
        // handle click event instead of built-in "tab shown" event

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
        if (confirm("Are you sure?")) {
            // check to see if we are on the master, we should be, since this button should only be on the master
            if (currentAffId == 0) {
                $("#editor_0").val(GetCurrentEditorCopy()); // store what is in the editor as the latest and greatest for use by replaceMasterTokensForAffiliate
            }



            $(".tab-pane").each(function (index) {
                var $tab = $(this);
                var affId = $tab.data("pane-affid");

                var localCopy = "";
                if (affId != 0) {
                    var affObj = arrayLookup(affs, "idUserAff", affId);
                    if (affObj != null) {
                        localCopy = replaceMasterTokensForAffiliate(affObj);
                    }

                    $("#editor_" + affId, $tab).val(localCopy);
                }

            });
        }
    });

    $(".create-campaign").on("click", function (e) {
        e.preventDefault();
        var $btn = $(this);
        var affiliateId = $btn.closest(".tab-pane").data("pane-affid");
        CreateCampaign($btn, affiliateId);

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

function GetAffTimeString(affId) {

    $.ajax({
        url: '/Admin/GetAffiliateTimeZone',
        type: 'POST',
        data: {
            "idAffiliate": affId,
            "idWebinar": $("#Webinar_idWebinar").val()
        },
        async: true,
        dataType: "json",

        success: function (result) {
            console.log(result.timeFormatDisplay);
            return result.timeFormatDisplay;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + " " + errorThrown);
        }
    });
};


function setDataForAffiliate(currentAffId) {

    // handle click event instead of built-in "tab shown" event

    formerAffId = currentAffId;
    //currentAffId = $(this).data("link-affid");

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
}
function showHideCampaign() {

    if ($(this).hasClass('active')) {
        $("#showAll").show();
        $("#showCampaign").hide();
    } else {
        $("#showAll").hide();
        $("#showCampaign").show();
    };
    $(this).hasClass('checked');
}
function GetCurrentEditorCopy() {
    var $editorTA = $("#editorTA");
    var currCopy = $.trim($editorTA.wijeditor("getText"));

    currCopy = stripWijNull(currCopy);
    return currCopy;

}

function SetwStatus() {
    var subject = prompt("Subject Line", _subject);

    if (subject != null) {

        var idWebinar = $("#Webinar_idWebinar").val();
        console.log(_subject + " " + subject);
        $.ajax({
            url: '/Webinar/SetStatus',
            type: 'GET',
            data: { "idWebinar": idWebinar, "status": valOfStatus, "subject": subject },
            dataType: "json",
            //contentType: "json",
            success: function (result) {
                $("#sendAll").show();
                if (result.status === "Scheduled") {
                    $(".setStatusBtn").text("Status set to: Pending");

                } else {
                    $(".setStatusBtn").text("Status set to: Scheduled");
                    valOfStatus = result.status;

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus + " " + errorThrown);
            }

        });
    }
};
function GetAffiliateCopy(affiliateId, isActive) {

    var currCopy = "";
    console.log("GetAffiliateCopy was passed: " + affiliateId + " isActive: " + isActive);

    // is this affiliate currently showing?  if so, grab Editor value rather than hidden text area
    // Addendum: this test is returning false at the point where true is expected. 
    if (isActive) {
        currCopy = $.trim($("#editorTA").wijeditor("getText"));
    } else {
        currCopy = $.trim($("#editor_" + affiliateId).val());

    }
    // HACK: the above test is returning false at the point where true is expected. 
    //currCopy = $.trim($("#editorTA").wijeditor("getText"));
    console.log(currCopy);
    currCopy = stripWijNull(currCopy);
    return currCopy;
}

function stripWijNull(copy) {
    // if it contains our special empty comment (staticComment / <!--WIJ-NULL-->) remove that
    return copy.replace(/<!--WIJ-NULL-->/g, "");
}

function SetCopyToClipboardTextArea(selectText) {
    var currCopy = GetCurrentEditorCopy();
    var $fld = $("#copyToClipboard");
    $fld.val(currCopy);
    if (selectText)
        $fld.select();
}

// not the cleanest function ever... pending a refactor?
function SetEditorTabForAffiliate(affiliateId, affiliateCopy) {
    var localCopy = affiliateCopy;

    //$.ajax({
    //    url: '/Admin/GetAffiliateTimeZone',
    //    type: 'POST',
    //    data: {
    //        "idAffiliate": affiliateId,
    //        "idWebinar": $("#Webinar_idWebinar").val()
    //    },
    //    dataType: "json",

    //    success: function (result) {
    //        console.log(result);
    //        SetAffTimeString(result.timeFormatDisplay);
    //        //timeString = result.timeFormatDisplay;
    //    },
    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
    //        alert(textStatus + " " + errorThrown);
    //    }
    //});

    // if it's blank, run master conversion

    //update by sjh while trying to adapt for affiliate's use of this method
    // -- can not find conditions where this test ever evaluates to true - strange
    // -- because logging affiliateId and localCopy will always show expected values
    // -- am hacking around by adding ( || currentUserIsAffiliate). Detects that affiliate (not admin)
    // -- is source of call.
    if ((affiliateId !== 0 &&
        localCopy === "") || currentUserIsAffiliate) {
        //$.trim(localCopy) == "") {
        //timeString = GetAffTimeString(affiliateId);

        var affObj = arrayLookup(affs, "idUserAff", affiliateId);

        if (affObj != null) {
            console.log(affObj);
            localCopy = replaceMasterTokensForAffiliate(affObj);
        }
    }

    $("#editor_" + affiliateId).val(localCopy);
    // do this right away even though it's not really needed, makes it easier to think about when debugging

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
        if (currentUserIsAffiliate) {
            setDataForAffiliate(currentUserId);
        }
    });

}

function SendAll($btn) {

    var subject = prompt("Subject Line", _subject);

    if (subject != null) {

        // setup UI for user feedback
        var origBtnText = $btn.text();
        $btn.text("Saving...");
        $btn
            .append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');

        var errorAffs = [];
        var callsNeeded = $(".tab-pane").length - 1; // don't count Master tab, we're skipping that one
        var callsComplete = 0;
        var messages = [];

        // loop through all tabs...
        $(".tab-pane")
            .each(function (idx) {
                var $tab = $(this);
                var affiliateId = $tab.data("pane-affid");

                if (affiliateId == 0)
                    // skip the Master tab, although, conceivably, we could store that in a special file and use it for something...
                    return;

                // get affiliate specific copy
                var isActive = $tab
                    .hasClass("active");
                // should ALWAYS be false in this method, as the Save All button is only on the master tab
                var currCopy = GetAffiliateCopy(affiliateId, isActive);

                var affObj = arrayLookup(affs, "idUserAff", affiliateId);
                //currCopy = replaceMasterTokensForAffiliate(affObj);

                if (currCopy == "") // if they don't have customized copy we'll need to create it
                {
                    // generate from master...
                    
                    var affObj = arrayLookup(affs, "idUserAff", affiliateId);
                    if (affObj != null) {

                        console.log(affObj);
                        currCopy = replaceMasterTokensForAffiliate(affObj); // always pulls from editor_0 (master)
                        
                    }
                }


                $.ajax({
                    url: '/Admin/SendSinglePromo',
                    type: 'POST',
                    data: {
                        "affiliateId": affiliateId,
                        "subject": subject,
                        "messageBodyHtml": currCopy,
                        "webinarId": $("#Webinar_idWebinar").val(),
                        "sendDate": $("#SendDate").val()
                    },
                    dataType: "json",
                    success: function (result) {

                        //add feedback to promo sender
                        //$('#PromoSentToAffiliate').text($('#PromoSentToAffiliate').text() + "\n" + result.)
                        //$('#send' + affID).text("Success");
                        console.log(result);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        // $('#send' + affID).text(textStatus + " " + errorThrown);
                        alert("error " + textStatus + " " + errorThrown);
                    }
                });
                // remove spinner
                $('#submitSpinWrapper').remove();
                $btn.text("Send!");

                setTimeout(function () { $btn.text(origBtnText); }, 2000);

            });
    }
}

function CreateCampaign($btn, affiliateId) {

    var _sendTime = 10;
    var sendTime = prompt("Time To Send: ", _sendTime);
    // save time...
    if (sendTime) {

        var subjectForCampaign = prompt("Subject line: ", _subjectForCampaign);
        
        // setup UI for user feedback
        var origBtnText = $btn.text();
        $btn.text("Processing...");
        $btn
            .append('<span id="submitSpinWrapper">&nbsp;<span class=""><i id="spinner" class="icon-spinner icon-spin"></i></span></span>');

        var errorAffs = [];
        //var callsNeeded = $(".tab-pane").length - 1; // don't count Master tab, we're skipping that one
        var callsComplete = 0;
        var messages = [];

        var $tab = $(this);

        if (affiliateId == 0)
            // skip the Master tab, although, conceivably, we could store that in a special file and use it for something...
            return;

        // get affiliate specific copy
        var isActive = $tab
            .hasClass("active");
        // should ALWAYS be false in this method, as the Save All button is only on the master tab
        var currCopy = GetAffiliateCopy(affiliateId, true);

        $.ajax({
            url: '/Admin/GenerateMailChimpCampaign',
            type: 'POST',
            data: {
                "affiliateId": affiliateId,
                "messageBodyHtml": currCopy,
                "webinarId": $("#Webinar_idWebinar").val(),
                "sendDate": $("#SendDate").val(),
                "sendTime": sendTime,
                "subject": subjectForCampaign
            },
            dataType: "json",
            async: false,
            //contentType: "application/json",
            //contentType: "json",
            success: function (Result) {
                console.log(Result);
                if (Result.Success) {

                    alert("Campaign Created");

                } else {

                    alert("error: " + Result);

                }
                //$('#send' + affID).text("Success");
            },
            error: function (result) {

                // $('#send' + affID).text(textStatus + " " + errorThrown);

            },
            complete: function (result) {
                
            }
        });
        // remove spinner
        $('#submitSpinWrapper').remove();
        $btn.text("Complete!");

        //setTimeout(function () { $btn.text(origBtnText); }, 2000);
    }
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
                //"Affiliate.ContactEmail": $("#SendToList_" + affiliateId).val(),
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
            if (callsComplete >= callsNeeded) {
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
            //"Affiliate.ContactEmail": $("#SendToList_" + affiliateId).val(),
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

    var subject = prompt("Subject Line", _subject);

    if (subject != null) {

        //alert($("#SendToList_" + affiliateId).val());
        var currCopy = GetCurrentEditorCopy();


        $.ajax({
            url: '/Admin/SendSinglePromo',
            type: 'POST',
            data: {
                "affiliateId": affiliateId,
                "messageBodyHtml": currCopy,
                "webinarId": $("#Webinar_idWebinar").val(),
                subject
            },
            dataType: "json",
            //contentType: "json",
            success: function (result) {
                //$('#send' + affID).text("Success");
                alert('Success');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // $('#send' + affID).text(textStatus + " " + errorThrown);
                alert("Error: " + errorThrown);
            }
        });
    }
}

// unforatunately there is a server-side version of this function in the 
//AdminController file as well...
function replaceMasterTokensForAffiliate(aff) {
    var string = "";

    $.ajax({
        url: '/Admin/GetAffiliateTimeZoneAndList',
        type: 'POST',
        data: {
            "idAffiliate": aff.idUserAff,
            "idWebinar": $("#Webinar_idWebinar").val()
        },
        async: false,
        dataType: "json",

        success: function (result) {
            if (result.listName !== "") {
                console.log(result.listName);
                $("#opCaption").text("MC list name: " + result.listName);
            } else {
                $("#opCaption").text("");
            }
            string = result.timeFormatDisplay;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + " " + errorThrown);
        }
    });
    var copy = $("#editor_0").val(); // 0 is master
    copy = copy.replace(/\{aff_EmailBanner\}/gi, aff.EmailBanner);
    copy = copy.replace(/\{aff_EmailFooter\}/gi, aff.EmailFooter);
    copy = copy.replace(/\{aff_ttsdomain\}/gi, aff.ttsDomain);
    copy = copy.replace(/\{aff_idUserAff\}/gi, aff.idUserAff);
    //copy = copy.replace(/\{aff_timeZone\}/gi, aff.timeZone);
    copy = copy.replace(/\{aff_ContactPerson\}/gi, aff.ContactPerson);
    copy = copy.replace(/\{aff_NotiPromos\}/gi, aff.NotiPromos);
    copy = copy.replace(/\{aff_ContactPhone\}/gi, aff.ContactPhone);
    //copy = copy.replace(/\{aff_ContactPhone\}/gi, aff.ContactPhone);
    copy = copy.replace(/\{timeString\}/gi, string);
    console.log("GetAffTimeString is: " + string);
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