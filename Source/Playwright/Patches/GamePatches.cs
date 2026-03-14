using HarmonyLib;
using RimWorld;
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
            Core.CheckPatchFactionGoodwill();
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public class Game_LoadGamePatches
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            Core.CheckPatchFactionGoodwill();
        }
    }
}
