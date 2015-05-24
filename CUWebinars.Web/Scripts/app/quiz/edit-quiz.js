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
    ns.questionTemplate = '<div class="accordion-group" id="{0}-question" data-id={2}><div class="accordion-heading"><a class="accordion-toggle" data-toggle="collapse" data-parent="#questions" href="#collapse{0}">Question #{0}</a></div><div id="collapse{0}" class="accordion-body collapse in"><div id="{0}-options" class="accordion-inner">{1}</div><div class="btn-toolbar"><button class="btn btn-default btn-mini">&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="{0}-delete"></i>&nbsp;Delete Question</button><button class="btn btn-default btn-mini">&nbsp;<i class="icon-plus-sign icon-white" style="cursor: pointer" id="{0}-addOption"></i>&nbsp;Add Option</button><div id="{0}-oCnt">{3}</div></div></div></div>';
    ns.questionTextTemplate = '<div><input id="{0}-questionText" type="text" value="{1}" class="input input-xxlarge" /></div>';
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
                EQ.mainForm.append(questions);

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

        var optionCntDiv = $('#' + questionNumber + '-oCnt');
        var currentOptionCount = parseInt(optionCntDiv.text());
        optionCntDiv.text((currentOptionCount + 1).toString());


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


    ns.qTextEdited = function (e) {

        e.preventDefault();

        var accordionGroup = $(e.currentTarget.closest('div.accordion-group'));

        EQ.addToEditedQuestionsList(accordionGroup);

    };

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
                editedOption.OriginalCorrectStatus = origOption.setCorrectAnswer();
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
            editedQuestion.EditedOptions = _.without(editedQuestion.EditedOptions, _.find(editedQuestion.EditedOptions, function(editedOption, idx) {
                return editedOption.OptionId === optionId;
            }));
        }

        editedOption.EditType = QuizDomain.EditType.Deleted;

        EQ.reArrangeRemainingOptions(optionDiv.nextAll(), optionDivId, questionNumber);

        EQ.addToEditedOptionsList(optionDiv.next(), editedQuestion, optionId);

        var optionCntDiv = $('#' + questionNumber + '-oCnt');
        var optionCount = parseInt(optionCntDiv.text());
        optionCntDiv.text((optionCount - 1).toString());

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

            var editedQuestion = EQ.addOrUpdateQuestion(questionId);

            var origQuestion = _.find(EQ.questionsList, function(question) {
                return question.getQuestionId() === questionId;
            });

            editedQuestion.OriginalQuestionNumber = origQuestion.getQuestionNumber();
            editedQuestion.NewQuestionNumber = parseInt(questionPanel$.attr('id').slice(0,1));
            editedQuestion.OriginalQuestionText = origQuestion.getText();
            editedQuestion.NewQuestionText = questionPanel$.find('input[id$="-questionText"]').val();
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

            accordionGroup$.find('div[id$="-oCnt"]').attr('id', newId.toString() + '-oCnt');

            newId += 1;
        });

        return followingQuestions;
    };


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
            DeletedQuestions: EQ.deletedQuestions,
            EditedQuestions: EQ.editedQuestions
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