using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.DefOfs
{
    [DefOf]
    public static class ScenPartDefOf
    {
        [MayRequireRoyalty]
        public static ScenPartDef Playwright_StartWithHonor;

        public static ScenPartDef Playwright_StartWithNonMinifiedThing;
        public static ScenPartDef Playwright_FactionNaturalGoodwill;
        public static ScenPartDef Playwright_FactionForcedGoodwill;
        public static ScenPartDef Playwright_FactionStartingGoodwill;
    }
}
