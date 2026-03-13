using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class ScenarioSavedPopupWindow : Window
    {
        private Action<bool> OnClose;

        public ScenarioSavedPopupWindow(Action<bool> onClose = null) : base()
        {
            this.OnClose = onClose;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Playwright.ScenarioSaved".Translate());
            if (this.OnClose != null)
            {
                listing.Label("Playwright.ScenarioSaved.1".Translate());
                if (listing.ButtonText("Yes".Translate(), null, 0.25f))
                {
                    this.Confirm();
                }
                if (listing.ButtonText("No".Translate(), null, 0.25f))
                {
                    this.Cancel();
                }
            }
            listing.End();
        }

        public override void Notify_ClickOutsideWindow()
        {
            this.Cancel();
        }

        private void Confirm()
        {
            this.Close(true);
            if (this.OnClose != null)
            {
                this.OnClose(true);
            }
        }

        private void Cancel()
        {
            this.Close(true);
            if (this.OnClose != null)
            {
                this.OnClose(false);
            }
        }
    }
}
