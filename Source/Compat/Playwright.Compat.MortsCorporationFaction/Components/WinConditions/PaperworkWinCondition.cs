using RimWorld;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction.Components.WinConditions
{
    public class PaperworkWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Compat_MF_Corporate_Paperwork";

        public int Amount = 5000;
        private string AmountBuffer = "5000";

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            base.DoSettingsContents(winConditionContentListing);
            // Paperwork count selector
            winConditionContentListing.Label("Playwright.Components.WinConditions.Compat_MF_Corporate_Paperwork.Amount".Translate());
            winConditionContentListing.IntEntry(ref Amount, ref AmountBuffer, min: 1);
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtility.MakeWinConditionSellItemsPart(DefOfs.ThingDefOf.MF_CorporateCompletedPaperwork, Amount));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Amount, nameof(Amount), 5000);
        }
    }
}
