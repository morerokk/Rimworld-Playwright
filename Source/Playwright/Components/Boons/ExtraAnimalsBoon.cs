using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class ExtraAnimalsBoon : BoonComponent
    {
        public override string Id => "Boons.ExtraAnimals";

        public List<ExtraAnimalEntry> Animals = new List<ExtraAnimalEntry>();

        private IEnumerable<PawnKindDef> PossibleAnimals()
        {
            var result = DefDatabase<PawnKindDef>.AllDefs
                .Where(pawnKindDef => pawnKindDef.RaceProps.Animal);
            if (Core.Settings.HideDryadsInAnimals)
            {
                result = result.Where(pawnKindDef => !pawnKindDef.RaceProps.Dryad);
            }
            return result;
        }

        protected override void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            if (boonContentListing.ButtonText("Playwright.Components.Boons.ExtraAnimals.Add".Translate()))
            {
                Animals.Add(new ExtraAnimalEntry());
                boonContentListing.InvalidateGroup();
                SoundUtils.PlayAdd();
            }

            DoItemsContents(boonContentListing);
        }

        private void DoItemsContents(Listing_AutoFitVertical boonContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (ExtraAnimalEntry animal in Animals.ToList())
            {
                Rect rect = boonContentListing.GetRect(35f);
                // UI divided into Count 25%, (Animal) 50%, last 25% taken up by (DeleteButton)

                // Count
                Rect countRect = new Rect(rect);
                countRect.width = rect.width * 0.25f;
                Widgets.TextFieldNumericLabeled(countRect, "Playwright.Components.Boons.ExtraAnimals.Count".Translate(), ref animal.Count, ref animal.CountBuffer, 1);

                // Animal
                Rect animalRect = new Rect(rect);
                animalRect.width = rect.width * 0.5f;
                animalRect.x += countRect.width;
                if (Widgets.ButtonText(animalRect, animal.AnimalLabel))
                {
                    var options = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("RandomPet".Translate().CapitalizeFirst(), () =>
                        {
                            animal.Animal = null;
                            boonContentListing.InvalidateGroup();
                        })
                    };
                    foreach (var animalDef in PossibleAnimals())
                    {
                        options.Add(new FloatMenuOption(animalDef.LabelCap, () => animal.Animal = animalDef));
                    }
                    SoundUtils.PlayClick();
                    PlaywrightUtils.OpenFloatMenu(options);
                }

                // Delete button
                Rect deleteButtonRect = new Rect(rect)
                {
                    width = rect.width * 0.25f,
                    x = rect.x + countRect.width + animalRect.width
                };
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    Animals.Remove(animal);
                    SoundUtils.PlayRemove();
                    boonContentListing.InvalidateGroup();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (ExtraAnimalEntry animal in Animals)
            {
                scenarioParts.Add(ScenPartUtils.MakeStartingAnimalPart(animal.Animal, animal.Count, 1f));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref Animals, nameof(Animals), LookMode.Deep);
        }

        public class ExtraAnimalEntry : IExposable
        {
            public PawnKindDef Animal;
            /// <summary>
            /// If constructing an instance of this manually, set <see cref="CountBuffer"/> to <see cref="Count"/>.ToString() for the editor to work properly.
            /// </summary>
            public int Count = 1;
            /// <summary>
            /// If constructing an instance of this manually, set this to <see cref="Count"/>.ToString() for the editor to work properly.
            /// </summary>
            public string CountBuffer = "1";

            public string AnimalLabel => Animal != null ? Animal.LabelCap : "RandomPet".Translate().CapitalizeFirst();

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Animal, nameof(Animal));
                Scribe_Values.Look(ref Count, nameof(Count));
                CountBuffer = Count.ToString();
            }
        }
    }
}
