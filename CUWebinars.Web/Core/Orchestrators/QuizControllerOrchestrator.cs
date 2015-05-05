using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json.Linq;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class QuizControllerOrchestrator : IQuizControllerOrchestrator
    {
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;

        public QuizControllerOrchestrator(IWebinarManagementService webinarManagementService, IOrderManagementService orderManagementService, IStateService stateService)
        {
            _webinarManagementService = webinarManagementService;
            _orderManagementService = orderManagementService;
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

        public ActionResult ShowQuizLandingView(int idOrder, string quizCode, string quizCodeWithOrderId, IIdentity userIdentity)
        {
            var claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)userIdentity;

            if (claimsIdentityOfAuthenticatedUser.IsAuthenticated)
            {
                var viewResult = GetIndexViewResult(quizCode, idOrder);

                return viewResult;
            }

            if (_stateService.HasValue(WebUiConstants.QuizAnonUserIdentified) && _stateService.GetValue<bool>(WebUiConstants.QuizAnonUserIdentified))
            {
                var viewResult = GetIndexViewResult(quizCode, idOrder);

                return viewResult;
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Identify", controller = "Quiz", onDemandCode = quizCodeWithOrderId }));
        }

        private ViewResult GetIndexViewResult(string quizCode, int orderId)
        {
            var questionCountAndWebinarId = _webinarManagementService.GetQuizQuestionCountAndWebinarId(quizCode);

            var quizModel = new QuizModel
            {
                OrderId = orderId,
                QuestionCount = questionCountAndWebinarId.QuestionCount,
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
                var newJson = new JProperty(string.Concat("QuizAccessByAnonUser-", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)),
                    new JObject(
                        new JProperty("Name", identifyModel.FullName),
                        new JProperty("Email", identifyModel.Email)
                        ));

                order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, newJson);

                _orderManagementService.SaveChanges();

                _stateService.SetValue(WebUiConstants.QuizAnonUserIdentified, true);

                return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Quiz", onDemandCode = identifyModel.OnDemandCode }));
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
                        Letter = questionWithOptions.Letter[0],
                        Text = questionWithOptions.Option.Text
                    });

                    if (questionWithOptions.CorrectAnswer)
                        quizQuestion.Solutions.Add(questionWithOptions.Letter[0]);
                };

                quizQuestion.QuestionNumber =
                        question.QuizWithQuestions.First(q => q.idQuestion == question.Id).QuestionNumber;

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