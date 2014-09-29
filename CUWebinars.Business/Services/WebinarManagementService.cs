using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System.Net;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Services
{
    public class WebinarManagementService : IEventSource, IWebinarManagementService
    {
        //private readonly IAffiliateRepository _affiliateRepository;
        private readonly IRegTypeRepository _regTypeRepository;
        //private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly IWebinarRepository _webinarRepository;
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public WebinarManagementService(
            //IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            //IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
            IWebinarRepository webinarRepository,
            ILogger logger,
            TtsConfiguration ttsConfig)
        {
            //_affiliateRepository = affiliateRepository;
            _regTypeRepository = regTypeRepository;
            //_orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
            _webinarRepository = webinarRepository;
            _logger = logger;
            _webUserRepository = webUserRepository;
        }
        public IEnumerable<Presenter> GetAllPresenters()
        {
            try
            {
                return _refDataRepository.GetAllPresenters();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllPresenters method", exception);
                throw;
            }

        }

        public void AddWebinar(Webinar webinar)
        {
            _webinarRepository.Add(webinar);
        }

        public void DeleteWebinar(int webinarId)
        {
            var webinar = GetWebinar(webinarId);
            _webinarRepository.Delete(webinar);
        }


        public IEnumerable<Webinar> GetByTopic(int topicId)
        {
            return _webinarRepository.GetByTopic(topicId);
        }
        public Webinar GetCompliancePerspectives()
        {
            GetRegistrants_CP();
            return null;

        }
        private IList<WebUser> GetRegistrants_CP()
        {
            return null;

        }
        public IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetTopicsPerWebinar(idWebinar).ToList();
        }

        public IEnumerable<Webinar> GetRecordedWebinars()
        {
            return _webinarRepository.GetRecorded().ToList();
        }
        public IEnumerable<Webinar> GetUpcomingWebinars()
        {
            return _webinarRepository.GetUpcoming().ToList();
        }

        public Webinar GetWebinar(int id)
        {
            return _webinarRepository.FindByIdLoaded(id);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            try
            {
                return _webinarRepository.GetWebinarByIdIncludingAllWebinarsByPresenter(id);
                //return webinar;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Failed attempt to retrieve webinar with id: " + id.ToString(), exception);
                return null;
            }

        }




        public IEnumerable<RegType> FindRegTypesByWebinarId(int webinarId)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(webinarId, false);
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return _events;
        }

        public void Clear()
        {
            _events.Clear();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();


                _regTypeRepository.Dispose();
                _webinarRepository.Dispose();
                _webUserRepository.Dispose();
                
                _disposed = true;
            }
        }

    //    string IWebinarManagementService.CreateCalendarEvent(string title,
    //string body,
    //DateTime startDate,
    //double duration,
    //string location,
    //string organizer,
    //string eventId,
    //bool allDayEvent)
    //    {
    //        try
    //        {
    //            return CreateCalendarEvent(title, body, startDate, duration, location, organizer, eventId, allDayEvent);
    //        }
    //        catch (Exception exception)
    //        {
    //            _logger.ErrorException(string.Format("Title:{0},body:{1},startDate:{2},duration{3},location{4},organizer{5},eventId{6},allDayEvent" +
    //                                                 "{7}", title, body, startDate.ToString("yyyy-MM-dd-hh-mm-ss-fff-tt"), duration, location,
    //                                                 organizer, eventId, allDayEvent),
    //                                                 exception
    //                                                 );
    //            throw;
    //        }
    //    }

        public IEnumerable<Webinar> GetAllActive()
        {
            try
            {
                return _webinarRepository.GetAllActive().ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllActive method", exception);
                throw;
            }
        }

        public void UpdateWebinar(Webinar webinar)
        {
            _webinarRepository.Update(webinar);
        }


    }
}
