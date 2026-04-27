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
    // Override the natural goodwill of a faction if any "Set faction natural/forced goodwill" scenpart is detected
    [HarmonyPatchCategory(FactionPatchChecker.GoodwillCategory)]
    [HarmonyPatch(typeof(GoodwillSituationManager), nameof(GoodwillSituationManager.GetNaturalGoodwill))]
    public class GoodwillSituationManager_GetNaturalGoodwillPatches
    {
        [HarmonyPrefix]
        static bool Prefix(Faction other, ref int __result)
        {
            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return true;
            }

            // If there is a natural/forced goodwill scenario part for a faction, use that and ignore everything else.
            FactionGoodwill factionGoodwillPart = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill || part.def == DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill)
                .Cast<FactionGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def);

            if (factionGoodwillPart == null)
            {
                return true;
            }

            __result = factionGoodwillPart.Goodwill;
            return false;
        }
    }
}
