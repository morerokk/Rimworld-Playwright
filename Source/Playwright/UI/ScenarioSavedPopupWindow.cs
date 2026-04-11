using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class ScenarioSavedPopupWindow : Window
    {
        private Action<bool> OnClose;
        public override Vector2 InitialSize => new Vector2(400, 300);

        public ScenarioSavedPopupWindow(Action<bool> onClose = null) : base()
        {
            this.OnClose = onClose;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            const float ButtonHeight = 38f;
            const float ButtonWidth = 140f;
            const float ButtonMargin = 8f;

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Playwright.ScenarioSaved".Translate());
            if (this.OnClose != null)
            {
                listing.Label("Playwright.ScenarioSaved.1".Translate());

                float buttonY = inRect.yMax - ButtonHeight - ButtonMargin;
                float leftX = inRect.x;
                Rect noRect = new Rect(leftX, buttonY, ButtonWidth, ButtonHeight);
                if (Widgets.ButtonText(noRect, "No".Translate()))
                {
                    this.Cancel();
                }

                Rect yesRect = new Rect(inRect.xMax - ButtonWidth - ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
                if (Widgets.ButtonText(yesRect, "Yes".Translate()))
                {
                    this.Confirm();
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
