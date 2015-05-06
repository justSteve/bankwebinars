/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var DOQUIZ = {}; // object to holds all references and methods.

var DQ = DOQUIZ; // create shortcut alias

// document.ready function
$(function () {

    DQ.primeDomVariables();

    DQ.startButton.on('click', DQ.startQuiz);
    DQ.prevButton.on('click', DQ.prevClicked);
    DQ.nextButton.on('click', DQ.nextClicked);

});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionCount = 0;
    ns.currentQuestionNr = 1;

    ns.questionTemplateFirst = '<div id="{0}-question"><p id="{0}-questionText">{1}</p><div id="{0}-options">{2}</div></div>';
    ns.questionTemplate = '<div id="{0}-question" class="initialHide"><p id="{0}-questionText">{1}</p><div id="{0}-options">{2}</div></div>';
    ns.optionsTemplate = '<label class="radio"><input id="{0}-option" name="option" type="radio" value="{1}" />{2}</label>';

    ns.primeDomVariables = function() {
        DQ.startButton = $('#startButton');
        DQ.startPanel = $('#startPanel');
        DQ.questionPanel = $('#questionPanel');
        DQ.questionsWrapper = $('#questionWrapper');
        DQ.prevButton = $('#prev');
        DQ.nextButton = $('#next');
    };

    ns.startQuiz = function(e) {
        e.preventDefault();

        DQ.setUiState('start');
        DQ.getQuestions();
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

                _.each(DQ.quiz.getQuestions(), function(question, idx) {

                    var optionsHtml = '';

                    _.each(question.getOptions(), function(option, idx) {
                        optionsHtml += DQ.optionsTemplate.format((idx + 1).toString(), option.getLetter(), option.getText());
                    });

                    if (idx === 0) {
                        var questionHtml = DQ.questionTemplateFirst.format((idx + 1).toString(), question.getText(), optionsHtml);
                    } else {
                        var questionHtml = DQ.questionTemplate.format((idx + 1).toString(), question.getText(), optionsHtml);
                    }

                    DQ.questionsWrapper.append(questionHtml);
                });

            } else {

            }
        });


    };

    ns.populateClientObjects = function(data) {
        DQ.quiz = new QuizDomain.Quiz();
        DQ.quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        DQ.quiz.setOrderId(data.OrderId);
        DQ.quiz.setWebinarId(data.WebinarId);
        DQ.quiz.setWebUserId(data.WebUserId);
        
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

        DQ.quiz.setQuestions(questions);
        DQ.questionCount = DQ.quiz.getQuestions().length;
    };

    ns.setUiState = function(state) {

        switch(state) {
            case 'start':
                DQ.startPanel.fadeOut(400, function() {
                    DQ.questionPanel.fadeIn(400, function () {
                        $(this).removeClass('initialHide');
                    });
                });
                break;
            default:
                break;
        }


    };

    ns.nextClicked = function(e) {
        e.preventDefault();

        if (DQ.currentQuestionNr + 1 === DQ.questionCount) {
            $(this).attr('disabled', 'disabled');
        } else {
            DQ.dealWithDisabled($(this));
        }

        if (DQ.currentQuestionNr === 1) {
            DQ.prevButton.removeAttr('disabled');
        }

        $('#' + DQ.currentQuestionNr + '-question').fadeOut(400, function() {

            DQ.currentQuestionNr += 1;

            $('#' + DQ.currentQuestionNr + '-question').fadeIn(400, function () {
                if($(this).hasClass('initialHide'))
                    $(this).removeClass('initialHide');
            });

        });

    };

    ns.prevClicked = function (e) {
        e.preventDefault();

        if (DQ.currentQuestionNr - 1 === 1) {
            $(this).attr('disabled', 'disabled');
        } else {
            DQ.dealWithDisabled($(this));
        }

        if (DQ.currentQuestionNr === DQ.questionCount) {
            DQ.nextButton.removeAttr('disabled');
        }

        $('#' + DQ.currentQuestionNr + '-question').fadeOut(400, function () {

            DQ.currentQuestionNr -= 1;

            $('#' + DQ.currentQuestionNr + '-question').fadeIn(400);

        });
    };

    ns.dealWithDisabled = function(jqueryObject) {
        var disabledAttr = jqueryObject.attr('disabled');

        if (typeof disabledAttr !== typeof 'undefined' && disabledAttr !== false) {
            jqueryObject.removeAttr('disabled');
        }
    };
})(DQ);