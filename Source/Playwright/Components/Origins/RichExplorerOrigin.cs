using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rokk.Playwright.Components.Origins
{
    public class RichExplorerOrigin : OriginComponent
    {
        public override string Id => "Origins.RichExplorer";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.TheRichExplorer;
    }
}
