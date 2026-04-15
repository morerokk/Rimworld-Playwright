using System;
using Verse;

namespace Rokk.Playwright
{
    public class Settings : ModSettings
    {
        public bool EnablePlaywrightButton = true;
        public bool HideReplacedFactions = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnablePlaywrightButton, nameof(EnablePlaywrightButton), true);
            Scribe_Values.Look(ref HideReplacedFactions, nameof(HideReplacedFactions), true);
        }

        public void ResetToDefaults()
        {
            EnablePlaywrightButton = true;
        }
    }
}
