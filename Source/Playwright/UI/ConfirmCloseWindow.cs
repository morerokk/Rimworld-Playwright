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
        /// <summary>
        /// The unscaled time at which this window was opened.
        /// </summary>
        private float OpenTime;

        public ConfirmCloseWindow(Action onConfirm) : base()
        {
            this.OnConfirm = onConfirm;
            this.closeOnClickedOutside = true;
            this.OpenTime = Time.unscaledTime;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Playwright.ConfirmClose".Translate());
            listing.End();

            const float ButtonHeight = 38f;
            const float ButtonWidth = 150f;
            const float ButtonMargin = 8f;

            float buttonY = inRect.yMax - ButtonHeight - ButtonMargin;
            float leftX = inRect.x;
            Rect noRect = new Rect(leftX, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(noRect, "Cancel".Translate()))
            {
                this.Cancel();
            }

            Rect yesRect = new Rect(inRect.xMax - ButtonWidth - ButtonMargin, buttonY, ButtonWidth, ButtonHeight);
            if (Widgets.ButtonText(yesRect, "Playwright.CloseWithoutSaving".Translate()))
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

        public override void OnAcceptKeyPressed()
        {
            if (Time.unscaledTime - this.OpenTime >= 0.05f)
            {
                this.Confirm();
            }
        }

        public override void OnCancelKeyPressed()
        {
            if (Time.unscaledTime - this.OpenTime >= 0.05f)
            {
                this.Cancel();
            }
        }
    }
}
