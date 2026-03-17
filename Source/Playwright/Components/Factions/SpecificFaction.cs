using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.UI;
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

        // Since this is a non-specific faction, you are allowed to add multiple.
        // However, the same faction may still not be added twice.
        public override int MaxInGroup => int.MaxValue;
        public override int MaxTotal => int.MaxValue;

        // Settings
        protected virtual List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer)
                .ToList();
        }

        public override float SettingsHeight => 100f;

        protected string FactionLabelText => Faction != null ? Faction.LabelCap.ToString() : "Playwright.Components.Factions.Specific.Faction.Select".Translate().ToString();

        public override void DoSettingsContents(Rect inRect)
        {
            Rect currentRect = new Rect(inRect);

            // Label
            currentRect = PlaywrightDrawHelper.NextLabel(currentRect, "Playwright.Components.Factions.Specific.Faction");

            // Faction selector button
            Rect buttonRect = new Rect(currentRect);
            buttonRect.height *= 0.5f;
            if (Widgets.ButtonText(buttonRect, FactionLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => Faction = factionDef));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            // Is forced
            Rect checkboxRect = new Rect(buttonRect);
            checkboxRect.y += checkboxRect.height;
            Widgets.CheckboxLabeled(checkboxRect, "Playwright.Components.Faction.Specific.ForcedDisposition".Translate(), ref ForceDisposition, !AllowForcedDisposition);
            if (Mouse.IsOver(checkboxRect))
            {
                TooltipHandler.TipRegion(checkboxRect, "Playwright.Components.Faction.Specific.ForcedDisposition.Help".Translate());
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Faction, nameof(Faction));
        }
    }
}
