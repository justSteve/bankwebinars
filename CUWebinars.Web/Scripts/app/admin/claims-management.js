/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var CLAIMSMANAGEMENT = {};

var CM = CLAIMSMANAGEMENT;

$(function () {

    CM.stockClaimsVisible = true;
    CM.addClaimMsgLabelWrap = $('#addClaimMsgLabelWrap');
    CM.addClaimForm = $('#addClaimForm');
    CM.validationErrors = $('#validationErors');
    CM.addClaimFormWrapper = $('#addClaimFormWrapper');
    CM.userEmail = $('#UserEmail');
    CM.emailInput = $('#emailInput');
    CM.populateClaimsButton = $('#popClaims');
    CM.viewClaimsPanel = $('#viewClaimsPanel');

    CM.addClaimForm.on('submit', CM.addClaim);
    
    $('#addClaimsTab a').on('shown', function(e) {
        e.preventDefault();

        if(!CM.addClaimFormWrapper.is(':visible'))
            CM.addClaimFormWrapper.show();

        CM.userEmail.val(CM.emailInput.val());
    });

    $('#popClaimsForm').on('submit', CM.populateClaims);


    $('#useCustomFromListCheck').on('change', CM.toggleControls);
    $('#useFrameworkCheck').on('change', CM.toggleControls);
    $('#useCustomFromTextCheck').on('change', CM.toggleControls);

    CM.userEmail.addClass('input');
    CM.userEmail.addClass('input-xlarge');
    CM.emailInput.focus();
});

(function(ns) {

    CM.addClaim = function(e) {

        e.preventDefault();

        CM.validationErrors.empty();

        //  First, sort out the Antiforgery token for json POST
        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var formInputs = CM.getRelevantInputs();

        $.each(formInputs, function(idx, value) {
            $(value).css('background-color', '#FFFFFF');
        });

        var newClaimType = $('#NewClaimType');
        var url = CM.addClaimForm.attr('action');

        // Find the relevant input for the ClaimType which is not disabled.
        var enabledSelectElement = _.filter(formInputs, function(element, index, collection) {
            if (element.name === 'SelectedClaimType' || element.name === 'CustomClaimType') {
                return element;
            }
        });

        var wrappedEnabledElement = $(enabledSelectElement[0]);

        if (enabledSelectElement[0].name === 'SelectedClaimType') {
            if (wrappedEnabledElement.find(':selected').text() === 'Choose a Claim') {
                CM.validationErrors.append('<span class="label label-important"><i class="icon icon-exclamation-sign"></i>You need to select a claim type</span>');
                wrappedEnabledElement.css('background-color', '#f2bcbc');
                return;
            }
        } else {
            if (wrappedEnabledElement.val() === '') {
                CM.validationErrors.append("<span class='label label-important'><i class='icon icon-exclamation-sign'></i>You need to enter or select a claim type</span>");
                wrappedEnabledElement.css('background-color', '#f2bcbc');
                return;
            }
        }


        newClaimType.val(wrappedEnabledElement.val());

        var inputs = formProcessor.getApplicableInputs('addClaimForm');

        var payload = formProcessor.processInputs(inputs);

        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(payload),
            dataType: constants.JsonDataType,
            contentType: constants.JsonContentType,
            headers: headers,
            beforeSend: function(xhr) {
                CM.addClaimMsgLabelWrap.show();
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-default">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Adding claim ...</span>');
                CM.validationErrors.empty();
            }
        }).done(function(data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-success">&nbspClaim added!</span>');
            } else {
                CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Error ...</span>');
                formProcessor.lightUpValidationSummary('addClaimSummary', data);
            }
        }).fail(function(jqXHR, textStatus, errorThrown) {
            CM.addClaimMsgLabelWrap.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Server Error ...</span>');
        });

    };

    CM.disableOtherInputs = function (enabledInput, checkboxName, form) {
        form.find('input[type=text]').not('#UserEmail, #NewClaimValue').val('').attr('disabled', 'disabled').removeAttr('style');
        form.find('input[type=checkbox]').not('#' + checkboxName).prop('checked', false);
        form.find('select').val('').attr('disabled', 'disabled').removeAttr('style');
        enabledInput.removeAttr('disabled');
    };

    CM.toggleControls = function () {

        if (!$(this).is(':checked')) {
            $(this).prop('checked', true);
            return;
        }

        var input = $(this).prev();
        input.removeAttr('disabled');
        CM.disableOtherInputs(input, $(this).attr('id'), CM.addClaimForm);
    };

    CM.getRelevantInputs = function() {
        return CM.addClaimForm.find('input, select').not('input[disabled=disabled], select[disabled=disabled], input[type=checkbox], [name="__RequestVerificationToken"]');
    };

    CM.deleteClaim = function(e) {

        e.preventDefault();

        var self = $(this);

        var userClaimCell = $(this).parent().prevAll().eq(1);
        var claimValueCell = userClaimCell.next();

        var payload = {
            email: CM.emailInput.val(),
            claim: userClaimCell.text(),
            claimValue: claimValueCell.text()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            data: JSON.stringify(payload),
            url: '/admin/DeleteClaim',
            dataType: constants.JsonDataType,
            beforeSend: function () {
                self.after('<span id="deleteClaimsSpinner">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
            }
        }).done(function (data) {

            $('#deleteClaimsSpinner').remove();

            if (data.Result === 'Success') {
                userClaimCell.parent().fadeOut(500, function () {
                    $(this).remove();
                });
            }

        });
    };

    CM.populateClaims = function(e) {

        e.preventDefault();

        var url = $(this).attr('action');
        var payload = { email: CM.emailInput.val() };

        //  First, sort out the Antiforgery token for json POST
        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            data: JSON.stringify(payload),
            url: url,
            dataType: constants.HtmlDataType,
            headers: headers,
            beforeSend: function() {
                CM.populateClaimsButton.append('<span id="getClaimsSpinner">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
                CM.viewClaimsPanel.html('<span id="bigGetClaimsSpinner" style="font-size: 20px">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
            }
        }).done(function(data) {
            $('#viewClaimsPanel').html(data);

            CM.claimsTable = $('#claimsTable');

            CM.claimsTable.find('tr td > i').on('click', CM.deleteClaim);

            $('#getClaimsSpinner').remove();
        }).fail(function(jqXHR, textStatus, errorThrown) {
            CM.viewClaimsPanel.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Server Error ...</span>');
        });
    };

})(CM);