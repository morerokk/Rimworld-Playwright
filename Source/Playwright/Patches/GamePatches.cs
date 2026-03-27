using HarmonyLib;
using RimWorld;
using Rokk.Playwright.PatchCheckers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatch(typeof(Game), nameof(Game.InitNewGame))]
    public class Game_InitNewGamePatches
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            // Most patches start un-applied.
            // On new game/game load, un-apply them if necessary and check if they should be re-applied.
            // This way, we run as little of our code as possible, only enough to make the player's current ScenParts work.
            FactionPatchChecker.CheckPatchFactionGoodwill();
            WinConditionPatchChecker.CheckPatchShipStartup();
            WinConditionPatchChecker.CheckPatchWinConditions();
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public class Game_LoadGamePatches
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            FactionPatchChecker.CheckPatchFactionGoodwill();
            WinConditionPatchChecker.CheckPatchShipStartup();
            WinConditionPatchChecker.CheckPatchWinConditions();
        }
    }
}
