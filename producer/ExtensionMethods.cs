using System;
using System.Collections.Generic;

namespace kafka_poc
{
    public static class ExtensionMethods
    {
        public static void NullSafeForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable ?? new T[] { }) action(item);
        }
    }
}
