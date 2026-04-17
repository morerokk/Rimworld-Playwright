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
    public class ColonyWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Colony";

        public int Colonists = 30;
        private string ColonistsBuffer = "30";

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            base.DoSettingsContents(winConditionContentListing);
            winConditionContentListing.IntEntry(ref Colonists, ref ColonistsBuffer, min: 1);
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtility.MakeWinConditionColonyPart(Colonists));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Colonists, nameof(Colonists), 30);
            ColonistsBuffer = Colonists.ToString();
        }
    }
}
