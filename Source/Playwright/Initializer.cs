using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright
{
    // Patch harmony stuff and load settings when the game is almost done loading.
    // Technically these worked in the mod (Core) constructor too, but that's a potential minefield.
    // (For example, defs aren't bound yet that early, so if we ever save defs in settings, it will silently not work)
    [StaticConstructorOnStartup]
    internal static class Initializer
    {
        static Initializer()
        {
            Core.Harmony.PatchAllUncategorized();
            Core.Current = LoadedModManager.GetMod<Core>();
            Core.Settings = Core.Current.GetSettings<Settings>();
        }
    }
}
