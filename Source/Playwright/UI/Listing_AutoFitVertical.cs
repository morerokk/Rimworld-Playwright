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
        public const float MarginBottom = 10f;

        public Rect? FittedRect { get; protected set; }
        public bool AutoInvalidateOnHeightExceeded { get; set; } = true;

        protected Action InvalidateGroupHook;

        public Listing_AutoFitVertical() : base()
        {

        }

        public Listing_AutoFitVertical(Action invalidateGroupHook) : base()
        {
            InvalidateGroupHook = invalidateGroupHook;
        }

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
            // If something inside the listing has invalidated the height (like a button), leave it for next frame
            if (FittedRect == null)
            {
                return;
            }
            Rect rect = FittedRect.Value;
            rect.height = curY + MarginBottom;
            FittedRect = rect;
            // We assume that an AutoFitVertical implies you just want a scrollable vertical list with a known width.
            // If curX is greater than the column width, oops, we've made a new column! Things are almost certain to silently break and not render.
            // In this case, we force an Invalidate.
            // This is NOT a replacement for managing Invalidate calls yourself, as it would still look weird if the listing was never able to shrink.
            // This is just a last chance to ensure my stupid chud son that I hate doesn't silently stop rendering content with no indication that something is wrong.
            // (Checking curX is the only way without Harmony, as all methods that make new columns are non-virtual)
            if (AutoInvalidateOnHeightExceeded && curX > ColumnWidth)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the calculated height to default.
        /// Useful if the contents of the listing have changed.
        /// </summary>
        public virtual void Invalidate()
        {
            FittedRect = null;
        }

        /// <summary>
        /// If this instance was constructed with a hook, calls the hook.
        /// If not constructed with a hook, just calls <see cref="Invalidate"/>.
        /// Useful for when every auto-listing has to be invalidated.
        /// (For instance, <seealso cref="Components.Boons.ExtraItemsBoon"/> may add content to the scenario's summary, which requires the summary to expand)
        /// </summary>
        public virtual void InvalidateGroup()
        {
            if (InvalidateGroupHook != null)
            {
                InvalidateGroupHook();
            }
            else
            {
                Invalidate();
            }
        }

        public virtual Rect GetScrollViewInnerRect(Rect rect)
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
