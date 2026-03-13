using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    public static class PlaywrightDrawHelper
    {
        public static Rect RectWithMargin(Rect contentRect, float margin)
        {
            return new Rect(contentRect.x + margin, contentRect.y + margin, contentRect.width - (margin * 2), contentRect.height - (margin * 2));
        }

        public static Rect RectForLabel(Rect contentRect, string text)
        {
            Vector2 textSize = Text.CalcSize(text);
            return new Rect(contentRect.x, contentRect.y, textSize.x, textSize.y);
        }

        public static Rect NextLabel(Rect contentRect, string translationKey, float margin = 0f)
        {
            if (margin != 0f)
            {
                contentRect = RectWithMargin(contentRect, margin);
            }

            string translated = translationKey.Translate();
            Rect rect = RectForLabel(contentRect, translated);
            Widgets.Label(rect, translated);
            return new Rect(contentRect.x, contentRect.y + rect.height, contentRect.width, contentRect.height - rect.height);
        }
    }
}
