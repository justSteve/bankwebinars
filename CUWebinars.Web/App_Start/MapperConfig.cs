using CUWebinars.Web.Mapping.Profiles;

namespace CUWebinars.Web.App_Start
{
    public class MapperConfig
    {
        public static void Initialize()
        {
            var profile = new TtsProfile();
            AutoMapper.Mapper.Initialize(p => p.AddProfile(profile));
        }

    }
}