using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using ScenarioPawnsAndCorpses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Compat.ScenarioPawnsAndCorpses.Components.SpecialConditions
{
    public class ForcedBackstoriesSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.Compat_ScenarioPawnsAndCorpses_ForcedBackstories";

        public List<BackstoryEntry> Backstories = new List<BackstoryEntry>();

        protected virtual IEnumerable<BackstoryDef> GetAllowedBackstories(BackstorySlot backstorySlot)
        {
            return DefDatabase<BackstoryDef>.AllDefs
                .Where(b => b.slot == backstorySlot);
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.Compat_ScenarioPawnsAndCorpses_ForcedBackstories.AddChild".Translate()))
            {
                Backstories.Add(new BackstoryEntry() { BackstorySlot = BackstorySlot.Childhood });
                SoundUtils.PlayAdd();
                specialConditionContentListing.InvalidateGroup();
            }
            DoBackstoriesContents(specialConditionContentListing, BackstorySlot.Childhood);

            // Gap needed to avoid the slider stealing mouse input
            specialConditionContentListing.Gap(12f);

            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.Compat_ScenarioPawnsAndCorpses_ForcedBackstories.AddAdult".Translate()))
            {
                Backstories.Add(new BackstoryEntry() { BackstorySlot = BackstorySlot.Adulthood });
                SoundUtils.PlayAdd();
                specialConditionContentListing.InvalidateGroup();
            }
            DoBackstoriesContents(specialConditionContentListing, BackstorySlot.Adulthood);
        }

        private void DoBackstoriesContents(Listing_AutoFitVertical specialConditionContentListing, BackstorySlot backstorySlot)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            float oldWidth = specialConditionContentListing.ColumnWidth;
            specialConditionContentListing.ColumnWidth *= 0.9f;

            foreach (BackstoryEntry backstory in Backstories.Where(b => b.BackstorySlot == backstorySlot).ToList())
            {
                specialConditionContentListing.GapLine();
                Rect deleteButtonRect = specialConditionContentListing.GetRect(0f);
                deleteButtonRect.height = 50f;
                // Draw buttons (select backstory), (select pawn generation context)
                Rect rowRect = specialConditionContentListing.GetRect(35f);

                Rect backstoryButtonRect = new Rect(rowRect);
                backstoryButtonRect.width *= 0.4f;
                if (Widgets.ButtonText(backstoryButtonRect, backstory.BackstoryLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var backstoryDef in GetAllowedBackstories(backstorySlot))
                    {
                        options.Add(new FloatMenuOption(backstoryDef.TitleCapFor(Gender.Male), () =>
                        {
                            backstory.Backstory = backstoryDef;
                            specialConditionContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                Rect contextButtonRect = new Rect(rowRect);
                contextButtonRect.width *= 0.4f;
                contextButtonRect.x += backstoryButtonRect.width;
                if (Widgets.ButtonText(contextButtonRect, backstory.ContextLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (PawnGenerationContext context in Enum.GetValues(typeof(PawnGenerationContext)))
                    {
                        options.Add(new FloatMenuOption(("PawnGenerationContext_" + context.ToString()).Translate().CapitalizeFirst(), () =>
                        {
                            backstory.Context = context;
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Backstories.Remove(backstory);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                // Finally, draw a chance slider on a new line
                backstory.Chance = specialConditionContentListing.SliderLabeled("Playwright.ChancePercentage".Translate(backstory.Chance * 100f), backstory.Chance, 0f, 1f, 0.25f, "Playwright.Components.SpecialConditions.Compat_ScenarioPawnsAndCorpses_ForcedBackstories.Chance.Help".Translate());
                backstory.Chance = MathF.Round(backstory.Chance, 2);
            }

            specialConditionContentListing.ColumnWidth = oldWidth;
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var backstoryEntry in Backstories)
            {
                if (backstoryEntry.Backstory == null)
                {
                    continue;
                }

                ScenPart_ForcedBackstoryInterface part;
                if (backstoryEntry.BackstorySlot == BackstorySlot.Childhood)
                {
                    part = (ScenPart_ForcedChildhoodBackstory)ScenarioMaker.MakeScenPart(ScenarioPawnsAndCorpsesDefOf.ScenPart_ForcedChildhoodBackstory);
                }
                else
                {
                    part = (ScenPart_ForcedAdultBackstory)ScenarioMaker.MakeScenPart(ScenarioPawnsAndCorpsesDefOf.ScenPart_ForcedAdultBackstory);
                }

                FieldInfo backstoryDefInfo = AccessTools.Field(typeof(ScenPart_ForcedBackstoryInterface), "backstoryDef");
                backstoryDefInfo.SetValue(part, backstoryEntry.Backstory);

                FieldInfo chanceInfo = AccessTools.Field(typeof(ScenPart_ForcedBackstoryInterface), "chance");
                chanceInfo.SetValue(part, backstoryEntry.Chance);

                FieldInfo contextInfo = AccessTools.Field(typeof(ScenPart_ForcedBackstoryInterface), "context");
                contextInfo.SetValue(part, backstoryEntry.Context);

                scenarioParts.Add(part);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Backstories, nameof(Backstories), LookMode.Deep);
        }

        public class BackstoryEntry : IExposable
        {
            public BackstoryDef Backstory;
            public BackstorySlot BackstorySlot;
            public PawnGenerationContext Context = PawnGenerationContext.All;
            public float Chance = 1f;

            public string BackstoryLabel => Backstory != null ? Backstory.TitleCapFor(Gender.Male).ToString() : "Playwright.Components.SpecialConditions.Compat_ScenarioPawnsAndCorpses_ForcedBackstories.Choose".Translate().ToString();
            public string ContextLabel => ("PawnGenerationContext_" + Context.ToString()).Translate().CapitalizeFirst();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Backstory, nameof(Backstory));
                Scribe_Values.Look(ref BackstorySlot, nameof(BackstorySlot));
                Scribe_Values.Look(ref Context, nameof(Context));
                Scribe_Values.Look(ref Chance, nameof(Chance));
            }
        }
    }
}
