using HarmonyLib;
using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    // Much of this is copied from ScenPart_ForcedHediff.
    public class ForcedImplant : ScenPart_PawnModifier
    {
        public HediffDef Hediff;
        public BodyPartDef BodyPart;
        public ImplantSide Side;

        protected virtual List<BodyPartDef> BodyPartsThisCanApplyTo
        {
            get
            {
                var list = new List<BodyPartDef>();
                if (Hediff == null)
                {
                    return list;
                }

                RecipeDef recipe = DefDatabase<RecipeDef>.AllDefsListForReading
                    .FirstOrDefault(r => r.addsHediff == Hediff);
                if (recipe == null)
                {
                    return list;
                }

                return recipe.appliedOnFixedBodyParts;
            }
        }

        // Left/Right side mirrored tag is not implemented on all mirrored parts, have to make some exceptions
        protected virtual bool HasRightSide
        {
            get
            {
                if (Hediff == null)
                {
                    return false;
                }

                if (BodyPart == null)
                {
                    return false;
                }

                if (BodyPart.IsMirroredPart)
                {
                    return true;
                }

                if (BodyPart.defName == "Hand" || BodyPart.defName == "Leg")
                {
                    return true;
                }

                return false;
            }
        }
        protected string HediffLabel => Hediff != null ? Hediff.LabelCap.ToString() : "-";
        protected string BodyPartLabel => BodyPart != null ? BodyPart.LabelCap.ToString() : "-";
        protected virtual string SideLabel => ("Playwright.ScenParts.ForcedImplant." + Side.ToString()).Translate();

        protected virtual List<HediffDef> PossibleHediffs()
        {
            return DefDatabase<HediffDef>.AllDefsListForReading
                .Where((HediffDef x) => x.countsAsAddedPartOrImplant)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            int rows = 5;
            List<BodyPartDef> bodyPartChoices = BodyPartsThisCanApplyTo;
            if (bodyPartChoices.Count > 1)
            {
                rows += 1;
            }
            else if (bodyPartChoices.Count == 1)
            {
                BodyPart = bodyPartChoices.FirstOrDefault();
            }
            else
            {
                BodyPart = null;
            }

            if (HasRightSide)
            {
                rows += 1;
            }

            Rect scenPartRect = listing.GetScenPartRect(this, RowHeight * rows);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, rows);

            // Draw dropdown for part
            if (Widgets.ButtonText(helper.NextRect(), HediffLabel))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<HediffDef> allowedHediffs = PossibleHediffs().ToList();
                foreach (HediffDef hediffDef in allowedHediffs)
                {
                    floatMenuOptions.Add(new FloatMenuOption(hediffDef.LabelCap, () => Hediff = hediffDef));
                }
                PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
            }

            // Dropdown for which body part to put this on
            if (bodyPartChoices.Count > 1)
            {
                if (Widgets.ButtonText(helper.NextRect(), BodyPartLabel))
                {
                    var floatMenuOptions = new List<FloatMenuOption>();
                    foreach (BodyPartDef bodyPartDef in bodyPartChoices)
                    {
                        floatMenuOptions.Add(new FloatMenuOption(bodyPartDef.LabelCap, () => BodyPart = bodyPartDef));
                    }
                    PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
                }
            }

            // Dropdown for left/right side
            if (HasRightSide)
            {
                if (Widgets.ButtonText(helper.NextRect(), SideLabel))
                {
                    var floatMenuOptions = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Playwright.ScenParts.ForcedImplant.Left".Translate(), () => Side = ImplantSide.Left),
                        new FloatMenuOption("Playwright.ScenParts.ForcedImplant.Right".Translate(), () => Side = ImplantSide.Right),
                        new FloatMenuOption("Playwright.ScenParts.ForcedImplant.Random".Translate(), () => Side = ImplantSide.Random)
                    };
                    PlaywrightUtils.OpenFloatMenu(floatMenuOptions);
                }
            }

            base.DoPawnModifierEditInterface(helper.NextRect(2));

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.ForcedImplant.Help".Translate()));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Hediff, nameof(Hediff));
            Scribe_Defs.Look(ref BodyPart, nameof(BodyPart));
            Scribe_Values.Look(ref Side, nameof(Side));
        }

        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.ForcedImplant.Summary".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), HediffLabel).CapitalizeFirst();
        }

        protected override void ModifyNewPawn(Pawn p)
        {
            this.AddHediff(p);
        }

        protected override void ModifyHideOffMapStartingPawnPostMapGenerate(Pawn p)
        {
            this.AddHediff(p);
        }

        protected virtual void AddHediff(Pawn pawn)
        {
            // Oh my god this is ass, should I just have gone for a scenario part that pre-applies a surgery?
            if (pawn == null || pawn.health == null || Hediff == null)
            {
                return;
            }

            // Try to find the body part this applies to.
            // If there should be one, try to find it, abort if we can't find it.
            // If there shouldn't be one, we leave it as null, as then it probably applies to the Whole Body.
            BodyPartRecord bodyPartToApplyTo = null;
            List<BodyPartDef> bodyPartsThisCanApplyTo = BodyPartsThisCanApplyTo;
            if (bodyPartsThisCanApplyTo.Count > 0)
            {
                List<BodyPartRecord> allMatchingParts = pawn.RaceProps.body.GetPartsWithDef(BodyPart).ToList();
                switch (Side)
                {
                    case ImplantSide.Left:
                        bodyPartToApplyTo = allMatchingParts.FirstOrDefault();
                        break;
                    case ImplantSide.Right:
                        bodyPartToApplyTo = allMatchingParts.LastOrDefault();
                        break;
                    case ImplantSide.Random:
                        bodyPartToApplyTo = Rand.Bool ? allMatchingParts.FirstOrDefault() : allMatchingParts.LastOrDefault();
                        break;
                }

                if (bodyPartToApplyTo == null)
                {
                    return;
                }
            }

            pawn.health.AddHediff(Hediff, bodyPartToApplyTo);
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            ForcedImplant part = other as ForcedImplant;
            return part == null || this.Hediff != part.Hediff || this.BodyPart != part.BodyPart || this.Side != part.Side || this.context != part.context;
        }

        public enum ImplantSide
        {
            Left = 0,
            Right = 1,
            Random = 2
        }
    }
}
