var formProcessor = {};

(function (ns) {

    ns.processInputs = function (inputs) {
        
        var dataAsArrays = _.map(inputs, function (input) {
            var wrapInput = $(input);
            return [wrapInput.attr('name'), wrapInput.val()];
        });

        return _.object(dataAsArrays);

    };

    ns.clearInputs = function(inputs) {
        _.each(inputs, function (input) {
            var wrapInput = $(input);
            wrapInput.val('');
        });
    };

    ns.getApplicableInputs = function(formId) {
        return $('#' + formId + ' input, textarea, select').not(':input[type=button], :input[type=submit], :input[type=reset], [name="__RequestVerificationToken"], :input[type=checkbox]');
    };

    ns.lightUpValidationSummary = function (id, data) {

        var valSummary = $('#' + id);
        valSummary.removeClass('validation-summary-valid').addClass('validation-summary-errors');

        var errorsList = valSummary.find('ul');
        errorsList.empty();

        for (var error in data.data) {
            if (data.data.hasOwnProperty(error)) {
                errorsList.append('<li>' + data.data[error] + '</li>');
                //console.log(data.data[error]);
            }
        }
    };

    ns.clearValidationSummary = function (valSummary) {
        
        valSummary.removeClass('validation-summary-errors').addClass('validation-summary-valid');

        var errorsList = valSummary.find('ul');
        errorsList.empty();
        errorsList.append('<li style="display:none"></li>');
    };

})(formProcessor)