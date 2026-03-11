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
    public class FactionNaturalGoodwill : ScenPart
    {
        public FactionDef FactionToAffect;

        public int NaturalGoodwill = 0;

        private string FactionToAffectLabelText => FactionToAffect != null ? FactionToAffect.LabelCap.ToString() : "-";

        public static List<FactionDef> GetAllowedFactions()
        {
            // Hidden factions not selectable. You probably don't want natural goodwill with hidden factions,
            // that's more of a Forced Goodwill thing.
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 6);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 6);

            // Faction selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.FactionNaturalGoodwill.Faction".Translate());
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

            // Natural goodwill slider
            NaturalGoodwill = Mathf.FloorToInt(Widgets.HorizontalSlider(helper.NextRect(), NaturalGoodwill, -200, 200, true, "Playwright.ScenParts.FactionNaturalGoodwill.NaturalGoodwill".Translate(), null, null, 1f));
            Widgets.Label(helper.NextRect(), NaturalGoodwill.ToString());

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionNaturalGoodwill.Help".Translate()));
            }
        }

        // Modify faction starting goodwill on world gen
        // This cannot affect natural goodwill, that's handled in patches
        public override void PostWorldGenerate()
        {
            List<Faction> factions = Find.FactionManager.AllFactions
                .Where(f => f.def == FactionToAffect)
                .ToList();

            foreach (var faction in factions)
            {
                int currentPlayerGoodwill = faction.GoodwillWith(Faction.OfPlayer);
                int differenceToReachTarget = Mathf.FloorToInt(NaturalGoodwill) - currentPlayerGoodwill;
                faction.TryAffectGoodwillWith(Faction.OfPlayer, differenceToReachTarget, false, false);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look<FactionDef>(ref FactionToAffect, nameof(FactionToAffect));
            Scribe_Values.Look<int>(ref NaturalGoodwill, nameof(NaturalGoodwill), 0, false);
            
        }

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionNaturalGoodwill.Summary".Translate(FactionToAffect.label, NaturalGoodwill);
        }
    }
}
