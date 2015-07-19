/// <reference path="../Utilities/formProcessor.js" />
/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />
/// <reference path="Quiz.js" />

if (EDITQUIZ === null || typeof EDITQUIZ === 'undefined')
    var EDITQUIZ = {}; // object to holds all references and methods.

var EQ = EDITQUIZ; // create shortcut alias

// document.ready function
$(function () {

    EQ.primeDomVariables();
    EQ.wireUpHandlers();
    
    EQ.utilities = new Common.Utilities();
    EQ.toastLogger = new Common.Logger(); // for toast notifications

    EQ.getQuestions();

});

// self-invoking function for creating methods using Module pattern.
(function(ns) {

    ns.questionsList = [];
    ns.deletedQuestions = [];
    ns.editedQuestions = [];
    ns.newQuestions = [];

    ns.newQuestionLastId = 0;

    ns.accordionTemplate = '<div class="accordion" id="questions">{0}</div>';
    ns.questionTemplate = '<div class="accordion-group" id="{0}-question" data-id={2}><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#questions" href="#collapse{0}">Question #{0}</a></div><div id="collapse{0}" class="accordion-body collapse in"><div id="{0}-options" class="accordion-inner">{1}</div><div class="btn-toolbar"><button class="btn btn-default btn-mini">&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}-delete"></i>&nbsp;Delete Question</button><button class="btn btn-default btn-mini">&nbsp;<i class="icon-plus-sign icon-white" style="cursor: pointer" id="{0}-addOption"></i>&nbsp;Add Option</button><span id="{0}-oCnt">{3}</span></div></div></div>';
    ns.questionTextTemplate = '<div><input id="{0}-questionText" type="text" value="{1}" class="input input-xxlarge" placeholder="Enter question text here" /></div>';
    ns.optionsTemplate = '<div id="{0}{1}-option" data-id="{5}" class="input-prepend input-append"><span class="add-on">{2}</span><input id="{0}{1}-optionInput" name="Options[{0}{1}].Text" class="input input-xxlarge" value="{3}" placeholder="Enter option text and tick box at end if correct answer"  type="text" />&nbsp;<span class="add-on"><input id="{0}{1}-answer" type="checkbox" title="Is this the answer?" {4} />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}{1}-delete"></i></span></div>';
    

    ns.primeDomVariables = function() {
        EQ.mainForm = $('#mainForm');
        EQ.form = $('#EditCloneQuizForm');
        EQ.webinarId = $('#WebinarId');
        EQ.quizId = $('#QuizId');
        EQ.editCloneSubmitButton = $('#EditCloneSubmitButton');
        EQ.editCloneSubmitButtonTop = $('#EditCloneSubmitButtonTop');
        EQ.addQuestionButton = $('#AddQuestionButton');
        EQ.CloneQuizButton = $('#CloneWebinar');
        EQ.cloneInput = $('#CloneInput');
        EQ.editWebinarButton = $('#EditWebinar');
        EQ.editInput = $('#EditInput');
    };

    ns.wireUpHandlers = function() {
        EQ.editCloneSubmitButton.on('click', EQ.submitEditedQuiz);
        EQ.editCloneSubmitButtonTop.on('click', EQ.submitEditedQuiz);
        EQ.addQuestionButton.on('click', EQ.addQuestionClicked);
        EQ.CloneQuizButton.on('click', EQ.CloneQuiz);
        EQ.editWebinarButton.on('click', EQ.editWebinar);
    };

    // this method re-populates inputs following a post to the server.
    ns.rePopulateInputs = function() {

        EQ.mainForm.fadeOut(300, function() {
            EQ.mainForm.empty();

            EQ.addQuestionButton = $('<button id="AddQuestionButton" class="btn btn-default">&nbsp;<i class="icon-plus-sign icon-white"></i>&nbsp;Add Question</button>');
            EQ.editCloneSubmitButton = $('<button id="EditCloneSubmitButton" class="btn btn-primary">Submit Edits</button>');
            EQ.editCloneSubmitButton.on('click', EQ.submitEditedQuiz);
            EQ.addQuestionButton.on('click', EQ.addQuestionClicked);

            EQ.questionsList.length = 0;
            EQ.deletedQuestions.length = 0;
            EQ.editedQuestions.length = 0;
            EQ.newQuestions.length = 0;
            ns.newQuestionLastId = 0;

            EQ.getQuestions();

            EQ.mainForm.fadeIn(200);
        });

    };

    ns.populateClientObjectsEmptyList = function () {

        EQ.quiz = new QuizDomain.Quiz();
        EQ.quiz.setCompleted(QuizDomain.CompletionStatus.NotStarted);
        EQ.quiz.setWebinarId(parseInt(EQ.webinarId.val()));
        EQ.quiz.setQuizId(-1);

        var questions = [];
        
        var question = new QuizDomain.EditableQuestion();
        question.setText('');
        question.setQuestionNumber(1);
        question.setQuestionId(-1);

        var options = [];

        question.setOptions(options);
        questions.push(question);

        EQ.quiz.setQuestions(questions);
        EQ.questionsList = questions;

        return questions;
    };

    ns.setStateForAddQuizStory = function () {

        EQ.populateClientObjectsEmptyList();

        var optionsHtml = EQ.questionTextTemplate.format('1', '');

        optionsHtml += EQ.optionsTemplate.format('1', '1', 'a', '', '', '-1');

        EQ.newQuestionLastId -= 1;
        var newQuestionHtml = EQ.questionTemplate.format(1, optionsHtml, EQ.newQuestionLastId, '1');

        EQ.mainForm.append(newQuestionHtml);

        var newQuestionPanel = $('#collapse1');

        var newQid = '11';

        var trashOption = newQuestionPanel.find('i#' + newQid + '-delete');
        trashOption.on('click', EQ.newQuDeleteOption);

        // hook up event handlers
        newQuestionPanel.find('button').first().on('click', EQ.newQuDeleteQuestion);
        newQuestionPanel.find('button').last().on('click', EQ.newQuAddOption);

        EQ.newQuestions.push(EQ.newQuestionLastId);

        EQ.mainForm.append('<div id="SubmitButtons" class="btn-toolbar"></div>');
        EQ.submitButtons = $('#SubmitButtons');
        EQ.addQuestionButton.appendTo(EQ.submitButtons);
        EQ.editCloneSubmitButton.appendTo(EQ.submitButtons);
    };

    // This method is the one which retrieves the questions for the quiz from the server.
    ns.getQuestions = function () {

        var payload = {
            webinarId: EQ.webinarId.val(),
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

                if (!data.Quiz) {
                    EQ.setStateForAddQuizStory();
                    return;
                }

                EQ.populateClientObjects(data.Quiz);

                var questions = '';

                _.each(EQ.quiz.getQuestions(), function(question, idx) {

                    var optionsHtml = EQ.questionTextTemplate.format(question.getQuestionNumber(), question.getText());

                    var optionsCount = 0;
                    _.each(question.getOptions(), function (option, innerIdx) {
                        optionsCount += 1;
                        optionsHtml += EQ.optionsTemplate.format((idx + 1).toString(), (innerIdx + 1).toString(), option.getLetter(), option.getText(), option.getCorrectAnswer() ? 'checked' : '', option.getOptionId());
                    });
                    
                    questions += EQ.questionTemplate.format(question.getQuestionNumber(), optionsHtml, question.getQuestionId(), optionsCount);
                });

                EQ.accordionTemplate.format(questions);
                EQ.mainForm.append(questions); // accordion set as child of mainForm div in dom
                EQ.mainForm.append('<div id="SubmitButtons" class="btn-toolbar"></div>');
                EQ.submitButtons = $('#SubmitButtons');
                EQ.addQuestionButton.appendTo(EQ.submitButtons);
                EQ.editCloneSubmitButton.appendTo(EQ.submitButtons);

                var optionTrashCans = EQ.mainForm.find('div[id$="-option"]').find('i.icon-trash');

                _.each(optionTrashCans, function (trash, idx) {
                    $(trash).on('click', EQ.deleteOption);
                });

                var deleteQuestionButtons = EQ.mainForm.find('div[id^="collapse"] button').find('i.icon-trash').parent();

                _.each(deleteQuestionButtons, function (trash, idx) {
                    $(trash).on('click', EQ.deleteQuestion);
                });

                var addOptionButtons = EQ.mainForm.find('div[id^="collapse"] button').find('i.icon-plus-sign').parent();

                _.each(addOptionButtons, function (btn, idx) {
                    $(btn).on('click', EQ.addOption);
                });

                var questionTexts = EQ.mainForm.find('input[id$="-questionText"]');

                _.each(questionTexts, function(questionTextInput, idx) {
                    var input$ = $(questionTextInput);
                    input$.on('blur', EQ.qTextEdited);
                });

                var optionTexts = EQ.mainForm.find('input[id$="-optionInput"]');

                _.each(optionTexts, function (optionTextInput, idx) {
                    var input$ = $(optionTextInput);
                    input$.on('blur', EQ.optionEdited);
                });

                var optionChkBoxes = EQ.mainForm.find('input[id$="-answer"]');

                _.each(optionChkBoxes, function (optionChkBox, idx) {
                    var input$ = $(optionChkBox);
                    input$.on('change', EQ.optionEdited);
                });

                $(".collapse").not('#collapse1').collapse('hide');

            } else {
                // todo: unhappy path
            }
        });
    };

    ns.addOption = function (e) {

        e.preventDefault();

        var btnClicked$ = $(e.currentTarget);

        var lastOption = btnClicked$.parent().parent().find('div[id$="-option"]').last();

        // make sure that the most recent option added has text
        if (lastOption.length !== 0 && $.trim(lastOption.find('input[type="text"]').val()) === '') {
            console.info('toast this');
            return;
        }

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        var optionCntSpan = $('#' + questionNumber + '-oCnt');
        var currentOptionCount = parseInt(optionCntSpan.text());
        optionCntSpan.text((currentOptionCount + 1).toString());


        var letterOfLastOption,
            nextletter,
            idOfLastOption;

        if (lastOption.length === 0) {
            nextletter = 'a';
            idOfLastOption = 0;
        } else {
            letterOfLastOption = lastOption.find('span:first-child').text();
            nextletter = String.fromCharCode((letterOfLastOption.charCodeAt(0) + 1));
            idOfLastOption = parseInt(lastOption.data('id'));
        }

        var newOptionId = 0;
        // If last option was already existing, make the new one -1. Else, decrement it by 1.
        // The idea being each new option has a lower negative number.
        if (idOfLastOption > 0) {
            newOptionId = -1;
        } else {
            newOptionId = idOfLastOption - 1;
        }

        var newOptionDiv = EQ.optionsTemplate.format(questionNumber, currentOptionCount + 1, nextletter, '', '', newOptionId);

        btnClicked$.parent().prev().append(newOptionDiv);

        // hook up events for new option
        var newOptionDiv$ = accordionGroup.find('#' + questionNumber.toString() + (currentOptionCount + 1).toString() + '-option');
        newOptionDiv$.find('i.icon-trash').on('click', EQ.deleteOption);
        newOptionDiv$.find('input[id$="-optionInput"]').on('blur', EQ.optionEdited);
        newOptionDiv$.find('input[id$="-answer"]').on('change', EQ.optionEdited);

        var editedOption = new QuizDomain.EditableOption();
        editedOption.EditType = QuizDomain.EditType.Added;
        editedOption.NewOptionLetter = nextletter;
        editedOption.NewCorrectStatus = false;
        editedOption.OptionId = newOptionId;


        // if we are adding an option, the question will now have a status of edited. 
        var editedQuestion = EQ.addOrUpdateQuestion(questionId);

        editedQuestion.EditedOptions.push(editedOption);

        EQ.addToEditedOptionsList($(newOptionDiv), editedQuestion, newOptionId);
    };

    // this handler gets invoked on the blur event of an input. It checks to see whether the question text has changed.
    ns.qTextEdited = function (e) {

        e.preventDefault();

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));

        EQ.addToEditedQuestionsList(accordionGroup);

    };

    // this handler gets invoked on the blur or change event of an Option input. It checks to see whether the Option text has changed.
    ns.optionEdited = function (e) {

        e.preventDefault();

        var currentTarget = e.currentTarget;

        var optionDiv = null;
        if (currentTarget.id.includes('-answer')) {
            optionDiv = $(currentTarget).parent().parent();
        } else {
            optionDiv = $(currentTarget).parent();
        }
        
        var optionId = optionDiv.data('id');
        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');

        var editedQuestion = EQ.addOrUpdateQuestion(questionId);
        var editedOption = EQ.addOrUpdateOption(editedQuestion, optionId);
        
        editedOption.EditType = EQ.assignEditType(editedOption.EditType);

        EQ.addToEditedOptionsList($(optionDiv), editedQuestion, optionId);
    };

    ns.assignEditType = function(editType) {

        // if we are editing a newly added option, leave its EdiType as 'Added'
        if (editType === null || typeof editType === 'undefined' || editType != QuizDomain.EditType.Added)
            editType = QuizDomain.EditType.Edited;

        return editType;
    };

    ns.addToEditedOptionsList = function (optionDivs, editedQuestion) {

        _.each(optionDivs, function(optionDiv, idx) {

            var optionDiv$ = $(optionDiv);

            var optionId = optionDiv$.data('id');

            var origQuestion = _.find(EQ.questionsList, function (question) {
                return question.getQuestionId() === editedQuestion.QuestionId;
            });

            var origOption = _.find(origQuestion.getOptions(), function(option, idx) {
                return option.getOptionId() === optionId;
            });

            var editedOption = EQ.addOrUpdateOption(editedQuestion, optionId);

            editedOption.EditType = EQ.assignEditType(editedOption.EditType);

            if (!origOption) {
                editedOption.NewOptionLetter = optionDiv$.find('span:first-child').text();
                editedOption.NewOptionText = optionDiv$.find('input[type="text"]').val();
                editedOption.NewCorrectStatus = optionDiv$.find('input[type="checkbox"]').prop('checked');
            } else {
                editedOption.OriginalOptionLetter = origOption.getLetter();
                editedOption.NewOptionLetter = optionDiv$.find('span:first-child').text();
                editedOption.OriginalOptionText = origOption.getText();
                editedOption.NewOptionText = optionDiv$.find('input[type="text"]').val();
                editedOption.OriginalCorrectStatus = origOption.getCorrectAnswer();
                editedOption.NewCorrectStatus = optionDiv$.find('input[type="checkbox"]').prop('checked');
            }
        });
    };

    
    ns.deleteOption = function(e) {
        e.preventDefault();

        var currentTarget = e.currentTarget;

        // get info about the question
        var accordionGroup = $(currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        // get info about the option
        var optionDiv = $(currentTarget).parent().parent();
        var optionId = optionDiv.data('id');
        var optionDivId = optionDiv.attr('id').slice(1,2);

        // If in the list of edited options, set its status to 'deleted'
        // 1st, check if the question is already in the edited questions list.
        var editedQuestion = EQ.addOrUpdateQuestion(questionId);

        var editedOption = EQ.addOrUpdateOption(editedQuestion, optionId);

        // If option is one that was Added in this editing session, just remove it from the 
        // question's EditedOption's list.
        if (optionId < 0) {
            var qInList = _.find(editedQuestion.EditedOptions, function (editedOption, idx) {
                return editedOption.OptionId === optionId;
            });
            editedQuestion.EditedOptions = _.without(editedQuestion.EditedOptions, qInList);
        }

        editedOption.EditType = QuizDomain.EditType.Deleted;

        EQ.reArrangeRemainingOptions(optionDiv.nextAll(), optionDivId, questionNumber);

        EQ.addToEditedOptionsList(optionDiv.next(), editedQuestion, optionId);

        var optionCntSpan = $('#' + questionNumber + '-oCnt');
        var optionCount = parseInt(optionCntSpan.text());
        optionCntSpan.text((optionCount - 1).toString());

        // delete this question from the dom.
        optionDiv.fadeOut(200, function () {
            $(this).remove();
        });
    };


    ns.deleteQuestion = function (e) {
        e.preventDefault();

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        // if in the list of edited questions, remove it, because we are deleting it
        var qInList = _.find(EQ.editedQuestions, function(editedQuestion) { return editedQuestion.QuestionId === questionId; });

        if (qInList) {
            EQ.editedQuestions = _.without(EQ.editedQuestions, qInList);
        }

        // record the id of the question which we are deleting from the quiz.
        EQ.deletedQuestions.push(questionId);

        // shuffle the remaining questions up and re-number them accordingly.
        var followingQuestions = EQ.reArrangeRemainingQuestions(accordionGroup, questionNumber);

        EQ.addToEditedQuestionsList(followingQuestions);

        // delete this question from the dom.
        accordionGroup.fadeOut(200, function() {
            $(this).remove();
            $('#collapse1').collapse('show');
        });
    };

    // this method is used when an option is deleted. It shuffles up all the following options. E.g. if (a)
    // is deleted, (b) becomes (a), (c) becomes (b) etc.
    ns.reArrangeRemainingOptions = function (remainingOptionDivs, deletedOptionId, questionNumber) {

        var newId = deletedOptionId;

        _.each(remainingOptionDivs, function (remainingOptionDiv, idx) {
            var remainingOptionDiv$ = $(remainingOptionDiv);
            var newIdentifier = questionNumber.toString() + newId.toString();

            var optionLetterSpan = remainingOptionDiv$.find('span:first-child');

            var currentLetter = optionLetterSpan.text();

            var newletter = String.fromCharCode((currentLetter.charCodeAt(0) - 1));

            optionLetterSpan.text(newletter);

            remainingOptionDiv$.attr('id', newIdentifier + '-option');

            remainingOptionDiv$.find('input[type="text"]').attr('id', newIdentifier + '-optionInput').attr('name', 'Options[' + newIdentifier + '].Text');
            remainingOptionDiv$.find('input[type="checkbox"]').attr('id', newIdentifier + '-answer');
            remainingOptionDiv$.find('i').attr('id', newIdentifier + '-delete');

            newId += 1;
        });
    };

    ns.addOrUpdateQuestion = function (questionId) {

        // check if the question is already in the edited questions list.
        var editedQuestion = _.find(EQ.editedQuestions, function (question) {
            return question.QuestionId === questionId;
        });

        // if not, create it.
        if (!editedQuestion) {
            editedQuestion = new QuizDomain.EditedQuestion(questionId);
            editedQuestion.EditedOptions = [];
            EQ.editedQuestions.push(editedQuestion);
        }

        return editedQuestion;
    };

    ns.addOrUpdateOption = function (editedQuestion, optionId) {

        // check if the question is already in the edited questions list.
        var editedOption = _.find(editedQuestion.EditedOptions, function (option) {
            return option.OptionId === optionId;
        });

        // if not, create it.
        if (!editedOption) {
            editedOption = new QuizDomain.EditedOption();
            editedOption.OptionId = optionId;
            editedQuestion.EditedOptions.push(editedOption);
        }

        return editedOption;
    };

    ns.addToEditedQuestionsList = function (questionsToAdd) {

        _.each(questionsToAdd, function(questionPanel, idx) {

            var questionPanel$ = $(questionPanel);
            var questionId = questionPanel$.data('id');

            // if questionId = 0, we know we have a new question and need not bother with adding it to the edited question array.
            if (questionId > 0) {
                var editedQuestion = EQ.addOrUpdateQuestion(questionId);

                var origQuestion = _.find(EQ.questionsList, function(question) {
                    return question.getQuestionId() === questionId;
                });

                if (origQuestion) {
                    editedQuestion.OriginalQuestionNumber = origQuestion.getQuestionNumber();
                    editedQuestion.NewQuestionNumber = parseInt(questionPanel$.attr('id').slice(0, 1));
                    editedQuestion.OriginalQuestionText = origQuestion.getText();
                    editedQuestion.NewQuestionText = questionPanel$.find('input[id$="-questionText"]').val();
                }
            }
        });

    };

    // this method is used when a question is deleted. It shuffles up all the following questions. E.g. if (1)
    // is deleted, (2) becomes (1), (3) becomes (2) etc.
    ns.reArrangeRemainingQuestions = function (accordionGroup, questionNumber) {

        var followingQuestions = accordionGroup.nextAll();
        var newId = questionNumber;

        _.each(followingQuestions, function (accordionGroup, idx) {
            var accordionGroup$ = $(accordionGroup);
            accordionGroup$.attr('id', newId.toString() + '-question');

            var anchorTitle = accordionGroup$.find('div > a');
            anchorTitle.attr('href', '#collapse' + newId.toString());
            anchorTitle.text('Question #' + newId.toString());

            var collapseDiv = accordionGroup$.find('div[id^="collapse"]');
            collapseDiv.attr('id', 'collapse' + newId.toString());
            collapseDiv.find('div[id$="-options"]').attr('id', newId.toString() + '-options');

            collapseDiv.find('div > div > input[id$="-questionText"]').attr('id', newId.toString() + '-questionText');

            _.each(collapseDiv.find('div > div[id$="-option"]'), function(option, idx) {
                var optionDiv = $(option);
                var newIdentifier = newId.toString() + (idx + 1).toString();
                optionDiv.attr('id', newIdentifier + '-option');
                optionDiv.find('input[type="text"]').attr('id', newIdentifier + '-optionInput').attr('name', 'Options[' + newIdentifier + '].Text');
                optionDiv.find('input[type="checkbox"]').attr('id', newIdentifier + '-answer');
                optionDiv.find('i').attr('id', newIdentifier + '-delete');
            });

            accordionGroup$.find('button i').attr('id', newId.toString() + '-delete');

            accordionGroup$.find('span[id$="-oCnt"]').attr('id', newId.toString() + '-oCnt');

            newId += 1;
        });

        return followingQuestions;
    };

    // this method populates some strongly-types Typescript objects to recreate our domain objects at the client.
    ns.populateClientObjects = function(data) {
        EQ.quiz = new QuizDomain.Quiz();
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

    ns.postAddQuiz = function (btnClickedId, webinarId, questions) {
                alert("hit");
        var payload = {
            Questions: questions,
            SelectedWebinar: webinarId
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Quiz/AddQuiz',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {

                if (btnClickedId === 'EditCloneSubmitButtonTop') {
                    EQ.editCloneSubmitButtonTop.append('&nbsp;<i id="editQuizSpinner" class="icon-spinner icon-spin"></i>');
                } else {
                    EQ.editCloneSubmitButton.append('&nbsp;<i id="editQuizSpinner" class="icon-spinner icon-spin"></i>');
                }
                $('#editQuizResult').remove();

            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {

                EQ.webinarId.val(data.WebinarId);
                EQ.rePopulateInputs();

            } else {

            }

            $('#editQuizSpinner').remove();
        });


    
    };

    ns.submitEditedQuiz = function (e) {

        e.preventDefault();

        var btnClickedId = $(e.currentTarget).attr('id');

        var url = EQ.form.attr('action');

        var webinarId = EQ.webinarId.val();
        var quizId = EQ.quizId.val();
        
        if (!quizId) {
            var questions = EQ.getNewQuestionsFromDom();

            if (questions['valid']) {
                EQ.postAddQuiz(btnClickedId, webinarId, questions['data']);
            } else {
                EQ.toastErrorMessage(questions['data']);
            }
            return;
        }
        
        questions = EQ.getNewQuestionsFromDom();

        if (!questions['valid']) {
            EQ.toastErrorMessage(questions['data']);
            return;
        }

        var payload = {
            SelectedWebinar: webinarId,
            QuizId: quizId,
            DeletedQuestions: EQ.deletedQuestions,
            EditedQuestions: EQ.editedQuestions,
            NewQuestions: questions['data']
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                if (btnClickedId === 'EditCloneSubmitButtonTop') {
                    EQ.editCloneSubmitButtonTop.append('&nbsp;<i id="editQuizSpinner" class="icon-spinner icon-spin"></i>');
                } else {
                    EQ.editCloneSubmitButton.append('&nbsp;<i id="editQuizSpinner" class="icon-spinner icon-spin"></i>');
                }
                $('#editQuizResult').remove();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                if (btnClickedId === 'EditCloneSubmitButtonTop') {
                    EQ.editCloneSubmitButtonTop.after('<span id="editQuizResult">&nbsp;<span class="label label-success"><span> Quiz has been edited! </span></span></span>').hide().fadeIn(200);
                } else {
                    EQ.editCloneSubmitButton.parent().after('<span id="editQuizResult">&nbsp;<span class="label label-success"><span> Quiz has been edited! </span></span></span>').hide().fadeIn(200);
                }

                EQ.rePopulateInputs();

            } else if (!data.isSuccessful) {

            } else {

            }
            $('#editQuizSpinner').remove();
        });
    };

    ns.toastErrorMessage = function (invalidArray) {
        
        var logMsg = '';

        for (var index in invalidArray) {
            logMsg += "Question Nr " + index + " has " + invalidArray[index] + " answers<br />";
        }

        var logValidationFail = EQ.toastLogger.getLogFn('', 'logError');
        logValidationFail(logMsg, null, true);
    };

    ns.addQuestionClicked = function (e) {
        e.preventDefault();

        var bottomAccordionBar = EQ.mainForm.find('div.accordion-group').last();
        var questionNumber;

        // edge case, questions being created after all had previously been deleted until there were no questions on the screen
        if (bottomAccordionBar.length === 0) {
            questionNumber = 1;
        } else {
            questionNumber = 1 + parseInt(bottomAccordionBar.attr('id').slice(0, 1));
        }
        
        var optionsHtml = EQ.questionTextTemplate.format(questionNumber, '');

        optionsHtml += EQ.optionsTemplate.format(questionNumber.toString(), '1', 'a', '', '', '-1');

        EQ.newQuestionLastId -= 1;
        var newQuestionHtml = EQ.questionTemplate.format(questionNumber, optionsHtml, EQ.newQuestionLastId, '1');

        EQ.submitButtons.before(newQuestionHtml);

        var newQuestionPanel = bottomAccordionBar.next();

        if (newQuestionPanel.length === 0) {
            newQuestionPanel = $('#collapse1');
        }

        var newQid = questionNumber.toString() + '1';
        
        var trashOption = newQuestionPanel.find('i#' + newQid + '-delete');
        trashOption.on('click', EQ.newQuDeleteOption);

        // hook up event handlers
        newQuestionPanel.find('button').first().on('click', EQ.newQuDeleteQuestion);
        newQuestionPanel.find('button').last().on('click', EQ.newQuAddOption);

        EQ.newQuestions.push(EQ.newQuestionLastId);
        $('#' + questionNumber + '-questionText').focus();
    };
    
    ns.newQuDeleteOption = function (e) {

        e.preventDefault();
    
        var currentTarget = e.currentTarget;

        // get info about the question
        var accordionGroup = $(currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        // get info about the option
        var optionDiv = $(currentTarget).parent().parent();
        var optionId = optionDiv.data('id');
        var optionDivId = optionDiv.attr('id').slice(1, 2);

        EQ.reArrangeRemainingOptions(optionDiv.nextAll(), optionDivId, questionNumber);
        
        var optionCntSpan = $('#' + questionNumber + '-oCnt');
        var optionCount = parseInt(optionCntSpan.text());
        optionCntSpan.text((optionCount - 1).toString());

        // delete this option from the dom.
        optionDiv.fadeOut(200, function () {
            $(this).remove();
        });
    };

    ns.newQuDeleteQuestion = function(e) {
        e.preventDefault();

        var currentTarget = e.currentTarget;

        // get info about the question
        var accordionGroup = $(currentTarget.closest('div.accordion-group'));
        var questionId = accordionGroup.data('id');
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        EQ.newQuestions = _.without(EQ.newQuestions, _.find(EQ.newQuestions, function(newQuestionId) {
            return newQuestionId === questionId;
        }));

        EQ.reArrangeRemainingQuestions(accordionGroup, questionNumber);

        // delete this question from the dom.
        accordionGroup.fadeOut(200, function () {
            $(this).remove();
        });
    };

    ns.newQuAddOption = function(e) {
        e.preventDefault();

        var btnClicked$ = $(e.currentTarget);

        var lastOption = btnClicked$.parent().parent().find('div[id$="-option"]').last();

        // make sure that the most recent option added has text
        if (lastOption.length !== 0 && $.trim(lastOption.find('input[type="text"]').val()) === '') {
            console.info('toast this');
            return;
        }

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));
        var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

        var optionCntSpan = $('#' + questionNumber + '-oCnt');
        var currentOptionCount = parseInt(optionCntSpan.text());
        optionCntSpan.text((currentOptionCount + 1).toString());

        var letterOfLastOption,
            nextletter,
            idOfLastOption;

        if (lastOption.length === 0) {
            nextletter = 'a';
            idOfLastOption = 0;
        } else {
            letterOfLastOption = lastOption.find('span:first-child').text();
            nextletter = String.fromCharCode((letterOfLastOption.charCodeAt(0) + 1));
            idOfLastOption = parseInt(lastOption.data('id'));
        }

        var newOptionId = 0;
        // If last option was already existing, make the new one -1. Else, decrement it by 1.
        // The idea being each new option has a lower negative number.
        if (idOfLastOption > 0) {
            newOptionId = -1;
        } else {
            newOptionId = idOfLastOption - 1;
        }

        var newOptionDiv = EQ.optionsTemplate.format(questionNumber, currentOptionCount + 1, nextletter, '', '', newOptionId);

        btnClicked$.parent().prev().append(newOptionDiv);

        // hook up events for new option
        var newOptionDiv$ = accordionGroup.find('#' + questionNumber.toString() + (currentOptionCount + 1).toString() + '-option');
        newOptionDiv$.find('i.icon-trash').on('click', EQ.newQuDeleteOption);
        newOptionDiv$.find('input[type="text"]').focus();
    };

    ns.getNewQuestionsFromDom = function() {

        var options;
        var questions = [];
        var quizQuizWithQuestions;
        var invalidAnswersList = []; // for validating number of answers per question.
        var numOfCorrectAnswers;

        _.each(EQ.newQuestions, function (questionId, idx) {

            var accordionGroup = EQ.mainForm.find('div[class="accordion-group"][data-id="' + questionId + '"]');
            var optionDivs = accordionGroup.find('div[id$="-option"]');
            var questionNumber = parseInt(accordionGroup.attr('id').slice(0, 1));

            options = null;
            options = [];
            quizQuizWithQuestions = [];
            numOfCorrectAnswers = [];

            _.each(optionDivs, function (optionDiv, idx) {

                var commonIdPrefix = '#' + questionNumber.toString() + (idx + 1).toString();

                // Next couple of lines check to ensure that there is at least 1 answer and at most 1 answer.
                // If not, function returns the number of answers.
                var answer = $(commonIdPrefix + '-option').find('input[type="checkbox"]').is(':checked');
                if (answer) {
                    numOfCorrectAnswers.push(commonIdPrefix);
                }

                var optionText = $.trim($(commonIdPrefix + '-optionInput').val());

                var optionId;
                if ($.trim(optionText).toLowerCase() === 'true') {
                    optionId = 1;
                } else if ($.trim(optionText).toLowerCase() === 'false') {
                    optionId = 2;
                }

                var opt;
                if (optionId) {
                    opt = { Text: optionText, Id: optionId };
                } else {
                    opt = { Text: optionText };
                }
                var questionWithOption = {
                    Letter: $(commonIdPrefix + '-option').find('span:first-child').text(),
                    CorrectAnswer: $(commonIdPrefix + '-option').find('input[type="checkbox"]').prop('checked'),
                    Option: opt
                };

                options.push(questionWithOption);
            });

            var quizWithQuestion = {
                idQuiz: EQ.quizId.val() || -1, // -1 is important as the fact that a value less than 0 is given for new Quizes is used at the server.
                QuestionNumber: questionNumber
            };

            quizQuizWithQuestions.push(quizWithQuestion);

            questions.push({
                Text: $('#' + questionNumber.toString() + '-questionText').val(),
                QuestionWithOptions: options,
                QuizWithQuestions: quizQuizWithQuestions
            });

            if (numOfCorrectAnswers.length !== 1) {
                invalidAnswersList[questionNumber.toString()] = numOfCorrectAnswers.length;
            }
        });

        if (invalidAnswersList.length > 0) {
            return { valid: false, data: invalidAnswersList };
        }
        else {
            return { valid: true, data: questions };
        }
    };

    ns.CloneQuiz = function(e) {
        e.preventDefault();

        var payload = {
            webinarId: EQ.cloneInput.val(),
            quizId: EQ.quizId.val()
        };
            
        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Quiz/CloneQuiz',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function () {
                
            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                EQ.quizId.val(data.QuizId);
                $('#webinarIdSpan').text(payload.webinarId);
                EQ.webinarId.val(payload.webinarId);
                EQ.cloneInput.val('');

                EQ.rePopulateInputs();

            } else {
                
            }            
        });


    };
	  
    ns.editWebinar = function(e) {

        e.preventDefault();

        var payload = {
            webinarId: EQ.editInput.val()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Quiz/LoadExistingQuiz',
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            beforeSend: function() {

            }
        }).done(function(data, textStatus, jqXHR) {
            if (data.Result === 'Success') {
                EQ.quizId.val(data.QuizId);
                $('#webinarIdSpan').text(payload.webinarId);
                EQ.webinarId.val(payload.webinarId);
                EQ.editInput.val('');

                EQ.rePopulateInputs();

            } else {

            }
        });


    };


})(EQ);