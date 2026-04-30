using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.PatchCheckers
{
    /// <summary>
    /// Intentionally internal because it deals with patches and code that only exist in this assembly,
    /// and are tailored to this assembly's ScenParts. I don't see a reason for outside code to access this, but if you need it, let me know
    /// </summary>
    internal static class FactionPatchChecker
    {
        internal const string GoodwillCategory = "Playwright.FactionGoodwill";
        internal static bool FactionGoodwillPatched { get; private set; } = false;

        /// <summary>
        /// Unpatches the Harmony faction-related patches, and re-patches them if necessary.
        /// The reason for this being that the faction patches are a little more invasive than they probably should be,
        /// and too many uncertainties exist.
        /// For players that aren't using these scenario parts, they should not be getting in the way.
        /// </summary>
        internal static void CheckPatchFactionGoodwill()
        {
            if (Core.Harmony == null)
            {
                return;
            }

            // Unpatch the spooky faction patches first
            UnpatchFactionGoodwill();

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                Log.Warning("[Playwright] Scenario was null while re-evaluating if faction goodwill patches have to be run. Faction goodwill-related scenario parts have been disabled.");
                return;
            }

            bool hasFactionParts = scenario.AllParts
                .Any(part =>
                    part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill
                    || part.def == Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill
                );

            if (hasFactionParts)
            {
                PatchFactionGoodwill();
            }
        }

        private static void PatchFactionGoodwill()
        {
            if (FactionGoodwillPatched)
            {
                return;
            }

            Core.Harmony.PatchCategory(GoodwillCategory);

            FactionGoodwillPatched = true;
        }

        private static void UnpatchFactionGoodwill()
        {
            if (!FactionGoodwillPatched)
            {
                return;
            }

            Core.Harmony.UnpatchCategory(GoodwillCategory);

            FactionGoodwillPatched = false;
        }
    }
}
