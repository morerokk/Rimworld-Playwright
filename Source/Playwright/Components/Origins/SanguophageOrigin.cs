using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class SanguophageOrigin : OriginComponent
    {
        public override string Id => "Origins.Sanguophage";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Sanguophage;
        public override bool IsAvailable => ModsConfig.BiotechActive;
    }
}
