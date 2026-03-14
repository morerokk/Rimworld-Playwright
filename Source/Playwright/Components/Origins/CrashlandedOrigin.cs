using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class CrashlandedOrigin : OriginComponent
    {
        public override string Id => "Origins.Crashlanded";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // With this much hardcoding we're basically doing all the stuff the game normally would do in XML, but does it really matter?
            // TODO: What if mods use xpath stuff to add new things to the default scenarios? Should we still load the default scenarios anyway where it applies?

            // Start with cryptosleep sickness
            scenarioParts.Add(ScenPartUtility.MakeForcedHediffPart(HediffDefOf.CryptosleepSickness, PawnGenerationContext.PlayerStarter, 0.5f, new FloatRange(1f, 1f)));

            // Start with a bunch of stuff
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(RimWorld.ThingDefOf.Silver, null, 800));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(RimWorld.ThingDefOf.MealSurvivalPack, null, 50));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(RimWorld.ThingDefOf.MedicineIndustrial, null, 30));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(RimWorld.ThingDefOf.ComponentIndustrial, null, 30));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(DefOfs.ThingDefOf.Gun_BoltActionRifle, null, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(DefOfs.ThingDefOf.Gun_Revolver, null, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(RimWorld.ThingDefOf.MeleeWeapon_Knife, RimWorld.ThingDefOf.Plasteel, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(DefOfs.ThingDefOf.Apparel_FlakPants, null, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(DefOfs.ThingDefOf.Apparel_FlakVest, null, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingThingDefinedPart(DefOfs.ThingDefOf.Apparel_AdvancedHelmet, RimWorld.ThingDefOf.Plasteel, 1));

            // Start with a random pet
            scenarioParts.Add(ScenPartUtility.MakeStartingAnimalPart(null, 1, 1f));

            // Scatter random steel and wood around the start
            scenarioParts.Add(ScenPartUtility.MakeScatterThingsNearPlayerPart(RimWorld.ThingDefOf.Steel, null, 450));
            scenarioParts.Add(ScenPartUtility.MakeScatterThingsNearPlayerPart(RimWorld.ThingDefOf.WoodLog, null, 300));

            // Scatter random junk anywhere
            scenarioParts.Add(ScenPartUtility.MakeScatterThingsAnywherePart(RimWorld.ThingDefOf.ShipChunk, null, 3));
            scenarioParts.Add(ScenPartUtility.MakeScatterThingsAnywherePart(RimWorld.ThingDefOf.Steel, null, 720));
            scenarioParts.Add(ScenPartUtility.MakeScatterThingsAnywherePart(RimWorld.ThingDefOf.MealSurvivalPack, null, 7));

            // Game start dialog
            scenarioParts.Add(ScenPartUtility.MakeGameStartDialogPart(null, "GameStartDialog"));
        }
    }
}
