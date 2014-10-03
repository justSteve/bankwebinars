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

})(formProcessor)