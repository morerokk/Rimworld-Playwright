using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Utilities;
using System;
using System.IO;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class Dialog_PlaywrightList_Load : Dialog_PlaywrightFileList
    {
        private Action<PlaywrightStructure> StructureCallback;

        public Dialog_PlaywrightList_Load(Action<PlaywrightStructure> structureCallback)
        {
            StructureCallback = structureCallback;
            interactButLabel = "LoadGameButton".Translate();
            deleteTipKey = "Playwright.DeleteTip";
        }

        protected override void DoFileInteraction(string fileName)
        {
            string filePath = PlaywrightUtils.PlaywrightFolderPath + "/" + fileName + Extension;
            PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.None, () =>
            {
                if (TryLoadPlaywright(filePath, out PlaywrightStructure playwright))
                {
                    StructureCallback(playwright);
                }
                Close();
            });
        }

        private static bool TryLoadPlaywright(string filePath, out PlaywrightStructure Playwright)
        {
            // Intentionally capitalized to match other places where Playwright is used as a field or property, do not un-capitalize Playwright
            Playwright = null;
            try
            {
                Log.Message("[Playwright] Trying to load playwright at path: " + filePath);
                Scribe.loader.InitLoading(filePath);
                try
                {
                    ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.None, true);
                    Scribe_Deep.Look(ref Playwright, nameof(Playwright));
                    Scribe.loader.FinalizeLoading();
                }
                catch
                {
                    Scribe.ForceStop();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error("[Playwright] Error loading saved playwright: " + ex.Message);
                Playwright = null;
                Scribe.ForceStop();
            }

            return Playwright != null;
        }
    }
}
