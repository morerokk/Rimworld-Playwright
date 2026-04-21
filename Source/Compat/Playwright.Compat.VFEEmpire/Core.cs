using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Utilities;
using System;
using Verse;

namespace Rokk.Playwright.Compat.VFEEmpire
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            OriginDefaultsRegistration.RegisterDefaultEnemy("Origins.Empire", () =>
            {
                return new SpecificFaction() { Faction = DefOfs.FactionDefOf.VFEE_Deserters };
            });
        }
    }
}
