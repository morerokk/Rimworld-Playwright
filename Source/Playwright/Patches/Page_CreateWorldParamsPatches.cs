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
    // In the pre world generate screen, deselect some factions by default if a "no factions except" or a "deselect faction" scenpart is detected.
    // They can still be re-added manually.
    // Unremovable factions are kept (like Deserters, if both VFE Empire and VFE Deserters are installed).
    [HarmonyPatch(typeof(Page_CreateWorldParams), "ResetFactionCounts")]
    public class Page_CreateWorldParams_ResetFactionCountsPatches
    {
        [HarmonyPostfix]
        static void Postfix(ref List<FactionDef> ___factions, ref List<FactionDef> ___initialFactions)
        {
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

            // If a faction is hostile to everyone except X factions,
            // and at least one X faction is a player faction,
            // assume that this faction is normally supposed to always be neutral towards the player (like Cyberpunks, from Mort's Factions: The Corporation).
            // This is a weird edge case if the mod author intended to make a faction neutral to player tribals but hostile to player crashlanders,
            // but this is acceptable because this whole patch file is purely a convenience feature.
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
