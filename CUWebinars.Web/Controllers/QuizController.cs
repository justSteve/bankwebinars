using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json;

namespace CUWebinars.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizControllerOrchestrator _quizControllerOrchestrator;

        public QuizController(IQuizControllerOrchestrator quizControllerOrchestrator)
        {
            _quizControllerOrchestrator = quizControllerOrchestrator;
        }

        public ActionResult Identify(string onDemandCode)
        {
            var model = new IdentifyModel
            {
                Email = string.Empty,
                OnDemandCode = onDemandCode,
                FullName = string.Empty,
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
        public ActionResult SubmitQuiz(UserQuizEditModel userQuizEditModel)
        {
            return  _quizControllerOrchestrator.ScoreQuizAndPersistResults(userQuizEditModel);
        }
    }
}