using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class CrashlandedOrigin : OriginComponent
    {
        public override string Id => "Origins.Crashlanded";
        public override ScenarioDef BasedOnScenario => RimWorld.ScenarioDefOf.Crashlanded;
    }
}
