using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Utilities
{
    public static class FactionUtils
    {
        /// <summary>
        /// Returns all selectable non-player factions that would be selectable in the game's faction UI when starting a new game.
        /// </summary>
        /// <remarks>
        /// Depending on settings, this can return just factions that the game would add by default (no factions that are replaced by others).
        /// </remarks>
        public static IEnumerable<FactionDef> GetAllNpcFactions()
        {
            var result = FactionGenerator.ConfigurableFactions
                .Where(def => def.displayInFactionSelection);

            if (Core.Settings.HideReplacedFactions)
            {
                var replacedFactions = DefDatabase<FactionDef>.AllDefs
                    .Where(def => def.replacesFaction != null)
                    .Select(def => def.replacesFaction);
                result = result.Where(def => !replacedFactions.Contains(def));
            }

            return result;
        }

        /// <summary>
        /// Returns all selectable factions that the game would consider an "enemy" faction to the player by default.
        /// </summary>
        /// <remarks>
        /// Depending on settings, this can return just factions that the game would add by default (no factions that are replaced by others).
        /// </remarks>
        public static IEnumerable<FactionDef> GetDefaultEnemyFactions()
        {
            return GetAllNpcFactions()
                .Where(def =>
                def.naturalEnemy
                || def.permanentEnemy
                || (def.permanentEnemyToEveryoneExcept?.Count > 0 && !def.permanentEnemyToEveryoneExcept.Any(exceptFaction => exceptFaction?.isPlayer == true)));
        }

        /// <summary>
        /// Returns all selectable factions that the game would consider a "neutral" faction to the player by default.
        /// </summary>
        /// <remarks>
        /// Depending on settings, this can return just factions that the game would add by default (no factions that are replaced by others).
        /// </remarks>
        public static IEnumerable<FactionDef> GetDefaultNeutralFactions()
        {
            // We assume that when "permanentEnemyToEveryoneExcept" is used, if at least one faction is a player faction,
            // they probably intend for all the player factions to be in there.
            // Some factions may be permanently hostile to new arrivals but can be swayed into neutral/ally by new tribes,
            // but at that point we're stretching the limits of what we can do in the UI. It's fine.
            return GetAllNpcFactions()
                .Where(def =>
                !def.naturalEnemy
                && !def.permanentEnemy
                && (def.permanentEnemyToEveryoneExcept == null || def.permanentEnemyToEveryoneExcept.Any(exceptFaction => exceptFaction?.isPlayer == true)));
        }

        /// <summary>
        /// Returns all selectable factions that the game does not consider a "permanent enemy" faction to the player.
        /// If they are neutral or can ever be made non-hostile through goodwill, this will return them.
        /// </summary>
        /// <remarks>
        /// Depending on settings, this can return just factions that the game would add by default (no factions that are replaced by others).
        /// </remarks>
        public static IEnumerable<FactionDef> GetNotPermanentlyHostileFactions()
        {
            return GetAllNpcFactions()
                .Where(def => !def.permanentEnemy
                && (def.permanentEnemyToEveryoneExcept == null || def.permanentEnemyToEveryoneExcept.Any(exceptFaction => exceptFaction?.isPlayer == true)));
        }

        /// <summary>
        /// Gets all possible player factions (such as New Arrivals, New Tribe, Gravship Crew, etc.)
        /// </summary>
        public static IEnumerable<FactionDef> GetPlayerFactions()
        {
            return DefDatabase<FactionDef>.AllDefs
                .Where(def => def.isPlayer);
        }
    }
}
