using System;
using System.Collections.Generic;

namespace ManagedToNativeWrapperGenerator
{
    public static class IEnumerableExtension
    {

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

    }
}
