using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.BSSapientAnimals.Components.Boons;
using System;
using Verse;

namespace Rokk.Playwright.Compat.BSSapientAnimals
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterBoon(new SapientAnimalsBoon());
        }
    }
}
