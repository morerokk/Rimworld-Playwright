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
        /// This faction's def. Can be null if this doesn't apply, but if your component is for a specific faction, this should be rare.
        /// </summary>
        public virtual FactionDef FactionDef => null;

        /// <summary>
        /// What dispositions this faction is allowed to have during selection.
        /// This should usually be "anything", but insectoids and mechanoids don't really work if they're not always the default of hostile.
        /// </summary>
        public virtual HashSet<FactionDisposition> AllowedDispositions => new HashSet<FactionDisposition>()
        {
            FactionDisposition.Default,
            FactionDisposition.Neutral,
            FactionDisposition.InitiallyHostile,
            FactionDisposition.AlwaysHostile,
            FactionDisposition.InitiallyAllied,
            FactionDisposition.AlwaysAllied,
        };

        /// <summary>
        /// What natural or forced goodwill the faction will have.
        /// </summary>
        public enum FactionDisposition
        {
            Default = 0,
            Neutral = 100,
            InitiallyHostile = 99,
            AlwaysHostile = 98,
            InitiallyAllied = 101,
            AlwaysAllied = 102
        }
    }
}
