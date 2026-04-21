using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.VREAndroids.Components.Boons;
using Rokk.Playwright.Compat.VREAndroids.Components.Origins;
using Rokk.Playwright.Utilities;
using System;
using Verse;

namespace Rokk.Playwright.Compat.VREAndroids
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            var androidsBoon = new AndroidsBoon();
            ComponentRegistration.RegisterOrigin(new AndroidsOrigin());
            ComponentRegistration.RegisterBoon(androidsBoon);
            if (PlaywrightUtils.IsModActive("MortStrudel.CorporationFaction"))
            {
                OriginDefaultsRegistration.RegisterDefaultBoon("Origins.Compat_MF_Corporate", () => { return androidsBoon; });
            }
        }
    }
}
