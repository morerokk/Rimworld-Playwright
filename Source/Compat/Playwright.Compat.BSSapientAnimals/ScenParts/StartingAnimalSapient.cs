using BigAndSmall;
using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Compat.BSSapientAnimals.Things;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.Noise;

namespace Rokk.Playwright.Compat.BSSapientAnimals.ScenParts
{
    /// <summary>
    /// Similar to ScenPart_StartingAnimal for obvious reasons, except that it generates a sapient animal.
    /// </summary>
    /// Since ScenPart_StartingAnimal has almost everything remotely useful already privated, we're forced to duplicate a lot of it.
    public class StartingAnimalSapient : ScenPart
    {
        public PawnKindDef AnimalKind;
        public int Count = 1;
        public string CountBuffer = "1";

        public string AnimalKindLabel => AnimalKind != null ? AnimalKind.LabelCap : "RandomPet".Translate().CapitalizeFirst();

        protected virtual IEnumerable<PawnKindDef> GetAllowedAnimals()
        {
            return DefDatabase<PawnKindDef>.AllDefs
                .Where(pawnKindDef => pawnKindDef.RaceProps.Animal);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            var scenPartRect = listing.GetScenPartRect(this, RowHeight * 5);
            var helper = new ScenPartDrawHelper(scenPartRect, RowHeight, 5);

            // Animal selector
            if (Widgets.ButtonText(helper.NextRect(), AnimalKindLabel))
            {
                var options = new List<FloatMenuOption>()
                {
                    new FloatMenuOption("RandomPet".Translate().CapitalizeFirst(), () => AnimalKind = null)
                };
                foreach (PawnKindDef animalDef in GetAllowedAnimals())
                {
                    options.Add(new FloatMenuOption(animalDef.LabelCap, () => AnimalKind = animalDef));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            Widgets.Label(helper.NextRect(), "Playwright.Count".Translate());
            Widgets.IntEntry(helper.NextRect(), ref Count, ref CountBuffer);
            if (Count < 0)
            {
                Count = 0;
                CountBuffer = "0";
            }

            helper.Skip();

            if (Widgets.ButtonText(helper.NextRect(), "?"))
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.Compat.BSSapientAnimals.ScenParts.StartingAnimalSapient.Help".Translate()));
            }
        }

        protected virtual IEnumerable<PawnKindDef> RandomPets()
        {
            IEnumerable<PawnKindDef> enumerable = from td in this.GetAllowedAnimals()
                                                  where td.RaceProps.petness > 0f
                                                  select td;
            if (enumerable.EnumerableNullOrEmpty<PawnKindDef>())
            {
                enumerable = from td in this.GetAllowedAnimals()
                             where td.RaceProps.petness > 0f
                             select td;
            }
            return enumerable;
        }

        public override void GenerateIntoMap(Map map)
        {
            // Only spawn on the first map
            if (Find.GameInitData == null)
            {
                return;
            }

            for (int i = 0; i < this.Count; i++)
            {
                PawnKindDef pawnKindDef;
                if (this.AnimalKind != null)
                {
                    pawnKindDef = this.AnimalKind;
                }
                else
                {
                    pawnKindDef = this.RandomPets().RandomElement();
                }
                SpawnDelayedSpawner(map, pawnKindDef);
            }
        }

        protected virtual void SpawnDelayedSpawner(Map map, PawnKindDef animalDef)
        {
            // We have to spawn an invisible ethereal thing that does nothing except spawn the animal and then make it sapient on a slight delay.
            // We can't do it during generation or during tick 0 because the game will panic at a starting animal being deleted and replaced
            ThingDef spawnerDef = DefOfs.ThingDefOf.Playwright_DelayedSapientAnimalSpawner;
            var spawner = (DelayedSapientAnimalSpawner)ThingMaker.MakeThing(spawnerDef);
            spawner.AnimalKind = animalDef;

            IntVec3 spawnLoc = CellFinder.RandomClosewalkCellNear(map.Center, map, 10);
            GenSpawn.Spawn(spawner, spawnLoc, map, Rot4.Random);
        }

        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", ScenPart_StartingThing_Defined.PlayerStartWithIntro);
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == "PlayerStartsWith")
            {
                yield return "Playwright.Compat.BSSapientAnimals.ScenParts.StartingAnimalSapient.Summary".Translate(AnimalKindLabel, Count.ToString());
            }
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            StartingAnimalSapient part = other as StartingAnimalSapient;
            return part == null || this.AnimalKind != part.AnimalKind;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref AnimalKind, nameof(AnimalKind));
            Scribe_Values.Look(ref Count, nameof(Count), 1);
            CountBuffer = Count.ToString();
        }

        public static StartingAnimalSapient MakeStartingAnimalPart(PawnKindDef animalKind = null, int count = 1)
        {
            StartingAnimalSapient part = (StartingAnimalSapient)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_StartingAnimalSapient);
            part.AnimalKind = animalKind;
            part.Count = count;
            part.CountBuffer = count.ToString();
            return part;
        }
    }
}
