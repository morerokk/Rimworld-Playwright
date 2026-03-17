using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.WinConditions
{
    // Placeholder since the default state is for this part to be present.
    // This represents building a ship or finding the Journey Offer.
    // If disabled, the startup sequence for your own ship cannot be initiated, and the Journey Offer will not be, uh, offered.
    // DOES NOT disable the royal ascent quest if removed!
    public class ShipWinCondition : WinConditionComponent
    {
        public const string ComponentId = "WinConditions.Ship";
        public override string Id => ComponentId;
    }
}
