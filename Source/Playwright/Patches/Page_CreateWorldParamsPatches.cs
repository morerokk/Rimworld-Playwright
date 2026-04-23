using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatch(typeof(Page_CreateWorldParams), "ResetFactionCounts")]
    public class Page_CreateWorldParams_ResetFactionCountsPatches
    {
        [HarmonyPostfix]
        static void Postfix(ref List<FactionDef> ___factions, ref List<FactionDef> ___initialFactions)
        {
            // On initial setup of the factions or on reset button click, filter out any factions that are removed by default.
            // They can still be re-added manually, but with this patch, they will be removed on entering this page or when clicking reset
            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            // Filter out specific factions first
            List<RemoveFaction> removeFactionParts = scenario.AllParts
                .Where(p => p.def == DefOfs.ScenPartDefOf.Playwright_RemoveFaction)
                .Cast<RemoveFaction>()
                .ToList();
            if (removeFactionParts.Count > 0)
            {
                ___initialFactions = ___initialFactions
                    .Where(f => !removeFactionParts.Any(part => part.Faction == f) || FactionUnremovable(f))
                    .ToList();
                ___factions = ___factions
                    .Where(f => !removeFactionParts.Any(part => part.Faction == f) || FactionUnremovable(f))
                    .ToList();
            }

            // Do blanket filtering if parts are selected
            NoNeutralFactionsExcept neutralsToKeep = scenario.AllParts
                .Where(p => p.def == DefOfs.ScenPartDefOf.Playwright_NoNeutralFactionsExcept)
                .Cast<NoNeutralFactionsExcept>()
                .FirstOrDefault();

            NoHostileFactionsExcept hostilesToKeep = scenario.AllParts
                .Where(p => p.def == DefOfs.ScenPartDefOf.Playwright_NoHostileFactionsExcept)
                .Cast<NoHostileFactionsExcept>()
                .FirstOrDefault();

            if (neutralsToKeep == null && hostilesToKeep == null)
            {
                return;
            }

            if (neutralsToKeep != null)
            {
                ___initialFactions = ___initialFactions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Neutral || neutralsToKeep.ExceptFactions.Contains(f) || f.hidden || FactionUnremovable(f))
                    .ToList();
                ___factions = ___factions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Neutral || neutralsToKeep.ExceptFactions.Contains(f) || f.hidden || FactionUnremovable(f))
                    .ToList();
            }

            if (hostilesToKeep != null)
            {
                ___initialFactions = ___initialFactions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Hostile || hostilesToKeep.ExceptFactions.Contains(f) || f.hidden || FactionUnremovable(f))
                    .ToList();
                ___factions = ___factions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Hostile || hostilesToKeep.ExceptFactions.Contains(f) || f.hidden || FactionUnremovable(f))
                    .ToList();
            }
        }

        private static FactionRelationKind GetFactionRelationWithPlayer(FactionDef factionDef)
        {
            // Taken from Faction (TryMakeInitialRelationsWith), not sure about compatibility
            if (factionDef.permanentEnemy)
            {
                return FactionRelationKind.Hostile;
            }

            if (factionDef.naturalEnemy)
            {
                return FactionRelationKind.Hostile;
            }

            if (factionDef.permanentEnemyToEveryoneExcept != null && !factionDef.permanentEnemyToEveryoneExcept.Any(f => f.isPlayer))
            {
                return FactionRelationKind.Hostile;
            }

            return FactionRelationKind.Neutral;
        }

        /// <summary>
        /// Uses hardcoded exceptions to determine if a faction is unremovable and should be skipped by these ScenParts.
        /// </summary>
        /// I'd like to read out Vanilla Expanded Framework's modextensions for this to prevent hardcoding for their mods, but they might be subject to change,
        /// and the license doesn't allow it anyway. The mods that use this are likely a known, limited subset, so this workaround should be fine.
        /// (Stop using CC licenses for software, CC themselves tell you not to use it for software)
        public static bool FactionUnremovable(FactionDef factionDef)
        {
            // VFEE_Deserters is only present with VFE Empire installed, but becomes mandatory with VFE Deserters.
            // Since they disallow removing it from the faction list, don't allow it here, either
            if (factionDef.defName == "VFEE_Deserters" && PlaywrightUtils.IsModActive("OskarPotocki.VFE.Deserters"))
            {
                return true;
            }

            return false;
        }
    }
}
