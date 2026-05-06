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
        public static IEnumerable<string> UnremovableFactionDefNames => UnremovableFactionDefs.AsReadOnly();
        public static void RegisterUnremovableFaction(string factionDefName)
        {
            UnremovableFactionDefs.Add(factionDefName);
        }
        public static void RegisterUnremovableFaction(FactionDef factionDef)
        {
            RegisterUnremovableFaction(factionDef.defName);
        }
    }
}
