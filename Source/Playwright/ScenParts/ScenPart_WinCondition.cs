using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public abstract class ScenPart_WinCondition : ScenPart
    {
        // Summarize all win conditions under 1 summary heading
        public string SummaryTag => "Playwright_WinCondition";
        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, SummaryTag, "Playwright.ScenParts.WinCondition.SummaryIntro".Translate());
        }
        // Require the win condition to specify its summary list entry
        public abstract override IEnumerable<string> GetSummaryListEntries(string tag);

        protected abstract void DoHelpButton();
        
    }
}
