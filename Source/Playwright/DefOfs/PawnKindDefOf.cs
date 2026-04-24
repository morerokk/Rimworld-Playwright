using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.DefOfs
{
    [DefOf]
    public static class PawnKindDefOf
    {
        // Things that are in the base game but have not been added to a DefOf
        [MayRequireBiotech]
        public static PawnKindDef Mechanitor;
        [MayRequireBiotech]
        public static PawnKindDef Mech_Lifter;
        [MayRequireBiotech]
        public static PawnKindDef Mech_Constructoid;
    }
}
