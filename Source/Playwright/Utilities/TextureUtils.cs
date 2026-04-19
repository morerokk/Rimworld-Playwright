using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Utilities
{
    public static class TextureUtils
    {
        /// <summary>
        /// Texture for clickable "add something to a list" button.
        /// </summary>
        public static Texture2D AddButtonTex => ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        /// <summary>
        /// Texture for clickable "remove something from a list" button.
        /// </summary>
        public static Texture2D DeleteButtonTex => ContentFinder<Texture2D>.Get("UI/Buttons/Dismiss", true);
        /// <summary>
        /// Texture for a clickable "show me a help window" button
        /// </summary>
        public static Texture2D HelpButtonTex => ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
    }
}
