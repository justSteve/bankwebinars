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
            Profile.CreateMap<Discount, DiscountModel>();
        }
    }
}