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
    public class CorporateOrigin : OriginComponent
    {
        public override string Id => "Origins.Corporate";
        public static bool IsAvailable => ModsConfig.RoyaltyActive && ModsConfig.IsActive("MortStrudel.CorporationFaction");
        public int StartWithHonor { get; set; } = 7;

        public override void MutateScenario(List<ScenPart> scenarioParts)
        {
            ScenPartDef startWithHonorDef = Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_StartWithHonor;
            StartWithHonor part = (StartWithHonor)ScenarioMaker.MakeScenPart(startWithHonorDef);
            part.StartingHonor = StartWithHonor;
            // TODO: Set part.FactionToStartWithHonorFor to Mort's Corporation faction
            scenarioParts.Add(part);
        }
    }
}
