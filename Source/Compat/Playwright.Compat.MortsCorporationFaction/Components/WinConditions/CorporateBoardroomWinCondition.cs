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
    public class CorporateBoardroomWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Compat_MF_CorporateBoardroom";

        public int Colonists = 10;
        private string ColonistsBuffer = "10";

        public RoyalTitleDef Title = DefOfs.RoyalTitleDefOf.MF_RegionalExecutive;
        private string TitleText => Title != null ? Title.LabelCap.ToString() : "-";

        public override void DoSettingsContents(Listing_AutoFitVertical winConditionContentListing)
        {
            base.DoSettingsContents(winConditionContentListing);
            // Colonist count selector
            winConditionContentListing.Label("Playwright.Components.WinConditions.Compat_MF_CorporateBoardroom.Colonists".Translate());
            winConditionContentListing.IntEntry(ref Colonists, ref ColonistsBuffer, min: 1);
            // Title selector
            if (Title == null)
            {
                Title = DefOfs.RoyalTitleDefOf.MF_RegionalExecutive;
            }
            if (winConditionContentListing.ButtonTextLabeled("Playwright.Components.WinConditions.Compat_MF_CorporateBoardroom.Title".Translate(), TitleText))
            {
                List<RoyalTitleDef> allowedTitles = DefOfs.FactionDefOf.MF_Corporation.RoyalTitlesAwardableInSeniorityOrderForReading;
                var floatMenuOptions = new List<FloatMenuOption>();
                foreach (RoyalTitleDef titleDef in allowedTitles)
                {
                    floatMenuOptions.Add(new FloatMenuOption(titleDef.LabelCap, () => Title = titleDef));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            base.MutateScenario(scenario, scenarioParts);
            scenarioParts.Add(ScenPartUtility.MakeWinConditionRoyalTitlesPart(Colonists, DefOfs.FactionDefOf.MF_Corporation, Title));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Colonists, nameof(Colonists), 10);
            Scribe_Defs.Look(ref Title, nameof(Title));
        }
    }
}
