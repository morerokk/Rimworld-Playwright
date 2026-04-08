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
        [MayRequireRoyalty]
        public static ScenPartDef Playwright_ForcedPsylinkLevel;

        public static ScenPartDef Playwright_StartWithNonMinifiedThing;
        public static ScenPartDef Playwright_FactionNaturalGoodwill;
        public static ScenPartDef Playwright_FactionForcedGoodwill;
        public static ScenPartDef Playwright_FactionStartingGoodwill;
        public static ScenPartDef Playwright_AdditionalUnwaveringChance;
        public static ScenPartDef Playwright_NoNeutralFactionsExcept;
        public static ScenPartDef Playwright_NoHostileFactionsExcept;
        public static ScenPartDef Playwright_RemoveFaction;
        public static ScenPartDef Playwright_DisableShipStartup;
        public static ScenPartDef Playwright_ForcedImplant;
        public static ScenPartDef Playwright_ReplaceXenotype;
        public static ScenPartDef Playwright_ReplaceXenotypeWithCustom;

        // Win conditions
        public static ScenPartDef Playwright_WinCondition_Colony;
        public static ScenPartDef Playwright_WinCondition_Conquest;
        [MayRequireRoyalty]
        public static ScenPartDef Playwright_WinCondition_RoyalTitles;
        public static ScenPartDef Playwright_WinCondition_Time;

        // Things that are in the base game but have not been added to a DefOf
        public static ScenPartDef Naked;
        public static ScenPartDef NoPossessions;
        public static ScenPartDef ForcedHediff;
        public static ScenPartDef StartingThing_Defined;
        public static ScenPartDef StartingAnimal;
        public static ScenPartDef ScatterThingsNearPlayerStart;
        public static ScenPartDef ScatterThingsAnywhere;
        public static ScenPartDef GameStartDialog;
        public static ScenPartDef GameCondition_Planetkiller;
        public static ScenPartDef DisableIncident;
    }
}
