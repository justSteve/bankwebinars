/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />

var EDITQUIZ = {}; // object to holds all references and methods.

var EQ = EDITQUIZ; // create shortcut alias

// document.ready function
$(function () {

    EQ.primeDomVariables();

    EQ.editCloneSubmitButton.on('click', EQ.submitEditedQuiz);
    //EQ.prevButton.on('click', EQ.prevClicked);
    //EQ.nextButton.on('click', EQ.nextClicked);
    //EQ.submitButton.on('click', EQ.submitClicked);
    //EQ.retakeButton.on('click', EQ.retakeClicked);
    //EQ.homeButton.on('click', EQ.homeClicked);

    EQ.utilities = new Common.Utilities();

    EQ.getQuestions();
});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionsList = [];
    ns.questionCount = 0;
    ns.currentQuestionNr = 1;

    ns.deletedQuestions = [];
    ns.editedQuestions = [];
    ns.newQuestions = [];

    ns.accordionTemplate = '<div class="accordion" id="questions">{0}</div>';
    ns.questionTemplate = '<div class="accordion-group" id="{0}-question" data-id={2}><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#questions" href="#collapse{0}">Question #{0}</a></div><div id="collapse{0}" class="accordion-body collapse in"><div id="{0}-options" class="accordion-inner">{1}</div><button class="btn btn-default btn-mini">&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}-delete"></i>&nbsp;Delete Question</button><button class="btn btn-default btn-mini">Add Option</button></div></div>';
    ns.optionsTemplate = '<div id="{0}{1}-option" data-id="{5}" class="input-prepend input-append"><span class="add-on">{2}</span><input id="{0}{1}-optionInput" name="Options[{0}{1}].Text" class="input input-xxlarge" value="{3}" placeholder="Enter option text and tick box at end if correct answer"  type="text" />&nbsp;<span class="add-on"><input id="{0}{1}-answer" type="checkbox" title="Is this the answer?" {4} />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}{1}-delete"></i></span></div>';

    ns.primeDomVariables = function() {
        EQ.mainForm = $('#mainForm');
        EQ.form = $('#EditCloneQuizForm');
        EQ.webinarId = $('#WebinarId');
        EQ.quizId = $('#QuizId');
        EQ.editCloneSubmitButton = $('#EditCloneSubmitButton');
    };

    ns.wireUpOptions = function(jQuerySet) {

        // this wires up the change handler for each radio button.
        //_.each(jQuerySet, function(jQueryObj) {
        //    $(jQueryObj).on('change', EQ.optionSelected);
        //});
    };

    

    ns.getQuestions = function () {

        var payload = {
            webinarId: $('#WebinarId').val(),
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Quiz/GetQuestionsByWebinarId',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {

            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                EQ.populateClientObjects(data.Quiz);

                var questions = '';
                var count = 1;

                _.each(EQ.quiz.getQuestions(), function(question, idx) {

                    var optionsHtml = '';

                    _.each(question.getOptions(), function(option, innerIdx) {
                        optionsHtml += EQ.optionsTemplate.format((idx + 1).toString(),(innerIdx + 1).toString(), option.getLetter(), option.getText(), option.getCorrectAnswer() ? 'checked' : '', option.getOptionId());
                    });
                    
                    questions += EQ.questionTemplate.format(count, optionsHtml, question.getQuestionId());

                    count += 1;
                });

                EQ.accordionTemplate.format(questions);
                EQ.mainForm.append(questions);

                var optionTrashCans = $('div[id$="-option"]').find('i.icon-trash');

                _.each(optionTrashCans, function (trash, idx) {
                    $(trash).on('click', EQ.deleteOption);
                });

                var questionTrashCans = $('div[id^="collapse"] button').find('i.icon-trash').parent();

                _.each(questionTrashCans, function (trash, idx) {
                    $(trash).on('click', EQ.deleteQuestion);
                });


                $(".collapse").not('#collapse1').collapse('hide');

            } else {

            }
        });
    };

    ns.deleteOption = function(e) {
        e.preventDefault();

        console.info('delete');
    };


    ns.deleteQuestion = function (e) {
        e.preventDefault();

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        // record the id of the question which we are deleting from the quiz.
        EQ.deletedQuestions.push(questionId);

        // shuffle the remaining questions up and re-number them accordingly.
        EQ.reArrangeRemainingQuestions(accordionGroup, questionNumber);

        // delete this question from the dom.
        accordionGroup.fadeOut(200, function() {
            $(this).remove();
        });

    };


    ns.reArrangeRemainingQuestions = function (accordionGroup, questionId) {

        var followingQuestions = accordionGroup.nextAll();
        var newId = questionId;

        _.each(followingQuestions, function (accordionGroup, idx) {
            var accordionGroup$ = $(accordionGroup);
            accordionGroup$.attr('id', newId.toString() + '-question');

            var anchorTitle = accordionGroup$.find('div > a');
            anchorTitle.attr('href', '#collapse' + newId.toString());
            anchorTitle.text('Question #' + newId.toString());

            var collapseDiv = accordionGroup$.find('div[id^="collapse"]');
            collapseDiv.attr('id', 'collapse' + newId.toString());
            collapseDiv.find('div[id$="-options"]').attr('id', newId.toString() + '-options');

            _.each(collapseDiv.find('div > div[id$="-option"]'), function(option, idx) {
                var optionDiv = $(option);
                var newIdentifier = newId.toString() + (idx + 1).toString();
                optionDiv.attr('id', newIdentifier + '-option');
                optionDiv.find('input[type="text"]').attr('id', newIdentifier + '-optionInput').attr('name', 'Options[' + newIdentifier + '].Text');
                optionDiv.find('input[type="checkbox"]').attr('id', newIdentifier + '-answer');
                optionDiv.find('i').attr('id', newIdentifier + '-delete');
            });

            accordionGroup$.find('button i').attr('id', newId.toString() + '-delete');

            newId += 1;
        });


    };


    ns.populateClientObjects = function(data) {
        EQ.quiz = new QuizDomain.EditableQuiz();
        EQ.quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        EQ.quiz.setWebinarId(data.WebinarId);
        EQ.quiz.setQuizId(EQ.quizId.val());
        
        var questions = [];

        _.each(data.QuizQuestions, function(item, index) {

            var question = new QuizDomain.EditableQuestion();
            question.setText(item.QuestionText);
            question.setQuestionNumber(item.QuestionNumber);
            question.setQuestionId(item.QuestionId);

            var options = [];

            _.each(item.Options, function(option, index) {
                var optionForQu = new QuizDomain.EditableOption();
                optionForQu.setText(option.Text);
                optionForQu.setLetter(option.Letter);
                optionForQu.setOptionId(option.QuestionOptionId);
                
                if (option.Letter == item.Solutions[0]) {
                    optionForQu.setCorrectAnswer(true);
                } else {
                    optionForQu.setCorrectAnswer(false);
                }

                options.push(optionForQu);
            });

            question.setOptions(options);
            questions.push(question);
        });

        EQ.quiz.setQuestions(questions);
        EQ.questionCount = EQ.quiz.getQuestions().length;
        EQ.questionsList = questions;
    };

    ns.getQuestionsList = function () {

        var options;
        var questions = [];

        _.each(EQ.questionsList, function (question) {

            options = null;
            options = [];
            quizWithQuestions = [];

            _.each(question.getOptions(), function (option) {

                var opt = { Text: option.getText() };
                var questionWithOption = {
                    Letter: option.getLetter(),
                    CorrectAnswer: option.getCorrectAnswer(),
                    Option: opt
                };

                options.push(questionWithOption);
            });

            quizWithQuestions.push({
                QuestionNumber: question.getQuestionNumber(),
                idQuiz: EQ.quizId.val(),
                idQuestion: question.getQuestionId()
            });


            questions.push({
                Text: question.getText(),
                QuestionWithOptions: options,
                QuizWithQuestions: quizWithQuestions
            });
        });

        return questions;
    };

    ns.submitEditedQuiz = function (e) {

        e.preventDefault();

        var url = EQ.form.attr('action');

        var webinarId = EQ.webinarId.val();
        var quizId = EQ.quizId.val();
        var questions = EQ.getQuestionsList();
        

        var payload = {
            SelectedWebinar: webinarId,
            Questions: questions,
            QuizId: quizId,
            DeletedQuestions: EQ.deletedQuestions
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                EQ.editCloneSubmitButton.append('&nbsp;<i id="editQuizSpinner" class="icon-spinner icon-spin"></i>');
                $('#editQuizResult').remove();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                EQ.editCloneSubmitButton.after('<span id="editQuizResult">&nbsp;<span class="label label-success"><span> Quiz has been added! </span></span></span>').fadeIn(200).before().hide();

            } else if (!data.isSuccessful) {

            } else {

            }
            $('#editQuizSpinner').remove();
        });
    };

})(EQ);