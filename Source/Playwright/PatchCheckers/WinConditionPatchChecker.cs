using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.PatchCheckers
{
    public static class WinConditionPatchChecker
    {
        internal const string ShipStartupCategory = "Playwright.ShipStartup";
        internal static bool ShipStartupPatched = false;

        internal static void CheckPatchShipStartup()
        {
            if (Core.Harmony == null)
            {
                return;
            }

            UnpatchShipStartup();

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            if (scenario.AllParts.Any(p => p.def == DefOfs.ScenPartDefOf.Playwright_DisableShipStartup))
            {
                PatchShipStartup();
            }
        }

        private static void PatchShipStartup()
        {
            if (ShipStartupPatched)
            {
                return;
            }
            Core.Harmony.PatchCategory(ShipStartupCategory);
            ShipStartupPatched = true;
        }

        private static void UnpatchShipStartup()
        {
            if (!ShipStartupPatched)
            {
                return;
            }
            Core.Harmony.UnpatchCategory(ShipStartupCategory);
            ShipStartupPatched = false;
        }

        internal const string TradeCategory = "Playwright.Trade";
        internal static bool TradePatched = false;

        internal static void CheckPatchWinConditions()
        {
            if (Core.Harmony == null)
            {
                return;
            }

            UnpatchTrade();

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            if (scenario.AllParts.Any(p => p.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_SellItems))
            {
                PatchTrade();
            }
        }

        private static void PatchTrade()
        {
            if (TradePatched)
            {
                return;
            }
            Core.Harmony.PatchCategory(TradeCategory);
            TradePatched = true;
        }

        private static void UnpatchTrade()
        {
            if (!TradePatched)
            {
                return;
            }
            Core.Harmony.UnpatchCategory(TradeCategory);
            TradePatched = false;
        }
    }
}
