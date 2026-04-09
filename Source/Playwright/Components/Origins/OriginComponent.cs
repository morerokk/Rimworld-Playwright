using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Composer;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static System.Collections.Specialized.BitVector32;

namespace Rokk.Playwright.Components.Origins
{
    /// <summary>
    /// Defines the starting point of the scenario.
    /// Usually, an Origin is a starting scenario + default components (like boons or factions).
    /// However, origins do not need to be based on a scenario.
    /// </summary>
    public abstract class OriginComponent : PlaywrightComponent
    {
        /// <summary>
        /// The scenario that this origin uses as a base to copy everything from.
        /// Defaults to null, which results in a "default-ish" scenario (Naked Brutality, but without the naked/no possessions part).
        /// Basing your Origin on a (hidden) scenario is recommended.
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
        /// Recommended to be <see cref="FactionDefOf.PlayerColony"/> in most cases.
        /// </summary>
        public virtual FactionDef PlayerFaction => null;

        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Boons.
        /// </summary>
        public virtual List<BoonComponent> DefaultBoons => null;
        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Factions as Allies.
        /// </summary>
        public virtual List<FactionComponent> DefaultAllies => null;
        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Factions as Neutrals.
        /// </summary>
        public virtual List<FactionComponent> DefaultNeutrals => null;
        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Factions as Enemies.
        /// </summary>
        public virtual List<FactionComponent> DefaultEnemies => null;
        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Win Conditions.
        /// </summary>
        public virtual List<WinConditionComponent> DefaultWinConditions => null;
        /// <summary>
        /// If not null, selecting this Origin will also auto-set the specified Special Conditions.
        /// </summary>
        public virtual List<SpecialConditionComponent> DefaultSpecialConditions => null;

        public override string Description => BasedOnScenario != null ? "-" : base.Description;
        public override string DescriptionTranslated => BasedOnScenario != null ? BasedOnScenario.scenario.description : base.DescriptionTranslated;

        /// <summary>
        /// Summary is used for summarizing the starting conditions (your faction is new arrivals, start with X, you're enemies with Y, etc).
        /// If <see cref="BasedOnScenario"/> is not null, defaults to that scenario's summary text.
        /// If your Origin is not based on a scenario, you will have to provide a summary text.
        /// </summary>
        public virtual string SummaryTranslated => BasedOnScenario != null ? PlaywrightUtils.GetScenarioSummary(BasedOnScenario.scenario) : ("Playwright.Components." + this.Id + ".Summary").Translate().ToString();

        // TODO: Do we still want this? I feel like we should try to make this work
        public virtual string SuggestedIdeo => "Playwright.Components." + this.Id + ".SuggestedIdeo";
        public virtual string SuggestedIdeoTranslated => SuggestedIdeo.Translate();

        /// <summary>
        /// Used for the Scenario's tagline, underneath the name ("3 crashlanded survivors.").
        /// If null, the base scenario's tagline is used.
        /// </summary>
        public virtual string DescriptionShortTranslated => BasedOnScenario != null ? BasedOnScenario.scenario.summary : ("Playwright.Components." + this.Id + ".Description.Short").Translate().ToString();

        /// <summary>
        /// Contents that come before the origin content.
        /// Useful for creating settings that might completely change the description (like ImportOrigin).
        /// </summary>
        /// <param name="originContentListing">The listing that the origin contents are drawing in.</param>
        public virtual void DoPreContents(Listing_AutoFitVertical originContentListing)
        {

        }

        /// <summary>
        /// Additional content for the origin, like flavor text.
        /// By default, this lists out the origin's extra default-selected boons/factions/etc.
        /// </summary>
        /// <param name="originContentListing">The listing that the origin contents are drawing in.</param>
        public virtual void DoAdditionalContents(Listing_AutoFitVertical originContentListing)
        {
            if (DefaultBoons != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultBoons".Translate());
                foreach (BoonComponent boon in DefaultBoons)
                {
                    sb.Append(Environment.NewLine + "- " + boon.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            if (DefaultAllies != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultAllies".Translate());
                foreach (FactionComponent faction in DefaultAllies)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            if (DefaultNeutrals != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultNeutrals".Translate());
                foreach (FactionComponent faction in DefaultNeutrals)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            if (DefaultEnemies != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultEnemies".Translate());
                foreach (FactionComponent faction in DefaultEnemies)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            if (DefaultWinConditions != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultWinConditions".Translate());
                foreach (WinConditionComponent winCondition in DefaultWinConditions)
                {
                    sb.Append(Environment.NewLine + "- " + winCondition.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            if (DefaultSpecialConditions != null)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultSpecialConditions".Translate());
                foreach (SpecialConditionComponent specialCondition in DefaultSpecialConditions)
                {
                    sb.Append(Environment.NewLine + "- " + specialCondition.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }
        }

        /// <summary>
        /// Settings for the origin.
        /// </summary>
        /// <param name="originContentListing">The listing that the origin contents are drawing in.</param>
        public virtual void DoSettingsContents(Listing_AutoFitVertical originContentListing)
        {

        }

        public static List<OriginComponent> GetAvailableOrigins()
        { 
            var origins = new List<OriginComponent>();
            // Base game stuff
            origins.Add(new CrashlandedOrigin());
            origins.Add(new TribalOrigin());
            origins.Add(new RichExplorerOrigin());
            origins.Add(new NakedBrutalityOrigin());

            // DLC stuff
            OriginComponent mechanitor = new MechanitorOrigin();
            if (mechanitor.IsAvailable)
            {
                origins.Add(mechanitor);
            }
            OriginComponent sanguophage = new SanguophageOrigin();
            if (sanguophage.IsAvailable)
            {
                origins.Add(sanguophage);
            }
            OriginComponent anomaly = new AnomalyOrigin();
            if (anomaly.IsAvailable)
            {
                origins.Add(anomaly);
            }
            OriginComponent gravship = new GravshipOrigin();
            if (gravship.IsAvailable)
            {
                origins.Add(gravship);
            }

            // Playwright stuff
            OriginComponent empire = new EmpireOrigin();
            if (empire.IsAvailable)
            {
                origins.Add(empire);
            }

            // Stuff from addons
            foreach (OriginComponent origin in ComponentRegistration.Origins)
            {
                if (origin.IsAvailable)
                {
                    origins.Add(origin);
                }
            }

            // Custom placeholder origin, selected from scenario
            origins.Add(new ImportOrigin());

            return origins;
        }
    }
}
