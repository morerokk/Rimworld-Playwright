using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class EmpireOrigin : OriginComponent
    {
        public override string Id => "Origins.Empire";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeStartWithHonorPart(FactionDefOf.Empire, 7, true));
            //scenarioParts.Add(ScenPartUtility.MakeForcedHediffPart(HediffDefOf.PsychicAmplifier))
        }
    }
}
