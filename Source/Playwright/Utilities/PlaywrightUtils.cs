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
    }
}
