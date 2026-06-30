using System;
using Verse;

namespace Rokk.Playwright
{
    public class Settings : ModSettings
    {
        public bool EnablePlaywrightButtonInMainMenu = false;
        public bool EnablePlaywrightButtonInScenarioSelect = true;
        public bool EnablePlaywrightButtonInScenarioEditor = true;
        public bool CategorizeItemSelectors = true;
        public bool HideReplacedFactions = true;
        public bool HideDryadsInAnimals = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnablePlaywrightButtonInMainMenu, nameof(EnablePlaywrightButtonInMainMenu), false);
            Scribe_Values.Look(ref EnablePlaywrightButtonInScenarioSelect, nameof(EnablePlaywrightButtonInScenarioSelect), true);
            Scribe_Values.Look(ref EnablePlaywrightButtonInScenarioEditor, nameof(EnablePlaywrightButtonInScenarioEditor), true);
            Scribe_Values.Look(ref CategorizeItemSelectors, nameof(CategorizeItemSelectors), true);
            Scribe_Values.Look(ref HideReplacedFactions, nameof(HideReplacedFactions), true);
            Scribe_Values.Look(ref HideDryadsInAnimals, nameof(HideDryadsInAnimals), true);
        }

        public void ResetToDefaults()
        {
            EnablePlaywrightButtonInMainMenu = false;
            EnablePlaywrightButtonInScenarioSelect = true;
            EnablePlaywrightButtonInScenarioEditor = true;
            CategorizeItemSelectors = true;
            HideReplacedFactions = true;
            HideDryadsInAnimals = true;
        }
    }
}
