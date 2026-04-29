using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.VREAndroids.Components.Boons;
using Rokk.Playwright.Compat.VREAndroids.Components.Origins;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.VREAndroids
{
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
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
