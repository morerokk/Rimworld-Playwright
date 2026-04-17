using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.Sound;

namespace Rokk.Playwright.Utilities
{
    public static class SoundUtils
    {
        public static void PlayClick()
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
        }

        public static void PlayAdd()
        {
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
        }

        public static void PlayRemove()
        {
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }
    }
}
