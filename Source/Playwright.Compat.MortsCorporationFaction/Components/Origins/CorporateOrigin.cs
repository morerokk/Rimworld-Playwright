using RimWorld;
using Rokk.Playwright.Compat.MortsCorporationFaction.DefOfs;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins
{
    public class CorporateOrigin : OriginComponent
    {
        public override string Id => "Origins.Corporate";
        public int StartWithHonor { get; set; } = 7;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            ScenPartDef startWithHonorDef = Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_StartWithHonor;
            StartWithHonor part = (StartWithHonor)ScenarioMaker.MakeScenPart(startWithHonorDef);
            part.StartingHonor = StartWithHonor;
            part.FactionToStartWithHonorFor = Rokk.Playwright.Compat.MortsCorporationFaction.DefOfs.FactionDefOf.MF_Corporation;
            scenarioParts.Add(part);
        }
    }
}
