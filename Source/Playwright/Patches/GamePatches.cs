using HarmonyLib;
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
        [HarmonyPostfix]
        static void Postfix()
        {
            // On game load, clear caches in case players reload a different save
            GoodwillSituationManager_GetNaturalGoodwillPatches.ClearCaches();
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public class Game_LoadGamePatches
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            GoodwillSituationManager_GetNaturalGoodwillPatches.ClearCaches();
        }
    }
}
