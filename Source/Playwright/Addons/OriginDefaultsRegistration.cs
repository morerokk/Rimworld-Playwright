using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Addons
{
    /// <summary>
    /// Helper class that can be used to add new default selections to an Origin from mod compatibility layers, or from addons.
    /// The origin doesn't need to actually exist to add default components to it,
    /// but you are encouraged to only call these methods if you expect the Origin to potentially be present (such as a ModsConfig check).
    /// The origin can be a built-in Playwright origin, but it could also be an Origin added by another mod.
    /// </summary>
    public static class OriginDefaultsRegistration
    {
        private static Dictionary<string, List<BoonComponent>> DefaultBoonRegistrations = new Dictionary<string, List<BoonComponent>>();
        private static Dictionary<string, List<FactionComponent>> DefaultAllyRegistrations = new Dictionary<string, List<FactionComponent>>();
        private static Dictionary<string, List<FactionComponent>> DefaultNeutralRegistrations = new Dictionary<string, List<FactionComponent>>();
        private static Dictionary<string, List<FactionComponent>> DefaultEnemyRegistrations = new Dictionary<string, List<FactionComponent>>();
        private static Dictionary<string, List<WinConditionComponent>> DefaultWinConditionRegistrations = new Dictionary<string, List<WinConditionComponent>>();
        private static Dictionary<string, List<SpecialConditionComponent>> DefaultSpecialConditionRegistrations = new Dictionary<string, List<SpecialConditionComponent>>();

        /// <summary>
        /// Add a Boon to the specified Origin's default boons.
        /// </summary>
        /// <param name="originId">The ID of the origin to add a boon to.</param>
        /// <param name="boon">The boon to add to the defaults.</param>
        public static void RegisterDefaultBoon(string originId, BoonComponent boon)
        {
            if (!DefaultBoonRegistrations.ContainsKey(originId))
            {
                DefaultBoonRegistrations[originId] = new List<BoonComponent>();
            }
            DefaultBoonRegistrations[originId].Add(boon);
        }

        public static void RegisterDefaultAlly(string originId, FactionComponent ally)
        {
            if (!DefaultAllyRegistrations.ContainsKey(originId))
            {
                DefaultAllyRegistrations[originId] = new List<FactionComponent>();
            }
            DefaultAllyRegistrations[originId].Add(ally);
        }

        public static void RegisterDefaultNeutral(string originId, FactionComponent neutralFaction)
        {
            if (!DefaultNeutralRegistrations.ContainsKey(originId))
            {
                DefaultNeutralRegistrations[originId] = new List<FactionComponent>();
            }
            DefaultNeutralRegistrations[originId].Add(neutralFaction);
        }

        public static void RegisterDefaultEnemy(string originId, FactionComponent enemy)
        {
            if (!DefaultEnemyRegistrations.ContainsKey(originId))
            {
                DefaultEnemyRegistrations[originId] = new List<FactionComponent>();
            }
            DefaultEnemyRegistrations[originId].Add(enemy);
        }

        public static void RegisterDefaultWinCondition(string originId, WinConditionComponent winCondition)
        {
            if (!DefaultWinConditionRegistrations.ContainsKey(originId))
            {
                DefaultWinConditionRegistrations[originId] = new List<WinConditionComponent>();
            }
            DefaultWinConditionRegistrations[originId].Add(winCondition);
        }

        public static void RegisterDefaultSpecialCondition(string originId, SpecialConditionComponent specialCondition)
        {
            if (!DefaultSpecialConditionRegistrations.ContainsKey(originId))
            {
                DefaultSpecialConditionRegistrations[originId] = new List<SpecialConditionComponent>();
            }
            DefaultSpecialConditionRegistrations[originId].Add(specialCondition);
        }


        internal static List<BoonComponent> GetDefaultBoons(string originId)
        {
            if (DefaultBoonRegistrations.TryGetValue(originId, out var boons))
            {
                return boons;
            }
            return new List<BoonComponent>();
        }

        internal static List<FactionComponent> GetDefaultAllies(string originId)
        {
            if (DefaultAllyRegistrations.TryGetValue(originId, out var allies))
            {
                return allies;
            }
            return new List<FactionComponent>();
        }

        internal static List<FactionComponent> GetDefaultNeutrals(string originId)
        {
            if (DefaultNeutralRegistrations.TryGetValue(originId, out var neutrals))
            {
                return neutrals;
            }
            return new List<FactionComponent>();
        }

        internal static List<FactionComponent> GetDefaultEnemies(string originId)
        {
            if (DefaultEnemyRegistrations.TryGetValue(originId, out var enemies))
            {
                return enemies;
            }
            return new List<FactionComponent>();
        }

        internal static List<WinConditionComponent> GetDefaultWinConditions(string originId)
        {
            if (DefaultWinConditionRegistrations.TryGetValue(originId, out var winConditions))
            {
                return winConditions;
            }
            return new List<WinConditionComponent>();
        }

        internal static List<SpecialConditionComponent> GetDefaultSpecialConditions(string originId)
        {
            if (DefaultSpecialConditionRegistrations.TryGetValue(originId, out var specialConditions))
            {
                return specialConditions;
            }
            return new List<SpecialConditionComponent>();
        }
    }
}
