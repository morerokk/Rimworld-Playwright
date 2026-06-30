using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rokk.Playwright.Utilities
{
    /// <summary>
    /// Helper class to help categorize Things.
    /// This is primarily intended for the scenario editor, to help declutter the HUGE float menus when you select items to start with.
    /// </summary>
    public static class CategorizedThingUtils
    {
        public static IEnumerable<ThingDef> GetItems()
        {
            return DefDatabase<ThingDef>.AllDefs
                .Where(thing => IsThingAllowedInScenarios(thing));
        }

        public static IEnumerable<ThingCategoryDef> GetRootCategories()
        {
            return DefDatabase<ThingCategoryDef>.AllDefs
                .Where(category => category.parent != null && category.parent.defName == "Root")
                .Where(category => category.DescendantThingDefs.Any(thing => IsThingAllowedInScenarios(thing)));
        }

        public static IEnumerable<ThingDef> GetThingDefsByCategory(ThingCategoryDef thingCategoryDef)
        {
            return thingCategoryDef.DescendantThingDefs
                .Where(thing => IsThingAllowedInScenarios(thing));
        }

        public static List<FloatMenuOption> GetItemCategoryOptions(Action<ThingDef> onSelected)
        {
            var options = new List<FloatMenuOption>();
            foreach (ThingCategoryDef category in GetRootCategories())
            {
                options.Add(new FloatMenuOption(category.LabelCap, () =>
                {
                    SoundUtils.PlayClick();
                    PlaywrightUtils.OpenFloatMenu(GetItemOptionsByCategory(category, onSelected));
                }));
            }

            options.Add(new FloatMenuOption("Playwright.CategoryAll".Translate(), () =>
            {
                SoundUtils.PlayClick();
                PlaywrightUtils.OpenFloatMenu(GetItemOptionsAll(onSelected));
            }));

            return options;
        }

        public static List<FloatMenuOption> GetItemOptionsByCategory(ThingCategoryDef category, Action<ThingDef> onSelected)
        {
            var options = new List<FloatMenuOption>();
            foreach (ThingDef thing in GetThingDefsByCategory(category))
            {
                options.Add(new FloatMenuOption(thing.LabelCap, () =>
                {
                    SoundUtils.PlayClick();
                    onSelected(thing);
                }, thing.uiIcon, thing.uiIconColor));
            }

            return options;
        }

        public static List<FloatMenuOption> GetItemOptionsAll(Action<ThingDef> onSelected)
        {
            var options = new List<FloatMenuOption>();
            foreach (ThingDef thing in GetItems())
            {
                options.Add(new FloatMenuOption(thing.LabelCap, () =>
                {
                    SoundUtils.PlayClick();
                    onSelected(thing);
                }, thing.uiIcon, thing.uiIconColor));
            }

            return options;
        }

        public static bool IsThingAllowedInScenarios(ThingDef thingDef)
        {
            return (thingDef.category == ThingCategory.Item && thingDef.scatterableOnMapGen && !thingDef.destroyOnDrop)
                    || (thingDef.category == ThingCategory.Building && thingDef.Minifiable)
                    || (thingDef.defName == "ShipChunk"); // Specifically ship chunks are added by the default scenarios but can't be selected normally
        }
    }
}
