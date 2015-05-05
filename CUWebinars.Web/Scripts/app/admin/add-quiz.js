/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var ADDQUIZ = {}; // object to holds all references and methods.

var AQ = ADDQUIZ; // create shortcut alias

// document.ready function
$(function () {

    AQ.addQuestionTextButton = $('#addQuestionText');
    AQ.questionTextInput = $('#questionText');
    AQ.newQuestionTextSpan = $('#newQuestionTextSpan');
    AQ.newQuestionTextDiv = $('#newQuestionTextDiv');
    AQ.editQuestionTextButton = $('#editQuestionTextButton');
    AQ.questionTextAreaWrapper = $('#questionTextAreaWrapper');
    AQ.questionsWrapper = $('#questionsWrapper');
    AQ.optionsWrapper = $('#optionsWrapper');
    AQ.addOptionButton = $('#addOptionButton');
    AQ.addQuizButton = $('#addQuizButton');
    AQ.addNextQuizButton = $('#addNextQuizButton');
    AQ.selectedWebinar = $('#SelectedWebinar');
    AQ.selectedWebinar.val('');

    AQ.addQuestionTextButton.on('click', function(e) {

        e.preventDefault();

        var text = AQ.questionTextInput.val();

        AQ.questionTextAreaWrapper.fadeOut(400, function () {
            AQ.newQuestionTextDiv.fadeIn(400, function() {
                AQ.newQuestionTextSpan.text(text);
                AQ.editQuestionTextButton.fadeIn(400);
                $(this).removeClass('initialHide');
            });
            AQ.optionsWrapper.fadeIn(400, function () { $(this).removeClass('initialHide'); });
        });

        $(this).fadeOut(400);

    });

    AQ.addOptionButton.on('click', function(e) {

        e.preventDefault();

        var currentCount = AQ.getOptionsCount();

        if (currentCount < 1) {
            AQ.optionsWrapper.after($('<button>',
            {
                id: 'addNewQuestionButton',
                text: 'Add New Question',
                'class': 'btn btn-mini btn-primary',
            }));

            $('#addNewQuestionButton').on('click', AQ.addNewQuestionToList);
        }

        AQ.optionsWrapper.append(AQ.optionInputHtml.format(currentCount, String.fromCharCode(97 + currentCount)));
        $('#optionInput-' + currentCount).focus();

    });

    AQ.addQuizButton.on('click', function (e) {

        e.preventDefault();

        var url = $('#AddQuizForm').attr('action');

        var webinar = AQ.selectedWebinar.val();
        var questions = AQ.getQuestions();

        var payload = {
            SelectedWebinar: webinar,
            Questions: questions 
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                AQ.addQuizButton.append('&nbsp;<i id="addQuizSpinner" class="icon-spinner icon-spin"></i>');
                $('#addQuizResult').remove();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                AQ.addQuizButton.after('<span id="addQuizResult">&nbsp;<span class="label label-success"><span> Quiz has been added! </span></span></span>').fadeIn(200).before().hide();
                $('#addQuizResult')
                AQ.selectedWebinar.val('');
                AQ.newQuestionTextSpan.text('');
                AQ.questionTextAreaWrapper.show();
                AQ.newQuestionTextDiv.hide();
                AQ.addNextQuizButton.show();
            } else if (!data.isSuccessful) {
                
            } else {

            }
            $('#addQuizSpinner').remove();
        });
    });

    AQ.addNextQuizButton.on('click', function(e) {
        e.preventDefault();

        AQ.questionsWrapper.empty();
        $('#addQuizResult').fadeOut(400);
        $(this).fadeOut(400);
        AQ.addQuizButton.show();
    });

    AQ.selectedWebinar.focus();
});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionsList = [];

    ns.optionInputHtml = '<div id="option-{0}" class="input-prepend input-append"><span class="add-on">{1}</span><input id="optionInput-{0}" name="Options[{0}].Text" class="input input-xxlarge" placeholder="Enter option text and tick box at end if correct answer"  type="text" />&nbsp;<span class="add-on"><input id="answer-{0}" type="checkbox" title="Is this the answer?"/>&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}-delete"></i></span></div>';

    ns.getOptionsCount = function () {
        return AQ.optionsWrapper.find('input[type="text"]').length;
    };

    ns.addNewQuestionToList = function(e) {
        e.preventDefault();
        
        AQ.newQuestion = null;
        AQ.newQuestion = new QuizDomain.Question();
        AQ.newQuestion.setText(AQ.newQuestionTextSpan.text());
        AQ.questionsList.push(AQ.newQuestion);
        AQ.newQuestion.setOptions(AQ.getOptionsForNewQuestion());

        AQ.questionsWrapper.append('<div id="{0}" class="quSummary"><div class="quSummaryPart">Question {1}</div><div class="quSummaryPart"><i class="icon-trash icon-white" id="qdelete-{2}"></i></div><div class="quSummaryPart"><i class="icon-pencil icon-white" id="qedit-{2}"></i></div></div>'.format('question-' + AQ.numberOfQuestions(), AQ.numberOfQuestions()));

        AQ.clearOptionsWrapper();
        AQ.optionsWrapper.hide();

        AQ.newQuestionTextDiv.hide();
        AQ.questionTextInput.val('');
        AQ.questionTextAreaWrapper.show();
        AQ.addQuestionTextButton.show();

        $(this).remove();

        AQ.addQuizButton.removeClass('initialHide'); // with first question created, display the 'add quiz' button.
    };

    ns.clearOptionsWrapper = function() {
        
        var optionInputDivs = AQ.optionsWrapper.find('div[id^="option-"]');

        _.each(optionInputDivs, function (input) {
            input.remove();
        });

    };

    ns.getOptionsForNewQuestion = function() {

        var optionInputs = AQ.optionsWrapper.find('input[type="text"]');

        var optionsList = [];

        _.each(optionInputs, function(input) {
            var input$ = $(input); 
            var option = new QuizDomain.Option();

            option.setText(input$.val());
            option.setLetter(input$.prev().text());
            option.setCorrectAnswer(input$.parent().find('input[type="checkbox"]').prop('checked'));

            optionsList.push(option);
        });

        return optionsList;
    };

    ns.numberOfQuestions = function() { return AQ.questionsList.length; };

    ns.getQuestions = function() {

        var options;
        var questions = [];

        _.each(AQ.questionsList, function (question) {

            options = null;
            options = [];

            _.each(question.getOptions(), function(option) {

                var opt = { Text: option.getText() };
                var questionWithOption = {
                    Letter: option.getLetter(),
                    CorrectAnswer: option.getCorrectAnswer(),
                    Option: opt
                };

                options.push(questionWithOption);
            });

            questions.push({
                Text: question.getText(),
                QuestionWithOptions: options
            });
        });

        return questions;
    };

})(AQ);