using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
using Rokk.Playwright.PatchCheckers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatchCategory(WinConditionPatchChecker.ShipStartupCategory)]
    [HarmonyPatch(typeof(ShipUtility), nameof(ShipUtility.ShipStartupGizmos))]
    public class ShipUtility_ShipStartupGizmosPatches
    {
        [HarmonyPostfix]
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values)
        {
            Scenario scenario = Find.Scenario;
            if (!scenario.AllParts.Any(p => p.def == DefOfs.ScenPartDefOf.Playwright_DisableShipStartup))
            {
                foreach (Gizmo gizmo in values)
                {
                    yield return gizmo;
                }
            }

            // Intercept each returned gizmo, if it's the ship startup button then don't return it
            foreach (Gizmo gizmo in values)
            {
                Command_Action action = gizmo as Command_Action;
                if (action == null || action.defaultLabel != "CommandShipStartup".Translate())
                {
                    yield return gizmo;
                }
            }
            yield break;
        }
    }
}
