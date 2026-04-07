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
    /// When a pawn is generated with <see cref="FromXenotype"/>, it will instead be replaced with <see cref="ToXenotype"/>.
    /// This is a bad idea in my opinion. Not included in the Playwright designer because then the player should just use a faction control/xenotype control mod.
    /// Modifying pawns after generation is probably the approach that is least likely to break things. Patches would make this a nightmare.
    /// 
    /// Note: sometimes a generated pawn may need to be capable of violence (like faction leaders).
    /// The game will give up and ignore this ScenPart after ~100 regeneration attempts if that happens (and maybe ignore other ScenParts too!).
    /// </summary>
    public class ReplaceXenotype : ScenPart_PawnModifier
    {
        public XenotypeDef FromXenotype;
        public XenotypeDef ToXenotype;

        private string FromXenotypeLabel => FromXenotype != null ? FromXenotype.LabelCap.ToString() : "-";
        private string ToXenotypeLabel => ToXenotype != null ? ToXenotype.LabelCap.ToString() : "-";

        public List<XenotypeDef> GetFromXenotypes()
        {
            return DefDatabase<XenotypeDef>.AllDefsListForReading;
        }

        public List<XenotypeDef> GetToXenotypes()
        {
            // Initially considered only allowing a "To" xenotype that matched the From's xenotype/endotype-ness (inheritable yes/no),
            // but that is probably unnecessary since we don't mess with hybrids
            return DefDatabase<XenotypeDef>.AllDefsListForReading;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 9);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 9);

            helper.Skip(1);

            // From selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.ReplaceXenotype.FromXenotype".Translate());
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
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.ReplaceXenotype.ToXenotype".Translate());
            if (Widgets.ButtonText(helper.NextRect(), ToXenotypeLabel))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<XenotypeDef> allowedXenotypes = GetToXenotypes();
                foreach (XenotypeDef xenotypeDef in allowedXenotypes)
                {
                    floatMenuOptions.Add(new FloatMenuOption(xenotypeDef.LabelCap, () => ToXenotype = xenotypeDef, xenotypeDef.Icon, Color.white));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            base.DoPawnModifierEditInterface(helper.NextRect(2));

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.ReplaceXenotype.Help".Translate()));
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

            // Set pawn xenotype back to baseliner and then re-apply the new one, because paranoia and it can't hurt.
            // Thankfully we don't need to support custom xenotypes here (surely :) ).
            // If players want to use a custom xenotype here, recommend using a mod/tool that exports a custom xenotype to a new mod,
            // so that it's no longer a second-class citizen (which you *can* do)
            geneTracker.SetXenotype(XenotypeDefOf.Baseliner);
            geneTracker.SetXenotype(ToXenotype);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref FromXenotype, nameof(FromXenotype));
            Scribe_Defs.Look(ref ToXenotype, nameof(ToXenotype));
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.ReplaceXenotype.Summary".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), FromXenotypeLabel, ToXenotypeLabel).CapitalizeFirst();
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            ReplaceXenotype part = other as ReplaceXenotype;
            return part == null || this.FromXenotype != part.FromXenotype || this.ToXenotype != part.ToXenotype || this.context != part.context;
        }

        public override bool HasNullDefs()
        {
            return base.HasNullDefs() || FromXenotype == null || ToXenotype == null;
        }
    }
}
