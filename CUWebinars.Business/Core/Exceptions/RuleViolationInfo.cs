using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class RuleViolationInfo
    {
        public RuleViolationInfo()
            :this(String.Empty, String.Empty)
        {
        }

        public RuleViolationInfo(string propertyName, string errorDescription)
        {
            PropertyName = propertyName;
            ErrorDescription = errorDescription;
        }

        public string PropertyName { get; set; }
        public string ErrorDescription { get; set; }
    }
}
