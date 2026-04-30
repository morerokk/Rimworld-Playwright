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
    public class ConquestWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Conquest";

        public bool AllowAllies = true;

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            base.DoSettingsContents(winConditionContentListing);
            winConditionContentListing.CheckboxLabeled("Playwright.Components.WinConditions.Conquest.AllowAllies".Translate(), ref AllowAllies, "Playwright.Components.WinConditions.Conquest.AllowAllies.Help".Translate());
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtils.MakeWinConditionConquestPart(AllowAllies));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AllowAllies, nameof(AllowAllies), true);
        }
    }
}
