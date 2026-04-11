using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class NovicesBoon : BoonComponent
    {
        public override string Id => "Boons.Novices";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeStartWithHonorPart(FactionDefOf.Empire, 7, 1f, PawnGenerationContext.PlayerStarter, true));
            scenarioParts.Add(ScenPartUtility.MakeForcedPsylinkLevelPart(new FloatRange(1f, 1f), 1f, PawnGenerationContext.PlayerStarter));
        }
    }
}
