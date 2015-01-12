/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var CLAIMSMANAGEMENT = {};

var CM = CLAIMSMANAGEMENT;

$(function () {

    CM.stockClaimsVisible = true;
    CM.addClaimMsgLabelWrap = $('#addClaimMsgLabelWrap');

    $('#addClaimLabel').on('click', function (e) {

        e.preventDefault();

        if (CM.stockClaimsVisible) {
            $('#stockClaimsWrapper').fadeOut(500, function () {
                $('#customClaimWrapper').show();
            });

            CM.stockClaimsVisible = false;
            $(this).text('Pick Stock Claim');
        } else {
            $('#customClaimWrapper').fadeOut(500, function () {
                $('#stockClaimsWrapper').show();
            });
            CM.stockClaimsVisible = true;
            $(this).text('Add custom claim');
        }
    });

    $('#createClaimButton').on('click', function (e) {
        e.preventDefault();

        var newClaimType = $('#NewClaimType');
        var url = $('#addClaimForm').attr('action');

        //  First, sort out the Antiforgery token for json POST
        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        if ($('#stockClaimsWrapper').is(':visible')) {
            newClaimType.val($('#SelectedClaimType').val());
        } else {
            newClaimType.val($('#CustomClaimType').val());
        }
        
        var inputs = formProcessor.getApplicableInputs('addClaimForm');

        var payload = formProcessor.processInputs(inputs);

        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(payload),
            dataType: constants.JsonDataType,
            contentType: constants.JsonContentType,
            headers: headers,
            beforeSend: function (xhr) {
                CM.addClaimMsgLabelWrap.show();
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-default">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Adding claim ...</span>');
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'success') {
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-success">&nbspClaim added!</span>');
            } else {
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-danger">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Error ...</span>');
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-danger">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Server Error ...</span>');
        });

    });
});