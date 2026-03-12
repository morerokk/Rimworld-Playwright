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
    /// Holds the structure for the player's chosen screenplay.
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
        /// Factions that the player starts allied with.
        /// For example, starting out as being allied with the Empire.
        /// </summary>
        public List<FactionComponent> Allies = new List<FactionComponent>();
        /// <summary>
        /// Factions that the player starts as enemies with.
        /// For example, starting out as enemies with Insectoids and Mechanoids.
        /// Removing these would disable said factions and their incidents in the scenario editor.
        /// Adding a faction would instead make that faction start as enemies.
        /// </summary>
        public List<FactionComponent> Enemies = new List<FactionComponent>();
        /// <summary>
        /// Other factions that the player starts out as neutral with.
        /// For example, starting out as enemies with Insectoids and Mechanoids.
        /// Removing these would disable said factions and their incidents in the scenario editor.
        /// Adding a faction would instead make that faction start as enemies.
        /// </summary>
        public List<FactionComponent> OtherFactions = new List<FactionComponent>();

        public static PlaywrightStructure CreateDefault()
        {
            return new PlaywrightStructure()
            {
                Origin = new CrashlandedOrigin(),
                Boons = new List<BoonComponent>(),
                Allies = new List<FactionComponent>()
                {
                    new UnspecifiedFaction()
                },
                Enemies = new List<FactionComponent>()
                {
                    new InsectoidHiveFaction(),
                    new MechanoidHiveFaction(),
                    new UnspecifiedFaction()
                },
                OtherFactions = new List<FactionComponent>()
                {
                    new UnspecifiedFaction()
                }
            };
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Origin, nameof(Origin));

            Scribe_Collections.Look(ref Boons, nameof(Boons), LookMode.Deep);
            Scribe_Collections.Look(ref Allies, nameof(Allies), LookMode.Deep);
            Scribe_Collections.Look(ref Enemies, nameof(Enemies), LookMode.Deep);
            Scribe_Collections.Look(ref OtherFactions, nameof(OtherFactions), LookMode.Deep);
        }
    }
}
