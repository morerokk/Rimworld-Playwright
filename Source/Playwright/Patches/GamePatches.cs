using HarmonyLib;
using RimWorld;
using Rokk.Playwright.PatchCheckers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatch(typeof(Game), nameof(Game.InitNewGame))]
    public class Game_InitNewGamePatches
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            // Most patches start un-applied thanks to Harmony patch categories.
            // On game start/load, we un-apply all patches, then only re-apply them if we found a relevant ScenPart that would necessitate the patch.
            FactionPatchChecker.CheckPatchFactionGoodwill();
            WinConditionPatchChecker.CheckPatchShipStartup();
            WinConditionPatchChecker.CheckPatchWinConditions();
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public class Game_LoadGamePatches
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            FactionPatchChecker.CheckPatchFactionGoodwill();
            WinConditionPatchChecker.CheckPatchShipStartup();
            WinConditionPatchChecker.CheckPatchWinConditions();
        }
    }
}
