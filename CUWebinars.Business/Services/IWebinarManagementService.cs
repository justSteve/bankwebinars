using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IWebinarManagementService : IDisposable
    {
        void AddWebinar(Webinar webinar);
        void DeleteWebinar(int idWebinar );
        IEnumerable<RegType> FindRegTypesByWebinarId(int webinarId);
        IEnumerable<Webinar> GetAllActive();
        IEnumerable<Presenter> GetAllPresenters();
        IEnumerable<Webinar> GetByTopic(int topicId);
        IEnumerable<Webinar> GetRecordedWebinars();
        IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar);
        IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar);
        IEnumerable<Webinar> GetUpcomingWebinars();
        Webinar GetWebinar(int id);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        void UpdateWebinar(Webinar webinar);
        Webinar GetCompliancePerspectives();

    }
}
