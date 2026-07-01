using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Extensions
{
    public static class EnumExtensions
    {
        public static string Translate(this ScatterType scatterType)
        {
            return ("Playwright.ScatterType." + scatterType.ToString()).Translate();
        }
    }
}
