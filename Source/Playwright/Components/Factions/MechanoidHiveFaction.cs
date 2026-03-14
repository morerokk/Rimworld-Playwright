using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.Factions
{
    public class MechanoidHiveFaction : FactionComponent
    {
        public override string Id => "Factions.MechanoidHive";

        public override FactionDef FactionDef => FactionDefOf.Mechanoid;

        public override HashSet<FactionRelationKind> AllowedDispositions => new HashSet<FactionRelationKind>()
        {
            FactionRelationKind.Hostile
        };

        public override bool AllowForcedDisposition => false;

        public override void MutateScenario(Scenario scenario,List<ScenPart> scenarioParts)
        {
            
        }
    }
}
