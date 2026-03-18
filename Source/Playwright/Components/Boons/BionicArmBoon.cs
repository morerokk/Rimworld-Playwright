using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class BionicArmBoon : BoonComponent
    {
        public override string Id => "Boons.BionicArm";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {            
            scenarioParts.Add(ScenPartUtility.MakeForcedImplantPart(DefOfs.HediffDefOf.BionicArm, BodyPartDefOf.Arm, ForcedImplant.ImplantSide.Random));
        }
    }
}
