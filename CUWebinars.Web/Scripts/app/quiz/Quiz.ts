/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />

module QuizDomain {

    declare var $; // declare jQuery
    declare var _; // declare underscore

    export enum CompletionStatus { NotStarted, Incomlete, Complete };

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
            this.quizId= val;
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
        private QuestionNumber: number;
        private Options: Option[];
        private UserAnswers: string[];
        private Questiontext: string;

        constructor() { }

        incrementOptionsCount(): void {
            this.optionsCount++;
        }
        decrementOptionsCount(): void {
            this.optionsCount--;
        }

        addOption(newOption: Option) {
            this.Options.push(newOption);
        }

        getOptions(): Option[] {
            return this.Options;
        }

        getOptionsCount(): number {
            return this.Options.length;
        }

        getQuestionNumber(): number {
            return this.QuestionNumber;
        }

        getUserAnswers(): string[] {
            return this.UserAnswers;
        }

        getText(): string {
            return this.Questiontext;
        }

        getAnswer(): string {
            _.each(this.Options, function (option) {
                if (option.getCorrectAnswer() === true)
                    return option.getLetter();
                return '';
            });
            return '';
        }

        setQuestionNumber(value: number): void {
            this.QuestionNumber = value;
        }

        setText(value: string): void {
            this.Questiontext = value;
        }

        setUserAnswers(value: string[]): void {
            this.UserAnswers = null;
            this.UserAnswers = value;
        }

        setOptions(value: Option[]): void {
            this.Options = value;
        }
    };

    export class Option {
        private Text: string;
        private Letter: string;
        private correctAnswer: boolean; // only relevant for Admin section in creating the Quiz

        constructor() { }


        getText(): string {
            return this.Text;
        }

        getLetter(): string {
            return this.Letter;
        }

        getCorrectAnswer(): boolean {
            return this.correctAnswer;
        }

        setCorrectAnswer(value: boolean): void {
            this.correctAnswer = value;
        }

        setText(value: string): void {
            this.Text = value;
        }

        setLetter(value: string): void {
            this.Letter = value;
        }
    };

    
} 