namespace CUWebinars.Business.Validation.Json
{
    public class ValidationString
    {
        public string StringValue { get; set; }

        public ValidationString(string stringValue)
        {
            StringValue = stringValue;
        }
    }
}