using HarmonyLib;
using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Patches
{
    // Add Playwright button to scenario edit page
    [HarmonyPatch(typeof(Page_ScenarioEditor), nameof(Page_ScenarioEditor.DoWindowContents))]
    public class Page_ScenarioEditor_DoWindowContentsPatches
    {
        [HarmonyPostfix]
        static void Postfix(Rect rect, Page_ScenarioEditor __instance)
        {
            if (Core.Settings == null || !Core.Settings.EnablePlaywrightButtonInScenarioEditor)
            {
                return;
            }

            float buttonX = rect.x + rect.width * 0.25f;
            float buttonY = rect.y + rect.height - 38f;
            Rect buttonRect = new Rect(buttonX, buttonY, 150f, 38f);
            if (Widgets.ButtonText(buttonRect, "Playwright.OpenPlaywrightButton".Translate()))
            {
                // Check if there are unsaved changes so we don't throw away any player edits
                // (This is probably the least ideal place to have this button tbh, but some menu-changing mods make this necessary)
                if (__instance.ScenarioEdited)
                {
                    PlaywrightUtils.OpenPlaywrightWindow(false);
                }
                else
                {
                    __instance.Close();
                    PlaywrightUtils.OpenPlaywrightWindow(true);
                }
            }
        }
    }
}
