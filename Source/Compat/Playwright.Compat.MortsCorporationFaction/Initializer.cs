using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Boons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction
{
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
        {
            ComponentRegistration.RegisterOrigin(new CorporateOrigin());
            ComponentRegistration.RegisterBoon(new CorporateGoonsBoon());
            ComponentRegistration.RegisterWinCondition(new CorporateBoardroomWinCondition());
            ComponentRegistration.RegisterWinCondition(new PaperworkWinCondition());
        }
    }
}
