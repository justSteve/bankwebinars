
namespace CUWebinars.Web.Services
{
    public interface IStateService
    {
        void SetValue<T>(string key, T value);
        /// <summary>
        /// Returns value associated with the specified key.
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(string key);
        void ClearValue(string key);
        bool HasValue(string key);
    }
}
