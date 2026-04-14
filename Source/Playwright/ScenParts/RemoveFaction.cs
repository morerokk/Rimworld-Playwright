using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public class RemoveFaction : ScenPart
    {
        public FactionDef Faction;

        protected string FactionLabelText => Faction != null ? Faction.LabelCap.ToString() : "-";

        public string SummaryTag => "Playwright_RemoveFaction";

        protected virtual List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 3);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 3);

            // Faction selector
            if (Widgets.ButtonText(helper.NextRect(), FactionLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => Faction = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
            }

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.RemoveFaction.Help".Translate()));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
        }

        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, SummaryTag, "Playwright.ScenParts.RemoveFaction.SummaryIntro".Translate());
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag && Faction != null)
            {
                yield return Faction.LabelCap.CapitalizeFirst();
            }
        }
    }
}
