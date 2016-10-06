/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />
var AddQuiz;
(function (AddQuiz) {
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
    AddQuiz.Question = Question;
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
    AddQuiz.Option = Option;
    ;
})(AddQuiz || (AddQuiz = {}));
//# sourceMappingURL=question.js.map
