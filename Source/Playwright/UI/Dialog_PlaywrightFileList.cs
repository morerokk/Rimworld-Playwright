using RimWorld;
using Rokk.Playwright.Utilities;
using System;
using System.IO;
using Verse;

namespace Rokk.Playwright.UI
{
    public abstract class Dialog_PlaywrightFileList : Dialog_FileList
    {
        protected static string DocumentElementName => "SavedPlaywright";

        protected override void ReloadFiles()
        {
            files.Clear();
            foreach (FileInfo playwrightFile in PlaywrightUtils.AllPlaywrightFiles)
            {
                try
                {
                    SaveFileInfo saveFileInfo = new SaveFileInfo(playwrightFile);
                    saveFileInfo.LoadData();
                    files.Add(saveFileInfo);
                }
                catch (Exception ex)
                {
                    Log.Error("[Playwright] Exception loading " + playwrightFile.Name + ": " + ex.ToString());
                }
            }
        }
    }
}
