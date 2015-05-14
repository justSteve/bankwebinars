using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CUWebinars.Business.Core.Extensions;
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
            JObject ob;
            JArray jArray;

            var jObject = new JObject(CreateJsonPropertiesFromObject(objectToJsonify));


            if (string.IsNullOrWhiteSpace(jsonAsString))
            {
                ob = new JObject();
                jArray = new JArray(jObject);
                ob.Add(keyOfArray, jArray);

                return ob.ToString(Formatting.None);
            }

            ob = JObject.Parse(jsonAsString);

            // 1st, see if json is stored at all

            if (!ReferenceEquals(null, ob))
            {
                JToken jToken;

                //  see if array property already exists
                if (ob.TryGetValue(keyOfArray, StringComparison.OrdinalIgnoreCase, out jToken))
                {
                    jArray = jToken.Value<JArray>();

                    jArray.Add(jObject);
                    return ob.ToString(Formatting.None);

                }

                // if not, just create it
                jArray = new JArray(jObject);
                ob.Add(keyOfArray, jArray);
                return ob.ToString(Formatting.None);
            }

            return string.Empty;
        }

    }
}
