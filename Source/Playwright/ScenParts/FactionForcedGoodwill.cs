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
    public class FactionForcedGoodwill : FactionGoodwill
    {
        protected override List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer)
                .ToList();
        }

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.FactionForcedGoodwill.Summary".Translate(FactionToAffect.label, Goodwill);
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.FactionForcedGoodwill.Help".Translate()));
        }
    }
}
