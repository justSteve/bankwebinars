/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var DOQUIZ = {}; // object to holds all references and methods.

var DQ = DOQUIZ; // create shortcut alias

// document.ready function
$(function () {

    DQ.startButton = $('#startButton');
    DQ.startPanel = $('#startPanel');

    DQ.startButton.on('click', DQ.startQuiz);

});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionsList = [];

    ns.startQuiz = function(e) {
        e.preventDefault();

        DQ.getQuestions();

        DQ.setUiState('start');
    };

    ns.getQuestions = function () {

        var payload = {
            quizCode: $('#codeInput').val(),
            orderId: $('#orderIdInput').val()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Quiz/GetQuestions',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {

            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                DQ.populateClientObjects(data.Quiz);
            } else {

            }
        });


    };

    ns.populateClientObjects = function(data) {
        var quiz = new QuizDomain.Quiz();
        quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        quiz.setOrderId(data.OrderId);
        quiz.setWebinarId(data.WebinarId);
        quiz.setWebUserId(data.WebUserId);

        var questions = [];

        _.each(data.QuizQuestions, function(item, index) {

            var question = new QuizDomain.Question();
            question.setText(item.QuestionText);
            question.setQuestionNumber(item.QuestionNumber);

            var options = [];

            _.each(item.Options, function(option, index) {
                var optionForQu = new QuizDomain.Option();
                optionForQu.setText(option.Text);
                optionForQu.setLetter(option.Letter);
                options.push(optionForQu);
            });

            question.setOptions(options);
            questions.push(question);

        });

    };

    ns.setUiState = function(state) {

        switch(state) {
            case 'start':
                DQ.startPanel.fadeOut(400);
                break;
            default:
                break;
        }


    };

})(DQ);