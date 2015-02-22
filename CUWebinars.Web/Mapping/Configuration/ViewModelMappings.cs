using AutoMapper;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

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
            //  Initialize all individual mappings here
            Profile.CreateMap<Discount, DiscountModel>()
                //.ForMember(discountModel => discountModel.Cost, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.DateBilled, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.DateValidFrom, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.DateValidTo, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.DiscountCode, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.FlatOff, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.PercentOff, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.Notes, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.RenewalTerm, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.Status, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.UsesRemain, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.WebUserDiscountXref, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.idDiscount, discount => discount.Ignore())
                //.ForMember(discountModel => discountModel.TypeOfDiscount, discount => discount.Ignore())
                .ForMember(discountModel => discountModel.TypeOfDiscount, discount => discount.MapFrom(d => d.DiscountType));
                //.ForMember(discountModel => discountModel.UsesCount, discount => discount.Ignore());
        }
    }
}