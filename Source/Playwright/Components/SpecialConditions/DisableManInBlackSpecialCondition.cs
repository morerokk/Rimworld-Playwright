using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class DisableManInBlackSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.DisableManInBlack";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeDisableIncidentPart(DefOfs.IncidentDefOf.StrangerInBlackJoin));
        }
    }
}
