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
        // Things that are in the base game but have not been added to a DefOf
        [MayRequireAnomaly]
        public static FactionDef ResearchExpedition;

        [MayRequireOdyssey]
        public static FactionDef GravshipCrew;
    }
}
