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
        // ID's must be unique, so prefix it with something distinct
        public override string Id => "Origins.Compat_MF_Corporate";
        // Only available if Royalty is also installed. This is redundant for this particular mod but serves as an example.
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Same thing as the base Empire origin, but with The Corporation from Mort's Factions - The Corporation instead.
            scenarioParts.Add(ScenPartUtility.MakeStartWithHonorPart(DefOfs.FactionDefOf.MF_Corporation, 7, true));
        }
    }
}
