using BigAndSmall;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Compat.BSSapientAnimals.Things
{
    /// <summary>
    /// A Thing that spawns an animal and turns it sapient on a small delay.
    /// </summary>
    /// <remarks>
    /// This is necessary as the game will panic and throw an exception if we destroy a pawn during game start (tick 0), which turning an animal sapient does.
    /// So instead, this invisible and non-interactable Thing spawns the animal tick 1, turns it sapient tick 2,
    /// and self-destructs tick 3, preventing any more code from being run unnecessarily.
    /// (Thanks for the advice, Sam!)
    /// </remarks>
    public class DelayedSapientAnimalSpawner : Thing
    {
        public PawnKindDef AnimalKind;
        public Pawn SpawnedAnimal;

        protected override void Tick()
        {
            base.Tick();

            int ticksRun = Find.TickManager.TicksGame - TickSpawned;

            if (ticksRun == 0)
            {
                // Do nothing yet
            }
            else if (ticksRun == 1)
            {
                // First tick, spawn the animal, do nothing else
                SpawnAnimalOnMap();
            }
            else if (ticksRun == 2)
            {
                // Second tick, turn it sapient
                MakeAnimalSapient();
            }
            else
            {
                // Third tick, self-destruct, job's done
                this.Destroy();
            }
        }

        protected virtual void SpawnAnimalOnMap()
        {
            Pawn animal = PawnGenerator.GeneratePawn(AnimalKind, Faction.OfPlayer);
            if (animal.Name == null || animal.Name.Numerical)
            {
                animal.Name = PawnBioAndNameGenerator.GeneratePawnName(animal, NameStyle.Full, null, false, null);
            }

            IntVec3 spawnLoc = CellFinder.RandomClosewalkCellNear(Map.Center, Map, 10);
            SpawnedAnimal = (Pawn)GenSpawn.Spawn(animal, spawnLoc, Map, Rot4.Random);
        }

        protected virtual void MakeAnimalSapient()
        {
            RaceMorpher.SwapAnimalToSapientVersion(SpawnedAnimal);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref AnimalKind, nameof(AnimalKind));
            Scribe_Values.Look(ref SpawnedAnimal, nameof(SpawnedAnimal));
        }
    }
}
