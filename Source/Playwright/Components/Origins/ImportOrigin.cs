using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
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
        public const string ComponentId = "Origins.Import";
        public override string Id => ComponentId;
        public override ScenarioDef BasedOnScenario => Scenario;

        /// <summary>
        /// Player's selected scenario to import from
        /// </summary>
        public ScenarioDef Scenario;

        private string ScenarioLabel => Scenario != null ? Scenario.LabelCap.ToString() : "-";

        /// <summary>
        /// A list of scenario defs that shouldn't appear in the choices menu, as they would be redundant
        /// </summary>
        protected virtual List<string> ScenarioDefsToSkip => new List<string>()
        {
            "Crashlanded",
            "LostTribe",
            "TheRichExplorer",
            "NakedBrutality",
            "Mechanitor",
            "Sanguophage",
            "TheAnomaly",
            "TheGravship"
        };

        public override void DoPreContents(Listing_AutoFitVertical originContentListing)
        {
            if (originContentListing.ButtonTextLabeled("Playwright.Components.Origins.Import.Scenario".Translate(), ScenarioLabel))
            {
                List<string> defsToSkip = ScenarioDefsToSkip;
                // If the scenario appears in an origin that's been registered by an addon, hide it, too
                foreach (OriginComponent origin in ComponentRegistration.Origins)
                {
                    if (origin.BasedOnScenario != null)
                    {
                        defsToSkip.Add(origin.BasedOnScenario.defName);
                    }
                }

                List<ScenarioDef> availableScenarios = DefDatabase<ScenarioDef>.AllDefsListForReading
                    .Where(def => !defsToSkip.Contains(def.defName) && def.scenario.showInUI)
                    .ToList();

                var options = new List<FloatMenuOption>();
                foreach (ScenarioDef scenario in availableScenarios)
                {
                    options.Add(new FloatMenuOption(scenario.LabelCap, () =>
                    {
                        this.Scenario = scenario;
                        originContentListing.Invalidate();
                    }));
                }

                PlaywrightUtils.OpenFloatMenu(options);
            }
        }

        public override void DoAdditionalContents(Listing_AutoFitVertical originContentListing)
        {
            // base intentionally not called.
            // This origin will never have default components, and listing the tech level is a bit dicey
            // because I don't know what people might do with scenarios.
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Scenario, nameof(Scenario));
        }
    }
}
