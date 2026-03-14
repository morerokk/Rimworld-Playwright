using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Utilities;
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

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeStartWithHonorPart(DefOfs.FactionDefOf.MF_Corporation, 7, true));
        }
    }
}
