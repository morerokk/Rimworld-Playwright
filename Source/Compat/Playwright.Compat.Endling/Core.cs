using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.Endling.Components.Boons;
using System;
using Verse;

namespace Rokk.Playwright.Compat.Endling
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterBoon(new EndlingsBoon());
        }
    }
}
