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
    public class WinCondition_Colony : ScenPart_WinCondition
    {
        public int Colonists = 30;
        private string ColonistsBuffer = "30";

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 3);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 3);

            Widgets.IntEntry(helper.NextRect(), ref Colonists, ref ColonistsBuffer);
            if (Colonists < 1)
            {
                Colonists = 1;
                ColonistsBuffer = "1";
            }

            helper.Skip(1);

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                DoHelpButton();
            }
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == SummaryTag)
            {
                yield return "Playwright.ScenParts.WinCondition_Colony.Summary".Translate(Colonists);
            }
        }

        protected override void DoHelpButton()
        {
            Find.WindowStack.Add(new InfoPopupWindow("Playwright.ScenParts.WinCondition_Colony.Help".Translate()));
        }

        public override void Tick()
        {
            // Always run base.Tick(). This handles the fade to white countdown, and showing the credits once the fade is done.
            // You can skip running this if you want to use your own fade logic, or if you call WinGame() directly without fade.
            base.Tick();
            if (Won)
            {
                return;
            }

            // Winning is kind of rare (tends to only happen once per run after all).
            // When making a Win Condition, limit how often it runs. This one runs once every 3000 ticks.
            // Try to stagger the numbers so you're not "doing nothing" most of the time,
            // and then suddenly checking a whole bunch of win conditions all on the same tick.
            // If your win condition checks are even slightly laggy, this would result in noticeable lagspikes.
            if (Find.TickManager.TicksGame % 3000 == 0)
            {
                int playerPawnCount = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction
                    .Count(p => !p.IsAnimal);

                if (playerPawnCount >= Colonists)
                {
                    FadeOutAndWinGame(
                        "Playwright.ScenParts.WinCondition_Colony.WinIntro",
                        "Playwright.ScenParts.WinCondition_Colony.WinEnding",
                        "Playwright.ScenParts.WinCondition_Colony.WinColonists"
                    );
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Colonists, nameof(Colonists));
        }
    }
}
