using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public class InfoPopupWindow : Window
    {
        private string Text;

        public InfoPopupWindow(string text) : base()
        {
            Text = text;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(inRect, Text);
        }

        public override void Notify_ClickOutsideWindow()
        {
            this.Close(true);
        }
    }
}
