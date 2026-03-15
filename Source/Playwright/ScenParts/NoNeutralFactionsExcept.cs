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
    public class NoNeutralFactionsExcept : NoFactionsExcept
    {
        public override FactionRelationKind RelationKind => FactionRelationKind.Neutral;

        protected override string SummaryTag => "Playwright_NoNeutralFactionsExcept";
        protected override string SummaryIntro => "Playwright.ScenParts.NoNeutralFactionsExcept.SummaryIntro".Translate();
        protected override string SummaryNoIntro => "Playwright.ScenParts.NoNeutralFactionsExcept.Summary".Translate();
        protected override string HelpText => "Playwright.ScenParts.NoNeutralFactionsExcept.Help".Translate();
    }
}
