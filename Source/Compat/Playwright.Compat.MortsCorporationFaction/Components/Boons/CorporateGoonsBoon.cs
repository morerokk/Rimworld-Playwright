using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction.Components.Boons
{
    public class CorporateGoonsBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_MF_Corporate_Goons";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeStartWithHonorPart(DefOfs.FactionDefOf.MF_Corporation, 7, 1f, PawnGenerationContext.PlayerStarter, true));
        }
    }
}
