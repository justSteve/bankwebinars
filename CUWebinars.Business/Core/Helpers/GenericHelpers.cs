using System;

namespace CUWebinars.Business.Core.Helpers
{
    public class GenericHelpers
    {
        public static string CleanGenericName(Type type)
        {
            var name = type.Name;
            var index = name.IndexOf('`');

            if (index > 0)
            {
                return name.Substring(0, index);
            }

            return name;
        }

    }
}
