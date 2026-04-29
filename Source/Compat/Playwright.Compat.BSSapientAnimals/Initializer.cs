using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.BSSapientAnimals.Components.Boons;
using Rokk.Playwright.Compat.BSSapientAnimals.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.BSSapientAnimals
{
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
        {
            ComponentRegistration.RegisterOrigin(new FailedExperimentOrigin());
            ComponentRegistration.RegisterBoon(new SapientAnimalsBoon());
        }
    }
}
