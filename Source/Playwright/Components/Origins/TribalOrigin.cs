using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class TribalOrigin : OriginComponent
    {
        public override string Id => "Origins.Tribal";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.LostTribe;
    }
}
