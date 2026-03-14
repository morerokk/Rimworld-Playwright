using Rokk.Playwright.Components;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Composer
{
    /// <summary>
    /// Holds all of the player's choices.
    /// </summary>
    public class PlaywrightStructure : IExposable
    {
        /// <summary>
        /// The Origin of the player, as in "why are they here"? The premise.
        /// Only one such premise can exist.
        /// </summary>
        public OriginComponent Origin;

        /// <summary>
        /// Extra bonuses (if any) that the player starts with.
        /// For example, starting with a pre-placed Odyssey shuttle or some extra goodies.
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

        public static PlaywrightStructure CreateDefault()
        {
            return new PlaywrightStructure()
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
                }
            };
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
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Origin, nameof(Origin));

            Scribe_Collections.Look(ref Boons, nameof(Boons), LookMode.Deep);
            Scribe_Collections.Look(ref AllyFactions, nameof(AllyFactions), LookMode.Deep);
        }
    }
}
