using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class QuizControllerOrchestrator : IQuizControllerOrchestrator
    {
        public const string UserHasAlreadyAchievedAResultForThisQuiz = "A user has already achieved a result for this quiz.";
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;

        public QuizControllerOrchestrator(IWebinarManagementService webinarManagementService, 
            IOrderManagementService orderManagementService, 
            IMembershipService membershipService,
            IStateService stateService)
        {
            _webinarManagementService = webinarManagementService;
            _orderManagementService = orderManagementService;
            _membershipService = membershipService;
            _stateService = stateService;
        }

        public UserQuizEditModel BuildUserQuizEditModel(int idWebinar)
        {
            var quiz = _webinarManagementService.GetQuizByWebinarId(idWebinar);
            return GetUserQuizEditModelFromQuiz(quiz);
        }
        
        public UserQuizEditModel BuildUserQuizEditModel(string quizCode, int orderId)
        {
            var quiz = _webinarManagementService.GetQuizByCode(quizCode);
            var model = GetUserQuizEditModelFromQuiz(quiz);
            model.OrderId = orderId;

            return model;
        }

        public ActionResult ShowQuizLandingView(int idOrder, string quizCode, string quizCodeWithOrderId, IIdentity userIdentity, string email = null)
        {
            var claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)userIdentity;

            if (claimsIdentityOfAuthenticatedUser.IsAuthenticated)
            {
                var viewResult = GetIndexViewResult(quizCode, idOrder, email);

                return viewResult;
            }

            if (_stateService.HasValue(WebUiConstants.QuizAnonUserIdentified) && _stateService.GetValue<bool>(WebUiConstants.QuizAnonUserIdentified))
            {
                var viewResult = GetIndexViewResult(quizCode, idOrder, email);
                _stateService.ClearValue(WebUiConstants.QuizAnonUserIdentified);
                return viewResult;
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Identify", controller = "Quiz", onDemandCode = quizCodeWithOrderId }));
        }

        public ContentResult ScoreQuizAndPersistResults(UserQuizEditModel userQuizEditModel)
        {
            int score;

            ContentResult result = ScoreQuiz(userQuizEditModel, out score);
            
            PersistResultsForQuizAttempt(userQuizEditModel, score);

            //var quizScores = _webinarManagementService.GetQuizScoreForUser(userQuizEditModel.QuizId, userQuizEditModel.Email,
            //    userQuizEditModel.OrderId);

            return result;
        }

        public void ProcessEditModel(EditQuizEditModel model)
        {
            var quizId = model.QuizId;
            bool noUsersHaveAttemptedQuizYet;

            if (!ReferenceEquals(null, model.DeletedQuestions) && model.DeletedQuestions.Any())
            {
                 noUsersHaveAttemptedQuizYet = _webinarManagementService.RemoveQuestionsFromQuiz(model.DeletedQuestions, quizId);
                if (!noUsersHaveAttemptedQuizYet) throw new InvalidOperationException(UserHasAlreadyAchievedAResultForThisQuiz);
            }
            
            if (!ReferenceEquals(null, model.EditedQuestions) && model.EditedQuestions.Any())
            {
                noUsersHaveAttemptedQuizYet = _webinarManagementService.UpdateQuestions(model.EditedQuestions, quizId);
                if (!noUsersHaveAttemptedQuizYet) throw new InvalidOperationException(UserHasAlreadyAchievedAResultForThisQuiz);
            }
            
            if (!ReferenceEquals(null, model.NewQuestions) && model.NewQuestions.Any())
            {
                noUsersHaveAttemptedQuizYet = _webinarManagementService.AddQuestionsToQuiz(model.SelectedWebinar, model.NewQuestions);
                if (!noUsersHaveAttemptedQuizYet) throw new InvalidOperationException(UserHasAlreadyAchievedAResultForThisQuiz);
            }

        }

        private void PersistResultsForQuizAttempt(UserQuizEditModel userQuizEditModel, int score)
        {
            Quiz quiz = _webinarManagementService.GetQuizByQuizId(userQuizEditModel.QuizId);

            var quizUserOrder = new QuizUserOrder
            {
                Email = userQuizEditModel.Email,
                idOrder = userQuizEditModel.OrderId,
                idQuiz = quiz.Id,
                Score = score,
                QuestionCount = userQuizEditModel.QuizQuestions.Count(),
                DateQuizTaken = DateTime.Now
            };

            quiz.QuizUserOrders.Add(quizUserOrder);

            foreach (var quizWithQuestion in quiz.QuizWithQuestions)
            {
                var quizQuestion =
                    userQuizEditModel.QuizQuestions.First(q => q.QuestionNumber == quizWithQuestion.QuestionNumber);

                quizWithQuestion.QuizUserAnswers.Add(new QuizUserAnswer
                {
                    Email = userQuizEditModel.Email,
                    idQuizQuestion = quizWithQuestion.Id,
                    Letter = quizQuestion.UserAnswers.First().ToString(),
                    QuizUserOrder = quizUserOrder
                });
            }

            _webinarManagementService.SaveChanges();

        }

        private ContentResult ScoreQuiz(UserQuizEditModel userQuizEditModel, out int score)
        {
            Quiz quiz = _webinarManagementService.GetQuizByQuizId(userQuizEditModel.QuizId);

            // ReSharper disable once TooWideLocalVariableScope
            bool isCorrect;
            int talley = 0;
            var questionResult = new Dictionary<int, bool>();

            // Compare user attempts with stored solutions. Then create talley for score and 
            // populate dictionary with answer to each question against that question number.
            foreach (var result in quiz.QuizWithQuestions)
            {
                isCorrect = false; // assume user got it wrong until determined otherwise (see inner foreach).

                foreach (var quizQuestion in userQuizEditModel.QuizQuestions)
                {
                    if (result.QuestionNumber == quizQuestion.QuestionNumber)
                    {
                        var correctAnswers = result.Question.QuestionWithOptions
                            .Select(q => new { QuId = q.idQuestion, Letter = q.Letter, Solution = q.CorrectAnswer })
                            .First(q => q.QuId == result.idQuestion && q.Solution);

                        if (quizQuestion.UserAnswers.First() == correctAnswers.Letter.ElementAt(0))
                        {
                            talley++;
                            isCorrect = true;
                        }
                    }
                }
                questionResult.Add(result.QuestionNumber, isCorrect);
            }

            score = talley;

            return new ContentResult
            {
                // use Json.NET to handle serialization of the Dictionary. Hence, did not use JsonResult.
                Content = JsonConvert.SerializeObject(new { Result = WebUiConstants.Success, Score = talley, QuestionsResult = questionResult }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        private ViewResult GetIndexViewResult(string quizCode, int orderId, string email = null)
        {
            var questionCountAndWebinarId = _webinarManagementService.GetQuizQuestionCountAndWebinarId(quizCode);

            var quizModel = new QuizModel
            {
                Email = email ?? string.Empty,
                OrderId = orderId,
                QuestionCount = questionCountAndWebinarId.QuestionCount,
                QuizId = questionCountAndWebinarId.QuizId,
                QuizCode = quizCode,
                Webinar = _webinarManagementService.GetWebinarThin(questionCountAndWebinarId.WebinarId)
            };

            var viewResult = new ViewResult {ViewName = "Index", ViewData = {Model = quizModel}};

            return viewResult;
        }

        public ActionResult Identify(IdentifyModel identifyModel)
        {

            var idOrder = Convert.ToInt32(identifyModel.OnDemandCode.Substring(0, 5));
            // do something with name and email address
            var order = _orderManagementService.GetOrderById(idOrder);

            if (!ReferenceEquals(null, order))
            {
                //var newJson = new JProperty(string.Concat("QuizAccessByAnonUser-", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)),
                //    new JObject(
                //        new JProperty("Name", identifyModel.FullName),
                //        new JProperty("Email", identifyModel.Email)
                //        ));

                //order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, newJson);

                //_orderManagementService.SaveChanges();

                _stateService.SetValue(WebUiConstants.QuizAnonUserIdentified, true);

                return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Quiz", quizCode = identifyModel.OnDemandCode }));
            }

            return new ViewResult { ViewName = "Identify", ViewData = { Model = identifyModel } };
        }

        private UserQuizEditModel GetUserQuizEditModelFromQuiz(Quiz quiz)
        {
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
                        QuestionOptionId = questionWithOptions.Id, 
                        Letter = questionWithOptions.Letter[0],
                        Text = questionWithOptions.Option.Text
                    });

                    if (questionWithOptions.CorrectAnswer)
                        quizQuestion.Solutions.Add(questionWithOptions.Letter[0]);
                };

                quizQuestion.QuestionNumber =
                        question.QuizWithQuestions.First(q => q.idQuestion == question.Id).QuestionNumber;
                quizQuestion.QuestionText = question.Text;
                quizQuestion.QuestionId = question.Id;
                quizQuestions.Add(quizQuestion);
            }

            var model = new UserQuizEditModel
            {
                QuizId = quiz.Id,
                QuizQuestions = quizQuestions,
                WebinarId = quiz.idWebinar
            };

            return model;
           
        }
    }
}