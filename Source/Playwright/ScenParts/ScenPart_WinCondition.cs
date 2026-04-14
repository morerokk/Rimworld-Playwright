using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public abstract class ScenPart_WinCondition : ScenPart
    {
        /// <summary>
        /// Whether the player has won, for *this specific* ScenPart.
        /// A ScenPart_WinCondition can only award a win once, per part.
        /// </summary>
        public bool Won;

        // Used to make "fade out and then win" work
        private float FadeEndTime;
        private Action CountdownEnded;

        // Summarize all win conditions under 1 summary heading
        public string SummaryTag => "Playwright_WinCondition";
        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, SummaryTag, "Playwright.ScenParts.WinCondition.SummaryIntro".Translate());
        }
        public abstract override IEnumerable<string> GetSummaryListEntries(string tag);

        protected abstract void DoHelpButton();

        public override void Tick()
        {
            if (CountdownEnded == null)
            {
                return;
            }

            if (Time.realtimeSinceStartup >= FadeEndTime)
            {
                CountdownEnded();
                CountdownEnded = null;
            }
        }

        /// <summary>
        /// Fades the screen to white over 5 seconds and then runs <seealso cref="WinGame"/>. Recommended way to trigger a win.
        /// </summary>
        /// <param name="winIntroKey">Translation key to use for the win screen intro. Example text: "You've launched the ship!"</param>
        /// <param name="winEndingKey">Translation key to use for the win screen outro, before the credits. Example text: "Your machine persona will now try to guide the ship..."</param>
        /// <param name="winColonistsKey">Translation key to use for the summary heading of the spotlight colonists. Example text: "These colonists escaped:"</param>
        /// <param name="spotlightPawns">List of pawns to put in the spotlight, listing their names in the win screen under the <paramref name="winColonistsKey"/> text. If null, defaults to all alive colonists.</param>
        /// <param name="showLeftBehindPawns">If true, any alive colonists not in <paramref name="spotlightPawns"/> are shown as "These colonists were left behind:". If false, they are not shown in the credits at all.</param>
        /// <param name="creditsSong">The credits song to play. If null, defaults to <see cref="SongDefOf.EndCreditsSong"/>.</param>
        /// <param name="fadeColor">The color to fade out to. If null, defaults to <see cref="Color.white"/>.</param>
        /// <param name="fadeDuration">How long the fade out should last. Defaults to 5f (5 seconds).</param>
        protected virtual void FadeOutAndWinGame(string winIntroKey, string winEndingKey, string winColonistsKey, List<Pawn> spotlightPawns = null, bool showLeftBehindPawns = false, SongDef creditsSong = null, Color? fadeColor = null, float fadeDuration = 5f)
        {
            if (fadeColor == null)
            {
                fadeColor = Color.white;
            }

            Won = true;
            FadeEndTime = Time.realtimeSinceStartup + 5f;
            CountdownEnded = () =>
            {
                WinGame(winIntroKey, winEndingKey, winColonistsKey, spotlightPawns, showLeftBehindPawns, creditsSong);
            };
            ScreenFader.StartFade(fadeColor.Value, fadeDuration);
        }

        /// <summary>
        /// Immediately wins the game. Not recommended to use directly, use <seealso cref="FadeOutAndWinGame"/> instead to avoid jumpscaring players.
        /// </summary>
        /// <param name="winIntroKey">Translation key to use for the win screen intro. Example text: "You've launched the ship!"</param>
        /// <param name="winEndingKey">Translation key to use for the win screen outro, before the credits. Example text: "Your machine persona will now try to guide the ship..."</param>
        /// <param name="winColonistsKey">Translation key to use for the summary heading of the spotlight colonists. Example text: "These colonists escaped:"</param>
        /// <param name="spotlightPawns">List of pawns to put in the spotlight, listing their names in the win screen under the <paramref name="winColonistsKey"/> text. If null, defaults to all alive colonists.</param>
        /// <param name="showLeftBehindPawns">If true, any alive colonists not in <paramref name="spotlightPawns"/> are shown as "These colonists were left behind:". If false, they are not shown in the credits at all.</param>
        /// <param name="creditsSong">The credits song to play. If null, defaults to <see cref="SongDefOf.EndCreditsSong"/>.</param>
        protected virtual void WinGame(string winIntroKey, string winEndingKey, string winColonistsKey, List<Pawn> spotlightPawns = null, bool showLeftBehindPawns = false, SongDef creditsSong = null)
        {
            Won = true;
            if (creditsSong == null)
            {
                creditsSong = SongDefOf.EndCreditsSong;
            }

            if (spotlightPawns == null)
            {
                spotlightPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }

            List<Pawn> pawns;
            if (showLeftBehindPawns)
            {
                pawns = spotlightPawns;
            }
            else
            {
                pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }

            StringBuilder spotlightBuilder = new StringBuilder();
            foreach (Pawn spotlightPawn in spotlightPawns)
            {
                spotlightBuilder.AppendLine(spotlightPawn.LabelCap);
            }

            string credits = GameVictoryUtility.MakeEndCredits(
                winIntroKey.Translate(),
                winEndingKey.Translate(),
                spotlightBuilder.ToString(),
                winColonistsKey, pawns);
            GameVictoryUtility.ShowCredits(credits, SongDefOf.EndCreditsSong, false, 5f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Won, nameof(Won), false);
        }
    }
}
