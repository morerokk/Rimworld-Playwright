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
    public class AdditionalUnwaveringChance : ScenPart
    {
        public float Chance = 1f;
        private string ChanceReadable => Mathf.FloorToInt(Chance * 100f).ToString() + "%";

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 5);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 5);

            helper.Skip(1);

            Chance = Widgets.HorizontalSlider(helper.NextRect(), Chance, 0f, 1f, false, "Playwright.ScenParts.AdditionalUnwaveringChance.Chance".Translate(), "0%", "100%", 0.01f);
            Widgets.Label(helper.NextRect(), ChanceReadable);

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.AdditionalUnwaveringChance.Help".Translate()));
            }
        }

        public override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context, bool redressed)
        {
            if (context == PawnGenerationContext.NonPlayer && pawn.guest != null && pawn.Faction != Faction.OfPlayerSilentFail && Rand.Chance(Chance))
            {
                pawn.guest.Recruitable = false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Chance, nameof(Chance), 1f, false);
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            return "Playwright.ScenParts.AdditionalUnwaveringChance.Summary".Translate(ChanceReadable);
        }
    }
}
