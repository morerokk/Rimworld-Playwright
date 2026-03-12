using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class CrashlandedOrigin : OriginComponent
    {
        public override string Id => "Origins.Crashlanded";

        public override void MutateScenario(Scenario scenario,List<ScenPart> scenarioParts)
        {
            // TODO: Add parts for spawning with supplies, starting weapons and stuff for 3 people
        }
    }
}
