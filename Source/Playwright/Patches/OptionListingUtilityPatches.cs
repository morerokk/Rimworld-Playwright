using HarmonyLib;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Patches
{
    // Add Playwright button to main menu
    [HarmonyPatch(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing))]
    public class OptionListingUtility_DrawOptionListingPatches
    {
        [HarmonyPrefix]
        static void Prefix(ref List<ListableOption> optList)
        {
            if (Current.ProgramState != ProgramState.Entry || Core.Settings == null)
            {
                return;
            }

            if (!Core.Settings.EnablePlaywrightButton)
            {
                return;
            }

            // Hacky way to check that we are in the correct list to add the button to.
            // TODO: Is there a better way to do this?
            string creditsLabel = "Credits".Translate();
            bool correctList = optList.Any(opt => opt.label == creditsLabel);
            if (!correctList)
            {
                return;
            }

            optList.Insert(0, new ListableOption("Playwright.OpenPlaywrightButton".Translate(), () =>
            {
                PlaywrightUtils.OpenPlaywrightWindow(true);
            }));
        }
    }
}
