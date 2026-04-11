using RimWorld;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Composer;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Rokk.Playwright.Addons
{
    /// <summary>
    /// Class for registering hook functions that will run at certain times during Playwright or scenario building.
    /// For example, you can run your own code right before/after the scenario is mutated by its parts.
    /// This should be used for things that don't fit in a single component, like code that requires knowledge of multiple components at once.
    /// If you prefer to use Harmony instead that's fine, but if you *can* use these methods you probably should.
    /// </summary>
    public static class HookRegistration
    {
        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPreMutatedHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();
        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPostMutatedHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();

        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPreFactionHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();
        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPostFactionHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();

        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPreWinConditionHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();
        private static List<Action<PlaywrightStructure, Scenario, List<ScenPart>>> ScenarioPostWinConditionHooks = new List<Action<PlaywrightStructure, Scenario, List<ScenPart>>>();

        private static List<Action<PlaywrightWindow, PlaywrightStructure, Rect>> PlaywrightWindowPreWindowContentsHooks = new List<Action<PlaywrightWindow, PlaywrightStructure, Rect>>();
        private static List<Action<PlaywrightWindow, PlaywrightStructure, Rect>> PlaywrightWindowPostWindowContentsHooks = new List<Action<PlaywrightWindow, PlaywrightStructure, Rect>>();

        private static List<Action<PlaywrightStructure>> PlaywrightDefaultStructureHooks = new List<Action<PlaywrightStructure>>();
        private static List<Action<PlaywrightStructure, OriginComponent>> PlaywrightChangeOriginHooks = new List<Action<PlaywrightStructure, OriginComponent>>();

        /// <summary>
        /// Register something that should happen before the <see cref="PlaywrightBuilder"/> starts mutating the initial "blank" scenario.
        /// Useful for doing some preparation that requires knowledge of the entire playwright and scenario.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPreMutated(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPreMutatedHooks.Add(hook);
        }

        /// <summary>
        /// Register something that should happen after the <see cref="PlaywrightBuilder"/> has finished building the structure into a scenario.
        /// Useful for doing some fix-ups that requires knowledge of the entire playwright and scenario.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPostMutated(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPostMutatedHooks.Add(hook);
        }

        // Internal-only functions to call registered hooks, these shouldn't be accidentally used from outside the assembly
        internal static void CallScenarioPreMutated(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPreMutatedHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        internal static void CallScenarioPostMutated(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPostMutatedHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        /// <summary>
        /// Register something that should be run before the <see cref="PlaywrightBuilder"/> starts mutating the scenario based on faction components.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPreFaction(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPreFactionHooks.Add(hook);
        }

        /// <summary>
        /// Register something that should be run after the <see cref="PlaywrightBuilder"/> has finished mutating the scenario based on faction components.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPostFaction(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPostFactionHooks.Add(hook);
        }

        internal static void CallScenarioPreFaction(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPreFactionHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        internal static void CallScenarioPostFaction(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPostFactionHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        /// <summary>
        /// Register something that should be run before the <see cref="PlaywrightBuilder"/> starts mutating the scenario based on win condition components.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPreWinCondition(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPreWinConditionHooks.Add(hook);
        }

        /// <summary>
        /// Register something that should be run after the <see cref="PlaywrightBuilder"/> has finished mutating the scenario based on win condition components.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterScenarioPostWinCondition(Action<PlaywrightStructure, Scenario, List<ScenPart>> hook)
        {
            ScenarioPostWinConditionHooks.Add(hook);
        }

        internal static void CallScenarioPreWinCondition(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPreWinConditionHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        internal static void CallScenarioPostWinCondition(PlaywrightStructure playwright, Scenario scenario, List<ScenPart> scenParts)
        {
            foreach (var hook in ScenarioPostWinConditionHooks)
            {
                hook(playwright, scenario, scenParts);
            }
        }

        /// <summary>
        /// Register something that should happen before the <see cref="PlaywrightWindow"/> starts drawing the window contents.
        /// This could be useful for mutating the Playwright structure to remove/resolve any incompatible selections, for example.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterPlaywrightWindowPreWindowContents(Action<PlaywrightWindow, PlaywrightStructure, Rect> hook)
        {
            PlaywrightWindowPreWindowContentsHooks.Add(hook);
        }

        /// <summary>
        /// Register something that should happen after the <see cref="PlaywrightWindow"/> has finished drawing the window contents.
        /// </summary>
        /// <param name="hook">Your function to register.</param>
        public static void RegisterPlaywrightWindowPostWindowContents(Action<PlaywrightWindow, PlaywrightStructure, Rect> hook)
        {
            PlaywrightWindowPostWindowContentsHooks.Add(hook);
        }

        internal static void CallPlaywrightWindowPreWindowContents(PlaywrightWindow window, PlaywrightStructure playwright, Rect inRect)
        {
            foreach (var hook in PlaywrightWindowPreWindowContentsHooks)
            {
                hook(window, playwright, inRect);
            }
        }

        internal static void CallPlaywrightWindowPostWindowContents(PlaywrightWindow window, PlaywrightStructure playwright, Rect inRect)
        {
            foreach (var hook in PlaywrightWindowPostWindowContentsHooks)
            {
                hook(window, playwright, inRect);
            }
        }

        /// <summary>
        /// Register a function to be executed when the default Playwright structure is created.
        /// Used for changing or adding extra components to the default Playwright structure, like factions or win conditions.
        /// This is called when the player opens the Playwright designer, and when they change their origin.
        /// </summary>
        /// <param name="hook">The function to be called.</param>
        public static void RegisterPlaywrightDefaultStructure(Action<PlaywrightStructure> hook)
        {
            PlaywrightDefaultStructureHooks.Add(hook);
        }

        internal static void CallPlaywrightDefaultStructure(PlaywrightStructure structure)
        {
            foreach (var hook in PlaywrightDefaultStructureHooks)
            {
                hook(structure);
            }
        }

        /// <summary>
        /// Register a function to be executed when the origin is changed.
        /// Primarily used to remove default components for certain origins (such as disabling the ship win condition), or adding new defaults.
        /// If you do this, ensure you check that the new Origin's ID matches your component's ID's, as this hook is fired any time an origin is changed.
        /// </summary>
        /// <param name="hook">The function to be called. Takes the changed playwrightstructure and the new origin as parameters.</param>
        public static void RegisterPlaywrightOriginChanged(Action<PlaywrightStructure, OriginComponent> hook)
        {
            PlaywrightChangeOriginHooks.Add(hook);
        }

        internal static void CallPlaywrightOriginChanged(PlaywrightStructure structure, OriginComponent newOrigin)
        {
            foreach (var hook in PlaywrightChangeOriginHooks)
            {
                hook(structure, newOrigin);
            }
        }
    }
}
