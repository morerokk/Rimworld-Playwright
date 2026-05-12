using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Components.Origins
{
    public class CrashlandedOrigin : OriginComponent
    {
        public override string Id => "Origins.Crashlanded";
        public override ScenarioDef BasedOnScenario => RimWorld.ScenarioDefOf.Crashlanded;
    }
}
