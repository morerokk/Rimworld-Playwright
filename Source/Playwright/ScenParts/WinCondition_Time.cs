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
    public class WinCondition_Time : ScenPart_WinCondition
    {
        public int Days = 600;
        private string DaysBuffer = "600";

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 6);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 6);

            helper.Skip(1);

            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.WinCondition_Time.Days".Translate());
            Widgets.IntEntry(helper.NextRect(), ref Days, ref DaysBuffer);
            if (Days < 1)
            {
                Days = 1;
                DaysBuffer = "1";
            }
            int years = Days / 60;
            int days = Days % 60;
            Widgets.Label(helper.NextRect(), "Playwright.ScenParts.WinCondition_Time.YearsAndDays".Translate(years, days));

            helper.Skip(1);

            if(Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                yield return "Playwright.ScenParts.WinCondition_Time.Summary".Translate(Days);
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_Time.Help".Translate()));
        }

        public override void Tick()
        {
            base.Tick();
            if (Won)
            {
                return;
            }

            if (Find.TickManager.TicksGame % 3300 == 0)
            {
                // A day is exactly 60,000 ticks, this seems to work regardless of how much the player moves around the world or re-settles elsewhere
                float daysPassed = (float)Find.TickManager.TicksGame / 60000f;
                if (daysPassed >= Days)
                {
                    int aliveColonists = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction
                        .Count(p => !p.IsAnimal);
                    if (aliveColonists > 0)
                    {
                        FadeOutAndWinGame(
                            "Playwright.ScenParts.WinCondition_Time.WinIntro",
                            "Playwright.ScenParts.WinCondition_Time.WinEnding",
                            "Playwright.ScenParts.WinCondition_Time.WinColonists"
                        );
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Days, nameof(Days));
        }
    }
}
