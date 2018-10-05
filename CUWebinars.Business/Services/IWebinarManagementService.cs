using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using CUWebinars.Business.Repository;

namespace CUWebinars.Business.Services
{
    public interface IWebinarManagementService : IDisposable
    {
        //void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice);
        bool AddQuestionsToQuiz(int selectedWebinar, IEnumerable<Question> newQuestions);
        void AddWebinar(Webinar webinar);
        void AddWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        void DeleteWebinar(int idWebinar );
        void DeleteWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        void DeleteRegTypeGroupXRef(Webinar webinar, int idRegTypeGroupXRef);
        IDictionary<RegType, bool> FindRegTypesByWebinarId(int webinarId);
        //IEnumerable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar);
        IEnumerable<Webinar> GetAllActive();
        IEnumerable<Presenter> GetAllPresenters();
        IEnumerable<Topic> GetAllTopics();
        IEnumerable<Webinar> GetByTopic(int topicId);
        Quiz GetQuizByWebinarId(int idWebinar);
        Quiz GetQuizByOrderId(int idOrder);
        IEnumerable<Webinar> GetRecordedWebinars();
        IEnumerable<Webinar> GetDesWebinars();
        IEnumerable<Webinar> GetCcsWebinars();

        
        IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar);
        IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar);
        IEnumerable<Webinar> GetUpcomingWebinars();
        IEnumerable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar);
        IEnumerable<RegTypesGroup> GetUpcomingRegTypesForWebinars();
        Webinar GetWebinar(int id);
        Webinar GetWebinarThin(int id);
        IEnumerable<Webinar> GetWebinarByPresenterLastName(string lastName);
        IEnumerable<Webinar> GetWebinarsByPresenterFullName(string searchTerm);
        IEnumerable<Webinar> GetWebinarByDescription(string topicDescription);
        WebinarFile GetWebinarFile(int idWebinarFile);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        int SaveChanges();
        void UpdateWebinar(Webinar webinar);
        void UpdateWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        Webinar GetCompliancePerspectives();

        int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar);
  
        int GetRegTypeByRateWatch(string registrationType, int idWebinar);
       
        int GetRegTypeByACS(string registrationType, int idWebinar);
        void DeleteWebinarTopicXref(Webinar webinar, int exisingTopicId);
        Quiz GetQuizByCode(string quizCode);
        QuestionCountAndWebinarId GetQuizQuestionCountAndWebinarId(string quizCode);
        Quiz GetQuizByQuizId(int quizId);
        IList<QuizScore> GetQuizScoreForUser(int quizId, string email, int orderId);
        bool RemoveQuestionsFromQuiz(IEnumerable<int> deletedQuestions, int quizId);
        bool UpdateQuestions(IEnumerable<EditedQuestion> editedQuestions, int quizId);
        int CloneQuizForWebinar(Webinar webinar, int existingQuizId);
        int GetQuizIdByWebinarId(int idWebinar);
        IList<Webinar> GetSearchDTO(string searchTerm);
        int? GetNextCompliancePerspectives();
        IList<Webinar> GetWebinarsForWeeklyInvoices(DateTime startDate);
        IEnumerable<Webinar> GetRelated(int? idWebinar);
        IEnumerable<Webinar> GetTopicsByWebinar(int? idWebinar);

        int ImportLu(WebinarLU importWebinar);
        string ParseForSpeakerName(string descriptionText);
        int? GetUserIdByFirstNameLastName(string fullName);
        string ImportLUEvents();

    }
}
