using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Composer
{
    /// <summary>
    /// Compiles a PlaywrightStructure into a scenario.
    /// </summary>
    public class PlaywrightBuilder
    {
        public Scenario MakeScenario(PlaywrightStructure playwright)
        {
            if (playwright == null)
            {
                throw new ArgumentNullException(nameof(playwright), "playwright cannot be null");
            }

            if (playwright.Origin == null)
            {
                throw new PlaywrightBuilderException("Playwright Origin was null", playwright);
            }

            Scenario scenario = this.GenerateDefaultScenario(playwright);

            // This is ugly but the game accesses this directly too, it just has an internal access modifier on it so we have to use reflection
            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            HookRegistration.CallScenarioPreMutated(playwright, scenario, parts);

            // Origin
            playwright.Origin.MutateScenario(scenario, parts);

            // Boons
            foreach (BoonComponent boon in playwright.Boons)
            {
                boon.MutateScenario(scenario, parts);
            }

            // Factions
            if (playwright.CustomizeFactions)
            {
                HookRegistration.CallScenarioPreFaction(playwright, scenario, parts);
                this.ProcessFactions(playwright, scenario, parts);
                HookRegistration.CallScenarioPostFaction(playwright, scenario, parts);
            }

            // Win conditions
            if (playwright.CustomizeWinConditions)
            {
                this.ProcessWinConditions(playwright, scenario, parts);
            }

            HookRegistration.CallScenarioPostMutated(playwright, scenario, parts);

            return scenario;
        }

        private void ProcessFactions(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            // Separate processing for the (All Others) faction, strip out all factions that weren't chosen
            if (!playwright.NeutralFactions.Any(fc => fc.Id == AllOtherFactions.ComponentId))
            {
                List<FactionDef> factionsToKeep = playwright.NeutralFactions
                    .Where(f => f.FactionDef != null)
                    .Select(f => f.FactionDef)
                    .ToList();
                parts.Add(ScenPartUtility.MakeNoNeutralFactionsExceptPart(factionsToKeep));
            }
            if (!playwright.EnemyFactions.Any(fc => fc.Id == AllOtherFactions.ComponentId))
            {
                List<FactionDef> factionsToKeep = playwright.EnemyFactions
                    .Where(f => f.FactionDef != null)
                    .Select(f => f.FactionDef)
                    .ToList();
                parts.Add(ScenPartUtility.MakeNoHostileFactionsExceptPart(factionsToKeep));
            }

            // If mechanoids/insectoids weren't chosen, add parts for that
            if (!playwright.EnemyFactions.Any(f => f.Id == InsectoidHiveFaction.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeRemoveFactionPart(FactionDefOf.Insect));
            }
            if (!playwright.EnemyFactions.Any(f => f.Id == MechanoidHiveFaction.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeRemoveFactionPart(FactionDefOf.Mechanoid));
            }

            // For any factions that remain, set their dispositions

            // Ally
            List<FactionComponent> allyFactions = playwright.AllyFactions
                .Where(fc => fc.Id != AllOtherFactions.ComponentId)
                .Where(f => f.FactionDef != null)
                .ToList();

            foreach(FactionComponent allyFaction in allyFactions)
            {
                if (allyFaction.AllowForcedDisposition && allyFaction.ForceDisposition)
                {
                    parts.Add(ScenPartUtility.MakeFactionForcedGoodwillPart(allyFaction.FactionDef, 100));
                }
                else
                {
                    parts.Add(ScenPartUtility.MakeFactionNaturalGoodwillPart(allyFaction.FactionDef, 80));
                }
            }

            // Neutral
            List<FactionComponent> neutralFactions = playwright.NeutralFactions
                .Where(fc => fc.Id != AllOtherFactions.ComponentId)
                .Where(f => f.FactionDef != null)
                .ToList();

            foreach (FactionComponent neutralFaction in neutralFactions)
            {
                if (neutralFaction.AllowForcedDisposition && neutralFaction.ForceDisposition)
                {
                    parts.Add(ScenPartUtility.MakeFactionForcedGoodwillPart(neutralFaction.FactionDef, 0));
                }
                else
                {
                    parts.Add(ScenPartUtility.MakeFactionNaturalGoodwillPart(neutralFaction.FactionDef, 0));
                }
            }

            // Hostile
            List<FactionComponent> enemyFactions = playwright.EnemyFactions
                .Where(fc => fc.Id != AllOtherFactions.ComponentId && fc.Id != InsectoidHiveFaction.ComponentId && fc.Id != MechanoidHiveFaction.ComponentId)
                .Where(f => f.FactionDef != null)
                .ToList();

            foreach (FactionComponent enemyFaction in enemyFactions)
            {
                if (enemyFaction.AllowForcedDisposition && enemyFaction.ForceDisposition)
                {
                    parts.Add(ScenPartUtility.MakeFactionForcedGoodwillPart(enemyFaction.FactionDef, -100));
                }
                else
                {
                    parts.Add(ScenPartUtility.MakeFactionNaturalGoodwillPart(enemyFaction.FactionDef, -80));
                }
            }
        }

        private void ProcessWinConditions(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            // If ship wasn't chosen, disable the journey offer quest
            // Not being allowed to start the ship is a separate patch,
            // it should still be researchable and buildable for the reactor and mod compatibility reasons.
            if (!playwright.WinConditions.Any(wc => wc.Id == ShipWinCondition.ComponentId))
            {
                // Disable journey offer quest-giver incident
                parts.Add(ScenPartUtility.MakeDisableIncidentPart(DefOfs.IncidentDefOf.GiveQuest_EndGame_ShipEscape));
                // Disable the ship startup sequence (handled in patches separately)
                parts.Add(ScenPartUtility.MakeDisableShipStartupPart());
            }

            // If Royal Ascent wasn't chosen, disable the quest-giver incident
            if (ModsConfig.RoyaltyActive && !playwright.WinConditions.Any(wc => wc.Id == RoyalAscentWinCondition.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeDisableIncidentPart(DefOfs.IncidentDefOf.GiveQuest_EndGame_RoyalAscent));
            }

            // If Archonexus wasn't chosen, disable the quest-giver incident
            if (ModsConfig.IdeologyActive && !playwright.WinConditions.Any(wc => wc.Id == ArchonexusWinCondition.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeDisableIncidentPart(DefOfs.IncidentDefOf.GiveQuest_EndGame_ArchonexusVictory));
            }
        }

        private Scenario GenerateDefaultScenario(PlaywrightStructure playwright)
        {
            // The below is mostly taken from ScenarioMaker.GenerateNewRandomScenario(),
            // except without the random parts (obviously)
            // I'd like to make this more compatible with stuff, but if I call the original method I unavoidably get all the other random dogshit with it.
            Scenario scenario = new Scenario();
            scenario.Category = ScenarioCategory.CustomLocal;
            scenario.name = "Playwright.ScenarioNamePrefix".Translate() + NameGenerator.GenerateName(RulePackDefOf.NamerScenario, null, false, null, null, null);
            scenario.description = playwright.Origin.DescriptionTranslated;
            scenario.summary = playwright.Origin.DescriptionShortTranslated;

            // Why is so much Scenario shit internal?
            // Get part list
            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            // Set player faction
            var playerFactionPart = (ScenPart_PlayerFaction)ScenarioMaker.MakeScenPart(ScenPartDefOf.PlayerFaction);
            FieldInfo playerFactionPartFactionInfo = AccessTools.Field(typeof(ScenPart_PlayerFaction), "factionDef");
            playerFactionPartFactionInfo.SetValue(playerFactionPart, playwright.Origin.PlayerFaction);

            FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
            playerFactionInfo.SetValue(scenario, playerFactionPart);
            
            // Selectable pawn count (3 out of 8, etc)
            var startingPawnsConfigurePart = (ScenPart_ConfigPage_ConfigureStartingPawns)ScenarioMaker.MakeScenPart(ScenPartDefOf.ConfigPage_ConfigureStartingPawns);
            startingPawnsConfigurePart.pawnCount = playwright.Origin.StartingColonistsSelectable;
            startingPawnsConfigurePart.pawnChoiceCount = playwright.Origin.StartingColonistsTotal;
            parts.Add(startingPawnsConfigurePart);

            // Arrival method (Crashland in pods, be already standing as tribals, etc)
            var pawnArriveMethodPart = (ScenPart_PlayerPawnsArriveMethod)ScenarioMaker.MakeScenPart(ScenPartDefOf.PlayerPawnsArriveMethod);
            FieldInfo arrivalMethodInfo = AccessTools.Field(typeof(ScenPart_PlayerPawnsArriveMethod), "method");
            arrivalMethodInfo.SetValue(pawnArriveMethodPart, playwright.Origin.ArrivalMethod);
            parts.Add(pawnArriveMethodPart);

            // Set planet layers with a surface layer
            var surfaceLayer = new ScenPart_PlanetLayer()
            {
                def = ScenPartDefOf.PlanetLayerFixed,
                layer = PlanetLayerDefOf.Surface,
                settingsDef = PlanetLayerSettingsDefOf.Surface,
                hide = true,
                tag = "Surface"
            };
            FieldInfo surfaceLayerInfo = AccessTools.Field(typeof(Scenario), "surfaceLayer");
            surfaceLayerInfo.SetValue(scenario, surfaceLayer);

            // If Odyssey is installed, add a new orbit layer and set it to connect to the surface layer by zooming in/out far enough
            // Does the game add this retroactively if you activate Odyssey mid-save? Fortunately not my problem since we do the same thing here as vanilla
            if (ModsConfig.OdysseyActive)
            {
                var planetLayer = new ScenPart_PlanetLayer()
                {
                    def = ScenPartDefOf.PlanetLayerFixed,
                    layer = PlanetLayerDefOf.Orbit,
                    settingsDef = PlanetLayerSettingsDefOf.Orbit,
                    hide = true,
                    tag = "Orbit"
                };
                parts.Add(planetLayer);

                surfaceLayer.connections.Add(new LayerConnection()
                {
                    tag = planetLayer.tag,
                    zoomMode = LayerConnection.ZoomMode.ZoomOut
                });
                planetLayer.connections.Add(new LayerConnection()
                {
                    tag = surfaceLayer.tag,
                    zoomMode = LayerConnection.ZoomMode.ZoomIn
                });
            }

            return scenario;
        }
    }
}
