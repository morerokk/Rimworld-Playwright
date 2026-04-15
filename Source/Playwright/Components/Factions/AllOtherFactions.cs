using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rokk.Playwright.Composer;

namespace Rokk.Playwright.Components.Factions
{
    /// <summary>
    /// This is a placeholder that represents other factions that this mod did not account for.
    /// If you leave this OUT of the <see cref="PlaywrightStructure.OtherFactions"/> list for instance,
    /// all factions that aren't allies or enemies will be wiped off the map.
    /// By default, this is left inside all lists, and if left alone, it will simply leave the other factions alone.
    /// </summary>
    public class AllOtherFactions : FactionComponent
    {
        // Needs special treatment, ID exposed as constant
        public const string ComponentId = "Factions.AllOther";
        public override string Id => ComponentId;
        public override int MaxTotal => int.MaxValue;
        public override bool AllowForcedDisposition => false;

        public override int SortOrder => 1000;
    }
}
