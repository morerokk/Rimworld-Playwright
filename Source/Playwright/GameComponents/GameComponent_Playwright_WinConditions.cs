using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.GameComponents
{
    public class GameComponent_Playwright_WinConditions : GameComponent
    {
        private bool ColonyEnabled = false;
        private bool ColonyWon = false;
        private int ColonyRequiredColonists;

        private bool ConquestEnabled = false;
        private bool ConquestWon = false;
        private bool ConquestAllowAllies;

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
            // Colony
            if (ColonyEnabled && !ColonyWon && ColonyRequiredColonists > 0 && Find.TickManager.TicksGame % 3000 == 0)
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

            // Conquest
            if (ConquestEnabled && !ConquestWon && Find.TickManager.TicksGame % 3100 == 0)
            {
                var settlements = Find.WorldObjects.Settlements
                    .Where(s => s.Faction != null && !s.Faction.IsPlayer);

                if (ConquestAllowAllies)
                {
                    settlements = settlements.Where(s => s.Faction.PlayerRelationKind != FactionRelationKind.Ally);
                }

                if (settlements.Count() == 0)
                {
                    StartFadeCountdown(5f, WinGameConquest);
                    ConquestWon = true;
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

            // Conquest
            WinCondition_Conquest conquestPart = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_Conquest)
                .Cast<WinCondition_Conquest>()
                .FirstOrDefault();
            if (conquestPart != null)
            {
                ConquestAllowAllies = conquestPart.AllowAllies;
                ConquestEnabled = true;
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

        private void WinGameConquest()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            foreach (Pawn pawn in pawns)
            {
                stringBuilder.AppendLine(pawn.LabelCap);
            }

            // Check if we should show the "you and your alliance" credits text, or just "you"
            bool playerHasAllies = false;
            if (ConquestAllowAllies)
            {
                FieldInfo relationsFieldInfo = AccessTools.Field(typeof(Faction), "relations");
                var playerRelations = (List<FactionRelation>)relationsFieldInfo.GetValue(Faction.OfPlayer);
                playerHasAllies = playerRelations.Any(r => r.kind == FactionRelationKind.Ally);
            }
            string winIntroKey = "Playwright.ScenParts.WinCondition_Conquest.WinIntro";
            if (!playerHasAllies)
            {
                winIntroKey = "Playwright.ScenParts.WinCondition_Conquest.WinIntroNoAllies";
            }

            string credits = GameVictoryUtility.MakeEndCredits(
                winIntroKey.Translate(),
                "Playwright.ScenParts.WinCondition_Conquest.WinEnding".Translate(),
                stringBuilder.ToString(),
                "Playwright.ScenParts.WinCondition_Conquest.WinColonists", pawns);
            GameVictoryUtility.ShowCredits(credits, SongDefOf.EndCreditsSong, false, 5f);

            ConquestWon = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ColonyWon, nameof(ColonyWon));
            Scribe_Values.Look(ref ConquestWon, nameof(ConquestWon));
        }
    }
}
