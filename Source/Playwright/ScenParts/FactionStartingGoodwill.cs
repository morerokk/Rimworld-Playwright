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
    public class FactionStartingGoodwill : FactionGoodwill
    {
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionStartingGoodwill.Summary".Translate(FactionToAffect.label, Goodwill);
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionStartingGoodwill.Help".Translate()));
        }
    }
}
