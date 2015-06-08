using System;
using System.Collections.Generic;

namespace CppCliBridgeGenerator
{
    public static class IEnumerableExtension
    {

        /// <summary>
        /// ForEach extension for IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of parameter.</typeparam>
        /// <param name="enumeration">Object implementing IEnumerable</param>
        /// <param name="action">Action to be called on every item.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

    }
}
