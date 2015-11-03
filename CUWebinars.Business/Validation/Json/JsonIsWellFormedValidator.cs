using System;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CUWebinars.Business.Validation.Json
{
    public class JsonIsWellFormedValidator : AbstractValidator<ValidationString>
    {
        //private string _jsonString;
        public JsonIsWellFormedValidator()
        {
            RuleFor(jsonString => jsonString.StringValue).Must(ValueMustBeValidJson)
                .WithMessage("Value is not valid Json: {0}", jsonString => jsonString.StringValue);
        }

        private bool ValueMustBeValidJson(ValidationString json, string jsonString)
        {
            try
            {
                JObject.Parse(jsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
