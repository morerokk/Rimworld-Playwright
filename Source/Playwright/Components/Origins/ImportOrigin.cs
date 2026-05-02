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

        // Change origin name and descriptions to ensure the correct text is in the summary
        public override string NameTranslated => Scenario != null ? Scenario.scenario.name : base.NameTranslated;
        public override string DescriptionTranslated => Scenario != null ? Scenario.scenario.description : base.DescriptionTranslated;
        public override string DescriptionShortTranslated => Scenario != null ? Scenario.scenario.summary : base.DescriptionShortTranslated;

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

        protected virtual IEnumerable<ScenarioDef> GetAvailableScenarios(IEnumerable<string> defNamesToSkip = null)
        {
            var result = DefDatabase<ScenarioDef>.AllDefsListForReading
                    .Where(def => def.scenario.showInUI);
            if (defNamesToSkip != null)
            {
                result = result.Where(def => !defNamesToSkip.Contains(def.defName));
            }
            return result;
        }

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

                var options = new List<FloatMenuOption>();
                foreach (var scenarioDef in GetAvailableScenarios())
                {
                    if (defsToSkip.Contains(scenarioDef.defName))
                    {
                        continue;
                    }
                    options.Add(new FloatMenuOption(scenarioDef.LabelCap, () =>
                    {
                        this.Scenario = scenarioDef;
                        originContentListing.InvalidateGroup();
                    }));
                }

                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
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
