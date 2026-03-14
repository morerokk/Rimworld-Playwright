using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Utilities
{
    /// <summary>
    /// Utility class for making scenario parts, as many of them involve a lot of reflection boilerplate.
    /// </summary>
    public static class ScenPartUtility
    {
        public static ScenPart_ForcedHediff MakeForcedHediffPart(HediffDef hediffDef, PawnGenerationContext pawnGenerationContext, float chance, FloatRange severityRange)
        {
            ScenPart_ForcedHediff forcedHediffPart = (ScenPart_ForcedHediff)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.ForcedHediff);

            FieldInfo contextInfo = AccessTools.Field(typeof(ScenPart_ForcedHediff), "context");
            contextInfo.SetValue(forcedHediffPart, pawnGenerationContext);

            FieldInfo chanceInfo = AccessTools.Field(typeof(ScenPart_ForcedHediff), "chance");
            chanceInfo.SetValue(forcedHediffPart, chance);

            FieldInfo hediffDefInfo = AccessTools.Field(typeof(ScenPart_ForcedHediff), "hediff");
            hediffDefInfo.SetValue(forcedHediffPart, hediffDef);

            FieldInfo severityRangeInfo = AccessTools.Field(typeof(ScenPart_ForcedHediff), "severityRange");
            severityRangeInfo.SetValue(forcedHediffPart, severityRange);

            return forcedHediffPart;
        }

        public static StartWithHonor MakeStartWithHonorPart(FactionDef factionDef, int startingHonor, bool applyTitles = true)
        {
            StartWithHonor startWithHonorPart = (StartWithHonor)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_StartWithHonor);
            startWithHonorPart.StartingHonor = startingHonor;
            startWithHonorPart.FactionToStartWithHonorFor = factionDef;
            startWithHonorPart.ApplyTitles = applyTitles;
            return startWithHonorPart;
        }

        public static ScenPart_StartingThing_Defined MakeStartingThingDefinedPart(ThingDef thingDef, ThingDef stuff = null, int count = 1, QualityCategory? quality = null)
        {
            ScenPart_StartingThing_Defined startingThingPart = (ScenPart_StartingThing_Defined)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.StartingThing_Defined);

            FieldInfo thingDefInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "thingDef");
            thingDefInfo.SetValue(startingThingPart, thingDef);

            FieldInfo stuffInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "stuff");
            stuffInfo.SetValue(startingThingPart, stuff);

            FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "count");
            countInfo.SetValue(startingThingPart, count);

            FieldInfo qualityInfo = AccessTools.Field(typeof(ScenPart_StartingThing_Defined), "quality");
            qualityInfo.SetValue(startingThingPart, quality);

            return startingThingPart;
        }

        public static ScenPart_ScatterThingsNearPlayerStart MakeScatterThingsNearPlayerPart(ThingDef thingDef, ThingDef stuff = null, int count = 1, QualityCategory? quality = null, bool allowRoofed = false)
        {
            ScenPart_ScatterThingsNearPlayerStart scatterThingsNearPlayerPart = (ScenPart_ScatterThingsNearPlayerStart)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.ScatterThingsNearPlayerStart);

            FieldInfo thingDefInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "thingDef");
            thingDefInfo.SetValue(scatterThingsNearPlayerPart, thingDef);

            FieldInfo stuffInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "stuff");
            stuffInfo.SetValue(scatterThingsNearPlayerPart, stuff);

            FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "count");
            countInfo.SetValue(scatterThingsNearPlayerPart, count);

            FieldInfo qualityInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "quality");
            qualityInfo.SetValue(scatterThingsNearPlayerPart, quality);

            FieldInfo allowRoofedInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsNearPlayerStart), "allowRoofed");
            allowRoofedInfo.SetValue(scatterThingsNearPlayerPart, allowRoofed);

            return scatterThingsNearPlayerPart;
        }

        public static ScenPart_ScatterThingsAnywhere MakeScatterThingsAnywherePart(ThingDef thingDef, ThingDef stuff = null, int count = 1, QualityCategory? quality = null, bool allowRoofed = false)
        {
            ScenPart_ScatterThingsAnywhere scatterThingsAnywherePart = (ScenPart_ScatterThingsAnywhere)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.ScatterThingsAnywhere);

            FieldInfo thingDefInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "thingDef");
            thingDefInfo.SetValue(scatterThingsAnywherePart, thingDef);

            FieldInfo stuffInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "stuff");
            stuffInfo.SetValue(scatterThingsAnywherePart, stuff);

            FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "count");
            countInfo.SetValue(scatterThingsAnywherePart, count);

            FieldInfo qualityInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "quality");
            qualityInfo.SetValue(scatterThingsAnywherePart, quality);

            FieldInfo allowRoofedInfo = AccessTools.Field(typeof(ScenPart_ScatterThingsAnywhere), "allowRoofed");
            allowRoofedInfo.SetValue(scatterThingsAnywherePart, allowRoofed);

            return scatterThingsAnywherePart;
        }

        /// <param name="animalKind">The def of the animal to keep as pet. Null means "Random pet".</param>
        public static ScenPart_StartingAnimal MakeStartingAnimalPart(PawnKindDef animalKind = null, int count = 1, float bondToRandomPlayerPawnChance = 1f)
        {
            ScenPart_StartingAnimal startingAnimalPart = (ScenPart_StartingAnimal)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.StartingAnimal);

            FieldInfo animalKindInfo = AccessTools.Field(typeof(ScenPart_StartingAnimal), "animalKind");
            animalKindInfo.SetValue(startingAnimalPart, animalKind);

            FieldInfo countInfo = AccessTools.Field(typeof(ScenPart_StartingAnimal), "count");
            countInfo.SetValue(startingAnimalPart, count);

            return startingAnimalPart;
        }

        public static ScenPart_GameStartDialog MakeGameStartDialogPart(string text = null, string textKey = null, SoundDef closeSound = null)
        {
            if (closeSound == null)
            {
                closeSound = SoundDefOf.GameStartSting;
            }

            ScenPart_GameStartDialog gameStartDialogPart = (ScenPart_GameStartDialog)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.GameStartDialog);

            FieldInfo textInfo = AccessTools.Field(typeof(ScenPart_GameStartDialog), "text");
            textInfo.SetValue(gameStartDialogPart, text);

            FieldInfo textKeyInfo = AccessTools.Field(typeof(ScenPart_GameStartDialog), "textKey");
            textKeyInfo.SetValue(gameStartDialogPart, textKey);

            FieldInfo closeSoundInfo = AccessTools.Field(typeof(ScenPart_GameStartDialog), "closeSound");
            closeSoundInfo.SetValue(gameStartDialogPart, closeSound);

            return gameStartDialogPart;
        }
    }
}
