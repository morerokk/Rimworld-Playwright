using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class PlanetkillerSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.Planetkiller";

        public int Days = 360;
        private string DaysBuffer = "360";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeGameConditionPlanetkillerPart(Days));
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            // Days input
            specialConditionContentListing.Label("Playwright.Components.SpecialConditions.Planetkiller.Days".Translate());
            specialConditionContentListing.IntEntry(ref Days, ref DaysBuffer, 1, 1);
            // Years/days summary
            int years = Days / 60;
            int days = Days % 60;
            specialConditionContentListing.Label("Playwright.Components.SpecialConditions.Planetkiller.YearsAndDays".Translate(years, days));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Days, nameof(Days));
            DaysBuffer = Days.ToString();
        }
    }
}
