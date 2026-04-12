using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Rokk.Playwright.Components.SpecialConditions.XenotypeSwapSpecialCondition;

namespace Rokk.Playwright.Components.Boons
{
    public class ExtraItemsBoon : BoonComponent
    {
        public override string Id => "Boons.ExtraItems";

        public List<ExtraItemEntry> Items = new List<ExtraItemEntry>();

        private IEnumerable<ThingDef> PossibleThingDefs()
        {
            return DefDatabase<ThingDef>.AllDefs
                .Where(thingDef => (thingDef.category == ThingCategory.Item && thingDef.scatterableOnMapGen && !thingDef.destroyOnDrop) || (thingDef.category == ThingCategory.Building && thingDef.Minifiable))
                .OrderBy(thingDef => thingDef.label);
        }

        // These names lmao
        private IEnumerable<ThingDef> PossibleStuffForThing(ThingDef thing)
        {
            return GenStuff.AllowedStuffsFor(thing)
                .OrderBy(thingDef => thingDef.label);
        }

        public override void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.ExtraItems.Add".Translate()))
            {
                Items.Add(new ExtraItemEntry());
                boonContentListing.Invalidate();
                AddSound();
            }

            DoItemsContents(boonContentListing);
        }

        private void DoItemsContents(Listing_AutoFitVertical boonContentListing)
        {
            // Lots of stuff taken from ScenPart_ThingCount
            Texture2D deleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            foreach (ExtraItemEntry item in Items.ToList())
            {
                Rect rect = boonContentListing.GetRect(35f);
                // UI divided into Count (Thing) (Stuff) (Quality) each 22% wide, last 12% taken up by (DeleteButton)
                float colWidth = rect.width * 0.22f;

                // Count
                Rect countRect = new Rect(rect);
                countRect.width = colWidth;
                Widgets.TextFieldNumericLabeled(countRect, "Playwright.Components.Boons.ExtraItems.Count".Translate(), ref item.Count, ref item.CountBuffer, 1);

                // Thing
                Rect thingRect = new Rect(rect);
                thingRect.width = colWidth;
                thingRect.x += colWidth;
                if (Widgets.ButtonText(thingRect, item.ThingLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (ThingDef thingDef in PossibleThingDefs())
                    {
                        options.Add(new FloatMenuOption(thingDef.LabelCap, () => item.Thing = thingDef, thingDef.uiIcon, thingDef.uiIconColor));
                    }
                    ClickSound();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // Stuff (only if the Thing supports it)
                if (item.Thing?.MadeFromStuff == true)
                {
                    Rect stuffRect = new Rect(rect);
                    stuffRect.width = colWidth;
                    stuffRect.x += colWidth * 2;
                    if (Widgets.ButtonText(stuffRect, item.StuffLabel))
                    {
                        var options = new List<FloatMenuOption>();
                        foreach (ThingDef stuffThingDef in PossibleStuffForThing(item.Thing))
                        {
                            options.Add(new FloatMenuOption(stuffThingDef.LabelCap, () => item.Stuff = stuffThingDef, stuffThingDef.uiIcon, stuffThingDef.uiIconColor));
                        }
                        ClickSound();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    item.Stuff = null;
                }

                // Quality (only if the Thing supports it)
                if (item.Thing?.HasComp(typeof(CompQuality)) == true)
                {
                    Rect qualityRect = new Rect(rect);
                    qualityRect.width = colWidth;
                    qualityRect.x += colWidth * 3;
                    if (Widgets.ButtonText(qualityRect, item.QualityLabel))
                    {
                        var options = new List<FloatMenuOption>()
                        {
                            new FloatMenuOption("Default".Translate().CapitalizeFirst(), () => item.Quality = null)
                        };
                        foreach (QualityCategory qualityCategory in QualityUtility.AllQualityCategories)
                        {
                            options.Add(new FloatMenuOption(qualityCategory.GetLabel().CapitalizeFirst(), () => item.Quality = qualityCategory));
                        }
                        ClickSound();
                        PlaywrightUtils.OpenFloatMenu(options);
                    }
                }
                else
                {
                    item.Quality = null;
                }

                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.12f,
                    x = rect.x + colWidth * 4
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Items.Remove(item);
                    RemoveSound();
                }
            }
        }

        private void ClickSound()
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
        }

        private void AddSound()
        {
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
        }

        private void RemoveSound()
        {
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (ExtraItemEntry item in Items)
            {
                if (item.Thing == null)
                {
                    Log.Warning("[Playwright] Extra items boon: item thing was null");
                    continue;
                }
                scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(item.Thing, item.Stuff, item.Count, item.Quality));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Items, nameof(Items), LookMode.Deep);
        }

        public class ExtraItemEntry : IExposable
        {
            public ThingDef Thing;
            public ThingDef Stuff;
            public QualityCategory? Quality;
            public int Count = 1;
            public string CountBuffer = "1";

            public string ThingLabel => Thing != null ? Thing.LabelCap.ToString() : "-";
            public string StuffLabel => Stuff != null ? Stuff.LabelCap.ToString() : "-";
            public string QualityLabel => Quality != null ? Quality.Value.GetLabel().CapitalizeFirst() : "-";

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Thing, nameof(Thing));
                Scribe_Defs.Look(ref Stuff, nameof(Stuff));
                Scribe_Values.Look(ref Quality, nameof(Quality));
                Scribe_Values.Look(ref Count, nameof(Count));
                CountBuffer = Count.ToString();
            }
        }
    }
}
