using HarmonyLib;
using RimWorld;
using Rokk.Playwright.GameComponents;
using Rokk.Playwright.PatchCheckers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatchCategory(WinConditionPatchChecker.TradeCategory)]
    [HarmonyPatch(typeof(Tradeable), nameof(Tradeable.ResolveTrade))]
    public static class Tradeable_ResolveTradePatches
    {
        [HarmonyPostfix]
        static void Postfix(Tradeable __instance)
        {
            // When the player sells an item, notify the win conditions gamecomponent
            if (__instance.ActionToDo != TradeAction.PlayerSells)
            {
                return;
            }

            var component = Current.Game.GetComponent<GameComponent_Playwright_WinConditions>();
            if (component == null)
            {
                return;
            }

            foreach (Thing thing in __instance.thingsColony)
            {
                component.NotifyThingSold(thing);
            }
        }
    }
}
