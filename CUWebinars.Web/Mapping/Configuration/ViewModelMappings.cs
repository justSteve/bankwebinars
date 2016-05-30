using AutoMapper;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;
using System.Linq;

namespace CUWebinars.Web.Mapping.Configuration
{
    public class ViewModelMappings : MappingBase, IMappingInitializer
    {
        public ViewModelMappings(Profile profile)
            : base(profile)
        {

        }

        public void Initialize()
        {
            /*  Initialize all individual mappings here */

            Profile.CreateMap<Discount, DiscountModel>()
                .ForMember(discountModel => discountModel.TypeOfDiscount, discount => discount.MapFrom(d => d.DiscountType));

            Profile.CreateMap<Discount, DiscountDTO>()
                .ForMember(discountModel => discountModel.DiscountType, discount => discount.MapFrom(d => d.DiscountType));

            Profile.CreateMap<Discount, CompliancePerspectivesModel>()
                .ForMember(discountModel => discountModel.TypeOfDiscount, discount => discount.MapFrom(d => d.DiscountType));

            Profile.CreateMap<Webinar, WebinarEditModel>()
                .ForMember(webinarEditModel => webinarEditModel.PostedTopics, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.Presenters, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.AdditionalLocationsPrice, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedPresenter, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedStatus, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedTopics, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.Statuses, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.Topics, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.RegTypeGroups, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedRegTypeGroups, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.PostedRegTypeGroups, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.LivePlusFive, webinar => webinar.Ignore());

            // flattens the Webinar structure a little bit to allow
            //  easier consumption by server-side DataTables pattern
            //  https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
            Profile.CreateMap<Webinar, SearchDTO>()

                .ForMember(d => d.NumOfOrders, s => s.Ignore())
                .ForMember(d => d.PresenterName,
                           map => map.MapFrom(s => s.Presenter.WebUser.FullName))
                .ForMember(d => d.PresenterPhotoFull,
                           map => map.MapFrom(s => s.Presenter.PhotoFull))
                .ForMember(d => d.RelatedTopicsString,
                           map => map.MapFrom(s => string.Join(", ", s.WebinarTopicXrefs.Select(x => x.Topic.topicDesc).ToList())))
                           ;

            Profile.CreateMap<AffiliateReportDTO, AffiliateReportDTO>()
                .ForMember(d => d.Affiliate, s => s.Ignore())
                .ForMember(d => d.Orders, s => s.Ignore())
                .ForMember(d => d.NumOfOrders,
                            map => map.MapFrom(s => s.Orders.Count))
                .ForMember(d => d.TotalRevenues,
                           map => map.MapFrom(s => s.Orders.Select(o => o.Total).Sum()))
                .ForMember(d => d.TotalCommissions,
                           map => map.MapFrom(s => s.Orders.Select(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Royalty).Sum()))
                .ForMember(d => d.Affiliate_ttsDomain,
                           map => map.MapFrom(s => s.Affiliate.ttsDomain))
                 .ForMember(d => d.Affiliate_ContactName,
                           map => map.MapFrom(s => s.Affiliate.ContactPerson + " " + s.Affiliate.ttsDomain))
                 .ForMember(d => d.idAff,
                           map => map.MapFrom(s => s.Affiliate.idUserAff));

            Profile.CreateMap<Order, OrderDTO>()
                            .ForMember(d => d.Webinar, s => s.Ignore())
                .ForMember(d => d.Affiliate_ttsDomain,
                           map => map.MapFrom(s => s.Affiliate.ttsDomain))
                .ForMember(d => d.Institution,
                           map => map.MapFrom
                               (s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Order.WebUser.Institution.InstitutionName + "<br>\n"
                               + s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Order.WebUser.Institution.City + ", "
                               + s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Order.WebUser.Institution.State))
                .ForMember(d => d.WebinarDateTitleString,
                           map => map.MapFrom
                               (s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Webinar.Title + "\n<br><span style=\"font-size: smaller; font-style: italic;\" >" +
                                     "(" + s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Webinar.Date.ToString().Replace(":00 ", " ").ToLower() + " CT)</span>"))
               .ForMember(d => d.TtsJoinUrl,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).TtsJoinUrl))
                .ForMember(d => d.Discount,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Discount))
                .ForMember(d => d.Royalty,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Royalty))
                .ForMember(d => d.ShippedDate,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).ShipmentDate))
                .ForMember(d => d.ShippedDateString,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).ShipmentDate))
                .ForMember(d => d.RegistrationType,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).RegistrationType))
                .ForMember(d => d.Webinar_IsActive,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Webinar.Status == WebinarStatus.Active))
                .ForMember(d => d.Webinar_IsRecorded,
                        map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Webinar.Status == WebinarStatus.Recorded));

            Profile.CreateMap<WebUser, UserDTO>()
                .ForMember(d => d.Institution,
                map => map.MapFrom(s => s.Institution.InstitutionName))
                //.ForMember(d => d.Orders,
                //map => map.MapFrom(s => s.Orders))
                ;

        }
    }
}