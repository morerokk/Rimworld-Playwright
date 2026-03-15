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
    public class NoAllyFactionsExcept : NoFactionsExcept
    {
        public override FactionRelationKind RelationKind => FactionRelationKind.Ally;

        protected override string SummaryTag => "Playwright_NoAllyFactionsExcept";
        protected override string SummaryIntro => "Playwright.ScenParts.NoAllyFactionsExcept.SummaryIntro".Translate();
        protected override string SummaryNoIntro => "Playwright.ScenParts.NoAllyFactionsExcept.Summary".Translate();
        protected override string HelpText => "Playwright.ScenParts.NoAllyFactionsExcept.Help".Translate();
    }
}
