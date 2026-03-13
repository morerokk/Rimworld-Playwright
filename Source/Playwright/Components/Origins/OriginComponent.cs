using RimWorld;
using Rokk.Playwright.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public abstract class OriginComponent : PlaywrightComponent
    {
        /// <summary>
        /// How many colonists you can select at the start screen to take with you
        /// </summary>
        public virtual int StartingColonistsSelectable => 3;
        /// <summary>
        /// How many colonist choices are available to you total
        /// </summary>
        public virtual int StartingColonistsTotal => 8;
        /// <summary>
        /// How your colonists arrive on the planet
        /// </summary>
        public virtual PlayerPawnsArriveMethod ArrivalMethod => PlayerPawnsArriveMethod.DropPods;
        /// <summary>
        /// What the starting faction of the player is when they start the scenario (new arrivals, new tribe etc)
        /// </summary>
        public virtual FactionDef PlayerFaction => FactionDefOf.PlayerColony;

        public virtual string Summary
        {
            get
            {
                return "Playwright.Components." + this.Id + ".Summary";
            }
        }
        public virtual string SummaryTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Summary").Translate();
            }
        }

        public virtual string SuggestedIdeo
        {
            get
            {
                return "Playwright.Components." + this.Id + ".SuggestedIdeo";
            }
        }

        public virtual string SuggestedIdeoTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".SuggestedIdeo").Translate();
            }
        }

        /// <summary>
        /// Draw additional extra content for your origin here. This will be shown in the Playwright UI below the Origin's summary.
        /// This is only used for content. For settings, use <see cref="PlaywrightComponent.DoSettingsContents"/>.
        /// </summary>
        /// <param name="listing">The <see cref="Listing_Standard"/> that the UI is drawing in.</param>
        public virtual void DrawAdditionalContent(Listing_Standard listing)
        {

        }

        public static List<OriginComponent> GetAvailableOrigins()
        { 
            var origins = new List<OriginComponent>();
            origins.Add(new CrashlandedOrigin());
            origins.Add(new TribalOrigin());
            origins.Add(new RichExplorerOrigin());

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
