using AutoMapper;
using CUWebinars.Web.Mapping.Configuration;

namespace CUWebinars.Web.Mapping.Profiles
{
    public class TtsProfile : Profile
    {
        const string TtsProfileName = "TtsProfile";

        public override string ProfileName
        {
            get { return TtsProfileName; }
        }

        protected override void Configure()
        {
            new ViewModelMappings(this).Initialize();
        }
    }
}