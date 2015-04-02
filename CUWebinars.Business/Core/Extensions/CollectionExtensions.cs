using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Taken from the WebformsMVP project - http://webformsmvp.com/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
                target.Add(item);
        }
    }
}
