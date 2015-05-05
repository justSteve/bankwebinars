using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CUWebinars.Business.Core.Helpers
{
    public class JsonHelpers
    {
        public static string MergeJsonWithStoredField(string existingJson, JProperty newJson)
        {
            JObject jObject;

            if (string.IsNullOrWhiteSpace(existingJson))
            {
                jObject = new JObject(newJson);
            }
            else
            {
                jObject = JObject.Parse(existingJson.Trim());
                jObject.Add(newJson);
            }

            return jObject.ToString(Formatting.None);
        }
    }
}
