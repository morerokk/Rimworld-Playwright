using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Utilities;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Rokk.Playwright
{
    public class Core : Mod
    {
        internal static Harmony Harmony;
        internal static Core Current;
        public static Settings Settings;

        public Core(ModContentPack content) : base(content)
        {
            Harmony = new Harmony("rokk.playwright");
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("Playwright.Settings.EnablePlaywrightButton".Translate(), ref Settings.EnablePlaywrightButton, "Playwright.Settings.EnablePlaywrightButton.Help".Translate());
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("Playwright.Settings.HideReplacedFactions".Translate(), ref Settings.HideReplacedFactions, "Playwright.Settings.HideReplacedFactions.Help".Translate());
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("Playwright.Settings.HideDryadsInAnimals".Translate(), ref Settings.HideReplacedFactions, "Playwright.Settings.HideDryadsInAnimals.Help".Translate());
            listingStandard.Gap();

            // Make editor always accessible from options menu, in case the main menu playwright button is disabled or if some other mod completely redoes the main menu
            if (listingStandard.ButtonText("Playwright.Settings.OpenPlaywrightButton".Translate(), "Playwright.Settings.OpenPlaywrightButton.Help", 0.25f))
            {
                PlaywrightUtils.OpenPlaywrightWindow(false);
            }

            listingStandard.Gap(48f);

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
    }
}
