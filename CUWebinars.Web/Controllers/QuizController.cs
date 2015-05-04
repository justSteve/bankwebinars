using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizControllerOrchestrator _quizControllerOrchestrator;

        public QuizController(IQuizControllerOrchestrator quizControllerOrchestrator)
        {
            _quizControllerOrchestrator = quizControllerOrchestrator;
        }

        // GET: Quiz
        public ActionResult Index(int? id)
        {
            var userQuizEditModel = _quizControllerOrchestrator.BuildUserQuizEditModel(id.Value);
            
            return View(userQuizEditModel);
        }
    }
}