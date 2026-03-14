using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.WinConditions
{
    public class RoyalAscentWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.RoyalAscent";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // If disabled, the Royal Ascent quest will not be offered.
        }
    }
}
