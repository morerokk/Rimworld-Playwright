using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.DefOfs
{
    [DefOf]
    public static class FactionDefOf
    {
        // Player factions that are missing in factiondef
        [MayRequireAnomaly]
        public static FactionDef ResearchExpedition;

        [MayRequireOdyssey]
        public static FactionDef GravshipCrew;
    }
}
