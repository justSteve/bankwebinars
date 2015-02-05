
namespace CUWebinars.Web.Mapping.Mappers
{
    public interface IUniversalMapper
    {
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }

}
