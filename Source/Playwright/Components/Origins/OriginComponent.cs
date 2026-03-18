using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public abstract class OriginComponent : PlaywrightComponent
    {
        /// <summary>
        /// The scenario that this origin uses as a base to copy everything from.
        /// Defaults to null, which results in a "default-ish" scenario (Naked Brutality, but without the naked/no possessions part).
        /// </summary>
        public virtual ScenarioDef BasedOnScenario => null;
        /// <summary>
        /// How many colonists you can select at the start screen to take with you.
        /// If null, uses the one from <see cref="BasedOnScenario"/>.
        /// </summary>
        public virtual int? StartingColonistsSelectable => null;
        /// <summary>
        /// How many colonist choices are available to you total.
        /// If null, uses the one from <see cref="BasedOnScenario"/>.
        /// </summary>
        public virtual int? StartingColonistsTotal => null;
        /// <summary>
        /// How your colonists arrive on the planet.
        /// If null, uses the one from <see cref="BasedOnScenario"/>.
        /// </summary>
        public virtual PlayerPawnsArriveMethod? ArrivalMethod => null;
        /// <summary>
        /// What the starting faction of the player is when they start the scenario (new arrivals, new tribe etc).
        /// If null, uses the one from <see cref="BasedOnScenario"/>.
        /// </summary>
        public virtual FactionDef PlayerFaction => null;

        public override string Description => BasedOnScenario != null ? "-" : base.Description;
        public override string DescriptionTranslated => BasedOnScenario != null ? BasedOnScenario.scenario.description : base.DescriptionTranslated;

        /// <summary>
        /// Summary is used for summarizing the starting conditions (start with X, you're enemies with Y, etc).
        /// Not every Origin is based on an existing scenario, so for that to work, you will have to provide a summary text.
        /// </summary>
        public virtual string SummaryTranslated => BasedOnScenario != null ? PlaywrightUtils.GetScenarioSummary(BasedOnScenario.scenario) : ("Playwright.Components." + this.Id + ".Summary").Translate().ToString();

        public virtual string SuggestedIdeo => "Playwright.Components." + this.Id + ".SuggestedIdeo";
        public virtual string SuggestedIdeoTranslated => SuggestedIdeo.Translate();

        /// <summary>
        /// Used for the Scenario's tagline underneath the name.
        /// If null, the base scenario's tagline is used.
        /// </summary>
        public virtual string DescriptionShortTranslated => BasedOnScenario != null ? BasedOnScenario.scenario.summary : ("Playwright.Components." + this.Id + ".Description.Short").Translate().ToString();

        /// <summary>
        /// Required for drawing additional content. If you use <see cref="DoAdditionalContents"/>, you should increase this number to fit your content.
        /// </summary>
        public virtual float AdditionalContentsHeight => 0f;

        /// <summary>
        /// Draw additional extra content for your origin here. This will be shown in the Playwright UI below the Origin's summary.
        /// This is only used for content. For settings, use <see cref="OriginComponent.DoSettingsContents"/>.
        /// </summary>
        /// <param name="listing">The <see cref="Listing_Standard"/> that the UI is drawing in.</param>
        /// <param name="inRect">The whole <see cref="Rect"/> that the Origin UI is drawing in. Spans the whole origin box, not just the listing contents, be careful!</param>
        public virtual void DoAdditionalContents(Listing_Standard listing, Rect inRect)
        {

        }

        public virtual void DoSettingsContents(Rect inRect)
        {

        }

        public static List<OriginComponent> GetAvailableOrigins()
        { 
            var origins = new List<OriginComponent>();
            origins.Add(new CrashlandedOrigin());
            origins.Add(new TribalOrigin());
            origins.Add(new RichExplorerOrigin());
            origins.Add(new NakedBrutalityOrigin());

            OriginComponent empire = new EmpireOrigin();
            if (empire.IsAvailable)
            {
                origins.Add(empire);
            }

            foreach (OriginComponent origin in ComponentRegistration.Origins)
            {
                if (origin.IsAvailable)
                {
                    origins.Add(origin);
                }
            }

            return origins;
        }
    }
}
