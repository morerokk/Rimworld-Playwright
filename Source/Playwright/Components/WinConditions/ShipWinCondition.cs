using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.WinConditions
{
    public class ShipWinCondition : WinConditionComponent
    {
        public override string Id => "WinConditions.Ship";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Any win condition that results getting off the planet on a built ship. Building a ship or finding the Journey Offer.
            // If disabled, the startup sequence for your own ship cannot be initiated, and the Journey Offer will not be, uh, offered.
            // DOES NOT disable the royal ascent quest if removed!
        }
    }
}
