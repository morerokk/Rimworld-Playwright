using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    public class RoyalCouncilWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.RoyalCouncil";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public int Colonists = 10;
        private string ColonistsBuffer = "10";

        public RoyalTitleDef Title = RoyalTitleDefOf.Count;
        private string TitleText => Title != null ? Title.LabelCap.ToString() : "-";

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            base.DoSettingsContents(winConditionContentListing);
            // Colonist count selector
            winConditionContentListing.Label("Playwright.Components.WinConditions.RoyalCouncil.Colonists".Translate());
            winConditionContentListing.IntEntry(ref Colonists, ref ColonistsBuffer, min: 1);
            // Title selector
            if (winConditionContentListing.ButtonTextLabeled("Playwright.Components.WinConditions.RoyalCouncil.Title".Translate(), TitleText))
            {
                List<RoyalTitleDef> allowedTitles = FactionDefOf.Empire.RoyalTitlesAwardableInSeniorityOrderForReading;
                var floatMenuOptions = new List<FloatMenuOption>();
                foreach (RoyalTitleDef titleDef in allowedTitles)
                {
                    floatMenuOptions.Add(new FloatMenuOption(titleDef.LabelCap, () => Title = titleDef));
                }
                PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtility.MakeWinConditionRoyalTitlesPart(Colonists, FactionDefOf.Empire, Title));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Colonists, nameof(Colonists), 10);
            Scribe_Defs.Look(ref Title, nameof(Title));
        }
    }
}
