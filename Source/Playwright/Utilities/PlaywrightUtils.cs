using RimWorld;
using RimWorld.Planet;
using Rokk.Playwright.UI;
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
        /// Opens the game's Custom Xenotype editor window from anywhere.
        /// </summary>
        public static void OpenXenotypeEditorWindow()
        {
            var dialog = new Dialog_CreateXenotype(0, () =>
            {
                CharacterCardUtility.cachedCustomXenotypes = null;
            });
            Find.WindowStack.Add(dialog);
        }

        /// <summary>
        /// Opens an Ideology editor window from anywhere in the main menu.
        /// </summary>
        /// <remarks>
        /// This does some spooky stuff to make the ideoligion editor work without needing to be in the world menu.
        /// It's also a bit jank and insists on adding a harmless random default ideology (which isn't saved so shouldn't get in the way).
        /// Do NOT use this while the game is ingame, and don't use this from any location in the main menu that's on or past the "New Colony" screen.
        /// This is basically only safe in the main menu landing, in options or in the playwright editor.
        /// </remarks>
        public static void OpenIdeoligionEditor()
        {
            // Do NOT try this at home
            Current.ProgramState = ProgramState.Entry;
            Current.Game = new Game();
            Current.Game.InitData = new GameInitData();
            Current.Game.Scenario = ScenarioDefOf.Crashlanded.scenario;
            Find.Scenario.PreConfigure();
            Current.Game.storyteller = new Storyteller(StorytellerDefOf.Cassandra, DifficultyDefOf.Rough);
            List<FactionDef> list = new List<FactionDef> { FactionDefOf.PlayerColony };
            Current.Game.World = WorldGenerator.GenerateWorld(0.05f, "test", OverallRainfall.Normal, OverallTemperature.Normal, OverallPopulation.AlmostNone, LandmarkDensity.Sparse, list, 0f);
            Find.GameInitData.startingTile = 0;
            var page = new Page_ConfigureIdeo_Playwright();
            page.doCloseX = true;
            page.next = null;
            page.prev = null;
            Find.WindowStack.Add(page);
        }

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
            // but at that point we're stretching the limits of what we can do in the UI. It's fine.
            // Theoretically this would be possible by just reading the origin's playerfaction now actually.
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
