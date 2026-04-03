using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.WinConditions;
using System;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            ComponentRegistration.RegisterOrigin(new CorporateOrigin());
            ComponentRegistration.RegisterWinCondition(new CorporateBoardroomWinCondition());
        }
    }
}
