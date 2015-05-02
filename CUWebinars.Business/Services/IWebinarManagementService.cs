using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IWebinarManagementService : IDisposable
    {
        void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice);
        void AddWebinar(Webinar webinar);
        void AddWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        void DeleteWebinar(int idWebinar );
        void DeleteWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        void DeleteRegTypeGroupXRef(Webinar webinar, int idRegTypeGroupXRef);
        IDictionary<RegType, bool> FindRegTypesByWebinarId(int webinarId);
        IEnumerable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar);
        IEnumerable<Webinar> GetAllActive();
        IEnumerable<Presenter> GetAllPresenters();
        IEnumerable<Topic> GetAllTopics();
        IEnumerable<Webinar> GetByTopic(int topicId);
        IEnumerable<Webinar> GetRecordedWebinars();
        IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar);
        IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar);
        IEnumerable<Webinar> GetUpcomingWebinars();
        IEnumerable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar);
        IEnumerable<RegTypesGroup> GetUpcomingRegTypesForWebinars();
        Webinar GetWebinar(int id);
        IEnumerable<Webinar> GetWebinarByPresenterLastName(string lastName);
        IEnumerable<Webinar> GetWebinarByDescription(string topicDescription);
        WebinarFile GetWebinarFile(int idWebinarFile);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        int SaveChanges();
        void UpdateWebinar(Webinar webinar);
        void UpdateWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        Webinar GetCompliancePerspectives();

        int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar);
        void SynchToLegacy();
        int GetRegTypeByACS(string registrationType, int idWebinar);
        void DeleteWebinarTopicXref(Webinar webinar, int exisingTopicId);
        void AddQuiz(int selectedWebinar, IEnumerable<Question> questions);
    }
}
