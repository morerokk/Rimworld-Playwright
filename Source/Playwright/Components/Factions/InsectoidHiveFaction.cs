using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.Factions
{
    public class InsectoidHiveFaction : FactionComponent
    {
        public const string ComponentId = "Factions.InsectoidHive";
        public override string Id => ComponentId;

        public override FactionDef FactionDef => FactionDefOf.Insect;

        public override HashSet<FactionRelationKind> AllowedDispositions => new HashSet<FactionRelationKind>()
        {
            FactionRelationKind.Hostile
        };

        public override bool AllowForcedDisposition => false;

        public override int SortOrder => 900;
    }
}
