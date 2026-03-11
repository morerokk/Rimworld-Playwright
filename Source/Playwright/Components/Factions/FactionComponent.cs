using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Components.Factions
{
    public abstract class FactionComponent : PlaywrightComponent
    {
        /// <summary>
        /// What natural or forced goodwill the faction should have.
        /// </summary>
        public enum FactionDisposition
        {
            Neutral = 0,
            InitiallyHostile = -1,
            AlwaysHostile = -2,
            InitiallyAllied = 1,
            AlwaysAllied = 2
        }
    }
}
