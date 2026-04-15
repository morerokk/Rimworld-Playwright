using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.ErrorOriginNull".Translate()));
                throw new PlaywrightBuilderException("Playwright Origin was null, did you select an Origin/Scenario?", playwright);
            }

            Scenario scenario = this.GenerateDefaultScenario(playwright);

            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            HookRegistration.CallScenarioPreMutated(playwright, scenario, parts);

            // Origin
            playwright.Origin.MutateScenario(scenario, parts);

            // Boons
            this.ProcessBoons(playwright, scenario, parts);

            // Factions
            HookRegistration.CallScenarioPreFaction(playwright, scenario, parts);
            this.ProcessFactions(playwright, scenario, parts);
            HookRegistration.CallScenarioPostFaction(playwright, scenario, parts);

            // Win conditions
            HookRegistration.CallScenarioPreWinCondition(playwright, scenario, parts);
            this.ProcessWinConditions(playwright, scenario, parts);
            HookRegistration.CallScenarioPostWinCondition(playwright, scenario, parts);

            // Special conditions
            this.ProcessSpecialConditions(playwright, scenario, parts);

            HookRegistration.CallScenarioPostMutated(playwright, scenario, parts);

            return scenario;
        }

        private void ProcessBoons(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            foreach (BoonComponent boon in playwright.Boons)
            {
                boon.MutateScenario(scenario, parts);
            }
        }

        private void ProcessFactions(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            // Separate processing for the absence of the "(All Others)" faction, strip out all factions that weren't explicitly chosen
            if (!playwright.NeutralFactions.Any(fc => fc.Id == AllOtherFactions.ComponentId))
            {
                var factionsToKeep = PlaywrightUtils.GetDefaultNeutralFactions()
                    .Where(factionDef => playwright.IsFactionSelectedAnywhere(factionDef))
                    .ToList();
                parts.Add(ScenPartUtility.MakeNoNeutralFactionsExceptPart(factionsToKeep));
            }
            if (!playwright.EnemyFactions.Any(fc => fc.Id == AllOtherFactions.ComponentId))
            {
                var factionsToKeep = PlaywrightUtils.GetDefaultEnemyFactions()
                    .Where(factionDef => playwright.IsFactionSelectedAnywhere(factionDef))
                    .ToList();
                parts.Add(ScenPartUtility.MakeNoHostileFactionsExceptPart(factionsToKeep));
            }

            // If mechanoids/insectoids weren't chosen, add parts for that
            if (!playwright.EnemyFactions.Any(f => f.Id == InsectoidHiveFaction.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeRemoveFactionPart(FactionDefOf.Insect));
                parts.Add(ScenPartUtility.MakeDisableIncidentPart(IncidentDefOf.Infestation));
                // There's an ideology-only incident called IncidentDefOf.Infestation_Jelly. Can someone let me know what this is? Ritual reward?
                // Either way, doesn't matter if we don't add it
            }
            if (!playwright.EnemyFactions.Any(f => f.Id == MechanoidHiveFaction.ComponentId))
            {
                parts.Add(ScenPartUtility.MakeRemoveFactionPart(FactionDefOf.Mechanoid));
                if (ModsConfig.RoyaltyActive)
                {
                    parts.Add(ScenPartUtility.MakeDisableIncidentPart(IncidentDefOf.MechCluster));
                }
            }

            // For any factions that remain, set their dispositions

            // Ally
            List<FactionComponent> allyFactions = playwright.AllyFactions
                .Where(fc => fc.Id != AllOtherFactions.ComponentId)
                .Where(f => f.FactionDef != null)
                .ToList();

            foreach (FactionComponent allyFaction in allyFactions)
            {
                if (allyFaction.AllowForcedDisposition && allyFaction.ForceDisposition)
                {
                    parts.Add(ScenPartUtility.MakeFactionForcedGoodwillPart(allyFaction.FactionDef, 100));
                }
                else
                {
                    parts.Add(ScenPartUtility.MakeFactionNaturalGoodwillPart(allyFaction.FactionDef, 80));
                }
                allyFaction.MutateScenario(scenario, parts);
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
                neutralFaction.MutateScenario(scenario, parts);
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
                enemyFaction.MutateScenario(scenario, parts);
            }
        }

        private void ProcessWinConditions(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            // If ship wasn't chosen, disable the journey offer quest.
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

            // Process all other win conditions
            foreach (WinConditionComponent winCondition in playwright.WinConditions)
            {
                winCondition.MutateScenario(scenario, parts);
            }
        }

        private void ProcessSpecialConditions(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> parts)
        {
            foreach (SpecialConditionComponent specialCondition in playwright.SpecialConditions)
            {
                specialCondition.MutateScenario(scenario, parts);
            }
        }

        private Scenario GenerateDefaultScenario(PlaywrightStructure playwright)
        {
            Scenario scenario = null;
            OriginComponent origin = playwright.Origin;
            if (origin.BasedOnScenario != null)
            {
                scenario = origin.BasedOnScenario.scenario.CopyForEditing();
            }
            else
            {
                scenario = GenerateDefaultishScenario();
            }

            if (scenario == null)
            {
                throw new PlaywrightBuilderException($"Unable to get a base scenario from the selected Playwright's origin. Origin was '{origin.Id}'.", playwright);
            }

            scenario.Category = ScenarioCategory.CustomLocal;
            // Set name, description and summary from Playwright if provided, otherwise use default from origin
            if (!string.IsNullOrWhiteSpace(playwright.Name))
            {
                scenario.name = playwright.Name;
            }
            else
            {
                scenario.name = "Playwright.ScenarioNamePrefix".Translate().ToString() + NameGenerator.GenerateName(RulePackDefOf.NamerScenario, null, false, null, null, null);
            }
            if (!string.IsNullOrWhiteSpace(playwright.Description))
            {
                scenario.description = playwright.Description;
            }
            else
            {
                scenario.description = origin.DescriptionTranslated;
            }
            if (!string.IsNullOrWhiteSpace(playwright.DescriptionShort))
            {
                scenario.summary = playwright.DescriptionShort;
            }
            else
            {
                scenario.summary = origin.DescriptionShortTranslated;
            }

            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            // Change the game start dialog IF it has been provided, otherwise we'll just leave it as-is
            if (!string.IsNullOrWhiteSpace(playwright.GameStartDialogText))
            {
                ScenPart_GameStartDialog dialogPart = parts
                    .Where(part => part.def == DefOfs.ScenPartDefOf.GameStartDialog)
                    .Cast<ScenPart_GameStartDialog>()
                    .FirstOrDefault();

                if (dialogPart == null)
                {
                    dialogPart = ScenPartUtility.MakeGameStartDialogPart(playwright.GameStartDialogText, null, SoundDefOf.GameStartSting);
                    parts.Add(dialogPart);
                }
                else
                {
                    FieldInfo textInfo = AccessTools.Field(typeof(ScenPart_GameStartDialog), "text");
                    textInfo.SetValue(dialogPart, playwright.GameStartDialogText);

                    FieldInfo textKeyInfo = AccessTools.Field(typeof(ScenPart_GameStartDialog), "textKey");
                    textKeyInfo.SetValue(dialogPart, null);
                }
            }

            // Set player faction
            if (origin.PlayerFaction != null)
            {
                FieldInfo playerFactionInfo = AccessTools.Field(typeof(Scenario), "playerFaction");
                ScenPart_PlayerFaction playerFactionPart = (ScenPart_PlayerFaction)playerFactionInfo.GetValue(scenario);

                FieldInfo playerFactionPartFactionInfo = AccessTools.Field(typeof(ScenPart_PlayerFaction), "factionDef");
                playerFactionPartFactionInfo.SetValue(playerFactionPart, origin.PlayerFaction);
            }

            // Selectable pawn count (3 out of 8, etc)
            if (origin.StartingColonistsSelectable != null || origin.StartingColonistsTotal != null)
            {
                ScenPart_ConfigPage_ConfigureStartingPawns startingPawnsConfigurePart = (ScenPart_ConfigPage_ConfigureStartingPawns)parts
                    .First(p => p.def == ScenPartDefOf.ConfigPage_ConfigureStartingPawns);
                if (origin.StartingColonistsSelectable != null)
                {
                    startingPawnsConfigurePart.pawnCount = origin.StartingColonistsSelectable.Value;
                }
                if (origin.StartingColonistsTotal != null)
                {
                    startingPawnsConfigurePart.pawnChoiceCount = origin.StartingColonistsTotal.Value;
                }
            }

            // Arrival method (Crashland in pods, be already standing, gravship, etc)
            if (origin.ArrivalMethod != null)
            {
                ScenPart_PlayerPawnsArriveMethod pawnArriveMethodPart = (ScenPart_PlayerPawnsArriveMethod)parts
                    .First(part => part.def == ScenPartDefOf.PlayerPawnsArriveMethod);

                FieldInfo arrivalMethodInfo = AccessTools.Field(typeof(ScenPart_PlayerPawnsArriveMethod), "method");
                arrivalMethodInfo.SetValue(pawnArriveMethodPart, origin.ArrivalMethod.Value);
            }

            return scenario;
        }

        /// <summary>
        /// Generate a barebones "default-ish" Scenario.
        /// This scenario is based on Naked Brutality, but without the naked or no possessions parts.
        /// </summary>
        public static Scenario GenerateDefaultishScenario()
        {
            Scenario scenario = DefOfs.ScenarioDefOf.NakedBrutality.scenario.CopyForEditing();

            FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");
            List<ScenPart> parts = partsInfo.GetValue(scenario) as List<ScenPart>;

            parts.RemoveAll(part =>
                part.def == DefOfs.ScenPartDefOf.Naked
                || part.def == DefOfs.ScenPartDefOf.NoPossessions
            );

            return scenario;
        }
    }
}
