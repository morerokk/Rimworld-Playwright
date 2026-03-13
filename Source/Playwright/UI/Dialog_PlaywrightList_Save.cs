using RimWorld;
using Rokk.Playwright.Composer;
using Rokk.Playwright.Utilities;
using System;
using System.IO;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class Dialog_PlaywrightList_Save : Dialog_PlaywrightFileList
    {
        private PlaywrightStructure Playwright;

        protected override bool ShouldDoTypeInField => true;

        public Dialog_PlaywrightList_Save(PlaywrightStructure playwright)
        {
            this.Playwright = playwright;
            this.interactButLabel = "OverwriteButton".Translate();
            this.typingName = this.Playwright.Origin.NameTranslated + PlaywrightUtils.PlaywrightExtension;
        }

        protected override void DoFileInteraction(string fileName)
        {
            fileName = GenFile.SanitizedFileName(fileName);
            string savePath = PlaywrightUtils.PlaywrightFolderPath + "/" + fileName;
            if (!savePath.EndsWith(PlaywrightUtils.PlaywrightExtension))
            {
                savePath += PlaywrightUtils.PlaywrightExtension;
            }

            LongEventHandler.QueueLongEvent(() =>
            {
                SaveFile(savePath);
            }, "SavingLongEvent", false,
            (Exception e) =>
            {
                Log.Error("[Playwright] Error while saving your Playwright to a file: " + e.Message);
            });

            Messages.Message("SavedAs".Translate(fileName), MessageTypeDefOf.SilentInput, false);
            this.Close();
        }

        private void SaveFile(string savePath)
        {
            SafeSaver.Save(savePath, DocumentElementName, () =>
            {
                ScribeMetaHeaderUtility.WriteMetaHeader();
                Scribe_Deep.Look(ref Playwright, nameof(Playwright));
            });
        }
    }
}
