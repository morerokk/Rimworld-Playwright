using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    /// <summary>
    /// Placeholder origin that lets the player select any scenario that is not covered by Playwright
    /// </summary>
    public class ImportOrigin : OriginComponent
    {
        public override string Id => "Origins.Import";
        public override ScenarioDef BasedOnScenario => Scenario;

        public ScenarioDef Scenario;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<ScenarioDef>(ref Scenario, nameof(Scenario));
        }
    }
}
