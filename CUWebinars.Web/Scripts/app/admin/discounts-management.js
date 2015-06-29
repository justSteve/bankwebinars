/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var DiscountSMANAGEMENT = {};

var DM = DiscountSMANAGEMENT;

$(function () {

    DM.stockDiscountsVisible = true;
    DM.addDiscountMsgLabelWrap = $('#addDiscountMsgLabelWrap');
    DM.addDiscountForm = $('#addDiscountForm');
    DM.validationErrors = $('#validationErors');
    DM.addDiscountFormWrapper = $('#addDiscountFormWrapper');
    DM.userEmail = $('#UserEmail');
    DM.emailInput = $('#emailInput');
    DM.populateDiscountsButton = $('#popDiscounts');
    DM.viewDiscountsPanel = $('#viewDiscountsPanel');

    DM.addDiscountForm.on('submit', DM.addDiscount);

    $('#addDiscountsTab a').on('shown', function (e) {
        e.preventDefault();

        if (!DM.addDiscountFormWrapper.is(':visible'))
            DM.addDiscountFormWrapper.show();

        DM.userEmail.val(DM.emailInput.val());
    });

    $('#popDiscountsForm').on('submit', DM.populateDiscounts);


    $('#useCustomFromListCheck').on('change', DM.toggleControls);
    $('#useFrameworkCheck').on('change', DM.toggleControls);
    $('#useCustomFromTextCheck').on('change', DM.toggleControls);

    DM.userEmail.addClass('input');
    DM.userEmail.addClass('input-xlarge');
    DM.emailInput.focus();
});

(function (ns) {

    DM.addDiscount = function (e) {

        e.preventDefault();

        DM.validationErrors.empty();

        //  First, sort out the Antiforgery token for json POST
        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var formInputs = DM.getRelevantInputs();

        $.each(formInputs, function (idx, value) {
            $(value).css('background-color', '#FFFFFF');
        });

        var newDiscountType = $('#NewDiscountType');
        var url = DM.addDiscountForm.attr('action');

        // Find the relevant input for the DiscountType which is not disabled.
        var enabledSelectElement = _.filter(formInputs, function (element, index, collection) {
            if (element.name === 'SelectedDiscountType' || element.name === 'CustomDiscountType') {
                return element;
            }
        });

        var wrappedEnabledElement = $(enabledSelectElement[0]);

        if (enabledSelectElement[0].name === 'SelectedDiscountType') {
            if (wrappedEnabledElement.find(':selected').text() === 'Choose a Discount') {
                DM.validationErrors.append('<span class="label label-important"><i class="icon icon-exclamation-sign"></i>You need to select a Discount type</span>');
                wrappedEnabledElement.css('background-color', '#f2bcbc');
                return;
            }
        } else {
            if (wrappedEnabledElement.val() === '') {
                DM.validationErrors.append("<span class='label label-important'><i class='icon icon-exclamation-sign'></i>You need to enter or select a Discount type</span>");
                wrappedEnabledElement.css('background-color', '#f2bcbc');
                return;
            }
        }


        newDiscountType.val(wrappedEnabledElement.val());

        var inputs = formProcessor.getApplicableInputs('addDiscountForm');

        var payload = formProcessor.processInputs(inputs);

        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(payload),
            dataType: constants.JsonDataType,
            contentType: constants.JsonContentType,
            headers: headers,
            beforeSend: function (xhr) {
                DM.addDiscountMsgLabelWrap.show();
                DM.addDiscountMsgLabelWrap.html('<span id="feedbackLabel" class="label label-default">&nbsp;<i class="icon-spinner icon-spin "></i>&nbsp;Adding Discount ...</span>');
                DM.validationErrors.empty();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                DM.addDiscountMsgLabelWrap.html('<span id="feedbackLabel" class="label label-success">&nbspDiscount added!</span>');
            } else if (data.Result === 'Fail') {
                DM.addDiscountMsgLabelWrap.html('<span id="feedbackLabel" class="label label-important">&nbsp' + data.Msg + '</span>');
            } else {
                DM.addDiscountMsgLabelWrap.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Error ...</span>');
                formProcessor.lightUpValidationSummary('addDiscountSummary', data);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            DM.addDiscountMsgLabelWrap.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Server Error ...</span>');
        });

    };

    DM.disableOtherInputs = function (enabledInput, checkboxName, form) {
        form.find('input[type=text]').not('#UserEmail, #NewDiscountValue').val('').attr('disabled', 'disabled').removeAttr('style');
        form.find('input[type=checkbox]').not('#' + checkboxName).prop('checked', false);
        form.find('select').val('').attr('disabled', 'disabled').removeAttr('style');
        enabledInput.removeAttr('disabled');
    };

    DM.toggleControls = function () {

        if (!$(this).is(':checked')) {
            $(this).prop('checked', true);
            return;
        }

        var input = $(this).prev();
        input.removeAttr('disabled');
        DM.disableOtherInputs(input, $(this).attr('id'), DM.addDiscountForm);
    };

    DM.getRelevantInputs = function () {
        return DM.addDiscountForm.find('input, select').not('input[disabled=disabled], select[disabled=disabled], input[type=checkbox], [name="__RequestVerificationToken"]');
    };

    DM.deleteDiscount = function (e) {

        e.preventDefault();

        var self = $(this);

        var userDiscountCell = $(this).parent().prevAll().eq(1);
        var DiscountValueCell = userDiscountCell.next();

        var payload = {
            email: DM.emailInput.val(),
            Discount: userDiscountCell.text(),
            DiscountValue: DiscountValueCell.text()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            data: JSON.stringify(payload),
            url: '/admin/DeleteDiscount',
            dataType: constants.JsonDataType,
            beforeSend: function () {
                self.after('<span id="deleteDiscountsSpinner">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
            }
        }).done(function (data) {

            $('#deleteDiscountsSpinner').remove();

            if (data.Result === 'Success') {
                userDiscountCell.parent().fadeOut(500, function () {
                    $(this).remove();
                });
            }

        });
    };

    DM.populateDiscounts = function (e) {

        e.preventDefault();

        var url = $(this).attr('action');
        var payload = { email: DM.emailInput.val() };

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
            beforeSend: function () {
                DM.populateDiscountsButton.append('<span id="getDiscountsSpinner">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
                DM.viewDiscountsPanel.html('<span id="bigGetDiscountsSpinner" style="font-size: 20px">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
            }
        }).done(function (data) {
            $('#viewDiscountsPanel').html(data);

            DM.DiscountsTable = $('#DiscountsTable');

            DM.DiscountsTable.find('tr td > i').on('click', DM.deleteDiscount);

            $('#getDiscountsSpinner').remove();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            DM.viewDiscountsPanel.html('<span id="feedbackLabel" class="label label-important">&nbsp;<i class="fa fa-exclamation-circle"></i>&nbsp;Server Error ...</span>');
        });
    };

})(CM);