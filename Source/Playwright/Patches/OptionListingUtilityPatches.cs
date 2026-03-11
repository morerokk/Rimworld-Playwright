using HarmonyLib;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatch(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing))]
    public class OptionListingUtility_DrawOptionListingPatches
    {
        [HarmonyPrefix]
        public static void Prefix(ref List<ListableOption> optList)
        {
            // Add Playwright option to main menu

            if (Current.ProgramState != ProgramState.Entry)
            {
                return;
            }

            // Hack to check that we are in the correct list just to be sure
            // Someone PLEASE tell me a better method to just add a button to the menu
            string creditsLabel = "Credits".Translate();
            bool correctList = optList.Any(opt => opt.label == creditsLabel);
            if (!correctList)
            {
                return;
            }

            optList.Insert(0, new ListableOption("Playwright.OpenPlaywrightButton".Translate(), () =>
            {
                Find.WindowStack.Add(new PlaywrightWindow());
            }));
        }
    }
}
