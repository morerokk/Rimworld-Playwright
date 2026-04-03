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

        private bool RoyalTitlesEnabled = false;
        private bool RoyalTitlesWon = false;
        private List<WinCondition_RoyalTitles> RoyalTitlesParts = new List<WinCondition_RoyalTitles>();

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

            // Royal titles
            if (RoyalTitlesEnabled && !RoyalTitlesWon && Find.TickManager.TicksGame % 3200 == 0)
            {
                var playerPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction
                    .Where(p => p.royalty != null)
                    .ToList();
                foreach (WinCondition_RoyalTitles royalTitlesPart in RoyalTitlesParts)
                {
                    List<Pawn> qualifiedPawns = playerPawns
                        .Where(p => p.royalty.AllTitlesInEffectForReading.Any(t => t.faction.def == royalTitlesPart.Faction && t.def.seniority >= royalTitlesPart.Title.seniority))
                        .ToList();
                    if (qualifiedPawns.Count >= royalTitlesPart.Colonists)
                    {
                        StartFadeCountdown(5f, () =>
                        {
                            WinGameRoyalTitles(qualifiedPawns);
                        });
                        RoyalTitlesWon = true;
                        return;
                    }
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

            // Royal titles
            RoyalTitlesParts = scenario.AllParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.Playwright_WinCondition_RoyalTitles)
                .Cast<WinCondition_RoyalTitles>()
                .ToList();
            if (RoyalTitlesParts.Count > 0)
            {
                RoyalTitlesEnabled = true;
            }
        }

        private void StartFadeCountdown(float duration, Action onCountDownEnded)
        {
            CountdownEnded = onCountDownEnded;
            Countdown = duration;
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

        private void WinGameRoyalTitles(List<Pawn> royalPawns)
        {
            StringBuilder stringBuilder = new StringBuilder();
            // Need list of all pawns anyway, to prevent "left behind" messages. However, the win screen will only list the royalty.
            List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            foreach (Pawn pawn in royalPawns)
            {
                stringBuilder.AppendLine(pawn.LabelCap);
            }

            string credits = GameVictoryUtility.MakeEndCredits(
                "Playwright.ScenParts.WinCondition_RoyalTitles.WinIntro".Translate(),
                "Playwright.ScenParts.WinCondition_RoyalTitles.WinEnding".Translate(),
                stringBuilder.ToString(),
                "Playwright.ScenParts.WinCondition_RoyalTitles.WinColonists", pawns);
            GameVictoryUtility.ShowCredits(credits, SongDefOf.EndCreditsSong, false, 5f);

            RoyalTitlesWon = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ColonyWon, nameof(ColonyWon));
            Scribe_Values.Look(ref ConquestWon, nameof(ConquestWon));
            Scribe_Values.Look(ref RoyalTitlesWon, nameof(RoyalTitlesWon));
        }
    }
}
