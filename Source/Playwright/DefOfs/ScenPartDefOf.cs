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
        public static ScenPartDef Playwright_AdditionalUnwaveringChance;

        // Built-in scenparts that the game does not expose
        public static ScenPartDef Naked;
        public static ScenPartDef NoPossessions;
        public static ScenPartDef ForcedHediff;
        public static ScenPartDef StartingThing_Defined;
        public static ScenPartDef StartingAnimal;
        public static ScenPartDef ScatterThingsNearPlayerStart;
        public static ScenPartDef ScatterThingsAnywhere;
        public static ScenPartDef GameStartDialog;
    }
}
