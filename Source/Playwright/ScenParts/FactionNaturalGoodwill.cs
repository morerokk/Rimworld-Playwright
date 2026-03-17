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
    public class FactionNaturalGoodwill : FactionGoodwill
    {
        // Min/max for this is 200, because natural goodwill ranges can exceed 100 for calculations
        protected override int MinGoodwill => -200;
        protected override int MaxGoodwill => 200;

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionNaturalGoodwill.Summary".Translate(FactionToAffect.label, Goodwill);
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionNaturalGoodwill.Help".Translate()));
        }
    }
}
