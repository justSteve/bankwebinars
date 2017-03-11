using CUWebinars.Business.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Converters;

namespace CUWebinars.Business.Core.Helpers
{
    public static class JsonHelpers
    {

        public static JToken RemoveFields(this JToken token, string[] fields)
        {
            http: //stackoverflow.com/questions/11676159/json-net-how-to-remove-nodes
            JContainer container = token as JContainer;
            if (container == null) return token;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                el.RemoveFields(fields);
            }

            foreach (JToken el in removeList)
            {
                el.Remove();
            }

            return token;
        }

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
                return "error: " + e.Message;

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


                }
                catch (Exception e)
                {
                    //if (e.GetType().IsSubclassOf(typeof (Exception)))
                    //    newJson = null;

                    Debug.WriteLine(e.Message);
                    //Handle the case when e is the base Exception
                    objectToValidate = JObject.FromObject(new
                    {
                        existing = "wrapped Json by MergeJsonWithStoredField = " + existingJson,
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

            JObject objectToValidate = null;
            if (string.IsNullOrWhiteSpace(existingJson))
            {
                jObject = new JObject(newJson);
            }
            else
            {
                try
                {

                    objectToValidate = JObject.Parse(existingJson);

                    IList<string> keys = objectToValidate.Properties().Select(p => p.Name).ToList();
                    Debug.WriteLine(keys);
                    foreach (var myKey in keys)
                    {
                        Debug.WriteLine(myKey);
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        existingJson = existingJson.TrimStart('[').TrimEnd(']');
                        objectToValidate = JObject.Parse(existingJson);
                    }
                    catch (Exception e1)
                    {
                        //Handle the case when e is the base Exception
                        objectToValidate = JObject.FromObject(new
                        {
                            existing = "wrapped Json by ReplaceJsonWIthStoredField =" + existingJson,
                            exceptionMsg = e1.Message,
                            exceptionStack = e1.StackTrace
                        });
                    }
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
            try
            {
                foreach (
                    var propertyInfo in
                    objectToJsonify.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                )
                {
                    if (propertyInfo.GetType() != typeof(Newtonsoft.Json.Linq.JValue))
                        properties.Add(new JProperty(propertyInfo.Name, propertyInfo.GetValue(objectToJsonify)));
                }
            }
            catch (Exception)
            {
                return null;
            }
            return properties;
        }

        public static IEnumerable<JProperty> CreateJsonPropertiesFromExpandoObject(ExpandoObject objectToJsonify)
        {
            ICollection<JProperty> properties = new List<JProperty>();

            var converter = new ExpandoObjectConverter();

            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(objectToJsonify.ToString(), converter);
            try
            {
                foreach (
                           var propertyInfo in objectToJsonify.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                           )
                {
                    
                    properties.Add(new JProperty(propertyInfo.Name, propertyInfo.GetValue(objectToJsonify)));
                }
            }
            catch (Exception)
            {
                properties.Add(obj);
            }
            return properties;

        }

        public static string AddObjectToJsonArray(string jsonAsString, string keyOfArray, Object objectToJsonify)
        {
            JObject existingStoredJsonObject;
            JArray jArray;
            var newJsonObject = new JObject();
            try
            {
                // convert object to a json object

                newJsonObject = new JObject(CreateJsonPropertiesFromObject(objectToJsonify));

                // if the existing stored string is null or emptiness, just create it and return.
                if (string.IsNullOrWhiteSpace(jsonAsString))
                {
                    existingStoredJsonObject = new JObject();
                    jArray = new JArray(newJsonObject);
                    existingStoredJsonObject.Add(keyOfArray, jArray);

                    return existingStoredJsonObject.ToString(Formatting.None);
                }
            }
            catch (Exception ex)
            {
                //is the incoming a flat string
                newJsonObject["msg"] = objectToJsonify.ToString();

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
                    existing = "wrapped Json by AddObjectToJsonArray =" + jsonAsString,
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
