using System;
using Verse;

namespace Rokk.Playwright
{
    public class Settings : ModSettings
    {
        public bool EnablePlaywrightButton = true;
        public bool HideReplacedFactions = true;
        public bool HideDryadsInAnimals = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnablePlaywrightButton, nameof(EnablePlaywrightButton), true);
            Scribe_Values.Look(ref HideReplacedFactions, nameof(HideReplacedFactions), true);
            Scribe_Values.Look(ref HideDryadsInAnimals, nameof(HideDryadsInAnimals), true);
        }

        public void ResetToDefaults()
        {
            EnablePlaywrightButton = true;
            HideReplacedFactions = true;
            HideDryadsInAnimals = true;
        }
    }
}
