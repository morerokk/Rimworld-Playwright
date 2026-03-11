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
    public class StartWithHonor : ScenPart
    {
        public int StartingHonor = 7;
        private string StartingHonorBuffer = "7";
        public bool ApplyTitles = true;

        public Faction FactionToStartWithHonorFor => Faction.OfEmpire;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 4);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 4);

            Widgets.IntEntry(helper.NextRect(), ref StartingHonor, ref StartingHonorBuffer);
            if (StartingHonor < 0)
            {
                StartingHonor = 0;
                StartingHonorBuffer = "0";
            }

            Widgets.CheckboxLabeled(helper.NextRect(), "Playwright.ScenParts.StartWithHonor.ApplyTitles".Translate(), ref ApplyTitles);

            helper.Skip(1);

            // TODO: tweak help text for VFE Empire
            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.StartWithHonor.Help".Translate()));
            }
        }

        public override void PostGameStart()
        {
            base.PostGameStart();

            foreach (var map in Current.Game.Maps)
            {
                foreach (var pawn in map.mapPawns.FreeColonists)
                {
                    ApplyToPawn(pawn);
                }
            }
        }

        private void ApplyToPawn(Pawn pawn)
        {
            int honorToApply = StartingHonor;
            var faction = FactionToStartWithHonorFor;
            if (ApplyTitles)
            {
                // Automatically award the pawn each title from lowest to highest, spending honor to get there
                var awardableTitles = faction.def.RoyalTitlesAwardableInSeniorityOrderForReading;
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

        // Boilerplate seemingly needed by scenarios to save/load them and stuff?
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref StartingHonor, nameof(StartingHonor), 7, false);
            Scribe_Values.Look<bool>(ref ApplyTitles, nameof(ApplyTitles), true, false);
        }

        // Scenario summary description
        public override string Summary(Scenario scen)
        {
            if (this.ApplyTitles)
            {
                return "Playwright.ScenParts.StartWithHonor.Summary.WithTitles".Translate(StartingHonor, FactionToStartWithHonorFor.Name);
            }

            return "Playwright.ScenParts.StartWithHonor.Summary".Translate(StartingHonor, FactionToStartWithHonorFor.Name);
        }
    }
}
