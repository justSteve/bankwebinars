/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />

module QuizDomain {

    declare var $; // declare jQuery
    declare var _; // declare underscore

    export enum CompletionStatus { NotStarted, Incomlete, Complete };
    export enum EditType { Added, Edited, Deleted };
    

    export class Quiz {
        private status: CompletionStatus; // not relevant for current requirements. Useful if incomplete quiz can be resumed.
        private orderId: number;
        private webinarId: number;
        private email: string;
        private quizQuestions: Question[];
        private quizId: number;
        
        getCompleted(): CompletionStatus {
            return this.status;
        }

        getQuestions(): Question[] {
            return this.quizQuestions;
        }

        getOrderrId(): number {
            return this.orderId;
        }

        getWebinarId(): number {
            return this.webinarId;
        }

        getEmail(): string {
            return this.email;
        }

        setCompleted(val: CompletionStatus): void {
            this.status = val;
        }

        setOrderId(val: number): void {
            this.orderId = val;
        }

        setQuestions(val: Question[]): void {
            this.quizQuestions = val;
        }

        setQuizId(val: number): void {
            this.quizId = val;
        }

        setWebinarId(val: number): void {
            this.webinarId = val;
        }

        setEmail(val: string): void {
            this.email = val;
        }
    }

    export class Question {

        private optionsCount: number;
        private questionNumber: number;
        private options: Option[];
        private userAnswers: string[];
        private questiontext: string;

        constructor() { }

        incrementOptionsCount(): void {
            this.optionsCount++;
        }
        decrementOptionsCount(): void {
            this.optionsCount--;
        }

        addOption(newOption: Option) {
            this.options.push(newOption);
        }

        getOptions(): Option[] {
            return this.options;
        }

        getOptionsCount(): number {
            return this.options.length;
        }

        getQuestionNumber(): number {
            return this.questionNumber;
        }

        getUserAnswers(): string[] {
            return this.userAnswers;
        }

        getText(): string {
            return this.questiontext;
        }

        // getAnswer not used as solutions are not stored in the dom. Have to go back to server for solution/s
        getAnswer(): string {
            _.each(this.options, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
                return '';
            });
            return '';
        }

        setQuestionNumber(value: number): void {
            this.questionNumber = value;
        }

        setText(value: string): void {
            this.questiontext = value;
        }

        setUserAnswers(value: string[]): void {
            this.userAnswers = null;
            this.userAnswers = value;
        }

        setOptions(value: Option[]): void {
            this.options = value;
        }
    };

    export class Option {
        private text: string;
        private letter: string;
        private correctAnswer: boolean; // only relevant for Admin section in creating the Quiz and Editing

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

    export class EditableQuestion extends Question {

        private questionId: number;


        constructor() { super(); }

        getQuestionId(): number {
            return this.questionId;
        }

        setQuestionId(value: number): void {
            this.questionId = value;
        }

    }

    export class EditableOption extends Option {

        private optionId: number;


        constructor() { super(); }

        getOptionId(): number {
            return this.optionId;
        }

        setOptionId(value: number): void {
            this.optionId= value;
        }

    }

    export class EditedQuestion {
        QuestionId: number;
        OriginalQuestionNumber: number;
        NewQuestionNumber: number;
        OriginalQuestionText: number;
        NewQuestionText: number;
        DeletedOptions: number[];
        EditedOptions: EditedOption[];

        constructor(id:number) {
            this.QuestionId = id;
        }


    }

    export class EditedOption {
        EditType: EditType; // not relevant for current requirements. Useful if incomplete quiz can be resumed.
        OptionId: number;
        OriginalOptionLetter: string; 
        NewOptionLetter: string;
        OriginalOptionText: string;
        NewOptionText: string;
        OriginalCorrectStatus: boolean; 
        NewCorrectStatus: boolean;
    }
} 