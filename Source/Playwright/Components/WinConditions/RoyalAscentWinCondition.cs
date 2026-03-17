using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    public class RoyalAscentWinCondition : WinConditionComponent
    {
        public const string ComponentId = "WinConditions.RoyalAscent";
        public override string Id => ComponentId;
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Handled in the composer: if disabled, the Royal Ascent quest will not be offered.
        }
    }
}
