using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
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
    [HarmonyPatch(typeof(GoodwillSituationManager), nameof(GoodwillSituationManager.GetNaturalGoodwill))]
    public class GoodwillSituationManager_GetNaturalGoodwillPatches
    {
        private static Dictionary<Faction, int> NaturalGoodwillCache = new Dictionary<Faction, int>();

        [HarmonyPostfix]
        static void Postfix(Faction other, ref int __result)
        {
            // If cached, write the value right away.
            // NOTE: Cache intentionally omits factions that are unaffected by the scenario parts!
            // I have no goddamn clue if some major event like sealing the void affects permanent goodwill or something
            // The old defs made this so much easier :(
            if (NaturalGoodwillCache.ContainsKey(other))
            {
                __result = NaturalGoodwillCache[other];
                return;
            }

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            // Find the natural goodwill. If there is a forced goodwill scenario part for a faction, use that.
            // Otherwise, try a natural goodwill scenario part
            int naturalGoodwill = 0;
            FactionForcedGoodwill forcedGoodwillPart = scenario.AllParts
                .Where(part => part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill)
                .Cast<FactionForcedGoodwill>()
                .FirstOrDefault(part => part.FactionToAffect == other.def);
            if (forcedGoodwillPart != null)
            {
                naturalGoodwill = forcedGoodwillPart.ForcedGoodwill;
            }
            else
            {
                FactionNaturalGoodwill naturalGoodwillPart = scenario.AllParts
                    .Where(part => part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill)
                    .Cast<FactionNaturalGoodwill>()
                    .FirstOrDefault(part => part.FactionToAffect == other.def);

                if (naturalGoodwillPart == null)
                {
                    NaturalGoodwillCache[other] = __result;
                    return;
                }

                naturalGoodwill = naturalGoodwillPart.NaturalGoodwill;
            }

            NaturalGoodwillCache[other] = naturalGoodwill;
            __result = naturalGoodwill;
        }

        public static void ClearCaches()
        {
            NaturalGoodwillCache.Clear();
        }
    }
}
