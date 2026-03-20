using RimWorld;
using Rokk.Playwright.Components.Boons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Compat.VREAndroids.Components.Boons
{
    public class AndroidsBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_VREAndroids_Androids";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // TODO: Add extra androids to spawn in with
        }
    }
}
