using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.WinConditions
{
    // Handled in the composer: if disabled, the Archonexus quests will not be offered.
    public class ArchonexusWinCondition : WinConditionComponent
    {
        public const string ComponentId = "WinConditions.Archonexus";
        public override string Id => ComponentId;
        public override bool IsAvailable => ModsConfig.IdeologyActive;
    }
}
