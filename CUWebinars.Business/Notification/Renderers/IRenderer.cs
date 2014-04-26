
namespace CUWebinars.Business.Notification.Renderers
{
    public interface IRenderer
    {
        string ConstructMessage<T>(
            EnvironmentInformation applicationInformation,
            string templatedText,
            T objectOfMessage
            );

    }
}
