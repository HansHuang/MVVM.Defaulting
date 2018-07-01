using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// provide foreach to IEnumerable. This is not a thread-safe operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="f"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> f)
        {
            foreach (var item in list)
            {
                f(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in list)
            {
                action(item, i);
                i++;
            }
        }

        /// <summary>
        /// Deal with the scenario of the key may be duplicated
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement> ToDictionarySafety<TSource, TKey, TElement>(this IEnumerable<TSource> source,
                Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var dict = new Dictionary<TKey, TElement>();
            foreach (var e in source)
            {
                var key = keySelector(e);
                var value = elementSelector(e);
                dict[key] = value;
            }

            return dict;
        }

        public static bool IsEmpty(this IEnumerable list)
        {
            if (list == null) return true;

            var enumerator = list.GetEnumerator();
            return !enumerator.MoveNext();
        }


    }
}
