using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.BSSapientAnimals.Components.Boons;
using Rokk.Playwright.Compat.BSSapientAnimals.Components.Origins;
using System;
using Verse;

namespace Rokk.Playwright.Compat.BSSapientAnimals
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterOrigin(new FailedExperimentOrigin());
            ComponentRegistration.RegisterBoon(new SapientAnimalsBoon());
        }
    }
}
