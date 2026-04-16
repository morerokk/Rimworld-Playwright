using HarmonyLib;
using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public static StartWithHonor MakeStartWithHonorPart(FactionDef factionDef, int startingHonor, float chance, PawnGenerationContext context, bool applyTitles = true)
        {
            StartWithHonor part = (StartWithHonor)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_StartWithHonor);
            part.StartingHonor = startingHonor;
            part.FactionToStartWithHonorFor = factionDef;
            part.ApplyTitles = applyTitles;

            FieldInfo chanceInfo = AccessTools.Field(typeof(ForcedPsylinkLevel), "chance");
            chanceInfo.SetValue(part, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ForcedPsylinkLevel), "context");
            contextInfo.SetValue(part, context);

            return part;
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

        public static ScenPart_GameCondition MakeGameConditionPlanetkillerPart(float durationDays)
        {
            // This scenpart is really weird, seemingly made for generic game conditions but then coded to only ever use planetkiller.
            // I guess it's 1 class meant to be reused by multiple scenpart defs?
            // The game condition comes from the scenpart's def (including whether it targets the world),
            // game condition starts at 0 days, lasts until durationDays.
            ScenPart_GameCondition gameConditionPart = (ScenPart_GameCondition)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.GameCondition_Planetkiller);

            FieldInfo durationDaysInfo = AccessTools.Field(typeof(ScenPart_GameCondition), "durationDays");
            durationDaysInfo.SetValue(gameConditionPart, durationDays);

            return gameConditionPart;
        }

        public static ScenPart_OnPawnDeathExplode MakeOnPawnDeathExplodePart(DamageDef damageDef = null, float radius = 5.9f)
        {
            if (damageDef == null)
            {
                damageDef = DamageDefOf.Bomb;
            }

            ScenPart_OnPawnDeathExplode part = (ScenPart_OnPawnDeathExplode)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.OnPawnDeathExplode);

            FieldInfo damageInfo = AccessTools.Field(typeof(ScenPart_OnPawnDeathExplode), "damage");
            damageInfo.SetValue(part, damageDef);

            FieldInfo radiusInfo = AccessTools.Field(typeof(ScenPart_OnPawnDeathExplode), "radius");
            damageInfo.SetValue(part, radius);

            return part;
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

        public static ScenPart_StartingResearch MakeStartingResearchPart(ResearchProjectDef researchProjectDef)
        {
            ScenPart_StartingResearch part = (ScenPart_StartingResearch)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.StartingResearch);

            FieldInfo projectInfo = AccessTools.Field(typeof(ScenPart_StartingResearch), "project");
            projectInfo.SetValue(part, researchProjectDef);

            return part;
        }

        public static ScenPart_PursuingMechanoids MakePursuingMechanoidsPart()
        {
            ScenPart_PursuingMechanoids part = (ScenPart_PursuingMechanoids)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.PursuingMechanoids);
            return part;
        }

        public static DisableShipStartup MakeDisableShipStartupPart()
        {
            DisableShipStartup part = (DisableShipStartup)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_DisableShipStartup);
            return part;
        }

        public static StartWithNonMinifiedThing MakeStartWithNonMinifiedThingPart(ThingDef thingDef)
        {
            StartWithNonMinifiedThing part = (StartWithNonMinifiedThing)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_StartWithNonMinifiedThing);
            part.thingDef = thingDef;
            return part;
        }

        public static ForcedPsylinkLevel MakeForcedPsylinkLevelPart(FloatRange severityRange, float chance, PawnGenerationContext context)
        {
            ForcedPsylinkLevel part = (ForcedPsylinkLevel)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_ForcedPsylinkLevel);
            part.SeverityRange = severityRange;

            FieldInfo chanceInfo = AccessTools.Field(typeof(ForcedPsylinkLevel), "chance");
            chanceInfo.SetValue(part, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ForcedPsylinkLevel), "context");
            contextInfo.SetValue(part, context);

            return part;
        }

        public static ForcedImplant MakeForcedImplantPart(HediffDef hediffDef, float chance, PawnGenerationContext context, BodyPartDef bodyPartDef = null, ForcedImplant.ImplantSide? implantSide = null)
        {
            if (implantSide == null)
            {
                implantSide = ForcedImplant.ImplantSide.Left;
            }

            ForcedImplant part = (ForcedImplant)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_ForcedImplant);
            part.Hediff = hediffDef;
            part.BodyPart = bodyPartDef;
            part.Side = implantSide.Value;

            FieldInfo chanceInfo = AccessTools.Field(typeof(ForcedImplant), "chance");
            chanceInfo.SetValue(part, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ForcedImplant), "context");
            contextInfo.SetValue(part, context);

            return part;
        }

        public static ReplaceXenotype MakeReplaceXenotypePart(XenotypeDef fromXenotype, XenotypeDef toXenotype, float chance, PawnGenerationContext context)
        {
            ReplaceXenotype part = (ReplaceXenotype)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_ReplaceXenotype);
            part.FromXenotype = fromXenotype;
            part.ToXenotype = toXenotype;

            FieldInfo chanceInfo = AccessTools.Field(typeof(ForcedImplant), "chance");
            chanceInfo.SetValue(part, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ForcedImplant), "context");
            contextInfo.SetValue(part, context);

            return part;
        }

        public static ReplaceXenotypeWithCustom MakeReplaceXenotypeWithCustomPart(XenotypeDef fromXenotype, CustomXenotype toCustomXenotype, float chance, PawnGenerationContext context)
        {
            ReplaceXenotypeWithCustom part = (ReplaceXenotypeWithCustom)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_ReplaceXenotypeWithCustom);
            part.FromXenotype = fromXenotype;
            part.ToCustomXenotype = toCustomXenotype;

            FieldInfo chanceInfo = AccessTools.Field(typeof(ForcedImplant), "chance");
            chanceInfo.SetValue(part, chance);

            FieldInfo contextInfo = AccessTools.Field(typeof(ForcedImplant), "context");
            contextInfo.SetValue(part, context);

            return part;
        }

        public static NoColonistRerolls MakeNoColonistRerollsPart()
        {
            NoColonistRerolls part = (NoColonistRerolls)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_NoColonistRerolls);
            return part;
        }

        public static AdditionalUnwaveringChance MakeAdditionalUnwaveringChancePart(float chance)
        {
            AdditionalUnwaveringChance part = (AdditionalUnwaveringChance)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_AdditionalUnwaveringChance);
            part.Chance = chance;
            return part;
        }

        public static WinCondition_Colony MakeWinConditionColonyPart(int colonists)
        {
            WinCondition_Colony part = (WinCondition_Colony)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_WinCondition_Colony);
            part.Colonists = colonists;
            return part;
        }

        public static WinCondition_Conquest MakeWinConditionConquestPart(bool allowAllies)
        {
            WinCondition_Conquest part = (WinCondition_Conquest)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_WinCondition_Conquest);
            part.AllowAllies = allowAllies;
            return part;
        }

        public static WinCondition_RoyalTitles MakeWinConditionRoyalTitlesPart(int colonists, FactionDef faction, RoyalTitleDef title)
        {
            WinCondition_RoyalTitles part = (WinCondition_RoyalTitles)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_WinCondition_RoyalTitles);
            part.Colonists = colonists;
            part.Faction = faction;
            part.Title = title;
            return part;
        }

        public static WinCondition_SellItems MakeWinConditionSellItemsPart(ThingDef thingToSell, int amountToSell)
        {
            WinCondition_SellItems part = (WinCondition_SellItems)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_WinCondition_SellItems);
            part.Thing = thingToSell;
            part.Amount = amountToSell;
            return part;
        }

        public static WinCondition_Time MakeWinConditionTimePart(int days)
        {
            WinCondition_Time part = (WinCondition_Time)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Playwright_WinCondition_Time);
            part.Days = days;
            return part;
        }

        /// <summary>
        /// Converts an existing <see cref="ScenPart_ConfigPage_ConfigureStartingPawns"/> part into a <see cref="ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes"/> part.
        /// The new part has no specific demands for pawn kinds or xenotypes and has the same choice counts as the original.
        /// You can add new entries to XenotypeCounts at will.
        /// Requires Biotech, will throw an error otherwise.
        /// </summary>
        public static ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes ConvertConfigureStartingPawnsToXenotypes(ScenPart_ConfigPage_ConfigureStartingPawns originalPart)
        {
            ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes part = (ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.ConfigurePawnsXenotypes);
            part.pawnChoiceCount = originalPart.pawnChoiceCount;
            part.xenotypeCounts.Add(new XenotypeCount()
            {
                count = originalPart.pawnCount,
                countBuffer = originalPart.pawnCount.ToString(),
                xenotype = XenotypeDefOf.Baseliner
            });

            return part;
        }

        public static ScenPart_ConfigPage_ConfigureStartingPawnsBase GetConfigureStartingPawnsPart(Scenario scenario)
        {
            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;
            return GetConfigureStartingPawnsPart(parts);
        }

        public static ScenPart_ConfigPage_ConfigureStartingPawnsBase GetConfigureStartingPawnsPart(List<ScenPart> parts)
        {
            ScenPart_ConfigPage_ConfigureStartingPawnsBase result = parts
                .Where(part =>
                    part.def == ScenPartDefOf.ConfigPage_ConfigureStartingPawns
                    || (ModsConfig.BiotechActive && part.def == DefOfs.ScenPartDefOf.ConfigurePawnsXenotypes)
                    || (ModsConfig.BiotechActive && part.def == DefOfs.ScenPartDefOf.ConfigurePawnsKindDefs)
                    || (ModsConfig.AnomalyActive && part.def == DefOfs.ScenPartDefOf.ConfigurePawnsMutants)
                )
                .Cast<ScenPart_ConfigPage_ConfigureStartingPawnsBase>()
                .FirstOrDefault();

            if (result == null)
            {
                // Try harder! Just get the first part that matches, is slower but works with modded configpages
                result = parts
                    .Where(part => part is ScenPart_ConfigPage_ConfigureStartingPawnsBase)
                    .Cast<ScenPart_ConfigPage_ConfigureStartingPawnsBase>()
                    .FirstOrDefault();
            }

            return result;
        }
    }
}
