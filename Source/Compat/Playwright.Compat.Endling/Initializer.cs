using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.Endling.Components.Boons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.Endling
{
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
        {
            ComponentRegistration.RegisterBoon(new EndlingsBoon());
        }
    }
}
