using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class AnomalyOrigin : OriginComponent
    {
        public override string Id => "Origins.Anomaly";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.TheAnomaly;
        public override bool IsAvailable => ModsConfig.AnomalyActive;
    }
}
