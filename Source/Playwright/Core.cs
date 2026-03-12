using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Composer;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Rokk.Playwright
{
    public class Core : Mod
    {
        public static Settings Settings;
        public static PlaywrightBuilder Builder;

        private static Harmony Harmony;
        private static bool FactionGoodwillPatched = false;

        public Core(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
            Builder = new PlaywrightBuilder();
            Harmony = new Harmony("rokk.playwright");
            Harmony.PatchAllUncategorized(Assembly.GetExecutingAssembly());
            // TODO: Should we split up or categorize harmony patches,
            // so that potentially invasive or destructive ones don't even run if the scenario part isn't loaded?
            // I'm concerned about the invasive stuff we're doing to factions
            
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("Playwright.Settings.EnablePlaywrightButton".Translate(), ref Settings.EnablePlaywrightButton);

            listingStandard.Gap();

            if (listingStandard.ButtonText("Playwright.Reset".Translate(), null, 0.25f))
            {
                Settings.ResetToDefaults();
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Playwright.ModTitle".Translate();
        }

        /// <summary>
        /// Unpatches the Harmony faction-related patches, and re-patches them if necessary.
        /// The reason for this being that the faction patches are a little more invasive than they probably should be,
        /// and too many uncertainties exist.
        /// For players that aren't using these scenario parts, they should not be getting in the way.
        /// </summary>
        public static void CheckPatchFactionGoodwill()
        {
            // Unpatch the spooky faction patches first
            UnpatchFactionGoodwill();

            Scenario scenario = Find.Scenario;
            if (scenario == null)
            {
                Log.Error("[Playwright] Scenario was null while re-evaluating if faction goodwill patches have to be run. Faction goodwill-related scenario parts have been disabled.");
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

            Harmony.PatchCategory(Assembly.GetExecutingAssembly(), "Playwright.FactionGoodwill");

            FactionGoodwillPatched = true;
        }

        private static void UnpatchFactionGoodwill()
        {
            if (!FactionGoodwillPatched)
            {
                return;
            }

            Harmony.UnpatchCategory(Assembly.GetExecutingAssembly(), "Playwright.FactionGoodwill");

            FactionGoodwillPatched = false;
        }
    }
}
