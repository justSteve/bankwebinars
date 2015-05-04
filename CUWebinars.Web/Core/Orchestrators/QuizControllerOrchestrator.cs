using CUWebinars.Business.Services;
using CUWebinars.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class QuizControllerOrchestrator : IQuizControllerOrchestrator
    {
        private readonly IWebinarManagementService _webinarManagementService;

        public QuizControllerOrchestrator(IWebinarManagementService webinarManagementService)
        {
            _webinarManagementService = webinarManagementService;
        }

        public UserQuizEditModel BuildUserQuizEditModel(int idWebinar)
        {
            var quiz = _webinarManagementService.GetQuizByWebinarId(idWebinar);
            var questions = quiz.QuizWithQuestions.Select(q => q.Question).ToArray();
            IList<QuizQuestion> quizQuestions = new List<QuizQuestion>();

            foreach (var question in questions)
            {
                var quizQuestion = new QuizQuestion
                {
                    Options = new List<QuestionOption>(), 
                    Solutions = new List<char>()
                };

                foreach (var questionWithOptions in questions.SelectMany(q => q.QuestionWithOptions).Where(q => q.idQuestion == question.Id))
                {
                    quizQuestion.Options.Add(new QuestionOption
                    {
                        Letter = questionWithOptions.Letter[0],
                        Text = questionWithOptions.Option.Text
                    });

                    if(questionWithOptions.CorrectAnswer)
                        quizQuestion.Solutions.Add(questionWithOptions.Letter[0]);
                };
            
                quizQuestion.QuestionNumber =
                        question.QuizWithQuestions.First(q => q.idQuestion == question.Id).QuestionNumber;
                
                quizQuestions.Add(quizQuestion);
            }

            var model = new UserQuizEditModel
            {
                Quiz = quiz,
                QuizQuestions = quizQuestions
            };

            return model;
        }
    }
}