using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace Rokk.Playwright.Utilities
{
    public static class PlaywrightUtils
    {
        public static string PlaywrightFolderPath => FolderUnderSaveData("Playwrights");
        public static string PlaywrightExtension => ".pwt";

        public static IEnumerable<FileInfo> AllPlaywrightFiles
        {
            get
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(PlaywrightFolderPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                return directoryInfo.GetFiles()
                    .Where(file => file.Extension == PlaywrightExtension)
                    .OrderByDescending(file => file.LastWriteTime);
            }
        }

        private static string FolderUnderSaveData(string folderName)
        {
            string text = Path.Combine(GenFilePaths.SaveDataFolderPath, folderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(text);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            return text;
        }

        /// <summary>
        /// Get just the scenario's summary (without leading description).
        /// </summary>
        public static string GetScenarioSummary(Scenario scenario)
        {
            string fullText = scenario.GetFullInformationText();
            if (fullText.StartsWith(scenario.description))
            {
                return fullText.Substring(scenario.description.Length).TrimStart();
            }
            return fullText;
        }

        /// <summary>
        /// Opens a float menu.
        /// If no options are provided, adds an unselectable default option instead of throwing an error.
        /// </summary>
        public static void OpenFloatMenu(List<FloatMenuOption> options)
        {
            if (options.Count == 0)
            {
                options.Add(new FloatMenuOption("Playwright.NoFloatMenuOptions".Translate(), null));
            }
            Find.WindowStack.Add(new FloatMenu(options));
        }

        /// <summary>
        /// Returns all selectable non-player factions.
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
        /// Returns all selectable factions that the game would consider a "neutral" faction to the player by default.
        /// </summary>
        /// <remarks>
        /// Depending on settings, this can return just factions that the game would add by default (no factions that are replaced by others).
        /// </remarks>
        public static IEnumerable<FactionDef> GetDefaultNeutralFactions()
        {
            // We assume that when "permanentEnemyToEveryoneExcept" is used, if at least one faction is a player faction,
            // they probably intend for all the player factions to be in there.
            // Some factions may be hostile to new arrivals but neutral towards new tribes,
            // but at that point we're stretching the limits of what we can do in the UI. It's fine
            return GetAllNpcFactions()
                .Where(def =>
                !def.naturalEnemy
                && !def.permanentEnemy
                && (def.permanentEnemyToEveryoneExcept == null || def.permanentEnemyToEveryoneExcept.Any(exceptFaction => exceptFaction?.isPlayer == true)));
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
    }
}
