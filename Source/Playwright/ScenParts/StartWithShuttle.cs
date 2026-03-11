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
    public class StartWithShuttle : ScenPart
    {
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.StartWithShuttle.Help".Translate()));
            }
        }

        public override void PostGameStart()
        {
            base.PostGameStart();

            // TODO: Actually start with shuttle (this is apparently impossible in vanilla without spawning one with dev mode)
        }
    }
}
