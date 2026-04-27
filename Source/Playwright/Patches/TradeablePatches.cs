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
    // When an item is sold to a trader, if there are any "Win condition: sell items" parts, notify those scenparts of the trade occurring.
    // TODO: This only handles items, because pawns (prisoners, slaves) have their own implementation of ResolveTrade instead.
    // Do we also have to handle prisoners/slaves? Could be fun tbh
    [HarmonyPatchCategory(WinConditionPatchChecker.TradeCategory)]
    [HarmonyPatch(typeof(Tradeable), nameof(Tradeable.ResolveTrade))]
    public static class Tradeable_ResolveTradePatches
    {
        [HarmonyPostfix]
        static void Postfix(Tradeable __instance)
        {
            if (__instance.ActionToDo != TradeAction.PlayerSells)
            {
                return;
            }

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            List<WinCondition_SellItems> sellItemsParts = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_SellItems)
                .Cast<WinCondition_SellItems>()
                .ToList();
            foreach (WinCondition_SellItems sellItemsPart in sellItemsParts)
            {
                foreach (Thing thing in __instance.thingsColony)
                {
                    sellItemsPart.NotifyThingSoldToTrader(thing);
                }
            }
        }
    }
}
