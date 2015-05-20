/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var EDITQUIZ = {}; // object to holds all references and methods.

var EQ = EDITQUIZ; // create shortcut alias

// document.ready function
$(function () {

    EQ.primeDomVariables();

    //EQ.startButton.on('click', EQ.startQuiz);
    //EQ.prevButton.on('click', EQ.prevClicked);
    //EQ.nextButton.on('click', EQ.nextClicked);
    //EQ.submitButton.on('click', EQ.submitClicked);
    //EQ.retakeButton.on('click', EQ.retakeClicked);
    //EQ.homeButton.on('click', EQ.homeClicked);

    EQ.utilities = new Common.Utilities();
});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionCount = 0;
    ns.currentQuestionNr = 1;

    ns.accordionTemplate = '<div class="accordion" id="accordion2">{0}</div>';
    ns.questionTemplate = '<div id="{0}-question" class="initialHide"><p><strong>Question Number <span id="{0}-questionNr"></span> of {3}</strong></p><p id="{0}-questionText">{1}</p><div id="{0}-options">{2}</div></div>';
    ns.optionsTemplate = '<label class="radio"><input id="{0}-option" name="{3}-option" type="radio" value="{1}" />{2}</label>';

    ns.primeDomVariables = function() {

    };

    ns.wireUpOptions = function(jQuerySet) {

        // this wires up the change handler for each radio button.
        _.each(jQuerySet, function(jQueryObj) {
            $(jQueryObj).on('change', EQ.optionSelected);
        });
    };

    ns.startQuiz = function(e) {
        e.preventDefault();

        EQ.setUiState('start');
        EQ.getQuestions();
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
                EQ.populateClientObjects(data.Quiz);

                var count = 1;

                _.each(EQ.quiz.getQuestions(), function(question, idx) {

                    var optionsHtml = '';

                    _.each(question.getOptions(), function(option, innerIdx) {
                        optionsHtml += EQ.optionsTemplate.format((idx + 1).toString() + (innerIdx + 1).toString(), option.getLetter(), option.getText(), count);
                    });

                    if (idx === 0) {
                        var questionHtml = EQ.questionTemplateFirst.format((idx + 1).toString(), question.getText(), optionsHtml, EQ.currentQuestionNr, EQ.questionCount);
                    } else {
                        var questionHtml = EQ.questionTemplate.format((idx + 1).toString(), question.getText(), optionsHtml, EQ.questionCount);
                    }

                    EQ.questionsWrapper.append(questionHtml);

                    count += 1;
                });

                EQ.wireUpOptions($('#questionWrapper').find('input[type="radio"]'));

            } else {

            }
        });


    };

    ns.populateClientObjects = function(data) {
        EQ.quiz = new QuizDomain.Quiz();
        EQ.quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        EQ.quiz.setOrderId(data.OrderId);
        EQ.quiz.setWebinarId(data.WebinarId);
        EQ.quiz.setEmail($('#emailInput').val());
        EQ.quiz.setQuizId($('#quizIdInput').val());
        
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

        EQ.quiz.setQuestions(questions);
        EQ.questionCount = EQ.quiz.getQuestions().length;
    };

    ns.setUiState = function(state) {

        switch(state) {
            case 'start':
                EQ.startPanel.fadeOut(400, function() {
                    EQ.questionPanel.fadeIn(400, function () {
                        $(this).removeClass('initialHide');
                    });
                });
                break;
            case 'restart':
                EQ.prevButton.attr('disabled', 'disabled');
                EQ.nextButton.removeAttr('disabled');
                EQ.submitButton.removeAttr('disabled').hide();
                EQ.retakeButton.hide();

                EQ.scores.empty();

                $('#' + EQ.currentQuestionNr + '-question').fadeOut(400, function() {

                    EQ.currentQuestionNr = 1;
                    $('#' + EQ.currentQuestionNr + '-question').fadeIn(500);

                });
                break;
            default:
                break;
        }


    };

    ns.validateOptions = function(jQuerySet) {

        var valid = _.some(jQuerySet, function (item, idx) {
            if ($(item).prop('checked') && $(item).prop('checked', true)) {
                return true;
            }
        });

        return valid;
    };

    ns.nextClicked = function(e) {
        e.preventDefault();

        $('#valFailMsg').remove();

        if (!(EQ.validateOptions($('#' + EQ.currentQuestionNr + '-options').find('input')))) {
            $(this).after('<div id="valFailMsg" style="display:inline-block;margin-left:5px;">&nbsp;<span class="label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;You have to select an option</span></div>');
            return false;
        }

        if (EQ.currentQuestionNr + 1 === EQ.questionCount) {
            $(this).attr('disabled', 'disabled');
            EQ.submitButton.fadeIn(800, function () { $(this).removeClass('initialHide'); });
            EQ.retakeButton.fadeIn(800, function () { $(this).removeClass('initialHide'); });
        } else {
            EQ.dealWithDisabled($(this));
        }

        if (EQ.currentQuestionNr === 1) {
            EQ.prevButton.removeAttr('disabled');
        }

        $('#' + EQ.currentQuestionNr + '-question').fadeOut(400, function() {

            EQ.currentQuestionNr += 1;

            var qNrSpan = $('#' + EQ.currentQuestionNr + '-questionNr');

            if (!qNrSpan.text())
                qNrSpan.text(EQ.currentQuestionNr);

            $('#' + EQ.currentQuestionNr + '-question').fadeIn(400, function () {
                if($(this).hasClass('initialHide'))
                    $(this).removeClass('initialHide');
            });

        });

    };

    ns.prevClicked = function (e) {
        e.preventDefault();

        if (EQ.currentQuestionNr - 1 === 1) {
            $(this).attr('disabled', 'disabled');
        } else {
            EQ.dealWithDisabled($(this));
        }

        if (EQ.currentQuestionNr === EQ.questionCount) {
            EQ.nextButton.removeAttr('disabled');
        }

        $('#' + EQ.currentQuestionNr + '-question').fadeOut(400, function () {

            EQ.currentQuestionNr -= 1;

            $('#' + EQ.currentQuestionNr + '-question').fadeIn(400);

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
        var question = _.find(EQ.quiz.getQuestions(), function(question) {
            return question.getQuestionNumber() == optionClickedId.slice(0, 1);
        });

        var userAnswers = [];
        userAnswers.push($('#' + optionClickedId).val());

        question.setUserAnswers(userAnswers);

        console.info(question.getUserAnswers());

    };

    ns.submitClicked = function(e) {
        e.preventDefault();

        $('#valFailMsg').remove();

        if (!(EQ.validateOptions($('#' + EQ.currentQuestionNr + '-options').find('input')))) {
            $(this).after('<div id="valFailMsg" style="display:inline-block;margin-left:5px;">&nbsp;<span class="label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;You have to select an option</span></div>');
            return false;
        }

        console.info(EQ.quiz);

        var self = this;

        var payload = { userQuizEditModel: EQ.quiz };
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

                EQ.scores.append('<div>You scored<strong> {0} out of {1}</strong>.</div>'.format(data.Score, EQ.questionCount));
                EQ.scores.append('<div class="topBuffer10">The breakdown of your results is:</div>'.format(data.Score, EQ.questionCount));

                var rows = '<th>Question Number</th><th>Result</th>';

                _.each(data.QuestionsResult, function(value, idx, coll) {
                    rows += '<tr><td>{0}</td><td>{1}</td></tr>'.format(idx, value === true ? '<span class="icon-ok-circle" style="color:green">' : '<span class="icon-remove-circle" style="color:red">');
                    }
                );

                EQ.scores.append('<table class="table table-striped">{0}</table>'.format(rows));
                
                EQ.scores.fadeIn(400, function () { $(this).removeClass('initialHide'); });

                EQ.retakeButton.removeAttr('disabled');

            } else {

            }

            $('#submitQuizSpinner').remove();
            $(self).attr('disabled', 'disabled');
            EQ.prevButton.attr('disabled', 'disabled');
        });


    };

    EQ.retakeClicked = function(e) {

        e.preventDefault();

        EQ.setUiState('restart');

        EQ.questionsWrapper.find('input[type="radio"]').prop('checked', false);
    };

    EQ.homeClicked = function (e) {

        e.preventDefault();

        EQ.utilities.goToUrl(''); // go home
    };

})(EQ);