using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Factions
{
    public abstract class FactionComponent : PlaywrightComponent
    {
        /// <summary>
        /// This faction's def. Can be null if this doesn't apply, but if your component is for a specific faction, this should be rare.
        /// </summary>
        public virtual FactionDef FactionDef => null;

        /// <summary>
        /// What dispositions this faction is allowed to have during selection.
        /// This should usually be "anything", but insectoids and mechanoids don't really work if they're not always the default of hostile.
        /// </summary>
        public virtual HashSet<FactionRelationKind> AllowedDispositions => new HashSet<FactionRelationKind>()
        {
            FactionRelationKind.Neutral,
            FactionRelationKind.Hostile,
            FactionRelationKind.Ally
        };

        /// <summary>
        /// Whether this faction allows the disposition to be forced (as opposed to natural)
        /// </summary>
        public virtual bool AllowForcedDisposition => true;

        /// <summary>
        /// Whether their disposition should be forced or natural.
        /// </summary>
        public bool ForceDisposition = false;

        /// <summary>
        /// How many of this faction can be added within a single group (Allies, Enemies, Neutral).
        /// This should usually be 1, but some factions are placeholders so they might be added multiple times.
        /// </summary>
        public virtual int MaxInGroup => 1;
        /// <summary>
        /// How many of this faction can be added in total, across all groups.
        /// This should usually be 1, but some factions are placeholders so they might be added multiple times.
        /// </summary>
        public virtual int MaxTotal => 1;
        /// <summary>
        /// The sort order of this faction in the faction listing window.
        /// Useful for placeholder factions that should always go last, for example.
        /// </summary>
        public virtual int SortOrder => 0;

        public virtual void DoFactionContents(Listing_AutoFitVertical factionContentListing, FactionRelationKind relationKind)
        {
            factionContentListing.Label(NameTranslated);
            factionContentListing.Gap(5f);
            DoSettingsContents(factionContentListing, relationKind);
        }

        /// <summary>
        /// Render the UI for the settings in your component.
        /// </summary>
        /// <param name="relationKind">
        /// The <see cref="FactionRelationKind"/> that the player is currently configuring.
        /// Can be used to disallow certain factions outside of the Enemies list, for instance.
        /// </param>
        public virtual void DoSettingsContents(Listing_AutoFitVertical factionContentListing, FactionRelationKind relationKind)
        {
            if (AllowForcedDisposition)
            {
                factionContentListing.CheckboxLabeled("Playwright.Components.Faction.Specific.ForcedDisposition".Translate(), ref ForceDisposition, "Playwright.Components.Faction.Specific.ForcedDisposition.Help".Translate());
            }
        }

        public static List<FactionComponent> GetAvailableFactions()
        {
            var list = new List<FactionComponent>();
            list.Add(new AllOtherFactions());
            list.Add(new SpecificFaction());
            list.Add(new InsectoidHiveFaction());
            list.Add(new MechanoidHiveFaction());

            foreach (var faction in ComponentRegistration.Factions)
            {
                if (faction.IsAvailable)
                {
                    list.Add(faction);
                }
            }

            return list;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref ForceDisposition, nameof(ForceDisposition), false);
        }
    }
}
