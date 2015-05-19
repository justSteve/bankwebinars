using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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


        public static IEnumerable<JProperty> CreateJsonPropertiesFromObject(Object objectToJsonify)
        {
            ICollection<JProperty> properties = new List<JProperty>();

            foreach (var propertyInfo in objectToJsonify.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                properties.Add(new JProperty(propertyInfo.Name, propertyInfo.GetValue(objectToJsonify)));
            }

            return properties;
        }

        public static string AddObjectToJsonArray(string jsonAsString, string keyOfArray, Object objectToJsonify)
        {
            JObject existingStoredJsonObject;
            JArray jArray;

            // convert object to a json object
            var newJsonObject = new JObject(CreateJsonPropertiesFromObject(objectToJsonify));

            // if the existing stored string is null or emptiness, just create it and return.
            if (string.IsNullOrWhiteSpace(jsonAsString))
            {
                existingStoredJsonObject = new JObject();
                jArray = new JArray(newJsonObject);
                existingStoredJsonObject.AddFirst(jArray);

                return existingStoredJsonObject.ToString(Formatting.None);
            }

            // ************* if we got here, we need to now process the existing stored json as an input *************
            existingStoredJsonObject = JObject.Parse(jsonAsString);

            // 1st, see if valid json is stored at all
            if (!ReferenceEquals(null, existingStoredJsonObject))
            {
                JToken jToken;

                // 2nd, see if the array which we are augmenting already exists
                if (existingStoredJsonObject.TryGetValue(keyOfArray, StringComparison.OrdinalIgnoreCase, out jToken))
                {
                    jArray = jToken.Value<JArray>();
                    jArray.AddFirst(newJsonObject); // add so most recent is at beginning of the array

                    return existingStoredJsonObject.ToString(Formatting.None);
                }

                // if the array does not exist in the existing json, just create it
                jArray = new JArray(newJsonObject);
                existingStoredJsonObject.Add(keyOfArray, jArray);

                return existingStoredJsonObject.ToString(Formatting.None);
            }
            
            throw new InvalidDataException("The stored string is not valid json.");
        }

    }
}
