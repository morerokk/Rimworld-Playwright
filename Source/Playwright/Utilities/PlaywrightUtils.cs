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
        /// Open and returns a new Playwright designer window.
        /// </summary>
        /// <param name="canGoToNewColonyScreenAfterwards">
        /// Whether the player should be asked if they want to go to the new colony screen after creating their scenario.
        /// Should be false if you're opening the designer from another window (such as Mod Options). Should probably only be true if opened directly from the main menu landing screen.
        /// </param>
        /// <returns>The opened window.</returns>
        public static PlaywrightWindow OpenPlaywrightWindow(bool canGoToNewColonyScreenAfterwards = false)
        {
            var window = new PlaywrightWindow(canGoToNewColonyScreenAfterwards);
            Find.WindowStack.Add(window);
            return window;
        }

        public static bool IsModActive(string modIdentifier)
        {
            return ModLister.GetActiveModWithIdentifier(modIdentifier, true)?.Active == true;
        }
    }
}
