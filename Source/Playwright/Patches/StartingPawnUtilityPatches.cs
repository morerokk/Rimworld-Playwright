using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Patches
{
    // Make the Randomize button a no-op in the colonist preparation screen if the "No Colonist Rerolls" ScenPart is detected
    [HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.RandomizePawn))]
    public class StartingPawnUtility_RandomizePawnPatches
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return true;
            }

            bool hasNoColonistRerollsPart = scenario.AllParts
                .Any(part => part.def == DefOfs.ScenPartDefOf.Playwright_NoColonistRerolls);

            if (hasNoColonistRerollsPart)
            {
                return false;
            }

            return true;
        }
    }
}
