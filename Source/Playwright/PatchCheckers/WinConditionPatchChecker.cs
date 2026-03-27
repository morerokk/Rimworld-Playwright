using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            Core.Harmony.PatchCategory(Assembly.GetExecutingAssembly(), ShipStartupCategory);
            ShipStartupPatched = true;
        }

        private static void UnpatchShipStartup()
        {
            if (!ShipStartupPatched)
            {
                return;
            }
            Core.Harmony.UnpatchCategory(Assembly.GetExecutingAssembly(), ShipStartupCategory);
            ShipStartupPatched = false;
        }


        // TODO: Is this even necessary now that we have a GameComponent for it?
        internal const string ColonyCategory = "Playwright.WinCondition_Colony";
        internal static bool ColonyPatched = false;

        internal static void CheckPatchWinConditions()
        {
            if (Core.Harmony == null)
            {
                return;
            }

            UnpatchColony();

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                return;
            }

            if (scenario.AllParts.Any(p => p.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_Colony))
            {
                PatchColony();
            }
        }

        private static void PatchColony()
        {
            if (ColonyPatched)
            {
                return;
            }
            Core.Harmony.PatchCategory(Assembly.GetExecutingAssembly(), ColonyCategory);
            ColonyPatched = true;
        }

        private static void UnpatchColony()
        {
            if (!ColonyPatched)
            {
                return;
            }
            Core.Harmony.UnpatchCategory(Assembly.GetExecutingAssembly(), ColonyCategory);
            ColonyPatched = false;
        }
    }
}
