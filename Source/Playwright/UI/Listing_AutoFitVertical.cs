using System;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.UI
{
    /// <summary>
    /// Helper class used for scrollable listings. Intended for use with scrollviews.
    /// Makes an absurdly big Rect when Begin() is called.
    /// When End() is called, shrinks the rect to its contents.
    /// Use Invalidate() to let the listing recalculate its size next time it begins.
    /// </summary>
    public class Listing_AutoFitVertical : Listing_Standard
    {
        public const float DefaultHeight = 99999f;

        public Rect? FittedRect { get; protected set; }

        public override void Begin(Rect rect)
        {
            if (FittedRect == null)
            {
                FittedRect = new Rect(rect)
                {
                    height = DefaultHeight
                };
            }
            base.Begin(FittedRect.Value);
        }

        public override void End()
        {
            base.End();
            Rect rect = FittedRect.Value;
            rect.height = curY;
            FittedRect = rect;
        }

        /// <summary>
        /// Resets the calculated height to default.
        /// Useful if the contents of the listing have changed.
        /// </summary>
        public virtual void Invalidate()
        {
            FittedRect = null;
        }

        public Rect GetScrollViewInnerRect(Rect rect)
        {
            if (FittedRect == null)
            {
                return new Rect(rect)
                {
                    height = DefaultHeight
                };
            }
            return new Rect(FittedRect.Value);
        }
    }
}
