using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
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

        public static ScenPart_Naked MakeNakedPart(float chance = 1f, PawnGenerationContext context = PawnGenerationContext.PlayerStarter)
        {
            ScenPart_Naked startNakedPart = (ScenPart_Naked)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Naked);

            FieldInfo chanceInfo = AccessTools.Field(typeof(ScenPart_Naked), "chance");
            chanceInfo.SetValue(startNakedPart, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ScenPart_Naked), "context");
            contextInfo.SetValue(startNakedPart, context);

            return startNakedPart;
        }

        public static ScenPart_NoPossessions MakeNoPossessionsPart()
        {
            ScenPart_NoPossessions noPossessionsPart = (ScenPart_NoPossessions)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.NoPossessions);
            return noPossessionsPart;
        }

        public static ScenPart_GameCondition MakeGameConditionPart(GameConditionDef gameConditionDef, FloatRange durationRandomRange, bool gameConditionTargetsWorld = true)
        {
            ScenPart_GameCondition gameConditionPart = (ScenPart_GameCondition)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.GameCondition_Planetkiller);

            FieldInfo gameConditionInfo = AccessTools.Field(typeof(ScenPart_GameCondition), "gameCondition");
            gameConditionInfo.SetValue(gameConditionPart, gameConditionDef);

            FieldInfo durationRandomRangeInfo = AccessTools.Field(typeof(ScenPart_GameCondition), "durationRandomRange");
            durationRandomRangeInfo.SetValue(gameConditionPart, durationRandomRange);

            FieldInfo gameConditionTargetsWorldInfo = AccessTools.Field(typeof(ScenPart_GameCondition), "gameConditionTargetsWorld");
            gameConditionTargetsWorldInfo.SetValue(gameConditionPart, gameConditionTargetsWorld);

            return gameConditionPart;
        }

        public static NoNeutralFactionsExcept MakeNoNeutralFactionsExceptPart(List<FactionDef> factionsToKeep = null)
        {
            NoNeutralFactionsExcept part = (NoNeutralFactionsExcept)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_NoNeutralFactionsExcept);
            if (factionsToKeep != null)
            {
                part.ExceptFactions = factionsToKeep;
            }
            return part;
        }

        public static NoHostileFactionsExcept MakeNoHostileFactionsExceptPart(List<FactionDef> factionsToKeep = null)
        {
            NoHostileFactionsExcept part = (NoHostileFactionsExcept)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_NoHostileFactionsExcept);
            if (factionsToKeep != null)
            {
                part.ExceptFactions = factionsToKeep;
            }
            return part;
        }

        public static FactionNaturalGoodwill MakeFactionNaturalGoodwillPart(FactionDef factionDef, int goodwill)
        {
            FactionNaturalGoodwill part = (FactionNaturalGoodwill)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_FactionNaturalGoodwill);
            part.FactionToAffect = factionDef;
            part.Goodwill = goodwill;
            return part;
        }

        public static FactionForcedGoodwill MakeFactionForcedGoodwillPart(FactionDef factionDef, int goodwill)
        {
            FactionForcedGoodwill part = (FactionForcedGoodwill)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_FactionForcedGoodwill);
            part.FactionToAffect = factionDef;
            part.Goodwill = goodwill;
            return part;
        }

        public static RemoveFaction MakeRemoveFactionPart(FactionDef factionDef)
        {
            RemoveFaction part = (RemoveFaction)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_RemoveFaction);
            part.Faction = factionDef;
            return part;
        }

        public static ScenPart_DisableIncident MakeDisableIncidentPart(IncidentDef incidentDef)
        {
            ScenPart_DisableIncident part = (ScenPart_DisableIncident)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.DisableIncident);

            FieldInfo incidentInfo = AccessTools.Field(typeof(ScenPart_DisableIncident), "incident");
            incidentInfo.SetValue(part, incidentDef);

            return part;

        }

        public static DisableShipStartup MakeDisableShipStartupPart()
        {
            DisableShipStartup part = (DisableShipStartup)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_DisableShipStartup);
            return part;
        }
    }
}
