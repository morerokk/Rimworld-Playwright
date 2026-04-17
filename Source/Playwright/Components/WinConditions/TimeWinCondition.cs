using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    public class TimeWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Time";

        public int Days = 600;
        private string DaysBuffer = "600";

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            // Days input
            winConditionContentListing.Label("Playwright.Components.WinConditions.Time.Days".Translate());
            winConditionContentListing.IntEntry(ref Days, ref DaysBuffer, 1, 1);
            // Years/days summary
            int years = Days / 60;
            int days = Days % 60;
            winConditionContentListing.Label("Playwright.Components.WinConditions.Time.YearsAndDays".Translate(years, days));
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtility.MakeWinConditionTimePart(Days));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Days, nameof(Days), 600);
            DaysBuffer = Days.ToString();
        }
    }
}
