using Rokk.Playwright.Addons;
using Rokk.Playwright.Components;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.SpecialConditions;
using Rokk.Playwright.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Composer
{
    /// <summary>
    /// Structure that holds all of the player's choices.
    /// Can be saved/loaded by Scribe.
    /// </summary>
    public class PlaywrightStructure : IExposable
    {
        /// <summary>
        /// The Origin of the player, which defines the starting point.
        /// During compilation of the scenario, this cannot be null.
        /// May be null temporarily in other cases, such as when loading a playwright with missing content.
        /// </summary>
        public OriginComponent Origin;

        /// <summary>
        /// Extra bonuses (if any) that the player starts with.
        /// For example, starting with a pre-placed Odyssey shuttle, extra goodies, or a royal title.
        /// </summary>
        public List<BoonComponent> Boons = new List<BoonComponent>();

        /// <summary>
        /// Allied factions in the world.
        /// </summary>
        public List<FactionComponent> AllyFactions = new List<FactionComponent>();

        /// <summary>
        /// Enemy factions in the world.
        /// </summary>
        public List<FactionComponent> EnemyFactions = new List<FactionComponent>();

        /// <summary>
        /// Neutral factions in the world.
        /// </summary>
        public List<FactionComponent> NeutralFactions = new List<FactionComponent>();

        /// <summary>
        /// The player's chosen win conditions (ship, archonexus, colonist count, etc).
        /// </summary>
        public List<WinConditionComponent> WinConditions = new List<WinConditionComponent>();

        /// <summary>
        /// The player's chosen special conditions (planetkiller, no recruitment, etc)
        /// </summary>
        public List<SpecialConditionComponent> SpecialConditions = new List<SpecialConditionComponent>();

        /// <summary>
        /// The name of the Playwright, which will end up being the scenario name as well.
        /// Example: "Lone Crashlander"
        /// </summary>
        public string Name;
        /// <summary>
        /// Short description, 2 short sentences max. Will be shown under the scenario name.
        /// Example: "Three crashlanded survivors. The classic Rimworld experience."
        /// </summary>
        public string DescriptionShort;
        /// <summary>
        /// Longer description that will be shown on the scenario, above the summary.
        /// Example: "You awaken to the sound of(...)"
        /// </summary>
        public string Description;
        /// <summary>
        /// Description that's shown on initial new game start.
        /// This is usually the same as Description, minus the "Note: this is a difficult scenario(...)" etc.
        /// </summary>
        public string GameStartDialogText;

        public static PlaywrightStructure CreateDefault()
        {
            var structure = new PlaywrightStructure()
            {
                Origin = new CrashlandedOrigin(),
                Boons = new List<BoonComponent>(),
                AllyFactions = new List<FactionComponent>()
                {
                    new AllOtherFactions()
                },
                EnemyFactions = new List<FactionComponent>()
                {
                    new InsectoidHiveFaction(),
                    new MechanoidHiveFaction(),
                    new AllOtherFactions()
                },
                NeutralFactions = new List<FactionComponent>()
                {
                    new AllOtherFactions()
                },
                WinConditions = new List<WinConditionComponent>()
                {
                    new ShipWinCondition()
                },
                SpecialConditions = new List<SpecialConditionComponent>()
            };

            var royalAscentWinCondition = new RoyalAscentWinCondition();
            if (royalAscentWinCondition.IsAvailable)
            {
                structure.WinConditions.Add(royalAscentWinCondition);
            }

            var archonexusWinCondition = new ArchonexusWinCondition();
            if (archonexusWinCondition.IsAvailable)
            {
                structure.WinConditions.Add(archonexusWinCondition);
            }

            // NOTE: I have opted to not include the Anomaly monolith in the selectable win conditions at all.
            // Anomaly is a bit of an oddball. In my opinion, if you're not going to go for the monolith,
            // you should just turn on Ambient Horror (or turn off the Anomaly stuff entirely).
            // If the player wants it as a win condition, they can just go for it anyway.
            // Either way, I think it's out of scope for this mod. We don't need to enable/disable anything for it.

            // TODO: What about Odyssey?

            HookRegistration.CallPlaywrightDefaultStructure(structure);

            structure.Name = structure.Origin.NameTranslated;
            structure.DescriptionShort = structure.Origin.DescriptionShortTranslated;
            structure.Description = structure.Origin.DescriptionTranslated;

            return structure;
        }

        /// <summary>
        /// Get a list of unavailable components, useful for validation after loading a saved Playwright.
        /// </summary>
        public List<PlaywrightComponent> GetUnavailableComponents()
        {
            List<PlaywrightComponent> unavailableComponents = new List<PlaywrightComponent>();
            if (!Origin.IsAvailable)
            {
                unavailableComponents.Add(Origin);
            }

            unavailableComponents.AddRange(Boons.Where(b => !b.IsAvailable));
            unavailableComponents.AddRange(AllyFactions.Where(f => !f.IsAvailable));
            unavailableComponents.AddRange(EnemyFactions.Where(f => !f.IsAvailable));
            unavailableComponents.AddRange(NeutralFactions.Where(f => !f.IsAvailable));
            unavailableComponents.AddRange(WinConditions.Where(w => !w.IsAvailable));
            unavailableComponents.AddRange(SpecialConditions.Where(s => !s.IsAvailable));

            return unavailableComponents;
        }

        /// <summary>
        /// Clear out any currently unavailable components.
        /// </summary>
        public void ClearUnavailableComponents()
        {
            if (!Origin.IsAvailable)
            {
                Origin = null;
            }

            Boons.RemoveAll(b => !b.IsAvailable);
            AllyFactions.RemoveAll(f => !f.IsAvailable);
            EnemyFactions.RemoveAll(f => !f.IsAvailable);
            NeutralFactions.RemoveAll(f => !f.IsAvailable);
            WinConditions.RemoveAll(w => !w.IsAvailable);
            SpecialConditions.RemoveAll(s => !s.IsAvailable);
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Origin, nameof(Origin));
            Scribe_Collections.Look(ref Boons, nameof(Boons), LookMode.Deep);
            Scribe_Collections.Look(ref AllyFactions, nameof(AllyFactions), LookMode.Deep);
            Scribe_Collections.Look(ref EnemyFactions, nameof(EnemyFactions), LookMode.Deep);
            Scribe_Collections.Look(ref NeutralFactions, nameof(NeutralFactions), LookMode.Deep);
            Scribe_Collections.Look(ref WinConditions, nameof(WinConditions), LookMode.Deep);
            Scribe_Collections.Look(ref SpecialConditions, nameof(SpecialConditions), LookMode.Deep);
            Scribe_Values.Look(ref Name, nameof(Name));
            Scribe_Values.Look(ref DescriptionShort, nameof(DescriptionShort));
            Scribe_Values.Look(ref Description, nameof(Description));
            Scribe_Values.Look(ref GameStartDialogText, nameof(GameStartDialogText));
        }
    }
}
