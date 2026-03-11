using System;
using Verse;

namespace Rokk.Playwright
{
    public class Settings : ModSettings
    {
        public bool EnablePlaywrightButton = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref EnablePlaywrightButton, nameof(EnablePlaywrightButton), true);
            base.ExposeData();
        }

        public void ResetToDefaults()
        {
            EnablePlaywrightButton = true;
        }
    }
}
