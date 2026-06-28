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
    // Add Playwright button to scenario select page
    [HarmonyPatch(typeof(Page_SelectScenario), nameof(Page_SelectScenario.DoWindowContents))]
    public class Page_SelectScenario_DoWindowContentsPatches
    {
        [HarmonyPostfix]
        static void Postfix(Rect rect, Page_SelectScenario __instance)
        {
            if (Core.Settings == null || !Core.Settings.EnablePlaywrightButtonInScenarioSelect)
            {
                return;
            }

            float buttonX = rect.x + rect.width * 0.25f;
            float buttonY = rect.y + rect.height - 38f;
            Rect buttonRect = new Rect(buttonX, buttonY, 150f, 38f);
            if (Widgets.ButtonText(buttonRect, "Playwright.OpenPlaywrightButton".Translate()))
            {
                __instance.Close();
                PlaywrightUtils.OpenPlaywrightWindow(true);
            }
        }
    }
}
