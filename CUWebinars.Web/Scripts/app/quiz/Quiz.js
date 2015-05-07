/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />
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
            this.Options.push(newOption);
        };

        Question.prototype.getOptions = function () {
            return this.Options;
        };

        Question.prototype.getOptionsCount = function () {
            return this.Options.length;
        };

        Question.prototype.getQuestionNumber = function () {
            return this.QuestionNumber;
        };

        Question.prototype.getUserAnswers = function () {
            return this.UserAnswers;
        };

        Question.prototype.getText = function () {
            return this.Questiontext;
        };

        Question.prototype.getAnswer = function () {
            _.each(this.Options, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
                return '';
            });
            return '';
        };

        Question.prototype.setQuestionNumber = function (value) {
            this.QuestionNumber = value;
        };

        Question.prototype.setText = function (value) {
            this.Questiontext = value;
        };

        Question.prototype.setUserAnswers = function (value) {
            this.UserAnswers = null;
            this.UserAnswers = value;
        };

        Question.prototype.setOptions = function (value) {
            this.Options = value;
        };
        return Question;
    })();
    QuizDomain.Question = Question;
    ;

    var Option = (function () {
        function Option() {
        }
        Option.prototype.getText = function () {
            return this.Text;
        };

        Option.prototype.getLetter = function () {
            return this.Letter;
        };

        Option.prototype.getCorrectAnswer = function () {
            return this.correctAnswer;
        };

        Option.prototype.setCorrectAnswer = function (value) {
            this.correctAnswer = value;
        };

        Option.prototype.setText = function (value) {
            this.Text = value;
        };

        Option.prototype.setLetter = function (value) {
            this.Letter = value;
        };
        return Option;
    })();
    QuizDomain.Option = Option;
    ;
})(QuizDomain || (QuizDomain = {}));
//# sourceMappingURL=quiz.js.map
