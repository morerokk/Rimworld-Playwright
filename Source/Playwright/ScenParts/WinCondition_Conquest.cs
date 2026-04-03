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
    public class WinCondition_Conquest : ScenPart_WinCondition
    {
        public bool AllowAllies = true;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 3);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 3);

            helper.Skip(1);

            Widgets.CheckboxLabeled(helper.NextRect(), "Playwright.ScenParts.WinCondition_Conquest.AllowAllies".Translate(), ref AllowAllies);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                if (AllowAllies)
                {
                    yield return "Playwright.ScenParts.WinCondition_Conquest.Summary".Translate();
                }
                else
                {
                    yield return "Playwright.ScenParts.WinCondition_Conquest.SummaryNoAllies".Translate();
                }
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_Conquest.Help".Translate()));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AllowAllies, nameof(AllowAllies), true);
        }
    }
}
