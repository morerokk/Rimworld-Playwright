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
    public class EmpireOrigin : OriginComponent
    {
        public override string Id => "Origins.Empire";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;
        public int StartWithHonor { get; set; } = 7;

        public override void MutateScenario(Scenario scenario,List<ScenPart> scenarioParts)
        {
            ScenPartDef startWithHonorDef = Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_StartWithHonor;
            StartWithHonor part = (StartWithHonor)ScenarioMaker.MakeScenPart(startWithHonorDef);
            part.StartingHonor = StartWithHonor;
            part.FactionToStartWithHonorFor = FactionDefOf.Empire;
            scenarioParts.Add(part);
        }
    }
}
