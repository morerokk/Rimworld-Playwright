using RimWorld;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    /// <summary>
    /// When a pawn is generated with <see cref="FromXenotype"/>, it will instead be replaced with the custom xenotype <see cref="ToCustomXenotype"/>.
    /// This is a bad idea in my opinion. Included in the Playwright designer with the recommendation to use a faction customizer instead, like WorldBuilder or Xenotype Spawn Control.
    /// Modifying pawns after generation is probably the approach that is least likely to break things. Patches would make this a nightmare.
    /// 
    /// Note: sometimes a generated pawn may need to be capable of violence (like faction leaders).
    /// The game will give up and ignore this ScenPart after ~100 regeneration attempts if that happens (and maybe ignore other ScenParts too!).
    /// </summary>
    public class ReplaceXenotypeWithCustom : ScenPart_PawnModifier
    {
        public XenotypeDef FromXenotype;
        public CustomXenotype ToCustomXenotype;

        private string FromXenotypeLabel => FromXenotype != null ? FromXenotype.LabelCap.ToString() : "-";
        private string ToCustomXenotypeLabel => ToCustomXenotype != null ? ToCustomXenotype.name : "-";

        public List<XenotypeDef> GetFromXenotypes()
        {
            return DefDatabase<XenotypeDef>.AllDefsListForReading;
        }

        public List<CustomXenotype> GetToXenotypes()
        {
            return new List<CustomXenotype>();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 9);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 9);

            helper.Skip(1);

            // From selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.ReplaceXenotypeWithCustom.FromXenotype".Translate());
            if (Widgets.ButtonText(helper.NextRect(), FromXenotypeLabel))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<XenotypeDef> allowedXenotypes = GetFromXenotypes();
                foreach (XenotypeDef xenotypeDef in allowedXenotypes)
                {
                    floatMenuOptions.Add(new FloatMenuOption(xenotypeDef.LabelCap, () => FromXenotype = xenotypeDef, xenotypeDef.Icon, Color.white));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            // To selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.ReplaceXenotypeWithCustom.ToCustomXenotype".Translate());
            if (Widgets.ButtonText(helper.NextRect(), ToCustomXenotypeLabel))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<CustomXenotype> allowedXenotypes = GetToXenotypes();
                foreach (CustomXenotype customXenotype in allowedXenotypes)
                {
                    floatMenuOptions.Add(new FloatMenuOption(customXenotype.name, () => ToCustomXenotype = customXenotype, customXenotype.IconDef.Icon, Color.white));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            base.DoPawnModifierEditInterface(helper.NextRect(2));

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.ReplaceXenotypeWithCustom.Help".Translate()));
            }
        }

        protected override void ModifyNewPawn(Pawn pawn)
        {
            ApplyToPawn(pawn);
        }

        protected override void ModifyHideOffMapStartingPawnPostMapGenerate(Pawn pawn)
        {
            ApplyToPawn(pawn);
        }

        private void ApplyToPawn(Pawn pawn)
        {
            Pawn_GeneTracker geneTracker = pawn.genes;
            if (geneTracker == null || geneTracker.hybrid || geneTracker.Xenotype != FromXenotype)
            {
                return;
            }

            if (ToCustomXenotype == null)
            {
                Log.Warning("[Playwright] Cannot apply ToCustomXenotype to pawn when generating pawn, ToCustomXenotype is null, skipping");
                return;
            }

            // Set pawn xenotype back to baseliner and then re-apply the new one, because paranoia can't hurt.
            // Custom xenotypes are second-class citizens, so to make this work,
            // we have to individually set each gene and manually set the name and icon (yes, really)
            geneTracker.SetXenotype(XenotypeDefOf.Baseliner);
            geneTracker.xenotypeName = ToCustomXenotype.name;
            geneTracker.iconDef = ToCustomXenotype.IconDef;
            foreach (GeneDef geneDef in ToCustomXenotype.genes)
            {
                geneTracker.AddGene(geneDef, !ToCustomXenotype.inheritable);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref FromXenotype, nameof(FromXenotype));
            Scribe_Values.Look(ref ToCustomXenotype, nameof(ToCustomXenotype));
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.ReplaceXenotypeWithCustom.Summary".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), FromXenotypeLabel, ToCustomXenotypeLabel).CapitalizeFirst();
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            ReplaceXenotypeWithCustom part = other as ReplaceXenotypeWithCustom;
            return part == null || this.FromXenotype != part.FromXenotype || this.ToCustomXenotype != part.ToCustomXenotype || this.context != part.context;
        }

        public override bool HasNullDefs()
        {
            return base.HasNullDefs() || FromXenotype == null || ToCustomXenotype == null;
        }
    }
}
