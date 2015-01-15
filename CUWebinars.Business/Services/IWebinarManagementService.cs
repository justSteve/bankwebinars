using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IWebinarManagementService : IDisposable
    {
        void AddWebinar(Webinar webinar);
        void AddWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        void DeleteWebinar(int idWebinar );
        void DeleteWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        IDictionary<RegType, bool> FindRegTypesByWebinarId(int webinarId);
        IEnumerable<Webinar> GetAllActive();
        IEnumerable<Presenter> GetAllPresenters();
        IEnumerable<Webinar> GetByTopic(int topicId);
        IEnumerable<Webinar> GetRecordedWebinars();
        IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar);
        IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar);
        IEnumerable<Webinar> GetUpcomingWebinars();
        Webinar GetWebinar(int id);
        IEnumerable<Webinar> GetWebinarByPresenterLastName(string lastName);
        IEnumerable<Webinar> GetWebinarByDescription(string topicDescription);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        void UpdateWebinar(Webinar webinar);
        void UpdateWebinarFiles(IEnumerable<WebinarFile> webinarFiles);
        Webinar GetCompliancePerspectives();

        int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar);
    }
}
