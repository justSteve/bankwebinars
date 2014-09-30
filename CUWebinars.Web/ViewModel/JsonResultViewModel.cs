using Newtonsoft.Json;

namespace CUWebinars.Web.ViewModel
{
    public class JsonResultViewModel<T>
    {
        public JsonResultViewModel()
        {
        }

        public JsonResultViewModel(T data)
        {
            Data = data;
        }
        public JsonResultViewModel(bool isSuccessful = false, string msg = "")
        {
            IsSuccessful = isSuccessful;
            Message = msg;
        }

        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}