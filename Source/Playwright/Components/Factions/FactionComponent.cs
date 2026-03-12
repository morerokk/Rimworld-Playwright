using RimWorld;
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
        /// This faction's def. Can be null if this doesn't apply, but this should be rare.
        /// </summary>
        public virtual FactionDef FactionDef => null;

        /// <summary>
        /// What dispositions this faction is allowed to have.
        /// </summary>
        public virtual HashSet<FactionDisposition> AllowedDispositions => new HashSet<FactionDisposition>()
        {
            FactionDisposition.Neutral,
            FactionDisposition.InitiallyHostile,
            FactionDisposition.AlwaysHostile,
            FactionDisposition.InitiallyAllied,
            FactionDisposition.AlwaysAllied
        };

        /// <summary>
        /// What natural or forced goodwill the faction will have.
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
