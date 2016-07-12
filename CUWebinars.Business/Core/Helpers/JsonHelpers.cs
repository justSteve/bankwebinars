using CUWebinars.Business.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CUWebinars.Business.Core.Helpers
{
    public class JsonHelpers
    {
        //usage:
        //    JProperty somthingMsg = new JProperty(
        //    JsonPropertyKeys.somthingKey,
        //    sometingVal.Value
        //    );

        // the msgs & keys have constants in 

        //newOrder.AdminComments = JsonHelpers.MergeJsonWithStoredField(newOrder.AdminComments, createdByImpersonatedUserMsg);

        public static string IsValidObjectSingle(string existingJson)
        {
            JObject objectToValidate;
            try
            {

                objectToValidate = JObject.Parse(existingJson);
                return "isSingle";
            }
            catch (Exception e)
            {
                return "error";

            }

        }

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

                    objectToValidate = JObject.Parse(existingJson);

                    IList<string> keys = objectToValidate.Properties().Select(p => p.Name).ToList();
                    foreach (var myKey in keys)
                    {
                        Debug.WriteLine(myKey);
                    }
                }
                catch (Exception e)
                {
                    //if (e.GetType().IsSubclassOf(typeof (Exception)))
                    //    newJson = null;

                    //Handle the case when e is the base Exception
                    objectToValidate = JObject.FromObject(new
                    {
                        existing = "wrapped Json=" + existingJson,
                        exceptionMsg = e.Message,
                        exceptionStack = e.StackTrace
                    });
                }
                jObject = JObject.Parse(objectToValidate.ToString());
                jObject.Add(newJson);
            }

            return jObject.ToString(Formatting.None);
        }


        public static string ReplaceJsonWithStoredField(string existingJson, JProperty newJson, string propToReplace)
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

                    objectToValidate = JObject.Parse(existingJson);

                    IList<string> keys = objectToValidate.Properties().Select(p => p.Name).ToList();
                    foreach (var myKey in keys)
                    {
                        Debug.WriteLine(myKey);
                    }
                }
                catch (Exception e)
                {
                    //if (e.GetType().IsSubclassOf(typeof (Exception)))
                    //    newJson = null;

                    //Handle the case when e is the base Exception
                    objectToValidate = JObject.FromObject(new
                    {
                        existing = "wrapped Json=" + existingJson,
                        exceptionMsg = e.Message,
                        exceptionStack = e.StackTrace
                    });
                }
                jObject = JObject.Parse(objectToValidate.ToString());

                jObject.Remove(propToReplace);
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
            try
            {
                existingStoredJsonObject = JObject.Parse(jsonAsString);

            }
            catch (Exception e)
            {
               existingStoredJsonObject = JObject.FromObject(new
                    {
                        existing = "wrapped Json=" + jsonAsString,
                        exceptionMsg = e.Message,
                        exceptionStack = e.StackTrace
                    });
            }

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

        public static string RemoveJObject(string hostObject, string objToRemove)
        {

            JArray jArray;
            JObject jPropHost;
            JObject jPropRemove;
            // convert object to a json object
            try
            {
                 jPropHost = JObject.Parse(hostObject);

            }
            catch (Exception ex)
            {
                return "failed to parse hostObject: " + ex;
            }
            

            // ************* if we got here, we need to now remove the existing prop *************
            try
            {
                jPropRemove = JObject.Parse(objToRemove);

            }
            catch (Exception ex)
            {
                return "failed to parse objToRemove: " + ex;
            }

            // 1st, see if valid json is stored at all
            if (!ReferenceEquals(null, jPropRemove))
            {
                JToken jToken;

                // 2nd, see if the array which we are augmenting already exists
                if (jPropHost.TryGetValue(objToRemove, StringComparison.OrdinalIgnoreCase, out jToken))
                {
                    jArray = jToken.Value<JArray>();
                    jArray.Remove(jPropRemove);

                    return jPropHost.ToString(Formatting.None);
                }

                return "failed to find jPropRemove";
            }

            return "failed";
        }
    }
}
