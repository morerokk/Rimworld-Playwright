using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins;
using System;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterOrigin(new CorporateOrigin());
        }
    }
}
