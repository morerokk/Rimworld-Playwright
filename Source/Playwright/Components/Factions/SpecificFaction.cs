using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
        protected virtual List<FactionDef> GetAllowedFactions(FactionRelationKind? relationKind = null)
        {
            var factions = DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden);
            if (relationKind != null && relationKind != FactionRelationKind.Hostile)
            {
                factions = factions.Where(def => !def.permanentEnemy);
            }
            return factions.ToList();
        }

        public override float SettingsHeight => 100f;

        protected string FactionLabelText => Faction != null ? Faction.LabelCap.ToString() : "Playwright.Components.Factions.Specific.Faction.Select".Translate().ToString();

        public override void DoSettingsContents(Rect inRect, FactionRelationKind relationKind)
        {
            Rect currentRect = new Rect(inRect);

            // Label
            currentRect = PlaywrightDrawHelper.NextLabelTranslated(currentRect, "Playwright.Components.Factions.Specific.Faction");

            // Faction selector button
            Rect buttonRect = new Rect(currentRect);
            buttonRect.height *= 0.5f;
            if (Widgets.ButtonText(buttonRect, FactionLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions(relationKind);
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => Faction = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
            }

            if (AllowForcedDisposition)
            {
                // Is forced
                Rect checkboxRect = new Rect(buttonRect);
                checkboxRect.y += checkboxRect.height;
                Widgets.CheckboxLabeled(checkboxRect, "Playwright.Components.Faction.Specific.ForcedDisposition".Translate(), ref ForceDisposition, !AllowForcedDisposition);
                if (Mouse.IsOver(checkboxRect))
                {
                    TooltipHandler.TipRegion(checkboxRect, "Playwright.Components.Faction.Specific.ForcedDisposition.Help".Translate());
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
        }
    }
}
