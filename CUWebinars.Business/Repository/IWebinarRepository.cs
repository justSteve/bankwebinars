using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IWebinarRepository : IDisposable
    {
        void Add(Webinar webinar);
        void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice);
        void Delete(Webinar webinar);
        void MarkForDeletion(object domainObject);
        Webinar FindById(int id);
        Webinar FindByIdLoaded(int id);
        IEnumerable<Webinar> FindByPresenterLastName(string lastName);
        IEnumerable<Webinar> FindByPresenterFullName(string searchTerm);
        IEnumerable<Webinar> FindByDescription(string topicDescription);
        IQueryable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar);
        IQueryable<Webinar> GetUpcoming();
        IQueryable<Webinar> GetRecorded();
        IQueryable<Webinar> GetAllActive();
        IQueryable<Topic> GetAllTopics();
        IQueryable<Order> GetOrdersByWebinar(int webinarId);
        IQueryable<Order> GetOrdersByWebinarForInvoice(int webinarId);
        IEnumerable<Order> GetOrdersByWebinarForPostEventClaims(int idWebinar);
        IQueryable<Order> GetAllOrdersByWebinarForUser(int webinarId, int userId);
        IQueryable<Webinar> GetByTopic(int topicId);
        List<RegType> GetCurrentOptions(int idWebinar);
        IList<Order> GetOrdersByWebinarForConnectionInfo(int id);
        IQueryable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar);
        RegTypesGroupsXref GetRegTypesGroupsXref(int idRegTypesGroupsXref, int idWebinar);
        IQueryable<RegTypesGroup> GetUpcomingRegTypesForWebinars();
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        IQueryable<Topic> GetTopicsPerWebinar(int idWebinar);
        void Update(Webinar webinar);
        IQueryable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar);
        WebinarTopicXref GetWebinarTopicXref(int idTopic, int idWebinar);
        int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar);
        int SaveChanges();
        IList<Webinar> MigrateWebinarsFromLegacy();
        int GetRegTypeByACS(string registrationType, int idWebinar);
        double[] GetCostOfUpgrades(int idWebinar);
        Webinar GetWebinarByJoinCode(string joinCode);
        IList<Webinar> GetSearchDTO(string searchTerm);

        IList<int> GetV3OrdersIdsByWebinar(int idWebinar);
        int? GetNextCompliancePerspectives();
        IList<Webinar> GetWebinarsForWeeklyInvoice(DateTime startDate);

        IEnumerable<Webinar> GetRelated(int? idWebinar);
        IEnumerable<Webinar> GetTopicsByWebinar(int? idWebinar);
    }
}