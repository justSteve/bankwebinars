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
        ActionResult ShowQuizLandingView(int idOrder, string quizCode, string quizCodeWithOrderId,
            IIdentity userIdentity);
    }
}