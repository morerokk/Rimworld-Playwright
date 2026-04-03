using RimWorld;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public class WinCondition_RoyalTitles : ScenPart_WinCondition
    {
        public int Colonists = 10;
        private string ColonistsBuffer = "10";
        public FactionDef Faction;
        public RoyalTitleDef Title;

        private string FactionLabelText => Faction != null ? Faction.LabelCap.ToString() : "-";
        private string TitleLabelText => Title != null ? Title.LabelCap.ToString() : "-";

        public List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden && def.HasRoyalTitles)
                .ToList();
        }

        public List<RoyalTitleDef> GetAllowedTitles()
        {
            if (Faction == null)
            {
                return new List<RoyalTitleDef>();
            }

            return Faction.RoyalTitlesAwardableInSeniorityOrderForReading;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 9);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 9);

            helper.Skip(1);

            // Amount of colonists
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.WinCondition_RoyalTitles.Colonists".Translate());
            Widgets.IntEntry(helper.NextRect(), ref Colonists, ref ColonistsBuffer);
            if (Colonists < 1)
            {
                Colonists = 1;
                ColonistsBuffer = "1";
            }

            // Faction
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.WinCondition_RoyalTitles.Faction".Translate());
            if (Widgets.ButtonText(helper.NextRect(), FactionLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => Faction = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            // Title
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.WinCondition_RoyalTitles.Title".Translate());
            if (Widgets.ButtonText(helper.NextRect(), TitleLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<RoyalTitleDef> allowedTitles = GetAllowedTitles();
                foreach (RoyalTitleDef titleDef in allowedTitles)
                {
                    floatMenuOptions.Add(new FloatMenuOption(titleDef.LabelCap, () => Title = titleDef));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                yield return "Playwright.ScenParts.WinCondition_RoyalTitles.Summary".Translate(Colonists, TitleLabelText, FactionLabelText);
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_RoyalTitles.Help".Translate()));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref Colonists, nameof(Colonists));
            Scribe_Defs.Look(ref Faction, nameof(Faction));
            Scribe_Defs.Look(ref Title, nameof(Title));
        }
    }
}
