using HarmonyLib;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Patches
{
    [HarmonyPatch(typeof(GameComponentUtility), nameof(GameComponentUtility.StartedNewGame))]
    public static class GameComponentUtility_StartedNewGamePatches
    {
        [HarmonyPostfix]
        public static void PostFix()
        {
            // Most of this is apparently not necessary, as scenario parts have callbacks for this. Neat!
            //LongEventHandler.ExecuteWhenFinished(OnGameLoaded);
        }

        /*
        private static void OnGameLoaded()
        {
            if (Current.Game == null)
            {
                return;
            }

            foreach (var part in Current.Game.Scenario.AllParts)
            {
                // Start all pawns off with X honor
                if (part is StartWithHonor startWithHonorPart)
                {
                    ApplyStartWithHonor(startWithHonorPart);
                }
            }
        }

        private static void ApplyStartWithHonor(StartWithHonor part)
        {
            var factions = part.FactionsToStartWithHonorFor;

            foreach (var map in Current.Game.Maps)
            {
                foreach (var pawn in map.mapPawns.FreeColonists)
                {
                    foreach (var faction in factions)
                    {
                        pawn.royalty.SetFavor(faction, part.StartingHonor, true);
                    }
                }
            }
        }
        */
    }
}
