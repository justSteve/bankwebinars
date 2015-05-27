using System;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Models;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizControllerOrchestrator _quizControllerOrchestrator;
        private readonly ILogger _logger;
        private readonly IAppHelper _appHelper;

        public QuizController(IQuizControllerOrchestrator quizControllerOrchestrator, ILogger logger, IAppHelper appHelper)
        {
            _quizControllerOrchestrator = quizControllerOrchestrator;
            _logger = logger;
            _appHelper = appHelper;
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult AddQuiz(AddQuizEditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _quizControllerOrchestrator.AddQuiz(model);

                    return Json(new {Result = WebUiConstants.Success, WebinarId = model.SelectedWebinar });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("EditQuiz | Session details: {0}", _appHelper.GetUserAuditInfo()), exception);
                }
            }

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult CloneWebinar(int? webinarId, int? quizId)
        {
            if (webinarId.HasValue && quizId.HasValue)
            {
                var newQuizId = _quizControllerOrchestrator.CloneQuizForWebinar(webinarId.Value, quizId.Value);
                return Json(new { Result = WebUiConstants.Success, QuizId = newQuizId });
            }

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult EditQuiz(EditQuizEditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _quizControllerOrchestrator.ProcessEditModel(model);

                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("EditQuiz | Session details: {0}", _appHelper.GetUserAuditInfo()), exception);
                }
            }

            return this.ModelStateJson(ModelState);
        }


        public ActionResult EditQuizFromDetails(int? webinarId)
        {
            if(webinarId.HasValue)
            {
                var model = _quizControllerOrchestrator.BuildUserQuizEditModel(webinarId.Value);
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "No Webinar Id was passed to the Server.");

            return View();
        }

        public ActionResult Identify(string onDemandCode)
        {
            var model = new IdentifyModel
            {
                Email = string.Empty,
                OnDemandCode = onDemandCode,
                SignInModel = new SignInModel
                {
                    ReturnUrl = "Quiz/Index/" + onDemandCode
                }
            };

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Identify(IdentifyModel identifyModel)
        {
            if (ModelState.IsValid)
            {
                TempData.Add(WebUiConstants.AnonUserEmail, identifyModel.Email);
                return _quizControllerOrchestrator.Identify(identifyModel);
            }

            return View(identifyModel);
        }

        // GET: Quiz
        public ActionResult Index(string quizCode)
        {
            // quizCode contains OrderId and the QuizCode in the following example format "32635-GS6DHJK"

            // get the idOrder
            var parts = quizCode.Trim().Split('-');

            int idOrder = 0;
            if(int.TryParse(parts.First(), out idOrder))
            {
                string email = null;
                if (User.Identity.IsAuthenticated)
                {
                    email = ((ClaimsIdentity) User.Identity).Claims.First(c => c.Type == ClaimTypes.Email).Value;
                }
                else
                {
                    var val = TempData[WebUiConstants.AnonUserEmail];
                    email = val == null ? null : val.ToString();
                }
                
                return _quizControllerOrchestrator.ShowQuizLandingView(idOrder, parts.ElementAt(1), quizCode, User.Identity, email);
            }

            ModelState.AddModelError(string.Empty, "Invalid Order Id");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult GetQuestions(string quizCode, int? orderId)
        {
            if (orderId.HasValue && !string.IsNullOrWhiteSpace(quizCode))
            {
                var model = _quizControllerOrchestrator.BuildUserQuizEditModel(quizCode, orderId.Value);
                return Json(new { Result = WebUiConstants.Success, Quiz = model} );
            }

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult GetQuestionsByWebinarId(int? webinarId)
        {
            if (webinarId.HasValue)
            {
                var model = _quizControllerOrchestrator.BuildUserQuizEditModel(webinarId.Value);
                return Json(new { Result = WebUiConstants.Success, Quiz = model} );
            }

            return Json(new { Result = WebUiConstants.Success }); // Quiz property will be null. Empty quiz created at client
        }

        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult SubmitQuiz(UserQuizEditModel userQuizEditModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return _quizControllerOrchestrator.ScoreQuizAndPersistResults(userQuizEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("SubmitQuiz | Session details: {0}", _appHelper.GetUserAuditInfo()), exception);
                }
            }

            return Json(new { Result = WebUiConstants.Fail }); // todo: error message for user. Not admin user, but quiz-taker
        }
    }
}