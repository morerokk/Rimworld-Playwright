using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class GravshipOrigin : OriginComponent
    {
        public override string Id => "Origins.Gravship";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.TheGravship;
        public override bool IsAvailable => ModsConfig.OdysseyActive;
    }
}
