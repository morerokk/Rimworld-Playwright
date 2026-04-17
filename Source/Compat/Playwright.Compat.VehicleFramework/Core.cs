using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.VehicleFramework.Components.Boons;
using System;
using Verse;

namespace Rokk.Playwright.Compat.VehicleFramework
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterBoon(new VehiclesBoon());
        }
    }
}
