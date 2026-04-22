using RimWorld;
using Rokk.Playwright.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Compat.BSSapientAnimals.Components.Origins
{
    public class FailedExperimentOrigin : OriginComponent
    {
        // ID's must be unique, so prefix it with something distinct
        public override string Id => "Origins.Compat_BSSapientAnimals";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Playwright_BSSapientAnimals_FailedExperiment;
    }
}
