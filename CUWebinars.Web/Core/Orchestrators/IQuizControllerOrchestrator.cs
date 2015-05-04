using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IQuizControllerOrchestrator
    {
        UserQuizEditModel BuildUserQuizEditModel(int idWebinar);
    }
}