using HarmonyLib;
using RimWorld;
using Rokk.Playwright.PatchCheckers;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Patches
{
    // Override the initial relations for a generated faction, if any "Set faction natural/forced goodwill" scenpart is detected.
    // This patch exists to deal with factions that are added mid-game,
    // as the scenparts themselves should have already taken care of factions present during worldgen.
    // 
    // Note: this method is called once, but when it's done, both factions have mutual relations with each other
    // (vanilla method adds the relation in this instance, and in the other instance, so both sides are available in postfixes)
    [HarmonyPatchCategory(FactionPatchChecker.GoodwillCategory)]
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

            // If no natural/forced goodwill parts exist, this doesn't concern us
            int initialGoodwill = 0;
            FactionGoodwill factionGoodwillPart = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill || part.def == DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill)
                .Cast<FactionGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def || part.FactionToAffect == __instance.def);
            if (factionGoodwillPart == null)
            {
                return;
            }

            // Natural goodwill slider can go between -200 and +200, but any "actual" goodwill values should be between -100 and +100
            initialGoodwill = Math.Clamp(factionGoodwillPart.Goodwill, -100, 100);

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

    // Since the relations have already been set up above, completely reject ALL attempts to change it,
    // if forced by a "Set faction forced goodwill" scenpart.
    [HarmonyPatchCategory(FactionPatchChecker.GoodwillCategory)]
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

    // If forced by a "Set faction forced goodwill" scenpart, let the game know that any goodwill changes here are going to be categorically rejected.
    // (Makes the UI look slightly nicer, does things like showing that transport pod items will be lost and will not change goodwill)
    [HarmonyPatchCategory(FactionPatchChecker.GoodwillCategory)]
    [HarmonyPatch(typeof(Faction), nameof(Faction.CanChangeGoodwillFor))]
    public class Faction_CanChangeGoodwillForPatches
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
