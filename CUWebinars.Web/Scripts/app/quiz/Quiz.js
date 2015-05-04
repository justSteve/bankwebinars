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
            return this.questionsList;
        };

        Quiz.prototype.getOrderrId = function () {
            return this.orderId;
        };

        Quiz.prototype.getWebinarId = function () {
            return this.webinarId;
        };

        Quiz.prototype.getWebUserId = function () {
            return this.webUserId;
        };

        Quiz.prototype.setCompleted = function (val) {
            this.status = val;
        };

        Quiz.prototype.setOrderId = function (val) {
            this.orderId = val;
        };

        Quiz.prototype.setWebinarId = function (val) {
            this.webinarId = val;
        };

        Quiz.prototype.setWebUserId = function (val) {
            this.webUserId = val;
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
            this.optionsList.push(newOption);
        };

        Question.prototype.getOptions = function () {
            return this.optionsList;
        };

        Question.prototype.getOptionsCount = function () {
            return this.optionsList.length;
        };

        Question.prototype.getText = function () {
            return this.text;
        };

        Question.prototype.getAnswer = function () {
            _.each(this.optionsList, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
            });
            return '';
        };

        Question.prototype.setText = function (value) {
            this.text = value;
        };

        Question.prototype.setOptions = function (value) {
            this.optionsList = value;
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
})(QuizDomain || (QuizDomain = {}));
//# sourceMappingURL=Quiz.js.map
