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
    }
}
