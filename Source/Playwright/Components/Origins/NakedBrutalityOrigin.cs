using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Components.Origins
{
    public class NakedBrutalityOrigin : OriginComponent
    {
        public override string Id => "Origins.NakedBrutality";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.NakedBrutality;
    }
}
