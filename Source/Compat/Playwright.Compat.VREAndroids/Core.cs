using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.VREAndroids.Components.Origins;
using Rokk.Playwright.Compat.VREAndroids.Components.Boons;
using System;
using Verse;

namespace Rokk.Playwright.Compat.VREAndroids
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterOrigin(new AndroidsOrigin());
            ComponentRegistration.RegisterBoon(new AndroidsBoon());
        }
    }
}
