using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.DefOfs
{
    [DefOf]
    public static class ScenarioDefOf
    {
        [MayRequireRoyalty]
        public static ScenarioDef Playwright_Empire;

        // Things that are in the base game but have not been added to a DefOf
        public static ScenarioDef LostTribe;
        public static ScenarioDef TheRichExplorer;
        public static ScenarioDef NakedBrutality;

        [MayRequireBiotech]
        public static ScenarioDef Mechanitor;
        [MayRequireBiotech]
        public static ScenarioDef Sanguophage;

        [MayRequireAnomaly]
        public static ScenarioDef TheAnomaly;

        [MayRequireOdyssey]
        public static ScenarioDef TheGravship;
    }
}
