using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rokk.Playwright.UI
{
    /// <summary>
    /// A draw helper for scenario part editors that helps keep track of things like row heights and rects
    /// </summary>
    public class ScenPartDrawHelper
    {
        public Rect ScenPartRect { get; private set; }
        public float RowHeight { get; private set; }
        public int MaxRows { get; private set; }
        
        public int NextRow { get; private set; } = 0;

        public ScenPartDrawHelper(Rect scenPartRect, float rowHeight, int maxRows)
        {
            ScenPartRect = scenPartRect;
            RowHeight = rowHeight;
            MaxRows = maxRows;
        }

        /// <summary>
        /// Get the next rect to draw in, and advance the counter.
        /// </summary>
        /// <param name="rows">How many rows the new rect should take up</param>
        public Rect NextRect(int rows = 1)
        {
            float height = ScenPartRect.height / MaxRows;
            height *= rows;
            Rect rect = new Rect(ScenPartRect.x, ScenPartRect.y + RowHeight * NextRow, ScenPartRect.width, height);
            NextRow += rows;
            return rect;
        }

        /// <summary>
        /// Advance the counter without getting a rect
        /// </summary>
        /// <param name="rows">How many rows to skip</param>
        public void Skip(int rows = 1)
        {
            NextRow += rows;
        }

        /// <summary>
        /// ID's that still mystify me, but IntRange asks for them so ok?
        /// </summary>
        public static class Ids
        {
            public const int NaturalGoodwill = 800122;
        }
    }
}
