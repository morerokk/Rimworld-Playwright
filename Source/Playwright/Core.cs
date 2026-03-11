using HarmonyLib;
using Rokk.Playwright.Composer;
using System;
using UnityEngine;
using Verse;

namespace Rokk.Playwright
{
    public class Core : Mod
    {
        public static Settings Settings;
        public static PlaywrightBuilder Builder;

        public Core(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
            Builder = new PlaywrightBuilder();
            Harmony harmony = new Harmony("rokk.playwright");
            harmony.PatchAll();
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
    }
}
