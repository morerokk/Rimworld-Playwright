using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.DefOfs
{
    [DefOf]
    public static class IncidentDefOf
    {
        // Things that are in the base game but have not been added to a DefOf
        public static IncidentDef GiveQuest_EndGame_ShipEscape;

        [MayRequireRoyalty]
        public static IncidentDef GiveQuest_EndGame_RoyalAscent;

        [MayRequireIdeology]
        public static IncidentDef GiveQuest_EndGame_ArchonexusVictory;

        [MayRequireOdyssey]
        public static IncidentDef GiveQuest_MechanoidSignal;
    }
}
