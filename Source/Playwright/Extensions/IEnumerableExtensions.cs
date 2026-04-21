using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ExecuteFuncs<T>(this IEnumerable<Func<T>> funcs)
        {
            foreach (var func in funcs)
            {
                yield return func();
            }
        }
    }
}
