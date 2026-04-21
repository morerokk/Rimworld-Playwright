using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Extensions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using Verse;

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
        /// If not empty, selecting this Origin will also auto-set the specified Boons.
        /// </summary>
        public virtual List<BoonComponent> DefaultBoons => new List<BoonComponent>();
        /// <summary>
        /// If not empty, selecting this Origin will also auto-set the specified Factions as Allies.
        /// </summary>
        public virtual List<FactionComponent> DefaultAllies => new List<FactionComponent>();
        /// <summary>
        /// If not empty, selecting this Origin will also auto-set the specified Factions as Neutrals.
        /// </summary>
        public virtual List<FactionComponent> DefaultNeutrals => new List<FactionComponent>();
        /// <summary>
        /// If not empty, selecting this Origin will also auto-set the specified Factions as Enemies.
        /// </summary>
        public virtual List<FactionComponent> DefaultEnemies => new List<FactionComponent>();
        /// <summary>
        /// If not empty, selecting this Origin will also auto-set the specified Win Conditions.
        /// </summary>
        public virtual List<WinConditionComponent> DefaultWinConditions => new List<WinConditionComponent>();
        /// <summary>
        /// If not empty, selecting this Origin will also auto-set the specified Special Conditions.
        /// </summary>
        public virtual List<SpecialConditionComponent> DefaultSpecialConditions => new List<SpecialConditionComponent>();

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
        public override string DescriptionShortTranslated => BasedOnScenario != null ? BasedOnScenario.scenario.summary : base.DescriptionShortTranslated;

        /// <summary>
        /// Get the tech level of the current origin.
        /// Read-only, used to display the tech level in the Origin selection screen.
        /// Actual ingame tech level is based on the player faction's tech level.
        /// </summary>
        /// I'm not a rocket scientist. I'm not going to mess with tech levels, as tempting as it is to add a selector for it.
        /// Add a selector to override the player faction instead? I'll leave that up to the origins if they want to.
        public virtual TechLevel TechLevel
        {
            get
            {
                FactionDef playerFaction = PlayerFaction;
                if (playerFaction != null)
                {
                    return playerFaction.techLevel;
                }
                ScenarioDef basedOnScenario = BasedOnScenario;
                if (basedOnScenario != null)
                {
                    ScenPart_PlayerFaction playerFactionPart = basedOnScenario.scenario.AllParts
                        .Where(part => part.def == ScenPartDefOf.PlayerFaction)
                        .Cast<ScenPart_PlayerFaction>()
                        .FirstOrDefault();
                    if (playerFactionPart == null)
                    {
                        return TechLevel.Undefined;
                    }
                    FieldInfo factionDefInfo = AccessTools.Field(typeof(ScenPart_PlayerFaction), "factionDef");
                    playerFaction = factionDefInfo.GetValue(playerFactionPart) as FactionDef;
                    if (playerFaction == null)
                    {
                        return TechLevel.Undefined;
                    }
                    return playerFaction.techLevel;
                }
                return TechLevel.Undefined;
            }
        }

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
        /// By default, this lists out the origin's tech level, and its extra default-selected boons/factions/etc.
        /// </summary>
        /// <param name="originContentListing">The listing that the origin contents are drawing in.</param>
        public virtual void DoAdditionalContents(Listing_AutoFitVertical originContentListing)
        {
            originContentListing.Label("Playwright.TechLevel".Translate() + " " + ("TechLevel_" + this.TechLevel.ToString()).Translate().CapitalizeFirst());

            var defaultBoons = OriginDefaultsRegistration.GetDefaultBoons(Id)
                .ExecuteFuncs()
                .Union(DefaultBoons)
                .ToList();
            if (defaultBoons.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultBoons".Translate());
                foreach (BoonComponent boon in defaultBoons)
                {
                    sb.Append(Environment.NewLine + "- " + boon.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            List<FactionComponent> defaultAllies = OriginDefaultsRegistration.GetDefaultAllies(Id)
                .ExecuteFuncs()
                .Union(DefaultAllies)
                .ToList();
            if (defaultAllies.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultAllies".Translate());
                foreach (FactionComponent faction in defaultAllies)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            List<FactionComponent> defaultNeutrals = OriginDefaultsRegistration.GetDefaultNeutrals(Id)
                .ExecuteFuncs()
                .Union(DefaultNeutrals)
                .ToList();
            if (defaultNeutrals.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultNeutrals".Translate());
                foreach (FactionComponent faction in defaultNeutrals)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            List<FactionComponent> defaultEnemies = OriginDefaultsRegistration.GetDefaultEnemies(Id)
                .ExecuteFuncs()
                .Union(DefaultEnemies)
                .ToList();
            if (DefaultEnemies.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultEnemies".Translate());
                foreach (FactionComponent faction in defaultEnemies)
                {
                    sb.Append(Environment.NewLine + "- " + faction.FactionDef?.LabelCap.ToString() ?? faction.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            List<WinConditionComponent> defaultWinConditions = OriginDefaultsRegistration.GetDefaultWinConditions(Id)
                .ExecuteFuncs()
                .Union(DefaultWinConditions)
                .ToList();
            if (defaultWinConditions.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultWinConditions".Translate());
                foreach (WinConditionComponent winCondition in defaultWinConditions)
                {
                    sb.Append(Environment.NewLine + "- " + winCondition.NameTranslated);
                }
                originContentListing.Label(sb.ToString());
            }

            List<SpecialConditionComponent> defaultSpecialConditions = OriginDefaultsRegistration.GetDefaultSpecialConditions(Id)
                .ExecuteFuncs()
                .Union(DefaultSpecialConditions)
                .ToList();
            if (defaultSpecialConditions.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Playwright.DefaultSpecialConditions".Translate());
                foreach (SpecialConditionComponent specialCondition in defaultSpecialConditions)
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
            var result = new List<OriginComponent>();

            var playwrightOrigins = new List<OriginComponent>()
            {
                // Base game
                new CrashlandedOrigin(),
                new TribalOrigin(),
                new RichExplorerOrigin(),
                new NakedBrutalityOrigin(),

                // DLC
                new MechanitorOrigin(),
                new SanguophageOrigin(),
                new AnomalyOrigin(),
                new GravshipOrigin(),

                // Playwright
                new EmpireOrigin()
            };

            foreach (OriginComponent origin in playwrightOrigins)
            {
                if (origin.IsAvailable)
                {
                    result.Add(origin);
                }
            }

            // Stuff from addons
            foreach (OriginComponent origin in ComponentRegistration.Origins)
            {
                if (origin.IsAvailable)
                {
                    result.Add(origin);
                }
            }

            // Custom origin
            result.Add(new CustomOrigin());

            // Placeholder origin, selected from scenario, should always be at the bottom
            result.Add(new ImportOrigin());

            return result;
        }
    }
}
