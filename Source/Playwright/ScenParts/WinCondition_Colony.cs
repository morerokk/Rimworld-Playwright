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
    public class WinCondition_Colony : ScenPart_WinCondition
    {
        public int Colonists = 30;
        private string ColonistsBuffer = "30";

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 3);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 3);

            Widgets.IntEntry(helper.NextRect(), ref Colonists, ref ColonistsBuffer);
            if (Colonists < 1)
            {
                Colonists = 1;
                ColonistsBuffer = "1";
            }

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                yield return "Playwright.ScenParts.WinCondition_Colony.Summary".Translate(Colonists);
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_Colony.Help".Translate()));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref Colonists, nameof(Colonists));
        }
    }
}
