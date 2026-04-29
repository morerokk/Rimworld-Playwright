using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Factions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.VFEEmpire
{
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
        {
            OriginDefaultsRegistration.RegisterDefaultEnemy("Origins.Empire", () =>
            {
                return new SpecificFaction() { Faction = DefOfs.FactionDefOf.VFEE_Deserters };
            });
        }
    }
}
