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
    public class StartWithHonor : ScenPart_PawnModifier
    {
        public int StartingHonor = 7;
        private string StartingHonorBuffer = "7";
        public bool ApplyTitles = true;

        public FactionDef FactionToStartWithHonorFor = FactionDefOf.Empire;

        private string FactionToAffectLabelText => FactionToStartWithHonorFor != null ? FactionToStartWithHonorFor.LabelCap.ToString() : "-";

        public static List<FactionDef> GetAllowedFactions()
        {
            return DefDatabase<FactionDef>.AllDefsListForReading
                .Where(def => !def.isPlayer && !def.hidden && def.HasRoyalTitles)
                .ToList();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 8);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 8);

            // Faction selector
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.StartWithHonor.Faction".Translate());
            if (Widgets.ButtonText(helper.NextRect(), FactionToAffectLabelText))
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                List<FactionDef> allowedFactions = GetAllowedFactions();
                foreach (FactionDef factionDef in allowedFactions)
                {
                    floatMenuOptions.Add(new FloatMenuOption(factionDef.LabelCap, () => FactionToStartWithHonorFor = factionDef, factionDef.FactionIcon, factionDef.DefaultColor));
                }
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }

            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.StartWithHonor.Honor".Translate());
            Widgets.IntEntry(helper.NextRect(), ref StartingHonor, ref StartingHonorBuffer);
            if (StartingHonor < 0)
            {
                StartingHonor = 0;
                StartingHonorBuffer = "0";
            }

            Widgets.CheckboxLabeled(helper.NextRect(), "Playwright.ScenParts.StartWithHonor.ApplyTitles".Translate(), ref ApplyTitles);

            base.DoPawnModifierEditInterface(helper.NextRect(2));

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.StartWithHonor.Help".Translate()));
            }
        }

        protected override void ModifyNewPawn(Pawn pawn)
        {
            List<Faction> factions = GetApplicableFactions();
            foreach (var faction in factions)
            {
                ApplyToPawn(pawn, faction);
            }
        }

        protected override void ModifyHideOffMapStartingPawnPostMapGenerate(Pawn pawn)
        {
            List<Faction> factions = GetApplicableFactions();
            foreach (var faction in factions)
            {
                ApplyToPawn(pawn, faction);
            }
        }

        private void ApplyToPawn(Pawn pawn, Faction faction)
        {
            int honorToApply = StartingHonor;
            if (ApplyTitles)
            {
                // Automatically award the pawn each title from lowest to highest, spending honor to get there
                var awardableTitles = FactionToStartWithHonorFor.RoyalTitlesAwardableInSeniorityOrderForReading;
                foreach (RoyalTitleDef titleDef in awardableTitles)
                {
                    if (titleDef.favorCost > honorToApply)
                    {
                        break;
                    }

                    pawn.royalty.SetTitle(faction, titleDef, true, false, false);

                    honorToApply -= titleDef.favorCost;
                }
            }
            pawn.royalty.SetFavor(faction, honorToApply, true);
        }

        private List<Faction> GetApplicableFactions()
        {
            List<Faction> factions = new List<Faction>();
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                if (faction.def == FactionToStartWithHonorFor)
                {
                    factions.Add(faction);
                }
            }

            if (factions.Count == 0)
            {
                Log.Error($"[Playwright] {nameof(StartWithHonor)}: Failed to find selected faction to award honor for. Faction was {FactionToStartWithHonorFor.LabelCap}");
            }
            return factions;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<FactionDef>(ref FactionToStartWithHonorFor, nameof(FactionToStartWithHonorFor));
            Scribe_Values.Look<int>(ref StartingHonor, nameof(StartingHonor), 7, false);
            Scribe_Values.Look<bool>(ref ApplyTitles, nameof(ApplyTitles), true, false);
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            if (this.ApplyTitles)
            {
                return "Playwright.ScenParts.StartWithHonor.Summary.WithTitles".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), StartingHonor, FactionToAffectLabelText).CapitalizeFirst();
            }

            return "Playwright.ScenParts.StartWithHonor.Summary".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent(), StartingHonor, FactionToAffectLabelText).CapitalizeFirst();
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            StartWithHonor part = other as StartWithHonor;
            return part == null || this.FactionToStartWithHonorFor != part.FactionToStartWithHonorFor;
        }

        public override bool HasNullDefs()
        {
            return base.HasNullDefs() || this.FactionToStartWithHonorFor == null;
        }
    }
}
