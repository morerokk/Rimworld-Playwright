using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            // They can still be re-added manually, but by doing this, they will be removed on entering this page or when clicking reset
            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

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
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Neutral || neutralsToKeep.ExceptFactions.Contains(f) || f.hidden)
                    .ToList();
                ___factions = ___factions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Neutral || neutralsToKeep.ExceptFactions.Contains(f) || f.hidden)
                    .ToList();
            }

            if (hostilesToKeep != null)
            {
                ___initialFactions = ___initialFactions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Hostile || hostilesToKeep.ExceptFactions.Contains(f) || f.hidden)
                    .ToList();
                ___factions = ___factions
                    .Where(f => GetFactionRelationWithPlayer(f) != FactionRelationKind.Hostile || hostilesToKeep.ExceptFactions.Contains(f) || f.hidden)
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

            return FactionRelationKind.Neutral;
        }
    }
}
