using System.Security.Principal;
using System.Web.Mvc;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IQuizControllerOrchestrator
    {
        UserQuizEditModel BuildUserQuizEditModel(int idWebinar);
        UserQuizEditModel BuildUserQuizEditModel(string quizCode, int orderId);
        ActionResult Identify(IdentifyModel identifyModel);
        ActionResult ShowQuizLandingView(int idOrder, string quizCode, string quizCodeWithOrderId, IIdentity userIdentity, string email = null);
        ContentResult ScoreQuizAndPersistResults(UserQuizEditModel userQuizEditModel);
        void ProcessEditModel(EditQuizEditModel model);
        void AddQuiz(AddQuizEditModel model);
        int? CloneQuizForWebinar(int webinarId, int existingQuizId);
    }
}