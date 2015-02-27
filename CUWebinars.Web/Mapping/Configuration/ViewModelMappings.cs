using AutoMapper;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

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
                .ForMember(webinarEditModel => webinarEditModel.SelectedPresenter, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedStatus, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedTopics, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.Topics, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.RegTypeGroups, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.SelectedRegTypeGroups, webinar => webinar.Ignore())
                .ForMember(webinarEditModel => webinarEditModel.PostedRegTypeGroups, webinar => webinar.Ignore());

        }
    }
}