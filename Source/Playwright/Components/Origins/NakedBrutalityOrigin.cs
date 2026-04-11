using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class NakedBrutalityOrigin : OriginComponent
    {
        public override string Id => "Origins.NakedBrutality";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.NakedBrutality;
    }
}
