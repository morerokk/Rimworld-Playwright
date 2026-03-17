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
    public class DisableShipStartup : ScenPart
    {
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 2);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 2);

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.DisableShipStartup.Help".Translate()));
            }
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.DisableShipStartup.Summary".Translate();
        }
    }
}
