using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rokk.Playwright.Addons
{
    /// <summary>
    /// Helper class that can be used to register components from mod compatibility layers, or from addons.
    /// </summary>
    public static class ComponentRegistration
    {
        private static List<OriginComponent> RegisteredOrigins = new List<OriginComponent>();
        private static List<BoonComponent> RegisteredBoons = new List<BoonComponent>();
        private static List<FactionComponent> RegisteredFactions = new List<FactionComponent>();
        private static List<WinConditionComponent> RegisteredWinConditions = new List<WinConditionComponent>();
        private static List<SpecialConditionComponent> RegisteredSpecialConditions = new List<SpecialConditionComponent>();

        public static IEnumerable<OriginComponent> Origins => RegisteredOrigins.AsReadOnly();
        public static IEnumerable<BoonComponent> Boons => RegisteredBoons.AsReadOnly();
        public static IEnumerable<FactionComponent> Factions => RegisteredFactions.AsReadOnly();
        public static IEnumerable<WinConditionComponent> WinConditions => RegisteredWinConditions.AsReadOnly();
        public static IEnumerable<SpecialConditionComponent> SpecialConditions => RegisteredSpecialConditions.AsReadOnly();

        /// <summary>
        /// Register a new Origin to be usable in Playwright.
        /// </summary>
        public static void RegisterOrigin(OriginComponent origin)
        {
            RegisteredOrigins.Add(origin);
        }

        /// <summary>
        /// Register a new Boon to be usable in Playwright.
        /// </summary>
        public static void RegisterBoon(BoonComponent boon)
        {
            RegisteredBoons.Add(boon);
        }

        /// <summary>
        /// Register a new Faction to be usable in Playwright.
        /// </summary>
        public static void RegisterFaction(FactionComponent faction)
        {
            RegisteredFactions.Add(faction);
        }

        /// <summary>
        /// Register a new Win Condition to be usable in Playwright.
        /// </summary>
        public static void RegisterWinCondition(WinConditionComponent winCondition)
        {
            RegisteredWinConditions.Add(winCondition);
        }

        /// <summary>
        /// Register a new Special Condition to be usable in Playwright.
        /// </summary>
        public static void RegisterSpecialCondition(SpecialConditionComponent specialCondition)
        {
            RegisteredSpecialConditions.Add(specialCondition);
        }
    }
}
