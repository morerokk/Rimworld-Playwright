using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class ForcedTraitsSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.ForcedTraits";

        public List<TraitEntry> Traits = new List<TraitEntry>();

        protected virtual IEnumerable<TraitDef> GetAllowedTraits()
        {
            return DefDatabase<TraitDef>.AllDefs
                .OrderBy(def => def.label);
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.ForcedTraits.Add".Translate()))
            {
                Traits.Add(new TraitEntry());
                SoundUtils.PlayAdd();
                specialConditionContentListing.InvalidateGroup();
            }

            DoTraitsContents(specialConditionContentListing);
        }

        private void DoTraitsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            float oldWidth = specialConditionContentListing.ColumnWidth;
            specialConditionContentListing.ColumnWidth *= 0.9f;

            foreach (TraitEntry trait in Traits.ToList())
            {
                specialConditionContentListing.GapLine();
                // Draw buttons (select trait), (select pawn generation context)
                Rect rowRect = specialConditionContentListing.GetRect(35f);

                Rect traitButtonRect = new Rect(rowRect);
                traitButtonRect.width *= 0.4f;
                if (Widgets.ButtonText(traitButtonRect, trait.TraitLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var traitDef in GetAllowedTraits())
                    {
                        foreach (var degreeData in traitDef.degreeDatas)
                        {
                            options.Add(new FloatMenuOption(degreeData.LabelCap, () =>
                            {
                                trait.Trait = traitDef;
                                trait.Degree = degreeData.degree;
                                specialConditionContentListing.InvalidateGroup();
                            }));
                        }
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                Rect contextButtonRect = new Rect(rowRect);
                contextButtonRect.width *= 0.4f;
                contextButtonRect.x += traitButtonRect.width;
                if (Widgets.ButtonText(contextButtonRect, trait.ContextLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (PawnGenerationContext context in Enum.GetValues(typeof(PawnGenerationContext)))
                    {
                        options.Add(new FloatMenuOption(("PawnGenerationContext_" + context.ToString()).Translate().CapitalizeFirst(), () =>
                        {
                            trait.Context = context;
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                Rect deleteButtonRect = new Rect(rowRect);
                deleteButtonRect.width *= 0.2f;
                deleteButtonRect.x += traitButtonRect.width + contextButtonRect.width;
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Traits.Remove(trait);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }

                // Finally, draw a chance slider on a new line
                trait.Chance = specialConditionContentListing.SliderLabeled("Playwright.Components.SpecialConditions.ForcedTraits.ChancePercentage".Translate(trait.Chance * 100f), trait.Chance, 0f, 1f, 0.5f, "Playwright.Components.SpecialConditions.ForcedTraits.Chance.Help".Translate());
                trait.Chance = MathF.Round(trait.Chance, 2);
            }

            specialConditionContentListing.ColumnWidth = oldWidth;
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var traitEntry in Traits)
            {
                if (traitEntry.Trait == null)
                {
                    continue;
                }
                scenarioParts.Add(ScenPartUtility.MakeForcedTraitPart(traitEntry.Trait, traitEntry.Degree, traitEntry.Context, traitEntry.Chance));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Traits, nameof(Traits), LookMode.Deep);
        }

        public class TraitEntry : IExposable
        {
            // A trait is actually a TraitDef along with a degree (Fast Walker vs. Jogger)
            public TraitDef Trait;
            public int Degree;
            public PawnGenerationContext Context = PawnGenerationContext.All;
            public float Chance = 1f;

            public string TraitLabel => Trait != null ? Trait.DataAtDegree(Degree).LabelCap : "Playwright.Components.SpecialConditions.ForcedTraits.Choose".Translate().ToString();
            public string ContextLabel => ("PawnGenerationContext_" + Context.ToString()).Translate().CapitalizeFirst();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Trait, nameof(Trait));
                Scribe_Values.Look(ref Degree, nameof(Degree));
                Scribe_Values.Look(ref Context, nameof(Context));
                Scribe_Values.Look(ref Chance, nameof(Chance));
            }
        }
    }
}
