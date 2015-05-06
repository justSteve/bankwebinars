/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />

module QuizDomain {

    declare var $; // declare jQuery
    declare var _; // declare underscore

    export enum CompletionStatus { NotStarted, Incomlete, Complete };

    export class Quiz {
        private status: CompletionStatus;
        private orderId: number;
        private webinarId: number;
        private webUserId: number;
        private questionsList: Question[];
        
        getCompleted(): CompletionStatus {
            return this.status;
        }

        getQuestions(): Question[] {
            return this.questionsList;
        }

        getOrderrId(): number {
            return this.orderId;
        }

        getWebinarId(): number {
            return this.webinarId;
        }

        getWebUserId(): number {
            return this.webUserId;
        }

        setCompleted(val: CompletionStatus): void {
            this.status = val;
        }

        setOrderId(val: number): void {
            this.orderId = val;
        }

        setQuestions(val: Question[]): void {
            this.questionsList = val;
        }

        setWebinarId(val: number): void {
            this.webinarId = val;
        }

        setWebUserId(val: number): void {
            this.webUserId = val;
        }
    }

    export class Question {

        private optionsCount: number;
        private questionNumber: number;
        private optionsList: Option[];
        private userAnswers: string[];
        private text: string;

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

        getQuestionNumber(): number {
            return this.questionNumber;
        }

        getuserAnswers(): string[] {
            return this.userAnswers;
        }

        getText(): string {
            return this.text;
        }

        getAnswer(): string {
            _.each(this.optionsList, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
            });
            return '';
        }

        setQuestionNumber(value: number): void {
            this.questionNumber = value;
        }

        setText(value: string): void {
            this.text = value;
        }

        setUserAnswers(value: string[]): void {
            this.userAnswers = value;
        }

        setOptions(value: Option[]): void {
            this.optionsList = value;
        }
    };

    export class Option {
        private text: string;
        private letter: string;
        private correctAnswer: boolean; // only relevant for Admin section in creating the Quiz

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