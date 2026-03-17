using HarmonyLib;
using RimWorld;
using Rokk.Playwright.UI;
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
    // The psylink hediff doesn't have scenarioCanAdd, and the float range slider is not suitable for the int-based levels of psylinks
    public class ForcedPsylinkLevel : ScenPart_PawnModifier
    {
        public HediffDef PsylinkHediff => HediffDefOf.PsychicAmplifier;

        public FloatRange SeverityRange = new FloatRange(1f, 1f);

        private float MaxSeverity
        {
            get
            {
                return PsylinkHediff.maxSeverity;
            }
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, RowHeight * 6);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 6);
            Widgets.FloatRange(helper.NextRect(2), listing.CurHeight.GetHashCode(), ref this.SeverityRange, 0f, this.MaxSeverity, "ConfigurableSeverity", ToStringStyle.FloatTwo, 0f, GameFont.Small, null, 1f);
            base.DoPawnModifierEditInterface(helper.NextRect(2));

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.ForcedPsylinkLevel.Help".Translate()));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<FloatRange>(ref this.SeverityRange, nameof(SeverityRange), default(FloatRange), false);
        }

        public override string Summary(Scenario scen)
        {
            string levelRange = SeverityRange.min.ToString();
            if (SeverityRange.min != SeverityRange.max)
            {
                levelRange += "-" + SeverityRange.max.ToString();
            }
            return "Playwright.ScenParts.ForcedPsylinkLevel.Summary".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), levelRange).CapitalizeFirst();
        }

        protected override void ModifyNewPawn(Pawn p)
        {
            this.AddHediff(p);
        }

        protected override void ModifyHideOffMapStartingPawnPostMapGenerate(Pawn p)
        {
            this.AddHediff(p);
        }

        private void AddHediff(Pawn p)
        {
            BodyPartRecord brainBodyPart = p.health.hediffSet.GetBrain();
            Hediff_Psylink hediff = (Hediff_Psylink)HediffMaker.MakeHediff(this.PsylinkHediff, p, brainBodyPart);

            float level = this.SeverityRange.RandomInRange;
            hediff.Severity = Mathf.Round(level);

            FieldInfo levelInfo = AccessTools.Field(typeof(Hediff_Psylink), "level");
            levelInfo.SetValue(hediff, Mathf.RoundToInt(level));

            p.health.AddHediff(hediff, brainBodyPart, null, null);
        }
    }
}
