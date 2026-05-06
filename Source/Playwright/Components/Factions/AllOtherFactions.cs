using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rokk.Playwright.Composer;

namespace Rokk.Playwright.Components.Factions
{
    /// <summary>
    /// This is a placeholder that represents all other factions that weren't selected.
    /// If you leave this OUT of the <see cref="PlaywrightStructure.NeutralFactions"/> list for instance,
    /// all factions that are neutral will be wiped off the map.
    /// By default, this is left inside all lists, and if left alone, it will simply leave the other factions alone.
    /// </summary>
    public class AllOtherFactions : FactionComponent
    {
        // Needs special treatment, ID exposed as constant
        public const string ComponentId = "Factions.AllOther";
        public override string Id => ComponentId;
        public override int MaxTotal => int.MaxValue;
        public override bool AllowForcedDisposition => false;
        public override HashSet<FactionRelationKind> AllowedDispositions => new HashSet<FactionRelationKind>()
        {
            FactionRelationKind.Neutral,
            FactionRelationKind.Hostile
        };

        public override int SortOrder => 1000;
    }
}
