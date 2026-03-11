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
        public FactionDef Faction;

        public float NaturalGoodwill = 0;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 4);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 4);

            NaturalGoodwill = Widgets.HorizontalSlider(helper.NextRect(), NaturalGoodwill, -100, 100, true, "Playwright.Scenparts.FactionNaturalGoodwill.NaturalGoodwill", null, null, 1f);

            helper.Skip(1);
            //Widgets.IntRange(helper.NextRect(), ScenPartDrawHelper.Ids.NaturalGoodwill, GoodwillRange,)

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionNaturalGoodwill.Help".Translate()));
            }
        }

        public override void PostGameStart()
        {
            base.PostGameStart();

            // TODO: Actually affect faction goodwill
        }

        // Boilerplate seemingly needed by scenarios to save/load them and stuff?
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look<FactionDef>(ref Faction, nameof(Faction));
            Scribe_Values.Look<float>(ref NaturalGoodwill, nameof(NaturalGoodwill), 0f, false);
            
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionNaturalGoodwill.Summary".Translate(Faction.label, NaturalGoodwill);
        }

        // Make the randomize button work
        public override void Randomize()
        {
            
        }
    }
}
