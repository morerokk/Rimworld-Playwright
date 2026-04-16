using RimWorld;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class UnwaveringLoyaltySpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.UnwaveringLoyalty";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeAdditionalUnwaveringChancePart(1f));
        }
    }
}
