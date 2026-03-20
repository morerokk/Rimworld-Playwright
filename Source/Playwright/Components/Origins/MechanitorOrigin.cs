using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class MechanitorOrigin : OriginComponent
    {
        public override string Id => "Origins.Mechanitor";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Mechanitor;
        public override bool IsAvailable => ModsConfig.BiotechActive;
    }
}
