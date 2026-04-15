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

        internal static Harmony Harmony;

        public Core(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
            Builder = new PlaywrightBuilder();
            Harmony = new Harmony("rokk.playwright");
            Harmony.PatchAllUncategorized(Assembly.GetExecutingAssembly());            
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("Playwright.Settings.EnablePlaywrightButton".Translate(), ref Settings.EnablePlaywrightButton);
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("Playwright.Settings.HideReplacedFactions".Translate(), ref Settings.HideReplacedFactions);

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
