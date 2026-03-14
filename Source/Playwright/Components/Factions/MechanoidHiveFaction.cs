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

        public override HashSet<FactionDisposition> AllowedDispositions => new HashSet<FactionDisposition>()
        {
            FactionDisposition.Default,
            FactionDisposition.AlwaysHostile
        };

        public override void MutateScenario(Scenario scenario,List<ScenPart> scenarioParts)
        {
            
        }
    }
}
