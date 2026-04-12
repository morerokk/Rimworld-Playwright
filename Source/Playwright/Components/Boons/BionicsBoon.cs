using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class BionicsBoon : BoonComponent
    {
        public override string Id => "Boons.Bionics";

        public bool ArmsEnabled = true;
        public bool LegsEnabled = false;

        public override void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {
            boonContentListing.CheckboxLabeled("Playwright.Components.Boons.Bionics.Arms".Translate(), ref ArmsEnabled, labelPct: 0.2f);
            boonContentListing.CheckboxLabeled("Playwright.Components.Boons.Bionics.Legs".Translate(), ref LegsEnabled, labelPct: 0.2f);
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            if (ArmsEnabled)
            {
                scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(DefOfs.HediffDefOf.BionicArm, 1f, PawnGenerationContext.PlayerStarter, BodyPartDefOf.Arm, ForcedImplant.ImplantSide.Left));
                scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(DefOfs.HediffDefOf.BionicArm, 1f, PawnGenerationContext.PlayerStarter, BodyPartDefOf.Arm, ForcedImplant.ImplantSide.Right));
            }
            if (LegsEnabled)
            {
                scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(DefOfs.HediffDefOf.BionicLeg, 1f, PawnGenerationContext.PlayerStarter, BodyPartDefOf.Leg, ForcedImplant.ImplantSide.Left));
                scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(DefOfs.HediffDefOf.BionicLeg, 1f, PawnGenerationContext.PlayerStarter, BodyPartDefOf.Leg, ForcedImplant.ImplantSide.Right));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ArmsEnabled, nameof(ArmsEnabled), true);
            Scribe_Values.Look(ref LegsEnabled, nameof(LegsEnabled), false);
        }
    }
}
