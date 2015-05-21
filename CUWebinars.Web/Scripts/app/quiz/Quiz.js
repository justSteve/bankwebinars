/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var QuizDomain;
(function (QuizDomain) {
    (function (CompletionStatus) {
        CompletionStatus[CompletionStatus["NotStarted"] = 0] = "NotStarted";
        CompletionStatus[CompletionStatus["Incomlete"] = 1] = "Incomlete";
        CompletionStatus[CompletionStatus["Complete"] = 2] = "Complete";
    })(QuizDomain.CompletionStatus || (QuizDomain.CompletionStatus = {}));
    var CompletionStatus = QuizDomain.CompletionStatus;
    ;

    var Quiz = (function () {
        function Quiz() {
        }
        Quiz.prototype.getCompleted = function () {
            return this.status;
        };

        Quiz.prototype.getQuestions = function () {
            return this.quizQuestions;
        };

        Quiz.prototype.getOrderrId = function () {
            return this.orderId;
        };

        Quiz.prototype.getWebinarId = function () {
            return this.webinarId;
        };

        Quiz.prototype.getEmail = function () {
            return this.email;
        };

        Quiz.prototype.setCompleted = function (val) {
            this.status = val;
        };

        Quiz.prototype.setOrderId = function (val) {
            this.orderId = val;
        };

        Quiz.prototype.setQuestions = function (val) {
            this.quizQuestions = val;
        };

        Quiz.prototype.setQuizId = function (val) {
            this.quizId = val;
        };

        Quiz.prototype.setWebinarId = function (val) {
            this.webinarId = val;
        };

        Quiz.prototype.setEmail = function (val) {
            this.email = val;
        };
        return Quiz;
    })();
    QuizDomain.Quiz = Quiz;

    var Question = (function () {
        function Question() {
        }
        Question.prototype.incrementOptionsCount = function () {
            this.optionsCount++;
        };
        Question.prototype.decrementOptionsCount = function () {
            this.optionsCount--;
        };

        Question.prototype.addOption = function (newOption) {
            this.options.push(newOption);
        };

        Question.prototype.getOptions = function () {
            return this.options;
        };

        Question.prototype.getOptionsCount = function () {
            return this.options.length;
        };

        Question.prototype.getQuestionNumber = function () {
            return this.questionNumber;
        };

        Question.prototype.getUserAnswers = function () {
            return this.userAnswers;
        };

        Question.prototype.getText = function () {
            return this.questiontext;
        };

        // getAnswer not used as solutions are not stored in the dom. Have to go back to server for solution/s
        Question.prototype.getAnswer = function () {
            _.each(this.options, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
                return '';
            });
            return '';
        };

        Question.prototype.setQuestionNumber = function (value) {
            this.questionNumber = value;
        };

        Question.prototype.setText = function (value) {
            this.questiontext = value;
        };

        Question.prototype.setUserAnswers = function (value) {
            this.userAnswers = null;
            this.userAnswers = value;
        };

        Question.prototype.setOptions = function (value) {
            this.options = value;
        };
        return Question;
    })();
    QuizDomain.Question = Question;
    ;

    var Option = (function () {
        function Option() {
        }
        Option.prototype.getText = function () {
            return this.text;
        };

        Option.prototype.getLetter = function () {
            return this.letter;
        };

        Option.prototype.getCorrectAnswer = function () {
            return this.correctAnswer;
        };

        Option.prototype.setCorrectAnswer = function (value) {
            this.correctAnswer = value;
        };

        Option.prototype.setText = function (value) {
            this.text = value;
        };

        Option.prototype.setLetter = function (value) {
            this.letter = value;
        };
        return Option;
    })();
    QuizDomain.Option = Option;
    ;

    var EditableQuestion = (function (_super) {
        __extends(EditableQuestion, _super);
        function EditableQuestion() {
            _super.call(this);
        }
        EditableQuestion.prototype.getQuestionId = function () {
            return this.questionId;
        };

        EditableQuestion.prototype.setQuestionId = function (value) {
            this.questionId = value;
        };
        return EditableQuestion;
    })(Question);
    QuizDomain.EditableQuestion = EditableQuestion;

    var EditableOption = (function (_super) {
        __extends(EditableOption, _super);
        function EditableOption() {
            _super.call(this);
        }
        EditableOption.prototype.getOptionId = function () {
            return this.optionId;
        };

        EditableOption.prototype.setOptionId = function (value) {
            this.optionId = value;
        };
        return EditableOption;
    })(Option);
    QuizDomain.EditableOption = EditableOption;

    var EditedQuestion = (function () {
        function EditedQuestion(id) {
            this.QuestionId = id;
        }
        return EditedQuestion;
    })();
    QuizDomain.EditedQuestion = EditedQuestion;

    var EditedOption = (function () {
        function EditedOption() {
        }
        return EditedOption;
    })();
    QuizDomain.EditedOption = EditedOption;
})(QuizDomain || (QuizDomain = {}));
//# sourceMappingURL=quiz.js.map
