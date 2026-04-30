using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Compat.Endling.Components.Boons
{
    public class EndlingsBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_Endling";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtils.MakeForcedTraitPart(DefOfs.TraitDefOf.Aelanna_Endling, 0, PawnGenerationContext.PlayerStarter, 1f));
        }
    }
}
