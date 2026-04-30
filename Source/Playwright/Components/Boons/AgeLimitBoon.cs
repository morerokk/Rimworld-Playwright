using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class AgeLimitBoon : BoonComponent
    {
        public override string Id => "Boons.AgeLimit";

        public IntRange AllowedAgeRange = new IntRange(15, 120);

        protected override void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            boonContentListing.Label("Playwright.Components.Boons.AgeLimit.AllowedAgeRange".Translate());
            boonContentListing.IntRange(ref AllowedAgeRange, 15, 120);
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtils.MakePawnFilterAgePart(AllowedAgeRange));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AllowedAgeRange, nameof(AllowedAgeRange), new IntRange(15, 120));
        }
    }
}
