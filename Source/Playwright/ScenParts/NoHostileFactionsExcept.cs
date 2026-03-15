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
    public class NoHostileFactionsExcept : NoFactionsExcept
    {
        public override FactionRelationKind RelationKind => FactionRelationKind.Hostile;

        protected override string SummaryTag => "Playwright_NoHostileFactionsExcept";
        protected override string SummaryNoIntro => "Playwright.ScenParts.NoHostileFactionsExcept.Summary".Translate();
        protected override string SummaryIntro => "Playwright.ScenParts.NoHostileFactionsExcept.SummaryIntro".Translate();
        protected override string HelpText => "Playwright.ScenParts.NoHostileFactionsExcept.Help".Translate();
    }
}
