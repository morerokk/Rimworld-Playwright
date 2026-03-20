using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class EmpireOrigin : OriginComponent
    {
        public override string Id => "Origins.Empire";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Playwright_Empire;
    }
}
