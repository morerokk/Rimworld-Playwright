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

        public override string Summary(Scenario scen)
        {
            return "No enemies! Placeholder";
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.NoHostileFactionsExcept.Help".Translate()));
        }
    }
}
