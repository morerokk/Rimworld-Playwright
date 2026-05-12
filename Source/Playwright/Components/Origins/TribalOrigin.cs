using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Components.Origins
{
    public class TribalOrigin : OriginComponent
    {
        public override string Id => "Origins.Tribal";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.LostTribe;
    }
}
