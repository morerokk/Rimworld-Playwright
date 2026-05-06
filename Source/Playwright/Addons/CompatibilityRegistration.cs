using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Addons
{
    /// <summary>
    /// Helper class to register mod compatibility stuff (such as factions that should not be removed by the faction-related scenparts)
    /// </summary>
    public static class CompatibilityRegistration
    {
        private static List<string> UnremovableFactionDefs = new List<string>();
        /// <summary>
        /// Factions that Playwright's ScenParts will refuse to automatically deselect in the faction screen, pre-world generation.
        /// </summary>
        public static IEnumerable<string> UnremovableFactionDefNames => UnremovableFactionDefs.AsReadOnly();
        /// <summary>
        /// Register a faction as being "unremovable", meaning that they cannot be deselected by the player and therefore also not by Playwright's scenparts.
        /// </summary>
        /// <remarks>
        /// This doesn't make the faction actually non-deselectable in the faction screen, this just notifies Playwright that a faction is non-deselectable.
        /// Primarily used by some factions that use Vanilla Expanded Framework,
        /// like VFE Empire's Deserters, which are unremovable if you have VFE Deserters installed.
        /// </remarks>
        /// <param name="factionDefName">The name of the faction def to mark unremovable.</param>
        public static void RegisterUnremovableFaction(string factionDefName)
        {
            UnremovableFactionDefs.Add(factionDefName);
        }
    }
}
