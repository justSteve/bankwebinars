using System.Collections.Generic;
using FluentValidation;
using System.Xml.Linq;
using FluentValidation.Results;

namespace CUWebinars.Business.Core.Helpers
{
    public class ValidationHelper
    {
        public static XElement GetMessagesAsXmlElement(IList<ValidationFailure> errors)
        {
            var element = new XElement("Errors");

            foreach (var error in errors)
            {
                element.Add(new XElement("Error", error.ErrorMessage));
            }

            return element;
        }
    }
}
