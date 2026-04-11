using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    // Handled in the composer: if disabled, the Royal Ascent quest will not be offered.
    public class RoyalAscentWinCondition : WinConditionComponent
    {
        public const string ComponentId = "WinConditions.RoyalAscent";
        public override string Id => ComponentId;
        public override bool IsAvailable => ModsConfig.RoyaltyActive;
    }
}
