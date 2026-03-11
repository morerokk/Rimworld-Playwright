using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Patches
{
    // This method is called once, but when it's done, both factions have mutual relations with each other
    // (vanilla method adds it in this, and in the other)
    // Setup initial relations for forced/natural goodwill scenario parts
    [HarmonyPatch(typeof(Faction), nameof(Faction.TryMakeInitialRelationsWith))]
    public class Faction_TryMakeInitialRelationsWithPatches
    {
        [HarmonyPostfix]
        static void Postfix(Faction other, Faction __instance)
        {
            if (!__instance.IsPlayer && !other.IsPlayer)
            {
                return;
            }

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            // Try forced goodwill part, if that doesn't exist try a natural goodwill part
            // If that still doesn't exist, this doesn't concern us
            int initialGoodwill = 0;
            FactionForcedGoodwill forcedGoodwillPart = scenario.AllParts
                .Where(part => part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill)
                .Cast<FactionForcedGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def || part.FactionToAffect == __instance.def);
            if (forcedGoodwillPart != null)
            {
                initialGoodwill = forcedGoodwillPart.ForcedGoodwill;
            }
            else
            {
                FactionNaturalGoodwill naturalGoodwillPart = scenario.AllParts
                .Where(part => part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill)
                .Cast<FactionNaturalGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def || part.FactionToAffect == __instance.def);
                if (naturalGoodwillPart == null)
                {
                    return;
                }
                initialGoodwill = naturalGoodwillPart.NaturalGoodwill;
            }

            FactionRelationKind relationKind = FactionRelationKind.Neutral;
            if (initialGoodwill <= -75)
            {
                relationKind = FactionRelationKind.Hostile;
            }
            else if (initialGoodwill >= 75)
            {
                relationKind = FactionRelationKind.Ally;
            }

            FactionRelation relation1 = __instance.RelationWith(other);
            FactionRelation relation2 = other.RelationWith(__instance);
            relation1.baseGoodwill = initialGoodwill;
            relation1.kind = relationKind;
            relation2.baseGoodwill = initialGoodwill;
            relation2.kind = relationKind;
        }
    }

    // Since the relations have already been set up above, completely reject ALL attempts to change it if forced by the scenario
    [HarmonyPatch(typeof(Faction), nameof(Faction.TryAffectGoodwillWith))]
    public class Faction_TryAffectGoodwillWithPatches
    {
        [HarmonyPrefix]
        static bool Prefix(Faction other, Faction __instance, ref bool __result)
        {
            if (!__instance.IsPlayer && !other.IsPlayer)
            {
                return true;
            }

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return true;
            }

            FactionForcedGoodwill forcedGoodwillPart = scenario.AllParts
                .Where(part => part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill)
                .Cast<FactionForcedGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def || part.FactionToAffect == __instance.def);

            if (forcedGoodwillPart == null)
            {
                return true;
            }

            // If we got this far, reject the goodwill change entirely.
            __result = false;
            return false;
        }
    }
}
