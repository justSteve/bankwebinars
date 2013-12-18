using System;
using System.Collections.Generic;
using System.Text;


namespace CUWebinars.Business.Core.Exceptions
{
    public class BusinessRuleGroupException: BusinessRuleException
    {
        public BusinessRuleGroupException() :
            this(new List<RuleViolationInfo>())
        {
        }

        public BusinessRuleGroupException(IList<RuleViolationInfo> violationList) :
            base()
        {
            mViolationsList = violationList;
        }

        public virtual void AddViolation(RuleViolationInfo violation)
        {
            mViolationsList.Add(violation);
        }

        private IList<RuleViolationInfo> mViolationsList;

        public override string Message
        {
            get
            {
                StringBuilder message = new StringBuilder();
                string separator = String.Empty;
                foreach (RuleViolationInfo violation in mViolationsList)
                {
                    message.Append(separator +  "Invalid " + violation.PropertyName + ": " + violation.ErrorDescription);

                    separator = Environment.NewLine;
                }

                return message.ToString();
            }
        }

        public virtual IList<RuleViolationInfo> ViolationList
        {
            get
            {
                return mViolationsList;
            }
        }
    }
}
