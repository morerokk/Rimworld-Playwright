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
    public class FactionForcedGoodwill : ScenPart
    {
        public FactionDef FactionToAffect;

        public int ForcedGoodwill = 0;

        private string FactionToAffectLabelText => FactionToAffect != null ? FactionToAffect.LabelCap.ToString() : "-";

        public static List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 6);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 6);

            // Faction selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.FactionForcedGoodwill.Faction".Translate());
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
            ForcedGoodwill = Mathf.FloorToInt(Widgets.HorizontalSlider(helper.NextRect(), ForcedGoodwill, -100, 100, true, "Playwright.ScenParts.FactionForcedGoodwill.ForcedGoodwill".Translate(), null, null, 1f));
            Widgets.Label(helper.NextRect(), ForcedGoodwill.ToString());

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionForcedGoodwill.Help".Translate()));
            }
        }

        // Modify faction starting goodwill on world gen
        public override void PostWorldGenerate()
        {
            List<Faction> factions = Find.FactionManager.AllFactions
                .Where(f => f.def == FactionToAffect)
                .ToList();

            foreach (var faction in factions)
            {
                int currentPlayerGoodwill = faction.GoodwillWith(Faction.OfPlayer);
                int differenceToReachTarget = Mathf.FloorToInt(ForcedGoodwill) - currentPlayerGoodwill;
                faction.TryAffectGoodwillWith(Faction.OfPlayer, differenceToReachTarget, false, false);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look<FactionDef>(ref FactionToAffect, nameof(FactionToAffect));
            Scribe_Values.Look<int>(ref ForcedGoodwill, nameof(ForcedGoodwill), 0, false);
            
        }

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionForcedGoodwill.Summary".Translate(FactionToAffect.label, ForcedGoodwill);
        }
    }
}
