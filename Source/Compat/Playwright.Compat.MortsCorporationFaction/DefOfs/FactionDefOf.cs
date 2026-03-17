using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Compat.MortsCorporationFaction.DefOfs
{
    // Use this to provide any DefOfs that the mod doesn't include in the C# code
    // (or if you don't want to have a reference to the mod's assemblies)
    // Use [MayRequire] attributes for things that are dependent on DLC's or other mods
    // (if those DLC's/mods aren't already listed as a hard dependency in your mod's About.xml)
    [DefOf]
    public static class FactionDefOf
    {
        public static FactionDef MF_Corporation;
    }
}
