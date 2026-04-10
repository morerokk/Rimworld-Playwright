using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.VREAndroids.Components.Origins
{
    public class AndroidsOrigin : OriginComponent
    {
        public override string Id => "Origins.Compat_VREAndroids_Androids";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.VREA_NewUtopia;

        public override List<WinConditionComponent> DefaultWinConditions => new List<WinConditionComponent>()
        {
            new ColonyWinCondition() { Colonists = 30 }
        };
    }
}
