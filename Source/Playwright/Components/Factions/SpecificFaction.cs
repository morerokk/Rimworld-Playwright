using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Factions
{
    /// <summary>
    /// This is a placeholder that represents a generic choosable faction.
    /// </summary>
    public class SpecificFaction : FactionComponent
    {
        public override string Id => "Factions.Specific";

        /// <summary>
        /// The faction that this will apply to.
        /// </summary>
        public FactionDef Faction;

        public override FactionDef FactionDef => Faction;
        // Permanent enemies are permanent enemies, checkbox doesn't apply
        public override bool AllowForcedDisposition => FactionDef != null ? !FactionDef.permanentEnemy : true;

        // Since this is a non-specific faction, you are allowed to add multiple.
        public override int MaxInGroup => int.MaxValue;
        public override int MaxTotal => int.MaxValue;

        // Settings
        protected virtual IEnumerable<FactionDef> GetAllowedFactions(PlaywrightStructure playwright = null, FactionRelationKind? relationKind = null)
        {
            // Disallow choosing permanently hostile factions in the ally/neutral sections
            IEnumerable<FactionDef> factions;
            if (relationKind == FactionRelationKind.Hostile)
            {
                factions = FactionUtils.GetAllNpcFactions();
            }
            else
            {
                factions = FactionUtils.GetNotPermanentlyHostileFactions();
            }

            if (playwright != null)
            {
                // Filter out factions that were already chosen
                factions = factions
                    .Where(factionDef => !playwright.AllFactions.Any(factionComponent => factionComponent.FactionDef == factionDef));
            }

            return factions;
        }

        protected string FactionLabelText => Faction != null ? Faction.LabelCap.ToString() : "Playwright.Components.Factions.Specific.Faction.Select".Translate().ToString();

        public override void DoSettingsContents(Listing_AutoFitVertical factionContentListing, FactionRelationKind relationKind, PlaywrightStructure playwright)
        {
            if (factionContentListing.ButtonText(FactionLabelText))
            {
                var options = new List<FloatMenuOption>();
                var allowedFactions = GetAllowedFactions(playwright, relationKind);
                foreach (FactionDef factionDef in allowedFactions)
                {
                    options.Add(new FloatMenuOption(factionDef.LabelCap, () => Faction = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                PlaywrightUtils.OpenFloatMenu(options);
            }
            factionContentListing.Gap(5f);

            base.DoSettingsContents(factionContentListing, relationKind, playwright);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
        }
    }
}
