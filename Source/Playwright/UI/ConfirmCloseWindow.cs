using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class ConfirmCloseWindow : Window
    {
        private Action OnConfirm;
        public override Vector2 InitialSize => new Vector2(400, 300);

        public ConfirmCloseWindow(Action onConfirm) : base()
        {
            this.OnConfirm = onConfirm;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Playwright.ConfirmClose".Translate());
            listing.End();

            const float ButtonHeight = 38f;
            const float ButtonWidth = 140f;
            const float ButtonMargin = 8f;

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

        public override void Notify_ClickOutsideWindow()
        {
            this.Cancel();
        }

        private void Confirm()
        {
            this.Close(true);
            this.OnConfirm();
        }

        private void Cancel()
        {
            this.Close(true);
        }
    }
}
