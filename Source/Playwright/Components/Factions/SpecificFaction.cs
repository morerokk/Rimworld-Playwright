using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rokk.Playwright.Composer;
using Verse;

namespace Rokk.Playwright.Components.Factions
{
    /// <summary>
    /// This is a placeholder that represents a generic choosable faction.
    /// </summary>
    public class SpecificFaction : FactionComponent
    {
        public override string Id => "Factions.Specific";

        /// <summary>
        /// The faction that this will apply to.
        /// </summary>
        public FactionDef Faction;

        /// <summary>
        /// Whether their disposition should be forced or natural.
        /// </summary>
        public bool ForceDisposition = false;

        public override FactionDef FactionDef => Faction;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
            Scribe_Values.Look<bool>(ref ForceDisposition, nameof(ForceDisposition), false);
        }
    }
}
