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
                .ForMember(webinarEditModel => webinarEditModel.PostedRegTypeGroups, webinar => webinar.Ignore());

            // flattens the Order structure a little bit to allow
            //  easiers consumption by server-side DataTables pattern
            //  https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
            Profile.CreateMap<Order, OrderDTO>()
                .ForMember(d => d.Affiliate_ttsDomain,
                           map => map.MapFrom(s => s.Affiliate.ttsDomain))
                .ForMember(d => d.TtsJoinUrl,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).TtsJoinUrl))
                .ForMember(d => d.Discount,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Discount))
                .ForMember(d => d.RegistrationType,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).RegistrationType))
                .ForMember(d => d.Webinar_IsActive,
                           map => map.MapFrom(s => s.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active).Webinar.Status == WebinarStatus.Active));

        }
    }
}