using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.GameComponents
{
    public class GameComponent_Playwright_WinConditions : GameComponent
    {
        public bool ColonyEnabled = false;
        public bool ColonyWon = false;
        public int ColonyRequiredColonists = 30;

        private float Countdown = 0f;
        private Action CountdownEnded;

        public GameComponent_Playwright_WinConditions(Game game)
        {

        }

        public override void GameComponentUpdate()
        {
            if (Countdown <= 0f || CountdownEnded == null)
            {
                return;
            }

            Countdown -= Time.deltaTime;
            if (Countdown <= 0f)
            {
                CountdownEnded();
                Countdown = 0f;
                CountdownEnded = null;
            }
        }

        public override void GameComponentTick()
        {
            if (ColonyEnabled && !ColonyWon && Find.TickManager.TicksGame % 2000 != 0)
            {
                int playerPawnCount = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction
                    .Count(p => !p.IsAnimal);

                if (playerPawnCount >= ColonyRequiredColonists)
                {
                    StartFadeCountdown(5f, WinGameColony);
                    ColonyWon = true;
                    return;
                }
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            LoadScenarioValues();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            LoadScenarioValues();
        }

        private void LoadScenarioValues()
        {
            Scenario scenario = Current.Game.Scenario;
            if (scenario == null)
            {
                return;
            }

            // Colony
            WinCondition_Colony colonyPart = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_Colony)
                .Cast<WinCondition_Colony>()
                .FirstOrDefault();
            if (colonyPart != null)
            {
                ColonyRequiredColonists = colonyPart.Colonists;
                ColonyEnabled = true;
            }
        }

        private void StartFadeCountdown(float duration, Action onCountDownEnded)
        {
            CountdownEnded = onCountDownEnded;
            Countdown = 5f;
            ScreenFader.StartFade(Color.white, duration);
        }

        private void WinGameColony()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            foreach (Pawn pawn in pawns)
            {
                stringBuilder.AppendLine(pawn.LabelCap);
            }

            string credits = GameVictoryUtility.MakeEndCredits(
                "Playwright.ScenParts.WinCondition_Colony.WinIntro".Translate(),
                "Playwright.ScenParts.WinCondition_Colony.WinEnding".Translate(),
                stringBuilder.ToString(),
                "Playwright.ScenParts.WinCondition_Colony.WinColonists", pawns);
            GameVictoryUtility.ShowCredits(credits, SongDefOf.EndCreditsSong, false, 5f);

            ColonyWon = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ColonyWon, nameof(ColonyWon));
        }
    }
}
