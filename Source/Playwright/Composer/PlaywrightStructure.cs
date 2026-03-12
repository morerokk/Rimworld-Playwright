using Rokk.Playwright.Components;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Composer
{
    /// <summary>
    /// Holds the structure for the player's chosen screenplay.
    /// </summary>
    public class PlaywrightStructure
    {
        /// <summary>
        /// The Origin of the player, as in "why are they here"? The premise.
        /// Only one such premise can exist.
        /// </summary>
        public OriginComponent Origin { get; set; }

        /// <summary>
        /// Extra bonuses (if any) that the player starts with.
        /// For example, starting with a pre-placed Odyssey shuttle or some extra goodies.
        /// </summary>
        public List<BoonComponent> Boons { get; set; } = new List<BoonComponent>();
    }
}
