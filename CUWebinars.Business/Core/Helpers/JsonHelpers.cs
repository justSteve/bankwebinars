using CUWebinars.Business.Constants;
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
                JObject objectToValidate;
                try
                {
                    if (existingJson.Contains("moneris"))
                        existingJson = existingJson.Replace("moneris", "MonerisDupeTx");
                    
                    //when moneris tx already exists (but why would that be?)
                    //code execution does not step into catch block - wtf.

                    //does this enlighten?
                    //http://stackoverflow.com/questions/29830198/newtonsoft-jobject-parse-throws-base-exception-how-to-handle

                    objectToValidate = JObject.Parse(existingJson);
                }
                catch (Exception e)
                {
                    if (e.GetType().IsSubclassOf(typeof(Exception)))
                        throw;

                    //Handle the case when e is the base Exception
                    objectToValidate = JObject.FromObject(new
                    {
                        existing = existingJson,
                        exceptionMsg = e.Message,
                        exceptionStack = e.StackTrace
                    });
                }
                jObject = JObject.Parse(objectToValidate.ToString());
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
                existingStoredJsonObject.Add(JsonPropertyKeys.PostEventMaterialsWereAccessed, jArray);

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

            return jsonAsString;
        }

        public static JObject CreateJsonObjectFromDictionary(IDictionary<string, string> dataForJson)
        {
            var outObject = new JObject();

            foreach (var keyValuePair in dataForJson)
            {
                outObject.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return outObject;
        }

    }
}
