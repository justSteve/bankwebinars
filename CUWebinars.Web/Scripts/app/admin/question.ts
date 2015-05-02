/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />


module AddQuiz {

    declare var $;
    declare var _;

    export class Question {
        
        private optionsCount: number;  
        private optionsList: Option[];
        private text : string;

        constructor() { }

        incrementOptionsCount(): void {
            this.optionsCount++;
        }
        decrementOptionsCount(): void {
            this.optionsCount--;
        }

        addOption(newOption: Option) {
            this.optionsList.push(newOption);
        }

        getOptions(): Option[] {
            return this.optionsList;
        }

        getOptionsCount(): number {
            return this.optionsList.length;
        }

        getText(): string {
            return this.text;
        }

        getAnswer(): string {
            _.each(this.optionsList, function(option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
            });
            return '';
        }

        setText(value: string): void {
            this.text = value;
        }

        setOptions(value: Option[]): void {
            this.optionsList = value;
        }
    };

    export class Option {
        private text: string;
        private letter: string;
        private correctAnswer: boolean;

        constructor() { }


        getText(): string {
            return this.text;
        }

        getLetter(): string {
            return this.letter;
        }

        getCorrectAnswer(): boolean {
            return this.correctAnswer;
        }

        setCorrectAnswer(value: boolean): void {
            this.correctAnswer = value;
        }

        setText(value: string): void {
            this.text = value;
        }

        setLetter(value: string): void {
            this.letter = value;
        }
    };


}