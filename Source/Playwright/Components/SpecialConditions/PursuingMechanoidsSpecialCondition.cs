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
    public class PursuingMechanoidsSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.PursuingMechanoids";
        public override bool IsAvailable => ModsConfig.OdysseyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakePursuingMechanoidsPart());
        }
    }
}
