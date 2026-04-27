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
    // Disable the ship reactor sequence button if the "Disable ship startup" scenpart is detected.
    // The rest of the ship still works (for mod compatibility reasons, or if the player just wants a really expensive vanometric power cell).
    // They just can't initiate the reactor sequence and the raid gauntlet, meaning they also can't launch it, as the gizmo is gone.
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
                yield break;
            }

            // Intercept each returned gizmo, if it's the ship startup button then don't return it
            // TODO: Can't we clean up this whole method by not making it an iterator, and using .Where() to filter instead?
            // The impact is really minor so I don't think it should matter
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
