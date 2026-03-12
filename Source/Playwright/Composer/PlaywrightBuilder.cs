using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Exceptions;
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

            Scenario scenario = GenerateDefaultScenario(playwright.Origin.StartingColonistsSelectable, playwright.Origin.StartingColonistsTotal, playwright.Origin.ArrivalMethod);

            // This is ugly but the game accesses this directly too, it just has an internal access modifier on it so we have to use reflection
            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            HookRegistration.CallScenarioPreMutated(playwright, scenario, parts);

            playwright.Origin.MutateScenario(scenario, parts);
            foreach (var boon in playwright.Boons)
            {
                boon.MutateScenario(scenario, parts);
            }

            HookRegistration.CallScenarioPostMutated(playwright, scenario, parts);

            return scenario;
        }

        private Scenario GenerateDefaultScenario(int selectablePawns = 3, int totalPawns = 8, PlayerPawnsArriveMethod arrivalMethod = PlayerPawnsArriveMethod.DropPods)
        {
            // The below is mostly taken from ScenarioMaker.GenerateNewRandomScenario(),
            // except without the random parts (obviously)
            // I'd like to make this more compatible with stuff, but if I call the original method I unavoidably get all the other random dogshit with it.
            Scenario scenario = new Scenario();
            scenario.Category = ScenarioCategory.CustomLocal;
            scenario.description = null;
            scenario.summary = null;

            // Why is this shit internal?
            // Get part list
            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            // Set player faction
            var playerFaction = (ScenPart_PlayerFaction)ScenarioMaker.MakeScenPart(ScenPartDefOf.PlayerFaction);
            FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
            playerFactionInfo.SetValue(scenario, playerFaction);

            // Selectable pawn count (3 out of 8, etc)
            var startingPawnsConfigurePart = (ScenPart_ConfigPage_ConfigureStartingPawns)ScenarioMaker.MakeScenPart(ScenPartDefOf.ConfigPage_ConfigureStartingPawns);
            startingPawnsConfigurePart.pawnCount = selectablePawns;
            startingPawnsConfigurePart.pawnChoiceCount = totalPawns;
            parts.Add(startingPawnsConfigurePart);

            // Arrival method (Crashland in pods, be already standing as tribals, etc)
            var pawnArriveMethodPart = (ScenPart_PlayerPawnsArriveMethod)ScenarioMaker.MakeScenPart(ScenPartDefOf.PlayerPawnsArriveMethod);
            FieldInfo arrivalMethodInfo = AccessTools.Field(typeof(ScenPart_PlayerPawnsArriveMethod), "method");
            arrivalMethodInfo.SetValue(pawnArriveMethodPart, arrivalMethod);
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
