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
    // Very similar to FactionNaturalGoodwill, but ONLY affects the post-world generation starting goodwill and nothing else
    // Natural goodwill is untouched
    public class FactionStartingGoodwill : ScenPart
    {
        public FactionDef FactionToAffect;

        public int StartingGoodwill = 0;

        private string FactionToAffectLabelText => FactionToAffect != null ? FactionToAffect.LabelCap.ToString() : "-";

        public static List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 6);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 6);

            // Faction selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.FactionStartingGoodwill.Faction".Translate());
            if (Widgets.ButtonText(helper.NextRect(), FactionToAffectLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => FactionToAffect = factionDef));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            helper.Skip(1);

            // Goodwill slider
            StartingGoodwill = Mathf.FloorToInt(Widgets.HorizontalSlider(helper.NextRect(), StartingGoodwill, -200, 200, true, "Playwright.ScenParts.FactionStartingGoodwill.StartingGoodwill".Translate(), null, null, 1f));
            Widgets.Label(helper.NextRect(), StartingGoodwill.ToString());

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionStartingGoodwill.Help".Translate()));
            }
        }

        // Modify faction starting goodwill on world gen
        public override void PostWorldGenerate()
        {
            List<Faction> factions = Find.FactionManager.AllFactionsListForReading
                .Where(f => f.def == FactionToAffect)
                .ToList();

            foreach (var faction in factions)
            {
                int currentPlayerGoodwill = faction.GoodwillWith(Faction.OfPlayer);
                int differenceToReachTarget = Mathf.FloorToInt(StartingGoodwill) - currentPlayerGoodwill;
                faction.TryAffectGoodwillWith(Faction.OfPlayer, differenceToReachTarget, false, false);
            }
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look<FactionDef>(ref FactionToAffect, nameof(FactionToAffect));
            Scribe_Values.Look<int>(ref StartingGoodwill, nameof(StartingGoodwill), 0, false);

            base.ExposeData();
        }

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionStartingGoodwill.Summary".Translate(FactionToAffect.label, StartingGoodwill);
        }
    }
}
