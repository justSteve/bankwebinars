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
    DQ.submitButton.on('click', DQ.submitClicked);
    DQ.retakeButton.on('click', DQ.retakeClicked);
    DQ.homeButton.on('click', DQ.homeClicked);

    DQ.utilities = new Common.Utilities();
});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionCount = 0;
    ns.currentQuestionNr = 1;

    ns.questionTemplateFirst = '<div id="{0}-question"><p><strong>Question Number {3} of {4}</strong></p><p id="{0}-questionText">{1}</p><div id="{0}-options">{2}</div></div>';
    ns.questionTemplate = '<div id="{0}-question" class="initialHide"><p><strong>Question Number <span id="{0}-questionNr"></span> of {3}</strong></p><p id="{0}-questionText">{1}</p><div id="{0}-options">{2}</div></div>';
    ns.optionsTemplate = '<label class="radio"><input id="{0}-option" name="{3}-option" type="radio" value="{1}" />{2}</label>';

    ns.primeDomVariables = function() {
        DQ.startButton = $('#startButton');
        DQ.startPanel = $('#startPanel');
        DQ.questionPanel = $('#questionPanel');
        DQ.questionsWrapper = $('#questionWrapper');
        DQ.prevButton = $('#prev');
        DQ.nextButton = $('#next');
        DQ.submitButton = $('#submitAnswersButton');
        DQ.scores = $('#scores');
        DQ.retakeButton = $('#retakeButton');
        DQ.homeButton = $('#homeButton');
    };

    ns.wireUpOptions = function(jQuerySet) {

        // this wires up the change handler for each radio button.
        _.each(jQuerySet, function(jQueryObj) {
            $(jQueryObj).on('change', DQ.optionSelected);
        });
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

                var count = 1;

                _.each(DQ.quiz.getQuestions(), function(question, idx) {

                    var optionsHtml = '';

                    _.each(question.getOptions(), function(option, innerIdx) {
                        optionsHtml += DQ.optionsTemplate.format((idx + 1).toString() + (innerIdx + 1).toString(), option.getLetter(), option.getText(), count);
                    });

                    if (idx === 0) {
                        var questionHtml = DQ.questionTemplateFirst.format((idx + 1).toString(), question.getText(), optionsHtml, DQ.currentQuestionNr, DQ.questionCount);
                    } else {
                        var questionHtml = DQ.questionTemplate.format((idx + 1).toString(), question.getText(), optionsHtml, DQ.questionCount);
                    }

                    DQ.questionsWrapper.append(questionHtml);

                    count += 1;
                });

                DQ.wireUpOptions($('#questionWrapper').find('input[type="radio"]'));

            } else {

            }
        });


    };

    ns.populateClientObjects = function(data) {
        DQ.quiz = new QuizDomain.Quiz();
        DQ.quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        DQ.quiz.setOrderId(data.OrderId);
        DQ.quiz.setWebinarId(data.WebinarId);
        DQ.quiz.setEmail($('#emailInput').val());
        DQ.quiz.setQuizId($('#quizIdInput').val());
        
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
            case 'restart':
                DQ.prevButton.attr('disabled', 'disabled');
                DQ.nextButton.removeAttr('disabled');
                DQ.submitButton.removeAttr('disabled').hide();
                DQ.retakeButton.hide();

                DQ.scores.hide();

                $('#' + DQ.currentQuestionNr + '-question').fadeOut(400, function() {

                    DQ.currentQuestionNr = 1;
                    $('#' + DQ.currentQuestionNr + '-question').fadeIn(500);

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
            DQ.submitButton.fadeIn(800, function () { $(this).removeClass('initialHide'); });
            DQ.retakeButton.fadeIn(800, function () { $(this).removeClass('initialHide'); });
        } else {
            DQ.dealWithDisabled($(this));
        }

        if (DQ.currentQuestionNr === 1) {
            DQ.prevButton.removeAttr('disabled');
        }

        $('#' + DQ.currentQuestionNr + '-question').fadeOut(400, function() {

            DQ.currentQuestionNr += 1;

            var qNrSpan = $('#' + DQ.currentQuestionNr + '-questionNr');

            if (!qNrSpan.text())
                qNrSpan.text(DQ.currentQuestionNr);

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

    ns.optionSelected = function(e) {
        e.preventDefault();

        var optionClickedId = e.currentTarget.id;
        var question = _.find(DQ.quiz.getQuestions(), function(question) {
            return question.getQuestionNumber() == optionClickedId.slice(0, 1);
        });

        var userAnswers = [];
        userAnswers.push($('#' + optionClickedId).val());

        question.setUserAnswers(userAnswers);

        console.info(question.getUserAnswers());

    };

    ns.submitClicked = function(e) {
        e.preventDefault();

        console.info(DQ.quiz);

        var self = this;

        var payload = { userQuizEditModel: DQ.quiz };
        var url = '/Quiz/SubmitQuiz';

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                $(self).append('<span id="submitQuizSpinner">&nbsp;<i class="icon-spinner icon-spin "></i></span>');
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {

                DQ.scores.append('<div>You scored<strong> {0} out of {1}</strong>.</div>'.format(data.Score, DQ.questionCount));
                DQ.scores.append('<div class="topBuffer10">The breakdown of your results is:</div>'.format(data.Score, DQ.questionCount));

                var rows = '<th>Question Number</th><th>Result</th>';

                _.each(data.QuestionsResult, function(value, idx, coll) {
                    rows += '<tr><td>{0}</td><td>{1}</td></tr>'.format(idx, value === true ? '<span class="icon-ok-circle" style="color:green">' : '<span class="icon-remove-circle" style="color:red">');
                    }
                );

                DQ.scores.append('<table class="table table-striped">{0}</table>'.format(rows));
                
                DQ.scores.fadeIn(400, function () { $(this).removeClass('initialHide'); });

                DQ.retakeButton.removeAttr('disabled');

            } else {

            }

            $('#submitQuizSpinner').remove();
            $(self).attr('disabled', 'disabled');
            DQ.prevButton.attr('disabled', 'disabled');
        });


    };

    DQ.retakeClicked = function(e) {

        e.preventDefault();

        DQ.setUiState('restart');

        DQ.questionsWrapper.find('input[type="radio"]').prop('checked', false);
    };

    DQ.homeClicked = function (e) {

        e.preventDefault();

        DQ.utilities.goToUrl(''); // go home
    };

})(DQ);