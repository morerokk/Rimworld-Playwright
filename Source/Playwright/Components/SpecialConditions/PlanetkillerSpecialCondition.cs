using RimWorld;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class PlanetkillerSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.Planetkiller";

        public FloatRange Days = new FloatRange(90, 500);

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeGameConditionPart(DefOfs.GameConditionDefOf.Planetkiller, Days, true));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<FloatRange>(ref Days, nameof(Days));
        }
    }
}
